namespace CarManufactureAPI.Models
{
    /// <summary>
    /// Clase estática que define los precios base y características especiales de cada modelo de automóvil.
    /// </summary>
    public static class CarModelPricing
    {
        // Precios base de cada modelo según especificaciones
        public const decimal SedanPrice = 8000m;
        public const decimal SUVPrice = 9500m;
        public const decimal OffroadPrice = 12500m;
        public const decimal SportPrice = 18200m;

        // Impuesto adicional para el modelo Sport (7%)
        public const decimal SportTaxRate = 0.07m;

        /// <summary>
        /// Calcula el precio final de un automóvil según su modelo.
        /// Aplica el impuesto del 7% al modelo Sport.
        /// </summary>
        /// <param name="modelType">Tipo de modelo de automóvil</param>
        /// <returns>Precio final del automóvil</returns>
        public static decimal GetPrice(CarModelType modelType)
        {
            decimal basePrice = modelType switch
            {
                CarModelType.Sedan => SedanPrice,
                CarModelType.SUV => SUVPrice,
                CarModelType.Offroad => OffroadPrice,
                CarModelType.Sport => SportPrice,
                _ => throw new ArgumentException($"Modelo no válido: {modelType}")
            };

            // Aplicar impuesto adicional al modelo Sport
            if (modelType == CarModelType.Sport)
            {
                basePrice += basePrice * SportTaxRate;
            }

            return basePrice;
        }
    }
}
