using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using LingoToneMVC.Tests.Helpers;
using Xunit;

namespace LingoToneMVC.Tests.AuthTests
{
    public class AuthorizationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public AuthorizationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Guest_Can_Access_Public_Routes()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Grammar");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Guest_Cannot_Access_Protected_Routes()
        {
            var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var response = await client.GetAsync("/Progress");
            
            // Should redirect to Login
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location.ToString().Should().Contain("Login");
        }

        [Fact]
        public async Task Guest_Cannot_Access_Admin()
        {
            var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var response = await client.GetAsync("/Admin");
            
            // Should redirect to Login or Forbidden
            response.StatusCode.Should().BeOneOf(HttpStatusCode.Redirect, HttpStatusCode.Forbidden);
        }
    }
}
