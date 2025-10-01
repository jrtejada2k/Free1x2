using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Free1X2.Shared.Configuration;

namespace Free1X2.Shared.Services
{
    /// <summary>
    /// Implementation of application configuration service
    /// Modernized version of VariablesGlobales that reads from the same sources
    /// </summary>
    public class AppConfiguration : IAppConfiguration
    {
        private readonly ILogger<AppConfiguration> _logger;
        private readonly Dictionary<string, string> _languageDictionary = new();
        private string _basePath;
        private bool _initialized = false;

        public AppConfiguration(ILogger<AppConfiguration> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _basePath = GetBasePath();
            Initialize();
        }

        #region File Paths
        public string DatabasePath => Path.Combine(_basePath, "parametros.free1x2");
        public string TempDirectory => Path.Combine(_basePath, "Temp");
        public string OutputDirectory => Path.Combine(_basePath, "Informes");
        public string EquiposDirectory => Path.Combine(_basePath, "Equipos");
        public string GanadorasDirectory => Path.Combine(_basePath, "Ganadoras");
        #endregion

        #region Application Settings
        public bool EnableLogging { get; private set; } = true;
        public bool EnableAnalysis { get; private set; } = true;
        public bool EnableFilters { get; private set; } = true;
        public string Language { get; private set; } = "es-ES";
        #endregion

        #region Analysis Settings
        public bool AnalyzeAll { get; private set; } = true;
        public bool AnalyzeVX2 { get; private set; } = true;
        public bool AnalyzeSequences { get; private set; } = true;
        public bool AnalyzeDrawings { get; private set; } = true;
        public bool AnalyzeInterruptions { get; private set; } = true;
        public bool AnalyzeFormats { get; private set; } = true;
        public bool AnalyzeContacts { get; private set; } = true;
        public bool AnalyzeWeights { get; private set; } = true;
        public bool AnalyzeValuations { get; private set; } = true;
        public bool AnalyzeDistances { get; private set; } = true;
        public bool AnalyzeSymmetries { get; private set; } = true;
        #endregion

        #region UI Settings
        public bool ShowToolbarFile { get; private set; } = true;
        public bool ShowToolbarUtilities { get; private set; } = true;
        public bool ShowConfirmationOnExit { get; private set; } = true;
        #endregion

        #region Update Settings
        public bool UpdateOnStartup { get; private set; } = false;
        public DateTime LastNotificationCheck { get; private set; } = DateTime.MinValue;
        #endregion

        private void Initialize()
        {
            if (_initialized) return;

            try
            {
                _logger.LogInformation("Initializing application configuration from: {BasePath}", _basePath);
                
                // Read configuration from the same sources as the original VariablesGlobales
                LoadFromParametersFile();
                LoadLanguageSettings();
                
                _initialized = true;
                _logger.LogInformation("Application configuration initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize application configuration");
                SetDefaultValues();
                _initialized = true;
            }
        }

