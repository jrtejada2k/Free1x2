using System.Threading.Tasks;
using Free1X2.Shared.Models;

namespace Free1X2.Shared.Services
{
    /// <summary>
    /// Interface for analysis operations - modernized version of the original MotorCalculo
    /// </summary>
    public interface IAnalysisService
    {
        /// <summary>
        /// Analyzes a set of matches and returns predictions
        /// </summary>
        Task<AnalysisResult> AnalyzeMatchesAsync(Match[] matches);
        
        /// <summary>
        /// Calculates statistics for a specific season and matchday
        /// </summary>
        Task<StatisticsResult> CalculateStatisticsAsync(string season, int matchday);
        
        /// <summary>
        /// Performs combination analysis similar to original desktop application
        /// </summary>
        Task<AnalysisResult> AnalyzeCombinationAsync(string[] combinations);
        
        /// <summary>
        /// Calculates column probabilities
        /// </summary>
        Task<AnalysisResult> CalculateColumnProbabilitiesAsync(Match[] matches);
    }
}