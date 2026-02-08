using AlertaService.Data;
using AlertaService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar JwtSettings
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Configurar DbContext
builder.Services.AddDbContext<AlertaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Registrar Repositories e Services
builder.Services.AddHostedService<AlertaWorker>();

#region [Swagger]
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AgroSolutions - AlertaService",
        Description = "Plataforma de IoT e análise de dados para serviço de agricultura 4.0 de precisão.",
        Contact = new OpenApiContact()
        {
            Name = "HACKATON",
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

//app.UseHttpsRedirection();
app.UseAuthorization();

#region [Endpoints]
app.MapGet("/api/alertas", async (AlertaDbContext db) =>
{
    var alertas = await db.Alertas
        .OrderByDescending(a => a.DataAlerta)
        .ThenByDescending(a => a.Id)
        .ToListAsync();

    return Results.Ok(new
    {
        total = alertas.Count,
        dados = alertas
    });
})
.RequireAuthorization()
.WithTags("Alertas")
.WithOpenApi(operation =>
{
    operation.Summary = "Lista todos os alertas";
    operation.Description = "Retorna todos os alertas cadastrados ordenados por data (mais recentes primeiro).";

    return operation;
});

app.MapGet("/api/alertas/talhao", async (
    int talhaoId,
    AlertaDbContext db) =>
{
    var alertas = await db.Alertas
        .Where(a => a.TalhaoId == talhaoId)
        .OrderByDescending(a => a.DataAlerta)
        .ThenByDescending(a => a.Id)
        .ToListAsync();

    return Results.Ok(new
    {
        total = alertas.Count,
        talhaoId,
        dados = alertas
    });
})
.RequireAuthorization()
.WithTags("Alertas")
.WithName("ListarAlertasPorTalhao")
.WithOpenApi(operation =>
{
    operation.Summary = "Lista alertas de um talhão";
    operation.Description = "Retorna todos os alertas de um talhão específico ordenados por data.";
    return operation;
});

app.MapGet("/api/alertas/talhaoperiodotipoalerta", async (
    int talhaoId,
    string tipoAlerta,
    string? dataInicio,
    string? dataFim,
    AlertaDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(tipoAlerta))
    {
        return Results.BadRequest(new { erro = "O tipo de alerta é obrigatório" });
    }

    var query = db.Alertas
        .Where(a => a.TalhaoId == talhaoId)
        .Where(a => a.TipoAlerta == tipoAlerta);

    if (!string.IsNullOrWhiteSpace(dataInicio) &&
        DateTime.TryParse(dataInicio, out var dInicio))
    {
        query = query.Where(a => a.DataAlerta >= dInicio);
    }

    if (!string.IsNullOrWhiteSpace(dataFim) &&
         DateTime.TryParse(dataFim, out var dFim))
    {
        var dFimAjustada = dFim.Date.AddDays(1).AddTicks(-1);
        query = query.Where(a => a.DataAlerta <= dFimAjustada);
    }

    var alertas = await query
        .OrderByDescending(a => a.DataAlerta)
        .ThenByDescending(a => a.Id)
        .ToListAsync();

    return Results.Ok(new
    {
        total = alertas.Count,
        dados = alertas
    });
})
.RequireAuthorization()
.WithTags("Alertas")
.WithName("PesquisarAlertas")
.WithOpenApi(operation =>
{
    operation.Summary = "Pesquisa alertas por talhão e tipo";
    operation.Description = "Tipos de alerta: Normal, Chuva, Temperatura, Solo ou Vento.";
    return operation;
});
#endregion

app.MapControllers();
app.Run();