using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using LingoToneMVC.Tests.Helpers;
using Xunit;

namespace LingoToneMVC.Tests.SmokeTests
{
    public class RouteSmokeTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public RouteSmokeTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Home")]
        [InlineData("/Lesson")]
        [InlineData("/Grammar")]
        [InlineData("/Character")]
        [InlineData("/Pronunciation")]
        [InlineData("/Quiz")]
        [InlineData("/Srs")]
        [InlineData("/Roleplay")]
        [InlineData("/Writing")]
        [InlineData("/HskEstimator")]
        [InlineData("/Progress")]
        [InlineData("/Account/Login")]
        [InlineData("/Account/Register")]
        public async Task PublicAndProtectedRoutes_Should_Not_Return_500(string url)
        {
            var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync(url);

            // It should be 200 OK or 302 Redirect (to Login), but definitely not 500
            response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
        }
    }
}
