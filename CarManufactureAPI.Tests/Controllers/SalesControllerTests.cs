using CarManufactureAPI.Controllers;
using CarManufactureAPI.DTOs;
using CarManufactureAPI.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CarManufactureAPI.Tests.Controllers
{
    /// <summary>
    /// Tests unitarios para el controlador SalesController.
    /// Verifica el comportamiento de los endpoints REST y manejo de errores.
    /// </summary>
    public class SalesControllerTests
    {
        private readonly Mock<ISalesService> _mockService;
        private readonly Mock<ILogger<SalesController>> _mockLogger;
        private readonly SalesController _controller;

        public SalesControllerTests()
        {
            _mockService = new Mock<ISalesService>();
            _mockLogger = new Mock<ILogger<SalesController>>();
            _controller = new SalesController(_mockService.Object, _mockLogger.Object);
        }

        #region CreateSale Tests

        [Fact]
        public void CreateSale_ValidRequest_Returns201Created()
        {
            // Arrange
            var request = new CreateSaleRequest
            {
                CarModel = "Sedan",
                DistributionCenterId = 1,
                Quantity = 5
            };

            var expectedResponse = new SaleResponse
            {
                Id = 1,
                CarModel = "Sedan",
                DistributionCenterId = 1,
                DistributionCenterName = "Centro Norte",
                Quantity = 5,
                UnitPrice = 8000m,
                TotalAmount = 40000m,
                SaleDate = DateTime.UtcNow,
                Message = "Venta creada exitosamente"
            };

            _mockService.Setup(s => s.CreateSale(request)).Returns(expectedResponse);

            // Act
            var result = _controller.CreateSale(request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult!.StatusCode.Should().Be(201);
            createdResult.Value.Should().BeEquivalentTo(expectedResponse);

            // Verificar que se llamó al servicio
            _mockService.Verify(s => s.CreateSale(request), Times.Once);
        }

        [Fact]
        public void CreateSale_InvalidModel_Returns400BadRequest()
        {
            // Arrange
            var request = new CreateSaleRequest
            {
                CarModel = "InvalidModel",
                DistributionCenterId = 1,
                Quantity = 5
            };

            _mockService.Setup(s => s.CreateSale(request))
                .Throws(new ArgumentException("El modelo 'InvalidModel' no es válido"));

            // Act
            var result = _controller.CreateSale(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public void CreateSale_InvalidCenter_Returns400BadRequest()
        {
            // Arrange
            var request = new CreateSaleRequest
            {
                CarModel = "Sedan",
                DistributionCenterId = 999,
                Quantity = 5
            };

            _mockService.Setup(s => s.CreateSale(request))
                .Throws(new ArgumentException("Centro de distribución con ID 999 no existe"));

            // Act
            var result = _controller.CreateSale(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public void CreateSale_ServiceThrowsException_Returns500InternalServerError()
        {
            // Arrange
            var request = new CreateSaleRequest
            {
                CarModel = "Sedan",
                DistributionCenterId = 1,
                Quantity = 5
            };

            _mockService.Setup(s => s.CreateSale(request))
                .Throws(new Exception("Error inesperado en la base de datos"));

            // Act
            var result = _controller.CreateSale(request);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
            objectResult.Value.Should().NotBeNull();
        }

        #endregion

        #region GetTotalVolume Tests

        [Fact]
        public void GetTotalVolume_ReturnsOkWithData()
        {
            // Arrange
            var expectedResponse = new TotalVolumeResponse
            {
                TotalUnits = 65,
                TotalAmount = 785000m,
                TotalSales = 20
            };

            _mockService.Setup(s => s.GetTotalVolume()).Returns(expectedResponse);

            // Act
            var result = _controller.GetTotalVolume();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(expectedResponse);

            // Verificar que se llamó al servicio
            _mockService.Verify(s => s.GetTotalVolume(), Times.Once);
        }

        [Fact]
        public void GetTotalVolume_ServiceThrowsException_Returns500()
        {
            // Arrange
            _mockService.Setup(s => s.GetTotalVolume())
                .Throws(new Exception("Error al calcular volumen total"));

            // Act
            var result = _controller.GetTotalVolume();

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
            objectResult.Value.Should().NotBeNull();
        }

        #endregion

        #region GetVolumeByCenter Tests

        [Fact]
        public void GetVolumeByCenter_ReturnsOkWithData()
        {
            // Arrange
            var expectedResponse = new VolumeByCenterResponse
            {
                Centers = new List<CenterVolumeDetail>
                {
                    new CenterVolumeDetail
                    {
                        DistributionCenterId = 1,
                        DistributionCenterName = "Centro Norte",
                        TotalUnits = 18,
                        TotalAmount = 215000m,
                        TotalSales = 6
                    },
                    new CenterVolumeDetail
                    {
                        DistributionCenterId = 2,
                        DistributionCenterName = "Centro Sur",
                        TotalUnits = 22,
                        TotalAmount = 280000m,
                        TotalSales = 7
                    }
                },
                GrandTotalUnits = 40,
                GrandTotalAmount = 495000m
            };

            _mockService.Setup(s => s.GetVolumeByCenter()).Returns(expectedResponse);

            // Act
            var result = _controller.GetVolumeByCenter();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(expectedResponse);

            // Verificar que se llamó al servicio
            _mockService.Verify(s => s.GetVolumeByCenter(), Times.Once);
        }

        [Fact]
        public void GetVolumeByCenter_ServiceThrowsException_Returns500()
        {
            // Arrange
            _mockService.Setup(s => s.GetVolumeByCenter())
                .Throws(new Exception("Error al obtener volumen por centro"));

            // Act
            var result = _controller.GetVolumeByCenter();

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
            objectResult.Value.Should().NotBeNull();
        }

        #endregion

        #region GetPercentageByModelAndCenter Tests

        [Fact]
        public void GetPercentageByModelAndCenter_ReturnsOkWithData()
        {
            // Arrange
            var expectedResponse = new PercentageByModelAndCenterResponse
            {
                Centers = new List<CenterPercentageDetail>
                {
                    new CenterPercentageDetail
                    {
                        DistributionCenterId = 1,
                        DistributionCenterName = "Centro Norte",
                        TotalUnitsInCenter = 18,
                        Models = new List<ModelPercentageDetail>
                        {
                            new ModelPercentageDetail
                            {
                                CarModel = "Sedan",
                                UnitsSold = 8,
                                PercentageOfCenter = 44.44m,
                                PercentageOfTotal = 12.31m
                            },
                            new ModelPercentageDetail
                            {
                                CarModel = "Sport",
                                UnitsSold = 2,
                                PercentageOfCenter = 11.11m,
                                PercentageOfTotal = 3.08m
                            }
                        }
                    }
                },
                TotalUnitsGlobal = 65
            };

            _mockService.Setup(s => s.GetPercentageByModelAndCenter()).Returns(expectedResponse);

            // Act
            var result = _controller.GetPercentageByModelAndCenter();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(expectedResponse);

            // Verificar que se llamó al servicio
            _mockService.Verify(s => s.GetPercentageByModelAndCenter(), Times.Once);
        }

        [Fact]
        public void GetPercentageByModelAndCenter_ServiceThrowsException_Returns500()
        {
            // Arrange
            _mockService.Setup(s => s.GetPercentageByModelAndCenter())
                .Throws(new Exception("Error al calcular porcentajes"));

            // Act
            var result = _controller.GetPercentageByModelAndCenter();

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
            objectResult.Value.Should().NotBeNull();
        }

        #endregion
    }
}
