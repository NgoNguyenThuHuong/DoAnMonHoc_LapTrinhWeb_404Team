// ============================================================
// LingoTone MVC - site.js
// Client-side interactivity
// ============================================================

// ===== DARK MODE =====
const DARK_KEY = 'lingotone_dark';
let isDark = localStorage.getItem(DARK_KEY) === 'true';

function applyTheme() {
    if (isDark) {
        document.documentElement.classList.add('dark');
        document.body.classList.add('dark');
        const icon = document.getElementById('theme-icon');
        if (icon) { icon.classList.remove('fa-moon'); icon.classList.add('fa-sun'); }
    } else {
        document.documentElement.classList.remove('dark');
        document.body.classList.remove('dark');
        const icon = document.getElementById('theme-icon');
        if (icon) { icon.classList.remove('fa-sun'); icon.classList.add('fa-moon'); }
    }
}

function toggleTheme() {
    isDark = !isDark;
    localStorage.setItem(DARK_KEY, isDark);
    applyTheme();
}

// ===== TOAST NOTIFICATION =====
function showToast(message, type = 'success', duration = 2500) {
    const existing = document.querySelectorAll('.toast');
    existing.forEach(t => t.remove());

    const icons = { success: 'fa-check-circle', error: 'fa-times-circle', info: 'fa-info-circle' };
    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    toast.innerHTML = `<i class="fas ${icons[type] || icons.success}"></i> ${message}`;
    document.body.appendChild(toast);

    setTimeout(() => {
        toast.style.animation = 'fadeOut 0.3s ease forwards';
        setTimeout(() => toast.remove(), 300);
    }, duration);
}

// ===== XP POPUP =====
function showXpPopup(xpAmount) {
    const existing = document.querySelectorAll('.xp-popup');
    existing.forEach(p => p.remove());

    const popup = document.createElement('div');
    popup.className = 'xp-popup';
    popup.innerHTML = `⚡ +${xpAmount} XP`;
    document.body.appendChild(popup);

    setTimeout(() => popup.remove(), 2200);
}

// ===== UPDATE HEADER XP/LEVEL =====
function updateHeaderStats(xp, level, streak) {
    const xpEls = document.querySelectorAll('.header-xp');
    const levelEls = document.querySelectorAll('.header-level');
    const streakEls = document.querySelectorAll('.header-streak');
    xpEls.forEach(el => el.textContent = xp);
    levelEls.forEach(el => el.textContent = level);
    streakEls.forEach(el => el.textContent = streak + ' ngày');
}

// ===== ANIMATE PROGRESS BARS =====
function animateProgressBars() {
    document.querySelectorAll('.progress-fill[data-width]').forEach(bar => {
        const target = bar.getAttribute('data-width');
        setTimeout(() => { bar.style.width = target + '%'; }, 100);
    });
}

