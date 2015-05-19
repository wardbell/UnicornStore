﻿/// <binding Clean='clean' />

var gulp = require("gulp"),
  rimraf = require("rimraf"),
  fs = require("fs");

eval("var project = " + fs.readFileSync("./project.json"));

var paths = {
  bower: "./bower_components/",
  lib: "./" + project.webroot + "/lib/"
};

gulp.task("clean", function (cb) {
  rimraf(paths.lib, cb);
});

gulp.task("copy", ["clean"], function () {
    var bower = {
    "angular": "angular/angular.{js,min.js}",
    "bootstrap": "bootstrap/dist/**/*.{js,map,css,ttf,svg,woff,eot}",
    "bootstrap-touch-carousel": "bootstrap-touch-carousel/dist/**/*.{js,css}",
    "breeze-client": "breeze-client/build/breeze.{debug.js,min.js}", 
    "breeze-angular": "breeze-client/build/adapters/breeze.bridge.angular.js",
    "breeze-client-labs":"breeze-client-labs/*.{js,css}",
    "hammer.js": "hammer.js/hammer*.{js,map}",
    "jquery": "jquery/jquery*.{js,map}",
    "jquery-validation": "jquery-validation/jquery.validate.js",
    "jquery-validation-unobtrusive": "jquery-validation-unobtrusive/jquery.validate.unobtrusive.js",
    "toastr" : "toastr/*.{js,css}"
  }

  for (var destinationDir in bower) {
    gulp.src(paths.bower + bower[destinationDir])
      .pipe(gulp.dest(paths.lib + destinationDir));
  }
});
