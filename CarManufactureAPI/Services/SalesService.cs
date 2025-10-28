using CarManufactureAPI.DTOs;
using CarManufactureAPI.Models;
using CarManufactureAPI.Repositories;

namespace CarManufactureAPI.Services
{
    /// <summary>
    /// Servicio que implementa la lógica de negocio para operaciones de ventas.
    /// </summary>
    public class SalesService : ISalesService
    {
        private readonly ISalesRepository _repository;

        public SalesService(ISalesRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Crea una nueva venta validando que el centro de distribución exista.
        /// </summary>
        public SaleResponse CreateSale(CreateSaleRequest request)
        {
            // Parsear el modelo del auto (puede venir como string o número)
            CarModelType carModel = ParseCarModel(request.CarModel);

            // Validar que el centro de distribución existe
            var center = _repository.GetDistributionCenter(request.DistributionCenterId);
            if (center == null)
            {
                throw new ArgumentException($"Centro de distribución con ID {request.DistributionCenterId} no existe");
            }

            // Crear la venta
            var sale = new Sale
            {
                CarModel = carModel,
                DistributionCenterId = request.DistributionCenterId,
                Quantity = request.Quantity
            };

            var createdSale = _repository.AddSale(sale);

            return new SaleResponse
            {
                Id = createdSale.Id,
                CarModel = createdSale.CarModel.ToString(),
                DistributionCenterId = createdSale.DistributionCenterId,
                DistributionCenterName = center.Name,
                Quantity = createdSale.Quantity,
                UnitPrice = createdSale.UnitPrice,
                TotalAmount = createdSale.TotalAmount,
                SaleDate = createdSale.SaleDate,
                Message = "Venta creada exitosamente"
            };
        }

        /// <summary>
        /// Parsea el modelo del auto desde un string que puede ser:
        /// - El nombre del modelo: "Sedan", "SUV", "Offroad", "Sport" (case-insensitive)
        /// - El índice numérico: "0", "1", "2", "3"
        /// </summary>
        private CarModelType ParseCarModel(string carModelInput)
        {
            if (string.IsNullOrWhiteSpace(carModelInput))
            {
                throw new ArgumentException("El modelo del automóvil no puede estar vacío");
            }

            // Intentar parsear como número primero
            if (int.TryParse(carModelInput.Trim(), out int modelIndex))
            {
                if (Enum.IsDefined(typeof(CarModelType), modelIndex))
                {
                    return (CarModelType)modelIndex;
                }
                throw new ArgumentException($"El índice {modelIndex} no corresponde a ningún modelo válido. Los valores válidos son: 0 (Sedan), 1 (SUV), 2 (Offroad), 3 (Sport)");
            }

            // Intentar parsear como nombre del modelo
            if (Enum.TryParse<CarModelType>(carModelInput.Trim(), ignoreCase: true, out CarModelType modelType))
            {
                return modelType;
            }

            // Si no se pudo parsear, mostrar error con opciones válidas
            var validModels = string.Join(", ", Enum.GetNames(typeof(CarModelType)));
            throw new ArgumentException($"El modelo '{carModelInput}' no es válido. Los modelos válidos son: {validModels}");
        }

        /// <summary>
        /// Obtiene el volumen total de ventas sumando todas las unidades y montos.
        /// </summary>
        public TotalVolumeResponse GetTotalVolume()
        {
            var allSales = _repository.GetAllSales();

            return new TotalVolumeResponse
            {
                TotalUnits = allSales.Sum(s => s.Quantity),
                TotalAmount = allSales.Sum(s => s.TotalAmount),
                TotalSales = allSales.Count()
            };
        }

        /// <summary>
        /// Obtiene el volumen de ventas agrupado por centro de distribución.
        /// Para cada centro calcula: total de unidades, monto total y cantidad de ventas.
        /// </summary>
        public VolumeByCenterResponse GetVolumeByCenter()
        {
            var salesByCenter = _repository.GetSalesGroupByCenter();
            var allCenters = _repository.GetAllDistributionCenters();

            var centerVolumes = new List<CenterVolumeDetail>();

            // Iterar por todos los centros para incluir también los que no tienen ventas
            foreach (var center in allCenters)
            {
                var centerSales = salesByCenter.ContainsKey(center.Id) ? salesByCenter[center.Id] : new List<Sale>();

                centerVolumes.Add(new CenterVolumeDetail
                {
                    DistributionCenterId = center.Id,
                    DistributionCenterName = center.Name,
                    TotalUnits = centerSales.Sum(s => s.Quantity),
                    TotalAmount = centerSales.Sum(s => s.TotalAmount),
                    TotalSales = centerSales.Count
                });
            }

            return new VolumeByCenterResponse
            {
                Centers = centerVolumes,
                GrandTotalUnits = centerVolumes.Sum(c => c.TotalUnits),
                GrandTotalAmount = centerVolumes.Sum(c => c.TotalAmount)
            };
        }

        /// <summary>
        /// Obtiene el porcentaje de unidades de cada modelo vendido en cada centro.
        /// Calcula dos tipos de porcentajes:
        /// 1. Porcentaje sobre el total del centro (qué porcentaje representa cada modelo en ese centro)
        /// 2. Porcentaje sobre el total global (qué porcentaje representa sobre todas las ventas)
        /// </summary>
        public PercentageByModelAndCenterResponse GetPercentageByModelAndCenter()
        {
            var salesByCenterAndModel = _repository.GetSalesGroupByCenterAndModel();
            var allCenters = _repository.GetAllDistributionCenters();
            var allSales = _repository.GetAllSales();

            // Calcular el total global de unidades para los porcentajes
            int totalUnitsGlobal = allSales.Sum(s => s.Quantity);

            var centerPercentages = new List<CenterPercentageDetail>();

            foreach (var center in allCenters)
            {
                var centerDetail = new CenterPercentageDetail
                {
                    DistributionCenterId = center.Id,
                    DistributionCenterName = center.Name,
                    Models = new List<ModelPercentageDetail>()
                };

                // Si el centro tiene ventas, procesarlas
                if (salesByCenterAndModel.ContainsKey(center.Id))
                {
                    var modelSales = salesByCenterAndModel[center.Id];
                    int totalUnitsInCenter = modelSales.Values.SelectMany(s => s).Sum(s => s.Quantity);
                    centerDetail.TotalUnitsInCenter = totalUnitsInCenter;

                    // Procesar cada modelo en el centro
                    foreach (var modelGroup in modelSales)
                    {
                        int unitsOfModel = modelGroup.Value.Sum(s => s.Quantity);

                        centerDetail.Models.Add(new ModelPercentageDetail
                        {
                            CarModel = modelGroup.Key.ToString(),
                            UnitsSold = unitsOfModel,
                            PercentageOfCenter = totalUnitsInCenter > 0 ? Math.Round((decimal)unitsOfModel / totalUnitsInCenter * 100, 2) : 0,
                            PercentageOfTotal = totalUnitsGlobal > 0 ? Math.Round((decimal)unitsOfModel / totalUnitsGlobal * 100, 2) : 0
                        });
                    }

                    // Ordenar por modelo
                    centerDetail.Models = centerDetail.Models.OrderBy(m => m.CarModel).ToList();
                }

                centerPercentages.Add(centerDetail);
            }

            return new PercentageByModelAndCenterResponse
            {
                Centers = centerPercentages,
                TotalUnitsGlobal = totalUnitsGlobal
            };
        }
    }
}
