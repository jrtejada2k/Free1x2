using System;
using System.Collections.Generic;

namespace Free1X2.Shared.Models
{
    /// <summary>
    /// Represents the result of an analysis operation
    /// </summary>
    public record AnalysisResult
    {
        public string AnalysisType { get; init; } = string.Empty;
        public Dictionary<string, object> Results { get; init; } = new();
        public DateTime AnalysisDate { get; init; } = DateTime.UtcNow;
        public bool IsSuccessful { get; init; } = true;
        public string? ErrorMessage { get; init; }
        public TimeSpan ProcessingTime { get; init; }
        
        public AnalysisResult() { }
        
        public AnalysisResult(string analysisType, Dictionary<string, object> results)
        {
            AnalysisType = analysisType ?? string.Empty;
            Results = results ?? new Dictionary<string, object>();
        }
        
        /// <summary>
        /// Creates a failed analysis result
        /// </summary>
        public static AnalysisResult Failed(string analysisType, string errorMessage)
        {
            return new AnalysisResult
            {
                AnalysisType = analysisType,
                IsSuccessful = false,
                ErrorMessage = errorMessage,
                Results = new Dictionary<string, object>()
            };
        }
    }
    
    /// <summary>
    /// Represents statistical information for a season or team
    /// </summary>
    public record StatisticsResult
    {
        public string StatisticsType { get; init; } = string.Empty;
        public string Season { get; init; } = string.Empty;
        public int? TeamId { get; init; }
        public Dictionary<string, double> Statistics { get; init; } = new();
        public DateTime GeneratedDate { get; init; } = DateTime.UtcNow;
        
        public StatisticsResult() { }
        
        public StatisticsResult(string statisticsType, string season, Dictionary<string, double> statistics)
        {
            StatisticsType = statisticsType ?? string.Empty;
            Season = season ?? string.Empty;
            Statistics = statistics ?? new Dictionary<string, double>();
        }
    }
    
    /// <summary>
    /// Represents the result of applying filters to data
    /// </summary>
    public record FilterResult
    {
        public string FilterType { get; init; } = string.Empty;
        public int TotalItems { get; init; }
        public int FilteredItems { get; init; }
        public Dictionary<string, object> FilteredData { get; init; } = new();
        public Dictionary<string, object> FilterParameters { get; init; } = new();
        public DateTime FilterDate { get; init; } = DateTime.UtcNow;
        
        public FilterResult() { }
        
        public FilterResult(string filterType, int totalItems, int filteredItems, Dictionary<string, object> filteredData)
        {
            FilterType = filterType ?? string.Empty;
            TotalItems = totalItems;
            FilteredItems = filteredItems;
            FilteredData = filteredData ?? new Dictionary<string, object>();
        }
        
        /// <summary>
        /// Gets the filter efficiency as a percentage
        /// </summary>
        public double FilterEfficiency => TotalItems > 0 ? (double)FilteredItems / TotalItems * 100 : 0;
    }
    
    /// <summary>
    /// Represents available filters in the system
    /// </summary>
    public record AvailableFilter
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public bool IsActive { get; init; } = true;
        public Dictionary<string, object> DefaultParameters { get; init; } = new();
        
        public AvailableFilter() { }
        
        public AvailableFilter(string id, string name, string description, string category)
        {
            Id = id ?? string.Empty;
            Name = name ?? string.Empty;
            Description = description ?? string.Empty;
            Category = category ?? string.Empty;
        }
    }
}