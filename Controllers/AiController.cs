using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace LingoToneMVC.Controllers
{
    public class AiController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AiController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        private async Task<(bool success, string responseText, string errorMessage)> CallGeminiApiAsync(string prompt, int maxTokens = 800)
        {
            var apiKey = _configuration["GeminiApiKey"];
            if (string.IsNullOrEmpty(apiKey) || apiKey == "PASTE_YOUR_GEMINI_API_KEY_HERE")
            {
                return (false, null, "API Key chưa được cấu hình.");
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

            int maxRetries = 3;
            for (int i = 0; i < maxRetries; i++)
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
                        return (true, aiText, null);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        if (i == maxRetries - 1)
                        {
                            return (false, null, "Hệ thống AI đang quá tải (Too Many Requests). Vui lòng thử lại sau ít phút.");
                        }
                        await Task.Delay(2000 * (i + 1)); // Exponential backoff: 2s, 4s...
                        continue;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return (false, null, $"Lỗi từ Google API: {response.StatusCode}");
                    }
                }
                catch (System.Exception ex)
                {
                    if (i == maxRetries - 1)
                    {
                        return (false, null, $"Lỗi hệ thống: {ex.Message}");
                    }
                    await Task.Delay(2000 * (i + 1));
                }
            }

            return (false, null, "Đã thử nhiều lần nhưng không thành công.");
        }

        [HttpGet]
        public async Task<IActionResult> AnalyzeHanzi(string character)
        {
            if (string.IsNullOrEmpty(character))
            {
                return BadRequest("Vui lòng cung cấp chữ Hán.");
            }

            var prompt = $@"Bạn là một chuyên gia về Hán tự và từ nguyên học. 
Nhiệm vụ của bạn là phân tích chữ Hán: '{character}'.
Hãy phân tích rõ chữ này được ghép từ những bộ thủ nào, và sáng tác một câu chuyện ngắn gọn (dưới 50 chữ) để người học dễ ghi nhớ mặt chữ.
Trả về KẾT QUẢ DUY NHẤT LÀ MỘT CHUỖI JSON ĐÚNG ĐỊNH DẠNG. Tuyệt đối không thêm markdown, không thêm text giải thích.
Cấu trúc JSON:
{{
  ""analysis"": ""Ghép từ bộ X và bộ Y..."",
  ""story"": ""Câu chuyện ghi nhớ...""
}}";

            var (success, aiText, error) = await CallGeminiApiAsync(prompt, 300);

            if (success)
            {
                try
                {
                    var resultJson = JsonDocument.Parse(aiText);
                    return Json(new
                    {
                        success = true,
                        analysis = resultJson.RootElement.GetProperty("analysis").GetString(),
                        story = resultJson.RootElement.GetProperty("story").GetString()
                    });
                }
                catch
                {
                    return Json(new { success = false, analysis = "Lỗi khi đọc kết quả từ AI.", story = aiText });
                }
            }

            return Json(new { success = false, analysis = "Lỗi", story = error });
        }

        public class CareerRoadmapRequest
        {
            public string career { get; set; }
            public string level { get; set; }
            public string goal { get; set; }
            public int minutesPerDay { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateCareerRoadmap([FromBody] CareerRoadmapRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.career)) return BadRequest();

            var prompt = $@"Bạn là một giáo viên tiếng Trung chuyên nghiệp. Hãy thiết kế lộ trình học tiếng Trung cho ngành {req.career}.
Trình độ hiện tại: {(req.level ?? "Cơ bản")}.
Mục tiêu: {(req.goal ?? "Giao tiếp công việc")}.
Thời gian học: {(req.minutesPerDay > 0 ? req.minutesPerDay : 20)} phút/ngày.
Hãy trả về ĐÚNG 1 CHUỖI JSON THEO ĐỊNH DẠNG DƯỚI ĐÂY, không có markdown.
{{
  ""roadmap"": [""Tuần 1: Từ vựng cơ bản ngành {req.career}"", ""Tuần 2: Mẫu câu giao tiếp...""],
  ""dailyVocabulary"": [
    {{ ""char"": ""chữ hán 1"", ""pinyin"": ""pinyin 1"", ""meaning"": ""nghĩa 1"" }},
    {{ ""char"": ""chữ hán 2"", ""pinyin"": ""pinyin 2"", ""meaning"": ""nghĩa 2"" }}
  ],
  ""roleplayScenario"": ""Tình huống mẫu:..."",
  ""quiz"": [""câu hỏi ôn tập 1"", ""câu hỏi ôn tập 2""]
}}";

            var (success, aiText, error) = await CallGeminiApiAsync(prompt, 800);
            if (success)
            {
                try { return Content(aiText, "application/json"); } catch { return Json(new { success = false, message = "Format lỗi" }); }
            }
            return Json(new { success = false, message = error });
        }

        public class CorrectGrammarRequest { public string sentence { get; set; } }

        [HttpPost]
        public async Task<IActionResult> CorrectGrammar([FromBody] CorrectGrammarRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.sentence)) return BadRequest();

            var prompt = $@"Kiểm tra ngữ pháp câu tiếng Trung sau: '{req.sentence}'.
