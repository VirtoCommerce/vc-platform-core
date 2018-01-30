/// <binding AfterBuild='min:js, min:css' />
"use strict";

var gulp = require("gulp"),
    filter = require("gulp-filter"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    htmlmin = require("gulp-htmlmin"),
    uglify = require("gulp-uglify"),
    merge = require("merge-stream"),
    del = require("del"),
    autoprefixer = require('gulp-autoprefixer'),
    mainBowerFiles = require('main-bower-files'),
    sass = require('gulp-sass'),
    rename = require('gulp-rename'),
    sourcemaps = require('gulp-sourcemaps');


gulp.task("min", ["min:bowerPackages:js", "min:bowerPackages:css", "min:js", "min:css"]);

// concatenate all css files from bower packages to single file
gulp.task('min:bowerPackages:css', function () {
    return gulp.src(mainBowerFiles({
        // Only the CSS files
        filter: /.*\.css$/i
    }))
        .pipe(concat('allPackages.css'))
        .pipe(gulp.dest('wwwroot/css/'));
});

gulp.task('min:bowerPackages:js', function () {
    return gulp.src(mainBowerFiles({
        // Only the JavaScript files
        filter: /.*\.js$/i,
        // Exclude angular-i18n packages files and include moment.js i18n files
        overrides: {
            "angular-i18n": {
                "main": ""
            },
            "moment": {
                "main": ["moment.js", "locale/*.js"]
            },
            "moment-timezone": {
                "main": ["builds/moment-timezone-with-data.js", "moment-timezone-utils.js"]
            }
        }
    }))
        .pipe(concat('allPackages.js'))
        .pipe(gulp.dest('wwwroot/js/'))
        .pipe(uglify())
        .pipe(rename({ extname: '.min.js' }))
        .pipe(gulp.dest('wwwroot/js/'));
});

gulp.task("min:js", function () {
    var src_paths = ['wwwroot/js/**/*.js', '!wwwroot/js/**/*.min.js', '!wwwroot/js/allPackages.js', '!wwwroot/js/platform.js'];
    var plainStream = gulp.src(src_paths)
        .pipe(sourcemaps.init())
        .pipe(concat('platform.js'))
        // Add transformation tasks to the pipeline here.
        .pipe(sourcemaps.write('../maps'))
        .pipe(gulp.dest('wwwroot/js/'));

    var minStream = gulp.src(src_paths)
        .pipe(sourcemaps.init())
        .pipe(concat('platform.min.js'))
        .pipe(uglify())
        .pipe(sourcemaps.write('../maps'))
        .pipe(gulp.dest('wwwroot/js/'));
    return merge(plainStream, minStream);
});

gulp.task("min:css", function () {

    var scssStream = gulp.src(['wwwroot/css/themes/main/sass/**/*.sass'])
        // must be executed straigh after source
        .pipe(sourcemaps.init())
        .pipe(sass({
            includePaths: require('node-bourbon').includePaths
        }))
        .pipe(autoprefixer({
            browsers: [
                'Explorer >= 10',
                'Edge >= 12',
                'Firefox >= 19',
                'Chrome >= 20',
                'Safari >= 8',
                'Opera >= 15',
                'iOS >= 8',
                'Android >= 4.4',
                'ExplorerMobile >= 10',
                'last 2 versions'
            ]
        }))
        // must be executed straight before output
        .pipe(sourcemaps.write('.', { includeContent: false, sourceRoot: '../sass' }));

    var cssStream = gulp.src(['wwwroot/css/themes/main/css/**/*.css', 'wwwroot/js/codemirror/**/*.css', '!wwwroot/css/themes/main/css/allPackages.css'])
        .pipe(concat('css-files.css'));

    return merge(scssStream, cssStream)
        .pipe(concat('platform.css'))
        .pipe(cssmin())
        .pipe(gulp.dest('wwwroot/css'));
});

gulp.task("clean", function () {
    var files = ['wwwroot/css/allPackages.css', 'wwwroot/css/platform.css', 'wwwroot/js/allPackages.(min.js|js)', 'wwwroot/js/platform.(min.js|js'];
    return del(files);
});

