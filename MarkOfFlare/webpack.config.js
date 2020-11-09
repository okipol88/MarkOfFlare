const path = require("path");
const fs = require("fs");

module.exports = {
    entry: () => fs.readdirSync("./Typescript/").filter(f => f.endsWith(".js")).map(f => `./Typescript/${f}`),
    devtool: "source-map",
    mode: "development",
    output: {
        filename: "app.js",
        path: path.resolve(__dirname, "./wwwroot/scripts")
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                enforce: "pre",
                use: ["source-map-loader"]
            }
        ]
    }
}