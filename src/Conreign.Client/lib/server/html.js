'use strict';
const fs = require('fs');
const path = require('path');
const serializeJs = require('serialize-javascript');

const index = fs.readFileSync(path.join(__dirname, './../client/dist/index.html'), 'utf8');

function render(store, content) {
    const state = store.getState();
    const part =
        `<div id="root">${content}</div>
         <script type="text/javascript">
            window.__INITIAL_STATE__ = ${serializeJs(state)};
         </script>`;
    const html = index.replace('<div id="root"></div>', part);
    return html;
}

module.exports = render;