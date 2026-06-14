const fs = require('fs');

const appendText = `
// ===== AI HANZI STORY DICTIONARY =====
const aiHanziStories = {
    '休': {
        analysis: 'Ghép từ bộ Nhân (亻 - người) và bộ Mộc (木 - cái cây).',
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
        story: 'Dưới mái nhà có người phụ nữ chăm lo gia đình thì nhà cửa sẽ êm ấm. Mang nghĩa "Bình an".'
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
        storyHtml = \`
            <div class="mt-4 p-3 animate-fade-in" style="background: linear-gradient(135deg, rgba(139, 92, 246, 0.1), rgba(59, 130, 246, 0.1)); border: 1px solid rgba(139, 92, 246, 0.2); border-radius: 16px;">
                <div class="d-flex align-items-center gap-2 mb-2">
                    <div style="width: 24px; height: 24px; background: linear-gradient(135deg, #8B5CF6, #3B82F6); border-radius: 6px; display: flex; align-items: center; justify-content: center;">
                        <i class="fa-solid fa-wand-magic-sparkles text-white" style="font-size: 10px;"></i>
                    </div>
                    <span class="fw-bold" style="color: #6d28d9; font-size: 14px;">AI Giải nghĩa từ nguyên</span>
                </div>
                <p style="font-size: 13px; color: #4c1d95; margin-bottom: 6px;"><strong>Phân tích:</strong> \${storyData.analysis}</p>
                <p style="font-size: 13px; color: #4c1d95; margin-bottom: 0; font-style: italic;"><strong>Câu chuyện:</strong> \${storyData.story}</p>
            </div>
        \`;
    } else {
        storyHtml = \`
            <div class="mt-4 p-3" style="background: rgba(0,0,0,0.03); border-radius: 16px; text-align: center;">
                <i class="fa-solid fa-microchip text-secondary mb-2 fs-5"></i>
                <p class="text-secondary m-0" style="font-size: 13px;">AI đang thu thập dữ liệu từ nguyên cho chữ này...</p>
            </div>
        \`;
    }

    overlay.innerHTML = \`
        <div class="glass-card char-modal" style="max-width: 400px; width: 90%; max-height: 90vh; overflow-y: auto;">
            <div class="d-flex justify-content-between align-items-start mb-3">
                <div class="char-modal-big" style="background: linear-gradient(135deg, #2dd4bf, #3b82f6); -webkit-background-clip: text; -webkit-text-fill-color: transparent;">\${char}</div>
                <button onclick="document.getElementById('char-modal-overlay').remove()" 
                    style="background:none;border:none;font-size:20px;cursor:pointer;color:#94a3b8;padding:4px;">
                    <i class="fas fa-times"></i>
                </button>
            </div>
            <div class="char-modal-row"><span>Pinyin</span><span style="color:#10B981;font-size:18px; font-weight:bold;">\${pinyin}</span></div>
            <div class="char-modal-row"><span>Hán Việt</span><span class="text-dark fw-medium">\${hanviet}</span></div>
            <div class="char-modal-row"><span>Nghĩa</span><span class="text-dark">\${meaning}</span></div>
            <div class="char-modal-row"><span>Bộ thủ</span><span class="text-dark">\${radical}</span></div>
            <div class="char-modal-row"><span>Số nét</span><span class="text-dark">\${strokes}</span></div>
            <div class="char-modal-row"><span>HSK</span><span><span class="char-hsk">\${hsk}</span></span></div>
            
            \${storyHtml}
        </div>\`;

    document.body.appendChild(overlay);
}
`;

fs.appendFileSync('./wwwroot/js/site.js', appendText, 'utf8');
console.log('Appended missing functions');
