using CarManufactureAPI.Models;

namespace CarManufactureAPI.Repositories
{
    /// <summary>
    /// Implementación en memoria del repositorio de ventas.
    /// Los datos se mantienen en memoria durante la ejecución de la aplicación.
    /// Incluye datos mockeados de centros de distribución y ventas de ejemplo.
    /// </summary>
    public class InMemorySalesRepository : ISalesRepository
    {
        private readonly List<Sale> _sales;
        private readonly List<DistributionCenter> _distributionCenters;
        private int _nextSaleId = 1;

        public InMemorySalesRepository()
        {
            // Inicializo 4 centros
            _distributionCenters = new List<DistributionCenter>
            {
                new DistributionCenter { Id = 1, Name = "Centro Norte", Location = "Buenos Aires Norte" },
                new DistributionCenter { Id = 2, Name = "Centro Sur", Location = "Buenos Aires Sur" },
                new DistributionCenter { Id = 3, Name = "Centro Este", Location = "Región Este" },
                new DistributionCenter { Id = 4, Name = "Centro Oeste", Location = "Región Oeste" }
            };

            // Inicializar con datos mockeados de ventas para testing
            _sales = new List<Sale>();
            //GenerateMockSales();
        }

        /// <summary>
        /// Genera ventas de ejemplo para cada centro y modelo.
        /// Esto permite probar los endpoints sin necesidad de insertar datos manualmente.
        /// </summary>
        private void GenerateMockSales()
        {
            var random = new Random(42);
            var carModels = Enum.GetValues<CarModelType>();

            // Generar 20 ventas aleatorias distribuidas entre centros y modelos
            for (int i = 0; i < 20; i++)
            {
                var carModel = carModels[random.Next(carModels.Length)];
                var centerId = random.Next(1, 5); // 1 a 4
                var quantity = random.Next(1, 6); // 1 a 5 unidades

                var sale = new Sale
                {
                    Id = _nextSaleId++,
                    CarModel = carModel,
                    DistributionCenterId = centerId,
                    Quantity = quantity,
                    SaleDate = DateTime.UtcNow.AddDays(-random.Next(0, 30))
                };
                sale.CalculateTotalAmount();
                _sales.Add(sale);
            }
        }

        public Sale AddSale(Sale sale)
        {
            sale.Id = _nextSaleId++;
            sale.CalculateTotalAmount();
            _sales.Add(sale);
            return sale;
        }

        public DistributionCenter? GetDistributionCenter(int centerId)
        {
            return _distributionCenters.FirstOrDefault(dc => dc.Id == centerId);
        }

        public IEnumerable<Sale> GetAllSales()
        {
            return _sales.AsReadOnly();
        }

        public IEnumerable<Sale> GetSalesByCenter(int centerId)
        {
            return _sales.Where(s => s.DistributionCenterId == centerId).ToList();
        }

        public IEnumerable<DistributionCenter> GetAllDistributionCenters()
        {
            return _distributionCenters.AsReadOnly();
        }

        public Dictionary<int, List<Sale>> GetSalesGroupByCenter()
        {
            return _sales
                .GroupBy(s => s.DistributionCenterId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public Dictionary<int, Dictionary<CarModelType, List<Sale>>> GetSalesGroupByCenterAndModel()
        {
            return _sales
                .GroupBy(s => s.DistributionCenterId)
                .ToDictionary(
                    centerGroup => centerGroup.Key,
                    centerGroup => centerGroup
                        .GroupBy(s => s.CarModel)
                        .ToDictionary(modelGroup => modelGroup.Key, modelGroup => modelGroup.ToList())
                );
        }
    }
}