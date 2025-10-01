using System;

namespace Free1X2.Shared.Configuration
{
    /// <summary>
    /// Interface for application configuration - modernized version of VariablesGlobales
    /// </summary>
    public interface IAppConfiguration
    {
        // File paths
        string DatabasePath { get; }
        string TempDirectory { get; }
        string OutputDirectory { get; }
        string EquiposDirectory { get; }
        string GanadorasDirectory { get; }
        
        // Application settings
        bool EnableLogging { get; }
        bool EnableAnalysis { get; }
        bool EnableFilters { get; }
        string Language { get; }
        
        // Analysis settings
        bool AnalyzeAll { get; }
        bool AnalyzeVX2 { get; }
        bool AnalyzeSequences { get; }
        bool AnalyzeDrawings { get; }
        bool AnalyzeInterruptions { get; }
        bool AnalyzeFormats { get; }
        bool AnalyzeContacts { get; }
        bool AnalyzeWeights { get; }
        bool AnalyzeValuations { get; }
        bool AnalyzeDistances { get; }
        bool AnalyzeSymmetries { get; }
        
        // UI settings
        bool ShowToolbarFile { get; }
        bool ShowToolbarUtilities { get; }
        bool ShowConfirmationOnExit { get; }
        
        // Update settings
        bool UpdateOnStartup { get; }
        DateTime LastNotificationCheck { get; }
        
        /// <summary>
        /// Reloads configuration from source
        /// </summary>
        void Reload();
        
        /// <summary>
        /// Saves current configuration
        /// </summary>
        void Save();
    }
}