        private void LoadFromParametersFile()
        {
            var parametersFile = DatabasePath;
            
            if (!File.Exists(parametersFile))
            {
                _logger.LogWarning("Parameters file not found: {ParametersFile}", parametersFile);
                return;
            }

            try
            {
                _logger.LogDebug("Loading configuration from: {ParametersFile}", parametersFile);
                
                // Read the binary parameters file (same format as original)
                using var fileStream = new FileStream(parametersFile, FileMode.Open, FileAccess.Read);
                using var reader = new BinaryReader(fileStream);

                // Read configuration values in the same order as VariablesGlobales
                // This maintains compatibility with the original desktop application
                
                if (fileStream.Length > 0)
                {
                    try
                    {
                        // Read boolean flags for analysis options
                        AnalyzeAll = ReadBooleanSafe(reader);
                        AnalyzeVX2 = ReadBooleanSafe(reader);
                        AnalyzeSequences = ReadBooleanSafe(reader);
                        AnalyzeDrawings = ReadBooleanSafe(reader);
                        AnalyzeInterruptions = ReadBooleanSafe(reader);
                        AnalyzeFormats = ReadBooleanSafe(reader);
                        AnalyzeContacts = ReadBooleanSafe(reader);
                        AnalyzeWeights = ReadBooleanSafe(reader);
                        AnalyzeValuations = ReadBooleanSafe(reader);
                        AnalyzeDistances = ReadBooleanSafe(reader);
                        AnalyzeSymmetries = ReadBooleanSafe(reader);
                        
                        // Read UI settings
                        ShowToolbarFile = ReadBooleanSafe(reader);
                        ShowToolbarUtilities = ReadBooleanSafe(reader);
                        ShowConfirmationOnExit = ReadBooleanSafe(reader);
                        
                        // Read update settings
                        UpdateOnStartup = ReadBooleanSafe(reader);
                        
                        _logger.LogDebug("Successfully loaded configuration from parameters file");
                    }
                    catch (EndOfStreamException)
                    {
                        _logger.LogWarning("Parameters file is shorter than expected, using default values for remaining settings");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load configuration from parameters file: {ParametersFile}", parametersFile);
            }
        }

        private bool ReadBooleanSafe(BinaryReader reader)
        {
            try
            {
                return reader.ReadBoolean();
            }
            catch (EndOfStreamException)
            {
                return false; // Default value
            }
        }

        private void LoadLanguageSettings()
        {
            var languageFile = Path.Combine(_basePath, "Idioma", $"{Language}.lang");
            
            if (!File.Exists(languageFile))
            {
                _logger.LogWarning("Language file not found: {LanguageFile}", languageFile);
                return;
            }

            try
            {
                _logger.LogDebug("Loading language settings from: {LanguageFile}", languageFile);
                
                var lines = File.ReadAllLines(languageFile);
                _languageDictionary.Clear();
                
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;
                        
                    var parts = line.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        _languageDictionary[parts[0].Trim()] = parts[1].Trim();
                    }
                }
                
                _logger.LogDebug("Loaded {Count} language entries", _languageDictionary.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load language settings from: {LanguageFile}", languageFile);
            }
        }

        private void SetDefaultValues()
        {
            _logger.LogInformation("Setting default configuration values");
            
            // Set safe defaults that match the original application
            AnalyzeAll = true;
            AnalyzeVX2 = true;
            AnalyzeSequences = true;
            AnalyzeDrawings = true;
            AnalyzeInterruptions = true;
            AnalyzeFormats = true;
            AnalyzeContacts = true;
            AnalyzeWeights = true;
            AnalyzeValuations = true;
            AnalyzeDistances = true;
            AnalyzeSymmetries = true;
            
            ShowToolbarFile = true;
            ShowToolbarUtilities = true;
            ShowConfirmationOnExit = true;
            
            UpdateOnStartup = false;
            LastNotificationCheck = DateTime.MinValue;
        }

        private string GetBasePath()
        {
            // Try to find the configuration path using the same logic as VariablesGlobales
            var possiblePaths = new[]
            {
                AppDomain.CurrentDomain.BaseDirectory,
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "",
                Environment.CurrentDirectory,
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Free1X2")
            };

            foreach (var path in possiblePaths)
            {
                if (Directory.Exists(path))
                {
                    var parametersFile = Path.Combine(path, "parametros.free1x2");
                    if (File.Exists(parametersFile))
                    {
                        _logger.LogInformation("Found configuration base path: {Path}", path);
                        return path;
                    }
                }
            }

            // Fallback to application base directory
            var fallbackPath = AppDomain.CurrentDomain.BaseDirectory;
            _logger.LogWarning("Could not find existing configuration, using fallback path: {Path}", fallbackPath);
            return fallbackPath;
        }

        public void Reload()
        {
            _logger.LogInformation("Reloading application configuration");
            _initialized = false;
            Initialize();
        }

        public void Save()
        {
            // For the API, we typically don't save configuration changes
            // The original desktop application handles configuration persistence
            _logger.LogInformation("Configuration save requested (API mode - no action taken)");
        }

        /// <summary>
        /// Gets a localized string by key
        /// </summary>
        public string GetLocalizedString(string key, string defaultValue = "")
        {
            return _languageDictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}