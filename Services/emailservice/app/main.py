import os
import uuid
import consul
import socket
from fastapi import FastAPI, HTTPException, status
from app.schema.email_schema import EmailSchema
from app.services.email_service import send_email as send_email_service

# --- Configurações do Serviço ---
SERVICE_NAME = os.getenv("SERVICE_NAME", "email-service")
SERVICE_PORT = int(os.getenv("SERVICE_PORT", 8000))
CONSUL_HOST = os.getenv("CONSUL_HOST", "localhost")
CONSUL_PORT = int(os.getenv("CONSUL_PORT", 8500))
SERVICE_ID = f"{SERVICE_NAME}-{uuid.uuid4()}"

app = FastAPI(
    title="Microsserviço de E-mail",
    description="API para envio de e-mails transacionais do projeto Carrocinha do Bem.",
    version="1.0.0"
)

consul_client = consul.Consul(host=CONSUL_HOST, port=CONSUL_PORT)

def get_service_ip():
    try:
        return socket.gethostbyname(socket.gethostname())
    except socket.gaierror:
        return "127.0.0.1"

def register_service():
    ip = get_service_ip()
    print(f"Registrando serviço {SERVICE_ID} em {ip}:{SERVICE_PORT} no Consul...")
    try:
        consul_client.agent.service.register(
            name=SERVICE_NAME,
            service_id=SERVICE_ID,
            address=ip,
            port=SERVICE_PORT,
            check={
                "http": f"http://{ip}:{SERVICE_PORT}/health",
                "interval": "10s",
                "timeout": "5s",
                "deregistercriticalserviceafter": "30s"
            }
        )
        print("Serviço registrado com sucesso no Consul.")
    except Exception as e:
        print(f"!!!!!! Erro ao registrar no Consul: {e}. O serviço continuará rodando localmente. !!!!!!")

def deregister_service():
    print(f"Desregistrando serviço {SERVICE_ID}...")
    try:
        consul_client.agent.service.deregister(service_id=SERVICE_ID)
        print("Serviço desregistrado do Consul.")
    except Exception as e:
        print(f"!!!!!! Erro ao desregistrar do Consul: {e}. !!!!!!")

@app.on_event("startup")
def on_startup():
    register_service()

@app.on_event("shutdown")
def on_shutdown():
    deregister_service()

@app.get("/health", 
         summary="Verifica a saúde do serviço (Health Check)", 
         status_code=status.HTTP_200_OK, 
         tags=["Operacional"])
def health_check():
    return {"status": "ok"}

@app.post("/send-email", 
            summary="Envia um e-mail",
            tags=["E-mails"])
def send_email_endpoint(email_data: EmailSchema):
    result = send_email_service(email_data.to, email_data.subject, email_data.body)
    if result["status"] == "error":
        raise HTTPException(status_code=500, detail=result["message"])
    return {"message": "E-mail enviado com sucesso!"}