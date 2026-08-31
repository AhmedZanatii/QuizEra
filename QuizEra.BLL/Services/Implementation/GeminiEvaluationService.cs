using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

public class GeminiEvaluationService : IAIEvaluationService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GeminiEvaluationService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["AI:GeminiAPI"]
                  ?? throw new ArgumentNullException("Gemini API Key is missing in appsettings.json");
    }

    public async Task<(int Grade, string Justification, bool IsCorrect)> EvaluateEssayAsync(
        string question, string correct, string student, int maxMarks)
    {
        var model = "gemini-3.6-flash";
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={_apiKey}";

        var prompt = $"Grade this exam out of {maxMarks}. Question: {question}. Ideal Answer: {correct}. Student's Answer: {student}. Return strictly JSON: {{\"Grade\": int, \"IsCorrect\": bool, \"Justification\": \"string\"}}";

        var payload = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        try
        {
            var response = await _httpClient.PostAsync(url, content);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return (0, $"AI Evaluation failed: {response.StatusCode}. {responseText}", false);
            }

            using var doc = JsonDocument.Parse(responseText);
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrWhiteSpace(text))
                return (0, "AI Evaluation failed: empty response.", false);

            var cleanJson = text
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            var result = JsonSerializer.Deserialize<AIGradeResult>(cleanJson);
            if (result == null)
                return (0, "AI Evaluation failed: invalid response.", false);

            return (result.Grade, result.Justification, result.IsCorrect);
        }
        catch (Exception ex)
        {
            return (0, $"AI Evaluation failed: {ex.Message}", false);
        }
    }

    private class AIGradeResult
    {
        public int Grade { get; set; }
        public bool IsCorrect { get; set; }
        public string Justification { get; set; } = string.Empty;
    }
}