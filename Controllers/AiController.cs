using LingoToneMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace LingoToneMVC.Controllers
{
    public class AiController : Controller
    {
        private readonly IAiService _aiService;

        public AiController(IAiService aiService)
        {
            _aiService = aiService;
        }

        [HttpGet]
        public async Task<IActionResult> AnalyzeHanzi(string character)
        {
            if (string.IsNullOrEmpty(character)) return BadRequest("Vui lòng cung cấp chữ Hán.");

            var result = await _aiService.GenerateHanziStoryAsync(character);
            if (result.Success && !result.IsFallback)
            {
                // Due to how we defined the prompt in service vs original controller, 
                // the new GenerateHanziStoryAsync returns direct text, but old AnalyzeHanzi returned JSON with analysis & story.
                // However, I see GenerateHanziStory is also present. Let's adapt the old AnalyzeHanzi to use the direct text format but wrapped in json.
                return Json(new { success = true, analysis = "Phân tích:", story = result.Content, isFallback = false });
            }

            return Json(new { success = false, analysis = "Lỗi", story = result.ErrorMessage, isFallback = true });
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

            var result = await _aiService.GenerateCareerRoadmapAsync(req.career, req.level, req.goal, req.minutesPerDay);
            if (result.Success) return Content(result.Content, "application/json");

            return Json(new { success = false, message = result.ErrorMessage, isFallback = result.IsFallback });
        }

        public class CorrectGrammarRequest { public string sentence { get; set; } }

        [HttpPost]
        public async Task<IActionResult> CorrectGrammar([FromBody] CorrectGrammarRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.sentence)) return BadRequest();

            var result = await _aiService.CheckGrammarAsync(req.sentence);
            if (result.Success) return Content(result.Content, "application/json");
            
            return Json(new { success = false, message = result.ErrorMessage, isFallback = result.IsFallback });
        }

        public class GenerateQuizRequest { public string topic { get; set; } public string level { get; set; } public int count { get; set; } }

        [HttpPost]
        public async Task<IActionResult> GenerateQuiz([FromBody] GenerateQuizRequest req)
        {
            if (req == null) return BadRequest();
            int count = req.count > 0 ? req.count : 5;
            string level = req.level ?? "HSK2";

            var result = await _aiService.GenerateQuizAsync(req.topic, level, count);
            if (result.Success && !result.IsFallback) return Content(result.Content, "application/json");

            // Fallback
            var fallbackJson = $@"{{
              ""isFallback"": true,
              ""questions"": [
                {{
                  ""question"": ""Hệ thống đang quá tải, đây là câu hỏi mẫu. {(req.topic ?? "giao tiếp hằng ngày")} - Câu 1: (Chọn A)"",
                  ""options"": [""A"", ""B"", ""C"", ""D""],
                  ""correctIndex"": 0,
                  ""explanation"": ""Đây là lỗi quá tải API.""
                }},
                {{
                  ""question"": ""{(req.topic ?? "giao tiếp hằng ngày")} - Câu 2: (Chọn B)"",
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

            var result = await _aiService.TranslateVocabularyAsync(req.englishText);
            if (result.Success && !result.IsFallback) return Content(result.Content, "application/json");

            return Json(new { vietnamese = req.englishText + " (Hệ thống AI đang bận)", isFallback = true });
        }

        public class EvaluateWritingRequest { public string text { get; set; } }

        [HttpPost]
        public async Task<IActionResult> EvaluateWriting([FromBody] EvaluateWritingRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.text)) return Json(new { success = false });

            var result = await _aiService.EvaluateWritingAsync(req.text);

            if (result.Success && !result.IsFallback)
            {
                try
                {
                    // Add isFallback flag to existing successful json string
                    var doc = JsonDocument.Parse(result.Content);
                    var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(doc.RootElement);
                    dict["isFallback"] = false;
                    dict["success"] = true;
                    return Json(dict);
                }
                catch
                {
                    return Content(result.Content, "application/json");
                }
            }

            // Fallback for demo when API limits are reached
            string safeText = req.text.Replace("\"", "\\\"").Replace("\n", " ").Replace("\r", "");
            var fallbackJson = $@"{{
              ""success"": true,
              ""isFallback"": true,
              ""totalScore"": 88,
              ""grammarScore"": 90,
              ""vocabularyScore"": 85,
              ""coherenceScore"": 90,
              ""naturalnessScore"": 87,
              ""estimatedHsk"": ""HSK 2"",
              ""feedback"": ""Tuyệt vời! Bài viết của bạn rất tốt! Hãy tiếp tục phát huy nhé."",
              ""correctedHtml"": ""{safeText}"",
              ""translatedText"": ""(Bản dịch mô phỏng) Đoạn văn của bạn có nội dung rất thú vị."",
              ""suggestions"": [
                ""Sử dụng thêm liên từ để câu văn liên kết mượt mà hơn."",
                ""Thử dùng các cấu trúc ngữ pháp phức tạp hơn.""
              ],
              ""errorAnalysis"": [],
              ""advancedVocabulary"": []
            }}";
            return Content(fallbackJson, "application/json");
        }

        public class EstimateHskLevelRequest { public string text { get; set; } }

        [HttpPost]
        public async Task<IActionResult> EstimateHskLevel([FromBody] EstimateHskLevelRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.text)) return BadRequest();

            var result = await _aiService.EstimateHskLevelAsync(req.text);
            if (result.Success && !result.IsFallback) return Content(result.Content, "application/json");

            // Fallback
            var fallbackJson = @"{
              ""isFallback"": true,
              ""estimatedLevel"": ""HSK 2"",
              ""levelMessage"": ""Bạn đã nắm vững kiến thức cơ bản. Hãy tiếp tục luyện tập để đạt HSK 3! (Kết quả mô phỏng)"",
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
              ""aiAdvice"": ""Bạn đang ở mức HSK 2, đây là nền tảng tốt!""
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

            var result = await _aiService.RoleplayReplyAsync(req.scenario, historyString, req.userMessage);
            if (result.Success && !result.IsFallback) return Content(result.Content, "application/json");

            // Contextual Fallback instead of generic message
            string fallbackZh = "好的。您想喝茶还是水？";
            string fallbackPy = "Hǎo de. Nín xiǎng hē chá háishì shuǐ?";
            string fallbackVi = "Vâng. Quý khách muốn uống trà hay nước?";
            
            if (req.scenario.Contains("mua sắm") || req.scenario.ToLower().Contains("shopping"))
            {
                fallbackZh = "这件衣服很适合你。你想试试吗？";
                fallbackPy = "Zhè jiàn yīfú hěn shìhé nǐ. Nǐ xiǎng shì shì ma?";
                fallbackVi = "Bộ đồ này rất hợp với bạn. Bạn có muốn thử không?";
            }
            else if (req.scenario.Contains("phỏng vấn") || req.scenario.ToLower().Contains("interview"))
            {
                fallbackZh = "很好。你对这个职位有什么问题吗？";
                fallbackPy = "Hěn hǎo. Nǐ duì zhège zhíwèi yǒu shénme wèntí ma?";
                fallbackVi = "Rất tốt. Bạn có câu hỏi gì về vị trí này không?";
            }

            var fallbackJson = $@"{{
                ""isFallback"": true,
                ""correction"": null,
                ""zh"": ""{fallbackZh}"",
                ""py"": ""{fallbackPy}"",
                ""vi"": ""{fallbackVi} (Dùng tạm do AI quá tải)""
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

            var result = await _aiService.GenerateHanziStoryAsync(req.character);

            if (result.Success && !result.IsFallback)
            {
                var cleanText = result.Content.Replace("```html", "").Replace("```", "").Trim();
                return Json(new { success = true, story = cleanText, isFallback = false });
            }

            return Json(new { success = false, message = result.ErrorMessage ?? "Lỗi gọi AI", isFallback = true });
        }
    }
}
