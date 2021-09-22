//naogjanje na MD5Hash od fajl
function calculateMD5Hash(file, bufferSize) {
    var def = Q.defer();

    var fileReader = new FileReader();
    var fileSlicer = File.prototype.slice || File.prototype.mozSlice || File.prototype.webkitSlice;
    var hashAlgorithm = new SparkMD5();
    var totalParts = Math.ceil(file.size / bufferSize);
    var currentPart = 0;
    var startTime = new Date().getTime();

    fileReader.onload = function (e) {
        currentPart += 1;

        def.notify({
            currentPart: currentPart,
            totalParts: totalParts
        });

        var buffer = e.target.result;
        hashAlgorithm.appendBinary(buffer);

        if (currentPart < totalParts) {
            processNextPart();
            return;
        }

        def.resolve({
            hashResult: hashAlgorithm.end()
        });
    };

    fileReader.onerror = function (e) {
        def.reject(e);
    };

    function processNextPart() {
        var start = currentPart * bufferSize;
        var end = Math.min(start + bufferSize, file.size);
        fileReader.readAsBinaryString(fileSlicer.call(file, start, end));
    }

    processNextPart();
    return def.promise;
}


//save file information into database (Send data to FileHandler.ashx)
function getFileInfo() {

    //se zema prikaceniot fajl
    var input = document.getElementById('file');
    if (!input.files.length) {
        return;
    }

    var file = input.files[0];
    var bufferSize = Math.pow(1024, 2) * 10; // 10MB
    var fileLastModifiedDate = file.lastModifiedDate;
    var filename = file.name;

    calculateMD5Hash(file, bufferSize).then(
      function (result) {

          // Success pecati na konzola (mozi da sedi i nemora)
          console.log("MD5HashFile ", result.hashResult);
          console.log("FileName ", filename)
          console.log("FileLastModifiedDate ", fileLastModifiedDate)

          var postdata = JSON.stringify(
          {
              "Name": filename,
              "MD5HashFile": result.hashResult,
              "ModifiedDate": fileLastModifiedDate
          });
          try {
              $.ajax({
                  type: "POST",
                  url: "FileHandler.ashx",
                  cache: false,
                  data: postdata,
                  success: getSuccess,
                  error: getFail
              });
          } catch (e) {
              alert(e);
          }

          //ova e funkcija sto se povikuva dokolu hendlerot uspesno zapisa vo baza
          function getSuccess(data, textStatus, jqXHR) {
              var sm = "-----SIGNED MESSAGE----- \n Hash: MD5 \n"
              var cerInf = "============================================================================ \n Certificate Information \n============================================================================ \n \n"
              var fileText = sm + cerInf + data
              var blob = new Blob([fileText], { type: "text/plain;charset=utf-8" });
              var id = data.substr(16, (data.indexOf(".") - 16))
              saveAs(blob, "TimeStamp-certificate_" + id + ".txt");
              document.getElementById("file").value = null;
              alert("You have successfully created a TimeStamp on your file!");

          };
          function getFail(jqXHR, textStatus, errorThrown) {
              document.getElementById("file").value = null;
              alert(errorThrown);
          };


      },
      function (err) {
          // There was an error,
      });
}


//check if file exists in database (Send data to CheckFileHandler.ashx)
function checkFileInfo() {

    var input = document.getElementById('CheckFile');
    if (!input.files.length) {
        return;
    }

    var file = input.files[0];
    var bufferSize = Math.pow(1024, 2) * 10; // 10MB
    var fileLastModifiedDate = file.lastModifiedDate;
    var filename = file.name;

    calculateMD5Hash(file, bufferSize).then(
      function (result) {

          // Success
          console.log("MD5HashFile ", result.hashResult);
          console.log("FileName ", filename)
          console.log("FileLastModifiedDate ", fileLastModifiedDate)

          var postdata = JSON.stringify(
          {
              "Name": filename,
              "MD5HashFile": result.hashResult,
              "ModifiedDate": fileLastModifiedDate
          });
          try {
              $.ajax({
                  type: "POST",
                  url: "CheckFileHandler.ashx",
                  cache: false,
                  data: postdata,
                  success: getSuccess,
                  error: getFail
              });
          } catch (e) {
              alert(e);
          }
          function getSuccess(data, textStatus, jqXHR) {
              document.getElementById("CheckFile").value = null;
              alert(data);

          };
          function getFail(jqXHR, textStatus, errorThrown) {
              document.getElementById("CheckFile").value = null;
              alert(errorThrown);
          };


      },
      function (err) {
          // There was an error,
      });
}
