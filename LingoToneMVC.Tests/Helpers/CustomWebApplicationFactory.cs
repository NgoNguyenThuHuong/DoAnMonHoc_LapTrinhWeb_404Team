using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Linq;
using System;
using LingoToneMVC.Data;
using LingoToneMVC.Models;
using LingoToneMVC.Services;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace LingoToneMVC.Tests.Helpers
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public Mock<IAiService> MockAiService { get; }
        private readonly string _dbName;

        public CustomWebApplicationFactory()
        {
            _dbName = Guid.NewGuid().ToString();
            MockAiService = new Mock<IAiService>();

            // Setup default AI mocks
            MockAiService.Setup(x => x.TranslateVocabularyAsync(It.IsAny<string>()))
                .ReturnsAsync(new AiResult { Success = true, Content = "{\"vietnamese\":\"Thử nghiệm\"}" });
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(System.Data.Common.DbConnection));
                if (dbConnectionDescriptor != null) services.Remove(dbConnectionDescriptor);

                // Add InMemory AppDbContext
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);
                });

                // Replace IAiService with Mock
                services.RemoveAll(typeof(IAiService));
                services.AddSingleton<IAiService>(MockAiService.Object);

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();

                    db.Database.EnsureCreated();

                    try
                    {
                        SeedTestData(db);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Seeding failed: " + ex.ToString());
                        throw;
                    }
                }
            });
        }

        private void SeedTestData(AppDbContext db)
        {
            // Seed Lessons
            if (!db.Lessons.Any())
            {
                db.Lessons.Add(new Lesson { Title = "Lesson 1", Description = "Test", HskLevel = "1", OrderIndex = 1 });
                db.Lessons.Add(new Lesson { Title = "Lesson 2", Description = "Test", HskLevel = "1", OrderIndex = 2 });
                db.Lessons.Add(new Lesson { Title = "Lesson 3", Description = "Test", HskLevel = "2", OrderIndex = 3 });
                db.SaveChanges();
            }

            if (!db.Vocabularies.Any(v => v.Vietnamese == ""))
            {
                var lesson1 = db.Lessons.First();
                db.Vocabularies.Add(new Vocabulary { LessonId = lesson1.Id, Chinese = "字无", Pinyin = "ziwu", Vietnamese = "" });
                db.SaveChanges();
            }

            // Seed QuizQuestions
            if (!db.QuizQuestions.Any())
            {
                var lesson1 = db.Lessons.First();
                for (int i = 1; i <= 5; i++)
                {
                    db.QuizQuestions.Add(new QuizQuestion { LessonId = lesson1.Id, Question = $"Q {i}", CorrectAnswer = "A", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D" });
                }
                db.SaveChanges();
            }
        }
    }

    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "TestScheme");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
