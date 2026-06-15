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
    public class GeminiAiServiceFallbackTests
    {
        private GeminiAiService CreateServiceWithMockHttp(HttpStatusCode statusCode, string responseContent, bool missingKey = false)
        {
            var inMemorySettings = new System.Collections.Generic.Dictionary<string, string?>();
            if (!missingKey)
            {
                inMemorySettings["GeminiApiKey"] = "test_key";
            }
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
                    StatusCode = statusCode,
                    Content = new StringContent(responseContent)
                });

            var httpClient = new HttpClient(mockMessageHandler.Object)
            {
                BaseAddress = new Uri("http://test.com")
            };

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var logger = new Mock<ILogger<GeminiAiService>>().Object;

            return new GeminiAiService(httpClient, configuration, logger, memoryCache);
        }

        [Fact]
        public async Task Should_Fallback_On_429_TooManyRequests()
        {
            var service = CreateServiceWithMockHttp(HttpStatusCode.TooManyRequests, "");
            var result = await service.TranslateVocabularyAsync("测试");

            result.Success.Should().BeFalse();
            result.IsFallback.Should().BeTrue();
            result.ErrorMessage.Should().Contain("quá tải");
        }

        [Fact]
        public async Task Should_Fallback_On_503_ServiceUnavailable()
        {
            var service = CreateServiceWithMockHttp(HttpStatusCode.ServiceUnavailable, "");
            var result = await service.TranslateVocabularyAsync("测试");

            result.Success.Should().BeFalse();
            result.IsFallback.Should().BeTrue();
            result.ErrorMessage.Should().Contain("quá tải");
        }

        [Fact]
        public async Task Should_Fallback_On_Missing_ApiKey()
        {
            var service = CreateServiceWithMockHttp(HttpStatusCode.OK, "", missingKey: true);
            var result = await service.TranslateVocabularyAsync("测试");

            result.Success.Should().BeFalse();
            result.IsFallback.Should().BeTrue();
            result.ErrorMessage.Should().Contain("API Key");
        }

        [Fact]
        public async Task Should_Not_Call_Api_If_Input_Empty()
        {
            var service = CreateServiceWithMockHttp(HttpStatusCode.OK, "");
            var result = await service.TranslateVocabularyAsync("");

            result.Success.Should().BeFalse();
            result.IsFallback.Should().BeFalse();
            result.ErrorMessage.Should().Contain("trống");
        }

        [Fact]
        public async Task Should_Fallback_On_Invalid_JsonResponse()
        {
            var service = CreateServiceWithMockHttp(HttpStatusCode.OK, "{ invalid json }");
            var result = await service.CheckGrammarAsync("测试");

            result.Success.Should().BeFalse();
            result.IsFallback.Should().BeTrue();
            result.ErrorMessage.Should().Contain("Lỗi");
        }
    }
}
