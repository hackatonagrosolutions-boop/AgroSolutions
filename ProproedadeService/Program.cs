using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PropriedadeService.Controller;
using PropriedadeService.Controller.Interface;
using PropriedadeService.Data;
using PropriedadeService.DTOs;
using PropriedadeService.Models;
using PropriedadeService.PropriedadeController;
using PropriedadeService.PropriedadeController.Interface;
using PropriedadeService.Repositories;
using PropriedadeService.Repositories.Interface;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configurar JwtSettings
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

        // Configurar DbContext
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<PropriedadesDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
             {
                 options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
             });


        builder.Services.AddEndpointsApiExplorer();

        // Registrar Repositories e Services
        builder.Services.AddScoped<IPropriedadeRepository, PropriedadeRepository>();
        builder.Services.AddScoped<IPropriedadeController, PropriedadeController>();
        builder.Services.AddScoped<ITalhaoRepository, TalhaoRepository>();
        builder.Services.AddScoped<ITalhaoController, TalhaoController>();

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

        builder.Services.AddAuthorization();

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
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        #region [Endpoints - Propriedade]
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
        })
        .WithTags("Propriedades")
        .RequireAuthorization();

        app.MapGet("/api/propriedades", async (
            [FromServices] IPropriedadeController propriedadeService) =>
        {
            var propriedades = await propriedadeService.ListarPropriedadeAsync();

            if (propriedades == null || !propriedades.Any())
                return Results.NotFound(new { message = "Não há propriedades cadastradas." });

            var response = propriedades.Select(p => new PropriedadeResponseResumoDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Cidade = p.Cidade,
                Estado = p.Estado,
                AreaHectares = p.AreaHectares,
                Talhoes = p.Talhoes?.Select(t => new TalhaoResponseResumoDto
                {
                    Id = t.Id,
                    Nome = t.Nome,
                    Cultura = t.Cultura,
                    Status = t.Status,
                    AreaHectares = (decimal)t.AreaHectares
                }).ToList() ?? new List<TalhaoResponseResumoDto>()
            });

            return Results.Ok(response);
        })
        .WithTags("Propriedades")
        .RequireAuthorization();

        app.MapGet("/api/propriedades/{id}", async (
            int id,
            [FromServices] IPropriedadeController propriedadeService) =>
        {
            var p = await propriedadeService.BuscarPorIdAsync(id);

            if (p is null)
                return Results.NotFound(new { message = $"Propriedade com ID {id} não encontrada." });

            var response = new PropriedadeResponseResumoDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Cidade = p.Cidade,
                Estado = p.Estado,
                AreaHectares = p.AreaHectares,
                Talhoes = p.Talhoes?.Select(t => new TalhaoResponseResumoDto
                {
                    Id = t.Id,
                    Nome = t.Nome,
                    Cultura = t.Cultura,
                    Status = t.Status,
                    AreaHectares = (decimal)t.AreaHectares
                }).ToList() ?? new List<TalhaoResponseResumoDto>()
            };

            return Results.Ok(response);
        })
        .WithTags("Propriedades")
        .RequireAuthorization();

        app.MapPut("/api/propriedades/{id}", async (
            int id,
            [FromBody] PropriedadeCreateDto PropriedadeDto,
            [FromServices] IPropriedadeController propriedadeService) =>
        {
            var p = await propriedadeService.BuscarPorIdAsync(id);
            if (p == null)
            {
                return Results.NotFound(new { message = $"Propriedade com ID {id} não encontrada." });
            }

            p.Nome = PropriedadeDto.Nome;
            p.Cidade = PropriedadeDto.Cidade;
            p.Estado = PropriedadeDto.Estado;
            p.AreaHectares = PropriedadeDto.AreaTotal;

            await propriedadeService.AtualizarPropriedadeAsync(p);

            var response = new PropriedadeResponseResumoDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Cidade = p.Cidade,
                Estado = p.Estado,
                AreaHectares = p.AreaHectares,
                Talhoes = p.Talhoes?.Select(t => new TalhaoResponseResumoDto
                {
                    Id = t.Id,
                    Nome = t.Nome,
                    Cultura = t.Cultura,
                    Status = t.Status,
                    AreaHectares = (decimal)t.AreaHectares
                }).ToList() ?? new List<TalhaoResponseResumoDto>()
            };
            return Results.Ok(response);
        })
        .WithTags("Propriedades")
        .RequireAuthorization();

        app.MapDelete("/api/propriedades/{id}", async (
            int id,
            [FromServices] IPropriedadeController propriedadeService) =>
        {
            var propriedade = await propriedadeService.BuscarPorIdAsync(id);
            if (propriedade == null)
            {
                return Results.NotFound(new { message = $"Propriedade com ID {id} não encontrada." });
            }

            await propriedadeService.RemoverPropriedadeAsync(id);
            return Results.Ok(new { message = $"Propriedade com ID {id} excluída com sucesso." });
        })
        .WithTags("Propriedades")
        .RequireAuthorization();
        #endregion

        #region[Endpoint - Talhão]
        app.MapPost("/api/talhoes", async (
            [FromBody] TalhaoCreateDto TalhaoDto,
            [FromServices] ITalhaoController talhaoService,
            [FromServices] IPropriedadeController propriedadeService) =>
        {
            var propriedade = await propriedadeService.BuscarPorIdAsync(TalhaoDto.PropriedadeId);

            if (propriedade == null)
            {
                return Results.BadRequest(new { message = $"A propriedade com ID {TalhaoDto.PropriedadeId} não existe." });
            }

            var talhao = new Talhao
            {
                Nome = TalhaoDto.Nome,
                Cultura = TalhaoDto.Cultura,
                AreaHectares = TalhaoDto.AreaHectares,
                PropriedadeId = TalhaoDto.PropriedadeId
            };

            await talhaoService.CriarTalhaoAsync(talhao);

            var talhaoResponseDto = new TalhaoResponseDto
            {
                Id = talhao.Id,
                Nome = talhao.Nome,
                Cultura = talhao.Cultura,
                Status = talhao.Status,
                AreaHectares = talhao.AreaHectares,
                PropriedadeId = talhao.PropriedadeId
            };

            return Results.Created($"/talhoes/{talhao.Id}", talhaoResponseDto);
        })
        .WithTags("Talhões")
        .RequireAuthorization();

        app.MapGet("/api/talhoes", async (
            [FromServices] ITalhaoController talhaoService) =>
        {
            var talhoes = await talhaoService.ListarTalhaoAsync();

            if (talhoes == null || !talhoes.Any())
            {
                return Results.NotFound(new { message = "Não há talhões cadastrados." });
            }

            var talhoesDto = talhoes.Select(t => new TalhaoResponseDto
            {
                Id = t.Id,
                Nome = t.Nome,
                Cultura = t.Cultura,
                Status = t.Status,
                AreaHectares = t.AreaHectares,
                PropriedadeId = t.PropriedadeId
            }).ToList();

            return Results.Ok(talhoesDto);
        })
        .WithTags("Talhões")
        .RequireAuthorization();

        app.MapGet("/api/talhoes/{id}", async (
            int id,
            [FromServices] ITalhaoController talhaoService) =>
        {
            var talhao = await talhaoService.BuscarPorIdAsync(id);

            if (talhao is null)
            {
                return Results.NotFound(new { message = $"Talhão com ID {id} não encontrado." });
            }

            var talhaoResponseDto = new TalhaoResponseDto
            {
                Id = talhao.Id,
                Nome = talhao.Nome,
                Cultura = talhao.Cultura,
                Status = talhao.Status,
                AreaHectares = talhao.AreaHectares,
                PropriedadeId = talhao.PropriedadeId,
            };

            return Results.Ok(talhaoResponseDto);
        })
        .WithTags("Talhões")
        .RequireAuthorization();

        app.MapPut("/api/talhoes/{id}", async (
            int id,
            [FromBody] TalhaoUpdateDto TalhaoDto,
            [FromServices] ITalhaoController talhaoService) =>
        {
            var talhao = await talhaoService.BuscarPorIdAsync(id);
            if (talhao == null)
            {
                return Results.NotFound(new { message = $"Talhão com ID {id} não encontrado." });
            }

            talhao.Nome = TalhaoDto.Nome;
            talhao.Cultura = TalhaoDto.Cultura;
            talhao.AreaHectares = TalhaoDto.AreaHectares;
            talhao.Status = TalhaoDto.Status;

            await talhaoService.AtualizarTalhaoAsync(talhao);

            var talhaoResponseDto = new TalhaoResponseDto
            {
                Id = talhao.Id,
                Nome = talhao.Nome,
                Cultura = talhao.Cultura,
                Status = talhao.Status,
                AreaHectares = talhao.AreaHectares,
                PropriedadeId = talhao.PropriedadeId
            };

            return Results.Ok(talhaoResponseDto);
        })
        .WithTags("Talhões")
        .RequireAuthorization();

        app.MapDelete("/api/talhoes/{id}", async (
            int id,
            [FromServices] ITalhaoController talhaoService) =>
        {
            var talhao = await talhaoService.BuscarPorIdAsync(id);
            if (talhao == null)
                return Results.NotFound(new { message = $"Talhão com ID {id} não encontrado." });

            await talhaoService.RemoverTalhaoAsync(id);
            return Results.Ok(new { message = $"Talhão com ID {id} excluído com sucesso." });
        })
        .WithTags("Talhões")
        .RequireAuthorization();
        #endregion

        app.Run();
    }
}