const fs = require('fs');
const path = './wwwroot/js/site.js';
const buf = fs.readFileSync(path);
// read as latin1, which preserves bytes
const str = buf.toString('latin1');
// convert those latin1 bytes representing utf8 back to string
const fixedStr = Buffer.from(str, 'latin1').toString('utf8');
fs.writeFileSync(path, fixedStr, 'utf8');
console.log('Fixed encoding!');
