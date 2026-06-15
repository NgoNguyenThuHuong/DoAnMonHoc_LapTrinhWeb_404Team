using LingoToneMVC.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LingoToneMVC.Services
{
    public class GeminiAiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GeminiAiService> _logger;
        private readonly IMemoryCache _cache;

        public GeminiAiService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiAiService> logger, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(15); // Global timeout
            _configuration = configuration;
            _logger = logger;
            _cache = cache;
        }

        public async Task<AiResult> CallAiAsync(string prompt, int maxTokens = 800, string cacheKey = null)
        {
            if (!string.IsNullOrEmpty(cacheKey) && _cache.TryGetValue(cacheKey, out string cachedContent))
            {
                return new AiResult { Success = true, Content = cachedContent, IsFallback = false };
            }

            var apiKey = _configuration["GeminiApiKey"];
            if (string.IsNullOrEmpty(apiKey) || apiKey == "PASTE_YOUR_GEMINI_API_KEY_HERE")
            {
                _logger.LogWarning("Gemini API Key is missing or invalid.");
                return new AiResult { Success = false, ErrorMessage = "API Key chưa được cấu hình.", IsFallback = true };
            }

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                },
                generationConfig = new
                {
                    temperature = 0.7,
                    maxOutputTokens = maxTokens,
                    responseMimeType = "application/json"
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

            int maxRetries = 2;
            for (int i = 0; i <= maxRetries; i++)
            {
                try
                {
                    var response = await _httpClient.PostAsync(url, content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var doc = JsonDocument.Parse(responseString);
                        var aiText = doc.RootElement
                                        .GetProperty("candidates")[0]
                                        .GetProperty("content")
                                        .GetProperty("parts")[0]
                                        .GetProperty("text")
                                        .GetString();

                        if (!string.IsNullOrEmpty(cacheKey) && !string.IsNullOrEmpty(aiText))
                        {
                            _cache.Set(cacheKey, aiText, TimeSpan.FromHours(1)); // Cache for 1 hour
                        }

                        return new AiResult { Success = true, Content = aiText, IsFallback = false };
                    }
                    
                    var statusCode = (int)response.StatusCode;
                    if (statusCode == 429 || statusCode == 503)
                    {
                        _logger.LogWarning($"Gemini API Overloaded ({statusCode}). Attempt {i + 1} of {maxRetries + 1}.");
                        if (i == maxRetries) break;
                        await Task.Delay(1500 * (i + 1));
                        continue;
                    }

                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Gemini API Error {statusCode}: {errorContent}");
                    return new AiResult { Success = false, ErrorMessage = $"Lỗi từ Google API: {response.StatusCode}", IsFallback = true };
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogWarning($"Gemini API Timeout. Attempt {i + 1} of {maxRetries + 1}. Error: {ex.Message}");
                    if (i == maxRetries) break;
                    await Task.Delay(1500 * (i + 1));
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError($"Gemini API HttpRequestException: {ex.Message}");
                    if (i == maxRetries) break;
                    await Task.Delay(1500 * (i + 1));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Gemini API Unexpected Exception: {ex.Message}");
                    return new AiResult { Success = false, ErrorMessage = "Lỗi hệ thống.", IsFallback = true };
                }
            }

            return new AiResult { Success = false, ErrorMessage = "Hệ thống AI đang quá tải. Vui lòng thử lại sau.", IsFallback = true };
        }

        public async Task<AiResult> CheckGrammarAsync(string sentence)
        {
            var prompt = $@"Kiểm tra ngữ pháp câu tiếng Trung sau: '{sentence}'.
Trả về ĐÚNG 1 CHUỖI JSON, không có markdown.
{{
  ""isCorrect"": true,
  ""errors"": [""lỗi 1"", ""lỗi 2""],
  ""correction"": ""câu sửa lại cho đúng và tự nhiên"",
  ""translation"": ""bản dịch tiếng Việt"",
  ""explanation"": ""giải thích lỗi sai""
}}";
            string cacheKey = $"grammar:{sentence.GetHashCode()}";
            return await CallAiAsync(prompt, 500, cacheKey);
        }

        public async Task<AiResult> EvaluateWritingAsync(string text)
        {
            var prompt = $@"Đóng vai là giám khảo HSK. Hãy chấm điểm đoạn văn tiếng Trung này theo thang điểm 100:
'{text}'
Trả về JSON. Không markdown. Chú ý: correctedHtml phải dùng thẻ <span class='err'> cho từ sai (nếu có) và <span class='corr'> cho từ sửa lại (nếu có).
{{
  ""totalScore"": 88,
  ""grammarScore"": 90,
  ""vocabularyScore"": 85,
  ""coherenceScore"": 90,
  ""naturalnessScore"": 87,
  ""estimatedHsk"": ""HSK 2"",
  ""feedback"": ""Tuyệt vời! Bài viết của bạn rất tốt! Hãy tiếp tục phát huy nhé."",
  ""correctedHtml"": ""我叫小明，<span class='err'>我今年二十岁</span><span class='corr'>我是学生</span>。..."",
  ""translatedText"": ""Tôi tên là Tiểu Minh..."",
  ""suggestions"": [
    ""Sử dụng thêm liên từ..."",
    ""Thử dùng các cấu trúc...""
  ],
  ""errorAnalysis"": [
    {{ ""original"": ""我今年二十岁"", ""corrected"": ""我是学生"", ""explanation"": ""Lỗi logic trong ngữ cảnh."" }}
  ],
  ""advancedVocabulary"": [
    {{ ""word"": ""热情"", ""pinyin"": ""rè qíng"", ""meaning"": ""Nhiệt tình"" }}
  ]
}}";
            string cacheKey = $"writing:{text.GetHashCode()}";
            return await CallAiAsync(prompt, 800, cacheKey);
        }

        public async Task<AiResult> EstimateHskLevelAsync(string text)
        {
            var prompt = $@"Dựa vào đoạn tiếng Trung sau, hãy đánh giá trình độ HSK của người viết (thang điểm 100 cho mỗi kỹ năng):
'{text}'
Trả về JSON. Không markdown.
{{
  ""estimatedLevel"": ""HSK 2"",
  ""levelMessage"": ""Bạn đã nắm vững kiến thức cơ bản. Hãy tiếp tục luyện tập để đạt HSK 3!"",
  ""listeningScore"": 65,
  ""vocabScore"": 72,
  ""grammarScore"": 60,
  ""readingScore"": 68,
  ""strengths"": [
    ""Nhận biết từ vựng chủ đề quen thuộc"",
    ""Hiểu câu đơn và câu ghép cơ bản""
  ],
  ""weaknesses"": [
    ""Ngữ pháp phức tạp (把, 被, 使...)"",
    ""Từ vựng học thuật và chủ đề nâng cao""
  ],
  ""aiAdvice"": ""Bạn đang ở mức HSK 2, đây là nền tảng tốt! Hãy học mỗi ngày 30 phút...""
}}";
            string cacheKey = $"hskestimate:{text.GetHashCode()}";
            return await CallAiAsync(prompt, 400, cacheKey);
        }

        public async Task<AiResult> TranslateVocabularyAsync(string englishText)
        {
            if (string.IsNullOrWhiteSpace(englishText)) return new AiResult { Success = false, ErrorMessage = "Dữ liệu đầu vào trống.", IsFallback = false };

            var prompt = $@"Dịch các nghĩa tiếng Anh của từ vựng này sang tiếng Việt ngắn gọn, cách nhau bằng dấu phẩy:
'{englishText}'
Chỉ trả về JSON format: {{ ""vietnamese"": ""Nghĩa tiếng Việt 1, Nghĩa tiếng Việt 2"" }}";
            string cacheKey = $"translate:{englishText.GetHashCode()}";
            return await CallAiAsync(prompt, 500, cacheKey);
        }

        public async Task<AiResult> RoleplayReplyAsync(string scenario, string history, string userMessage)
        {
            var prompt = $@"Bạn là một AI đóng vai trò người bản xứ luyện hội thoại tiếng Trung.
Tình huống hiện tại: {scenario}

Lịch sử hội thoại:
{history}

Người dùng vừa nói: '{userMessage}'

Nhiệm vụ:
1. Nếu câu của người dùng có lỗi ngữ pháp, không tự nhiên, hoặc có từ vựng tốt hơn, hãy viết nhận xét/gợi ý sửa lỗi vào 'correction'. Nếu câu hoàn hảo, hãy để 'correction' là null.
2. Đóng vai nhân vật trong tình huống và trả lời lại người dùng một cách tự nhiên bằng tiếng Trung để tiếp tục đoạn hội thoại.

Trả về JSON ĐÚNG CẤU TRÚC, không chứa Markdown:
{{
  ""correction"": ""Câu của bạn khá tốt. Tuy nhiên, thay vì nói X, bạn có thể nói Y để tự nhiên hơn."",
  ""zh"": ""Câu trả lời của bạn bằng tiếng Trung"",
  ""py"": ""Pinyin của câu trả lời"",
  ""vi"": ""Bản dịch tiếng Việt của câu trả lời""
}}";
            return await CallAiAsync(prompt, 300); // Roleplay usually doesn't cache well because of changing history
        }

        public async Task<AiResult> GenerateHanziStoryAsync(string character)
        {
            var prompt = $@"
Đóng vai một chuyên gia Hán ngữ. Hãy phân tích chữ Hán '{character}'.
1. Tách chữ này thành các bộ thủ (nếu có thể).
2. Sáng tạo một câu chuyện (Mnemonic story) thật ngắn gọn, dễ nhớ (khoảng 2-3 câu) liên kết ý nghĩa của các bộ thủ để giải thích ý nghĩa của chữ này.
3. Giải thích ngắn gọn chữ này trong tiếng Việt.

Chỉ trả về chuỗi văn bản thuần túy (có thể dùng HTML tags cơ bản như <b>, <br> nếu cần). Không dùng markdown code blocks.
Ví dụ nếu chữ là '好':
<b>Phân tích:</b> Chữ này ghép từ bộ Nữ (nữ giới) và bộ Tử (con trai).<br><br><b>Mẹo nhớ:</b> Người phụ nữ có con trai thì đó là điều vô cùng <b>tốt đẹp (Hảo)</b>.<br><br><b>Nghĩa:</b> Tốt, đẹp, khỏe.
";
            string cacheKey = $"hanzi:{character}";
            return await CallAiAsync(prompt, 300, cacheKey);
        }

        public async Task<AiResult> GenerateCareerRoadmapAsync(string career, string level, string goal, int minutesPerDay)
        {
            var prompt = $@"Bạn là một giáo viên tiếng Trung chuyên nghiệp. Hãy thiết kế lộ trình học tiếng Trung cho ngành {career}.
Trình độ hiện tại: {(level ?? "Cơ bản")}.
Mục tiêu: {(goal ?? "Giao tiếp công việc")}.
Thời gian học: {(minutesPerDay > 0 ? minutesPerDay : 20)} phút/ngày.
Hãy trả về ĐÚNG 1 CHUỖI JSON THEO ĐỊNH DẠNG DƯỚI ĐÂY, không có markdown.
{{
  ""roadmap"": [""Tuần 1: Từ vựng cơ bản ngành {career}"", ""Tuần 2: Mẫu câu giao tiếp...""],
  ""dailyVocabulary"": [
    {{ ""char"": ""chữ hán 1"", ""pinyin"": ""pinyin 1"", ""meaning"": ""nghĩa 1"" }},
    {{ ""char"": ""chữ hán 2"", ""pinyin"": ""pinyin 2"", ""meaning"": ""nghĩa 2"" }}
  ],
  ""roleplayScenario"": ""Tình huống mẫu:..."",
  ""quiz"": [""câu hỏi ôn tập 1"", ""câu hỏi ôn tập 2""]
}}";
            string cacheKey = $"roadmap:{career}_{level}_{goal}_{minutesPerDay}".GetHashCode().ToString();
            return await CallAiAsync(prompt, 800, cacheKey);
        }

        public async Task<AiResult> GenerateQuizAsync(string topic, string level, int count)
        {
            var prompt = $@"Tạo {count} câu hỏi trắc nghiệm tiếng Trung cấp độ {level} về chủ đề '{(topic ?? "giao tiếp hằng ngày")}'.
Trả về JSON. Không markdown.
{{
  ""questions"": [
    {{
      ""question"": ""câu hỏi bằng tiếng trung hoặc tiếng việt"",
      ""options"": [""A"", ""B"", ""C"", ""D""],
      ""correctIndex"": 0,
      ""explanation"": ""Giải thích đáp án bằng tiếng Việt""
    }}
  ]
}}";
            string cacheKey = $"quiz:{topic}_{level}_{count}".GetHashCode().ToString();
            return await CallAiAsync(prompt, 600, cacheKey);
        }
    }
}
