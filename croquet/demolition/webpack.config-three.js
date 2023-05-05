const HtmlWebPackPlugin = require('html-webpack-plugin');
const path = require('path');

module.exports = _env => ({
    entry : './index-three.js',
    output: {
        path: path.join(__dirname, 'dist'),
        pathinfo: false,
        filename: '[name]-[contenthash:8].js',
        chunkFilename: 'chunk-[name]-[contenthash:8].js',
        clean: true
    },
    resolve: {
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
        new HtmlWebPackPlugin({
            template: 'index-three.html',   // input
            filename: 'index.html',   // output filename in dist/
        }),
    ]
});