Trả về ĐÚNG 1 CHUỖI JSON, không có markdown.
{{
  ""isCorrect"": true,
  ""errors"": [""lỗi 1"", ""lỗi 2""],
  ""correction"": ""câu sửa lại cho đúng và tự nhiên"",
  ""translation"": ""bản dịch tiếng Việt"",
  ""explanation"": ""giải thích lỗi sai""
}}";

            var (success, aiText, error) = await CallGeminiApiAsync(prompt, 500);
            if (success) return Content(aiText, "application/json");
            return Json(new { success = false, message = error });
        }

        public class GenerateQuizRequest { public string topic { get; set; } public string level { get; set; } public int count { get; set; } }

        [HttpPost]
        public async Task<IActionResult> GenerateQuiz([FromBody] GenerateQuizRequest req)
        {
            if (req == null) return BadRequest();
            int count = req.count > 0 ? req.count : 5;
            string level = req.level ?? "HSK2";

            var prompt = $@"Tạo {count} câu hỏi trắc nghiệm tiếng Trung cấp độ {level} về chủ đề '{(req.topic ?? "giao tiếp hằng ngày")}'.
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
            var (success, aiText, error) = await CallGeminiApiAsync(prompt, 600);
            if (success) return Content(aiText, "application/json");

            // Fallback
            var fallbackJson = $@"{{
              ""questions"": [
                {{
                  ""question"": ""Hệ thống đang quá tải, đây là câu hỏi mẫu. {req.topic} - Câu 1: (Chọn A)"",
                  ""options"": [""A"", ""B"", ""C"", ""D""],
                  ""correctIndex"": 0,
                  ""explanation"": ""Đây là lỗi quá tải API.""
                }},
                {{
                  ""question"": ""{req.topic} - Câu 2: (Chọn B)"",
                  ""options"": [""A"", ""B"", ""C"", ""D""],
                  ""correctIndex"": 1,
                  ""explanation"": ""Đây là lỗi quá tải API.""
                }}
              ]
            }}";
            return Content(fallbackJson, "application/json");
        }

        public class TranslateRequest { public string englishText { get; set; } }

        [HttpPost]
        public async Task<IActionResult> TranslateEngToVie([FromBody] TranslateRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.englishText)) return BadRequest();

            var prompt = $@"Dịch các nghĩa tiếng Anh của từ vựng này sang tiếng Việt ngắn gọn, cách nhau bằng dấu phẩy:
'{req.englishText}'
Trả về JSON. Không markdown:
{{
  ""vietnamese"": ""Nghĩa tiếng Việt 1, Nghĩa tiếng Việt 2""
}}";
            var (success, aiText, error) = await CallGeminiApiAsync(prompt, 500);
            if (success) return Content(aiText, "application/json");

            return Json(new { vietnamese = req.englishText + " (Hệ thống AI đang bận)" });
        }

        public class EvaluateWritingRequest { public string text { get; set; } }

        [HttpPost]
        public async Task<IActionResult> EvaluateWriting([FromBody] EvaluateWritingRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.text)) return BadRequest();

            var prompt = $@"Đóng vai là giám khảo HSK. Hãy chấm điểm đoạn văn tiếng Trung này theo thang điểm 100:
'{req.text}'
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
            var (success, aiText, error) = await CallGeminiApiAsync(prompt, 800);
            if (success) return Content(aiText, "application/json");

            // Fallback for demo when API limits are reached
            string safeText = req.text.Replace("\"", "\\\"").Replace("\n", " ").Replace("\r", "");
            var fallbackJson = $@"{{
              ""totalScore"": 88,
              ""grammarScore"": 90,
              ""vocabularyScore"": 85,
              ""coherenceScore"": 90,
              ""naturalnessScore"": 87,
              ""estimatedHsk"": ""HSK 2"",
              ""feedback"": ""Tuyệt vời! Bài viết của bạn rất tốt! Hãy tiếp tục phát huy nhé. (Kết quả mô phỏng do API quá tải)"",
              ""correctedHtml"": ""{safeText} <span class='corr' style='color:#10b981;'>(Phần này là mô phỏng do API quá tải, xin vui lòng thử lại sau)</span>"",
              ""translatedText"": ""(Bản dịch mô phỏng) Đoạn văn của bạn có nội dung rất thú vị. Do kết nối AI đang quá tải nên chúng tôi không thể dịch chính xác từng câu chữ."",
              ""suggestions"": [
                ""Sử dụng thêm liên từ để câu văn liên kết mượt mà hơn."",
                ""Thử dùng các cấu trúc ngữ pháp phức tạp hơn như '虽然...但是...', '如果...就...'.""
              ],
              ""errorAnalysis"": [
                {{ ""original"": ""(Lỗi mẫu)"", ""corrected"": ""(Sửa mẫu)"", ""explanation"": ""Hệ thống đang quá tải, đây là dữ liệu mô phỏng."" }}
              ],
              ""advancedVocabulary"": [
                {{ ""word"": ""词汇"", ""pinyin"": ""cí huì"", ""meaning"": ""Từ vựng"" }},
                {{ ""word"": ""模拟"", ""pinyin"": ""mó nǐ"", ""meaning"": ""Mô phỏng"" }}
              ]
            }}";
            return Content(fallbackJson, "application/json");
        }

        public class EstimateHskLevelRequest { public string text { get; set; } }

        [HttpPost]
        public async Task<IActionResult> EstimateHskLevel([FromBody] EstimateHskLevelRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.text)) return BadRequest();

            var prompt = $@"Dựa vào đoạn tiếng Trung sau, hãy đánh giá trình độ HSK của người viết (thang điểm 100 cho mỗi kỹ năng):
