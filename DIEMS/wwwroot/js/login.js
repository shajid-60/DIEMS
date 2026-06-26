// ============================================================
// DIEMS — Login Page Logic
// ============================================================

/* ── Particle Field ─────────────────────────────────────────── */
(function createParticles() {
  const field = document.getElementById('particle-field');
  if (!field) return;

  const colors = ['rgba(255,78,0,', 'rgba(255,140,0,', 'rgba(255,214,0,', 'rgba(41,121,255,'];

  for (let i = 0; i < 40; i++) {
    const p = document.createElement('div');
    p.className = 'particle';

    const size  = Math.random() * 4 + 1.5;
    const color = colors[Math.floor(Math.random() * colors.length)];
    const left  = Math.random() * 100;
    const dur   = 8 + Math.random() * 14;
    const delay = Math.random() * dur * -1;
    const drift = (Math.random() - 0.5) * 80;

    p.style.cssText = `
      width: ${size}px;
      height: ${size}px;
      background: ${color}0.7);
      box-shadow: 0 0 ${size * 2}px ${color}0.5);
      left: ${left}%;
      bottom: -10px;
      animation-duration: ${dur}s;
      animation-delay: ${delay}s;
      --drift: ${drift}px;
    `;

    field.appendChild(p);
  }
})();

/* ── Role Selection ─────────────────────────────────────────── */
window.selectRole = function (role, btn) {
  document.querySelectorAll('.role-btn').forEach(b => b.classList.remove('active'));
  btn.classList.add('active');
  document.getElementById('login-id').placeholder = {
    admin:     'Enter admin ID (e.g. ADMIN-001)',
    official:  'Enter employee code (e.g. GOV-1234)',
    responder: 'Enter responder ID (e.g. RESP-5678)',
    citizen:   'Enter NID or mobile number',
  }[role] || 'Enter your user ID';
};

/* ── Password Toggle ────────────────────────────────────────── */
window.togglePassword = function () {
  const input = document.getElementById('login-pass');
  input.type  = input.type === 'password' ? 'text' : 'password';
};

/* ── Login Form Submit ──────────────────────────────────────── */
window.handleLogin = function (e) {
  e.preventDefault();

  const btn = document.getElementById('login-submit');
  btn.classList.add('loading');
  btn.disabled = true;

  // Simulate auth
  setTimeout(() => {
    btn.classList.remove('loading');
    btn.disabled = false;

    // Demo: any credentials go to dashboard
    window.location.href = 'pages/dashboard.html';
  }, 1800);
};

/* ── Live Stat Counter ──────────────────────────────────────── */
(function animateCounters() {
  function animateValue(el, start, end, dur) {
    let startTime = null;
    function step(timestamp) {
      if (!startTime) startTime = timestamp;
      const progress = Math.min((timestamp - startTime) / dur, 1);
      const val = Math.floor(progress * (end - start) + start);
      el.textContent = val.toLocaleString();
      if (progress < 1) requestAnimationFrame(step);
    }
    requestAnimationFrame(step);
  }

  document.querySelectorAll('.stat-num').forEach((el, i) => {
    const raw = el.textContent.replace(/,/g, '');
    const num = parseInt(raw, 10);
    if (!isNaN(num)) {
      setTimeout(() => animateValue(el, 0, num, 1800), i * 200 + 600);
    }
  });
})();

/* ── Simulate Live Alert Blink ──────────────────────────────── */
(function liveDisasterCount() {
  const el = document.getElementById('active-disasters');
  if (!el) return;

  let count = 14;
  setInterval(() => {
    const delta = Math.random() < 0.5 ? 0 : (Math.random() < 0.5 ? 1 : -1);
    count = Math.max(8, Math.min(22, count + delta));
    el.textContent = count;
  }, 8000);
})();
