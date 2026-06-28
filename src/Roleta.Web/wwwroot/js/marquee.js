// Posiciona as lâmpadas do marquee com ESPAÇAMENTO CONSTANTE, recalculando no resize.
// Há sempre uma lâmpada em cada um dos 4 cantos (posição fixa), e todas — inclusive os
// cantos — seguem o mesmo piscar alternado. As bordas opostas usam a mesma contagem, então
// o total é par e a alternância "fecha" certinho ao redor do anel.

export function init(container, options) {
    const opts = options || {};
    const gap = opts.gap || 42;       // distância-alvo entre centros das lâmpadas (px)
    const bulb = opts.bulb || 16;     // diâmetro da lâmpada (px)
    const margin = opts.margin || 16; // recuo da borda da tela (px)
    const cycle = opts.cycle || 1.2;  // duração do ciclo do piscar (s)
    const half = (cycle / 2).toFixed(3) + 's';

    function build() {
        const w = container.clientWidth;
        const h = container.clientHeight;
        if (!w || !h) return;

        // Retângulo onde ficam os CENTROS das lâmpadas.
        const left = margin + bulb / 2;
        const top = margin + bulb / 2;
        const right = w - margin - bulb / 2;
        const bottom = h - margin - bulb / 2;
        const innerW = right - left;
        const innerH = bottom - top;
        if (innerW <= 0 || innerH <= 0) return;

        // Contagens simétricas (topo=base, esquerda=direita) -> total par.
        const nH = Math.max(0, Math.round(innerW / gap) - 1);
        const nV = Math.max(0, Math.round(innerH / gap) - 1);

        const frag = document.createDocumentFragment();
        let k = 0;

        function add(x, y) {
            const b = document.createElement('div');
            b.className = 'bulb';
            b.style.left = (x - bulb / 2) + 'px';
            b.style.top = (y - bulb / 2) + 'px';
            // alterna seguindo a ordem ao redor do anel (cantos incluídos)
            b.style.setProperty('--d', (k % 2) ? half : '0s');
            k++;
            frag.appendChild(b);
        }

        // n lâmpadas ESTRITAMENTE entre dois pontos (sem repetir as pontas/cantos)
        function span(x1, y1, x2, y2, n) {
            for (let i = 1; i <= n; i++) {
                const t = i / (n + 1);
                add(x1 + (x2 - x1) * t, y1 + (y2 - y1) * t);
            }
        }

        add(left, top);                         // canto sup-esq
        span(left, top, right, top, nH);        // topo:    esq -> dir
        add(right, top);                        // canto sup-dir
        span(right, top, right, bottom, nV);    // direita: cima -> baixo
        add(right, bottom);                     // canto inf-dir
        span(right, bottom, left, bottom, nH);  // base:    dir -> esq
        add(left, bottom);                      // canto inf-esq
        span(left, bottom, left, top, nV);      // esquerda: baixo -> cima

        container.replaceChildren(frag);
    }

    build();

    let raf = 0;
    const ro = new ResizeObserver(() => {
        cancelAnimationFrame(raf);
        raf = requestAnimationFrame(build);
    });
    ro.observe(container);

    return {
        dispose() {
            cancelAnimationFrame(raf);
            ro.disconnect();
            container.replaceChildren();
        }
    };
}
