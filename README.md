PHẦN 1: PHÂN TÍCH HỆ THỐNG & KIẾN TRÚC
1. Kiến trúc tổng thể (System Architecture)

graph TD
    Client[Web Client Vue.js 3] --> CDN[CloudFront/CDN]
    Client --> SignalR[SignalR Hub - Realtime]
    Client --> API[ASP.NET Core Web API]
    
    API --> Redis[Redis Cache]
    API --> SQL[SQL Server]
    API --> Azure[Azure Blob Storage]
    
    API --> AI_GW[AI Gateway]
    AI_GW --> STT[Azure Speech-to-Text / Whisper]
    AI_GW --> LLM[Azure OpenAI GPT-4o / DeepSeek]
    AI_GW --> TTS[Azure Text-to-Speech / ElevenLabs]
    AI_GW --> PRON[Azure Pronunciation Assessment]
    
    SignalR --> Redis_Pub[Redis Pub/Sub]

Nguyên tắc:

Clean Architecture: Domain (Entities, Interfaces), Application (Use Cases), Infrastructure (Services, Data), Presentation (API, Hubs).

CQRS Pattern cho phần AI và Progress tracking.

Event-driven cho Streak, Badge, Leaderboard.

2. Database Design (ERD)
Bảng chính:

Users

Id, Email, PasswordHash, FullName, CurrentHSKLevel, Xp, StreakDays, LastActiveDate, IsDarkMode, AvatarUrl

UserSettings (1-1 với User): AiVoiceId, LearningGoal, DailyTargetMinutes

Vocabularies:

Id, Word, Pinyin, Meaning, HSKLevel, Topic (Giao tiếp: Hỏi đường, Nhà hàng...), AudioUrl

UserVocabProgress:

UserId, VocabularyId, MasteryLevel (0-5), LastReviewed, IsBookmarked

Lessons:

Id, Title, HSKLevel, Topic, LessonType (Listening, Reading, Speaking), ContentJson

QuizQuestions: Id, LessonId, QuestionType, OptionsJson, CorrectAnswer

UserQuizAttempts: Id, UserId, Score, TimeSpent, CompletedAt

AIConversationSessions:

Id, UserId, Role (Friend, Teacher, Waiter), HSKLevel, StartedAt

AIConversationMessages:

Id, SessionId, Role (User/AI), Text, AudioUrl, PronunciationScore, Timestamp

PronunciationPracticeHistory:

Id, UserId, OriginalText, UserAudioUrl, AccuracyScore, ToneScore, FluencyScore, FeedbackJson

Leaderboard: View tính toán từ UserStats hoặc bảng DailyUserStats.

3. API Design (RESTful + SignalR)
REST API (ASP.NET Core)
csharp
// Auth
POST /api/auth/register
POST /api/auth/login

// Vocabulary
GET /api/vocab?hskLevel=2&topic=restaurant
POST /api/vocab/{id}/bookmark
POST /api/vocab/{id}/review (cập nhật mastery)

// Lessons
GET /api/lessons?type=reading&level=3
GET /api/lessons/{id}/quiz

// Progress
GET /api/users/me/progress/dashboard
GET /api/users/me/streak

// Pronunciation
POST /api/pronunciation/assess (gửi audio file)

// AI Chat
POST /api/ai/session/create
GET /api/ai/session/{id}/history
SignalR Hub (Realtime)
IChatHub: SendVoiceChunk, ReceiveAIResponse, ReceiveTranscript, ReceiveTypingAnimation

IPronunciationHub: SendAudioStream, ReceiveWaveformData, ReceiveScoreUpdate

4. AI Service Architecture
yaml
AI Orchestrator:
  - Input: User voice stream, Conversation context, User HSK level.
  - STT: Azure Speech-to-Text (with custom acoustic model for Chinese learners).
  - LLM Prompt Engineering:
    "Bạn là nhân viên nhà hàng, HSK cấp 2. 
     Người học yếu thanh điệu. Hãy nói chậm, dùng câu ngắn, 
     nếu họ sai -> lặp lại đúng 1 lần.
     Response format: JSON { text, pinyin, difficulty_adjusted }"
  - TTS: Azure Neural TTS (cn-CN-XiaoxiaoNeural) - realtime streaming.
  - Pronunciation Assessment: Azure Cognitive Services - Chấm tone, phoneme, fluency.
