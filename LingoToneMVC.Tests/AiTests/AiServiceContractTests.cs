using System.Threading.Tasks;
using FluentAssertions;
using LingoToneMVC.Models;
using LingoToneMVC.Services;
using Moq;
using Xunit;

namespace LingoToneMVC.Tests.AiTests
{
    public class AiServiceContractTests
    {
        [Fact]
        public async Task IAiService_Should_Have_CheckGrammarAsync()
        {
            var mock = new Mock<IAiService>();
            mock.Setup(x => x.CheckGrammarAsync("测试")).ReturnsAsync(new AiResult { Success = true, IsFallback = false });

            var result = await mock.Object.CheckGrammarAsync("测试");

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.IsFallback.Should().BeFalse();
        }

        [Fact]
        public async Task IAiService_Should_Have_EvaluateWritingAsync()
        {
            var mock = new Mock<IAiService>();
            mock.Setup(x => x.EvaluateWritingAsync("测试")).ReturnsAsync(new AiResult { Success = true });

            var methodEvaluate = typeof(IAiService).GetMethod("EvaluateWritingAsync", new[] { typeof(string) });
            methodEvaluate.Should().NotBeNull();
            methodEvaluate.ReturnType.Should().Be(typeof(Task<AiResult>));
        }

        [Fact]
        public async Task IAiService_Should_Have_TranslateVocabularyAsync()
        {
            var mock = new Mock<IAiService>();
            mock.Setup(x => x.TranslateVocabularyAsync("测试")).ReturnsAsync(new AiResult { Success = true });

            var result = await mock.Object.TranslateVocabularyAsync("测试");

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task IAiService_Should_Have_RoleplayReplyAsync()
        {
            var mock = new Mock<IAiService>();
            mock.Setup(x => x.RoleplayReplyAsync("测试", "Test", "Test")).ReturnsAsync(new AiResult { Success = true });

            var result = await mock.Object.RoleplayReplyAsync("测试", "Test", "Test");

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }
    }
}
