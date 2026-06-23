using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using LingoToneMVC.Models;
using LingoToneMVC.Tests.Helpers;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace LingoToneMVC.Tests.ControllerTests
{
    public class WritingControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public WritingControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task EvaluateWriting_Returns_Error_If_Text_Empty()
        {
            var client = _factory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { text = "", level = "HSK1", essayType = "Email" }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/Ai/EvaluateWriting", content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(jsonString);

            ((bool)result.success).Should().BeFalse();
        }

        [Fact]
        public async Task EvaluateWriting_Returns_Data_If_Valid()
        {
            _factory.MockAiService.Setup(x => x.EvaluateWritingAsync(It.IsAny<string>()))
                .ReturnsAsync(new AiResult
                {
                    Success = true,
                    Content = "{ \"totalScore\": 85, \"feedback\": \"Good\", \"correctedText\": \"test\", \"grammarScore\": 80, \"vocabularyScore\": 90, \"fluencyScore\": 85 }",
                    IsFallback = false
                });

            var client = _factory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { text = "Valid text", level = "HSK1", essayType = "Email" }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/Ai/EvaluateWriting", content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(jsonString);

            ((int)result.totalScore).Should().Be(85);
            ((bool)result.isFallback).Should().BeFalse();
        }

        [Fact]
        public async Task EvaluateWriting_Returns_IsFallback_True_If_Ai_Overloaded()
        {
            _factory.MockAiService.Setup(x => x.EvaluateWritingAsync(It.IsAny<string>()))
                .ReturnsAsync(new AiResult
                {
                    Success = false,
                    ErrorMessage = "API quá tải",
                    IsFallback = true
                });

            var client = _factory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { text = "test error", level = "HSK1", essayType = "Email" }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/Ai/EvaluateWriting", content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(jsonString);

            ((bool)result.isFallback).Should().BeTrue();
            // Should provide a mock score so UI doesn't crash
            ((int?)result.totalScore).Should().NotBeNull();
            ((string)result.correctedText).Should().NotContain("API quá tải");
        }
    }
}
