using CarManufactureAPI.DTOs;

namespace CarManufactureAPI.Services
{
    /// <summary>
    /// Interface que define el contrato para el servicio de ventas.
    /// Contiene la lógica de negocio para operaciones relacionadas con ventas.
    /// </summary>
    public interface ISalesService
    {
        /// <summary>
        /// Crea una nueva venta en el sistema.
        /// </summary>
        SaleResponse CreateSale(CreateSaleRequest request);

        /// <summary>
        /// Obtiene el volumen total de ventas (unidades y monto).
        /// </summary>
        TotalVolumeResponse GetTotalVolume();

        /// <summary>
        /// Obtiene el volumen de ventas agrupado por centro de distribución.
        /// </summary>
        VolumeByCenterResponse GetVolumeByCenter();

        /// <summary>
        /// Obtiene el porcentaje de unidades de cada modelo vendido en cada centro sobre el total de ventas.
        /// </summary>
        PercentageByModelAndCenterResponse GetPercentageByModelAndCenter();
    }
}
