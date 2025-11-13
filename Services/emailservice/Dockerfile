# 1. Usar imagem leve do Python
FROM python:3.10-slim

# 2. Definir diretório de trabalho no container
WORKDIR /app

# 3. Copiar dependências e instalar
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# 4. Copiar todo o código fonte (pasta app, etc)
COPY . .

# 5. Expor a porta padrão (o código usa 8000 por padrão)
EXPOSE 8000

# 6. Comando para subir a aplicação
# Nota: como sua pasta se chama 'app', o módulo é 'app.main:app'
CMD ["uvicorn", "app.main:app", "--host", "0.0.0.0", "--port", "8000"]