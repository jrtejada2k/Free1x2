using System.Threading.Tasks;
using Free1X2.Shared.Models;

namespace Free1X2.Shared.Services
{
    /// <summary>
    /// Interface for statistics operations
    /// </summary>
    public interface IStatisticsService
    {
        /// <summary>
        /// Gets season statistics
        /// </summary>
        Task<StatisticsResult> GetSeasonStatisticsAsync(string season);
        
        /// <summary>
        /// Gets team statistics for a specific season
        /// </summary>
        Task<StatisticsResult> GetTeamStatisticsAsync(int teamId, string season);
        
        /// <summary>
        /// Gets matchday statistics
        /// </summary>
        Task<StatisticsResult> GetMatchdayStatisticsAsync(string season, int matchday);
        
        /// <summary>
        /// Gets historical trends and patterns
        /// </summary>
        Task<StatisticsResult> GetHistoricalTrendsAsync(string[] seasons);
    }
}