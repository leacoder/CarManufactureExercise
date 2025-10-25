using CarManufactureAPI.DTOs;

namespace CarManufactureAPI.Services
{
    /// <summary>
    /// Interface que define el contrato para el servicio de ventas.
    /// Contiene la lógica de negocio para operaciones relacionadas con ventas.
    /// </summary>
    public interface ISalesService
    {
        SaleResponse CreateSale(CreateSaleRequest request);
    }
}