PHẦN 2: THIẾT KẾ UI/UX CHI TIẾT (Phong cách HiHSK + Duolingo + ChatGPT)
Theme: Chủ đạo là màu Xanh Mint (#4CD964), Trắng ngà, Xám nhạt, Cam (#FF9500) cho highlight. Dark mode dùng nền đen carbon, chữ xám nhạt.

1. Trang Home (Sau Login)
text
+---------------------------------------------------+
| [Logo] LingoTone  |  HSK2 ▼  | 🔍 | 🔔 | 👤Avatar |
+---------------------------------------------------+
|                                                   |
|  Xin chào, Minh! 🔥 7 ngày streak | 🎯 Hôm nay: 50/100 XP |
|  [Thanh XP] ============░░░░░░░░                   |
|                                                   |
|  +-------------------------------------------------+
|  | 📢 TIẾP TỤC BÀI HỌC             |  🎲 Mini Game  |
|  | Bài 5: Gọi món trong nhà hàng   |  [Chơi ngay]   |
|  | [Tiếp tục học]                   |               |
|  +-------------------------------------------------+
|                                                   |
|  🧠 LUYỆN TẬP NHANH                               |
|  [Flashcard]  [Nghe]  [Đọc]  [Viết]  [🎙️ AI Chat] |
|                                                   |
|  📚 HỌC THEO CHỦ ĐỀ GIAO TIẾP                     |
|  [Chào hỏi] [Mua sắm] [Sân bay] [Bệnh viện] ...   |
|                                                   |
|  🏆 Bảng xếp hạng tuần                            |
|  1. Thanh - 2450 XP   2. Lan - 2100 XP ...       |
+---------------------------------------------------+
2. Trang AI Chat (Realtime Voice)
text
+---------------------------------------------------+
| ← Quay lại          Đang nói với: AI - Nhân viên sân bay (HSK3) |
|                                   [⚙️ Đổi vai]    |
+---------------------------------------------------+
|                                                   |
|  [Avatar AI]   Bạn: Chào, tôi muốn đổi vé.        |
|  [Anim đang nói] AI: 您好，请出示您的机票和护照。  |
|                (Nính hǎo, qǐng chūshì nín de jīpiào hé hùzhào) |
|                                                   |
|  [Avatar AI]   Bạn: Toi muon... (sai tone)        |
|  AI: ★★★★☆ (4/5) - Sai thanh 3 của "muốn". Hãy thử lại: "Wǒ xiǎng" |
|                                                   |
+---------------------------------------------------+
| 🎙️ [Nhấn giữ để nói] ⚡Đang nghe... 🔄 Realtime Transcript: |
| "Tôi muốn..."                                     |
+---------------------------------------------------+
| [Điểm phát âm] Thanh điệu: 85% | Độ trôi chảy: 70% |
+---------------------------------------------------+
3. Trang Luyện Speaking (Pronunciation)
text
+---------------------------------------------------+
| 🎤 Luyện tập: Thanh điệu "Mā, Má, Mǎ, Mà"         |
+---------------------------------------------------+
|                                                   |
|  [Waveform Animation] ▁▂▃▄▅▆▇██▇▆▅▄▃▂▁            |
|                                                   |
|  Màn hình hiển thị tone:                          |
|     [Mā] Cao ngang: Bạn: --- (Đúng) ✓             |
|     [Má] Lên cao: Bạn: --- (Sai, ra thanh 3) ✗   |
|                                                   |
|  Feedback:                                        |
|  ❌ Từ "Má": Bạn phát âm giống "Mǎ" (thanh 3).   |
|     Hãy kéo cao giọng hơn ở cuối.                |
|  🎯 Độ chính xác tổng: 78%                       |
|                                                   |
|  [Thử lại]  [Bài tiếp theo]                      |
+---------------------------------------------------+
4. Dashboard & Dark Mode
Dark Mode (Toggle góc phải):

Nền chính: #1A1A1A

Card: #2D2D2D

Border: #3D3D3D

Chữ: #E0E0E0

Xanh mint: #30D158

Dashboard: Biểu đồ tiến độ HSK1->6, số từ đã nhớ, giờ học/tuần, biểu đồ sai thanh điệu nhiều nhất.

PHẦN 3: FOLDER STRUCTURE & MODULES
text
LingoTone.sln
├── src/
│   ├── Domain/ (Entities, Enums, Interfaces)
│   ├── Application/ (UseCases: LoginCommand, AssessPronunciation, ChatWithAI)
│   ├── Infrastructure/
│   │   ├── Data/ (DbContext, Repositories)
│   │   ├── Services/ (AzureSTTService, OpenAIService, SignalRHub)
│   │   └── Cache/ (RedisService)
│   └── WebAPI/
│       ├── Controllers/
│       ├── Hubs/ (ChatHub, PronunciationHub)
│       └── Middlewares/
├── client/ (Vue.js 3)
│   ├── src/
│   │   ├── views/ (Home, Chat, Speaking, Dashboard, Profile, Flashcard)
│   │   ├── components/ (AudioWaveform, AiAvatar, ScoreRing, StreakCalendar)
│   │   ├── composables/ (useRealtimeSpeech, usePronunciation, useSignalR)
│   │   ├── stores/ (pinia: user, lesson, aiSession)
│   │   └── assets/ (dark.css, light.css)
PHẦN 4: AI NỔI BẬT & HƯỚNG MỞ RỘNG
Các tính năng AI đặc sắc:
AI Role-play với Context Window nhớ tới 10 turn – tự động giảm độ khó nếu user sai nhiều.

Tone Visualization 3D – hiển thị đường cong thanh điệu của user chồng lên mẫu chuẩn.

Speaking Score Breakdown: Tone (40%) + Pronunciation (30%) + Fluency (20%) + Speed (10%).

Smart Recommendation: Dùng Collaborative Filtering đề xuất bài học dựa trên lỗi sai của user.

Hướng mở rộng:
Mobile App (Flutter/React Native) dùng chung API.

AI Video Call (WebRTC + Azure Video Indexer).

Gamification nâng cao (Huy hiệu, Vật phẩm, Bảng xếp hạng theo khu vực).

Xuất báo cáo PDF tiến độ gửi giáo viên.

Hỗ trợ đa ngôn ngữ (Tiếng Anh, Việt, Nhật) cho UI.

PHẦN 5: TECHNICAL GUIDELINES (CHO TEAM DEV)
AI Models đề xuất:
Tác vụ	Model khuyến nghị
Speech-to-Text (CN)	Azure Cognitive Services (Custom model) hoặc Whisper V3
Pronunciation Assessment	Azure Pronunciation Assessment API
LLM cho role-play	GPT-4o (chi phí cao) hoặc DeepSeek-V2 (tối ưu tiếng Trung)
Text-to-Speech realtime	Azure Neural TTS (Websocket streaming)
Recommendation	Matrix Factorization (ML.NET) + Redis cache
Cache Strategy:
Redis: Session AI Chat (lưu 15 phút), HSK vocabulary lists, User streak tạm thời.

CDN: Audio files (TTS cached), Avatars.

Scaling Strategy:
Horizontal scaling cho ASP.NET Core + SignalR (dùng Redis backplane).

Separate AI Worker (Background service xử lý pronunciation async).

Rate limiting cho API AI (dùng Azure API Management).

Authentication Flow:
text
Vue.js -> OTP/Password -> ASP.NET Core Identity -> JWT Token (HttpOnly Cookie) + Refresh Token.
SignalR kết nối dùng AccessToken query string.
PHẦN 6: KẾT LUẬN & LỘ TRÌNH PHÁT TRIỂN (MVP 3 tháng)
Month 1: Clean Architecture + Auth + Vocabulary + Flashcard + Quiz (No AI).
Month 2: SignalR Chat + Pronunciation Assessment + Azure STT/TTS.
Month 3: AI Role-play + Dashboard analytics + Dark mode + Optimization.

Điểm mạnh khiến dự án này nổi bật với giám khảo/doanh nghiệp:

Ứng dụng AI thực tế không phải demo.

Tối ưu cho người Việt học tiếng Trung (so sánh thanh điệu).

Realtime kiến trúc enterprise (Clean Architecture, SignalR, Redis).

Trải nghiệm người dùng đẳng cấp (lấy cảm hứng Duolingo + ChatGPT).
