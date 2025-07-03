using System.Globalization;
using CsvHelper;
using Scriban;
using System.Text.Json;

public class CanonicalRecord
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime Joined { get; set; }
    public int Age { get; set; }
    public double Score { get; set; }
    public string Region { get; set; }
    public bool Active { get; set; }
    public DateTime LastSeen { get; set; }
}