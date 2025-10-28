namespace CarManufactureAPI.DTOs
{
    /// <summary>
    /// DTO que representa el porcentaje de unidades de un modelo específico en un centro.
    /// </summary>
    public class ModelPercentageDetail
    {
        public string CarModel { get; set; } = string.Empty;

        public int UnitsSold { get; set; }

        public decimal PercentageOfCenter { get; set; }

        public decimal PercentageOfTotal { get; set; }
    }

    /// <summary>
    /// DTO que representa las ventas de un centro con el desglose por modelo.
    /// </summary>
    public class CenterPercentageDetail
    {
        public int DistributionCenterId { get; set; }

        public string DistributionCenterName { get; set; } = string.Empty;

        public int TotalUnitsInCenter { get; set; }

        public List<ModelPercentageDetail> Models { get; set; } = new();
    }

    /// <summary>
    /// DTO que representa el porcentaje de unidades de cada modelo vendido en cada centro.
    /// </summary>
    public class PercentageByModelAndCenterResponse
    {
        public List<CenterPercentageDetail> Centers { get; set; } = new();

        public int TotalUnitsGlobal { get; set; }
    }
}
