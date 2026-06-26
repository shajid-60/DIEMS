// ============================================================
// DIEMS — Dashboard JS
// ============================================================

/* ── Live Clock ─────────────────────────────────────────────── */
(function liveClock() {
  function pad(n) { return String(n).padStart(2, '0'); }
  function tick() {
    const now = new Date();
    const el  = document.getElementById('live-clock');
    if (el) el.textContent = `${pad(now.getHours())}:${pad(now.getMinutes())}:${pad(now.getSeconds())}`;
  }
  tick();
  setInterval(tick, 1000);
})();

/* ── Sidebar Toggle ─────────────────────────────────────────── */
window.toggleSidebar = function() {
  document.getElementById('sidebar').classList.toggle('open');
};

/* ── Active Nav ─────────────────────────────────────────────── */
(function() {
  const path = window.location.pathname.split('/').pop();
  document.querySelectorAll('.nav-item').forEach(item => {
    item.classList.remove('active');
    if (item.getAttribute('href') === path) item.classList.add('active');
  });
})();

/* ── Counter Animations ─────────────────────────────────────── */
(function animateStats() {
  document.querySelectorAll('.qs-value').forEach((el, i) => {
    const raw = el.textContent.replace(/[^0-9]/g, '');
    const num = parseInt(raw, 10);
    if (isNaN(num) || num > 9999) return;

    let start = null;
    const duration = 1500;
    const orig = el.textContent;

    function step(ts) {
      if (!start) start = ts;
      const p = Math.min((ts - start) / duration, 1);
      const eased = 1 - Math.pow(1 - p, 3);
      const val = Math.floor(eased * num);
      el.textContent = orig.replace(/\d+/, val.toLocaleString());
      if (p < 1) requestAnimationFrame(step);
    }

    setTimeout(() => requestAnimationFrame(step), i * 100 + 300);
  });
})();

