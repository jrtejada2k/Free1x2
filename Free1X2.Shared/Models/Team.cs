using System;

namespace Free1X2.Shared.Models
{
    /// <summary>
    /// Represents a football team in the system
    /// </summary>
    public record Team
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Code { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public bool IsActive { get; init; } = true;
        
        public Team() { }
        
        public Team(int id, string name, string code, string category = "")
        {
            Id = id;
            Name = name ?? string.Empty;
            Code = code ?? string.Empty;
            Category = category ?? string.Empty;
        }
    }
}