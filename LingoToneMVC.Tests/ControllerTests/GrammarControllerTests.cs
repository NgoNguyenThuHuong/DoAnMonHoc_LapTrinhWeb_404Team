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
    public class GrammarControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public GrammarControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CheckGrammar_Returns_False_If_Sentence_Empty()
        {
            var client = _factory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { sentence = "" }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/Grammar/CheckGrammar", content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(jsonString);

            ((bool)result.success).Should().BeFalse();
        }

        [Fact]
        public async Task CheckGrammar_Returns_True_If_Valid_And_Mocked_Ai()
        {
            _factory.MockAiService.Setup(x => x.CheckGrammarAsync(It.IsAny<string>()))
                .ReturnsAsync(new AiResult
                {
                    Success = true,
                    Content = "{ \"original\": \"wo shi xuesheng\", \"corrected\": \"我是学生\", \"explanation\": \"Correct!\" }",
                    IsFallback = false
                });

            var client = _factory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { sentence = "wo shi xuesheng" }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/Grammar/CheckGrammar", content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(jsonString);

            ((bool)result.success).Should().BeTrue();
            ((bool)result.isFallback).Should().BeFalse();
            ((string)result.corrected).Should().Be("我是学生");
        }

        [Fact]
        public async Task CheckGrammar_Returns_IsFallback_True_If_Ai_Overloaded()
        {
            _factory.MockAiService.Setup(x => x.CheckGrammarAsync(It.IsAny<string>()))
                .ReturnsAsync(new AiResult
                {
                    Success = false,
                    ErrorMessage = "API quá tải",
                    IsFallback = true
                });

            var client = _factory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { sentence = "test error" }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/Grammar/CheckGrammar", content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(jsonString);

            // Our GrammarController returns success = true for fallback to allow UI to show fallback info smoothly
            ((bool)result.success).Should().BeTrue();
            ((bool)result.isFallback).Should().BeTrue();
        }
    }
}
