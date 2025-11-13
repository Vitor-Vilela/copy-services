// Carrega as variáveis de ambiente do arquivo .env
require("dotenv").config();

const express = require("express");
const cors = require("cors");
const { connectDB } = require("./config/database");
const authRoutes = require("./routes/auth.routes");

// --- Importações do Swagger ---
const swaggerUi = require("swagger-ui-express");
const swaggerJsDoc = require("swagger-jsdoc");

// Conecta ao banco de dados
connectDB();

const app = express();

app.use(cors());
app.use(express.json());

// --- Configuração do Swagger ---
const swaggerOptions = {
  definition: {
    openapi: "3.0.0",
    info: {
      title: "Auth Service API (Node.js + MySQL)",
      version: "1.0.0",
      description: "Microsserviço de Autenticação para o Projeto N2",
    },
    servers: [
      {
        url: "http://localhost:3000",
      },
    ],
    components: {
      securitySchemes: {
        bearerAuth: {
          type: "http",
          scheme: "bearer",
          bearerFormat: "JWT",
        },
      },
    },
  },
  // Indica onde o Swagger deve procurar por anotações (nos arquivos de rotas)
  apis: ["./routes/*.js"], 
};

const swaggerDocs = swaggerJsDoc(swaggerOptions);
// Rota para acessar a documentação: http://localhost:3000/api-docs
app.use("/api-docs", swaggerUi.serve, swaggerUi.setup(swaggerDocs));

// Rota de saúde
app.get("/", (req, res) => {
  res.send("Auth Microservice está rodando! Acesse /api-docs para documentação.");
});

// Rotas da API
app.use("/api/auth", authRoutes);

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`[AuthService-MySQL] Rodando na porta ${PORT}`);
  console.log(`[AuthService-MySQL] Swagger disponível em http://localhost:${PORT}/api-docs`);
});