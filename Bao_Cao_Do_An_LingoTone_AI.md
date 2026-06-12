# BÁO CÁO ĐỒ ÁN: LINGOTONE AI – WEBSITE HỌC TIẾNG TRUNG THÔNG MINH

## LỜI MỞ ĐẦU
Trong bối cảnh hội nhập quốc tế, tiếng Trung đang trở thành một trong những ngôn ngữ phổ biến và quan trọng nhất. Nhu cầu học và thi chứng chỉ HSK ngày càng cao. Tuy nhiên, việc học tiếng Trung trực tuyến hiện nay còn gặp nhiều trở ngại như: nội dung khô khan, thiếu tính tương tác, và đặc biệt là thiếu môi trường thực hành thực tế. 
Với mong muốn giải quyết những khó khăn đó, đề tài **"LingoTone AI – Website học tiếng Trung thông minh theo lộ trình HSK 1–6"** được thực hiện nhằm mang đến một nền tảng học tập kết hợp công nghệ AI, Gamification và các phương pháp ôn tập thông minh, giúp người học duy trì động lực và đạt hiệu quả cao nhất.

## LỜI CẢM ƠN
Em xin gửi lời cảm ơn chân thành đến Giảng viên hướng dẫn đã tận tình chỉ bảo, định hướng và cung cấp những kiến thức quý báu giúp em hoàn thành đồ án này. Em cũng xin cảm ơn nhà trường đã tạo môi trường học tập và nghiên cứu lý tưởng. Trong quá trình thực hiện không tránh khỏi những thiếu sót, em rất mong nhận được sự góp ý của quý thầy cô.

## LỜI CAM ĐOAN
Em xin cam đoan đồ án "LingoTone AI" là sản phẩm nghiên cứu và phát triển của riêng cá nhân/nhóm. Các tài liệu tham khảo, mã nguồn sử dụng đều được trích dẫn rõ ràng. Mọi kết quả trong báo cáo này là trung thực và chưa từng được công bố trong bất kỳ công trình nào khác.

---

[TỰ ĐỘNG CHÈN MỤC LỤC TẠI ĐÂY]
[TỰ ĐỘNG CHÈN DANH MỤC HÌNH ẢNH TẠI ĐÂY]
[TỰ ĐỘNG CHÈN DANH MỤC BẢNG BIỂU TẠI ĐÂY]

---

## CHƯƠNG 1: TỔNG QUAN ĐỀ TÀI

### 1.1. Lý do chọn đề tài
Nhu cầu học tiếng Trung đang tăng mạnh ở mọi lứa tuổi, đặc biệt là mục tiêu thi chứng chỉ HSK. Dù vậy, người học thường gặp khó khăn trong việc ghi nhớ Hán tự, phát âm chuẩn thanh điệu và duy trì động lực do thiếu môi trường luyện tập hội thoại. Các nền tảng học ngoại ngữ hiện tại đa phần thiếu sự tùy biến và tính năng AI hỗ trợ chuyên sâu. Do đó, LingoTone AI ra đời nhằm khắc phục các điểm yếu này thông qua một hệ sinh thái học tập toàn diện.

### 1.2. Mục tiêu đề tài
Xây dựng một website học tiếng Trung theo lộ trình HSK 1–6 với các mục tiêu:
- Cung cấp hệ thống bài học đa dạng: Từ vựng, Ngữ pháp, Hội thoại.
- Ứng dụng thuật toán sinh Quiz động (Dynamic Quiz) để tối ưu việc luyện tập.
- Tích hợp Gamification (XP, Combo, Leaderboard) để tăng hứng thú học tập.
- Tích hợp AI (Google Gemini API) để hỗ trợ nhập vai hội thoại (Roleplay) và tra cứu Hán tự thông minh.

### 1.3. Đối tượng và phạm vi nghiên cứu
- **Đối tượng sử dụng:** Người tự học tiếng Trung, học sinh, sinh viên, người đi làm có nhu cầu ôn thi HSK từ cấp 1 đến 6.
- **Phạm vi nghiên cứu:** 
  - Nền tảng: Web Application.
  - Nội dung: Chương trình HSK 1–6.
  - Chức năng: Đăng nhập, Học bài, Quiz, Bảng xếp hạng, Tương tác AI.
