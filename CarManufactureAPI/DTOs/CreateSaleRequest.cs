using CarManufactureAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace CarManufactureAPI.DTOs
{
    /// <summary>
    /// DTO para crear una nueva venta.
    /// Contiene validaciones básicas para asegurar datos correctos.
    /// El campo CarModel acepta tanto el "nombre del modelo" como el índice numérico "(ej: 0, 1, 2, 3)."
    /// </summary>
    public class CreateSaleRequest
    {
        /// <summary>
        /// Modelo del automóvil. Puede ser:
        /// - Nombre: "Sedan", "SUV", "Offroad", "Sport" (case-insensitive)
        /// - Índice numérico: 0 (Sedan), 1 (SUV), 2 (Offroad), 3 (Sport)
        /// </summary>
        [Required(ErrorMessage = "El modelo del automóvil es requerido")]
        public string CarModel { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ID del centro de distribución es requerido")]
        [Range(1, 4, ErrorMessage = "El ID del centro de distribución debe ser mayor a 0")]
        public int DistributionCenterId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }
    }
}
