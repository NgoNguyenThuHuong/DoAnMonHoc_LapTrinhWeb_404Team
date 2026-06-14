using LingoToneMVC.Models;
using LingoToneMVC.Controllers;
using System.Text.Json;

namespace LingoToneMVC.Services
{
    public class HskLessonService
    {
        private readonly IWebHostEnvironment _env;
        private List<Lesson>? _cachedLessons;

        private static readonly Dictionary<string, string> VietnameseMeanings = new()
        {
            ["爱"] = "yêu, thích",
            ["八"] = "số tám",
            ["爸爸"] = "bố, ba",
            ["杯子"] = "cái cốc, cái ly",
            ["北京"] = "Bắc Kinh",
            ["本"] = "quyển, vốn, gốc",
            ["不客气"] = "không có gì",
            ["不"] = "không",
            ["菜"] = "món ăn, rau",
            ["茶"] = "trà",
            ["吃"] = "ăn",
            ["出租车"] = "taxi",
            ["打电话"] = "gọi điện thoại",
            ["大"] = "to, lớn",
            ["的"] = "của",
            ["我"] = "tôi, mình",
            ["你"] = "bạn, cậu",
            ["好"] = "tốt, khỏe",
            ["是"] = "là",
            ["人"] = "người",
            ["很"] = "rất"
        };

        public HskLessonService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string? GetVietnameseMeaning(string hanzi)
        {
            if (VietnameseMeanings.TryGetValue(hanzi, out var meaning)) return meaning;
            return null;
        }

        public async Task<List<Lesson>> GetAllHskLessonsAsync()
        {
            if (_cachedLessons != null) return _cachedLessons;

            var jsonPath = Path.Combine(_env.WebRootPath, "data", "hsk.json");
            var json = await System.IO.File.ReadAllTextAsync(jsonPath);
            var hskData = JsonSerializer.Deserialize<List<HskWordDto>>(json) ?? new List<HskWordDto>();

            var lessons = new List<Lesson>();
            var lessonNames = new[] { "Từ vựng nền tảng", "Giao tiếp cơ bản", "Gia đình & con người", "Thời gian & thời tiết", "Ăn uống & sở thích", "Mua sắm & du lịch", "Công việc & học tập", "Sở thích 2", "Mở rộng 1", "Mở rộng 2", "Mở rộng 3" };

            for (int i = 1; i <= 6; i++)
            {
                var levelWords = hskData.Where(w => w.level == i).ToList();
                if (!levelWords.Any()) continue;

                int chunks = (int)Math.Ceiling(levelWords.Count / 15.0);

                for (int j = 0; j < chunks; j++)
                {
                    var chunkWords = levelWords.Skip(j * 15).Take(15).ToList();
                    var name = j < lessonNames.Length ? lessonNames[j] : $"Chủ đề {j + 1}";
                    
                    var lesson = new Lesson
                    {
                        Id = i * 1000 + (j + 1), // Format: 1001, 2005, etc.
                        Title = $"Bài {j + 1}: {name}",
                        Description = $"Chinh phục {chunkWords.Count} từ vựng",
                        OrderIndex = j + 1,
                        HskLevel = i.ToString(),
                        Vocabularies = chunkWords.Select(w => new Vocabulary
                        {
                            Chinese = w.hanzi,
                            Pinyin = w.pinyin,
                            Vietnamese = GetVietnameseMeaning(w.hanzi) ?? "Đang dịch...",
                            LessonId = i * 1000 + (j + 1)
                        }).ToList(),
                        GrammarPoints = new List<GrammarPoint>(),
                        Dialogues = new List<Dialogue>()
                    };
                    
                    foreach (var v in lesson.Vocabularies)
                    {
                        v.Lesson = lesson;
                    }
                    
                    lessons.Add(lesson);
                }
            }
            _cachedLessons = lessons;
            return lessons;
        }

        public async Task<Lesson?> GetHskLessonByIdAsync(int id)
        {
            var lessons = await GetAllHskLessonsAsync();
            return lessons.FirstOrDefault(l => l.Id == id);
        }
    }
}
