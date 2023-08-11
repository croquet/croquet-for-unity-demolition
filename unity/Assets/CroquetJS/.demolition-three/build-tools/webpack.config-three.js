const CopyPlugin = require('copy-webpack-plugin');
const HtmlWebPackPlugin = require('html-webpack-plugin');
const path = require('path');

module.exports = _env => ({
    entry : './index-three.js',
    output: {
        path: path.join(__dirname, 'dist'),
        pathinfo: false,
        filename: 'index-[contenthash:8].js',
        chunkFilename: 'chunk-[contenthash:8].js',
        clean: true
    },
    resolve: {
        modules: [path.resolve(__dirname, '../node_modules')],
        fallback: { "crypto": false }
    },
    experiments: {
        asyncWebAssembly: true,
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                enforce: "pre",
                use: ["source-map-loader"],
            },
        ],
    },
    plugins: [
        new CopyPlugin({
            patterns: [
                { from: `../../demolition/scene-definitions.txt`, to: 'scene-definitions.txt', noErrorOnMissing: true },
                { from: `../../.js-build/.last-installed-tools`, to: 'last-installed-tools.txt' }
            ]
        }),
        new HtmlWebPackPlugin({
            template: 'index-three.html',   // input
            filename: 'index.html',   // output filename in dist/
        }),
    ]
});
