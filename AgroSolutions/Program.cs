using AlertaService.Data;
using AlertaService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

#region [endpoints]
app.MapGet("/api/alertas", async (int? talhaoId, AlertaDbContext db) =>
{
    var query = db.Alertas.AsQueryable();

    if (talhaoId.HasValue)
    {
        query = query.Where(a => a.TalhaoId == talhaoId.Value);
    }

    var alertas = await query
        .OrderByDescending(a => a.DataAlerta)
        .ToListAsync();

    return Results.Ok(alertas);
})
.RequireAuthorization()
.WithName("GetAlertas")
.WithOpenApi();

app.MapGet("/api/alertas/talhao/{id}", async (int id, AlertaDbContext db) =>
{
    var alertas = await db.Alertas
        .Where(a => a.TalhaoId == id)
        .OrderByDescending(a => a.DataAlerta)
        .ToListAsync();

    return Results.Ok(alertas);
})
.RequireAuthorization()
.WithName("GetAlertasByTalhao")
.WithOpenApi();
#endregion

app.MapControllers();
app.Run();