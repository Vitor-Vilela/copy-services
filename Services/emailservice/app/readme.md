# Microsserviço de E-mail

## Equipe

- **Nome do Projeto:** [CarrocinhaDoBem]
- **Integrantes:**
  - [Vinicius da costa pereira] – @[Vinicosper]
  - [Vinicius Viana Gomes] – @[vini-vg]
  - [Vitor Vilela] – @[vitorvilela.pr]
  - [Marcos Maximo] - @[marcosvmaximo]

## Contexto Comercial

Este microsserviço é um componente da plataforma "Carrocinha do Bem" e possui a responsabilidade única de gerenciar o envio de e-mails transacionais. Ele centraliza a comunicação com os usuários, disparando notificações essenciais como confirmação de doações, status de processos de adoção e comunicados sobre apadrinhamento de animais. Sua separação garante que a lógica de comunicação seja desacoplada do serviço principal, permitindo manutenções e escalabilidade independentes.

## Stack Tecnológica

- **Linguagem de Programação:** Python
- **Framework:** FastAPI
- **Ferramentas de Integração:**
  - Consul (Service Discovery)
  - [Nome do Gateway que estão usando, ex: Ocelot] (API Gateway)

## Instruções de Execução e Teste

### 1. Preparação do Ambiente

Clone o repositório e navegue até a pasta deste microsserviço:

```bash
git clone [URL-DO-SEU-FORK]
cd Api/email-service
```

Crie um ambiente virtual para isolar as dependências do projeto:

```bash
python -m venv venv
```

### 2. Ativação e Instalação

Ative o ambiente virtual. No terminal PowerShell, execute:

```powershell
# Pode ser necessário permitir a execução de scripts para a sessão atual
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process

# Ative o venv
.\venv\Scripts\activate
```

Com o ambiente ativo, instale todas as bibliotecas necessárias:

```bash
pip install -r requirements.txt
```

### 3. Configuração

Crie um arquivo `.env` a partir do exemplo (se houver um `.env.example`) ou crie um novo. Preencha com suas credenciais de e-mail e as configurações do Consul.

### 4. Execução

Para iniciar o microsserviço, execute o seguinte comando:

```bash
uvicorn app.main:app --reload
```

O servidor estará disponível em `http://127.0.0.1:8000`.

### 5. Teste com Postman

1.  **Abra o Postman** e crie uma nova requisição.
2.  **Método:** `POST`
3.  **URL:** `http://127.0.0.1:8000/send-email`
4.  **Aba "Body"**:
    - Selecione a opção **"raw"**.
    - No menu dropdown, mude o tipo para **"JSON"**.
5.  **Corpo da Requisição (JSON)**: Cole o seguinte, alterando para seu e-mail de teste:
    ```json
    {
      "to": "seu-email-de-teste@exemplo.com",
      "subject": "Teste com Postman",
      "body": "<h1>API Funcionando!</h1><p>Requisição enviada com sucesso pelo Postman.</p>"
    }
    ```
6.  Clique em **"Send"**.

**Resultado Esperado:** Você deve receber uma resposta com status `200 OK` no Postman e o e-mail na sua caixa de entrada.