- **Giới hạn hiện tại:** Chưa hỗ trợ ứng dụng Mobile, chưa tích hợp cổng thanh toán khóa học.

### 1.4. Phương pháp thực hiện
- Phương pháp khảo sát và thu thập yêu cầu: Nghiên cứu các ứng dụng học ngoại ngữ hiện có (như Duolingo, SuperChinese).
- Phương pháp phân tích, thiết kế hệ thống hướng đối tượng (UML).
- Phương pháp phát triển phần mềm linh hoạt (Agile) để liên tục cập nhật tính năng.

### 1.5. Công nghệ sử dụng
- **Backend:** C#, ASP.NET Core MVC (.NET 8), Entity Framework Core.
- **Frontend:** Razor Pages, HTML5, CSS3, JavaScript, Bootstrap.
- **Database:** SQL Server.
- **AI Integration:** Google Gemini API.
- **Công cụ phát triển:** Visual Studio 2022, SSMS.

---

## CHƯƠNG 2: CƠ SỞ LÝ THUYẾT VÀ CÔNG NGHỆ

### 2.1. Nền tảng ASP.NET Core MVC và .NET 8
ASP.NET Core là framework mã nguồn mở đa nền tảng của Microsoft. Kiến trúc MVC (Model-View-Controller) giúp chia tách rõ ràng giữa phần xử lý logic (Controller), giao diện hiển thị (View) và dữ liệu (Model). Phiên bản .NET 8 mang lại hiệu suất tối ưu và khả năng mở rộng tốt, cực kỳ phù hợp cho ứng dụng web đòi hỏi tốc độ xử lý nhanh gọn.

### 2.2. Entity Framework Core và SQL Server
EF Core là một ORM (Object-Relational Mapper) mạnh mẽ, cho phép tương tác với CSDL SQL Server thông qua các đối tượng C# mà không cần viết quá nhiều câu lệnh SQL thuần. Hệ thống sử dụng EF Core để quản lý cấu trúc các thực thể phức tạp như `Users`, `Lessons`, `QuizResults`.

### 2.3. Google Gemini API
Gemini API cung cấp các mô hình ngôn ngữ lớn (LLM) tiên tiến. Trong LingoTone AI, Gemini được sử dụng chủ lực để:
- Đóng vai nhân vật trong tính năng AI Roleplay.
- Phân tích và giải thích cấu trúc Hán tự.
- Hỗ trợ sửa lỗi câu thông qua tính năng Writing Coach.

### 2.4. Hệ thống HSK và nhu cầu trực tuyến
HSK (Hanyu Shuiping Kaoshi) là bài thi kiểm tra năng lực Hán ngữ tiêu chuẩn quốc tế. Hệ thống học HSK chia thành 6 cấp độ từ cơ bản (HSK 1) đến thành thạo (HSK 6). Việc chuyển dịch sang nền tảng số hóa giúp người học tiết kiệm chi phí mua giáo trình và chủ động thời gian luyện tập.

### 2.5. Phương pháp giáo dục hiện đại: Gamification, SRS và Dynamic Quiz
- **Gamification:** Ứng dụng các yếu tố trò chơi (điểm kinh nghiệm XP, chuỗi ngày học Streak, bảng xếp hạng Leaderboard) vào quá trình học tập để tăng tính cạnh tranh và động lực.
- **SRS (Spaced Repetition System):** Phương pháp lặp lại ngắt quãng giúp ghi nhớ từ vựng lâu dài.
- **Dynamic Quiz:** Thay vì lưu cứng (hardcode) hàng nghìn câu hỏi cố định, hệ thống tự động sinh câu hỏi và đáp án nhiễu theo thời gian thực dựa trên tệp từ vựng của mỗi bài học.

---

## CHƯƠNG 3: PHÂN TÍCH VÀ THIẾT KẾ HỆ THỐNG

