using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LingoToneMVC.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace LingoToneMVC.Tests.AiTests
{
    public class AiCacheTests
    {
        [Fact]
        public async Task Should_Cache_Translation_Result_And_Not_Call_API_Twice()
        {
            var inMemorySettings = new System.Collections.Generic.Dictionary<string, string?> { { "GeminiApiKey", "test" } };
            IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{ \"candidates\": [ { \"content\": { \"parts\": [ { \"text\": \"{ \\\"vietnamese\\\": \\\"Thử nghiệm\\\" }\" } ] } } ] }")
                });

            var httpClient = new HttpClient(mockMessageHandler.Object)
            {
                BaseAddress = new Uri("https://generativelanguage.googleapis.com/")
            };

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var logger = new Mock<ILogger<GeminiAiService>>().Object;
            var service = new GeminiAiService(httpClient, configuration, logger, memoryCache);

            // First call
            var result1 = await service.TranslateVocabularyAsync("测试");
            
            // Second call
            var result2 = await service.TranslateVocabularyAsync("测试");

            result1.Success.Should().BeTrue();
            result2.Success.Should().BeTrue();
            result1.Content.Should().Be(result2.Content);

            // Verify SendAsync was only called ONCE
            mockMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
