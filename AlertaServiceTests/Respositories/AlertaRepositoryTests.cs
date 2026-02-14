using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using AlertaService.Data;
using AlertaService.Models;
using AlertaService.Repositories;
using Xunit;

namespace AlertaService.Tests.Repositories
{
    public class AlertaRepositoryTests
    {
        private AppDbContext CriarContexto()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_DevePersistirAlertaNoBanco()
        {
            // Arrange
            var context = CriarContexto();
            var repository = new AlertaRepository(context);
            var alerta = new Alerta
            {
                Descricao = "Invasão Detectada",
                Severidade = "Alta",
                PropriedadeId = 1,
                DataGeracao = DateTime.Now
            };

            // Act
            await repository.AddAsync(alerta);
            await context.SaveChangesAsync();

            // Assert
            var alertaNoBanco = await context.Alertas.FirstOrDefaultAsync();
            alertaNoBanco.Should().NotBeNull();
            alertaNoBanco.Descricao.Should().Be("Invasão Detectada");
        }
    }
}