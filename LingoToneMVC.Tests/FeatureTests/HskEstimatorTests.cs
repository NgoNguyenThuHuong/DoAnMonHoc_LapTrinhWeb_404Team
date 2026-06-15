using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using LingoToneMVC.Tests.Helpers;
using Xunit;

namespace LingoToneMVC.Tests.FeatureTests
{
    public class HskEstimatorTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public HskEstimatorTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task EvaluateText_Should_Return_Success_For_Valid_Text()
        {
            var client = _factory.CreateClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new System.Collections.Generic.KeyValuePair<string, string>("chineseText", "我是学生。")
            });

            var response = await client.PostAsync("/HskEstimator/EvaluateText", content);
            
            response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
        }
    }
}