### 3.1. Khảo sát và phân tích yêu cầu
**3.1.1. Yêu cầu chức năng**
- Quản lý tài khoản: Đăng ký, Đăng nhập, Xem Dashboard.
- Quản lý quá trình học: Truy cập danh sách HSK 1-6, học từ vựng (Hán tự, Pinyin, nghĩa), ngữ pháp, hội thoại.
- Luyện tập: Làm bài Quiz động theo bài, tính điểm XP, thống kê kết quả.
- Tính năng AI: Tương tác Roleplay, Tra cứu Hán tự, Writing Coach, HSK Estimator.

**3.1.2. Yêu cầu phi chức năng**
- Giao diện thân thiện, chuẩn UI/UX, tương thích đa nền tảng (Responsive).
- Hiệu năng mượt mà: Thuật toán trộn câu hỏi sinh Quiz xử lý dưới 1 giây.
- Bảo mật thông tin người dùng và bảo mật API Key của hệ thống AI.

### 3.2. Thiết kế Use Case
**3.2.1. Sơ đồ Use Case tổng quát**
Hệ thống xoay quanh Actor chính là **Người học (User)** với các Use Case: Học bài, Làm Quiz, Xem Leaderboard, Tương tác AI.
*(Chèn sơ đồ Use Case tổng quát)*

**3.2.2. Sơ đồ Use Case chi tiết: Làm Quiz**
- Actor: User.
- Mô tả: User chọn bài học, hệ thống sinh câu hỏi, User chọn đáp án, hệ thống tính điểm Combo/XP và lưu lịch sử vào Database.
*(Chèn sơ đồ Use Case chi tiết Làm Quiz)*

**3.2.3. Sơ đồ Use Case chi tiết: Tương tác AI Roleplay**
- Actor: User.
- Mô tả: User gửi thông điệp, hệ thống điều phối gọi Gemini API, lấy phản hồi và hiển thị cho User trên giao diện chat.
*(Chèn sơ đồ Use Case chi tiết Tương tác AI Roleplay)*

### 3.3. Thiết kế luồng hoạt động (Activity Diagram)
**3.3.1. Luồng Dynamic Quiz Generation**
- Bước 1: User truy cập trang Quiz theo `lessonId`.
- Bước 2: Hệ thống lấy thông tin Lesson.
- Bước 3: Kiểm tra DB, nếu không có câu hỏi dựng sẵn -> Truy xuất `Vocabularies` của bài.
- Bước 4: Sinh câu hỏi Pinyin / Nghĩa tiếng Việt ngẫu nhiên.
- Bước 5: Bốc ngẫu nhiên từ vựng khác để tạo đáp án nhiễu.
- Bước 6: Xáo trộn (Shuffle) danh sách và trả về giao diện trắc nghiệm.
*(Chèn Activity Diagram: Luồng Sinh Quiz Động)*

**3.3.2. Luồng hoàn thành bài học và cộng XP**
- Bước 1: User truy cập trang chi tiết bài học.
- Bước 2: User xem tuần tự các tab (Từ vựng, Ngữ pháp, Hội thoại, Quiz, AI).
- Bước 3: User bấm nút "Hoàn thành bài học".
- Bước 4: Hệ thống kiểm tra điều kiện (Đã xem đủ tab chưa?).
  - Nếu chưa: Báo lỗi.
  - Nếu đủ: Kiểm tra tiếp đã từng hoàn thành chưa.
- Bước 5: Nếu chưa từng hoàn thành -> Lưu `UserLessonProgress`, cộng 100 XP.
*(Chèn Activity Diagram: Luồng hoàn thành bài học)*

### 3.4. Thiết kế biểu đồ tuần tự (Sequence Diagram)
**Luồng tương tác Gemini API**
- `View` -> User nhập yêu cầu -> Gửi Request tới `AiController`.
- `AiController` -> Gọi hàm trong `AiService`.
- `AiService` -> Gắn Prompt ngữ cảnh, bọc dữ liệu -> Gọi tới `Google Gemini API`.
- `Gemini API` -> Xử lý mô hình ngôn ngữ -> Trả về kết quả chuỗi JSON.
- `AiService` -> Bóc tách dữ liệu JSON -> Trả kết quả về `AiController`.
- `AiController` -> Cập nhật thông tin lên `View`.
*(Chèn Sequence Diagram: Tương tác API Gemini)*

