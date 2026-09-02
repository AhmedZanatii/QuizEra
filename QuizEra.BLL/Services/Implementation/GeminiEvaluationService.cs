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

        var gradeJsonFormat = $@"{{""Grade"": <int from 0 to {maxMarks}>, ""IsCorrect"": <bool>, ""Justification"": ""<text>""}}";;

        var prompt = $@"You are an expert exam grader. Grade this essay question using PARTIAL CREDIT grading (like a human teacher would).

        QUESTION: {question}

        IDEAL ANSWER: {correct}

        STUDENT'S ANSWER: {student}

        GRADING GUIDELINES:
        - Maximum marks: {maxMarks}
        - Give full marks ({maxMarks}) ONLY if the student's answer is essentially complete and correct
        - Give partial credit for answers that have:
        * All major/critical components but with minor errors or less detail
        * Most critical points but missing some supporting details
        * Correct core concepts but incomplete explanations

        DEDUCTION RULES:
        - MAJOR DEDUCTION (50-80% point loss): Missing critical/essential information that directly answers the question
        - MINOR DEDUCTION (10-30% point loss): Missing helpful details, incomplete explanations (if the question requires it), or minor errors
        - NO DEDUCTION: Missing information that is:
        * Deducible from what's stated
        * Nice-to-have but not essential
        * Contextual/background information not core to answering

        ANALYSIS STEPS:
        1. Identify key components in the ideal answer
        2. Check which components the student covered
        3. Assess accuracy of covered components
        4. Determine if missing parts are critical or supporting
        5. Calculate grade based on coverage and accuracy

        IMPORTANT: Ensure Grade is an integer between 0 and {maxMarks}. Set IsCorrect to true only if grade is 50%+ of maxMarks.

        Return ONLY valid JSON (no markdown, no triple backticks):
        {gradeJsonFormat}";

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