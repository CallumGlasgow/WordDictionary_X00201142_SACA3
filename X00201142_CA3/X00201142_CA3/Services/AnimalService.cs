using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json;
using X00201142_CA3.Models;

namespace X00201142_CA3.Services;

public class AnimalService
{
    private readonly HttpClient _http;
    // for each imageurl, null = loading, "" means none found. Used ConcurrentDictionary since good for concurrent read/writes
    private readonly ConcurrentDictionary<string, string?> imageUrls = new();

    public AnimalService(HttpClient http) 
    { 
        _http = http;
    }

    public IReadOnlyDictionary<string, string?> ImageUrls => imageUrls;

    public event Action<string>? OnImageLoaded;

    public async Task<List<AnimalWord>> LoadAnimalsAsync()
    {
        try
        {
            var response = await _http.GetFromJsonAsync<ApiResponse>(
                "https://www.wordgamedb.com/api/v2/words?category=animal&limit=35");
            return response?.Words ?? new List<AnimalWord>();
        }
        catch
        {
            return new List<AnimalWord>();
        }
    }

    public void EnsureImageLoaded(string? word)
    {
        if (string.IsNullOrWhiteSpace(word)) return;
        var key = word.Trim();
        if (imageUrls.ContainsKey(key)) return;

        imageUrls.TryAdd(key, null);
        _ = LoadAndStoreImageAsync(key);
    }

    private async Task LoadAndStoreImageAsync(string word)
    {
        try
        {
            var url = $"https://en.wikipedia.org/api/rest_v1/page/summary/{Uri.EscapeDataString(word)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
            {
                imageUrls[word] = string.Empty;
                OnImageLoaded?.Invoke(word);
                return;
            }

            using var doc = await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync());
            if (doc.RootElement.TryGetProperty("thumbnail", out var thumb) &&
                thumb.TryGetProperty("source", out var src))
            {
                imageUrls[word] = src.GetString() ?? string.Empty;
            }
            else
            {
                imageUrls[word] = string.Empty;
            }
            OnImageLoaded?.Invoke(word);
        }
        catch
        {
            imageUrls[word] = string.Empty;
            OnImageLoaded?.Invoke(word);

        }
    }
}
