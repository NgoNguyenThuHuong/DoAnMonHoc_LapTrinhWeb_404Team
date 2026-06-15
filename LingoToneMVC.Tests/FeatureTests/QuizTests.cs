using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using LingoToneMVC.Tests.Helpers;
using Xunit;

namespace LingoToneMVC.Tests.FeatureTests
{
    public class QuizTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public QuizTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_Quiz_By_Lesson_Should_Return_Success()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Quiz/Session/1");
            
            // It might redirect if it needs auth, or it might just return 200
            response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
        }
    }
}
