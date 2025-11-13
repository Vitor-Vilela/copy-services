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

app.get("/api-docs/swagger.json", (req, res) => {
  res.setHeader("Content-Type", "application/json");
  res.send(swaggerDocs);
});

// Rota para acessar a documentação
app.use("/api-docs", swaggerUi.serve);
app.get("/api-docs", swaggerUi.setup(swaggerDocs));

// Rota de saúde
app.get("/", (req, res) => {
  res.send(
    "Auth Microservice está rodando! Acesse /api-docs para documentação."
  );
});

// Rotas da API
app.use("/api/auth", authRoutes);

const Consul = require("consul");
const { v4: uuidv4 } = require("uuid");

const consul = new Consul({
  host: process.env.CONSUL_HOST || "localhost",
  port: Number(process.env.CONSUL_PORT) || 8500,
});

const SERVICE_NAME = process.env.SERVICE_NAME || "auth-service";
const SERVICE_PORT = Number(process.env.PORT) || 3000;
const SERVICE_ADDRESS = process.env.ADDRESS || "auth-service";
const SERVICE_ID = `${SERVICE_NAME}-${uuidv4()}`;

const registerService = () => {
  const check = {
    http: `http://${SERVICE_ADDRESS}:${SERVICE_PORT}/`,
    interval: "10s",
    timeout: "5s",
    deregistercriticalserviceafter: "30s",
  };

  consul.agent.service.register(
    {
      name: SERVICE_NAME,
      id: SERVICE_ID,
      address: SERVICE_ADDRESS,
      port: SERVICE_PORT,
      check,
    },
    (err) => {
      if (err) console.error("Erro ao registrar no Consul:", err);
      else console.log("Auth Service registrado no Consul com sucesso.");
    }
  );
};

const PORT = SERVICE_PORT;
app.listen(PORT, () => {
  registerService();
  console.log(`[AuthService-MySQL] Rodando na porta ${PORT}`);
  console.log(
    `[AuthService-MySQL] Swagger disponível em http://localhost:${PORT}/api-docs`
  );
});