'{req.text}'
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
            var (success, aiText, error) = await CallGeminiApiAsync(prompt, 400);
            if (success) return Content(aiText, "application/json");

            // Fallback for demo when API limits are reached
            var fallbackJson = @"{
              ""estimatedLevel"": ""HSK 2"",
              ""levelMessage"": ""Bạn đã nắm vững kiến thức cơ bản. Hãy tiếp tục luyện tập để đạt HSK 3! (Kết quả mô phỏng do API quá tải)"",
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
              ""aiAdvice"": ""Bạn đang ở mức HSK 2, đây là nền tảng tốt! Hãy học mỗi ngày 30 phút, tập trung vào việc mở rộng từ vựng và luyện nghe.""
            }";
            return Content(fallbackJson, "application/json");
        }

        public class ChatMessage { public string role { get; set; } public string content { get; set; } }
        public class ChatRoleplayRequest { public string scenario { get; set; } public List<ChatMessage> history { get; set; } public string userMessage { get; set; } }

        [HttpPost]
        public async Task<IActionResult> ChatRoleplay([FromBody] ChatRoleplayRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.userMessage)) return BadRequest();

            string historyString = "";
            if (req.history != null && req.history.Count > 0)
            {
                foreach (var msg in req.history)
                {
                    historyString += $"{msg.role.ToUpper()}: {msg.content}\n";
                }
            }

            var prompt = $@"Bạn là một AI đóng vai trò người bản xứ luyện hội thoại tiếng Trung.
Tình huống hiện tại: {req.scenario}

Lịch sử hội thoại:
{historyString}

Người dùng vừa nói: '{req.userMessage}'

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
            var (success, aiText, error) = await CallGeminiApiAsync(prompt, 300);
            if (success) return Content(aiText, "application/json");

            // Fallback
            var fallbackJson = $@"{{
                ""correction"": null,
                ""zh"": ""Hệ thống AI đang quá tải. Hãy thử lại câu này nhé."",
                ""py"": ""Xìtǒng AI dāng guòzài. Hǎi shì lì zhè gè ba."",
                ""vi"": ""Hệ thống đang quá tải. Vui lòng gửi lại câu vừa rồi.""
            }}";
            return Content(fallbackJson, "application/json");
        }

        public class HanziStoryRequest
        {
            public string character { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateHanziStory([FromBody] HanziStoryRequest req)
        {
            if (string.IsNullOrWhiteSpace(req?.character))
            {
                return Json(new { success = false, message = "Character is required" });
            }

            var prompt = $@"
Đóng vai một chuyên gia Hán ngữ. Hãy phân tích chữ Hán '{req.character}'.
1. Tách chữ này thành các bộ thủ (nếu có thể).
2. Sáng tạo một câu chuyện (Mnemonic story) thật ngắn gọn, dễ nhớ (khoảng 2-3 câu) liên kết ý nghĩa của các bộ thủ để giải thích ý nghĩa của chữ này.
3. Giải thích ngắn gọn chữ này trong tiếng Việt.

Chỉ trả về chuỗi văn bản thuần túy (có thể dùng HTML tags cơ bản như <b>, <br> nếu cần). Không dùng markdown code blocks.
Ví dụ nếu chữ là '好':
<b>Phân tích:</b> Chữ này ghép từ bộ Nữ (nữ giới) và bộ Tử (con trai).<br><br><b>Mẹo nhớ:</b> Người phụ nữ có con trai thì đó là điều vô cùng <b>tốt đẹp (Hảo)</b>.<br><br><b>Nghĩa:</b> Tốt, đẹp, khỏe.
";

            var (success, aiText, error) = await CallGeminiApiAsync(prompt, 300);

            if (success)
            {
                // Remove markdown code blocks if gemini still adds them
                var cleanText = aiText.Replace("```html", "").Replace("```", "").Trim();
                return Json(new { success = true, story = cleanText });
            }

            return Json(new { success = false, message = error ?? "Lỗi gọi AI" });
        }
    }
}
