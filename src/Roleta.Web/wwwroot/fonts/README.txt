Fontes do show
==============

Coloque os arquivos das fontes nesta pasta. O CSS (wwwroot/app.css) já as referencia
por estes nomes — basta copiar os arquivos com os nomes abaixo (ou ajustar o src no app.css).

1) Fonte do sistema — "Trade Gothic Next LT Pro Heavy Compressed"
   Nomes esperados (qualquer um dos formatos serve; .woff2 é o preferido):
     - trade-gothic-next-heavy-compressed.woff2
     - trade-gothic-next-heavy-compressed.woff
     - trade-gothic-next-heavy-compressed.otf

2) Fonte dos pontos/placar — display de SETE SEGMENTOS, em vermelho.
   - Já vem embutida a fonte open-source "DSEG7 Classic" (dseg7-classic.woff2),
     usada por padrão para os pontos. Fonte: https://github.com/keshikan/DSEG (SIL OFL).
   - Se quiser usar a "Cursed Timer" no lugar, coloque os arquivos abaixo — ela tem
     prioridade sobre a DSEG7 no app.css:
     - cursed-timer.woff2
     - cursed-timer.woff
     - cursed-timer.ttf

Observações
-----------
- São fontes proprietárias/de terceiros: garanta a licença antes de distribuir.
- Enquanto os arquivos não estiverem aqui, o app usa os fallbacks definidos no app.css
  (Arial Narrow / Courier New), então a aplicação continua funcionando.
- Para converter .otf/.ttf em .woff2, use o utilitário do seu navegador/ferramenta de fontes.
