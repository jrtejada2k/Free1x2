using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Free1X2.Shared.Models;
using Free1X2.Shared.Services;

namespace Free1X2.WebAPI.Controllers
{
    /// <summary>
    /// API Controller for filter operations
    /// Provides endpoints for applying various filters to football data
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FiltersController : ControllerBase
    {
        private readonly IFilterService _filterService;
        private readonly ILogger<FiltersController> _logger;

        public FiltersController(IFilterService filterService, ILogger<FiltersController> logger)
        {
            _filterService = filterService ?? throw new ArgumentNullException(nameof(filterService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all available filters in the system
        /// </summary>
        /// <returns>Array of available filters</returns>
        [HttpGet]
        public async Task<ActionResult<AvailableFilter[]>> GetAvailableFilters()
        {
            try
            {
                _logger.LogInformation("Getting all available filters");
                var filters = await _filterService.GetAvailableFiltersAsync();
                return Ok(filters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available filters");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Gets filters by category
        /// </summary>
        /// <param name="category">Filter category (e.g., "Formats", "Drawings", "Statistics")</param>
        /// <returns>Array of filters in the specified category</returns>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<AvailableFilter[]>> GetFiltersByCategory(string category)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(category))
                {
                    return BadRequest(new { error = "Category is required" });
                }

                _logger.LogInformation("Getting filters by category: {Category}", category);
                var filters = await _filterService.GetFiltersByCategoryAsync(category);
                return Ok(filters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting filters by category: {Category}", category);
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Applies a specific filter to data
        /// </summary>
        /// <param name="filterId">Filter identifier</param>
        /// <param name="request">Filter application request</param>
        /// <returns>Filter results</returns>
        [HttpPost("{filterId}/apply")]
        public async Task<ActionResult<FilterResult>> ApplyFilter(string filterId, [FromBody] FilterRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filterId))
                {
                    return BadRequest(new { error = "Filter ID is required" });
                }

                if (request?.Data == null)
                {
                    return BadRequest(new { error = "Data is required for filter application" });
                }

                _logger.LogInformation("Applying filter: {FilterId}", filterId);
                var result = await _filterService.ApplyFilterAsync(filterId, request.Data, request.Parameters ?? new object());
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying filter: {FilterId}", filterId);
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Applies multiple filters in sequence
        /// </summary>
        /// <param name="request">Multiple filter application request</param>
        /// <returns>Combined filter results</returns>
        [HttpPost("apply-multiple")]
        public async Task<ActionResult<FilterResult>> ApplyMultipleFilters([FromBody] MultipleFilterRequest request)
        {
            try
            {
                if (request?.FilterIds == null || request.FilterIds.Length == 0)
                {
                    return BadRequest(new { error = "Filter IDs are required" });
                }

                if (request.Data == null)
                {
                    return BadRequest(new { error = "Data is required for filter application" });
                }

                _logger.LogInformation("Applying {FilterCount} filters in sequence", request.FilterIds.Length);
                var result = await _filterService.ApplyMultipleFiltersAsync(
                    request.FilterIds, 
                    request.Data, 
                    request.Parameters ?? Array.Empty<object>());
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying multiple filters");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Gets filter categories available in the system
        /// </summary>
        /// <returns>Array of filter categories</returns>
        [HttpGet("categories")]
        public ActionResult<string[]> GetFilterCategories()
        {
            try
            {
                _logger.LogInformation("Getting filter categories");
                
                // Based on the original Free1X2 filter system
                var categories = new[]
                {
                    "Formats",          // Formatos
                    "Drawings",         // Dibujos
                    "Statistics",       // Estadísticas
                    "Sequences",        // Seguidos
                    "Interruptions",    // Interrupciones
                    "Contacts",         // Contactos
                    "Weights",          // Pesos
                    "Valuations",       // Valoraciones
                    "Distances",        // Distancias
                    "Symmetries",       // Simetrías
                    "Groups",           // Grupos
                    "Differences",      // Diferencias
                    "Probabilities"     // Columnas Probables
                };
                
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting filter categories");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }
    }

    /// <summary>
    /// Request model for filter application
    /// </summary>
    public class FilterRequest
    {
        public object Data { get; set; } = new object();
        public object? Parameters { get; set; }
    }

    /// <summary>
    /// Request model for multiple filter application
    /// </summary>
    public class MultipleFilterRequest
    {
        public string[] FilterIds { get; set; } = Array.Empty<string>();
        public object Data { get; set; } = new object();
        public object[]? Parameters { get; set; }
    }
}