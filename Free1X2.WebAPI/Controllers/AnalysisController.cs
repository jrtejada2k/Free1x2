using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Free1X2.Shared.Models;
using Free1X2.Shared.Services;

namespace Free1X2.WebAPI.Controllers
{
    /// <summary>
    /// API Controller for analysis operations
    /// Provides endpoints for football match analysis and predictions
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly IAnalysisService _analysisService;
        private readonly ILogger<AnalysisController> _logger;

        public AnalysisController(IAnalysisService analysisService, ILogger<AnalysisController> logger)
        {
            _analysisService = analysisService ?? throw new ArgumentNullException(nameof(analysisService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Analyzes matches and returns predictions
        /// </summary>
        /// <param name="request">Analysis request containing match data</param>
        /// <returns>Analysis results with predictions</returns>
        [HttpPost("matches")]
        public async Task<ActionResult<AnalysisResult>> AnalyzeMatches([FromBody] AnalysisRequest request)
        {
            try
            {
                if (request?.Matches == null || request.Matches.Length == 0)
                {
                    return BadRequest(new { error = "Matches are required for analysis" });
                }

                _logger.LogInformation("Analyzing {MatchCount} matches", request.Matches.Length);
                var result = await _analysisService.AnalyzeMatchesAsync(request.Matches);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing matches");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Calculates statistics for a specific season and matchday
        /// </summary>
        /// <param name="season">Season identifier</param>
        /// <param name="matchday">Matchday number</param>
        /// <returns>Statistical analysis results</returns>
        [HttpGet("statistics/{season}/{matchday}")]
        public async Task<ActionResult<StatisticsResult>> CalculateStatistics(string season, int matchday)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(season))
                {
                    return BadRequest(new { error = "Season is required" });
                }

                if (matchday <= 0)
                {
                    return BadRequest(new { error = "Matchday must be a positive number" });
                }

                _logger.LogInformation("Calculating statistics for season: {Season}, matchday: {Matchday}", season, matchday);
                var result = await _analysisService.CalculateStatisticsAsync(season, matchday);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating statistics for season: {Season}, matchday: {Matchday}", season, matchday);
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Analyzes betting combinations
        /// </summary>
        /// <param name="request">Combination analysis request</param>
        /// <returns>Analysis results for the combinations</returns>
        [HttpPost("combinations")]
        public async Task<ActionResult<AnalysisResult>> AnalyzeCombination([FromBody] CombinationRequest request)
        {
            try
            {
                if (request?.Combinations == null || request.Combinations.Length == 0)
                {
                    return BadRequest(new { error = "Combinations are required for analysis" });
                }

                _logger.LogInformation("Analyzing {CombinationCount} combinations", request.Combinations.Length);
                var result = await _analysisService.AnalyzeCombinationAsync(request.Combinations);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing combinations");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Calculates column probabilities for given matches
        /// </summary>
        /// <param name="request">Column probability request</param>
        /// <returns>Column probability analysis results</returns>
        [HttpPost("column-probabilities")]
        public async Task<ActionResult<AnalysisResult>> CalculateColumnProbabilities([FromBody] ColumnProbabilityRequest request)
        {
            try
            {
                if (request?.Matches == null || request.Matches.Length == 0)
                {
                    return BadRequest(new { error = "Matches are required for column probability calculation" });
                }

                _logger.LogInformation("Calculating column probabilities for {MatchCount} matches", request.Matches.Length);
                var result = await _analysisService.CalculateColumnProbabilitiesAsync(request.Matches);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating column probabilities");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }
    }

    /// <summary>
    /// Request model for match analysis
    /// </summary>
    public class AnalysisRequest
    {
        public Match[] Matches { get; set; } = Array.Empty<Match>();
        public string AnalysisType { get; set; } = "Standard";
        public object? Parameters { get; set; }
    }

    /// <summary>
    /// Request model for combination analysis
    /// </summary>
    public class CombinationRequest
    {
        public string[] Combinations { get; set; } = Array.Empty<string>();
        public string AnalysisType { get; set; } = "Standard";
        public object? Parameters { get; set; }
    }

    /// <summary>
    /// Request model for column probability calculation
    /// </summary>
    public class ColumnProbabilityRequest
    {
        public Match[] Matches { get; set; } = Array.Empty<Match>();
        public string Method { get; set; } = "Standard";
        public object? Parameters { get; set; }
    }
}