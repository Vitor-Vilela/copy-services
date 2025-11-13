const User = require("../models/user.model");
const jwt = require("jsonwebtoken");

// Pega a chave secreta do .env
const JWT_SECRET = process.env.JWT_SECRET;

// Função interna para gerar o token
const generateToken = (id, email, role) => {
  // Assina o token com o ID, email e papel do usuário
  return jwt.sign({ id, email, role }, JWT_SECRET, {
    expiresIn: "8h", // Token expira em 8 horas
  });
};

/**
 * @route   POST /api/auth/register
 * @desc    Registra um novo usuário
 */
exports.register = async (req, res) => {
  const { name, email, password } = req.body;

  try {
    // 1. Verifica se o usuário já existe
    // SINTAXE SEQUELIZE: User.findOne({ where: { email: email } })
    let user = await User.findOne({ where: { email } });
    if (user) {
      return res.status(400).json({ message: "Usuário com este e-mail já existe." });
    }

    // 2. Cria o novo usuário
    // SINTAXE SEQUELIZE: User.create(...)
    // O hook no modelo (user.model.js) irá criptografar a senha automaticamente
    user = await User.create({
      name,
      email,
      password,
      // 'role' será "User" por padrão (definido no modelo)
    });

    // 3. Retorna o token para o usuário já sair logado
    const token = generateToken(user.id, user.email, user.role);
    res.status(201).json({ token });

  } catch (err) {
    console.error(err.message);
    res.status(500).send("Erro de servidor");
  }
};

/**
 * @route   POST /api/auth/login
 * @desc    Autentica o usuário e retorna um token
 */
exports.login = async (req, res) => {
  const { email, password } = req.body;

  try {
    // 1. Encontra o usuário pelo e-mail
    // SINTAXE SEQUELIZE: User.findOne({ where: { email } })
    const user = await User.findOne({ where: { email } });
    if (!user) {
      return res.status(401).json({ message: "Credenciais inválidas" });
    }

    // 2. Compara a senha enviada com a senha criptografada no banco
    // O método .comparePassword() foi definido no modelo (user.model.js)
    const isMatch = await user.comparePassword(password);
    if (!isMatch) {
      return res.status(401).json({ message: "Credenciais inválidas" });
    }

    // 3. Se deu tudo certo, gera e retorna o token
    const token = generateToken(user.id, user.email, user.role);
    res.status(200).json({ token });

  } catch (err) {
    console.error(err.message);
    res.status(500).send("Erro de servidor");
  }
};

/**
 * @route   GET /api/auth/validate
 * @desc    Valida um token (rota protegida)
 */
exports.validateToken = async (req, res) => {
  // Se a requisição chegou aqui, o middleware (auth.middleware.js)
  // já validou o token e colocou os dados do usuário em 'req.user'.
  // Apenas retornamos esses dados para quem chamou.
  res.status(200).json(req.user);
};