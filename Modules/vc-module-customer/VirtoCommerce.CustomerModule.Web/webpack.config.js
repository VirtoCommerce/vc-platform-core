const glob = require("glob");
const path = require("path");

module.exports = [
    {
        entry: glob.sync('./Scripts/**/*.js', { ignore: ['./Scripts/dist/**/*.*'], nosort: true }),
        output: {
            path: path.resolve(__dirname, 'Scripts/dist'),
            filename: 'app.js'
        }
    }
];
