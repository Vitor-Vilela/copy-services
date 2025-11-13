const { DataTypes } = require("sequelize");
const { sequelize } = require("../config/database"); // Importa a instância do Sequelize
const bcrypt = require("bcryptjs");

// Define o modelo 'User' (equivalente à sua classe User.cs)
const User = sequelize.define(
  "User",
  {
    // O Sequelize cria um 'id' (INT, primary key, auto-increment) por padrão
    name: {
      type: DataTypes.STRING,
      allowNull: false,
    },
    email: {
      type: DataTypes.STRING,
      allowNull: false,
      unique: true, // Garante e-mails únicos
      validate: {
        isEmail: true, // Validação de formato de e-mail
      },
    },
    password: {
      type: DataTypes.STRING,
      allowNull: false,
    },
    role: {
      type: DataTypes.ENUM("User", "Admin"), // Define os papéis possíveis
      allowNull: false,
      defaultValue: "User",
    },
  },
  {
    // Opções do modelo
    tableName: "users",
    timestamps: true, // Adiciona colunas 'createdAt' e 'updatedAt' automaticamente

    // Hooks (Ganchos) para criptografar a senha antes de salvar
    // (Substitui o seu PasswordService.cs)
    hooks: {
      // Usado ao criar um novo usuário
      beforeCreate: async (user) => {
        if (user.password) {
          const salt = await bcrypt.genSalt(10);
          user.password = await bcrypt.hash(user.password, salt);
        }
      },
      // Usado ao atualizar um usuário (ex: mudança de senha)
      beforeUpdate: async (user) => {
        if (user.changed("password")) {
          const salt = await bcrypt.genSalt(10);
          user.password = await bcrypt.hash(user.password, salt);
        }
      },
    },
  }
);

// Método para comparar a senha (usado no login)
User.prototype.comparePassword = async function (enteredPassword) {
  return await bcrypt.compare(enteredPassword, this.password);
};

module.exports = User;