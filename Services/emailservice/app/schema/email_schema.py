from pydantic import BaseModel, EmailStr

class EmailSchema(BaseModel):
    to: EmailStr
    subject: str
    body: str

    class Config:
        json_schema_extra = {
            "example": {
                "to": "destinatario@exemplo.com",
                "subject": "Assunto do E-mail",
                "body": "<h1>Olá Mundo!</h1><p>Este é um e-mail de teste.</p>"
            }
        }