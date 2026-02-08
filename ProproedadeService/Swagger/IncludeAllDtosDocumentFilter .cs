using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using PropriedadeService.DTOs;
using PropriedadeService.Models;

namespace PropriedadeService.Swagger
{
    public class IncludeAllDtosDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var typesToRegister = new[]
            {
                // DTOs de Propriedade
                typeof(PropriedadeCreateDto),
                typeof(PropriedadeResponseResumoDto),
                
                // DTOs de Talhão
                typeof(TalhaoCreateDto),
                typeof(TalhaoUpdateDto),
                typeof(TalhaoResponseDto),
                typeof(TalhaoResponseResumoDto),
                
                // Models
                typeof(Propriedade),
                typeof(Talhao),
            };

            foreach (var type in typesToRegister)
            {
                try
                {
                    // Verifica se já não foi registrado
                    var schemaId = GetSchemaId(type);

                    if (!context.SchemaRepository.Schemas.ContainsKey(schemaId))
                    {
                        context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
                    }
                }
                catch
                {
                    // Ignora erros silenciosamente
                    continue;
                }
            }
        }

        private string GetSchemaId(Type type)
        {
            var name = type.Name;

            if (type.IsGenericType)
            {
                var genericArgs = string.Join("_", type.GetGenericArguments().Select(t => t.Name));
                name = $"{name.Split('`')[0]}_{genericArgs}";
            }

            return name;
        }
    }
}