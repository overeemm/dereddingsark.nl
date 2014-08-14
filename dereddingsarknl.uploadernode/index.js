var readline = require('readline');
var spawn = require('child_process').spawn;
var JSFtp = require("jsftp");
var querystring = require('querystring');
var http = require('http');
var fs = require('fs');
var config = require('./config.js');

var categories = [
  { "name": "samenkomst baarn", "ftp": "public_html/preken/samenkomsten", "url": "preken/samenkomsten" },
  { "name": "samenkomst bunschoten", "ftp": "public_html/preken/samenkomsten", "url": "preken/samenkomsten" },
  { "name": "jeugddienst", "ftp":  "public_html/preken/jeugddiensten", "url": "preken/jeugddiensten" },
  { "name": "maranatha avond", "ftp":  "public_html/preken/maranatha", "url": "preken/maranatha" },
  { "name": "bijbelstudies", "ftp":  "public_html/bijbelstudies", "url": "bijbelstudies" }
];

var rl = readline.createInterface(process.stdin, process.stdout);

categories.forEach(function (e, i) {
  console.log('['+i+'] ' + e.name);  
});

var filePath = process.argv[2];
var tempFilePath = filePath + "small.mp3"
fs.unlink(tempFilePath, function () {
});

rl.question('Welke categorie? ', function(categorie) {
  categorie = parseInt(categorie);

  rl.question('Spreker? ', function(spreker) {

    rl.question('Titel? ', function(titel) {

      var today = getToday();
      rl.question('Welke datum (yyyy-MM-dd HH:mm:ss , of leeg voor ['+today+'])? ', function(datum) {
        datum = datum || today;
        var targetFileName = spreker + (titel ? "-" + titel : "");
        var name = targetFileName;
        targetFileName = datum.substring(0, 4) + datum.substring(5, 7) + datum.substring(8, 10) + "-" + targetFileName
        targetFileName = targetFileName.replace(new RegExp("[\. \x00\/\\:\*\?\\\"<>\| ]", 'gi'), '_').toLowerCase() + ".mp3";

        console.log("Going to upload "+filePath + " to " + categories[categorie].ftp + " as " + targetFileName);

        lame(function () {

          upload(categories[categorie].ftp + "/" + targetFileName, function () {

            addfile(categories[categorie].url + "/" + targetFileName, name, datum, categories[categorie], function () {
              
              fs.unlink(tempFilePath, function () {
                rl.close();
              });
            });

          });

        });
        
      });

    });

  });

});

function addfile(url, name, datum, category, callback){

  var post_data = querystring.stringify({
    "url": "http://www.dereddingsarkmedia.nl/"+url,
    "name": name,
    "datetime": datum, 
    "categorie": category.name
  });

  // An object of options to indicate where to post to
  var post_options = {
      host: 'www.dereddingsark.nl',
      port: '443',
      path: '/audio/add',
      method: 'POST',
      rejectUnauthorized: false,
      requestCert: true,
      agent: false,
      headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
          'Content-Length': Buffer.byteLength(post_data),
          "X-UserGuid": config["X-UserGuid"], 
          "X-Token": config["X-Token"]
      }
  };

  // Set up the request
  var post_req = http.request(post_options, function(res) {
    res.setEncoding('utf8');
    res.on('data', function (chunk) {
      console.log('Response: ' + chunk);
    });
    res.on('end', function () {
      console.log("Result: " + res.statusCode);
      callback();
    });
  });

  // post the data
  post_req.write(post_data);
  post_req.end();
}

function lame(callback) {
  var l = spawn('lame', ['-V9', '-b', '32', '-h', filePath, tempFilePath]); 

  l.stdout.on('data', function (data) {
    console.log('stdout: ' + data);
  });

  l.stderr.on('data', function (data) {
    console.log('stderr: ' + data);
  });

  l.on('close', function (code) {
    console.log("Finished lame with code " + code);
    callback();
  }); 
}

function upload(path, callback){

  var ftp = new JSFtp(config.ftp);

  ftp.put(tempFilePath, path, function(hadError) {
    if(!hadError){
      console.log("Finished upload to ftp ");
      callback();
    } else {
      console.log("Error on upload");
      console.log(hadError);
      rl.close();
    }
  });
}

function getToday() {
  var today = new Date();

  var month = (today.getMonth()+1);
  month = month < 10 ? "0"+month : month;
  
  var date = today.getDate();
  date = date < 10 ? "0"+date : date;
  var todayStr = today.getFullYear()+"-"+month+"-"+date+" 09:45:00";
  return todayStr;
}