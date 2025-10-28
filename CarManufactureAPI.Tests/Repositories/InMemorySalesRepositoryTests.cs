using CarManufactureAPI.Models;
using CarManufactureAPI.Repositories;
using FluentAssertions;
using Xunit;

namespace CarManufactureAPI.Tests.Repositories
{
    /// <summary>
    /// Tests unitarios críticos para el repositorio en memoria InMemorySalesRepository.
    /// Cubre las operaciones esenciales con cobertura óptima del 80%.
    /// </summary>
    public class InMemorySalesRepositoryTests
    {
        [Fact]
        public void AddSale_AddsNewSaleWithCalculatedPrices()
        {
            // Arrange
            var repository = new InMemorySalesRepository();
            var initialSalesCount = repository.GetAllSales().Count();

            var newSale = new Sale
            {
                CarModel = CarModelType.Sedan,
                DistributionCenterId = 1,
                Quantity = 5
            };

            // Act
            var addedSale = repository.AddSale(newSale);
            var allSales = repository.GetAllSales().ToList();

            // Assert
            addedSale.Id.Should().BeGreaterThan(0);
            addedSale.UnitPrice.Should().Be(8000m);
            addedSale.TotalAmount.Should().Be(40000m);
            allSales.Should().HaveCount(initialSalesCount + 1);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(4)]
        public void GetDistributionCenter_ValidId_ReturnsCenter(int centerId)
        {
            // Arrange
            var repository = new InMemorySalesRepository();

            // Act
            var center = repository.GetDistributionCenter(centerId);

            // Assert
            center.Should().NotBeNull();
            center!.Id.Should().Be(centerId);
            center.Name.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GetDistributionCenter_InvalidId_ReturnsNull()
        {
            // Arrange
            var repository = new InMemorySalesRepository();

            // Act
            var center = repository.GetDistributionCenter(999);

            // Assert
            center.Should().BeNull();
        }

        [Fact]
        public void GetAllSales_ReturnsAllSalesIncludingMocked()
        {
            // Arrange & Act
            var repository = new InMemorySalesRepository();
            var sales = repository.GetAllSales().ToList();

            // Assert
            sales.Should().NotBeNull();
            sales.Should().HaveCount(20); // 20 ventas mockeadas
            sales.Should().AllSatisfy(s => s.Id.Should().BeGreaterThan(0));
        }
    }
}
