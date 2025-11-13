from pydantic import BaseModel
from decimal import Decimal

# --- Modelos de Request (Entrada) ---

# Equivalente ao CreateChargeRequestDto
class CreateChargeRequest(BaseModel):
    amount: Decimal
    description: str
    donation_id: int
    success_url: str
    cancel_url: str

# Equivalente ao PaymentWebhookPayload
class PaymentWebhookPayload(BaseModel):
    event_type: str = "payment.succeeded"
    donation_id: int
    charge_id: str
    amount_paid: Decimal

# --- Modelos de Response (Saída) ---

class CreateChargeResponse(BaseModel):
    charge_id: str
    checkout_url: str

    class Config:
        orm_mode = True