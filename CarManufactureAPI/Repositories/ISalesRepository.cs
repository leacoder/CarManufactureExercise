using CarManufactureAPI.Models;

namespace CarManufactureAPI.Repositories
{
    /// <summary>
    /// Interface que define el contrato para el repositorio de ventas.
    /// Permite abstraer la persistencia de datos y facilitar testing.
    /// </summary>
    public interface ISalesRepository
    {
        /// <summary>
        /// Agrega una nueva venta al repositorio.
        /// </summary>
        Sale AddSale(Sale sale);

        /// <summary>
        /// Obtiene un centro de distribución por su ID.
        /// </summary>
        DistributionCenter? GetDistributionCenter(int centerId);

        /// <summary>
        /// Obtiene todas las ventas registradas.
        /// </summary>
        IEnumerable<Sale> GetAllSales();

        /// <summary>
        /// Obtiene todas las ventas de un centro de distribución específico.
        /// </summary>
        IEnumerable<Sale> GetSalesByCenter(int centerId);

        /// <summary>
        /// Obtiene todos los centros de distribución.
        /// </summary>
        IEnumerable<DistributionCenter> GetAllDistributionCenters();

        /// <summary>
        /// Obtiene todas las ventas agrupadas por centro de distribución.
        /// </summary>
        Dictionary<int, List<Sale>> GetSalesGroupByCenter();

        /// <summary>
        /// Obtiene todas las ventas agrupadas por centro y modelo de auto.
        /// </summary>
        Dictionary<int, Dictionary<CarModelType, List<Sale>>> GetSalesGroupByCenterAndModel();
    }
}
