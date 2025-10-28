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
        public void GetAllSales_ReturnsAllSales()
        {
            // Arrange
            var repository = new InMemorySalesRepository();

            // Agregar algunas ventas de prueba
            repository.AddSale(new Sale { CarModel = CarModelType.Sedan, DistributionCenterId = 1, Quantity = 5 });
            repository.AddSale(new Sale { CarModel = CarModelType.SUV, DistributionCenterId = 2, Quantity = 3 });
            repository.AddSale(new Sale { CarModel = CarModelType.Sport, DistributionCenterId = 1, Quantity = 2 });

            // Act
            var sales = repository.GetAllSales().ToList();

            // Assert
            sales.Should().NotBeNull();
            sales.Should().HaveCount(3);
            sales.Should().AllSatisfy(s => s.Id.Should().BeGreaterThan(0));
        }

        [Fact]
        public void GetSalesByCenter_ReturnsOnlySalesForSpecificCenter()
        {
            // Arrange
            var repository = new InMemorySalesRepository();
            var centerId = 1;

            // Agregar ventas en diferentes centros
            repository.AddSale(new Sale { CarModel = CarModelType.Sedan, DistributionCenterId = 1, Quantity = 5 });
            repository.AddSale(new Sale { CarModel = CarModelType.SUV, DistributionCenterId = 2, Quantity = 3 });
            repository.AddSale(new Sale { CarModel = CarModelType.Sport, DistributionCenterId = 1, Quantity = 2 });

            // Act
            var salesByCenter = repository.GetSalesByCenter(centerId).ToList();

            // Assert
            salesByCenter.Should().NotBeNull();
            salesByCenter.Should().HaveCount(2); // Solo las del centro 1
            salesByCenter.Should().AllSatisfy(s => s.DistributionCenterId.Should().Be(centerId));
        }

        [Fact]
        public void GetSalesByCenter_NonExistentCenter_ReturnsEmptyList()
        {
            // Arrange
            var repository = new InMemorySalesRepository();

            // Act
            var salesByCenter = repository.GetSalesByCenter(999).ToList();

            // Assert
            salesByCenter.Should().BeEmpty();
        }

        [Fact]
        public void GetAllDistributionCenters_ReturnsFourCenters()
        {
            // Arrange
            var repository = new InMemorySalesRepository();

            // Act
            var centers = repository.GetAllDistributionCenters().ToList();

            // Assert
            centers.Should().HaveCount(4);
            centers.Should().AllSatisfy(c =>
            {
                c.Id.Should().BeInRange(1, 4);
                c.Name.Should().NotBeNullOrEmpty();
                c.Location.Should().NotBeNullOrEmpty();
            });
        }

        [Fact]
        public void GetSalesGroupByCenter_ReturnsValidDictionary()
        {
            // Arrange
            var repository = new InMemorySalesRepository();

            // Agregar ventas en diferentes centros
            repository.AddSale(new Sale { CarModel = CarModelType.Sedan, DistributionCenterId = 1, Quantity = 5 });
            repository.AddSale(new Sale { CarModel = CarModelType.SUV, DistributionCenterId = 2, Quantity = 3 });
            repository.AddSale(new Sale { CarModel = CarModelType.Sport, DistributionCenterId = 1, Quantity = 2 });
            repository.AddSale(new Sale { CarModel = CarModelType.Offroad, DistributionCenterId = 3, Quantity = 4 });

            // Act
            var grouped = repository.GetSalesGroupByCenter();

            // Assert
            grouped.Should().NotBeNull();
            grouped.Should().NotBeEmpty();
            grouped.Should().HaveCount(3); // Centros 1, 2 y 3
            grouped.Keys.Should().AllSatisfy(k => k.Should().BeInRange(1, 4));
            grouped.Values.Should().AllSatisfy(v => v.Should().NotBeEmpty());

            // Verificar que centro 1 tiene 2 ventas
            grouped[1].Should().HaveCount(2);
            grouped[2].Should().HaveCount(1);
            grouped[3].Should().HaveCount(1);
        }

        [Fact]
        public void GetSalesGroupByCenterAndModel_ReturnsNestedDictionary()
        {
            // Arrange
            var repository = new InMemorySalesRepository();

            // Agregar ventas en diferentes centros y modelos
            repository.AddSale(new Sale { CarModel = CarModelType.Sedan, DistributionCenterId = 1, Quantity = 5 });
            repository.AddSale(new Sale { CarModel = CarModelType.SUV, DistributionCenterId = 1, Quantity = 3 });
            repository.AddSale(new Sale { CarModel = CarModelType.Sport, DistributionCenterId = 1, Quantity = 2 });
            repository.AddSale(new Sale { CarModel = CarModelType.Sedan, DistributionCenterId = 2, Quantity = 4 });
            repository.AddSale(new Sale { CarModel = CarModelType.Offroad, DistributionCenterId = 2, Quantity = 1 });

            // Act
            var grouped = repository.GetSalesGroupByCenterAndModel();

            // Assert
            grouped.Should().NotBeNull();
            grouped.Should().NotBeEmpty();
            grouped.Should().HaveCount(2); // Centros 1 y 2

            // Centro 1 debe tener 3 modelos
            grouped[1].Should().HaveCount(3);
            grouped[1].Should().ContainKey(CarModelType.Sedan);
            grouped[1].Should().ContainKey(CarModelType.SUV);
            grouped[1].Should().ContainKey(CarModelType.Sport);

            // Centro 2 debe tener 2 modelos
            grouped[2].Should().HaveCount(2);
            grouped[2].Should().ContainKey(CarModelType.Sedan);
            grouped[2].Should().ContainKey(CarModelType.Offroad);

            // Verificar que todas las ventas en cada grupo pertenecen al centro y modelo correctos
            foreach (var centerGroup in grouped)
            {
                var centerId = centerGroup.Key;
                foreach (var modelGroup in centerGroup.Value)
                {
                    var model = modelGroup.Key;
                    var sales = modelGroup.Value;

                    sales.Should().AllSatisfy(s =>
                    {
                        s.DistributionCenterId.Should().Be(centerId);
                        s.CarModel.Should().Be(model);
                    });
                }
            }
        }
    }
}
