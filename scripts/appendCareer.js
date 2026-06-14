const fs = require('fs');

const appendText = `
// ===== AI CAREER CHINESE DATA =====
const careerChineseData = {
    'it': {
        name: 'Công nghệ thông tin',
        icon: '💻',
        words: [
            { char: '电脑', pinyin: 'diàn nǎo', meaning: 'Máy tính' },
            { char: '软件', pinyin: 'ruǎn jiàn', meaning: 'Phần mềm' },
            { char: '编程', pinyin: 'biān chéng', meaning: 'Lập trình' },
            { char: '代码', pinyin: 'dài mǎ', meaning: 'Mã code' },
            { char: '网络', pinyin: 'wǎng luò', meaning: 'Mạng lưới' }
        ]
    },
    'logistics': {
        name: 'Logistics',
        icon: '🚢',
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
        icon: '📢',
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

    const savedCareer = localStorage.getItem('selectedCareer');
    if (!savedCareer) {
        showCareerOnboarding();
    } else {
        loadCareerWidget(savedCareer);
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
        optionsHtml += \`
            <div class="career-option" onclick="selectCareer('\${key}')">
                <span class="career-icon">\${data.icon}</span>
                <span class="career-name">\${data.name}</span>
            </div>
        \`;
    }

    overlay.innerHTML = \`
        <div class="glass-card char-modal" style="max-width: 500px; width: 90%; text-align: center; padding: 40px 30px;">
            <div style="width: 60px; height: 60px; background: linear-gradient(135deg, #8B5CF6, #3B82F6); border-radius: 16px; display: flex; align-items: center; justify-content: center; margin: 0 auto 20px;">
                <i class="fa-solid fa-briefcase text-white fs-3"></i>
            </div>
            <h3 class="fw-bold mb-2" style="background: linear-gradient(135deg, #1e293b, #3b82f6); -webkit-background-clip: text; -webkit-text-fill-color: transparent;">Lựa chọn lộ trình chuyên ngành</h3>
            <p class="text-secondary mb-4">LingoTone AI sẽ thiết kế lộ trình học từ vựng riêng biệt phù hợp với mục tiêu nghề nghiệp của bạn.</p>
            <div class="career-grid">
                \${optionsHtml}
            </div>
        </div>
    \`;

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

function loadCareerWidget(key) {
    const data = careerChineseData[key];
    if (!data) return;

    document.getElementById('career-title').innerHTML = \`\${data.icon} Lộ trình: <span style="color:#3b82f6;">\${data.name}</span>\`;
    
    let wordsHtml = '';
    data.words.forEach(word => {
        wordsHtml += \`
            <div class="career-word-card">
                <div class="d-flex justify-content-between align-items-center mb-1">
                    <span class="fw-bold fs-4 text-dark">\${word.char}</span>
                    <button class="btn btn-sm btn-light rounded-circle" style="width:28px;height:28px;padding:0;" onclick="speak('\${word.char}')">
                        <i class="fa-solid fa-volume-high text-secondary" style="font-size:12px;"></i>
                    </button>
                </div>
                <div class="text-success fw-medium" style="font-size:14px;">\${word.pinyin}</div>
                <div class="text-secondary mt-1" style="font-size:13px; line-height:1.2;">\${word.meaning}</div>
            </div>
        \`;
    });

    document.getElementById('career-words-container').innerHTML = wordsHtml;
    document.getElementById('career-chinese-widget').style.display = 'block';
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
`;

const path = 'd:\\\\Visual tím\\\\WebTiengTrung\\\\LingoToneMVC\\\\wwwroot\\\\js\\\\site.js';
fs.appendFileSync(path, appendText, 'utf8');
console.log('Appended career JS');
