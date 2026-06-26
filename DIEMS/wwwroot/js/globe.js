// ============================================================
// DIEMS — Three.js Globe for Login Page
// ============================================================

(function () {
  const canvas = document.getElementById('globe-canvas');
  if (!canvas || typeof THREE === 'undefined') return;

  const renderer = new THREE.WebGLRenderer({ canvas, antialias: true, alpha: true });
  renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
  renderer.setSize(window.innerWidth, window.innerHeight);
  renderer.setClearColor(0x000000, 0);

  const scene  = new THREE.Scene();
  const camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 1000);
  camera.position.set(0, 0, 3.2);

  // ── Globe ──────────────────────────────────────────────────
  const globeGeo = new THREE.SphereGeometry(1, 64, 64);
  const globeMat = new THREE.MeshPhongMaterial({
    color:       0x0a1628,
    emissive:    0x050c18,
    wireframe:   false,
    transparent: true,
    opacity:     0.95,
  });
  const globe = new THREE.Mesh(globeGeo, globeMat);
  scene.add(globe);

  // ── Wireframe Overlay ──────────────────────────────────────
  const wireGeo = new THREE.SphereGeometry(1.002, 32, 32);
  const wireMat = new THREE.MeshBasicMaterial({
    color:     0x1a3060,
    wireframe: true,
    transparent: true,
    opacity:   0.18,
  });
  scene.add(new THREE.Mesh(wireGeo, wireMat));

  // ── Atmosphere ─────────────────────────────────────────────
  const atmGeo = new THREE.SphereGeometry(1.08, 64, 64);
  const atmMat = new THREE.MeshPhongMaterial({
    color:       0xFF4E00,
    emissive:    0xFF2200,
    transparent: true,
    opacity:     0.06,
    side:        THREE.FrontSide,
  });
  scene.add(new THREE.Mesh(atmGeo, atmMat));

  // ── Outer Glow ─────────────────────────────────────────────
  const glowGeo = new THREE.SphereGeometry(1.15, 32, 32);
  const glowMat = new THREE.MeshBasicMaterial({
    color:       0xFF4E00,
    transparent: true,
    opacity:     0.04,
    side:        THREE.BackSide,
  });
  scene.add(new THREE.Mesh(glowGeo, glowMat));

  // ── Disaster Hot-Spots ─────────────────────────────────────
  const hotspots = [
    { lat: 23.7,  lon: 90.4,   color: 0xFF3B3B, size: 0.022, label: 'Dhaka' },    // Bangladesh
    { lat: 21.4,  lon: 91.9,   color: 0xFF3B3B, size: 0.018, label: 'Chittagong' },
    { lat: 24.9,  lon: 91.9,   color: 0xFF8C00, size: 0.016, label: 'Sylhet' },
    { lat: 13.1,  lon: 80.3,   color: 0xFF8C00, size: 0.020, label: 'Chennai' },
    { lat: 27.7,  lon: 85.3,   color: 0xFF8C00, size: 0.018, label: 'Kathmandu' },
    { lat: 35.7,  lon: 139.7,  color: 0xFFD600, size: 0.015, label: 'Tokyo' },
    { lat: 14.6,  lon: 121.0,  color: 0xFF4E00, size: 0.018, label: 'Manila' },
    { lat: 28.0,  lon: -82.5,  color: 0xFF3B3B, size: 0.015, label: 'Florida' },
    { lat: 37.8,  lon: -122.4, color: 0xFF8C00, size: 0.014, label: 'California' },
    { lat: -8.4,  lon: 115.2,  color: 0xFFD600, size: 0.013, label: 'Bali' },
  ];

  function latLonToVec3(lat, lon, radius) {
    const phi   = (90 - lat) * (Math.PI / 180);
    const theta = (lon + 180) * (Math.PI / 180);
    return new THREE.Vector3(
      -radius * Math.sin(phi) * Math.cos(theta),
       radius * Math.cos(phi),
       radius * Math.sin(phi) * Math.sin(theta)
    );
  }

  hotspots.forEach(hs => {
    const pos = latLonToVec3(hs.lat, hs.lon, 1.01);

    // Dot
    const dotGeo = new THREE.SphereGeometry(hs.size, 8, 8);
    const dotMat = new THREE.MeshBasicMaterial({ color: hs.color });
    const dot    = new THREE.Mesh(dotGeo, dotMat);
    dot.position.copy(pos);
    scene.add(dot);

    // Ring
    const ringGeo = new THREE.RingGeometry(hs.size * 1.8, hs.size * 2.2, 16);
    const ringMat = new THREE.MeshBasicMaterial({
      color: hs.color, transparent: true, opacity: 0.5, side: THREE.DoubleSide
    });
    const ring = new THREE.Mesh(ringGeo, ringMat);
    ring.position.copy(pos);
    ring.lookAt(new THREE.Vector3(0, 0, 0));
    scene.add(ring);

    // Animate ring
    ring.userData = { baseScale: 1, phase: Math.random() * Math.PI * 2 };
  });

  // ── Flight Arcs (connections) ──────────────────────────────
  function createArc(lat1, lon1, lat2, lon2, color) {
    const start = latLonToVec3(lat1, lon1, 1.01);
    const end   = latLonToVec3(lat2, lon2, 1.01);
    const mid   = new THREE.Vector3().addVectors(start, end).multiplyScalar(0.5);
    mid.normalize().multiplyScalar(1.3);

    const curve  = new THREE.QuadraticBezierCurve3(start, mid, end);
    const points = curve.getPoints(60);
    const geo    = new THREE.BufferGeometry().setFromPoints(points);
    const mat    = new THREE.LineBasicMaterial({ color, transparent: true, opacity: 0.35 });
    return new THREE.Line(geo, mat);
  }

  const arcs = [
    createArc(23.7, 90.4, 13.1, 80.3,  0xFF4E00),
    createArc(23.7, 90.4, 27.7, 85.3,  0xFF8C00),
    createArc(14.6, 121.0, 35.7, 139.7, 0xFFD600),
    createArc(28.0, -82.5, 37.8, -122.4, 0xFF4E00),
    createArc(21.4, 91.9, 14.6, 121.0,  0xFF8C00),
  ];

  arcs.forEach(a => scene.add(a));

  // ── Star Field ─────────────────────────────────────────────
  const starGeo = new THREE.BufferGeometry();
  const starPositions = [];
  for (let i = 0; i < 2000; i++) {
    const r = 8 + Math.random() * 12;
    const phi   = Math.random() * Math.PI * 2;
    const theta = Math.acos(2 * Math.random() - 1);
    starPositions.push(
      r * Math.sin(theta) * Math.cos(phi),
      r * Math.sin(theta) * Math.sin(phi),
      r * Math.cos(theta)
    );
  }
  starGeo.setAttribute('position', new THREE.Float32BufferAttribute(starPositions, 3));
  const starMat = new THREE.PointsMaterial({ color: 0x8899CC, size: 0.025, transparent: true, opacity: 0.7 });
  scene.add(new THREE.Points(starGeo, starMat));

  // ── Lighting ───────────────────────────────────────────────
  scene.add(new THREE.AmbientLight(0x334466, 0.8));
  const dirLight = new THREE.DirectionalLight(0x7799FF, 1.2);
  dirLight.position.set(5, 5, 5);
  scene.add(dirLight);
  const rimLight = new THREE.PointLight(0xFF4E00, 0.8, 10);
  rimLight.position.set(-3, 2, -3);
  scene.add(rimLight);

  // ── Mouse Interaction ──────────────────────────────────────
  let mouseX = 0, mouseY = 0;
  document.addEventListener('mousemove', e => {
    mouseX = (e.clientX / window.innerWidth  - 0.5) * 0.5;
    mouseY = (e.clientY / window.innerHeight - 0.5) * 0.3;
  });

  // ── Animate ────────────────────────────────────────────────
  let time = 0;
  function animate() {
    requestAnimationFrame(animate);
    time += 0.008;

    globe.rotation.y += 0.002;
    globe.rotation.x += (mouseY * 0.5 - globe.rotation.x) * 0.05;
    globe.rotation.y += (mouseX * 0.5 - globe.rotation.y) * 0.01;

    // Pulse rings
    scene.children.forEach(child => {
      if (child.userData && child.userData.baseScale !== undefined) {
        const pulse = 1 + 0.4 * Math.sin(time * 3 + child.userData.phase);
        child.scale.setScalar(pulse);
        child.material.opacity = 0.3 + 0.3 * Math.sin(time * 3 + child.userData.phase);
      }
    });

    // Sync all children to globe rotation
    arcs.forEach(a => {
      a.rotation.copy(globe.rotation);
    });

    renderer.render(scene, camera);
  }

  animate();

  // ── Resize ────────────────────────────────────────────────
  window.addEventListener('resize', () => {
    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(window.innerWidth, window.innerHeight);
  });

  // ── Globe position ─────────────────────────────────────────
  globe.position.set(1.2, -0.2, 0);
  scene.children.forEach(c => {
    if (c !== globe && c.type !== 'DirectionalLight' && c.type !== 'AmbientLight' && c.type !== 'PointLight') {
      c.position.x = globe.position.x;
      c.position.y = globe.position.y;
    }
  });
})();
