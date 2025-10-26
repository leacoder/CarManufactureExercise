using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace CarManufactureAPI.Filters
{
    /// <summary>
    /// Filtro de acción que mide y registra el tiempo de ejecución de cada endpoint.
    /// Cumple con el requerimiento de "Imprimir el tiempo de ejecución de cada método".
    /// </summary>
    public class PerformanceFilter : IActionFilter
    {
        private readonly ILogger<PerformanceFilter> _logger;
        private Stopwatch? _stopwatch;

        public PerformanceFilter(ILogger<PerformanceFilter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Se ejecuta antes de la acción del controlador.
        /// Inicia el cronómetro para medir el tiempo de ejecución.
        /// </summary>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();

            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];

            _logger.LogInformation(
                "[PERFORMANCE] Iniciando ejecución: {Controller}.{Action}",
                controllerName,
                actionName
            );
        }

        /// <summary>
        /// Se ejecuta después de la acción del controlador.
        /// Detiene el cronómetro y registra el tiempo transcurrido.
        /// </summary>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch?.Stop();

            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            var elapsedMilliseconds = _stopwatch?.ElapsedMilliseconds ?? 0;

            _logger.LogInformation(
                "[PERFORMANCE] Tiempo de ejecución: {Controller}.{Action} - {ElapsedTime}ms",
                controllerName,
                actionName,
                elapsedMilliseconds
            );

            // Agregar el tiempo al header de respuesta para facilitar debugging
            context.HttpContext.Response.Headers.TryAdd(
                "X-Execution-Time-Ms",
                elapsedMilliseconds.ToString()
            );
        }
    }
}
