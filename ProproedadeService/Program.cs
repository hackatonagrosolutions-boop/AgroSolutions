using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PropriedadeService.Data;
using PropriedadeService.DTOs;
using PropriedadeService.Models;
using PropriedadeService.PropriedadeController.Interface;
using PropriedadeService.PropriedadeController;
using PropriedadeService.Repositories;
using PropriedadeService.Repositories.Interface;
using System.Reflection;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configurar DbContext
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<PropriedadesDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();

        // Registrar Repositories e Services
        builder.Services.AddScoped<IPropriedadeRepository, PropriedadeRepository>();
        builder.Services.AddScoped<IPropriedadeController, PropriedadeController>();

        #region [Swagger]
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "HACKATON - AgroSolutions - PropriedadeService",
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

        //app.UseHttpsRedirection();
        //app.UseAuthorization();
        app.MapControllers();

        #region [endpoints - Propriedade]
        app.MapPost("/api/propriedades", async (
            [FromBody] PropriedadeCreateDto PropriedadeDto,
            [FromServices] IPropriedadeController propriedadeService) =>
        {
            var propriedade = new Propriedade
            {
                Nome = PropriedadeDto.Nome,
                Cidade = PropriedadeDto.Cidade,
                Estado = PropriedadeDto.Estado,
                AreaHectares = PropriedadeDto.AreaTotal
            };

            await propriedadeService.CriarPropriedadeAsync(propriedade);
            return Results.Created($"/propriedades/{propriedade.Id}", propriedade);
        });

        app.MapGet("/api/propriedades", async (
            [FromServices] IPropriedadeController propriedadeService) =>
        {
            var propriedades = await propriedadeService.ListarPropriedadeAsync();
            return (propriedades == null || !propriedades.Any()) ? Results.NotFound(new { message = "Não há propriedades cadastradas." }) : Results.Ok(propriedades);
        });

        app.MapGet("/api/propriedades/{id}", async (
            int id,
            [FromServices] IPropriedadeController propriedadeService) =>
        {
            var propriedades = await propriedadeService.BuscarPorIdAsync(id);
            return propriedades is null ? Results.NotFound(new { message = $"Propriedade com ID {id} não encontrada." }) : Results.Ok(propriedades);
        });

        app.MapPut("/api/propriedades/{id}", async (
            int id,
            [FromBody] PropriedadeCreateDto PropriedadeDto,
            [FromServices] IPropriedadeController propriedadeService) =>
        {
            var propriedade = await propriedadeService.BuscarPorIdAsync(id);
            if (propriedade == null)
                return Results.NotFound(new { message = $"Propriedade com ID {id} não encontrada." });
                    propriedade.Nome = PropriedadeDto.Nome;
                    propriedade.Cidade = PropriedadeDto.Cidade;
                    propriedade.Estado = PropriedadeDto.Estado;
                    propriedade.AreaHectares = PropriedadeDto.AreaTotal;

            await propriedadeService.AtualizarPropriedadeAsync(propriedade);
            return Results.Ok(propriedade);
        });

        app.MapDelete("/api/propriedades/{id}", async (
            int id,
            [FromServices] IPropriedadeController propriedadeService) =>
        {
            var propriedade = await propriedadeService.BuscarPorIdAsync(id);
            if (propriedade == null)
                return Results.NotFound(new { message = $"Propriedade com ID {id} não encontrada." });

            await propriedadeService.RemoverPropriedadeAsync(id);
            return Results.Ok(new { message = $"Propriedade com ID {id} excluída com sucesso." });
        });
        #endregion

        app.Run();
    }
}