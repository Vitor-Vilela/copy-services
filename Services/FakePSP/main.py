from fastapi import FastAPI, Depends, HTTPException, Request, Response
from fastapi.responses import HTMLResponse, RedirectResponse
from sqlalchemy.orm import Session
from decimal import Decimal
import qrcode
import httpx
import io
import base64

# Importações locais
import models
import schemas
import pix_utils
from database import engine, Base, get_db

# Cria as tabelas no banco de dados (se não existirem)
Base.metadata.create_all(bind=engine)

app = FastAPI(title="FakePSP API (Python)")

# Cliente HTTP assíncrono para disparar o webhook
# Adicionando verify=False para ignorar verificação SSL (comum em localhost)
http_client = httpx.AsyncClient(verify=False)


@app.post("/api/charges", response_model=schemas.CreateChargeResponse)
def create_charge(request_data: schemas.CreateChargeRequest, request: Request, db: Session = Depends(get_db)):
    """
    Cria uma nova cobrança PIX.
    Equivalente ao [HttpPost] CreateCharge
    """
    
    # Cria o novo objeto Charge no banco
    new_charge = models.Charge(
        amount=request_data.amount,
        description=request_data.description,
        donation_id=request_data.donation_id,
        success_url=request_data.success_url,
        cancel_url=request_data.cancel_url,
        status="PENDING"  # <<< ESTA É A CORREÇÃO
    )
    
    # Gera o payload PIX e salva no objeto
    new_charge.pix_payload = pix_utils.generate_pix_payload(new_charge.amount)
    
    db.add(new_charge)
    db.commit()
    db.refresh(new_charge)
    
    # Monta a URL de checkout
    # (request.url.scheme) pega http ou https
    # (request.url.netloc) pega localhost:port
    base_url = f"{request.url.scheme}://{request.url.netloc}"
    checkout_url = f"{base_url}/api/charges/checkout/{new_charge.id}"
    
    print(f"[FakePSP-Python] Cobrança PIX criada: {new_charge.id}. URL de Checkout: {checkout_url}")
    
    return schemas.CreateChargeResponse(
        charge_id=new_charge.id,
        checkout_url=checkout_url
    )


@app.get("/api/charges/checkout/{charge_id}", response_class=HTMLResponse)
def show_checkout_page(charge_id: str, request: Request, db: Session = Depends(get_db)):
    """
    Mostra a página de pagamento com QR Code.
    Equivalente ao [HttpGet("checkout/{chargeId}")]
    """
    charge = db.query(models.Charge).filter(models.Charge.id == charge_id).first()
    
    if not charge or charge.status != "PENDING":
        # Esta verificação agora vai funcionar
        return HTMLResponse(content="Página de pagamento inválida ou já processada.", status_code=404)

    # Gera a URL do formulário de confirmação
    base_url = f"{request.url.scheme}://{request.url.netloc}"
    confirm_url = f"{base_url}/api/charges/confirm/{charge_id}"

    # Gera QR Code em base64
    qr = qrcode.QRCode(version=1, box_size=10, border=4)
    qr.add_data(charge.pix_payload)
    qr.make(fit=True)
    img = qr.make_image(fill='black', back_color='white')
    
    buffer = io.BytesIO()
    img.save(buffer, format="PNG")
    qr_base64 = base64.b64encode(buffer.getvalue()).decode('utf-8')
    qr_data_uri = f"data:image/png;base64,{qr_base64}"

    # Retorna o mesmo HTML do C#
    html = f"""
        <!DOCTYPE html>
        <html>
        <head>
            <title>Checkout PIX - FakePSP (Python)</title>
            <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">
            <style>
                body {{ display: flex; align-items: center; justify-content: center; height: 100vh; background-color: #f0f2f5; }}
                .card {{ padding: 2.5rem; box-shadow: 0 6px 20px rgba(0,0,0,0.1); border: none; border-radius: 1rem; width: 100%; max-width: 450px; }}
                #qrcode img {{ margin: auto; padding: 10px; background: white; border-radius: 8px; margin-bottom: 1.5rem; }}
                .input-group-text {{ cursor: pointer; }}
            </style>
        </head>
        <body>
            <div class="card text-center">
                <h1 class="mb-2">Pague com PIX</h1>
                <p class="text-muted mb-4">Para doar <strong>R$ {charge.amount:.2f}</strong> para <strong>{pix_utils.MERCHANT_NAME}</strong></p>

                <div id="qrcode" class="d-flex justify-content-center">
                    <img src="{qr_data_uri}" alt="QR Code PIX" width="220" height="220">
                </div>

                <p class="fw-bold mt-2">PIX Copia e Cola</p>
                <div class="input-group mb-4">
                    <input type="text" id="pixCode" class="form-control" value="{charge.pix_payload}" readonly>
                    <span class="input-group-text" onclick="copyCode()">&#128203;</span>
                </div>

                <p class="text-muted small">Após pagar no app do seu banco, clique em 'Já Paguei!' abaixo.</p>

                <div class="d-grid gap-2 mt-2">
                    <form method="post" action="{confirm_url}">
                        <button type="submit" class="btn btn-success btn-lg w-100">Já Paguei!</button>
                    </form>
                    <a href="{charge.cancel_url}" class="btn btn-link text-danger">Cancelar Doação</a>
                </div>
            </div>
            <script>
                function copyCode() {{
                    const pixCodeInput = document.getElementById('pixCode');
                    pixCodeInput.select();
                    document.execCommand('copy');
                    alert('Código PIX copiado!');
                }}
            </script>
        </body>
        </html>"""

    return HTMLResponse(content=html)


@app.post("/api/charges/confirm/{charge_id}")
async def confirm_payment(charge_id: str, db: Session = Depends(get_db)):
    """
    Simula a confirmação do pagamento e dispara o Webhook.
    Equivalente ao [HttpPost("confirm/{chargeId}")]
    """
    charge = db.query(models.Charge).filter(models.Charge.id == charge_id).first()
    
    if not charge or charge.status != "PENDING":
        return HTMLResponse(content="Pagamento inválido ou já processado.", status_code=400)

    # Atualiza o status no banco de dados
    charge.status = "PAID"
    db.commit()
    print(f"[FakePSP-Python] Confirmação manual recebida para a cobrança: {charge.id}")
    
    # Prepara o payload do webhook
    webhook_payload = schemas.PaymentWebhookPayload(
        donation_id=charge.donation_id,
        charge_id=charge.id,
        amount_paid=charge.amount
    )
    
    # Envia o webhook para a API principal (webApi C#)
    try:
        print(f"[FakePSP-Python] Enviando webhook para {charge.webhook_url}")
        
        # Converte Decimal para float para serialização JSON
        payload_dict = webhook_payload.dict()
        payload_dict['amount_paid'] = float(webhook_payload.amount_paid)

        await http_client.post(charge.webhook_url, json=payload_dict)
        
    except httpx.RequestError as ex:
        # Se falhar, apenas loga. Em um projeto real, aqui teria um retry.
        print(f"[FakePSP-Python] ERRO ao enviar webhook: {ex}")

    # Redireciona o usuário para a página de sucesso
    return RedirectResponse(url=charge.success_url, status_code=303)