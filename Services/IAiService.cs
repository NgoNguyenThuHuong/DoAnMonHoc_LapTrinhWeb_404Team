using LingoToneMVC.Models;
using System.Threading.Tasks;

namespace LingoToneMVC.Services
{
    public interface IAiService
    {
        Task<AiResult> CallAiAsync(string prompt, int maxTokens = 800, string cacheKey = null);
        Task<AiResult> CheckGrammarAsync(string sentence);
        Task<AiResult> EvaluateWritingAsync(string text);
        Task<AiResult> EstimateHskLevelAsync(string text);
        Task<AiResult> TranslateVocabularyAsync(string englishText);
        Task<AiResult> RoleplayReplyAsync(string scenario, string history, string userMessage);
        Task<AiResult> GenerateHanziStoryAsync(string character);
        Task<AiResult> GenerateCareerRoadmapAsync(string career, string level, string goal, int minutesPerDay);
        Task<AiResult> GenerateQuizAsync(string topic, string level, int count);
    }
}
