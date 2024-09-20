const path = require('path');

module.exports = {
    entry: './src/index.js',  // Entry point where Webpack starts bundling
    output: {
        path: path.resolve(__dirname, 'wwwroot/js'),  // Output directory for the bundled file
        filename: 'bundle.js' 
    },
    module: {
        rules: [
            {
                test: /\.(js|jsx)$/,  // Transpile all .js and .jsx files
                exclude: /node_modules/,  // Do not transpile node_modules
                use: {
                    loader: 'babel-loader'  // Use babel-loader for transpiling
                }
            },
            {
                test: /\.css$/,  // Handle CSS files (if any)
                use: ['style-loader', 'css-loader'],  // Use style-loader and css-loader for CSS
            }
        ]
    },
    resolve: {
        extensions: ['.js', '.jsx']  // Automatically resolve JS and JSX files
    },
    devtool: 'source-map',  // Enable source maps for easier debugging
};
