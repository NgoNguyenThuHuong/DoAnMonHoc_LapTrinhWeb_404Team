const fs = require('fs');

const path = './wwwroot/js/site.js';
let content = fs.readFileSync(path, 'utf8');

// Remove the aiHanziStories dictionary entirely
content = content.replace(/\/\/ ===== AI HANZI STORY DICTIONARY =====[\s\S]*?(?=\/\/ ===== CHARACTER MODAL =====)/, '');

// Replace showCharModal function with async fetch
const newShowCharModal = `// ===== CHARACTER MODAL =====
function showCharModal(id, char, pinyin, hanviet, meaning, radical, strokes, hsk) {
    const existing = document.getElementById('char-modal-overlay');
    if (existing) existing.remove();

    const overlay = document.createElement('div');
    overlay.id = 'char-modal-overlay';
    overlay.className = 'char-modal-overlay';
    overlay.onclick = function(e) { if (e.target === overlay) overlay.remove(); };

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
            
            <div id="ai-story-container" class="mt-4 p-3" style="background: rgba(0,0,0,0.03); border-radius: 16px; text-align: center;">
                <div class="spinner-border text-primary spinner-border-sm mb-2" role="status"></div>
                <p class="text-secondary m-0" style="font-size: 13px;">AI đang thu thập dữ liệu từ nguyên thực tế...</p>
            </div>
        </div>\`;

    document.body.appendChild(overlay);

    // Call Real AI API
    fetch(\`/Ai/AnalyzeHanzi?character=\${encodeURIComponent(char)}\`)
        .then(res => res.json())
        .then(data => {
            const container = document.getElementById('ai-story-container');
            if (container) {
                if (data.success) {
                    container.className = "mt-4 p-3 animate-fade-in";
                    container.style.cssText = "background: linear-gradient(135deg, rgba(139, 92, 246, 0.1), rgba(59, 130, 246, 0.1)); border: 1px solid rgba(139, 92, 246, 0.2); border-radius: 16px; text-align: left;";
                    container.innerHTML = \`
                        <div class="d-flex align-items-center gap-2 mb-2">
                            <div style="width: 24px; height: 24px; background: linear-gradient(135deg, #8B5CF6, #3B82F6); border-radius: 6px; display: flex; align-items: center; justify-content: center;">
                                <i class="fa-solid fa-wand-magic-sparkles text-white" style="font-size: 10px;"></i>
                            </div>
                            <span class="fw-bold" style="color: #6d28d9; font-size: 14px;">AI Giải nghĩa từ nguyên</span>
                        </div>
                        <p style="font-size: 13px; color: #4c1d95; margin-bottom: 6px;"><strong>Phân tích:</strong> \${data.analysis}</p>
                        <p style="font-size: 13px; color: #4c1d95; margin-bottom: 0; font-style: italic;"><strong>Câu chuyện:</strong> \${data.story}</p>
                    \`;
                } else {
                    container.innerHTML = \`
                        <i class="fa-solid fa-triangle-exclamation text-danger mb-2 fs-5"></i>
                        <p class="text-danger m-0" style="font-size: 13px;">\${data.message || data.analysis}</p>
                    \`;
                }
            }
        })
        .catch(err => {
            const container = document.getElementById('ai-story-container');
            if (container) {
                container.innerHTML = \`<p class="text-danger m-0" style="font-size: 13px;">Không thể kết nối đến máy chủ AI.</p>\`;
            }
        });
}`;

content = content.replace(/\/\/ ===== CHARACTER MODAL =====[\s\S]*?(?=\/\/ ===== GRAMMAR MINI QUIZ =====)/, newShowCharModal + '\n\n');

fs.writeFileSync(path, content, 'utf8');
console.log('Updated site.js with Real API call');