### 3.5. Thiết kế Cơ sở dữ liệu (ERD)
Dữ liệu được tổ chức chặt chẽ thông qua Entity Framework. Các bảng cốt lõi bao gồm:
- **`ApplicationUser`**: Kế thừa IdentityUser, mở rộng thêm các trường: XP, Level, Streak.
- **`Lessons`**: Lưu cấu trúc bài học (LessonId, Title, HskLevel).
- **`Vocabularies`**: Cấu trúc từ vựng (Hanzi, Pinyin, Meaning) liên kết với LessonId.
- **`QuizResults`**: Lưu trữ lịch sử bài thi để làm Leaderboard (UserId, CorrectAnswers, TotalQuestions, XP_Earned).
- **`UserLessonProgress`**: Bảng trung gian theo dõi bài học user đã hoàn thành (UserId, LessonId, IsCompleted).
*(Chèn Sơ đồ ERD và bảng mô tả chi tiết)*

---

## CHƯƠNG 4: XÂY DỰNG HỆ THỐNG

### 4.1. Môi trường phát triển và Cấu trúc dự án
Dự án được triển khai trên nền tảng Visual Studio 2022, sử dụng C# 12 và .NET 8.
Cấu trúc thư mục theo chuẩn mô hình MVC:
- `Controllers/`: Điều hướng và tiếp nhận Request (HomeController, QuizController, AiController).
- `Models/`: Các Entity ánh xạ xuống DB.
- `ViewModels/`: Các class đóng gói dữ liệu phức tạp truyền ra View.
- `Views/`: Thư mục chứa các tệp Razor (`.cshtml`).

### 4.2. Giao diện người dùng
Hệ thống được thiết kế theo phong cách hiện đại, trực quan, đề cao trải nghiệm học tập.

**1. Màn hình Dashboard**
Tổng hợp tiến độ cá nhân, cấp độ, số điểm XP, và các lối tắt truy cập nhanh vào lộ trình học.
> `[Chèn hình: Màn hình Dashboard]`

**2. Màn hình Danh sách khóa học HSK & Chi tiết bài học**
Chia lộ trình rõ ràng từ HSK 1 đến 6. Trong chi tiết bài học, nội dung được chia tab (Từ vựng, Ngữ pháp, Hội thoại...) để tối ưu không gian màn hình.
> `[Chèn hình: Màn hình Chi tiết bài học]`

**3. Màn hình Quiz Arena**
Giao diện trắc nghiệm động, thống kê ngay lập tức số câu đúng, Combo chuỗi trả lời đúng và cho phép review lại đáp án.
> `[Chèn hình: Màn hình Quiz Arena]`

**4. Màn hình Leaderboard**
Bảng xếp hạng tuần truy xuất dữ liệu từ bảng `QuizResults`, thúc đẩy sự cạnh tranh lành mạnh giữa các người học.
> `[Chèn hình: Màn hình Leaderboard]`

**5. Màn hình AI Roleplay / AI Dictionary**
Giao diện trò chuyện trực tiếp với AI. AI được cấp ngữ cảnh để phản hồi tự nhiên, giúp người học luyện phản xạ giao tiếp.
> `[Chèn hình: Màn hình AI Roleplay / AI Dictionary]`

### 4.3. Cài đặt các chức năng cốt lõi

**4.3.1. Thuật toán Dynamic Quiz Generation**
Thuật toán lấy từ vựng của bài học, bốc ngẫu nhiên các từ vựng khác trong cơ sở dữ liệu làm "đáp án nhiễu", sau đó xáo trộn. Điều này giúp loại bỏ việc gõ cứng hàng nghìn câu hỏi.

```csharp
// Thuật toán cốt lõi sinh đáp án nhiễu tại QuizService
public async Task<List<QuizQuestion>> GenerateQuizAsync(int lessonId) {
    var vocabList = await _context.Vocabularies.Where(v => v.LessonId == lessonId).ToListAsync();
    var allVocab = await _context.Vocabularies.ToListAsync();
    var questions = new List<QuizQuestion>();

    foreach (var vocab in vocabList) {
        // Lấy 3 đáp án sai ngẫu nhiên
        var wrongAnswers = allVocab.Where(v => v.Id != vocab.Id)
                                   .OrderBy(r => Guid.NewGuid())
                                   .Take(3)
                                   .Select(v => v.Meaning).ToList();
        
        var options = new List<string>(wrongAnswers) { vocab.Meaning };
        options = options.OrderBy(x => Guid.NewGuid()).ToList(); // Shuffle

        questions.Add(new QuizQuestion {
            QuestionText = $"Nghĩa của từ '{vocab.Hanzi} ({vocab.Pinyin})' là gì?",
            CorrectAnswer = vocab.Meaning,
            Options = options
        });
    }
    return questions;
}
```

