# TÀI LIỆU HƯỚNG DẪN VẤN ĐÁP ĐỒ ÁN LINGOTONE AI

Tài liệu này được biên soạn đặc biệt dành cho nhóm 4 người để chuẩn bị cho buổi bảo vệ đồ án/vấn đáp với giảng viên. Vui lòng đọc kỹ phần được phân công của mình.

---

# PHẦN 1: TỔNG QUAN ĐỒ ÁN VÀ PROJECT

* **Tên project:** LingoTone AI (hoặc tên theo đăng ký với giảng viên).
* **Mục tiêu chính:** Xây dựng nền tảng học tiếng Trung trực tuyến thông minh, ứng dụng AI để cá nhân hóa lộ trình học, cải thiện ngữ pháp, giao tiếp và từ vựng.
* **Vấn đề giải quyết:** Người tự học tiếng Trung thường thiếu môi trường thực hành giao tiếp thực tế và người chấm chữa bài viết, dịch thuật. LingoTone cung cấp AI để đóng vai trò như một gia sư ảo 24/7.
* **Người dùng chính:** Sinh viên, người đi làm muốn học tiếng Trung từ cơ bản đến nâng cao (HSK 1 - HSK 6).
* **Các chức năng chính:**
  - Học theo lộ trình bài học (Lessons, Vocabularies, Grammar).
  - Làm bài trắc nghiệm (Quiz) và tính điểm.
  - Học từ vựng lặp lại ngắt quãng (SRS - Spaced Repetition System).
  - Tra cứu chữ Hán, chiết tự.
  - Các tính năng AI (Giao tiếp Roleplay, Chữa ngữ pháp, Đánh giá bài viết, Tạo lộ trình học).
  - Bảng xếp hạng (Leaderboard) thi đua giữa người dùng.
* **Công nghệ sử dụng:**
  - **Framework:** ASP.NET Core MVC (.NET 8).
  - **Database:** Entity Framework Core (ORM), SQL Server.
  - **Authentication:** ASP.NET Core Identity (Đăng nhập thường & Google OAuth 2.0).
  - **Frontend:** HTML, CSS, JavaScript, Bootstrap 5.
  - **AI:** Tích hợp Google Gemini AI qua HTTP Request.
  - **Testing:** xUnit, Moq (Unit test và E2E test cơ bản).
* **Kiến trúc tổng thể:** Áp dụng mô hình MVC (Model - View - Controller), kết hợp Dependency Injection (DI) để tiêm các Service (như `IAiService`) vào Controller.
* **MVC hoạt động như thế nào:** 
  - Người dùng gửi Request (bấm nút/gõ URL).
  - `Controller` nhận request, gọi `AppDbContext` hoặc `Service` để xử lý logic và lấy dữ liệu.
  - Dữ liệu được đóng gói vào `Model` hoặc `ViewModel`.
  - `Controller` gửi Model này sang `View`.
  - `View` (Razor `cshtml`) render ra HTML và trả về cho trình duyệt.
* **Luồng chạy chính:** `Program.cs` cấu hình các dịch vụ (DI) và pipeline -> Router điều hướng URL đến Controller tương ứng -> Action chạy -> Trả về View.

