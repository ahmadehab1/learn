
//Disable upload button while file dialog is open
$("#upload-btn").on("click", function () {
})
//Read the image file on selecting
$("#upload-btn").on('change', function (event) {
    var files = event.target.files;
    if (files.length > 0) {
        readFile(event);
    }
});

//File reading and binding function
function readFile(event) {
    console.log(event);
    var input = event.target;
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var fileName = input.files[0].name;
           upload(e.target.result, fileName);
        }
        reader.readAsDataURL(input.files[0]);
    }
}
//Uploading function
function upload(base64, fileName) {
    console.log(fileName);
    var uniqueFileName = "";
    var uniques = [];
    var isAttachment = false;
    var fileNames = [];
    var finishedFile = [];
    var sizeLimit = 105857600000; // 10MB in Bytes
    var uploadProgress = [];
    var flag = 0;
    var creds = {
        bucket: 'tapdeal-dev',
        access_key: 'AKIAIIOV5IY4W2WXHLXA',
        secret_key: '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'
    };
    AWS.config.update({ accessKeyId: creds.access_key, secretAccessKey: creds.secret_key });
    AWS.config.region = 'us-west-2';
    var bucket = new AWS.S3({ params: { Bucket: creds.bucket } });
    var binary = window.atob(base64.split(',')[1]);
    var array = [];
    for (var i = 0; i < binary.length; i++) {
        array.push(binary.charCodeAt(i));
    }
    var file = null;
    if (base64.indexOf('png') > 0) {
        file = new Blob([new Uint8Array(array)], { type: 'image/png' });
    }
    else if (base64.indexOf('jpeg') > 0) {
        file = new Blob([new Uint8Array(array)], { type: 'image/jpeg' });
    }
    var files = [file];
    if (files) {
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            // venorm File Size Check First
            var fileSize = Math.round(parseInt(file.size));
            if (fileSize > sizeLimit) {
                var msg = 'Sorry, your attachment is too big. Maximum ' + fileSizeLabel() + ' file attachment allowed' + '\nFile Too Large';
                humane.log(msg);
                return false;
            }
            // Prepend Unique String To Prevent Overwrites
            uniqueFileName = uniqueString() + '-' + fileName;
            var name = file.name;
            uniques.push(uniqueFileName);
            finishedFile[uniqueFileName] = -2;
            var params = { Key: uniqueFileName, ContentType: file.type, Body: file, ServerSideEncryption: 'AES256', ACL: 'public-read' };
            var options = { partSize: 5 * 1024 * 1024, queueSize: 1 };

            var progressBarElemWrapper = $(".progress-striped");
            var progressBarElem = $("#progress-bar");
            var progressBarElemText = $("#progress-bar span");
            progressBarElemText.hide();
            progressBarElemWrapper.fadeIn(200);

            bucket.upload(params, options, function (err, data) {
                if (err) {
                    //console.log(err.message, err.code);
                    return false;
                }
                else {
                    // Upload Successfully Finished
                    humane.log('File Uploaded Successfully');
                    // Reset The Progress Bar
                }
            }).on('httpUploadProgress', function (progress) {
                console.log(progress);
                //console.log(progressBarElem);
                var percent = Math.ceil((progress.loaded / progress.total) * 100);
                progressBarElem.css("width", percent + "%");
                progressBarElem.progressbar({
                    value: percent,
                    create: function (event, ui) {
                        progressBarElemText.text("Uploading...");
                        progressBarElemText.show();
                    },
                    complete: function (event, ui) {
                        //progressBarElemText.text("Done. Please wait...");
                    }
                });
            }).send(function (err, data) {
                progressBarElemWrapper.fadeOut(200);
                saveMessage(data.Location, "file");
                return data.Location;

            });
        }
    } else {
        // No File Selected
        humane.log('Please select a file to upload');
    }
    function fileSizeLabel() {
        return Math.round(sizeLimit / 1024 / 1024) + 'MB';
    }
    function uniqueString() {
        var text = "";
        var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        for (var i = 0; i < 8; i++) {
            text += possible.charAt(Math.floor(Math.random() * possible.length));
        }
        return text;
    }
};