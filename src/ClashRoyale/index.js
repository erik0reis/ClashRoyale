const express = require("express")
var app = express()
app.use('/', express.static(__dirname + "/app/GameAssets/update"))
app.listen(9340, () => {console.log("listening on port 9340")});
