using System.Collections.Generic;
namespace X00201142_CA3.Models;

public class AnimalWord
{
    public string? Id { get; set; }
    public string? Word { get; set; }
    public string? Category { get; set; }
    public int NumLetters { get; set; }
    public int NumSyllables { get; set; }
    public string? Hint { get; set; }
}

public class ApiResponse
{
    public List<AnimalWord> Words { get; set; } = new();
}