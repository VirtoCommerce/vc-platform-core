/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require("gulp"),
    mainBowerFiles = require('main-bower-files'),
    concat = require("gulp-concat"),
    uglify = require("gulp-uglify");

// minify all js files to single file
gulp.task('packJavaScript', function () {
    return gulp.src(mainBowerFiles({
            // Only the JavaScript files
            filter: /.*\.js$/i
        }))
        .pipe(concat('allPackages.js'))
        //.pipe(uglify())
        .pipe(gulp.dest('Scripts'));
});

// concatenate all css files to single file
gulp.task('packCss', function () {
    return gulp.src(mainBowerFiles({
            // Only the CSS files
            filter: /.*\.css$/i
        }))
        .pipe(concat('allStyles.css'))
        .pipe(gulp.dest('Content'));
});

// copy fonts for simple packages like angular-ui-grid.
gulp.task('copyMainFonts', function () {
    return gulp.src(mainBowerFiles({
            // Only return the font files
            filter: /.*\.(eot|svg|ttf|woff)$/i
        }))
        .pipe(gulp.dest('Content'));
});

gulp.task('packAll', ['packJavaScript', 'packCss', 'copyMainFonts']);
