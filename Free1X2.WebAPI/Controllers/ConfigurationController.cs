using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Free1X2.Shared.Configuration;

namespace Free1X2.WebAPI.Controllers
{
    /// <summary>
    /// API Controller for application configuration
    /// Provides endpoints for retrieving application settings and status
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IAppConfiguration _appConfiguration;
        private readonly ILogger<ConfigurationController> _logger;

        public ConfigurationController(IAppConfiguration appConfiguration, ILogger<ConfigurationController> logger)
        {
            _appConfiguration = appConfiguration ?? throw new ArgumentNullException(nameof(appConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets API information and status
        /// </summary>
        /// <returns>API information</returns>
        [HttpGet("info")]
        public ActionResult<object> GetApiInfo()
        {
            try
            {
                _logger.LogInformation("Getting API information");
                
                var info = new
                {
                    name = "Free1X2 Web API",
                    version = "0.77.1",
                    description = "Backend API for Free1X2 football pools analysis application",
                    status = "operational",
                    timestamp = DateTime.UtcNow,
                    endpoints = new
                    {
                        teams = "/api/teams",
                        analysis = "/api/analysis",
                        filters = "/api/filters",
                        configuration = "/api/configuration"
                    },
                    features = new[]
                    {
                        "Team Management",
                        "Match Analysis",
                        "Statistical Calculations",
                        "Filter Operations",
                        "Combination Analysis",
                        "Column Probabilities"
                    }
                };
                
                return Ok(info);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting API information");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Gets application settings (public settings only)
        /// </summary>
        /// <returns>Public application settings</returns>
        [HttpGet("settings")]
        public ActionResult<object> GetPublicSettings()
        {
            try
            {
                _logger.LogInformation("Getting public application settings");
                
                var settings = new
                {
                    language = _appConfiguration.Language,
                    analysisEnabled = _appConfiguration.EnableAnalysis,
                    filtersEnabled = _appConfiguration.EnableFilters,
                    lastUpdate = DateTime.UtcNow,
                    analysisOptions = new
                    {
                        analyzeAll = _appConfiguration.AnalyzeAll,
                        analyzeVX2 = _appConfiguration.AnalyzeVX2,
                        analyzeSequences = _appConfiguration.AnalyzeSequences,
                        analyzeDrawings = _appConfiguration.AnalyzeDrawings,
                        analyzeFormats = _appConfiguration.AnalyzeFormats,
                        analyzeContacts = _appConfiguration.AnalyzeContacts,
                        analyzeWeights = _appConfiguration.AnalyzeWeights,
                        analyzeValuations = _appConfiguration.AnalyzeValuations,
                        analyzeDistances = _appConfiguration.AnalyzeDistances,
                        analyzeSymmetries = _appConfiguration.AnalyzeSymmetries
                    }
                };
                
                return Ok(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting public settings");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Gets API health status
        /// </summary>
        /// <returns>Health status information</returns>
        [HttpGet("health")]
        public ActionResult<object> GetHealthStatus()
        {
            try
            {
                _logger.LogInformation("Getting health status");
                
                var health = new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime,
                    version = "0.77.1",
                    services = new
                    {
                        analysis = "operational",
                        teams = "operational", 
                        filters = "operational",
                        configuration = "operational"
                    }
                };
                
                return Ok(health);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting health status");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Gets supported analysis types
        /// </summary>
        /// <returns>Array of supported analysis types</returns>
        [HttpGet("analysis-types")]
        public ActionResult<string[]> GetSupportedAnalysisTypes()
        {
            try
            {
                _logger.LogInformation("Getting supported analysis types");
                
                // Based on the original Free1X2 analysis capabilities
                var analysisTypes = new[]
                {
                    "Standard",         // Análisis estándar
                    "VX2",             // Análisis VX2
                    "Sequences",       // Signos seguidos
                    "Drawings",        // Dibujos
                    "Interruptions",   // Interrupciones
                    "Formats",         // Formatos
                    "Contacts",        // Contactos
                    "Weights",         // Pesos
                    "Valuations",      // Valoraciones
                    "Distances",       // Distancias
                    "Symmetries",      // Simetrías
                    "Groups",          // Grupos de equipos
                    "Combinations",    // Análisis de combinaciones
                    "Probabilities"    // Columnas probables
                };
                
                return Ok(analysisTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting supported analysis types");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }
    }
}