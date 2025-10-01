using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Free1X2.Shared.Models;
using Free1X2.Shared.Services;
using Free1X2.Shared.Configuration;

namespace Free1X2.Shared.Services
{
    /// <summary>
    /// Implementation of team service that reads from the same data files as the original application
    /// </summary>
    public class TeamService : ITeamService
    {
        private readonly IAppConfiguration _configuration;
        private readonly ILogger<TeamService> _logger;
        private readonly List<Team> _cachedTeams = new();
        private DateTime _lastCacheUpdate = DateTime.MinValue;
        private readonly TimeSpan _cacheTimeout = TimeSpan.FromMinutes(30);

        public TeamService(IAppConfiguration configuration, ILogger<TeamService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Team[]> GetAllTeamsAsync()
        {
            await RefreshCacheIfNeeded();
            return _cachedTeams.ToArray();
        }

        public async Task<Team?> GetTeamByIdAsync(int teamId)
        {
            await RefreshCacheIfNeeded();
            return _cachedTeams.FirstOrDefault(t => t.Id == teamId);
        }

        public async Task<Team[]> GetTeamsByCategoryAsync(string category)
        {
            await RefreshCacheIfNeeded();
            return _cachedTeams
                .Where(t => string.Equals(t.Category, category, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        public async Task<Match[]> GetTeamHistoryAsync(int teamId, string season)
        {
            // For now, return empty array - this would require reading match history files
            // In a full implementation, this would read from historical match data
            _logger.LogInformation("Getting team history for team {TeamId} in season {Season}", teamId, season);
            
            await Task.CompletedTask;
            return Array.Empty<Match>();
        }

        public async Task<Team[]> SearchTeamsAsync(string searchTerm)
        {
            await RefreshCacheIfNeeded();
            
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Array.Empty<Team>();

            var lowerSearchTerm = searchTerm.ToLowerInvariant();
            
            return _cachedTeams
                .Where(t => t.Name.ToLowerInvariant().Contains(lowerSearchTerm) ||
                           t.Code.ToLowerInvariant().Contains(lowerSearchTerm))
                .ToArray();
        }

        private async Task RefreshCacheIfNeeded()
        {
            if (_cachedTeams.Count == 0 || DateTime.Now - _lastCacheUpdate > _cacheTimeout)
            {
                await LoadTeamsFromDataFiles();
                _lastCacheUpdate = DateTime.Now;
            }
        }

        private async Task LoadTeamsFromDataFiles()
        {
            try
            {
                _logger.LogInformation("Loading teams from data files");
                _cachedTeams.Clear();

                var equiposDirectory = _configuration.EquiposDirectory;
                
                if (!Directory.Exists(equiposDirectory))
                {
                    _logger.LogWarning("Equipos directory not found: {Directory}", equiposDirectory);
                    return;
                }

                // Load teams from different categories
                await LoadTeamsFromFile(Path.Combine(equiposDirectory, "equipos1.dat"), "Primera División");
                await LoadTeamsFromFile(Path.Combine(equiposDirectory, "equipos2.dat"), "Segunda División");
                await LoadTeamsFromFile(Path.Combine(equiposDirectory, "equipos2b.dat"), "Segunda División B");
                await LoadTeamsFromFile(Path.Combine(equiposDirectory, "equiposInt.dat"), "Internacional");

                _logger.LogInformation("Loaded {Count} teams from data files", _cachedTeams.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load teams from data files");
            }
        }

        private async Task LoadTeamsFromFile(string filePath, string category)
        {
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Team file not found: {FilePath}", filePath);
                return;
            }

            try
            {
                _logger.LogDebug("Loading teams from file: {FilePath}", filePath);
                
                var lines = await File.ReadAllLinesAsync(filePath);
                var teamId = _cachedTeams.Count + 1; // Continue numbering from existing teams
                
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;

                    var team = ParseTeamLine(line, teamId, category);
                    if (team != null)
                    {
                        _cachedTeams.Add(team);
                        teamId++;
                    }
                }
                
                _logger.LogDebug("Loaded {Count} teams from {Category}", 
                    lines.Length, category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load teams from file: {FilePath}", filePath);
            }
        }

        private Team? ParseTeamLine(string line, int teamId, string category)
        {
            try
            {
                // Team data files typically contain tab-separated values
                // Format: Name\tCode or just Name
                var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                
                if (parts.Length == 0)
                    return null;

                var name = parts[0].Trim();
                var code = parts.Length > 1 ? parts[1].Trim() : GenerateCodeFromName(name);
                
                if (string.IsNullOrWhiteSpace(name))
                    return null;

                return new Team(teamId, name, code, category)
                {
                    IsActive = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse team line: {Line}", line);
                return null;
            }
        }

        private string GenerateCodeFromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "UNK";

            // Generate a 3-letter code from the team name
            // Take first 3 consonants or first 3 letters
            var consonants = name.ToUpperInvariant()
                .Where(c => char.IsLetter(c) && !"AEIOU".Contains(c))
                .Take(3)
                .ToArray();

            if (consonants.Length >= 3)
                return new string(consonants);

            // Fallback: take first 3 letters
            var letters = name.ToUpperInvariant()
                .Where(char.IsLetter)
                .Take(3)
                .ToArray();

            return letters.Length > 0 ? new string(letters).PadRight(3, 'X') : "UNK";
        }

        /// <summary>
        /// Gets teams statistics for analysis purposes
        /// </summary>
        public async Task<Dictionary<int, object>> GetTeamStatisticsAsync()
        {
            await RefreshCacheIfNeeded();
            
            var statistics = new Dictionary<int, object>();
            
            foreach (var team in _cachedTeams)
            {
                statistics[team.Id] = new
                {
                    team.Name,
                    team.Code,
                    team.Category,
                    team.IsActive,
                    // Placeholder for additional statistics
                    MatchesPlayed = 0,
                    Wins = 0,
                    Draws = 0,
                    Losses = 0
                };
            }
            
            return statistics;
        }

        /// <summary>
        /// Gets team by code
        /// </summary>
        public async Task<Team?> GetTeamByCodeAsync(string code)
        {
            await RefreshCacheIfNeeded();
            return _cachedTeams.FirstOrDefault(t => 
                string.Equals(t.Code, code, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets all team categories
        /// </summary>
        public async Task<string[]> GetTeamCategoriesAsync()
        {
            await RefreshCacheIfNeeded();
            return _cachedTeams
                .Select(t => t.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToArray();
        }
    }
}