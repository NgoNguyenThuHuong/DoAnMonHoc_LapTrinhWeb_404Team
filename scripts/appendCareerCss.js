const fs = require('fs');

const appendCss = `
/* ===== AI CAREER CHINESE WIDGET & MODAL ===== */
.career-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
    gap: 15px;
    margin-top: 20px;
}
.career-option {
    background: rgba(255, 255, 255, 0.5);
    border: 1px solid rgba(255, 255, 255, 0.8);
    border-radius: 16px;
    padding: 20px 10px;
    cursor: pointer;
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 10px;
}
.career-option:hover {
    transform: translateY(-5px);
    background: rgba(255, 255, 255, 0.9);
    box-shadow: 0 10px 25px -5px rgba(59, 130, 246, 0.2);
    border-color: rgba(59, 130, 246, 0.3);
}
.career-icon {
    font-size: 32px;
}
.career-name {
    font-weight: 600;
    color: #334155;
    font-size: 14px;
}
.career-words-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(130px, 1fr));
    gap: 15px;
    margin-top: 15px;
}
.career-word-card {
    background: white;
    border: 1px solid #e2e8f0;
    border-radius: 16px;
    padding: 15px;
    transition: all 0.2s ease;
    cursor: pointer;
}
.career-word-card:hover {
    transform: translateY(-3px);
    box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.05);
    border-color: #cbd5e1;
}
.widget-glass {
    background: rgba(255, 255, 255, 0.7);
    backdrop-filter: blur(20px);
    -webkit-backdrop-filter: blur(20px);
    border: 1px solid rgba(255, 255, 255, 0.6);
    border-radius: 24px;
    box-shadow: 0 4px 30px rgba(0, 0, 0, 0.03);
}
`;

const path = 'd:\\\\Visual tím\\\\WebTiengTrung\\\\LingoToneMVC\\\\wwwroot\\\\css\\\\site.css';
fs.appendFileSync(path, appendCss, 'utf8');
console.log('Appended career CSS');