**4.3.2. Logic hoàn thành bài học (Chống gian lận XP)**
Hệ thống kiểm tra kỹ lịch sử trong `UserLessonProgress` để không cộng XP nhiều lần cho cùng một bài học.

```csharp
// Đoạn code kiểm tra hoàn thành bài tại LessonController
[HttpPost]
public async Task<IActionResult> CompleteLesson(int lessonId) {
    var userId = _userManager.GetUserId(User);
    var isCompleted = await _context.UserLessonProgress
        .AnyAsync(p => p.UserId == userId && p.LessonId == lessonId);

    if (!isCompleted) {
        _context.UserLessonProgress.Add(new UserLessonProgress {
            UserId = userId, LessonId = lessonId, IsCompleted = true
        });
        
        var user = await _userManager.FindByIdAsync(userId);
        user.XP += 100; // Thưởng 100 XP
        await _context.SaveChangesAsync();
        return Json(new { success = true, message = "Bạn nhận được 100 XP!" });
    }
    return Json(new { success = false, message = "Bài học này đã hoàn thành trước đó." });
}
```

**4.3.3. Tích hợp AI Gemini**
Hệ thống sử dụng HttpClient để gửi Request lên API của Google. Việc thiết kế `System Prompt` là bước quan trọng nhất để AI hiểu nó đang đóng vai một giáo viên tiếng Trung.

---

## CHƯƠNG 5: KIỂM THỬ VÀ ĐÁNH GIÁ

### 5.1. Môi trường cài đặt và khởi chạy
- **Yêu cầu hệ thống:** Visual Studio 2022, .NET 8 SDK, SQL Server.
- **Quy trình chạy:**
  1. Cấu hình Connection String trong `appsettings.json`.
  2. Bổ sung `GeminiApiKey` tương ứng.
  3. Mở Package Manager Console, chạy lệnh `Update-Database`.
  4. Chạy lệnh `dotnet run` để khởi động Server và truy cập từ trình duyệt.

### 5.2. Kịch bản kiểm thử (Test Cases)

| Mã TC | Tên chức năng | Các bước thực hiện | Kết quả mong đợi | Trạng thái |
|---|---|---|---|---|
| **TC01** | Hoàn thành bài học | 1. Vào chi tiết bài học<br>2. Bỏ qua một số tab<br>3. Bấm "Hoàn thành"<br>4. Xem đủ tab và bấm lại | - Chưa đủ tab: Thông báo từ chối.<br>- Đủ tab: Thông báo thành công, cộng XP.<br>- Bấm lần 2: Không cộng thêm XP. | Đạt |
| **TC02** | Sinh Quiz động | 1. Vào Quiz bài `lessonId=1001`<br>2. Làm mới trang liên tục | - Quiz lấy đúng từ vựng bài 1001.<br>- Các đáp án nhiễu đảo vị trí.<br>- Không có từ vựng tiếng Anh. | Đạt |
| **TC03** | Tính điểm Quiz | 1. Làm đúng một số câu<br>2. Hoàn thành bài | - Hiển thị đúng số câu, XP và Combo.<br>- Lưu thành công xuống bảng `QuizResults`. | Đạt |
| **TC04** | Leaderboard | 1. Có điểm XP từ làm Quiz<br>2. Truy cập bảng xếp hạng | - Tên User thật xuất hiện thay vì dữ liệu cứng.<br>- XP hiển thị đúng theo thực tế. | Đạt |
| **TC05** | Tương tác AI | 1. Vào trang AI Roleplay<br>2. Nhập một câu tiếng Trung<br>3. Gửi Request | - Nhận phản hồi đúng ngữ cảnh giao tiếp.<br>- Có xử lý lỗi nếu mất mạng. | Đạt |

