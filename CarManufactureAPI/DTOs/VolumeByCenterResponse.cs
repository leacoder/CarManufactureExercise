namespace CarManufactureAPI.DTOs
{
    /// <summary>
    /// DTO que representa el volumen de ventas de un centro de distribución específico.
    /// </summary>
    public class CenterVolumeDetail
    {
        public int DistributionCenterId { get; set; }
        
        public string DistributionCenterName { get; set; } = string.Empty;
        
        public int TotalUnits { get; set; }

        public decimal TotalAmount { get; set; }

        public int TotalSales { get; set; }
    }

    /// <summary>
    /// DTO que representa el volumen de ventas agrupado por centro de distribución.
    /// </summary>
    public class VolumeByCenterResponse
    {
        public List<CenterVolumeDetail> Centers { get; set; } = new();

        public int GrandTotalUnits { get; set; }

        public decimal GrandTotalAmount { get; set; }
    }
}
