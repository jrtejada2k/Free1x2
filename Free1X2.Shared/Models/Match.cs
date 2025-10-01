using System;

namespace Free1X2.Shared.Models
{
    /// <summary>
    /// Represents a football match between two teams
    /// </summary>
    public record Match
    {
        public int Id { get; init; }
        public Team HomeTeam { get; init; } = new();
        public Team AwayTeam { get; init; } = new();
        public DateTime MatchDate { get; init; }
        public int MatchDay { get; init; }
        public string Season { get; init; } = string.Empty;
        public MatchResult? Result { get; init; }
        public MatchPrediction? Prediction { get; init; }
        
        public Match() { }
        
        public Match(int id, Team homeTeam, Team awayTeam, DateTime matchDate, int matchDay, string season)
        {
            Id = id;
            HomeTeam = homeTeam;
            AwayTeam = awayTeam;
            MatchDate = matchDate;
            MatchDay = matchDay;
            Season = season ?? string.Empty;
        }
        
        /// <summary>
        /// Gets whether this match has been played (has result)
        /// </summary>
        public bool IsPlayed => Result != null;
        
        /// <summary>
        /// Gets whether this match has a prediction
        /// </summary>
        public bool HasPrediction => Prediction != null;
    }
    
    /// <summary>
    /// Represents the result of a played match
    /// </summary>
    public record MatchResult
    {
        public int HomeScore { get; init; }
        public int AwayScore { get; init; }
        public string Outcome { get; init; } = string.Empty; // "1", "X", "2"
        
        public MatchResult() { }
        
        public MatchResult(int homeScore, int awayScore)
        {
            HomeScore = homeScore;
            AwayScore = awayScore;
            Outcome = CalculateOutcome(homeScore, awayScore);
        }
        
        private static string CalculateOutcome(int homeScore, int awayScore)
        {
            if (homeScore > awayScore) return "1";
            if (homeScore < awayScore) return "2";
            return "X";
        }
    }
    
    /// <summary>
    /// Represents a prediction for a match outcome
    /// </summary>
    public record MatchPrediction
    {
        public double Probability1 { get; init; } // Home win probability
        public double ProbabilityX { get; init; } // Draw probability
        public double Probability2 { get; init; } // Away win probability
        public string RecommendedOutcome { get; init; } = string.Empty;
        public double Confidence { get; init; }
        
        public MatchPrediction() { }
        
        public MatchPrediction(double prob1, double probX, double prob2)
        {
            Probability1 = prob1;
            ProbabilityX = probX;
            Probability2 = prob2;
            RecommendedOutcome = GetRecommendedOutcome(prob1, probX, prob2);
            Confidence = Math.Max(Math.Max(prob1, probX), prob2);
        }
        
        private static string GetRecommendedOutcome(double prob1, double probX, double prob2)
        {
            if (prob1 >= probX && prob1 >= prob2) return "1";
            if (prob2 >= probX && prob2 >= prob1) return "2";
            return "X";
        }
    }
}