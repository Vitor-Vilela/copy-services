from brcode import BRCode
from decimal import Decimal

# Constantes do seu C#
PIX_KEY = "seu.email@dominio.com.br"
MERCHANT_NAME = "ONG CARROCINHA DO BEM"
MERCHANT_CITY = "CURITIBA"

def generate_pix_payload(amount: Decimal, tx_id: str = "***") -> str:

    # Criando o payload com a biblioteca 'python-brcode'
    brcode = BRCode(
        key=PIX_KEY,
        name=MERCHANT_NAME,
        city=MERCHANT_CITY,
        amount=amount,  # Esta biblioteca aceita Decimal diretamente
        transaction_id=tx_id
    )

    # O nosso main.py só precisa da string do payload.
    return str(brcode)