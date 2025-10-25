namespace CarManufactureAPI.Models
{
    /// <summary>
    /// Representa un centro de distribución de la fábrica de automóviles.
    /// </summary>
    public class DistributionCenter
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}
