using CarManufactureAPI.Models;

namespace CarManufactureAPI.DTOs
{
    /// <summary>
    /// DTO de respuesta al crear una venta.
    /// Incluye toda la información relevante de la venta creada.
    /// </summary>
    public class SaleResponse
    {
        public int Id { get; set; }
        public string CarModel { get; set; } = string.Empty;
        public int DistributionCenterId { get; set; }
        public string DistributionCenterName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime SaleDate { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
