const fs = require("fs");
const ejs = require("ejs");
const juice = require("juice");

function hexToRgb(hex) {
    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? {
        r: parseInt(result[1], 16),
        g: parseInt(result[2], 16),
        b: parseInt(result[3], 16)
    } : null;
}

function getLightValue(hexColor) {
    let primaryRgb = hexToRgb(hexColor);
    let L = ((primaryRgb.r * 0.2126) + (primaryRgb.g * 0.7152) + (primaryRgb.b * 0.0722)) / 255;
    return L;
}

function getHtml(callback, opts) {
    opts.accentTextColor = "#FAFAFA";

    if (opts.accentColor) {
        let L = getLightValue(opts.accentColor);
        if (L > 0.55) {
            opts.accentTextColor = "#333333";
        }
    }

    let text = fs.readFileSync("./Templating/default.ejs", "utf8");
    let template = ejs.compile(text, {});
    let result = template(opts);
    templateHtml = juice(result);

    callback(null, templateHtml);
}

module.exports = getHtml;