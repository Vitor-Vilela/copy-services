# 1. Usar a imagem oficial do Node.js (Alpine é uma versão leve)
FROM node:18-alpine

# 2. Definir o diretório de trabalho
WORKDIR /app

# 3. Copiar o package.json e package-lock.json (se existir)
COPY package*.json ./

# 4. Instalar as dependências de produção
# Usamos 'npm ci' para uma instalação limpa e rápida
RUN npm ci --only=production

# 5. Copiar o restante do código da aplicação
COPY . .

# 6. Expor a porta que o Express vai usar (definida no .env)
EXPOSE 3000

# 7. Comando para iniciar a aplicação
CMD ["node", "server.js"]