// ===== LESSON COMPLETE HANDLER =====
function completeLesson(lessonId, token) {
    const btn = document.getElementById('complete-btn');
    if (btn) {
        btn.disabled = true;
        btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Đang xử lý...';
    }

    fetch(`/Lesson/Complete/${lessonId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token
        }
    })
    .then(r => r.json())
    .then(data => {
        if (data.success) {
            showXpPopup(data.xpEarned || 100);
            showToast('🎉 Hoàn thành bài học! +' + (data.xpEarned || 100) + ' XP', 'success', 3000);
            updateHeaderStats(data.xp, data.level);

            if (btn) {
                btn.innerHTML = '✅ Đã hoàn thành bài học!';
                btn.style.background = 'linear-gradient(135deg, #10B981, #059669)';
            }

            const completedMsg = document.getElementById('completed-msg');
            if (completedMsg) completedMsg.style.display = 'block';

            // Update XP display in header
            const xpEls = document.querySelectorAll('.header-xp');
            xpEls.forEach(el => { el.textContent = data.xp; el.style.animation = 'xpPop 0.4s ease'; });
        } else {
            showToast(data.message || 'Bài học này đã hoàn thành rồi!', 'info');
            if (btn) {
                btn.disabled = false;
                btn.innerHTML = '✅ Hoàn thành bài học (+100 XP)';
            }
        }
    })
    .catch(() => {
        showToast('Có lỗi xảy ra, vui lòng thử lại!', 'error');
        if (btn) {
            btn.disabled = false;
            btn.innerHTML = '✅ Hoàn thành bài học (+100 XP)';
        }
    });
}

// ===== SRS FLASHCARD =====
let cardFlipped = false;

function flipCard() {
    const card = document.getElementById('srs-flashcard');
    if (!card) return;
    cardFlipped = !cardFlipped;
    card.classList.toggle('flipped', cardFlipped);
}

function reviewSrsCard(cardId, remembered, token) {
    const remBtn = document.getElementById('srs-remember-btn');
    const forgetBtn = document.getElementById('srs-forget-btn');
    if (remBtn) remBtn.disabled = true;
    if (forgetBtn) forgetBtn.disabled = true;

    fetch('/Srs/Review', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token
        },
        body: `cardId=${cardId}&remembered=${remembered}`
    })
    .then(r => r.json())
    .then(data => {
        if (data.success) {
            showXpPopup(data.xpEarned);
            updateHeaderStats(data.xp, data.level);

            if (data.remaining > 0 && data.nextCardId) {
                // Update card
                cardFlipped = false;
                const srsCard = document.getElementById('srs-flashcard');
                if (srsCard) srsCard.classList.remove('flipped');

                document.getElementById('srs-word').textContent = data.nextWord;
                document.getElementById('srs-pinyin').textContent = data.nextPinyin;
                document.getElementById('srs-meaning').textContent = data.nextMeaning;
                document.getElementById('srs-card-id').value = data.nextCardId;
                document.getElementById('srs-counter').textContent = `Còn ${data.remaining} thẻ cần ôn`;

                // Update level dots
                updateSrsLevelDots(data.nextSrsLevel || 0);

                if (remBtn) remBtn.disabled = false;
                if (forgetBtn) forgetBtn.disabled = false;
            } else {
                // All done
                const container = document.getElementById('srs-content');
                if (container) {
                    container.innerHTML = `
                        <div class="srs-done animate-bounce-in">
                            <i class="fas fa-check-circle"></i>
                            <h3>Tốt lắm! 🎉</h3>
                            <p>Hôm nay bạn đã ôn hết thẻ rồi.</p>
                            <p style="margin-top:8px;font-size:13px;color:#10B981;">Tổng XP: ${data.xp} | Level: ${data.level}</p>
                        </div>`;
                }
            }
        }
    })
    .catch(() => {
        showToast('Có lỗi xảy ra!', 'error');
        if (remBtn) remBtn.disabled = false;
        if (forgetBtn) forgetBtn.disabled = false;
    });
}

function updateSrsLevelDots(level) {
    const dots = document.querySelectorAll('.srs-level-dot');
    dots.forEach((dot, i) => {
        dot.classList.toggle('filled', i < level);
    });
}

// ===== QUIZ HANDLER =====
let quizAnswers = [];
let currentQuizIndex = 0;
let quizScore = 0;
let quizXP = 0;

function initQuiz(totalQuestions) {
    quizAnswers = new Array(totalQuestions).fill(-1);
    currentQuizIndex = 0;
    quizScore = 0;
    quizXP = 0;
    showQuizQuestion(0);
}

function selectQuizOption(questionIndex, optionIndex) {
    quizAnswers[questionIndex] = optionIndex;

    // Visual feedback
    const options = document.querySelectorAll(`.quiz-option[data-q="${questionIndex}"]`);
    options.forEach(opt => opt.classList.remove('selected'));
    const selected = document.querySelector(`.quiz-option[data-q="${questionIndex}"][data-a="${optionIndex}"]`);
    if (selected) selected.classList.add('selected');

    // Auto advance after 0.5s
    setTimeout(() => {
        if (questionIndex < quizAnswers.length - 1) {
            showQuizQuestion(questionIndex + 1);
        } else {
            showSubmitBtn();
        }
    }, 500);
}

function showQuizQuestion(index) {
    const questions = document.querySelectorAll('.quiz-question-block');
    questions.forEach((q, i) => {
        q.style.display = i === index ? 'block' : 'none';
    });
    currentQuizIndex = index;

    // Update progress bar
    const progressBar = document.getElementById('quiz-progress-bar');
    if (progressBar) {
        progressBar.style.width = ((index + 1) / questions.length * 100) + '%';
    }

    const counterEl = document.getElementById('quiz-counter');
    if (counterEl) counterEl.textContent = `Câu ${index + 1}/${questions.length}`;
}

function showSubmitBtn() {
    const submitArea = document.getElementById('quiz-submit-area');
    if (submitArea) submitArea.style.display = 'flex';
}

function submitQuiz(token) {
    const submitBtn = document.getElementById('quiz-submit-btn');
    if (submitBtn) {
        submitBtn.disabled = true;
        submitBtn.textContent = 'Đang chấm...';
    }

    const answersParam = quizAnswers.map((a, i) => `answers[${i}]=${a}`).join('&');

    fetch('/Quiz/Submit', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token
        },
        body: answersParam
    })
    .then(r => r.json())
    .then(data => {
        if (data.success) {
            // Show correct/wrong
            if (data.results) {
                data.results.forEach(r => {
                    const options = document.querySelectorAll(`.quiz-option[data-q]`);
                    // highlight results
                });
            }

            showQuizResult(data.score, data.total, data.xpEarned, data.xp, data.level);
            if (data.xpEarned > 0) showXpPopup(data.xpEarned);
        }
    })
    .catch(() => {
        showToast('Có lỗi xảy ra!', 'error');
        if (submitBtn) { submitBtn.disabled = false; submitBtn.textContent = 'Nộp bài'; }
    });
}

function showQuizResult(score, total, xpEarned, totalXp, level) {
    const container = document.getElementById('quiz-container');
    if (!container) return;

    const percent = Math.round(score / total * 100);
    let emoji = percent >= 80 ? '🏆' : percent >= 60 ? '⭐' : '💪';
    let msg = percent >= 80 ? 'Xuất sắc!' : percent >= 60 ? 'Khá tốt!' : 'Cố gắng thêm!';

    container.innerHTML = `
        <div class="text-center animate-fade-in p-4">
            <div class="display-1 mb-3">${emoji}</div>
            <h3 class="fw-bold text-dark">${msg}</h3>
            <p class="text-secondary mb-4">Bạn đạt ${score}/${total} câu đúng.</p>
            <div class="d-flex justify-content-center gap-3">
                <div class="p-3 bg-light rounded-4 text-center" style="min-width:100px;">
                    <div class="text-warning fw-bold fs-4">+${xpEarned}</div>
                    <div class="text-secondary" style="font-size:13px;">XP</div>
                </div>
            </div>
            <button onclick="window.location.reload()" class="btn btn-primary mt-4 rounded-pill px-4 py-2">Làm lại bài tập</button>
        </div>`;
}

// ===== AI GRAMMAR CORRECTOR =====
async function correctGrammarInput() {
    const inputEl = document.getElementById('grammar-input');
    if (!inputEl) return;
    const input = inputEl.value.trim();
    if (!input) {
        if (typeof showToast === 'function') showToast('Vui lòng nhập câu tiếng Trung!', 'warning');
        return;
    }

    const resultDiv = document.getElementById('grammar-result');
    if (!resultDiv) return;
    
    resultDiv.style.display = 'block';
    resultDiv.innerHTML = '<div class="text-center text-secondary"><i class="fas fa-spinner fa-spin"></i> AI đang phân tích...</div>';

    try {
        const res = await fetch('/Ai/CorrectGrammar', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ sentence: input })
        });

        if (!res.ok) throw new Error("API Error");
        const json = await res.json();

        if (json.isCorrect !== undefined) {
            let html = '';
            if (json.isCorrect) {
                html += `<div class="text-success fw-bold mb-2"><i class="fas fa-check-circle"></i> Câu này chuẩn rồi!</div>`;
            } else {
                html += `<div class="text-danger fw-bold mb-2"><i class="fas fa-times-circle"></i> Có lỗi ngữ pháp!</div>`;
                html += `<div class="text-dark mb-2"><strong>Câu sửa:</strong> <span class="text-success">${json.correction}</span></div>`;
                if (json.errors && Array.isArray(json.errors)) {
                    html += `<div class="text-danger mb-2" style="font-size:13px;"><strong>Lỗi:</strong> ${json.errors.join(', ')}</div>`;
                }
            }
            html += `<div class="text-secondary" style="font-size:13px;"><strong>Dịch:</strong> ${json.translation}</div>`;
            if (json.explanation) {
                html += `<hr class="my-2"><div class="text-muted" style="font-size:13px;"><em>Giải thích:</em> ${json.explanation}</div>`;
            }
            resultDiv.innerHTML = html;
        } else {
            throw new Error("Lỗi định dạng");
        }
    } catch (e) {
        console.error(e);
        resultDiv.innerHTML = '<div class="text-danger text-center"><i class="fa-solid fa-triangle-exclamation"></i> Lỗi kết nối AI. Vui lòng thử lại.</div>';
    }
}

// ===== INIT =====
document.addEventListener('DOMContentLoaded', function () {
    applyTheme();

    // Theme toggle
    const themeBtn = document.getElementById('theme-toggle');
    if (themeBtn) themeBtn.addEventListener('click', toggleTheme);

    // Animate progress bars
    animateProgressBars();

    // Active menu item based on current URL
    const path = window.location.pathname.toLowerCase();
    document.querySelectorAll('.menu-item[href]').forEach(item => {
        const href = item.getAttribute('href').toLowerCase();
        if (href !== '/' && path.startsWith(href)) {
            item.classList.add('active');
        } else if (href === '/' && path === '/') {
            item.classList.add('active');
        }
    });
});

function getCurrentReturnUrl() {
    return window.location.pathname + window.location.search;
}

function showAuthModal() {
    const modal = document.getElementById('authRequiredModal');
    if (!modal) return;

    const returnUrl = encodeURIComponent(getCurrentReturnUrl());

    const loginBtn = document.getElementById('authModalLoginBtn');
    const registerBtn = document.getElementById('authModalRegisterBtn');

    if (loginBtn) loginBtn.href = '/Account/Login?returnUrl=' + returnUrl;
    if (registerBtn) registerBtn.href = '/Account/Register?returnUrl=' + returnUrl;

    modal.classList.add('show');
}

function hideAuthModal() {
    const modal = document.getElementById('authRequiredModal');
    if (modal) modal.classList.remove('show');
}

function requireAuth(event) {
    if (window.isAuthenticated === true || window.isAuthenticated === 'true') {
        return true;
    }

    if (event) {
        event.preventDefault();
        event.stopPropagation();
    }

    showAuthModal();
    return false;
}

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.auth-required').forEach(function (el) {
        el.addEventListener('click', function (event) {
            return requireAuth(event);
        });
    });
});





// ===== AI HANZI STORY DICTIONARY =====
const aiHanziStories = {
    '?': {
        analysis: 'Gh�p t? b? Nh�n (? - ngu?i) v� b? M?c (? - c�i c�y).',
        story: 'H�nh ?nh m?t ngu?i dang t?a lung v�o g?c c�y d? ngh? m?t. T? d� mang nghia l� "Ngh? ngoi".'
    },
    '?': {
        analysis: 'Gh�p t? b? Nh?t (? - m?t tr?i) v� b? Nguy?t (? - m?t trang).',
        story: 'M?t tr?i v� m?t trang l� hai v?t s�ng nh?t. Khi d?ng c?nh nhau t?o ra �nh s�ng r?c r?, mang nghia "S�ng s?a".'
    },
    '?': {
        analysis: 'Gh�p t? b? N? (? - ph? n?) v� b? T? (? - d?a tr?).',
        story: 'Ph? n? sinh du?c con c�i l� m?t di?u v� c�ng t?t d?p v� h?nh ph�c. Do d� ch? n�y mang nghia "T?t", "Kh?e".'
    },
    '?': {
        analysis: 'Gh�p t? b? �i?n (? - ru?ng) v� b? L?c (? - s?c m?nh).',
        story: 'Ngu?i d�ng s?c (l?c) d? c�y c?y tr�n c�nh d?ng (di?n) ch�nh l� ngu?i d�n �ng tr? c?t gia d�nh.'
    },
    '?': {
    '休': {
        analysis: 'Ghép từ bộ Nhân (人 - người) và bộ Mộc (木 - cái cây).',
        story: 'Hình ảnh một người đang tựa lưng vào gốc cây để nghỉ mệt. Từ đó mang nghĩa là "Nghỉ ngơi".'
    },
    '明': {
        analysis: 'Ghép từ bộ Nhật (日 - mặt trời) và bộ Nguyệt (月 - mặt trăng).',
        story: 'Mặt trời và mặt trăng là hai vật sáng nhất. Khi đứng cạnh nhau tạo ra ánh sáng rực rỡ, mang nghĩa "Sáng sủa".'
    },
    '好': {
        analysis: 'Ghép từ bộ Nữ (女 - phụ nữ) và bộ Tử (子 - đứa trẻ).',
        story: 'Phụ nữ sinh được con cái là một điều vô cùng tốt đẹp và hạnh phúc. Do đó chữ này mang nghĩa "Tốt", "Khỏe".'
    },
    '男': {
        analysis: 'Ghép từ bộ Điền (田 - ruộng) và bộ Lực (力 - sức mạnh).',
        story: 'Người dùng sức (lực) để cày cấy trên cánh đồng (điền) chính là người đàn ông trụ cột gia đình.'
    },
    '安': {
        analysis: 'Ghép từ bộ Miên (宀 - mái nhà) và bộ Nữ (女 - phụ nữ).',
        story: 'Dưới mái nhà có người phụ nữ chăm lo gia đình thì nhà cửa sẽ ấm êm. Mang nghĩa "Bình an".'
    },
    '看': {
        analysis: 'Ghép từ bộ Thủ (手 - bàn tay, biến thể) ở trên và bộ Mục (目 - con mắt) ở dưới.',
        story: 'Đưa bàn tay lên che trên mắt để tránh nắng, giúp nhìn cho rõ hơn. Mang nghĩa "Nhìn", "Xem".'
    },
    '家': {
        analysis: 'Ghép từ bộ Miên (宀 - mái nhà) và bộ Thỉ (豕 - con lợn).',
        story: 'Thời xưa, nhà nào có nuôi lợn dưới gầm sàn mới thực sự là một hộ gia đình ấm no. Mang nghĩa "Gia đình".'
    }
};

// ===== CHARACTER MODAL =====
function showCharModal(id, char, pinyin, hanviet, meaning, radical, strokes, hsk) {
    const existing = document.getElementById('char-modal-overlay');
    if (existing) existing.remove();

    const overlay = document.createElement('div');
    overlay.id = 'char-modal-overlay';
    overlay.className = 'char-modal-overlay';
    overlay.onclick = function(e) { if (e.target === overlay) overlay.remove(); };

    // AI Story Block
    let storyHtml = '';
    const storyData = aiHanziStories[char];
    if (storyData) {
        storyHtml = `
            <div class="mt-4 p-3 animate-fade-in" style="background: linear-gradient(135deg, rgba(139, 92, 246, 0.1), rgba(59, 130, 246, 0.1)); border: 1px solid rgba(139, 92, 246, 0.2); border-radius: 16px;">
                <div class="d-flex align-items-center gap-2 mb-2">
                    <div style="width: 24px; height: 24px; background: linear-gradient(135deg, #8B5CF6, #3B82F6); border-radius: 6px; display: flex; align-items: center; justify-content: center;">
                        <i class="fa-solid fa-wand-magic-sparkles text-white" style="font-size: 10px;"></i>
                    </div>
                    <span class="fw-bold" style="color: #6d28d9; font-size: 14px;">AI Giải nghĩa từ nguyên</span>
                </div>
                <p style="font-size: 13px; color: #4c1d95; margin-bottom: 6px;"><strong>Phân tích:</strong> ${storyData.analysis}</p>
                <p style="font-size: 13px; color: #4c1d95; margin-bottom: 0; font-style: italic;"><strong>Câu chuyện:</strong> ${storyData.story}</p>
            </div>
        `;
    } else {
        storyHtml = `
            <div class="mt-4 p-3" style="background: rgba(0,0,0,0.03); border-radius: 16px; text-align: center;">
                <i class="fa-solid fa-microchip text-secondary mb-2 fs-5"></i>
                <p class="text-secondary m-0" style="font-size: 13px;">AI đang thu thập dữ liệu từ nguyên cho chữ này...</p>
            </div>
        `;
    }

    overlay.innerHTML = `
        <div class="glass-card char-modal" style="max-width: 400px; width: 90%; max-height: 90vh; overflow-y: auto;">
            <div class="d-flex justify-content-between align-items-start mb-3">
                <div class="char-modal-big" style="background: linear-gradient(135deg, #2dd4bf, #3b82f6); -webkit-background-clip: text; -webkit-text-fill-color: transparent;">${char}</div>
                <button onclick="document.getElementById('char-modal-overlay').remove()" 
                    style="background:none;border:none;font-size:20px;cursor:pointer;color:#94a3b8;padding:4px;">
                    <i class="fas fa-times"></i>
                </button>
            </div>
            <div class="char-modal-row"><span>Pinyin</span><span style="color:#10B981;font-size:18px; font-weight:bold;">${pinyin}</span></div>
            <div class="char-modal-row"><span>Hán Việt</span><span class="text-dark fw-medium">${hanviet}</span></div>
            <div class="char-modal-row"><span>Nghĩa</span><span class="text-dark">${meaning}</span></div>
            <div class="char-modal-row"><span>Bộ thủ</span><span class="text-dark">${radical}</span></div>
            <div class="char-modal-row"><span>Số nét</span><span class="text-dark">${strokes}</span></div>
            <div class="char-modal-row"><span>HSK</span><span><span class="char-hsk">${hsk}</span></span></div>
            
            ${storyHtml}
        </div>`;

    document.body.appendChild(overlay);
}

// ===== AI CAREER CHINESE DATA =====
const careerChineseData = {
    'it': {
        name: 'Công nghệ thông tin',
        icon: '💻',
        words: [
            { char: '电脑', pinyin: 'diàn nǎo', meaning: 'Máy tính' },
            { char: '软件', pinyin: 'ruǎn jiàn', meaning: 'Phần mềm' },
            { char: '编程', pinyin: 'biān chéng', meaning: 'Lập trình' },
            { char: '代码', pinyin: 'dài mǎ', meaning: 'Mã nguồn' },
            { char: '网络', pinyin: 'wǎng luò', meaning: 'Mạng lưới' }
        ]
    },
    'logistics': {
        name: 'Logistics',
        icon: '🚚',
        words: [
            { char: '仓库', pinyin: 'cāng kù', meaning: 'Nhà kho' },
            { char: '运输', pinyin: 'yùn shū', meaning: 'Vận chuyển' },
            { char: '海关', pinyin: 'hǎi guān', meaning: 'Hải quan' },
            { char: '出口', pinyin: 'chū kǒu', meaning: 'Xuất khẩu' },
            { char: '物流', pinyin: 'wù liú', meaning: 'Logistics' }
        ]
    },
    'marketing': {
        name: 'Marketing',
        icon: '📈',
        words: [
            { char: '广告', pinyin: 'guǎng gào', meaning: 'Quảng cáo' },
            { char: '品牌', pinyin: 'pǐn pái', meaning: 'Thương hiệu' },
            { char: '市场', pinyin: 'shì chǎng', meaning: 'Thị trường' },
            { char: '策略', pinyin: 'cè lüè', meaning: 'Chiến lược' },
            { char: '客户', pinyin: 'kè hù', meaning: 'Khách hàng' }
        ]
    },
    'accounting': {
        name: 'Kế toán',
        icon: '📊',
        words: [
            { char: '会计', pinyin: 'kuài jì', meaning: 'Kế toán' },
            { char: '财务', pinyin: 'cái wù', meaning: 'Tài vụ' },
            { char: '发票', pinyin: 'fā piào', meaning: 'Hóa đơn' },
            { char: '税务', pinyin: 'shuì wù', meaning: 'Thuế' },
            { char: '利润', pinyin: 'lì rùn', meaning: 'Lợi nhuận' }
        ]
    },
    'tourism': {
        name: 'Du lịch',
        icon: '✈️',
        words: [
            { char: '旅游', pinyin: 'lǚ yóu', meaning: 'Du lịch' },
            { char: '导游', pinyin: 'dǎo yóu', meaning: 'Hướng dẫn viên' },
            { char: '酒店', pinyin: 'jiǔ diàn', meaning: 'Khách sạn' },
            { char: '机票', pinyin: 'jī piào', meaning: 'Vé máy bay' },
            { char: '护照', pinyin: 'hù zhào', meaning: 'Hộ chiếu' }
        ]
    }
};

// ===== AI CAREER CHINESE LOGIC =====
document.addEventListener('DOMContentLoaded', () => {
    // Only run on dashboard (check if widget exists)
    const widget = document.getElementById('career-chinese-widget');
    if (!widget) return;

    const obDataStr = localStorage.getItem('lingotone_onboarding');
    if (!obDataStr) {
        window.location.href = '/Onboarding';
    } else {
        try {
            const obData = JSON.parse(obDataStr);
            if (obData && obData.career) {
                // Also update the roleplay button URL
                const roleplayBtn = document.getElementById('career-roleplay-btn');
                if (roleplayBtn) {
                    roleplayBtn.href = `/Roleplay?career=${obData.career}`;
                }
                loadCareerWidget(obData.career);
            }
        } catch(e) {
            console.error(e);
        }
    }
});

function showCareerOnboarding() {
    const existing = document.getElementById('career-modal-overlay');
    if (existing) existing.remove();

    const overlay = document.createElement('div');
    overlay.id = 'career-modal-overlay';
    overlay.className = 'char-modal-overlay animate-fade-in d-flex align-items-center justify-content-center';
    overlay.style.zIndex = '9999';

    let optionsHtml = '';
    for (const [key, data] of Object.entries(careerChineseData)) {
        optionsHtml += `
            <div class="career-option" onclick="selectCareer('${key}')">
                <span class="career-icon">${data.icon}</span>
                <span class="career-name">${data.name}</span>
            </div>
        `;
    }

    overlay.innerHTML = `
        <div class="glass-card char-modal" style="max-width: 500px; width: 90%; text-align: center; padding: 40px 30px;">
            <div style="width: 60px; height: 60px; background: linear-gradient(135deg, #8B5CF6, #3B82F6); border-radius: 16px; display: flex; align-items: center; justify-content: center; margin: 0 auto 20px;">
                <i class="fa-solid fa-briefcase text-white fs-3"></i>
            </div>
            <h3 class="fw-bold mb-2" style="background: linear-gradient(135deg, #1e293b, #3b82f6); -webkit-background-clip: text; -webkit-text-fill-color: transparent;">L?a ch?n l? tr�nh chuy�n ng�nh</h3>
            <p class="text-secondary mb-4">LingoTone AI s? thi?t k? l? tr�nh h?c t? v?ng ri�ng bi?t ph� h?p v?i m?c ti�u ngh? nghi?p c?a b?n.</p>
            <div class="career-grid">
                ${optionsHtml}
            </div>
        </div>
    `;

    document.body.appendChild(overlay);
}

function selectCareer(key) {
    localStorage.setItem('selectedCareer', key);
    const modal = document.getElementById('career-modal-overlay');
    if (modal) {
        modal.classList.add('animate-fade-out');
        setTimeout(() => {
            modal.remove();
            loadCareerWidget(key);
        }, 300);
    }
}

async function loadCareerWidget(key) {
    const dataFallback = careerChineseData[key];
    const container = document.getElementById('career-words-container');
    const titleEl = document.getElementById('career-title');
    
    if (!dataFallback) return;
    
    titleEl.innerHTML = `${dataFallback.icon} Lộ trình: <span style="color:#3b82f6;">${dataFallback.name}</span> <span class="spinner-border spinner-border-sm text-primary ms-2" id="career-loading" role="status"></span>`;
    container.innerHTML = '<div class="text-center text-muted w-100 p-3"><i class="fas fa-robot"></i> AI đang phân tích lộ trình học cho bạn...</div>';
    document.getElementById('career-chinese-widget').style.display = 'block';

    try {
        const req = {
            career: dataFallback.name,
            level: 'Cơ bản',
            goal: 'Giao tiếp công việc',
            minutesPerDay: 20
        };
        const res = await fetch('/Ai/GenerateCareerRoadmap', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(req)
        });
        
        if (!res.ok) throw new Error("API Error");
        const json = await res.json();
        
        if (json.dailyVocabulary && Array.isArray(json.dailyVocabulary)) {
            renderCareerWords(json.dailyVocabulary);
            document.getElementById('career-loading')?.remove();
            return;
        }
    } catch (e) {
        console.log("Real AI failed, using fallback.", e);
    }

    // Fallback to hardcoded
    document.getElementById('career-loading')?.remove();
    renderCareerWords(dataFallback.words);
}

function renderCareerWords(words) {
    let wordsHtml = '';
    if (!words || !Array.isArray(words)) return;
    words.forEach(word => {
        wordsHtml += `
            <div class="career-word-card d-flex flex-column align-items-center justify-content-center text-center" style="flex: 1; min-width: 90px; padding: 12px 8px;">
                <div class="fw-bold text-dark mb-1" style="font-size: 20px;">${word.char}</div>
                <div class="fw-medium mb-1" style="font-size:12px; color: #1e293b;">${word.pinyin}</div>
                <div class="text-secondary mb-3" style="font-size:11px; line-height:1.3; max-width: 100%; word-wrap: break-word;">${word.meaning}</div>
                <button class="btn btn-sm btn-light rounded-circle mt-auto" style="width:28px;height:28px;padding:0; display:flex; align-items:center; justify-content:center; border: 1px solid rgba(0,0,0,0.05);" onclick="speak('${word.char}')">
                    <i class="fa-solid fa-volume-high text-secondary" style="font-size:11px;"></i>
                </button>
            </div>
        `;
    });
    document.getElementById('career-words-container').innerHTML = wordsHtml;
}

function speak(text) {
    const msg = new SpeechSynthesisUtterance();
    msg.text = text;
    msg.lang = 'zh-CN';
    window.speechSynthesis.speak(msg);
}

function changeCareer() {
    localStorage.removeItem('selectedCareer');
    showCareerOnboarding();
}
