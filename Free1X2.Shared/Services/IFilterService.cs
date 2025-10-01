using System.Threading.Tasks;
using Free1X2.Shared.Models;

namespace Free1X2.Shared.Services
{
    /// <summary>
    /// Interface for filter operations - modernized version of the original filter system
    /// </summary>
    public interface IFilterService
    {
        /// <summary>
        /// Gets all available filters in the system
        /// </summary>
        Task<AvailableFilter[]> GetAvailableFiltersAsync();
        
        /// <summary>
        /// Applies a specific filter to data
        /// </summary>
        Task<FilterResult> ApplyFilterAsync(string filterId, object data, object parameters);
        
        /// <summary>
        /// Applies multiple filters in sequence
        /// </summary>
        Task<FilterResult> ApplyMultipleFiltersAsync(string[] filterIds, object data, object[] parameters);
        
        /// <summary>
        /// Gets filter by category (e.g., "Formats", "Drawings", "Statistics")
        /// </summary>
        Task<AvailableFilter[]> GetFiltersByCategoryAsync(string category);
    }
}