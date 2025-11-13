const jwt = require("jsonwebtoken");
const JWT_SECRET = process.env.JWT_SECRET;

exports.verifyToken = (req, res, next) => {
  let token = req.headers["authorization"]; // Padrão "Bearer <token>"

  if (!token) {
    return res.status(403).send({ message: "Nenhum token fornecido." });
  }

  try {
    // Remove o "Bearer " do início do token
    if (token.startsWith("Bearer ")) {
      token = token.slice(7, token.length);
    }

    // Verifica se o token é válido e decodifica o payload
    const decoded = jwt.verify(token, JWT_SECRET);
    
    // Adiciona o payload (id, email, role) no objeto 'req'
    req.user = decoded;
    next(); // Passa para a próxima função (o controller)

  } catch (err) {
    return res.status(401).send({ message: "Não autorizado! Token inválido ou expirado." });
  }
};