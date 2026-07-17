// Encaixa o texto no card do telão: reduz a fonte até caber (largura e altura).
// Observa resize do contêiner e troca de fonte (font-display: swap).

export function attach(el, options) {
    const opts = options || {};
    const minPx = opts.minPx ?? 20;
    const container = opts.container
        ? opts.container
        : (el.closest('.public-frame') || el.parentElement);

    if (!el || !container) {
        return { dispose() {} };
    }

    let maxPx = opts.maxPx ?? 0;
    let raf = 0;
    let disposed = false;

    function available() {
        const cs = getComputedStyle(container);
        const padX = parseFloat(cs.paddingLeft) + parseFloat(cs.paddingRight);
        const padY = parseFloat(cs.paddingTop) + parseFloat(cs.paddingBottom);
        return {
            w: Math.max(0, container.clientWidth - padX),
            h: Math.max(0, container.clientHeight - padY),
        };
    }

    function fits(avail) {
        // +1 evita falsos positivos por arredondamento de subpixel
        return el.scrollWidth <= avail.w + 1 && el.scrollHeight <= avail.h + 1;
    }

    function resolveMax() {
        if (opts.maxPx) return opts.maxPx;
        // Lê o tamanho "desejado" do CSS (clamp) sem o override inline.
        const prev = el.style.fontSize;
        el.style.fontSize = '';
        const fromCss = parseFloat(getComputedStyle(el).fontSize);
        el.style.fontSize = prev;
        return Number.isFinite(fromCss) && fromCss > 0 ? fromCss : 144;
    }

    function fit() {
        if (disposed) return;

        const avail = available();
        if (avail.w < 8 || avail.h < 8) return;

        if (!maxPx) maxPx = resolveMax();

        el.style.width = avail.w + 'px';

        let lo = minPx;
        let hi = Math.max(minPx, maxPx);

        el.style.fontSize = hi + 'px';
        if (fits(avail)) return;

        // busca binária: maior fonte que ainda cabe
        while (hi - lo > 0.5) {
            const mid = (lo + hi) / 2;
            el.style.fontSize = mid + 'px';
            if (fits(avail)) lo = mid;
            else hi = mid;
        }
        el.style.fontSize = lo + 'px';
    }

    function schedule() {
        cancelAnimationFrame(raf);
        raf = requestAnimationFrame(fit);
    }

    // Primeiro layout (e de novo quando a fonte do show carregar).
    schedule();
    if (document.fonts && document.fonts.ready) {
        document.fonts.ready.then(schedule).catch(() => {});
    }

    const ro = new ResizeObserver(schedule);
    ro.observe(container);

    return {
        refit: schedule,
        dispose() {
            disposed = true;
            cancelAnimationFrame(raf);
            ro.disconnect();
            el.style.fontSize = '';
            el.style.width = '';
        },
    };
}
