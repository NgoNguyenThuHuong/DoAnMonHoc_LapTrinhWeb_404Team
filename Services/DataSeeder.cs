using LingoToneMVC.Data;
using LingoToneMVC.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace LingoToneMVC.Services
{
    public class DataSeeder
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataSeeder(AppDbContext context, IWebHostEnvironment env,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await SeedRolesAndAdminAsync();

            try
            {
                if (!_context.HskWords.Any())
                {
                    Console.WriteLine("Importing HSK Data...");
                    var filePath = Path.Combine(_env.WebRootPath, "data", "hsk_vi.json");
                    if (File.Exists(filePath))
                    {
                        var json = await File.ReadAllTextAsync(filePath);
                        using JsonDocument doc = JsonDocument.Parse(json);
                        var words = new List<HskWord>();

                        foreach (JsonElement element in doc.RootElement.EnumerateArray())
                        {
                            var word = new HskWord
                            {
                                Hanzi = element.GetProperty("hanzi").GetString() ?? "",
                                Pinyin = element.TryGetProperty("pinyin", out var p) ? p.GetString() ?? "" : "",
                                MeaningVietnamese = element.TryGetProperty("vi", out var v) ? v.GetString() ?? "" : "",
                                MeaningEnglish = element.TryGetProperty("meaning", out var m) ? m.GetString() ?? "" : "",
                                HskLevel = element.TryGetProperty("level", out var l) ? (l.ValueKind == JsonValueKind.Number ? l.GetInt32().ToString() : l.GetString() ?? "") : "",
                                Radical = element.TryGetProperty("radicals", out var rad) ? rad.GetString() : (element.TryGetProperty("radical", out var rad2) ? rad2.GetString() : null)
                            };

                            if (element.TryGetProperty("strokes", out var stk) && stk.ValueKind == JsonValueKind.Number)
                            {
                                word.StrokeCount = stk.GetInt32();
                            }
                            else if (element.TryGetProperty("strokes", out var stkStr) && stkStr.ValueKind == JsonValueKind.String && int.TryParse(stkStr.GetString(), out var s))
                            {
                                word.StrokeCount = s;
                            }

                            // fallback for meaning if meaning is empty but translations.eng exists
                            if (string.IsNullOrEmpty(word.MeaningEnglish) && element.TryGetProperty("translations", out var trans) && trans.TryGetProperty("eng", out var eng) && eng.ValueKind == JsonValueKind.Array && eng.GetArrayLength() > 0)
                            {
                                word.MeaningEnglish = string.Join(", ", eng.EnumerateArray().Select(e => e.GetString()));
                            }

                            words.Add(word);
                        }

                        await _context.HskWords.AddRangeAsync(words);
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"Imported {words.Count} HSK Words Successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Seeding failed: File not found at {filePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding data: {ex.Message}");
            }
        }

        private async Task SeedRolesAndAdminAsync()
        {
            // Seed roles
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed default admin account
            const string adminEmail = "admin@lingotone.vn";
            const string adminPassword = "Admin@123";

            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    DisplayName = "Admin",
                    EmailConfirmed = true,
                    XP = 0,
                    Level = 1,
                    Streak = 0,
                    LastLoginDate = DateTime.Today
                };

                var result = await _userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine("Default admin account created: admin@lingotone.vn");
                }
            }
            else if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
