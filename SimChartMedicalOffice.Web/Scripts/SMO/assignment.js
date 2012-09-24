var imageCount = 1;
var imageReferenceList = new Array();
var isEdit = false;
var skillSetList;
var questionList;
var isSaveAndProceedStep2 = false;
var assignment = {
    previewPublish: {
        publish: function () {
            jConfirm(PUBLISH_MESSAGE, ALERT_TITLE, function (isOk) {
                if (isOk) {
                    startAjaxLoader();
                    var urlpublish = "../AssignmentBuilder/PublishAnAssignment?assignmentUrl=" + assignBuilder.commonFunctions.getAssignmentUniqueIdentifier();
                    doAjaxCall("POST", "", urlpublish, assignment.previewPublish.publishSuccess);
                }
            });
        },
        publishSuccess: function (ajaxResult) {
            if (ajaxResult != null && ajaxResult.Result) {
                jAlert(PUBLISHED_MESSAGE, ALERT_TITLE, function (isOk) {
                    if (isOk) {
                        assignment.previewPublish.publishedSettings();
                        closeAjaxLoader();
                    }
                });
            }
        },
        publishedSettings: function () {
            disableAButton("btnPublishAssignment");
            $("#backToStep4assignment").hide();
            $("#previewStep5assignment").removeClass("prefix_9").addClass("prefix_10");
        },
        backToStep4: function () {
        },
        preview: function () {
        }
    },
    /*Common functions used in Assignment module*/
    commonFunctions: {
        getOrSetImageReference: function (isValueReturn, imageRefName, imageRefId) {
            if (isValueReturn) {
                return getImageRefData(imageRefName);
            }
            else {
                var position = imageRefId.lastIndexOf("/");
                var imageId = imageRefId.slice(position + 1);
                imageId = "patientImage_" + imageId;
                $("<div class='grid_10' style='padding-top: 5px;' id='" + imageId + "_main'><div class='grid_17'><div id='patient_image' class='div-with-border image-holder-div'><img src='../Content/Images/Image_Div.png' alt='Image' id='" + imageId.toString() + "' class='image-holder' /></div> <div class='disabled-text' id='" + imageId.toString() + "_view'><u>View Large</u></div></div><div class='grid_15' id='" + imageId.toString() + "_remove'><div class='align-div-inline'><img alt='left' src='../Content/Images/Buttons/Button_left_curve_grey.jpg' class='standard-height-for-buttons' /></div><input type='button' class='remove-button align-div-inline standard-height-for-buttons' value='Remove' onclick='removeImage(this.parentNode.id)' /><div class='align-div-inline'><img alt='right' src='../Content/Images/Buttons/Button_right_curve_grey.jpg'class='standard-height-for-buttons' /></div></div><div class='grid_32 standard-text' style='padding-top:7px;'>Image Description:</div><input type='text' id='" + imageId.toString() + "_description' /><div class='clear-height-spacing clear'></div></div>").appendTo("#patient-image-content");
                loadImageToImageDiv(imageRefId, imageId, true);
                imageReferenceList.splice(imageCount, 0, imageId);
                imageCount++;
                $('#imageLoadAssignmentContent').dialog("close");
                if (imageCount == 7) {
                    $("#disabledUploadImageLink").show();
                    $("#uploadImageLink").hide();
                }
            }
        },
        /*function to removeattachment */
        removeAttachment: function (idMetadataContent) {
            // !!!important!!!!: Just removing the meta data in page. Image should be removed only on click of save.
            jConfirm(IMAGE_REMOVE, "Remove Image", function (isOk) {
                if (isOk) {
                    getImageRefData(idMetadataContent);
                    removeMetaDataForImage(idMetadataContent);
                    $("#" + idMetadataContent + "_main").remove();
                    for (var imgList = 0; imgList < imageCount - 1; imgList++) {
                        if (imageReferenceList[imgList] == idMetadataContent) {
                            imageReferenceList.splice(imgList, 1);
                        }
                    }
                    imageCount--;
                    if (imageCount == 6) {
                        $("#disabledUploadImageLink").hide();
                        $("#uploadImageLink").show();
                    }
                }
            });
        },

        assignmentImageUpload: function () {
            resetImageUploadForm();
            $('#imageLoadAssignmentContent').load("../QuestionBank/SimOfficeImageUpload", function () {
                $('#imageLoadAssignmentContent').dialog({
                    autoOpen: false,
                    modal: true,
                    closeOnEscape: false,
                    resizable: false,
                    open: function () {
                        applyClassForDialogHeader();
                    },
                    title: 'Image Upload',
                    minHeight: 100,
                    minWidth: 450
                });
                $('#imageLoadAssignmentContent').dialog("open");
            });
        },
        saveStep2: function (saveAndProceed) {
            if (saveAndProceed != null & saveAndProceed != undefined) {
                isSaveAndProceedStep2 = saveAndProceed;
            }
            if (assignment.commonFunctions.validatePatientFields()) {
                startAjaxLoader();
                $("#validationSummary").hide();
                var assignmentJson;
                var mceOutput;
                var imageDescriptionList = new Array();
                var imageUrlList = new Array();
                for (var imgId = 0; imgId < imageReferenceList.length; imgId++) {
                    imageDescriptionList[imgId] = $("#" + imageReferenceList[imgId] + "_description").val();
                    imageUrlList[imgId] = getImageRefData(imageReferenceList[imgId]);
                }
                mceOutput = tinyMCE.get("OrientationEditor").getContent();
                assignmentJson = {
                    "Orientation": mceOutput,
                    "PatientImageReferance": imageUrlList,
                    "PatientImageDescription": imageDescriptionList,
                    "VideoReferance": videoList,
                    "Url": assignBuilder.commonFunctions.getAssignmentUniqueIdentifier()
                };
                var urlToAssignment = "../AssignmentBuilder/SaveOrientation";
                //                if (assignmentJson != null) {
                //                    startAjaxLoader();
                //                    doAjaxCall("POST", assignmentJson, urlToAssignment, assignment.commonFunctions.getAssignment);
                //                    closeAjaxLoader();
                //                }
                if (assignmentJson != null) {
                    $.ajax({
                        url: urlToAssignment,
                        type: 'POST',
                        dataType: 'json',
                        data: JSON.stringify(assignmentJson),
                        error: function (result) {
                            if (result != null) {
                                jAlert('Error');
                            }
                        },
                        success: function (result) {
                            //closeAjaxLoader();
                            if (result.AjaxResult == "") {
                                assignment.commonFunctions.getAssignment(result.AssignmentUrl);

                                if (isSaveAndProceedStep2) {
                                    isSaveAndProceedStep2 = false;
                                    assignBuilder.redirectionFunctions.loadStep3OfAssignmentBuilder();
                                } else {
                                    jAlert("Saved", ALERT_TITLE, function () {

                                    });
                                }
                            }
                        }
                    });
                }
                return true;
            }
            else {
                return false;
            }
        },
        initializePopup: function () {
            $("#videoAudioUpload").show();
            $("#videoAudioUpload").dialog({ 
                minHeight: 350,
                minWidth: 450,
                modal: true,
                position: 'center',
                resizable: false,
                autoOpen: true,
                closeOnEscape: false,
                title: 'Video/Audio Upload',
                open: function () {
                    applyClassForDialogHeader();
                },
                overlay: { opacity: 0.5, background: 'black' }
            });
        },
        contentChanged: function (patientId, patientUrl) {            
            var urlToPatient = "../Patient/GetPatientForGuid?patientUrl=" + patientUrl;
            startAjaxLoader();
            $.ajax({
                url: urlToPatient,
                dataType: 'json',
                error: function (result) {
                    if (result != null) {
                        jAlert('Error');
                    }
                },
                success: function (result) {
                    if (result.Result.LastName != "" && result.Result.LastName != null) {
                        $(".clear-background").css("background", "none");
                        $(".clear-background").css("color", "black");
                        $("#" + patientId).css("background", "#0070C0");
                        $("#" + patientId).css("color", "white");
                        loadImageToImageDiv(result.Result.UploadImage, "assignmentPatientImage", true);
                        $("#patient-name").html(result.Result.LastName.toString() + ", " + result.Result.FirstName.toString() + " " + result.Result.MiddleInitial.toString());
                        $("#patient-gender").html(result.Result.Sex.toString() + ", " + result.PatientAge.toString());
                        $("#patient-DOB").html(result.Result.DateOfBirth.toString());
                    }
                }
            });
            closeAjaxLoader();
        },
        getPatientList: function () {
            for (var indexSource = 0; indexSource < result.Result.length; indexSource++) {
                var patientItem = '<div style="height:7px;">&nbsp;</div>' + '<div id="' + patientList[indexSource].UniqueIdentifier + '" class="grid_32 clear-background standard-text header-text select-hand" onclick=assignment.commonFunctions.contentChanged("' + patientList[indexSource].UniqueIdentifier + '","' + patientList[indexSource].Url + '")>' + patientList[indexSource].LastName + ", " + patientList[indexSource].FirstName + " " + patientList[indexSource].MiddleInitial + '</div>' + '<br />';
                $("#patient-list").append(patientItem);
            }
        },
        validatePatientFields: function () {
            var isValid = false;
            var errorMessage = "<UL>";
            if (isNullOrEmpty(tinyMCE.get("OrientationEditor").getContent())) {
                errorMessage += "<LI>" + INPUT_REQUIRED + "</LI>";
                isValid = true;
            }
            if (isValid) {
                errorMessage += "</UL>";
                $("#validationSummary")[0].innerHTML = errorMessage;
                $("#validationSummary").focus();
                $("#validationSummary").show();
                $("#OrientationData").scrollTo("#validationSummary", 300);
                return false;
            }
            else {
                return true;
            }
        },
        getAssignment: function (assignmentUrlResult) {
            var urlToAssignment = "../AssignmentBuilder/GetAssignment?assignmentUniqueIdentifier=" + assignmentUrlResult;
            $.ajax({
                url: urlToAssignment,
                dataType: 'json',
                type: 'POST',
                asynch: false,
                error: function (result) {
                    if (result != null) {
                        alert(result);
                    }
                },
                success: function (result) {
                    if (result.Result.Orientation != null && result.Result.Orientation != "") {
                        tinyMCE.get("OrientationEditor").setContent(result.Result.Orientation);
                    }
                    $("#patient-image-content").empty();
                    $("#video-content").empty();
                    imageCount = 1;
                    imageReferenceList = new Array();
                    //videoList = new Array();
                    if (result.Result.PatientImageReferance != null) {
//                        var imgList = new Array();
                        for (var imgCount = 0; imgCount < result.Result.PatientImageReferance.length; imgCount++) {
                            assignment.commonFunctions.getOrSetImageReference(false, "", result.Result.PatientImageReferance[imgCount]);
                            var position = result.Result.PatientImageReferance[imgCount].lastIndexOf("/");
                            var imageId = result.Result.PatientImageReferance[imgCount].slice(position + 1);
                            imageId = "patientImage_" + imageId;
                            $("#" + imageId + "_description").val(result.Result.PatientImageDescription[imgCount]);
                        }
                    }
                    if (result.Result.VideoReferance != null) {
                        for (var videoCount = 0; videoCount < result.Result.VideoReferance.length; videoCount++) {
                            setVideo(result.Result.VideoReferance[videoCount], videoCount);
                        }
                    }

                }
            });
        },
        initializeFlexboxForPatient: function () {
            var urlPatientNameList = "../AssignmentBuilder/GetAllPatientsName";
            doAjaxCall("POST", "", urlPatientNameList, assignment.commonFunctions.successFlexBoxForFilterByPatientName);
        },
        successFlexBoxForFilterByPatientName: function (result) {
            if (result.PatientList != null) {
                patientTypeList.results = eval('(' + result.PatientList + ')');
                patientTypeList.total = patientTypeList.results.count;
                $('#filterByPatientName').flexbox(patientTypeList, {
                    initialValue: savedPatientValue,
                    resultTemplate: '{name}',
                    width: 200,
                    paging: false,
                    maxVisibleRows: 10,
                    noResultsText: '',
                    noResultsClass: '',
                    matchClass: '',
                    matchAny: true
                    //                    onSelect: function () {
                    //                        startAjaxLoader();
                    //                        selectedQuestionTypeFilterText = $('#' + divId + "_hidden").val();
                    //                        skillSet.stepTwoSkillStructure.getQuestionBank();
                    //                        closeAjaxLoader();
                    //                    }
                });
                $("#filterByPatientName_ctr").attr("style", "left: 0px;top: 22px;width: 200px;");
                $("#filterByPatientName_ctr").hide();
                $("#filterByPatientName_input").watermark("Select", { className: 'watermark watermark-text' });
                $("#select-patient").show();
                $("#create-new-patient").hide();
            }
        },
        cancel: function () {
            var status = "Are you sure you want to cancel? Your changes will not be saved.";
            jConfirm(status, 'Cancel', function (isCancel) {
                if (isCancel) {
                    startAjaxLoader();
                    assignBuilder.redirectionFunctions.loadStep2OfAssignmentBuilder();
                    closeAjaxLoader();
                }
            });
        },
        patientRadioSelection: function () {
            if ($('input:radio[name=PatientOption]:checked').val() == "createNewPatient") {
                $("#create-new-patient").show();
                $("#select-patient").hide();
                $('#filterByPatientName_input').remove();
                $('#filterByPatientName_hidden').remove();
                $('#filterByPatientName_arrow').remove();
                $('#filterByPatientName_ctr').remove();
            }
            else {
                $('#filterByPatientName_input').remove();
                $('#filterByPatientName_hidden').remove();
                $('#filterByPatientName_arrow').remove();
                $('#filterByPatientName_ctr').remove();
                assignment.commonFunctions.initializeFlexboxForPatient();
            }
        }

    },
    officeSetting: {
        loadOfficeSettingpage: function () {
            var url = "../AssignmentBuilder/GetPatientList?assignmentUrl=" + assignBuilder.commonFunctions.getAssignmentUniqueIdentifier();
            //        doAjaxCall("GET", "", url, assignment.commonFunctions.getPatientList);
            startAjaxLoader();
            $.ajax({
                url: url,
                dataType: 'json',
                asynch: false,
                error: function (result) {
                    if (result != null) {
                        alert('Error');
                    }
                },
                success: function (result) {
                    $("#patient-list").empty();
                    if (result.Result != null) {
                        for (var indexSource = 0; indexSource < result.Result.length; indexSource++) {
                            var patientItem = '<div style="height:7px;">&nbsp;</div>' + '<div id="' + result.Result[indexSource].UniqueIdentifier + '" class="grid_32 clear-background standard-text select-hand" style="height:17px;padding-left: 5px;" onclick=assignment.commonFunctions.contentChanged("' + result.Result[indexSource].UniqueIdentifier + '","' + result.Result[indexSource].Url + '")>' + "  " + result.Result[indexSource].LastName + ", " + result.Result[indexSource].FirstName + " " + result.Result[indexSource].MiddleInitial + '</div>' + '<br />';
                            $("#patient-list").append(patientItem);
                            if (indexSource == 0) {
                                $("#" + result.Result[0].UniqueIdentifier).addClass("header-text");
                                assignment.commonFunctions.contentChanged(result.Result[0].UniqueIdentifier, result.Result[0].Url);
                            }
                        }
                    }
                }
            });
            closeAjaxLoader();
        }
    }
};