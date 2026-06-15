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
    public class RoleplayControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public RoleplayControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ChatRoleplay_Returns_Error_If_Message_Empty()
        {
            var client = _factory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { scenario = "Test", history = new object[] {}, userMessage = "" }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/Ai/ChatRoleplay", content);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ChatRoleplay_Returns_Fallback_If_Ai_Overloaded()
        {
            _factory.MockAiService.Setup(x => x.RoleplayReplyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new AiResult 
                { 
                    Success = false, 
                    ErrorMessage = "API quá tải",
                    IsFallback = true
                });

            var client = _factory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { scenario = "Nhà hàng", history = new object[] {}, userMessage = "两" }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/Ai/ChatRoleplay", content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(jsonString);

            ((bool)result.isFallback).Should().BeTrue();
            ((string)result.zh).Should().NotBeNullOrEmpty();
            ((string)result.vi).Should().NotBeNullOrEmpty();
            ((string)result.zh).Should().Contain("好的"); // Example fallback
        }
    }
}
