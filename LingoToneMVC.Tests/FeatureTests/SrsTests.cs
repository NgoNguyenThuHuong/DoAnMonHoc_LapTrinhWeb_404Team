using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using LingoToneMVC.Tests.Helpers;
using Xunit;

namespace LingoToneMVC.Tests.FeatureTests
{
    public class SrsTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public SrsTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_Srs_Should_Not_Return_500()
        {
            var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var response = await client.GetAsync("/Srs");

            response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
        }
    }
}
