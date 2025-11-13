# Microsserviço de Animal

## Equipe
- **Nome do Projeto:** [CarrocinhaDoBem]
- **Integrantes:**
    - [Vinicius da costa pereira] – @[Vinicosper]
    - [Vinicius Viana Gomes] – @[vini-vg]
    - [Vitor vilela] – @[vitorvilela.pr]
    - [Marcos maximo] - @[marcosvmaximo]

## Contexto Comercial

Este microsserviço é um componente da plataforma "Carrocinha do Bem" e tem como responsabilidade central o gerenciamento do catálogo de animais.  
Ele permite o cadastro, listagem e recuperação de animais disponíveis para adoção ou apadrinhamento. Além disso, armazena informações detalhadas como espécie, raça, porte, datas de resgate/nascimento e imagens associadas.

Sua separação garante que a gestão de dados dos animais seja independente de outros domínios da aplicação (como doações e usuários), o que facilita manutenções, evolução do modelo de dados e escalabilidade.

## Stack Tecnológica

- **Linguagem de Programação:** C#
- **Framework:** .NET 8 (ASP.NET Core Web API)
- **Banco de Dados:** MySQL 8 (com Entity Framework Core e Pomelo)
- **Ferramentas de Integração:**
  - Consul (Service Discovery)
  - [Nome do Gateway que estão usando, ex: Ocelot] (API Gateway)

## Instruções de Execução e Teste

### 1. Preparação do Ambiente

Clone o repositório e navegue até a pasta deste microsserviço:

```bash
git clone [URL-DO-SEU-FORK]
cd Services/animal-service
```

### 2. Configuração da Connection String

No arquivo `appsettings.json`, ajuste a conexão com seu banco MySQL local:

```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3306;database=animal_catalog;user=root;password=SUASENHA;SslMode=none;"
}
```

### 3. Execução das Migrações

Crie o banco de dados e aplique as migrations do Entity Framework:

```bash
dotnet ef database update
```

### 4. Execução do Serviço

Inicie o microsserviço com:

```bash
dotnet run
```

Por padrão, o servidor estará disponível em:  
👉 `http://localhost:5089/api/animals`

### 5. Teste com Postman

1. **Importe a Collection** fornecida (`animal.postman.json`).
2. Certifique-se de configurar a variável `baseUrl` como `http://localhost:5089/api`.
3. Teste os seguintes endpoints:
   - `GET /animals` – lista todos os animais.
   - `GET /animals/{id}` – retorna detalhes de um animal específico.
   - `POST /animals` – cria um animal.

---

📌 **Resultado Esperado:**  
Você deve conseguir cadastrar um animal via Postman, listar na API e visualizar as informações salvas no banco `animal_catalog`.
