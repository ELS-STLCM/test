var strListOfSelectedPatientItems = [];
var strUrlReferenceForSave = "";
var isEditMode = false;
var patient = {
    /*Common functions used in question bank module*/
    commonFunctions: {
        /*Search actions*/
        searchByText: function () {
            var searchText = $("#searchText").val();
            searchText = trimText(searchText);
            var isSearchValid = true;
            if (isNullOrEmpty(searchText) || searchText.length < 2) {
                isSearchValid = false;
                jAlert(SEARCH_VALIDATION, ALERT_TITLE, function (isOk) {
                });
            }
            if (isSearchValid) {
                var patientSearchContent = "../Patient/GiveSearchResults";
                var data = {
                    strSearchText: searchText
                };
                $("#authoringPractice").load(patientSearchContent, data);
            }
        }
    },
    gridOperations: {
        patientItemChanged: function (obj) {
            var idOfPatientItem = obj.id;
            strListOfSelectedPatientItems = check_AddOrRemoveIfItemExistsInString(strListOfSelectedPatientItems, idOfPatientItem, 'Ø', obj.checked);
            patientBuilderDataTable.fnDraw(false);
        },
        IsNullOrEmpty: function (value) {
            if (value == "" || value == null) {
                return true;
            }
            else {
                return false;
            }
        }
    },
    pageOperations: {
        savePatient: function (isEditMode, savePatientUrl) {
            if (patient.pageOperations.validatePatientFields()) {
                var patientJson = patient.pageOperations.getPatientJson();
                var urlToSavePatient = "../Patient/SavePatient?patientUrlReference=" + patient.pageOperations.getUrlReferenceForPatientSave(isEditMode, savePatientUrl) + "&folderIdentifier=" + getDivData("practiceTab", "currentFolderIdentifier") + "&isEditMode=" + isEditMode;
                if (patientJson != null) {
                    startAjaxLoader();
                    doAjaxCall("POST", patientJson, urlToSavePatient, patient.pageOperations.savePatientSuccess);
                    closeAjaxLoader();
                }
            }
        },
        getPatientJson: function () {
            var patientJson;
            patientJson = {
                "FirstName": $("#firstName").val(),
                "LastName": $("#lastName").val(),
                "MiddleInitial": $("#middleInitial").val(),
                "AgeInYears": ($("#ageInYears").val() == "" || $("#ageInYears").val() == null) ? "0" : $("#ageInYears").val(),
                "AgeInMonths": ($("#ageInMonths").val() == "" || $("#ageInMonths").val() == null) ? "0" : $("#ageInMonths").val(),
                "AgeInDays": ($("#ageInDays").val() == "" || $("#ageInDays").val() == null) ? "0" : $("#ageInDays").val(),
                "DateOfBirth": $("#dayOfBirth").val(),
                "Sex": $('input:radio[name=sex]:checked').val(),
                "UploadImage": patient.pageOperations.getOrSetImageReference(true, "patientImage", "NA"),
                "MedicalRecordNumber": $("#medicalRecordNumber").val(),
                "OfficeType": $("#officeType").val(),
                "Provider": $("#provider").val(),
                "CreatedTimeStamp": (createdTimestamp != "") ? createdTimestamp : "1/1/0001 9:22:42 PM"
            };
            return patientJson;
        },
        cancel: function (isEditMode, savePatientUrl) {
            var status = "Are you sure you want to cancel? Your changes will not be saved.";
            jConfirm(status, 'Cancel', function (isCancel) {
                if (isCancel) {
                    startAjaxLoader();
                    $("#authoringPractice").load("../Authoring/PatientBuilder");
                    closeAjaxLoader();
                }
            });
        },
        savePatientSuccess: function () {
            jAlert(SAVED_MESSAGE, "Alert", function (isOk) {
                if (isOk) {
                    $("#authoringPractice").load("../Patient/Patient");
                }
            });
        },
        validatePatientFields: function () {
            var errormessage = "";
            var isValid = false;
            errorMessage = "<UL>";
            if (isNullOrEmpty($("#firstName").val()) || isNullOrEmpty($("#lastName").val()) ||
                (isNullOrEmpty($("#ageInYears").val()) && isNullOrEmpty($("#ageInMonths").val()) &&
                isNullOrEmpty($("#ageInDays").val())) || isNullOrEmpty($("#dayOfBirth").val()) ||
                isNullOrEmpty($("#medicalRecordNumber").val()) || isNullOrEmpty($("#middleInitial").val()) ||
                hasDropDownValue($("#officeType").val()) || hasDropDownValue($("#provider").val()) ||
                ((patient.pageOperations.getOrSetImageReference(true, "patientImage", "NA") == undefined) ? true : false)) {
                errorMessage += "<LI>" + INPUT_REQUIRED + "</LI>";
                isValid = true;
            }
            if (isValid) {
                errorMessage += "</UL>";
                $("#validationSummary")[0].innerHTML = errorMessage;
                $("#validationSummary").focus();
                $("#validationSummary").show();
                $("#PatientData").scrollTo("#validationSummary", 300);
                return false;
            }
            else {
                return true;
            }
        },
        getUrlReferenceForPatientSave: function (isEditMode, savePatientUrl) {
            if (!isEditMode) {
                if (isNullOrEmpty(strUrlReferenceForSave)) {
                    return getDivData("practiceTab", "currentFolder");
                }
            }
            else {
                strUrlReferenceForSave = savePatientUrl;
            }
            return strUrlReferenceForSave;
        },
        getOrSetImageReference: function (isValueReturn, imageRefName, imageRefId) {
            var valueToreturn = "";
            if (isValueReturn) {
                return getImageRefData("patientImage")
            }
            else {
                loadImageToImageDiv(imageRefId, "patientImage", true)
            }
        },
        loadPatientInEditMode: function (strUrlOfPatient) {
            startAjaxLoader();
            $("#authoringPractice").empty();
            $("#authoringPractice").load("../Patient/RenderPatientInEditMode?patientUrl=" + strUrlOfPatient);
            closeAjaxLoader();
        }
    }
}