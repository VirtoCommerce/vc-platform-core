const glob = require("glob");
const path = require("path");

module.exports = [
    {
        entry: glob.sync('./Scripts/**/*.js', { nosort: true }),
        output: {
            path: path.resolve(__dirname, 'dist'),
            filename: 'app.js'
        }
    }
];