* **Các thư mục quan trọng:**
  - `Controllers/`: Nơi chứa code xử lý logic điều hướng, nhận request và trả về response.
  - `Models/`: Chứa các class biểu diễn bảng trong Database (Entities).
  - `Views/`: Chứa các file giao diện `.cshtml` (HTML kết hợp C#).
  - `Data/`: Chứa `AppDbContext.cs` (Cấu hình DB) và `SeedData.cs` (Tạo dữ liệu mẫu).
  - `Services/`: Chứa các class xử lý logic nghiệp vụ phức tạp (ví dụ: gọi API Gemini, sinh bài học HSK).
  - `ViewModels/`: Chứa các class chuyên dùng để truyền dữ liệu từ Controller sang View.
  - `wwwroot/`: Chứa file tĩnh công khai như `css`, `js`, `images`.
  - `Migrations/`: Chứa các file lịch sử thay đổi cấu trúc database do Entity Framework tạo ra.
  - `LingoToneMVC.Tests/` & `LingoToneMVC.E2ETests/`: Thư mục chứa các test.
  - `docs/`: Thư mục chứa các tài liệu dự án, bao gồm file hướng dẫn vấn đáp này.

---

# PHẦN 2: PHÂN CÔNG 4 THÀNH VIÊN

| Thành viên phụ trách | Phần code cần nắm | Thư mục/File cần học | Chức năng liên quan | Câu hỏi thường gặp | Cách trả lời |
|---|---|---|---|---|---|
| **A. Backend** | Logic Controller, ViewModels, Routing, Auth | `Controllers/`, `Program.cs`, `ViewModels/` | CRUD, Login/Register, Quiz logic, SRS logic | Controller này làm gì? Dữ liệu truyền sang view bằng cách nào? | Nêu rõ tên Controller, Action và ViewModel được dùng. |
| **B. Frontend** | Views, Layout, JS, CSS | `Views/`, `wwwroot/css/`, `wwwroot/js/` | Giao diện hiển thị, AJAX call, Form validation | Bấm nút này gọi API nào? Layout nằm ở đâu? | Giải thích file `.cshtml` tương ứng, thư mục `Shared/_Layout.cshtml`. |
| **C. Database** | Entity, DbContext, Migrations | `Models/`, `Data/AppDbContext.cs` | Bảng dữ liệu, quan hệ 1-N, N-N, dữ liệu mẫu | Bảng này liên kết bảng kia thế nào? FK là gì? | Nêu tên Model, thuộc tính điều hướng (Navigation property). |
| **D. AI** | Gemini Service, Prompts | `Services/GeminiAiService.cs` | Roleplay, Sửa lỗi viết, Sinh Quiz | AI được gọi như thế nào? Xử lý lỗi AI ra sao? | Giải thích cách cấu hình HttpClient, gửi JSON và xử lý Fallback. |

---

## A. BACKEND

**1. AccountController.cs**
- Mục đích: Xử lý Đăng ký, Đăng nhập, Đăng xuất, Quên mật khẩu, Đăng nhập Google.
- Hàm chính:
  - `Login(LoginViewModel)`: Nhận email, password -> Dùng `SignInManager.PasswordSignInAsync` để check DB -> Redirect về Home.
  - `ExternalLoginCallback`: Nhận thông tin từ Google -> Tạo user mới nếu chưa có -> Đăng nhập.
  - `ForgotPassword`: Nhận email -> Tạo token đổi mật khẩu -> (giả lập) gửi email.
- Trả lời giảng viên: "Dạ em dùng Identity có sẵn của .NET Core. SignInManager lo việc cấp cookie đăng nhập."

**2. LessonController.cs**
- Mục đích: Hiển thị lộ trình bài học và chi tiết bài học.
- Hàm chính:
  - `Index`: Lấy danh sách bài học chia theo HSK.
  - `Detail(id)`: `Include` Vocabulary, GrammarPoint -> truyền vào `LessonDetailViewModel` -> trả về View.
  - `Complete(id)`: Đánh dấu user đã học xong, cộng XP.
- Trả lời giảng viên: "Dạ em dùng Entity Framework Include để lấy kèm các bảng con thay vì phải query nhiều lần."

**3. QuizController.cs**
- Mục đích: Làm bài kiểm tra, chấm điểm và sinh câu hỏi AI.
- Hàm chính:
  - `SubmitQuiz(List<int> selectedAnswers)`: Chấm điểm dựa trên đáp án đúng lưu trong DB -> Trả về JSON điểm số.
- Trả lời giảng viên: "Dạ submit form bằng AJAX, Controller tính điểm và trả về JSON để Frontend báo kết quả mà không cần tải lại trang."

**4. SrsController.cs**
- Mục đích: Tính toán thuật toán lặp lại ngắt quãng (SRS) cho flashcard.
- Hàm chính: `Review(cardId, score)`: Dựa vào điểm (0,1,2), cập nhật `SrsLevel` và `NextReviewAt` (ngày ôn tập tiếp theo).
- Trả lời giảng viên: "Dạ thuật toán SRS tính toán dựa trên mức độ thuộc của User, nếu đúng thì số ngày chờ ôn lại sẽ tăng lên."

---

## B. FRONTEND

**1. Giao diện (Views/Shared/_Layout.cshtml)**
- Mục đích: Bộ khung của website chứa Header, Sidebar, Footer, nhúng CSS/JS chung.
- Câu hỏi: "Layout hoạt động như thế nào?"
- Trả lời: "Dạ Layout là master page. Các trang con được nhúng vào chỗ có biến `@RenderBody()`."

**2. Views/Home/Index.cshtml**
- Mục đích: Trang chủ giới thiệu, có các nút điều hướng.
- Trả lời: "Trang này em xài Bootstrap container và grid system (row, col) để làm reponsive."

**3. Làm Quiz (Views/Quiz/Detail.cshtml)**
- Hoạt động: Giao diện load danh sách câu hỏi. Khi bấm nộp bài, code Javascript (AJAX) sẽ lấy giá trị radio button -> fetch API gửi lên `QuizController/SubmitQuiz`.
- Câu hỏi: "Làm sao submit không bị load lại trang?"
- Trả lời: "Dạ em dùng fetch API trong thẻ `<script>` ở cuối View. Nhận JSON trả về và dùng SweetAlert2 để hiện popup chúc mừng."

**4. Chức năng Roleplay AI (Views/Roleplay/Index.cshtml)**
- Hoạt động: Có một khung chat (div). JS sẽ bắt sự kiện Enter hoặc bấm nút Gửi -> gọi API `AiController/ChatRoleplay` -> append đoạn HTML tin nhắn mới vào khung chat.

---

## C. DATA / DATABASE

**1. AppDbContext.cs**
- File: `Data/AppDbContext.cs`
- Mục đích: Kế thừa `IdentityDbContext<ApplicationUser>`, đại diện cho toàn bộ CSDL. Có các `DbSet<Model>` (như bảng trong SQL).
- Câu hỏi: "Làm sao EF Core biết tạo bảng nào?"
- Trả lời: "Dạ EF Core quét các thuộc tính DbSet trong AppDbContext. Khi chạy `dotnet ef database update`, nó dựa vào Migration để tạo bảng tương ứng dưới SQL."

**2. ApplicationUser.cs**
- File: `Models/ApplicationUser.cs`
- Mục đích: Kế thừa `IdentityUser` mặc định, thêm các cột tùy chỉnh như `DisplayName`, `XP`, `Level`, `Streak`.
- Trả lời: "Dạ em kế thừa IdentityUser để tận dụng chức năng auth, nhưng thêm XP và Level để làm tính năng gamification."

**3. Bảng Lesson và Vocabulary**
- Quan hệ: 1 Bài học (Lesson) có nhiều Từ vựng (Vocabulary) -> Quan hệ 1-Nhiều (1-N).
- Khóa ngoại: Trong `Vocabulary.cs` có `public int LessonId { get; set; }` và `public Lesson Lesson { get; set; }`.

**4. Migrations & SeedData**
- Migration: Dùng để đồng bộ thay đổi từ C# code xuống Database mà không bị mất dữ liệu cũ.
- SeedData: `Data/SeedData.cs` chạy lúc ứng dụng khởi động. Hàm `Initialize` tạo admin user và chèn dữ liệu Flashcard, Vocabulary mẫu nếu DB rỗng.

---

## D. AI

**1. GeminiAiService.cs**
- File: `Services/GeminiAiService.cs`
- Mục đích: Lớp chứa toàn bộ logic giao tiếp với Google Gemini API. Thực thi interface `IAiService`.
- Hàm chính:
  - `GenerateQuizAsync(topic)`: Nhận chủ đề -> Tạo prompt -> Gửi POST request đến Google API -> Xử lý JSON trả về.
  - Cấu hình Key: Key được đọc từ `_configuration["Gemini:ApiKey"]` (được giấu trong `appsettings.json` hoặc user secrets).
- Xử lý lỗi (Fallback): "Dạ nếu API lỗi hoặc hết quota, hàm sẽ trả về `AiResult { Success = false, IsFallback = true }`. Controller nhận được sẽ trả về dữ liệu mẫu (mô phỏng) để hệ thống không bị crash, người dùng vẫn test được giao diện."

**2. AiController.cs**
- Mục đích: Đóng vai trò cầu nối, nhận request từ Frontend (AJAX), gọi `GeminiAiService`, và format JSON trả về cho Frontend hiển thị.
- Trả lời: "Dạ em không gọi API trực tiếp từ Frontend để bảo mật API Key. Mọi luồng đều đi qua Controller backend rồi backend mới gọi Google."

---

# PHẦN 3: GIẢI THÍCH CÁC HÀM QUAN TRỌNG

| Tên file | Tên hàm | Chức năng | Input / Output | Vì sao cần hàm này? Câu hỏi có thể gặp |
|---|---|---|---|---|
| `GeminiAiService.cs` | `CallGeminiApiAsync` | Hàm private gọi HTTP đến Google | Đầu vào: Chuỗi prompt. Đầu ra: JSON string từ Google | Hàm dùng chung để tái sử dụng HttpClient. Hỏi: Làm sao gọi API ngoài? Trả lời: Dạ dùng `HttpClient.PostAsync`. |
| `AiController.cs` | `EvaluateWriting` | Nhận bài viết của user để AI chấm điểm | Input: `text`. Output: JSON chứa điểm và gợi ý. | Xử lý fallback nếu AI sập. Hỏi: Nếu đứt mạng AI thì sao? Trả lời: Dạ có cơ chế IsFallback trả về mock data ạ. |
| `LessonController.cs` | `Detail(int id)` | Hiển thị bài học chi tiết | Input: `id` bài học. Output: View kèm `LessonDetailViewModel`. | Hỏi: Làm sao lấy từ vựng của lesson? Trả lời: Em dùng `.Include(l => l.Vocabularies)` của EF Core ạ. |
| `SrsController.cs` | `Review` | Thuật toán ôn tập (SRS) | Input: `cardId`, `score` (0,1,2). Output: JSON success. | Cập nhật ngày ôn tiếp theo (`NextReviewAt`) dựa trên điểm. Hỏi: SRS là gì? Trả lời: Là lặp lại ngắt quãng ạ. |
| `AccountController.cs` | `ExternalLoginCallback` | Xử lý callback từ Google | Mặc định. Output: Redirect Home. | Giúp đăng nhập bằng Google. Hỏi: Identity xử lý OAuth thế nào? Trả lời: Nó dùng CookieScheme để tạo session ạ. |

---

# PHẦN 4: LUỒNG CHỨC NĂNG CHÍNH

**1. Luồng đăng nhập Google (OAuth 2.0)**
- Giao diện: Người dùng bấm "Đăng nhập bằng Google".
- Controller: `AccountController.ExternalLogin` tạo request gửi sang server Google.
- Xử lý: Google xác thực xong, đá ngược về URL `signin-google` (đã config). Sau đó vào hàm `ExternalLoginCallback`.
- Database: Kiểm tra email, nếu chưa có thì `_userManager.CreateAsync` để tạo tài khoản mới. Đăng nhập xong lưu cookie.

**2. Luồng Sửa lỗi bài viết (AI)**
- Giao diện: Người dùng nhập đoạn văn vào textarea, bấm "Phân tích" (JS Fetch API).
- Controller: `AiController.EvaluateWriting` nhận đoạn text.
- Service: Gọi `_aiService.EvaluateWritingAsync()`, tạo prompt yêu cầu trả về JSON điểm số và sửa lỗi.
- Trả về: Backend ném JSON về Frontend. Frontend parse JSON và hiển thị điểm, từ vựng nâng cao ra màn hình.

**3. Luồng Thuật toán SRS (Học Flashcard)**
- Giao diện: Màn hình hiện từ vựng, người dùng bấm "Quên", "Khó", "Dễ".
- Controller: Gửi điểm 0, 1, 2 lên `SrsController.Review`.
- Database: Lấy `SrsCard`. Dùng switch-case: Dễ (+ level, + nhiều ngày), Khó (giữ nguyên level), Quên (level về 0). Lưu `db.SaveChanges()`.

**4. Luồng xem bài học**
- Giao diện: Bấm vào thẻ bài học trên màn hình danh sách.
- Controller: `LessonController.Detail(id)` nhận request.
- Database: DbContext truy vấn bảng Lesson kèm Vocabularies và GrammarPoints.
- View: Model được truyền vào View `Detail.cshtml`, dùng thẻ `@foreach` để in danh sách từ vựng.

**5. Luồng lưu kết quả quiz**
- Giao diện: Người dùng bấm nộp bài. JS lấy id câu hỏi và phương án chọn đẩy lên JSON.
- Controller: `QuizController.SubmitQuiz`. Hàm sẽ lặp qua các câu hỏi, dò đáp án đúng trong bảng `QuizQuestion`, tính điểm.
- Database: Cập nhật bảng `UserQuizAttempt` và cộng XP vào bảng `AspNetUsers`.

---

# PHẦN 5: CÂU HỎI VẤN ĐÁP THEO TỪNG NGƯỜI

### A. Backend
1. **Mô hình MVC hoạt động như thế nào trong bài này?** -> Dạ request vào Router -> Controller xử lý -> gọi DbContext/Service -> Truyền ViewModel sang View.
2. **ViewModel dùng để làm gì?** -> Dạ để gộp nhiều Model lại (ví dụ gộp Lesson và danh sách Vocabulary) rồi truyền gọn sang View.
3. **Mật khẩu người dùng lưu thế nào?** -> Dạ không lưu plain text. Được ASP.NET Identity mã hóa (hash) tự động bằng thuật toán băm bảo mật.
4. **Cách lấy thông tin User đang đăng nhập?** -> Dạ dùng `_userManager.GetUserAsync(User)` trong Controller.
5. **Session và Cookie quản lý như thế nào?** -> Dạ project này dùng Identity, tự động quản lý qua Authentication Cookie.
6. **Dependency Injection (DI) nằm ở đâu?** -> Dạ nằm ở `Program.cs` thông qua `builder.Services.AddScoped`, truyền vào constructor của Controller.
7. **Làm sao chặn người dùng chưa đăng nhập truy cập?** -> Dạ dùng attribute `[Authorize]` trên đầu Controller hoặc Action.
8. **Controller nào dài và khó nhất?** -> Dạ `AiController` và `LessonController` vì xử lý nhiều logic chuyển đổi JSON và gọi service ngoài.
9. **Mô tả hàm Login?** -> Dạ nhận `LoginViewModel`, gọi `SignInManager.PasswordSignInAsync`, nếu thành công thì redirect.
10. **Làm sao biết người dùng làm đúng bao nhiêu câu quiz?** -> Dạ Controller nhận danh sách trả lời, query DB lấy đáp án thật ra so sánh và đếm số lượng câu đúng.
11. **Action trả về gì?** -> Dạ thường trả về View(), RedirectToAction(), hoặc Json() nếu gọi từ AJAX.
12. **Tính năng Quên mật khẩu hoạt động thế nào?** -> Dạ hệ thống tạo token, hiện tại đang giả lập gửi token qua Console log để user click vào link reset.
13. **Tại sao có IActionResult?** -> Dạ đây là kiểu trả về chung của ASP.NET, giúp trả về View, File, Json hay Redirect tùy ý.
14. **Service khác Controller chỗ nào?** -> Dạ Controller chỉ nên nhận request và trả response. Logic tính toán phức tạp nên đẩy vào Service.
15. **Làm sao bảo mật API AI?** -> Dạ API Key chỉ gọi từ Server Backend, Frontend không hề biết Key này.

### B. Frontend
1. **Web này có responsive không?** -> Dạ có, em dùng Grid System (col-md, col-lg) của Bootstrap 5.
2. **AJAX được sử dụng ở đâu?** -> Dạ ở chỗ làm bài Quiz và gọi AI, dùng hàm `fetch()` trong Javascript để không bị tải lại trang.
3. **Thư mục wwwroot chứa gì?** -> Dạ chứa các file tĩnh (CSS tự viết, JS, hình ảnh). Không chứa code C# bảo mật.
4. **Validation (kiểm tra lỗi form) hoạt động sao?** -> Dạ kết hợp thuộc tính `required` của HTML5 và thẻ `<span asp-validation-for="...">` của Razor.
5. **Partial View là gì? Mở xem thử?** -> Dạ partial view giống như component, file bắt đầu bằng dấu gạch dưới như `_LoginPartial.cshtml` dùng để tái sử dụng.
6. **Làm sao chèn code C# vào file HTML?** -> Dạ bằng cú pháp Razor (bắt đầu bằng `@`).
7. **`@model` ở đầu file cshtml làm gì?** -> Dạ khai báo kiểu dữ liệu mà Controller gửi sang View để có gợi ý code (strongly-typed).
8. **ViewBag và ViewData khác gì Model?** -> Dạ nó là biến tạm dùng chung không định kiểu, thường dùng để truyền Title hoặc thông báo lỗi đơn giản.
9. **Làm sao lấy ID bài học từ thẻ a?** -> Dạ gán ID vào `asp-route-id="@lesson.Id"` trên thẻ `<a>`.
10. **Form gửi dữ liệu thế nào?** -> Dạ qua thẻ `<form method="post" asp-action="...">` kết hợp với thẻ `asp-for` để tự động map tên trường.
11. **CSS tuỳ chỉnh nằm ở đâu?** -> Dạ nằm trong `wwwroot/css/site.css` hoặc các file CSS riêng.
12. **Javascript xử lý AI chat nằm ở đâu?** -> Dạ em viết thẻ `<script>` ở cuối View `Roleplay/Index.cshtml`, dùng `fetch` đẩy data.
13. **Thư viện SweetAlert2 dùng làm gì?** -> Dạ để hiển thị popup thông báo (như popup chúc mừng sau khi xong quiz) đẹp hơn thay vì dùng `alert()` mặc định.
14. **`_ViewImports.cshtml` dùng làm gì?** -> Dạ để using các thư viện C# một lần dùng cho toàn bộ View trong thư mục.
15. **Làm sao hiển thị lỗi khi user nhập sai email?** -> Dạ ModelState sẽ bắt lỗi ở Controller, đẩy về View và `asp-validation-summary` sẽ in ra.

### C. Data / Database
1. **Entity Framework Core là gì?** -> Dạ là công cụ ORM giúp thao tác CSDL bằng code C# thay vì viết câu lệnh SQL thuần.
2. **Khoá ngoại được khai báo thế nào?** -> Dạ khai báo kiểu int `LessonId` và thêm thuộc tính điều hướng (Navigation) `public virtual Lesson Lesson {get;set;}`.
3. **Seed Data chạy lúc nào?** -> Dạ chạy lúc khởi động Program.cs. Giúp tạo tài khoản admin và dữ liệu bài học mẫu nếu CSDL rỗng.
4. **Lệnh `Update-Database` làm gì?** -> Dạ để chạy các file trong thư mục Migrations, ánh xạ sự thay đổi cấu trúc class xuống bảng SQL.
5. **Database có bao nhiêu bảng chính?** -> Dạ có bảng Users (Identity), Lessons, Vocabularies, GrammarPoints, UserProgress...
6. **Làm sao lấy danh sách Từ vựng của một Bài học?** -> Dạ dùng `_db.Lessons.Include(l => l.Vocabularies).FirstOrDefault(...)`.
7. **Bảng ApplicationUser kế thừa từ đâu?** -> Dạ kế thừa từ `IdentityUser` của thư viện ASP.NET Core Identity.
8. **Dữ liệu được config dùng SQLite hay SQL Server?** -> Dạ config tại `appsettings.json`, dòng `UseSqlServer(...)` trong `Program.cs`.
9. **Mối quan hệ giữa User và UserProgress?** -> Dạ quan hệ 1-Nhiều. Một User có thể hoàn thành nhiều Bài học.
10. **Làm sao thêm cột `XP` vào bảng User có sẵn?** -> Dạ thêm property `XP` vào class `ApplicationUser`, tạo Migration mới rồi update database.
11. **LINQ là gì?** -> Dạ là ngôn ngữ truy vấn tích hợp. Thay vì gõ `SELECT * FROM`, tụi em gõ `_db.Lessons.Where(x => ...).ToList()`.
12. **DbSet là gì?** -> Dạ là một thuộc tính trong `AppDbContext` đại diện cho một bảng dữ liệu dưới DB.
13. **Cách lưu xuống DB sau khi đổi dữ liệu?** -> Dạ phải gọi lệnh `_db.SaveChangesAsync()`.
14. **Bảng SrsCard dùng để làm gì?** -> Dạ lưu thông tin tiến độ học Flashcard của từng User (điểm nhớ, ngày ôn tiếp theo).
15. **Làm sao xóa 1 bài học khỏi DB?** -> Dạ `_db.Lessons.Remove(lesson)` rồi lưu thay đổi. Nếu có khóa ngoại thì các từ vựng con sẽ bị xóa theo (Cascade delete).

### D. AI
1. **Mô hình AI nào đang được sử dụng?** -> Dạ là Google Gemini AI (cụ thể là Gemini 1.5 flash hoặc tùy cấu hình API).
2. **Prompt AI được thiết kế thế nào?** -> Dạ trong `GeminiAiService`, em nối chuỗi yêu cầu rõ AI đóng vai giáo viên, trả về format chuẩn JSON.
3. **Làm sao không để lộ API Key?** -> Dạ lưu trong `appsettings.json` hoặc user secrets, đọc ra bằng biến `IConfiguration`.
4. **Nếu AI trả về chuỗi bậy bạ (hallucination) thì sao?** -> Dạ hàm `CleanAndParseJson` của em sẽ cắt bỏ các markdown dư thừa và cố gắng parse lại.
5. **Fallback mechanism (cơ chế dự phòng) là gì?** -> Dạ khi hết tiền API hoặc Google sập, API trả lỗi, catch block sẽ nhận diện và trả về cờ `isFallback=true` kèm dữ liệu cứng mẫu (mock data).
6. **Gửi request HTTP lên Google bằng class nào?** -> Dạ bằng `HttpClient` thông qua hàm `PostAsync`.
7. **Làm sao báo cho người dùng biết hệ thống đang dùng dữ liệu mô phỏng?** -> Dạ JSON trả về có cờ `isFallback`, Frontend sẽ hiện chữ "Dữ liệu mô phỏng do API quá tải".
8. **Làm sao truyền ngữ cảnh chat cũ vào AI để nó nhớ?** -> Dạ tụi em gom tất cả mảng `history` (lịch sử chat) thành 1 chuỗi dài dán vào phần thân của Prompt.
9. **Giao tiếp Roleplay dùng chung một Prompt hay chia nhỏ?** -> Dạ dùng Prompt động, gắn kèm tham số `scenario` (bối cảnh như "Đi nhà hàng", "Mua sắm") vào trước câu lệnh.
10. **Làm sao biết câu trả lời của AI là JSON hay Plain text?** -> Dạ em ép nó trả JSON bằng cách ghi rõ "Please output ONLY valid JSON without any markdown" trong Prompt.
11. **Lớp IAiService dùng để làm gì?** -> Dạ là interface định nghĩa các hàm AI. Đảm bảo tính lỏng lẻo (loose coupling) để dễ mock khi viết Unit Test.
12. **Vì sao phải dùng async/await khi gọi API?** -> Dạ gọi mạng rất lâu, dùng async để không block luồng xử lý chính của web server.
13. **Làm sao lấy giá trị Key từ file config?** -> Dạ dùng `_configuration["Gemini:ApiKey"]` trong hàm khởi tạo của Service.
14. **Tạo lộ trình học cá nhân hóa dựa trên gì?** -> Dạ dựa vào mục tiêu (goal), ngành nghề (career), và thời gian học mỗi ngày (minutesPerDay).
15. **Có thể thay thế Gemini bằng ChatGPT không?** -> Dạ có, chỉ cần viết một Service mới kế thừa `IAiService` mà không cần sửa code ở Controller.

---

# PHẦN 6: NHỮNG FILE MỖI NGƯỜI CẦN ĐỌC KỸ

| Người phụ trách | File cần đọc | Mức độ | Lý do cần đọc | Nội dung cần nhớ |
|---|---|---|---|---|
| **Backend** | `Controllers/AccountController.cs` | Cao | Thường bị hỏi về phân quyền, đăng nhập. | Nhớ cách dùng SignInManager, UserManager. |
| **Backend** | `Controllers/LessonController.cs` | Cao | Xử lý logic hiển thị bài học chính. | Nhớ cách gọi Include, lấy List Vocabularies. |
| **Backend** | `Program.cs` | Trung bình | File khởi động hệ thống. | Nhớ vị trí config DI (AddScoped), Route. |
| **Frontend** | `Views/Shared/_Layout.cshtml` | Cao | File cốt lõi giao diện, thanh điều hướng. | Nhớ biến `@RenderBody()` ở giữa trang. |
| **Frontend** | `Views/Quiz/Detail.cshtml` | Cao | Có chứa AJAX / Fetch API phức tạp. | Nhớ cách bắt sự kiện submit, lấy value form. |
| **Frontend** | `wwwroot/css/site.css` | Trung bình | Custom CSS đè lên Bootstrap. | Nhớ chỗ để sửa style. |
| **Database** | `Data/AppDbContext.cs` | Rất Cao | Nơi đăng ký toàn bộ Entity. | Nhớ các DbSet, kế thừa IdentityDbContext. |
| **Database** | `Data/SeedData.cs` | Cao | Hiểu cách dữ liệu mẫu được nạp vào DB. | Nhớ hàm Initialize, AddRange và SaveChanges. |
| **Database** | `Models/ApplicationUser.cs` | Trung bình | Model kế thừa IdentityUser. | Nhớ các property thêm vào như XP, Level. |
| **AI** | `Services/GeminiAiService.cs` | Rất Cao | Trái tim của mọi xử lý AI. | Nhớ cách tạo HTTP Request, gắn header API Key, xử lý Json, catch exception. |
| **AI** | `Controllers/AiController.cs` | Cao | Hiểu cách Frontend đẩy data lên Backend gọi AI. | Nhớ các class RequestModel, hàm `Post`. |

---

# PHẦN 7: TÓM TẮT HỌC NHANH TRƯỚC KHI VẤN ĐÁP

* **Backend cần nhớ:**
  - Quy trình chạy: Request -> Route -> Controller -> DbContext/Service -> Model -> View -> Response.
  - Từ khóa: `[Authorize]` chặn khách, `[HttpPost]` nhận form.
  - Identity lo việc tạo cookie session.
* **Frontend cần nhớ:**
  - View viết bằng `HTML + C# (Razor)`.
  - CSS dùng thư viện `Bootstrap 5` (lưới 12 cột).
  - Không muốn tải lại trang thì dùng `AJAX/fetch API`.
  - Popup đẹp xài `SweetAlert2`.
* **Data/Database cần nhớ:**
  - `Entity Framework Core` là thư viện ORM, thay thế gõ SQL thuần.
  - Quan hệ 1-Nhiều dùng khóa ngoại (Ví dụ: `int LessonId`).
  - Migration dùng để Update DB từ cấu trúc Code.
  - Lệnh lưu: `_db.SaveChangesAsync()`.
* **AI cần nhớ:**
  - Gemini là API bên thứ 3. Giao tiếp qua `HttpClient`.
  - Có cơ chế **Fallback**: API sập/hết giới hạn thì trả về dữ liệu mẫu (được gán cứng trong code) thay vì báo lỗi đỏ trang web.
  - API Key được bảo mật ở file `appsettings.json` backend.

---

# PHẦN 8: CHECKLIST DEMO CHO NHÓM

- [ ] **Lệnh chạy:** Mở terminal tại thư mục gốc, gõ `dotnet run` (hoặc nhấn nút Run trong Visual Studio). Chờ console báo `Now listening on: https://localhost:7034`.
- [ ] **Tài khoản test (nếu seed sẵn):**
      - Admin: `admin@lingotone.vn` / `Admin@123`
      - Hoặc tự tạo một User mới bằng nút "Đăng ký" trên web.
- [ ] **Các màn hình cần mở tab sẵn (để demo cho mượt):**
      - Trang chủ (Home).
      - Lộ trình bài học HSK 1.
      - Chức năng Chat Roleplay (đóng vai AI).
      - Chức năng Chấm điểm bài viết.
- [ ] **Các thao tác nên test kĩ trước:**
      - Thử bấm "Nộp bài" ở Quiz để xem điểm lên chuẩn chưa.
      - Xem "Sửa bài viết" AI trả về nhanh hay chậm. Nếu chậm hãy giải thích khéo với thầy là do mạng hoặc API Google.
- [ ] **Các lỗi thường gặp và cách giải thích:**
      - Lỗi AI báo chữ "Dữ liệu mô phỏng": Giải thích là "Dạ do quota free của Google giới hạn số request/phút, hệ thống của tụi em đã tự động chuyển qua chế độ Fallback (mock data) để user vẫn trải nghiệm được giao diện".
      - Lỗi không đăng nhập được Google: Giải thích là "Dạ do Secret Key cũ đã bị vô hiệu hóa hoặc cấu hình cổng Localhost thay đổi".

---

# PHẦN 9: THỐNG KÊ CODE PROJECT

Nhóm có thể dùng các số liệu này để báo cáo quy mô dự án trước Hội đồng:
* **Tổng số Controller chính:** `15` Controllers (xử lý toàn bộ logic routing, bài học, ngữ pháp, flashcard, ai, auth, admin...).
* **Tổng số Model/Entity chính:** `23` Models.
* **Tổng số Service chính:** `5` Services (GeminiAiService, HskLessonService, DataSeeder, EmailSender...).
* **Hệ thống Testing:** Có sẵn các dự án Test tự động (`LingoToneMVC.Tests`, `LingoToneMVC.E2ETests`) bao gồm **45 Unit/E2E Tests**. Đây là điểm cộng lớn về tư duy phát triển phần mềm chuẩn mực.
* **Số lượng tính năng AI tích hợp:** Rất phong phú (Roleplay, Grammar check, Writing check, Srs flashcards, Generate quiz...).

**Mỗi thành viên hãy chọn 1-2 tính năng mình hiểu rõ nhất để tự tin demo và thuyết trình! Chúc nhóm bảo vệ thành công!**
