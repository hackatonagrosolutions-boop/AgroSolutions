using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PropriedadeService.Data;
using PropriedadeService.Models;
using PropriedadeService.Repositories;
using Xunit;

namespace PropriedadeServiceTests.Repositories;

public class PropriedadeRepositoryTests
{
    private PropriedadesDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<PropriedadesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PropriedadesDbContext(options);
    }

    [Fact]
    public async Task CriarAsync_DeveSalvarNoBanco()
    {
        var context = CriarContexto();
        var repository = new PropriedadeRepository(context);

        var propriedade = new Propriedade
        {
            Nome = "Fazenda FIAP RJ",
            Cidade = "Rio de Janeiro",
            Estado = "RJ",
            AreaHectares = 500
        };

        await repository.AddAsync(propriedade);

        var salvo = await context.Propriedades.FirstOrDefaultAsync();

        salvo.Should().NotBeNull();
        salvo.Nome.Should().Be("Fazenda FIAP RJ");
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarDados()
    {
        var context = CriarContexto();

        context.Propriedades.Add(new Propriedade
        {
            Nome = "Fazenda FIAP SP",
            Cidade = "Ribeirão Preto", 
            Estado = "SP",           
            AreaHectares = 400         
        });

        await context.SaveChangesAsync();

        var repository = new PropriedadeRepository(context);

        var lista = await repository.GetAllAsync();

        lista.Should().NotBeNull();
        lista.Should().HaveCount(1);
        lista.First().Nome.Should().Be("Fazenda FIAP SP");
    }
}
