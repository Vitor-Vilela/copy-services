import uuid
from sqlalchemy import Column, String, Numeric, Integer
from database import Base

class Charge(Base):
    __tablename__ = "charges"

    # Colunas da tabela
    id = Column(String, primary_key=True, index=True, default=lambda: str(uuid.uuid4().hex))
    amount = Column(Numeric(10, 2), nullable=False)
    description = Column(String, nullable=True)
    donation_id = Column(Integer, index=True, nullable=False)
    status = Column(String, default="PENDING")
    success_url = Column(String, nullable=False)
    cancel_url = Column(String, nullable=False)
    webhook_url = Column(String, default="https://localhost:7001/api/donation/webhook")
    pix_payload = Column(String, nullable=True)