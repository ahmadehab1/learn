var url = window.location.href.split('/');
var action, controller;
if (url[url.length - 1] === "Create") {
    action = url[url.length - 1];
    controller = url[url.length - 2]
}
else {
    action = url[url.length - 2];
    controller = url[url.length - 3]
}
var editing = false;
//HS:Check if in editing mode
if (action === "Edit") editing = true;
//HS:Croppers objects (their IDs, widths and heights)
var productsCroppersArr = [{ ord: 1, width: 810, height: 387 }, { ord: 2, width: 100, height: 100 }, { ord: 3, width: 400, height: 400 }, { ord: 4, width: 250, height: 130 }];

initializeCropper('ev', productsCroppersArr);
function initializeCropper(page, arr) {
    for (var i = 0; i < arr.length; i++) {
        innerInitializer(page, arr, i);
    }
}
//HS:Loop function
function innerInitializer(page, arr, index) {
    var $uploadCrop, imageName;
    var num = arr[index]["ord"];
    var width = arr[index]["width"];
    var height = arr[index]["height"];
    $(".crop-btn" + num).addClass('disabled');
    //HS:Initialize croppie
    $uploadCrop = $('#' + page + '-img' + num).croppie({
        viewport: {
            width: width,
            height: height,
            type: 'square'
        },
        enableExif: true
    });
    //HS:Bind croppie with uploaded image
    $uploadCrop.croppie('bind', {
        url: $("#" + page + "-img" + num + "-name").val()
    });
    if ($("#" + page + "-img" + num + "-name").val() !== "") {
        $uploadCrop.croppie('bind', {
            url: $("#" + page + "-img" + num + "-name").val(),
            zoom: 0
        }).then(function () {
            //$("#" + page + "-img" + num + "-img-overlay").hide();
        });
    }
    else {
        $("#" + page + "-img" + num + "-img-overlay-text").show();
        $(".cr-slider-wrap").hide();
    }
    if (editing) {
        if ($("#" + page + "-img" + num + "-name").val() !== "") {
            $("#" + page + "-img" + num + "-img-overlay").show();
            $uploadCrop.croppie('bind', {
                url: $("#" + page + "-img" + num + "-name").val(),
                zoom: 0
            }).then(function () {
                $("#" + page + "-img" + num + "-img-overlay").hide();
            });
        }
        else {
            $("#" + page + "-img" + num + "-img-overlay-text").show();
        }
    }
    //HS:Disable upload button while file dialog is open
    $(".upload-btn" + num).on("click", function () {
        $(this).attr("disabled", "disabled");
        $(document).on("focus", "input", function () {
            $(".upload-btn" + num).removeAttr("disabled");
        })
    })
    //HS:Read the image file on selecting
    $("#" + page + "-img" + num + "-upload").on('change', function (event) {
        var files = event.target.files;
        if (files.length > 0) {
            $("#" + page + "-img" + num + "-img-overlay-text").hide();
            $(".cr-slider-wrap").show();
            if (files[0].size > 4000000) {
                swal("Image size must not exceed 4 MB.");
                return;
            }
            imageName = files[0].name;
            readFile(event, page, num, $uploadCrop);
        } 
    });
    //HS:Upload the image file, retrieve it's URL, and bind it with croppie
    $("#" + page + "-img" + num + "-crop").on('click', function (e) {
        $(".upload-btn" + num + ", .crop-btn" + num).addClass('disabled');
        $uploadCrop.croppie('result', {
            type: 'canvas',
            size: 'viewport'
        }).then(function (resp) {
            upload(resp, page, num, imageName);
        }, function () {
            $(".upload-btn" + num + ", .crop-btn" + num).removeClass('disabled');
            swal("Please upload a different image.");
        });
    });
}
//HS:Image file reading and binding function
function readFile(event, page, num, cropperElem) {
    $("#" + page + "-img" + num + "-img-overlay").show();
    var input = event.target;
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            //console.log(e);
            $('#' + page + '-img' + num).addClass('ready');
            if (cropperElem) {
                cropperElem.croppie('bind', {
                    url: e.target.result
                }).then(function () {
                    $("#" + page + "-img" + num + "-img-overlay").hide();
                    $(".crop-btn" + num).removeClass('disabled');
                    //console.log('jQuery bound');
                });
            }
            else {
                $("#" + page + "-img" + num + "-img-overlay").hide();
                $("#perfc-img6").attr("src", e.target.result);
            }
        }
        reader.readAsDataURL(input.files[0]);
    }
}
//Uploading function
function upload(base64, page, num, fileName) {
    
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
    if (base64.indexOf('png') > 0)
        file = new Blob([new Uint8Array(array)], { type: 'image/png' });
    else
        file = new Blob([new Uint8Array(array)], { type: 'image/jpeg' });
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
            var imgElem = $("#" + page + "-img" + num);
            //console.log(num);
            var imgOverlayElemBar = $("#" + page + "-img" + num + "-img-overlay-bar");
            var progressBarElem = $("#" + page + "-img" + num + "-pbar");
            var progressBarElemText = $("#" + page + "-img" + num + "-pbar span");
            var imgNameElem = $("#" + page + "-img" + num + "-name");
            var uploadingElem = $("#" + page + "-img" + num + "-upload-btn");
            imgOverlayElemBar.fadeIn(200, function () {
                progressBarElemText.hide();
                progressBarElem.fadeIn(100);
            });
            bucket.upload(params, options, function (err, data) {
                // console.log(data); /
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
                //console.log(progressBarElem);
                var percent = Math.ceil((progress.loaded / progress.total) * 100);
                progressBarElem.progressbar({
                    value: percent,
                    create: function (event, ui) {
                        progressBarElemText.text("Uploading...");
                        progressBarElemText.show();
                    },
                    complete: function (event, ui) {
                        progressBarElemText.text("Done. Please wait...");
                    }
                });
                //console.log(progress);
            }).send(function (err, data) {
                //console.log(data);
                if (num !== 6) {
                    var croppingElem = $("#" + page + "-img" + num + "-crop");
                    imgElem.croppie('bind', {
                        url: data.Location
                    }).then(function () {
                        imgElem.croppie('setZoom', '0.1');
                        imgNameElem.val(data.Location);
                        progressBarElem.fadeOut(200, function () {
                            imgOverlayElemBar.fadeOut();
                            $(".upload-btn" + num + ", .crop-btn" + num).removeClass('disabled');
                            progressBarElem.progressbar("destroy");
                        });
                        
                    });
                }
                else {
                    progressBarElem.fadeOut(200, function () {
                        imgOverlayElemBar.fadeOut();
                        $(".upload-btn" + num + ", .crop-btn" + num).removeClass('disabled');
                        progressBarElemText.hide(function () {
                            progressBarElem.progressbar("destroy");
                        });
                    })
                    imgNameElem.val(data.Location);
                    $("#perfc-img6").attr("src", data.Location);
                }
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