/* ── Chart.js Global Config ─────────────────────────────────── */
if (typeof Chart !== 'undefined') {
  Chart.defaults.color = '#8A9BC4';
  Chart.defaults.borderColor = 'rgba(255,255,255,0.06)';
  Chart.defaults.font.family = "'Outfit', sans-serif";

  /* ── Disaster Trend Chart ───────────────────────────────── */
  const trendCtx = document.getElementById('trend-chart');
  if (trendCtx) {
    const months = ['Jul','Aug','Sep','Oct','Nov','Dec','Jan','Feb','Mar','Apr','May','Jun'];

    window.trendChart = new Chart(trendCtx, {
      type: 'line',
      data: {
        labels: months,
        datasets: [
          {
            label: 'Floods',
            data: [4, 6, 3, 2, 1, 0, 0, 1, 2, 5, 8, 7],
            borderColor: '#2979FF',
            backgroundColor: 'rgba(41,121,255,0.08)',
            fill: true,
            tension: 0.4,
            pointRadius: 4,
            pointHoverRadius: 6,
            pointBackgroundColor: '#2979FF',
          },
          {
            label: 'Cyclones',
            data: [1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3],
            borderColor: '#AA00FF',
            backgroundColor: 'rgba(170,0,255,0.06)',
            fill: true,
            tension: 0.4,
            pointRadius: 4,
            pointHoverRadius: 6,
            pointBackgroundColor: '#AA00FF',
          },
          {
            label: 'Earthquakes',
            data: [0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1],
            borderColor: '#FF8C00',
            backgroundColor: 'rgba(255,140,0,0.06)',
            fill: true,
            tension: 0.4,
            pointRadius: 4,
            pointHoverRadius: 6,
            pointBackgroundColor: '#FF8C00',
          },
          {
            label: 'Fires',
            data: [2, 3, 4, 3, 1, 0, 0, 1, 2, 3, 4, 3],
            borderColor: '#FF3B3B',
            backgroundColor: 'rgba(255,59,59,0.06)',
            fill: true,
            tension: 0.4,
            pointRadius: 4,
            pointHoverRadius: 6,
            pointBackgroundColor: '#FF3B3B',
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        interaction: { mode: 'index', intersect: false },
        plugins: {
          legend: {
            display: true,
            position: 'top',
            labels: {
              boxWidth: 12,
              padding: 16,
              font: { size: 12 }
            }
          },
          tooltip: {
            backgroundColor: 'rgba(13,21,32,0.95)',
            borderColor: 'rgba(255,255,255,0.1)',
            borderWidth: 1,
            padding: 12,
            titleFont: { size: 13, weight: '600' },
            bodyFont:  { size: 12 }
          }
        },
        scales: {
          x: {
            grid: { color: 'rgba(255,255,255,0.04)' },
            ticks: { font: { size: 11 } }
          },
          y: {
            grid: { color: 'rgba(255,255,255,0.04)' },
            ticks: { font: { size: 11 }, stepSize: 2 },
            beginAtZero: true
          }
        }
      }
    });
  }

  /* ── Shelter Doughnut ───────────────────────────────────── */
  const shelterCtx = document.getElementById('shelter-chart');
  if (shelterCtx) {
    new Chart(shelterCtx, {
      type: 'doughnut',
      data: {
        labels: ['Occupied', 'Available', 'Reserved'],
        datasets: [{
          data: [34120, 6880, 3000],
          backgroundColor: [
            'rgba(255,78,0,0.7)',
            'rgba(0,230,118,0.7)',
            'rgba(255,214,0,0.7)'
          ],
          borderColor: ['#FF4E00','#00E676','#FFD600'],
          borderWidth: 2,
          hoverOffset: 8,
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '72%',
        plugins: {
          legend: {
            position: 'bottom',
            labels: {
              boxWidth: 10,
              padding: 12,
              font: { size: 11 }
            }
          },
          tooltip: {
            backgroundColor: 'rgba(13,21,32,0.95)',
            borderColor: 'rgba(255,255,255,0.1)',
            borderWidth: 1,
            padding: 10,
            callbacks: {
              label: ctx => ` ${ctx.label}: ${ctx.parsed.toLocaleString()}`
            }
          }
        }
      }
    });
  }

  /* ── Disaster Type Pie ──────────────────────────────────── */
  const typeCtx = document.getElementById('type-chart');
  if (typeCtx) {
    new Chart(typeCtx, {
      type: 'polarArea',
      data: {
        labels: ['Floods', 'Cyclones', 'Earthquakes', 'Fires', 'Landslides'],
        datasets: [{
          data: [7, 3, 1, 2, 1],
          backgroundColor: [
            'rgba(41,121,255,0.6)',
            'rgba(170,0,255,0.6)',
            'rgba(255,140,0,0.6)',
            'rgba(255,59,59,0.6)',
            'rgba(0,230,118,0.6)',
          ],
          borderColor: ['#2979FF','#AA00FF','#FF8C00','#FF3B3B','#00E676'],
          borderWidth: 2,
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'bottom',
            labels: { boxWidth: 10, padding: 10, font: { size: 11 } }
          },
          tooltip: {
            backgroundColor: 'rgba(13,21,32,0.95)',
            borderColor: 'rgba(255,255,255,0.1)',
            borderWidth: 1,
            padding: 10
          }
        },
        scales: {
          r: {
            grid: { color: 'rgba(255,255,255,0.06)' },
            ticks: { display: false }
          }
        }
      }
    });
  }

  /* ── Chart Tab Switch ───────────────────────────────────── */
  window.switchChart = function(type) {
    if (!window.trendChart) return;

    document.querySelectorAll('.active-tab').forEach(b => b.classList.remove('active-tab'));

    if (type === 'bar') {
      document.getElementById('tab-bar').classList.add('active-tab');
      window.trendChart.config.type = 'bar';
      window.trendChart.data.datasets.forEach(ds => { ds.fill = false; ds.tension = 0; });
    } else {
      document.getElementById('tab-line').classList.add('active-tab');
      window.trendChart.config.type = 'line';
      window.trendChart.data.datasets.forEach(ds => { ds.fill = true; ds.tension = 0.4; });
    }
    window.trendChart.update();
  };

} // end Chart.js block

/* ── Risk Heatmap ────────────────────────────────────────────── */
(function buildHeatmap() {
  const districts = [
    { name: 'Cox\'s Bazar', risk: 4 },
    { name: 'Chittagong',   risk: 4 },
    { name: 'Sylhet',       risk: 3 },
    { name: 'Sunamganj',    risk: 3 },
    { name: 'Dhaka',        risk: 2 },
    { name: 'Noakhali',     risk: 3 },
    { name: 'Feni',         risk: 3 },
    { name: 'Rangamati',    risk: 3 },
    { name: 'Khulna',       risk: 2 },
    { name: 'Barisal',      risk: 2 },
    { name: 'Patuakhali',   risk: 3 },
    { name: 'Bhola',        risk: 2 },
    { name: 'Mymensingh',   risk: 1 },
    { name: 'Rajshahi',     risk: 1 },
    { name: 'Bogra',        risk: 1 },
    { name: 'Comilla',      risk: 2 },
    { name: 'Brahmanbaria', risk: 2 },
    { name: 'Jessore',      risk: 1 },
    { name: 'Dinajpur',     risk: 0 },
    { name: 'Naogaon',      risk: 0 },
    { name: 'Pabna',        risk: 0 },
  ];

  const riskColors = {
    0: 'rgba(255,255,255,0.06)',
    1: 'rgba(255,214,0,0.25)',
    2: 'rgba(255,140,0,0.4)',
    3: 'rgba(255,78,0,0.55)',
    4: 'rgba(255,59,59,0.75)',
  };

  const riskLabels = { 0:'No Risk', 1:'Low', 2:'Moderate', 3:'High', 4:'Critical' };

  const container = document.getElementById('risk-heatmap');
  if (!container) return;

  const grid = document.createElement('div');
  grid.style.cssText = 'display:grid;grid-template-columns:repeat(7,1fr);gap:8px;';

  districts.forEach(d => {
    const cell = document.createElement('div');
    cell.style.cssText = `
      background: ${riskColors[d.risk]};
      border: 1px solid rgba(255,255,255,0.08);
      border-radius: 8px;
      padding: 10px 6px;
      text-align: center;
      cursor: pointer;
      transition: all 0.2s;
    `;
    cell.innerHTML = `
      <div style="font-size:11px;font-weight:700;color:#fff;margin-bottom:3px;">${d.name}</div>
      <div style="font-size:10px;color:rgba(255,255,255,0.55);">${riskLabels[d.risk]}</div>
    `;
    cell.addEventListener('mouseenter', () => { cell.style.transform = 'scale(1.05)'; cell.style.zIndex = '1'; });
    cell.addEventListener('mouseleave', () => { cell.style.transform = 'scale(1)'; cell.style.zIndex = ''; });
    grid.appendChild(cell);
  });

  container.appendChild(grid);
})();

/* ── Live Activity Feed ──────────────────────────────────────── */
(function liveActivity() {
  const feed = document.getElementById('activity-feed');
  if (!feed) return;

  const events = [
    { type: 'blue',   text: 'New victim registration — 142 people — Cox\'s Bazar Emergency Center' },
    { type: 'orange', text: 'Volunteer Team Delta dispatched — 45 members — Sylhet flood zone' },
    { type: 'green',  text: 'Rescue successful — 23 people airlifted from Chittagong Hills' },
    { type: 'red',    text: 'Water supply critical — Rangamati — requesting emergency refill' },
    { type: 'blue',   text: 'Ambulance #14 dispatched — Noakhali — cardiac emergency' },
    { type: 'orange', text: 'Resource allocation updated — 500 food packets sent to Shelter #08' },
  ];

  let idx = 0;
  setInterval(() => {
    const ev = events[idx % events.length];
    idx++;
    const now = new Date();
    const item = document.createElement('div');
    item.className = 'activity-item';
    item.style.animation = 'float-up 0.4s ease both';
    item.innerHTML = `
      <div class="activity-dot ${ev.type}"></div>
      <div>
        <div class="activity-text">${ev.text}</div>
        <div class="activity-time">Just now</div>
      </div>`;
    feed.insertBefore(item, feed.firstChild);
    if (feed.children.length > 10) feed.removeChild(feed.lastChild);
  }, 6000);
})();

/* ── Gauge Animation ─────────────────────────────────────────── */
(function animateGauge() {
  const circle = document.getElementById('gauge-circle');
  const numEl  = document.getElementById('gauge-num');
  if (!circle || !numEl) return;

  const circumference = 2 * Math.PI * 50;  // r=50
  const score = 87;
  const offset = circumference - (score / 100) * circumference;

  circle.style.strokeDasharray  = circumference;
  circle.style.strokeDashoffset = circumference;

  setTimeout(() => {
    circle.style.transition = 'stroke-dashoffset 1.5s cubic-bezier(0.16,1,0.3,1)';
    circle.style.strokeDashoffset = offset;
  }, 500);

  // Animate number
  let start = null;
  function step(ts) {
    if (!start) start = ts;
    const p = Math.min((ts - start) / 1500, 1);
    const eased = 1 - Math.pow(1 - p, 3);
    numEl.textContent = Math.floor(eased * score);
    if (p < 1) requestAnimationFrame(step);
  }
  setTimeout(() => requestAnimationFrame(step), 500);
})();
