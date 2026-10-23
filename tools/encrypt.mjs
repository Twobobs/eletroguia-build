// Gera Resources/app.bin a partir de html/Eletroguia.html e grava a chave
// (dividida em duas metades) dentro de Program.cs.
// Rode de novo sempre que o HTML mudar, antes de publicar:
//   node tools/encrypt.mjs
import fs from "fs";
import crypto from "crypto";

const html = fs.readFileSync("html/Eletroguia.html");

const key = crypto.randomBytes(32);
const mask = crypto.randomBytes(32);
const nonce = crypto.randomBytes(12);

const cipher = crypto.createCipheriv("aes-256-gcm", key, nonce);
const ct = Buffer.concat([cipher.update(html), cipher.final()]);
const tag = cipher.getAuthTag();

fs.writeFileSync("Resources/app.bin", Buffer.concat([nonce, tag, ct]));

const ka = [...key].map((b, i) => b ^ mask[i]);
const km = [...mask];

let cs = fs.readFileSync("Program.cs", "utf8");
cs = cs.replace(
  /\/\/KEY-BEGIN[\s\S]*?\/\/KEY-END/,
  `//KEY-BEGIN
    static readonly byte[] KA = { ${ka.join(", ")} };
    static readonly byte[] KM = { ${km.join(", ")} };
    //KEY-END`
);
fs.writeFileSync("Program.cs", cs);

console.log(`ok: ${html.length} bytes de HTML -> ${ct.length + 28} bytes cifrados em Resources/app.bin`);
