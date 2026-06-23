using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using LingoToneMVC.Data;
using LingoToneMVC.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Newtonsoft.Json;
using LingoToneMVC.Services;
using Xunit;

namespace LingoToneMVC.Tests.ControllerTests
{
    public class AdminVocabularyTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _testFactory;

        public AdminVocabularyTests(CustomWebApplicationFactory<Program> factory)
        {
            _testFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = "TestScheme";
                        options.DefaultChallengeScheme = "TestScheme";
                    }).AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
                });
            });
            _client = _testFactory.CreateClient();
        }

        [Fact]
        public async Task TranslateVocabulary_Returns_404_If_Id_Not_Found()
        {
            var response = await _client.PostAsync("/Admin/TranslateVocabulary?id=9999", null);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task TranslateVocabulary_Calls_Ai_And_Saves_If_Meaning_Empty()
        {
            using var scope = _testFactory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var vocab = db.Vocabularies.First(v => string.IsNullOrEmpty(v.Vietnamese));
            int id = vocab.Id;

            var response = await _client.PostAsync($"/Admin/TranslateVocabulary/{id}", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(jsonString);

            ((bool)result.success).Should().BeTrue();
            ((string)result.vietnamese).Should().Be("Thử nghiệm");

            db.Entry(vocab).Reload();
            vocab.Vietnamese.Should().Be("Thử nghiệm");
        }
        [Fact]
        public async Task Get_Vocabularies_Does_Not_Call_Ai()
        {
            var mockAi = _testFactory.Services.GetRequiredService<IAiService>();
            Mock.Get(mockAi).Invocations.Clear();

            var response = await _client.GetAsync("/Admin/Vocabularies");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            Mock.Get(mockAi).Verify(a => a.TranslateVocabularyAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
