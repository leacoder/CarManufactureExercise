namespace CarManufactureAPI.DTOs
{
    /// <summary>
    /// DTO que representa el volumen total de ventas.
    /// Incluye la cantidad total de unidades vendidas y el monto total en dólares.
    /// </summary>
    public class TotalVolumeResponse
    {
        /// <summary>
        /// Cantidad total de unidades vendidas (suma de todas las ventas)
        /// </summary>
        public int TotalUnits { get; set; }

        /// <summary>
        /// Monto total de ventas en dólares USD
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Cantidad total de ventas registradas
        /// </summary>
        public int TotalSales { get; set; }
    }
}
