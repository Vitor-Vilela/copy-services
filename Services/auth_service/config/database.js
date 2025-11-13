const { Sequelize } = require("sequelize");

const sequelize = new Sequelize(
  process.env.DB_NAME || "auth_db",
  process.env.DB_USER || "root",
  process.env.DB_PASS || "root",
  {
    host: process.env.DB_HOST || "mysql-db",
    dialect: "mysql",
    logging: false,
  }
);

const connectDB = async () => {
  try {
    await sequelize.authenticate();
    console.log("[AuthService] Conexão com MySQL estabelecida com sucesso.");

    await sequelize.sync();
    console.log("[AuthService] Tabelas sincronizadas.");
  } catch (error) {
    console.error("[AuthService] Erro ao conectar no MySQL:", error.message);
  }
};

module.exports = { sequelize, connectDB };
