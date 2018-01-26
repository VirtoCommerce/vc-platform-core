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
    sourcemaps = require('gulp-sourcemaps'),
    print = require('gulp-print'),
    gulpsync = require('gulp-sync')(gulp);

// ToDo gulp-print for degug, refactoring this file

gulp.task("min", gulpsync.sync([ "min:bowerPackages:js", "min:bowerPackages:css", "translateSass", "min:js", "min:css", "min:allPackages:css"]));

// concatenate all css files
gulp.task('min:allPackages:css', function () {
    return gulp.src(['wwwroot/css/*.css','!wwwroot/css/allPackages.css', '!wwwroot/css/platform.css'])
        .pipe(print())
        .pipe(concat('allPackages.css'))
        .pipe(gulp.dest('wwwroot/css/'));
});


// concatenate all css files from bower packages to single file
gulp.task('min:bowerPackages:css', function () {
    return gulp.src(mainBowerFiles({
        // Only the CSS files
        filter: /.*\.css$/i
    }))
        .pipe(print())
        .pipe(concat('bowerPackages.css'))
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
        .pipe(print())
        .pipe(concat('allPackages.js'))
        .pipe(gulp.dest('wwwroot/js/'))
        .pipe(uglify())
        .pipe(rename({ extname: '.min.js' }))
        .pipe(gulp.dest('wwwroot/js/'));
});

gulp.task("min:js", function () {
    var src_paths = ['wwwroot/js/**/*.js', '!wwwroot/js/**/*.min.js', '!wwwroot/js/allPackages.js', '!wwwroot/js/platform.js'];
    var plainStream = gulp.src(src_paths)
        .pipe(print())
        .pipe(sourcemaps.init())
        .pipe(concat('platform.js'))
        // Add transformation tasks to the pipeline here.
        .pipe(sourcemaps.write('../maps'))
        .pipe(gulp.dest('wwwroot/js/'));

    var minStream = gulp.src(src_paths)
        .pipe(sourcemaps.init())
        .pipe(concat('platform.min.js'))
        .pipe(uglify().on('error', function (e) {
            console.log(e);
        }))
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
        .pipe(print())
        .pipe(concat('css-files.css'));

    return merge(scssStream, cssStream)
        .pipe(concat('platform.css'))
        .pipe(cssmin())
        .pipe(gulp.dest('wwwroot/css')); 
});

// translate sass to css
gulp.task('translateSass', function () {
    return gulp.src(['wwwroot/css/themes/main/sass/**/*.sass'])
        .pipe(print())
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
        .pipe(sourcemaps.write('.', { includeContent: false, sourceRoot: '../sass' }))
        .pipe(gulp.dest('wwwroot/css'));
});

// Watch on sass to enable auto-translation
gulp.task('watch',function() {
        gulp.watch('wwwroot/css/themes/main/sass/**/*.sass', ['translateSass']);
    });

gulp.task("clean", function () {
    var files = ['wwwroot/css/allPackages.css', 'wwwroot/css/platform.css', 'wwwroot/css/bowerPackages.css', 'wwwroot/css/main.css', 'wwwroot/js/allPackages.(min.js|js)', 'wwwroot/js/platform.(min.js|js'];
    return del(files);
});

