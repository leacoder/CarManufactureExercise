using CarManufactureAPI.DTOs;
using CarManufactureAPI.Models;
using CarManufactureAPI.Repositories;
using CarManufactureAPI.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace CarManufactureAPI.Tests.Services
{
    /// <summary>
    /// Tests unitarios para el servicio SalesService - CreateSale.
    /// Verifica la lógica de negocio de creación de ventas.
    /// </summary>
    public class SalesServiceTests
    {
        private readonly Mock<ISalesRepository> _mockRepository;
        private readonly SalesService _service;

        public SalesServiceTests()
        {
            _mockRepository = new Mock<ISalesRepository>();
            _service = new SalesService(_mockRepository.Object);
        }

        [Fact]
        public void CreateSale_WithValidModelName_ReturnsSaleResponse()
        {
            // Arrange
            var request = new CreateSaleRequest
            {
                CarModel = "Sedan",
                DistributionCenterId = 1,
                Quantity = 5
            };

            var center = new DistributionCenter { Id = 1, Name = "Centro Norte", Location = "Buenos Aires" };

            _mockRepository.Setup(r => r.GetDistributionCenter(1)).Returns(center);
            _mockRepository.Setup(r => r.AddSale(It.IsAny<Sale>()))
                .Returns((Sale s) =>
                {
                    s.Id = 1;
                    s.CalculateTotalAmount();
                    return s;
                });

            // Act
            var result = _service.CreateSale(request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.CarModel.Should().Be("Sedan");
            result.DistributionCenterId.Should().Be(1);
            result.DistributionCenterName.Should().Be("Centro Norte");
            result.Quantity.Should().Be(5);
            result.UnitPrice.Should().Be(8000m);
            result.TotalAmount.Should().Be(40000m);
        }

        [Theory]
        [InlineData("0", "Sedan")]
        [InlineData("3", "Sport")]
        public void CreateSale_WithNumericIndex_MapsToCorrectModel(string index, string expectedModel)
        {
            // Arrange
            var request = new CreateSaleRequest
            {
                CarModel = index,
                DistributionCenterId = 1,
                Quantity = 1
            };

            var center = new DistributionCenter { Id = 1, Name = "Centro Norte", Location = "Buenos Aires" };

            _mockRepository.Setup(r => r.GetDistributionCenter(1)).Returns(center);
            _mockRepository.Setup(r => r.AddSale(It.IsAny<Sale>()))
                .Returns((Sale s) =>
                {
                    s.Id = 4;
                    s.CalculateTotalAmount();
                    return s;
                });

            // Act
            var result = _service.CreateSale(request);

            // Assert
            result.Should().NotBeNull();
            result.CarModel.Should().Be(expectedModel);
        }

        [Fact]
        public void CreateSale_WithInvalidModelName_ThrowsException()
        {
            // Arrange
            var request = new CreateSaleRequest
            {
                CarModel = "InvalidModel",
                DistributionCenterId = 1,
                Quantity = 1
            };

            // Act
            Action act = () => _service.CreateSale(request);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*InvalidModel*no es válido*");
        }

        [Fact]
        public void CreateSale_WithInvalidModelIndex_ThrowsException()
        {
            // Arrange
            var request = new CreateSaleRequest
            {
                CarModel = "99",
                DistributionCenterId = 1,
                Quantity = 1
            };

            // Act
            Action act = () => _service.CreateSale(request);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*índice*no corresponde a ningún modelo válido*");
        }

        [Fact]
        public void CreateSale_WithEmptyModel_ThrowsException()
        {
            // Arrange
            var request = new CreateSaleRequest
            {
                CarModel = "",
                DistributionCenterId = 1,
                Quantity = 1
            };

            // Act
            Action act = () => _service.CreateSale(request);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*modelo del automóvil no puede estar vacío*");
        }

        [Fact]
        public void CreateSale_InvalidDistributionCenter_ThrowsException()
        {
            // Arrange
            var request = new CreateSaleRequest
            {
                CarModel = "Sedan",
                DistributionCenterId = 999,
                Quantity = 5
            };

            _mockRepository.Setup(r => r.GetDistributionCenter(999)).Returns((DistributionCenter?)null);

            // Act
            Action act = () => _service.CreateSale(request);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Centro de distribución*999*no existe");
        }

        [Fact]
        public void GetTotalVolume_WithMultipleSales_ReturnsCorrectTotals()
        {
            // Arrange
            var sales = new List<Sale>
            {
                new Sale { Id = 1, CarModel = CarModelType.Sedan, DistributionCenterId = 1, Quantity = 5, UnitPrice = 8000m, TotalAmount = 40000m },
                new Sale { Id = 2, CarModel = CarModelType.SUV, DistributionCenterId = 2, Quantity = 3, UnitPrice = 9500m, TotalAmount = 28500m },
                new Sale { Id = 3, CarModel = CarModelType.Sport, DistributionCenterId = 1, Quantity = 2, UnitPrice = 19474m, TotalAmount = 38948m }
            };

            _mockRepository.Setup(r => r.GetAllSales()).Returns(sales);

            // Act
            var result = _service.GetTotalVolume();

            // Assert
            result.Should().NotBeNull();
            result.TotalUnits.Should().Be(10); // 5 + 3 + 2
            result.TotalAmount.Should().Be(107448m); // 40000 + 28500 + 38948
            result.TotalSales.Should().Be(3);
        }

        [Fact]
        public void GetTotalVolume_WithNoSales_ReturnsZeros()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllSales()).Returns(new List<Sale>());

            // Act
            var result = _service.GetTotalVolume();

            // Assert
            result.Should().NotBeNull();
            result.TotalUnits.Should().Be(0);
            result.TotalAmount.Should().Be(0);
            result.TotalSales.Should().Be(0);
        }

        [Fact]
        public void GetVolumeByCenter_WithMultipleCenters_ReturnsCorrectVolumes()
        {
            // Arrange
            var centers = new List<DistributionCenter>
            {
                new DistributionCenter { Id = 1, Name = "Centro Norte", Location = "Norte" },
                new DistributionCenter { Id = 2, Name = "Centro Sur", Location = "Sur" }
            };

            var salesByCenter = new Dictionary<int, List<Sale>>
            {
                {
                    1, new List<Sale>
                    {
                        new Sale { Id = 1, CarModel = CarModelType.Sedan, DistributionCenterId = 1, Quantity = 5, UnitPrice = 8000m, TotalAmount = 40000m },
                        new Sale { Id = 2, CarModel = CarModelType.Sport, DistributionCenterId = 1, Quantity = 2, UnitPrice = 19474m, TotalAmount = 38948m }
                    }
                },
                {
                    2, new List<Sale>
                    {
                        new Sale { Id = 3, CarModel = CarModelType.SUV, DistributionCenterId = 2, Quantity = 3, UnitPrice = 9500m, TotalAmount = 28500m }
                    }
                }
            };

            _mockRepository.Setup(r => r.GetAllDistributionCenters()).Returns(centers);
            _mockRepository.Setup(r => r.GetSalesGroupByCenter()).Returns(salesByCenter);

            // Act
            var result = _service.GetVolumeByCenter();

            // Assert
            result.Should().NotBeNull();
            result.Centers.Should().HaveCount(2);
            result.GrandTotalUnits.Should().Be(10); // 5 + 2 + 3
            result.GrandTotalAmount.Should().Be(107448m); // 40000 + 38948 + 28500

            var center1 = result.Centers.First(c => c.DistributionCenterId == 1);
            center1.TotalUnits.Should().Be(7); // 5 + 2
            center1.TotalAmount.Should().Be(78948m); // 40000 + 38948
            center1.TotalSales.Should().Be(2);

            var center2 = result.Centers.First(c => c.DistributionCenterId == 2);
            center2.TotalUnits.Should().Be(3);
            center2.TotalAmount.Should().Be(28500m);
            center2.TotalSales.Should().Be(1);
        }

        [Fact]
        public void GetVolumeByCenter_WithCenterWithoutSales_IncludesZeroValues()
        {
            // Arrange
            var centers = new List<DistributionCenter>
            {
                new DistributionCenter { Id = 1, Name = "Centro Norte", Location = "Norte" },
                new DistributionCenter { Id = 2, Name = "Centro Sur", Location = "Sur" }
            };

            var salesByCenter = new Dictionary<int, List<Sale>>
            {
                {
                    1, new List<Sale>
                    {
                        new Sale { Id = 1, CarModel = CarModelType.Sedan, DistributionCenterId = 1, Quantity = 5, UnitPrice = 8000m, TotalAmount = 40000m }
                    }
                }
                // Centro 2 no tiene ventas
            };

            _mockRepository.Setup(r => r.GetAllDistributionCenters()).Returns(centers);
            _mockRepository.Setup(r => r.GetSalesGroupByCenter()).Returns(salesByCenter);

            // Act
            var result = _service.GetVolumeByCenter();

            // Assert
            result.Should().NotBeNull();
            result.Centers.Should().HaveCount(2);

            var center2 = result.Centers.First(c => c.DistributionCenterId == 2);
            center2.TotalUnits.Should().Be(0);
            center2.TotalAmount.Should().Be(0);
            center2.TotalSales.Should().Be(0);
        }

        [Fact]
        public void GetPercentageByModelAndCenter_CalculatesCorrectPercentages()
        {
            // Arrange
            var centers = new List<DistributionCenter>
            {
                new DistributionCenter { Id = 1, Name = "Centro Norte", Location = "Norte" }
            };

            var allSales = new List<Sale>
            {
                new Sale { Id = 1, CarModel = CarModelType.Sedan, DistributionCenterId = 1, Quantity = 6, UnitPrice = 8000m, TotalAmount = 48000m },
                new Sale { Id = 2, CarModel = CarModelType.Sport, DistributionCenterId = 1, Quantity = 4, UnitPrice = 19474m, TotalAmount = 77896m }
            };

            var salesByCenterAndModel = new Dictionary<int, Dictionary<CarModelType, List<Sale>>>
            {
                {
                    1, new Dictionary<CarModelType, List<Sale>>
                    {
                        { CarModelType.Sedan, new List<Sale> { allSales[0] } },
                        { CarModelType.Sport, new List<Sale> { allSales[1] } }
                    }
                }
            };

            _mockRepository.Setup(r => r.GetAllDistributionCenters()).Returns(centers);
            _mockRepository.Setup(r => r.GetAllSales()).Returns(allSales);
            _mockRepository.Setup(r => r.GetSalesGroupByCenterAndModel()).Returns(salesByCenterAndModel);

            // Act
            var result = _service.GetPercentageByModelAndCenter();

            // Assert
            result.Should().NotBeNull();
            result.TotalUnitsGlobal.Should().Be(10); // 6 + 4
            result.Centers.Should().HaveCount(1);

            var center1 = result.Centers.First();
            center1.TotalUnitsInCenter.Should().Be(10);
            center1.Models.Should().HaveCount(2);

            var sedanModel = center1.Models.First(m => m.CarModel == "Sedan");
            sedanModel.UnitsSold.Should().Be(6);
            sedanModel.PercentageOfCenter.Should().Be(60m); // 6/10 * 100
            sedanModel.PercentageOfTotal.Should().Be(60m); // 6/10 * 100

            var sportModel = center1.Models.First(m => m.CarModel == "Sport");
            sportModel.UnitsSold.Should().Be(4);
            sportModel.PercentageOfCenter.Should().Be(40m); // 4/10 * 100
            sportModel.PercentageOfTotal.Should().Be(40m); // 4/10 * 100
        }

        [Fact]
        public void GetPercentageByModelAndCenter_WithNoSales_ReturnsZeroGlobal()
        {
            // Arrange
            var centers = new List<DistributionCenter>
            {
                new DistributionCenter { Id = 1, Name = "Centro Norte", Location = "Norte" }
            };

            _mockRepository.Setup(r => r.GetAllDistributionCenters()).Returns(centers);
            _mockRepository.Setup(r => r.GetAllSales()).Returns(new List<Sale>());
            _mockRepository.Setup(r => r.GetSalesGroupByCenterAndModel())
                .Returns(new Dictionary<int, Dictionary<CarModelType, List<Sale>>>());

            // Act
            var result = _service.GetPercentageByModelAndCenter();

            // Assert
            result.Should().NotBeNull();
            result.TotalUnitsGlobal.Should().Be(0);
            result.Centers.Should().HaveCount(1);
            result.Centers.First().Models.Should().BeEmpty();
        }
    }
}
