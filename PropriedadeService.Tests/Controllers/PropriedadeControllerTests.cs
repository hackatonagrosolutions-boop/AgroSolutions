using FluentAssertions;
using Moq;
using PropriedadeService.PropriedadeController;
using PropriedadeService.Repositories.Interface;
using PropriedadeService.Models;
using Xunit;

namespace PropriedadeServiceTests.Controllers;

public class PropriedadeControllerTests
{
    private readonly Mock<IPropriedadeRepository> _repositoryMock;
    private readonly PropriedadeController _controller;

    public PropriedadeControllerTests()
    {
        _repositoryMock = new Mock<IPropriedadeRepository>();
        _controller = new PropriedadeController(_repositoryMock.Object);
    }

    [Fact]
    public async Task CriarPropriedadeAsync_DeveChamarRepository()
    {
        var propriedade = new Propriedade
        {
            Nome = "Fazenda FIAP SP",
            Cidade = "Ribeirão Preto",
            Estado = "SP",
            AreaHectares = 120
        };

        await _controller.CriarPropriedadeAsync(propriedade);

        _repositoryMock.Verify(r =>
            r.AddAsync(It.IsAny<Propriedade>()),
            Times.Once);
    }

    [Fact]
    public async Task ListarPropriedadeAsync_DeveRetornarLista()
    {
        var lista = new List<Propriedade>
        {
            new Propriedade { Id = 1, Nome = "Fazenda FIAP SP" }
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(lista);

        var resultado = await _controller.ListarPropriedadeAsync();

        resultado.Should().HaveCount(1);
        resultado.First().Nome.Should().Be("Fazenda FIAP SP");
    }

    [Fact]
    public async Task BuscarPorIdAsync_DeveRetornarPropriedade()
    {
        var propriedade = new Propriedade
        {
            Id = 10,
            Nome = "Fazenda X"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(10))
            .ReturnsAsync(propriedade);

        var resultado = await _controller.BuscarPorIdAsync(10);

        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(10);
    }

    [Fact]
    public async Task RemoverPropriedadeAsync_DeveChamarRepository()
    {
        await _controller.RemoverPropriedadeAsync(5);

        _repositoryMock.Verify(r =>
            r.DeleteAsync(5),
            Times.Once);
    }
}
