# 1. Usar a imagem oficial do Python
FROM python:3.10-slim

# 2. Definir o diretório de trabalho
WORKDIR /app

# 3. Copiar o arquivo de dependências
COPY requirements.txt .

# 4. Instalar as dependências
RUN pip install --no-cache-dir -r requirements.txt

# 5. Copiar o restante do código da aplicação
COPY . .

# 6. Expor a porta que o uvicorn vai usar
EXPOSE 8000

# 7. Comando para iniciar a aplicação
#    Uvicorn vai rodar o objeto 'app' de dentro do arquivo 'main.py'
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]