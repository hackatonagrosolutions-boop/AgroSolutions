using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using SersorService.DTOs;
using SersorService.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar JwtSettings
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Registrar MongoDB
builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration.GetValue<string>("MongoDbSettings:ConnectionString")));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Registrar Repositories e Services
builder.Services.AddScoped<MensageriaService>();

#region [Swagger]
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HACKATON - AgroSolutions - SensorService",
        Description = "Plataforma de IoT e análise de dados para serviço de agricultura 4.0 de precisão.",
        Contact = new OpenApiContact()
        {
            Name = "AgroSolutions",
            Email = "hackatonagrosolutions@gmail.com"
        },
        License = new OpenApiLicense()
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        },
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT desta forma: Bearer seu_token_aqui"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath)) // Evita erro se o arquivo XML não for gerado
    {
        options.IncludeXmlComments(xmlPath);
    }
});
#endregion

#region [JWT]

//Autentica o token JWT para proteger os endpoints da API
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
        ValidAudience = jwtSettings.Audience,
        ValidIssuer = jwtSettings.Issuer,
        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true
    };
});
#endregion

var app = builder.Build();

// Configuração do Middleware do Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Agro Solutions v1");
    });
}

// app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

#region [Endpoints]
app.MapPost("/api/sensores/leitura", async (
    [FromBody] LeituraSensorCreateDto dto,
    IMongoClient mongoClient,
    MensageriaService rabbitService) =>
{
    var database = mongoClient.GetDatabase("AgroSolutions_Sensores");
    var collection = database.GetCollection<LeituraSensor>("Leituras");

    var leitura = new LeituraSensor
    {
        TalhaoId = dto.TalhaoId,
        UmidadeSolo = dto.UmidadeSolo,
        Temperatura = dto.Temperatura,
        Vento = dto.Vento,
        DataLeitura = dto.DataLeitura ?? DateTime.UtcNow
    };

    await collection.InsertOneAsync(leitura);

    if (leitura.UmidadeSolo < 30 || leitura.Vento > 50)
    {
        await rabbitService.PublicarAlertaAsync(new
        {
            Mensagem = "Condições críticas detectadas!",
            TalhaoId = leitura.TalhaoId,
            Umidade = leitura.UmidadeSolo,
            Vento = leitura.Vento,
            Data = leitura.DataLeitura
        });
    }

    return Results.Created($"/api/sensores/leitura/{leitura.Id}", leitura);
})
.RequireAuthorization();
#endregion

app.Run();