### 5.3. Bài học kinh nghiệm và Các lỗi đã khắc phục
Trong suốt quá trình phát triển, dự án đã ghi nhận và khắc phục thành công nhiều vấn đề logic quan trọng:
1. **Lỗi Quiz lấy sai bài học & Bị lặp câu hỏi:** 
   *Ban đầu:* Thuật toán random dễ tạo câu trùng và dữ liệu cứng không linh hoạt.
   *Khắc phục:* Chuyển sang sinh Quiz động hoàn toàn theo `lessonId`, áp dụng thuật toán Shuffle List chuẩn xác.
2. **Lỗi Đáp án nhiễu bị tiếng Anh:** 
   *Ban đầu:* Hệ thống bốc nhầm field dữ liệu tiếng Anh vào đáp án.
   *Khắc phục:* Chuẩn hóa truy vấn EF Core, chỉ lấy `Meaning` là tiếng Việt.
3. **Lỗi Leaderboard dùng dữ liệu cứng:** 
   *Ban đầu:* Bảng xếp hạng hiển thị các tên giả định (Linh, Minh, An).
   *Khắc phục:* Triển khai bảng `QuizResults` để xếp hạng người dùng từ dữ liệu thực tiễn.
4. **Lỗi Spam nút hoàn thành bài học:** 
   *Ban đầu:* Người học có thể nhấn liên tục để nhận XP không giới hạn.
   *Khắc phục:* Chặn logic cộng điểm bằng việc tra cứu bảng `UserLessonProgress`.

### 5.4. Đánh giá ưu, nhược điểm của hệ thống
**Ưu điểm:**
- Hệ thống hoạt động mượt mà, cấu trúc MVC chuẩn mực dễ dàng mở rộng.
- Khắc phục triệt để các lỗi logic về dữ liệu cứng (Hardcode) trong Quiz và Leaderboard.
- Tính năng AI mang lại trải nghiệm đột phá trong việc tự học giao tiếp, giúp LingoTone khác biệt với nhiều sản phẩm khác.

**Hạn chế:**
- Tốc độ phản hồi của AI hoàn toàn phụ thuộc vào Server của Google.
- Phiên bản hiện tại tập trung trên nền tảng Web, chưa có ứng dụng Mobile.
- AI Roleplay vẫn đang giới hạn ở dạng Text, chưa hỗ trợ Voice-to-Text realtime.

---

## KẾT LUẬN VÀ HƯỚNG PHÁT TRIỂN

**1. Kết quả đạt được**
Đồ án đã xuất sắc hoàn thành việc xây dựng website **LingoTone AI** dành cho người học HSK. Ứng dụng đáp ứng được đầy đủ các tiêu chí kỹ thuật: áp dụng thuật toán động, thao tác với CSDL phức tạp, và gọi API tích hợp trí tuệ nhân tạo. Giao diện trực quan kết hợp Gamification đã giải quyết tốt bài toán tạo động lực cho người học ngoại ngữ.

**2. Hướng phát triển tương lai**
Trong tương lai, hệ thống LingoTone AI sẽ tiếp tục được tối ưu với các mục tiêu:
- Tích hợp công nghệ nhận diện giọng nói (Speech Recognition) để tự động chấm điểm phát âm cho người học.
- Xây dựng hệ thống chức năng Thi thử HSK tính giờ mô phỏng đề thi thực tế.
- Đóng gói và phát hành ứng dụng trên các nền tảng di động Android / iOS (sử dụng công nghệ React Native hoặc .NET MAUI).
- Phát triển bảng xếp hạng theo tháng, theo giải đấu (Tournament) để tăng tính cạnh tranh.

---
## TÀI LIỆU THAM KHẢO
1. Microsoft Documentation. *ASP.NET Core MVC Overview*. 
2. Microsoft Documentation. *Entity Framework Core & SQL Server*.
3. Google AI Developers. *Gemini API Documentation*.
4. Tài liệu cấu trúc bài thi HSK chuẩn từ tổ chức Hanban.
5. Diễn đàn chia sẻ kiến thức lập trình: StackOverflow, GitHub.
