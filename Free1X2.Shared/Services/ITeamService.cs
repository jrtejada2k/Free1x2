using System.Threading.Tasks;
using Free1X2.Shared.Models;

namespace Free1X2.Shared.Services
{
    /// <summary>
    /// Interface for team management operations
    /// </summary>
    public interface ITeamService
    {
        /// <summary>
        /// Gets all teams in the system
        /// </summary>
        Task<Team[]> GetAllTeamsAsync();
        
        /// <summary>
        /// Gets a specific team by ID
        /// </summary>
        Task<Team?> GetTeamByIdAsync(int teamId);
        
        /// <summary>
        /// Gets teams by category
        /// </summary>
        Task<Team[]> GetTeamsByCategoryAsync(string category);
        
        /// <summary>
        /// Gets match history for a specific team
        /// </summary>
        Task<Match[]> GetTeamHistoryAsync(int teamId, string season);
        
        /// <summary>
        /// Searches teams by name or code
        /// </summary>
        Task<Team[]> SearchTeamsAsync(string searchTerm);
    }
}