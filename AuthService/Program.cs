using AuthService.Data;
using AuthService.DTOs;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Repositories.Interface;
using AuthService.Services;
using AuthService.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar JwtSettings
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Configurar DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UsuariosDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Registrar Repositories e Services
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAuthService, AuthenticationService>();

#region [JWT]
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero
    };
});
#endregion

builder.Services.AddAuthorization();

#region [Swagger]
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HACKATON - AgroSolutions - AuthService",
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

app.UseAuthentication();
app.UseAuthorization();

#region [Endpoints]
app.MapPost("/api/auth/login", async (
    [FromBody] LoginDto loginDto,
    [FromServices] IAuthService authService) =>
{
    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(loginDto);

    if (!Validator.TryValidateObject(loginDto, validationContext, validationResults, true))
    {
        var errors = validationResults.ToDictionary(
            v => v.MemberNames.FirstOrDefault() ?? "Error",
            v => v.ErrorMessage ?? "Erro de validação"
        );
        return Results.BadRequest(new { errors });
    }

    try
    {
        var response = await authService.LoginAsync(loginDto);
        return Results.Ok(response);
    }
    catch (UnauthorizedAccessException ex)
    {
        return Results.Unauthorized();
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.WithName("Login")
.WithTags("Auth")
.Produces<LoginResponseDto>(200)
.ProducesProblem(401)
.ProducesProblem(400);

app.MapPost("/api/usuarios", async (
    [FromBody] UsuarioCreateDto dto,
    [FromServices] IUsuarioService service) =>
{
    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(dto);

    bool isValid = Validator.TryValidateObject(dto, validationContext, validationResults, validateAllProperties: true);

    if (!isValid)
    {
        var errors = validationResults.ToDictionary(
            v => v.MemberNames.FirstOrDefault() ?? "Error",
            v => v.ErrorMessage ?? "Erro de validação"
        );
        return Results.BadRequest(new { errors });
    }

    var usuario = new Usuario
    {
        Nome = dto.Nome,
        Email = dto.Email,
        SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
        Perfil = dto.Perfil
    };

    await service.CriarUsuarioAsync(usuario);
    return Results.Created($"/api/usuarios/{usuario.Id}", usuario);
})
.WithName("CreateUsuario")
.WithTags("Usuarios")
.AllowAnonymous();

app.MapGet("/api/usuarios/{id}", async (
    int id,
    [FromServices] IUsuarioService service) =>
{
    var usuario = await service.BuscarPorIdAsync(id);
    return usuario is null ? Results.NotFound() : Results.Ok(usuario);
})
.WithName("GetUsuarioById")
.WithTags("Usuarios")
.RequireAuthorization();

app.MapGet("/api/usuarios", async (
    [FromServices] IUsuarioService service) =>
{
    var usuarios = await service.ListarUsuariosAsync();
    return Results.Ok(usuarios);
})
.WithName("GetAllUsuarios")
.WithTags("Usuarios")
.RequireAuthorization(policy => policy.RequireRole("Admin"));

app.MapPut("/api/usuarios/{id}", async (
    int id,
    [FromBody] UsuarioUpdateDto dto,
    [FromServices] IUsuarioService service) =>
{
    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(dto);

    bool isValid = Validator.TryValidateObject(dto, validationContext, validationResults, validateAllProperties: true);

    if (!isValid)
    {
        var errors = validationResults.ToDictionary(
            v => v.MemberNames.FirstOrDefault() ?? "Error",
            v => v.ErrorMessage ?? "Erro de validação"
        );
        return Results.BadRequest(new { errors });
    }

    var usuarioExistente = await service.BuscarPorIdAsync(id);
    if (usuarioExistente == null)
        return Results.NotFound(new { message = $"Usuário com ID {id} não encontrado" });

    usuarioExistente.Nome = dto.Nome;
    usuarioExistente.Email = dto.Email;
    usuarioExistente.Perfil = dto.Perfil;

    if (!string.IsNullOrEmpty(dto.Senha))
    {
        usuarioExistente.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
    }

    await service.AtualizarUsuarioAsync(usuarioExistente);
    return Results.Ok(usuarioExistente);
})
.WithName("UpdateUsuario")
.WithTags("Usuarios")
.RequireAuthorization();

app.MapDelete("/api/usuarios/{id}", async (
    int id,
    [FromServices] IUsuarioService service) =>
{
    var usuario = await service.BuscarPorIdAsync(id);
    if (usuario == null)
        return Results.NotFound(new { message = $"Usuário com ID {id} não encontrado" });

    await service.RemoverUsuarioAsync(id);
    return Results.NoContent();
})
.WithName("DeleteUsuario")
.WithTags("Usuarios")
.RequireAuthorization(policy => policy.RequireRole("Admin"));
#endregion

app.Run();