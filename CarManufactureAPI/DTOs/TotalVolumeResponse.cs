namespace CarManufactureAPI.DTOs
{
    /// <summary>
    /// DTO que representa el volumen total de ventas.
    /// Incluye la cantidad total de unidades vendidas y el monto total en dólares.
    /// </summary>
    public class TotalVolumeResponse
    {
        public int TotalUnits { get; set; }

        public decimal TotalAmount { get; set; }

        public int TotalSales { get; set; }
    }
}
