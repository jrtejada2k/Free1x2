using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Free1X2.Shared.Models;
using Free1X2.Shared.Services;

namespace Free1X2.WebAPI.Controllers
{
    /// <summary>
    /// API Controller for team management operations
    /// Provides endpoints for retrieving team information
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly ILogger<TeamsController> _logger;

        public TeamsController(ITeamService teamService, ILogger<TeamsController> logger)
        {
            _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all teams in the system
        /// </summary>
        /// <returns>Array of all teams</returns>
        [HttpGet]
        public async Task<ActionResult<Team[]>> GetAllTeams()
        {
            try
            {
                _logger.LogInformation("Getting all teams");
                var teams = await _teamService.GetAllTeamsAsync();
                return Ok(teams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all teams");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Gets a specific team by ID
        /// </summary>
        /// <param name="id">Team ID</param>
        /// <returns>Team information or 404 if not found</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeamById(int id)
        {
            try
            {
                _logger.LogInformation("Getting team with ID: {TeamId}", id);
                var team = await _teamService.GetTeamByIdAsync(id);
                
                if (team == null)
                {
                    return NotFound(new { error = "Team not found", teamId = id });
                }
                
                return Ok(team);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting team with ID: {TeamId}", id);
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Gets teams by category
        /// </summary>
        /// <param name="category">Team category</param>
        /// <returns>Array of teams in the specified category</returns>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<Team[]>> GetTeamsByCategory(string category)
        {
            try
            {
                _logger.LogInformation("Getting teams by category: {Category}", category);
                var teams = await _teamService.GetTeamsByCategoryAsync(category);
                return Ok(teams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting teams by category: {Category}", category);
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Searches teams by name or code
        /// </summary>
        /// <param name="searchTerm">Search term for team name or code</param>
        /// <returns>Array of matching teams</returns>
        [HttpGet("search")]
        public async Task<ActionResult<Team[]>> SearchTeams([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { error = "Search term is required" });
                }

                _logger.LogInformation("Searching teams with term: {SearchTerm}", searchTerm);
                var teams = await _teamService.SearchTeamsAsync(searchTerm);
                return Ok(teams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching teams with term: {SearchTerm}", searchTerm);
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Gets match history for a specific team
        /// </summary>
        /// <param name="id">Team ID</param>
        /// <param name="season">Season (optional)</param>
        /// <returns>Array of matches for the team</returns>
        [HttpGet("{id}/history")]
        public async Task<ActionResult<Match[]>> GetTeamHistory(int id, [FromQuery] string? season = null)
        {
            try
            {
                _logger.LogInformation("Getting team history for ID: {TeamId}, Season: {Season}", id, season ?? "current");
                
                // Use current season if not specified
                var targetSeason = season ?? DateTime.Now.Year.ToString();
                
                var matches = await _teamService.GetTeamHistoryAsync(id, targetSeason);
                return Ok(matches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting team history for ID: {TeamId}", id);
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }
    }
}