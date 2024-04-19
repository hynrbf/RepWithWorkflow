const path = require("path");
const gulp = require("gulp");
const svgmin = require("gulp-svgmin");
const cheerio = require("gulp-cheerio");
const replace = require("gulp-replace");
const svgSprite = require("gulp-svg-sprite");

/* ICONS SPRITE */
const ICONS_PATH = "./public/icons/*.svg";
const ICONS_SPRITE_DIR = "./public/sprite";
const ICONS_CONFIG = {
  mode: {
    symbol: {
      dest: ".",
      sprite: "sprite-icons.svg",
    },
  },
};
gulp.task("icons", function () {
  return (
    gulp
      .src(ICONS_PATH)
      // minify svg
      .pipe(
        svgmin({
          js2svg: {
            pretty: true,
          },
        })
      )
      // remove all fill, style and stroke declarations in out shapes
      .pipe(
        cheerio({
          run: function ($) {
            $("[fill]").attr("fill", function (index, value) {
              return value === "none" ? "none" : "currentColor";
            });
            $("[stroke]").attr("stroke", "currentColor");
            $("[style]").removeAttr("style");
          },
          parserOptions: { xmlMode: true },
        })
      )
      // cheerio plugin create unnecessary string '>', so replace it.
      .pipe(replace(">", ">"))
      .pipe(svgSprite(ICONS_CONFIG))
      .pipe(gulp.dest(ICONS_SPRITE_DIR))
  );
});


/* FLAGS SPRITE */
const FLAGS_PATH = "./public/flags/*.svg";
const FLAGS_SPRITE_DIR = "./public/sprite";
const FLAGS_CONFIG = {
  mode: {
    symbol: {
      dest: ".",
      sprite: "sprite-flags.svg",
    },
  },
  shape: {
    id: {
      generator(name) {
        return name.split(/\s+-\s+/)[0].trim();
      }
    }
  }
};
gulp.task("flags", function () {
  return (
    gulp
      .src(FLAGS_PATH)
      // minify svg
      .pipe(
        svgmin({
          js2svg: {
            pretty: true,
          },
        })
      )
      .pipe(svgSprite(FLAGS_CONFIG))
      .pipe(gulp.dest(FLAGS_SPRITE_DIR))
  );
});

gulp.task("default", gulp.series(["icons", "flags"]));
