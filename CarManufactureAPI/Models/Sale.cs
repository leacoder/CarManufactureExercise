namespace CarManufactureAPI.Models
{
    /// <summary>
    /// Representa una venta de automóvil en un centro de distribución específico.
    /// </summary>
    public class Sale
    {
        public int Id { get; set; }
        public CarModelType CarModel { get; set; }
        public int DistributionCenterId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime SaleDate { get; set; }

        public Sale()
        {
            SaleDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Calcula el monto total de la venta basado en el modelo y cantidad.
        /// </summary>
        public void CalculateTotalAmount()
        {
            UnitPrice = CarModelPricing.GetPrice(CarModel);
            TotalAmount = UnitPrice * Quantity;
        }
    }
}
