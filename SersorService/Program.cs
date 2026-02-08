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
        Chuva = dto.Chuva,
        DataLeitura = dto.DataLeitura ?? DateTime.UtcNow
    };

    await collection.InsertOneAsync(leitura);

    // Alerta de umidade do solo abaixo de 30% e leitura com mais de 1 dia
    bool enviarAlertaUmidadeSolo = false;
    if (leitura.UmidadeSolo < 30 && leitura.DataLeitura.AddDays(1) <= DateTime.UtcNow)
    {
        enviarAlertaUmidadeSolo = true;
        await rabbitService.PublicarAlertaAsync(new
        {
            Mensagem = "Alerta de Seca!",
            TalhaoId = leitura.TalhaoId,
            UmidadeSolo = leitura.UmidadeSolo,
            Vento = leitura.Vento,
            Temperatura = leitura.Temperatura,
            Chuva = leitura.Chuva,
            TipoAlerta = "Solo",
            Data = leitura.DataLeitura
        });
    }

    // Alerta de vento acima de 39 km/h
    bool enviarAlertaVento = false;
    if (leitura.Vento > 39)
    {
        string mensagemAlertaVento = "";
        enviarAlertaVento = true;

        if (leitura.Vento >= 40 && leitura.Vento <= 50)
        {
            mensagemAlertaVento = "Alerta de Vento Forte!";
        }
        else if (leitura.Vento > 50 && leitura.Vento <= 100)
        {
            mensagemAlertaVento = "Alerta de Vendaval!";
        }
        else if (leitura.Vento > 100 && leitura.Vento <= 250)
        {
            mensagemAlertaVento = "Alerta de Furacão!";
        }
        else if (leitura.Vento > 250)
        {
            mensagemAlertaVento = "Alerta de Tufão!";
        }

        await rabbitService.PublicarAlertaAsync(new
        {
            Mensagem = mensagemAlertaVento,
            TalhaoId = leitura.TalhaoId,
            UmidadeSolo = leitura.UmidadeSolo,
            Vento = leitura.Vento,
            Temperatura = leitura.Temperatura,
            Chuva = leitura.Chuva,
            TipoAlerta = "Vento",
            Data = leitura.DataLeitura
        });
    }

    // Alerta de temperatura
    string mensagemAlertaTemp = "";
    bool enviarAlertaTemp = false;

    if (leitura.Temperatura <= 0)
    {
        mensagemAlertaTemp = "Geada Detectada!";
        enviarAlertaTemp = true;
    }
    else if (leitura.Temperatura > 0 && leitura.Temperatura <= 10)
    {
        mensagemAlertaTemp = "Baixa Temperatura!";
        enviarAlertaTemp = true;
    }
    else if (leitura.Temperatura > 32 && leitura.Temperatura <= 40)
    {
        mensagemAlertaTemp = "Calor Excessivo!";
        enviarAlertaTemp = true;
    }
    else if (leitura.Temperatura > 40)
    {
        mensagemAlertaTemp = "Calor Extremo!";
        enviarAlertaTemp = true;
    }

    if (enviarAlertaTemp)
    {
        await rabbitService.PublicarAlertaAsync(new
        {
            Mensagem = mensagemAlertaTemp,
            TalhaoId = leitura.TalhaoId,
            UmidadeSolo = leitura.UmidadeSolo,
            Vento = leitura.Vento,
            Temperatura = leitura.Temperatura,
            Chuva = leitura.Chuva,
            TipoAlerta = "Temperatura",
            Data = leitura.DataLeitura
        });
    }

    // Alerta de chuva
    string mensagemAlertaChuva = "";
    bool enviarAlertaChuva = false;

    if (leitura.Chuva > 20)
    {
        mensagemAlertaChuva = "Chuva Moderada. Atenção ao acúmulo de água.";
        enviarAlertaChuva = true;
    }
    else if (leitura.Chuva > 20 && leitura.Chuva <= 50)
    {
        mensagemAlertaChuva = "Alerta: Chuva Forte! Risco de erosão e solo encharcado.";
        enviarAlertaChuva = true;
    }
    else if (leitura.Chuva > 50)
    {
        mensagemAlertaChuva = "ALERTA CRÍTICO: Tempestade Severa! Risco de inundação e danos às culturas.";
        enviarAlertaChuva = true;
    }

    if (enviarAlertaChuva)
    {
        await rabbitService.PublicarAlertaAsync(new
        {
            Mensagem = mensagemAlertaChuva,
            TalhaoId = leitura.TalhaoId,
            UmidadeSolo = leitura.UmidadeSolo,
            Vento = leitura.Vento,
            Temperatura = leitura.Temperatura,
            Chuva = leitura.Chuva,
            TipoAlerta = "Chuva",
            Data = leitura.DataLeitura
        });
    }

    // Alerta Solo
    bool enviarAlertaChuvaUmidadeSolo = false;
    if (leitura.Chuva > 30 && leitura.UmidadeSolo > 80)
    {
        enviarAlertaChuvaUmidadeSolo = true;
        await rabbitService.PublicarAlertaAsync(new
        {
            Mensagem = "PERIGO: Solo saturado com chuva forte. Risco alto de deslizamento ou perda de raiz.",
            TalhaoId = leitura.TalhaoId,
            UmidadeSolo = leitura.UmidadeSolo,
            Vento = leitura.Vento,
            Temperatura = leitura.Temperatura,
            Chuva = leitura.Chuva,
            TipoAlerta = "Solo",
            Data = leitura.DataLeitura
        });
    }

    // Se tudo estiver dentro dos parâmetros normais, envia este alerta
    if (!enviarAlertaUmidadeSolo && !enviarAlertaChuva && !enviarAlertaTemp && !enviarAlertaVento && !enviarAlertaChuvaUmidadeSolo)
    {
        await rabbitService.PublicarAlertaAsync(new
        {
            Mensagem = "Tlahão com status Normal!",
            TalhaoId = leitura.TalhaoId,
            UmidadeSolo = leitura.UmidadeSolo,
            Vento = leitura.Vento,
            Temperatura = leitura.Temperatura,
            Chuva = leitura.Chuva,
            TipoAlerta = "Normal",
            Data = leitura.DataLeitura
        });
    }

    return Results.Created($"/api/sensores/leitura/{leitura.Id}", leitura);
})
.WithTags("Leitura Sensor")
.WithOpenApi()
.RequireAuthorization();
#endregion

app.Run();