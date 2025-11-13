import smtplib
import os
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText
from dotenv import load_dotenv

load_dotenv()

def send_email(to: str, subject: str, body: str):
    """
    Conecta-se a um servidor SMTP e envia um e-mail.
    """
    try:
        msg = MIMEMultipart()
        msg['From'] = os.getenv("EMAIL_FROM")
        msg['To'] = to
        msg['Subject'] = subject
        msg.attach(MIMEText(body, 'html'))

        with smtplib.SMTP(os.getenv("EMAIL_HOST"), int(os.getenv("EMAIL_PORT"))) as server:
            server.starttls()
            server.login(os.getenv("EMAIL_USER"), os.getenv("EMAIL_PASSWORD"))
            server.send_message(msg)
        
        return {"status": "success"}
    except Exception as e:
        print(f"ERRO DE SERVIÇO DE E-MAIL: {e}")
        return {"status": "error", "message": str(e)}