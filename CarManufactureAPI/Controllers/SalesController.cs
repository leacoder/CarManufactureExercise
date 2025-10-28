using CarManufactureAPI.DTOs;
using CarManufactureAPI.Filters;
using CarManufactureAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarManufactureAPI.Controllers
{
    /// <summary>
    /// Controlador que expone los endpoints REST para operaciones de ventas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ServiceFilter(typeof(PerformanceFilter))]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;
        private readonly ILogger<SalesController> _logger;

        public SalesController(ISalesService salesService, ILogger<SalesController> logger)
        {
            _salesService = salesService;
            _logger = logger;
        }

        /// <summary>
        /// Inserta una nueva venta en el sistema.
        /// </summary>
        /// <param name="request">Datos de la venta a crear (modelo, centro de distribución, cantidad)</param>
        /// <returns>Información detallada de la venta creada incluyendo precios calculados</returns>
        /// <response code="201">Venta creada exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        [HttpPost]
        [ProducesResponseType(typeof(SaleResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateSale([FromBody] CreateSaleRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "Creando venta: Modelo={Model}, Centro={CenterId}, Cantidad={Quantity}",
                    request.CarModel,
                    request.DistributionCenterId,
                    request.Quantity
                );

                var result = _salesService.CreateSale(request);

                _logger.LogInformation("Venta creada con ID: {SaleId}", result.Id);

                return CreatedAtAction(nameof(CreateSale), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Error de validación al crear venta: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear venta");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el volumen total de ventas (unidades y monto total).
        /// </summary>
        /// <returns>Volumen total incluyendo cantidad de unidades, monto total y número de ventas</returns>
        /// <response code="200">Volumen total obtenido exitosamente</response>
        [HttpGet("total-volume")]
        [ProducesResponseType(typeof(TotalVolumeResponse), StatusCodes.Status200OK)]
        public IActionResult GetTotalVolume()
        {
            try
            {
                _logger.LogInformation("Obteniendo volumen total de ventas");

                var result = _salesService.GetTotalVolume();

                _logger.LogInformation(
                    "Volumen total: {TotalUnits} unidades, ${TotalAmount} USD, {TotalSales} ventas",
                    result.TotalUnits,
                    result.TotalAmount,
                    result.TotalSales
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener volumen total de ventas");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }
    }
}