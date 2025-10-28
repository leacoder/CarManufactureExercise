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
    }
}
