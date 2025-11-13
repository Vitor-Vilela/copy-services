// Importa os pacotes necessários para o Entity Framework Core e para o seu DataContext.

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using AnimalCatalog.API.Data.Context; // Certifique-se que o namespace aqui está correto!
using AnimalCatalog.API.Infrastructure.Consul;

var builder = WebApplication.CreateBuilder(args);

// --- Adicionar serviços ao contêiner ---

// 1. Adiciona os serviços necessários para a API, incluindo controllers.
builder.Services.AddControllers();

// 2. Configura a documentação da API (Swagger/OpenAPI).
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// 3. Configura a conexão com o banco de dados MySQL.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// 4. Registra o AutoMapper para fazer as conversões de objetos.
// Ele vai procurar automaticamente por classes que herdam de 'Profile', como a sua 'AnimalProfile'.
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddHealthChecks();

var app = builder.Build();

// --- Configuração do pipeline de requisições HTTP ---

// Mostra a UI do Swagger apenas em ambiente de desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();

// Habilita o roteamento para os seus Controllers (como o AnimalController).
app.MapControllers();

// endpoint de health
app.MapHealthChecks("/health");

// registro no consul
app.RegisterWithConsul(app.Configuration, app.Lifetime);

app.Run();