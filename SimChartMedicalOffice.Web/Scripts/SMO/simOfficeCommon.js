var imageRefIdNameTemp = "";
var contentPlaceHolderforImage = "";
function encodeSpecialSymbols(value) {
    if (value != undefined) {
        value = value.replace(/\\/g, "\\\\");
        value = value.replace(/\"/g, "\\\"");
        return encodeURIComponent(value);
    } else {
        return value;
    }
}
//  Competency Master Load Content 
function showExcelFileUpload() {
    $('#CompetencyMasterLoadContent').dialog({
        autoOpen: true,
        modal: true,
        title: 'Excel File Upload',
        minHeight: 100,
        minWidth: 400
    });
}
//function closeDialog(dialogToClose) {
//    $('#' + dialogToClose).dialog("close").dialog('destroy');
//}

function validateError() {
    jAlert(SAVE_ERROR);
}

function validateSuccess(result) {
    // call has been success now check what we have got        
    jAlert(result.Success);
}
function startAjaxLoader() {
    $.blockUI({
        message: "<h1><img src='../Content/Images/loading_black.gif'/><p style='color:#fff'>Please wait...</p></h1>",
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: "",
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });
}
function closeAjaxLoader() {
    setTimeout($.unblockUI, 1000);
}
function startBackgound_Blur(){
    $.blockUI({
        message: null,
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: "",
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });
}
function closeBackgound_Blur() {
    setTimeout($.unblockUI, 500);
}

/*
* Action       : common method to do ajax Calls
* Input params : typeOfCall - either GET/POST
data  - object/list.
urlForAjax - URL of call
successFunction - function name to be called after successful ajax
*/
function doAjaxCall(typeOfCall, data, urlForAjax, successFunction) {
    $.ajax({
        async: false,
        type: typeOfCall,
        dataType: 'json',
        data: JSON.stringify(data),
        url: urlForAjax,
        success: successFunction,
        error: ajaxError
    });
}


/*
* Action       : common method to be called after a error in ajax request
* Input params : inbuilt from the ajax plugin
*/
function ajaxError(xhr, status, error) {
    var xhrResponse = xhr.responseText;
    var xhrStatus = xhr.status + ':' + xhr.statusText;
    if (xhrResponse != null && xhrResponse != undefined && xhrResponse != "") {
        if (xhrResponse.indexOf("SESSIONTIMEOUT") != -1) {
            //window.location.href = "TO-DO";
        }
    }
    jAlert("Error Message");
    return false;
}

/*
* Action       : To check if value is null/empty/undefined
* Input params : value - value to check
* Return params: true if value is null/empty/undefined
*                            false otherwise
*/
function isNullOrEmpty(value) {
    if (value == "" || value == null || value == undefined || value == "null") {
        return true;
    }
    else {
        return false;
    }
}


/*
* Action       : To eval a json to object
* Input params : jsonString - value to eval
* Return params: object
*/
function evaluateResult(jsonString) {
    return eval('(' + jsonString + ')');
}


/*
* Action       : To add class to a dom
* Input params : objId - Object id of dom.
*                            classToAdd - class to be added
*                            
*/
function addClassToObj(objId, classToAdd) {
    $("#" + objId).addClass(classToAdd);
}

/*
* Action       : To remove class to a dom
* Input params : objId - Object id of dom.
*                            classToRemove - class to be removed
*                            
*/
function removeClassToObj(objId, classToRemove) {
    $("#" + objId).removeClass(classToRemove);
}

/*
* Action       : To check if dom has specific class
* Input params : objId - Object id of dom.
*                            classToRemove - Class to check
* Return params: true if has the class else false.
*/
function checkIfDomHasClass(objId, classToCheck) {
    $("#" + objId).hasClass(classToRemove);
}


/*
* Action       : To display a string with ellipsis(...)
* Input params : actualString - string to check
*                            maxLengthToDisplay - MaxLength for ellipsis
* Return params: string with ellipsis
*/
function getStringToDisplayFromMaxLength(actualString, maxLengthToDisplay) {
    if (actualString.length > maxLengthToDisplay) {
        return actualString.substring(0, maxLengthToDisplay - 3) + ELLIPSIS;
    }
    else {
        return actualString;
    }
}

/*
* Action       : To convert date to string/ string to date
* Input params : actualString - string to convert to a date / date to convert to string
* Return params: date/string
*/
function convertStringToDate(stringValue) {
    return new Date(stringValue);
}
function convertDateToString(dateValue) {
    return dateFormat(new Date(dateValue), JS_FORMAT);
}


/*
* Action       : To trim spaces in a text
* Input params : stringToTrim - string to trim
*/
function trimText(stringToTrim) {
    if (!isNullOrEmpty(stringToTrim))
        return stringToTrim.replace(/^\s+|\s+$/g, "");
    else
        return stringToTrim;
}

function trimTrailingAndPrecedingSpace(stringToTrim) {
    if (!isNullOrEmpty(stringToTrim))
        return $.trim(stringToTrim);
    else
        return stringToTrim;
}

/*
* Action       : To get client system time stamp
*/
function getClientTimeStamp() {
    return new Date();
}
function getClientTimeStampString() {
    return dateFormat(getClientTimeStamp(), JS_CLIENT_DATE_FORMAT);
}
function getLoginUserId() {
    return "";
}
/*
* Action : for validating MM-DD-YYYY
*/
function isValidDate(subject) {
    if (subject.match(/^(?:(0[1-9]|1[012])[\- \/.](0[1-9]|[12][0-9]|3[01])[\- \/.](19|20)[0-9]{2})$/)) {
        return true;
    } else {
        return false;
    }
}

/*
* Action : to show image upload dialog
*/

function showImageUpload(imageId) { 
    qnOrAnsImageRefIdNameTemp = imageId;
    resetImageUploadForm();
    $('#imageLoadContent').load("../QuestionBank/SimOfficeImageUpload", function () {
        $('#imageLoadContent').dialog({
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
        $('#imageLoadContent').dialog("open");
    });
}

//function showImageUploadForPatient(imageId) {
//    qnOrAnsImageRefIdNameTemp = imageId;
//    resetImageUploadForm();
//    $('#imageLoadContentPatient').load("../QuestionBank/SimOfficeImageUpload", function () {
//        var $dialog = $('#imageLoadContentPatient').dialog({
//            autoOpen: false,
//            modal: true,
//            closeOnEscape: false,
//            resizable: false,
//            open: function (event, ui) {
//                applyClassForDialogHeader();
//            },
//            title: 'Image Upload',
//            minHeight: 100,
//            minWidth: 450
//        });
//        $('#imageLoadContentPatient').dialog("open");
//    });
//}

function showImageUploadForPatient(imageId,contentId) {
    imageRefIdNameTemp = imageId;
    contentPlaceHolderforImage = contentId;
    resetImageUploadForm();
    $('#' + contentPlaceHolderforImage).load("../QuestionBank/SimOfficeImageUpload", function () {
        $('#' + contentPlaceHolderforImage).dialog({
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
        $('#' + contentPlaceHolderforImage).dialog("open");
    });
}



/*
* Action : to reset iamge upload ajax form
*/
function resetImageUploadForm() {
    $("#ajaxUploadForm").resetForm();
    $("#validation_summary").text('').html("");
    $(".validation-summary").text('').html("");
    $(".file-upload-input").val("");
}

/*Action : Image Upload success function*/

function checkTheFileUploadSuccess(resultOfAjax) {
    $(".file-upload-popup").unblock();
    if (checkImageUploadAndDisplayMessage(resultOfAjax)) {
        questionBank.commonFunctions.getOrSetImageReference(false, qnOrAnsImageRefIdNameTemp, resultOfAjax.imageRefId);
        patient.pageOperations.getOrSetImageReference(false, imageRefIdNameTemp, resultOfAjax.imageRefId);
        assignment.commonFunctions.getOrSetImageReference(false, imageRefIdNameTemp, resultOfAjax.imageRefId);
        $("#imageLoadContent").dialog("close");
        if (contentPlaceHolderforImage != "") {
            $('#' + contentPlaceHolderforImage).dialog("close");
        }        
        $("#ajaxUploadForm").resetForm();
    }
    return false;
}

/*
* Action : to show close any dialog knowing the Id
*/
function closeDialog(dialogToClose) {
    $('#' + dialogToClose).dialog("close").dialog('destroy').remove();
    return false;
}
function closeDialogWithoutDestroy(dialogToClose) {
    $('#' + dialogToClose).dialog("close");
    return false;
}
/*
* Action : to display the success message after image upload.
*/
function checkImageUploadAndDisplayMessage(jsonResult, imageId) {    
    if (jsonResult != undefined) {
        if (!jsonResult.isSuccessfulUpload) {
            $("#ajaxUploadForm #validation_summary").text(jsonResult.strUploadMessage).show();
            return false;
        } else {
            jAlert(jsonResult.strUploadMessage);
            return true;
        }
    }
    return false;
}
/* Action : To load image to a content Div */
function loadImageToImageDiv(imageGuid, imageContentId, isMetaDataSet) {
    $("#" + imageContentId).attr('src', "");
    $("#" + imageContentId).attr('src', '../QuestionBank/GetAttachment?strGuId=' + imageGuid + '&nocache=1');
    if (isMetaDataSet) {
        setMetaDataToImage(imageContentId, imageGuid);
    }
}
function setMetaDataToImage(imageContentId, imageGuid) {
    $("#" + imageContentId + "_view").removeClass("disabled-text").addClass("link select-hand");
    $("#" + imageContentId + "_view").data("imgRef", imageGuid);
    $("#qn_remove_image").data("imgRef", imageGuid);
    $("#" + imageContentId + "_remove").removeClass("hide-content").addClass("show-content");
    $("#" + imageContentId + "_view").live('click', function () {
        var imagRefToLoad = $("#" + this.id).data("imgRef");
        var imageIdLoad = this.id.split('_view')[0];
        viewImageLarger(imageIdLoad, "image_view"); //always have the id of image as image_view
    });
}
function removeMetaDataForImage(imageContentId) {
    $("#" + imageContentId + "_view").removeClass("link select-hand").addClass("disabled-text");
    $("#" + imageContentId + "_view").removeData("imgRef");
    $("#" + imageContentId + "_view").die('click');
    $("#" + imageContentId).attr('src', "");
    jAlert("Removed", "Alert", function (isOk) {
        if (isOk) {
            $("#" + imageContentId + "_remove").removeClass("show-content").addClass("hide-content");
            questionBank.commonFunctions.unCheckRadioOnRemovingImage(imageContentId);
        }
    });
}
/* Action : To load image to a content Div */
function getImageRefData(imgRefIdDiv) {
    var imagRefToLoad = $("#" + imgRefIdDiv + "_view").data("imgRef");
    return imagRefToLoad;
}
/*Action : To show loading image on loading image*/
function blockUploadOnLoading() {
    $(".file-upload-popup").block({
        message: '<h3><img height="16px" width="16px" src="../Content/Images/loading.gif"/> Uploading file...</h3>'
    });
}

/*Action : To initialize a ajaxForm*/
function initializeAjaxForm(divIdToInitialize) {

    $(".file-upload-popup").ajaxForm({
        iframe: true,
        dataType: "json",
        beforeSubmit: function () { blockUploadOnLoading(); },
        success: function (result) { checkTheFileUploadSuccess(result); }
    });
}

/*Action : To view image larger*/
function viewImageLarger(imageId, imgLargeId) {
    startAjaxLoader();
    loadImageToImageDiv(getImageRefData(imageId), imgLargeId, false);
    $('#image_view_load_inner_content').dialog({
        autoOpen: false,
        modal: true,
        closeOnEscape: false,
        resizable: false,
        open: function () {
            $('#image_view_load_inner_content').css('overflow', 'hidden');
            removeDialogHeader();
        },
        beforeClose: function () {
            reapplyDialogHeader();
        },
        width: 430
    });
    $('#image_view_load_inner_content').dialog("open");
    closeAjaxLoader();
}
/*Action : Remove title bar of ui -widget - dialog*/
function removeDialogHeader() {
    $(".ui-dialog-titlebar").removeClass("ui-widget-header");
    $(".ui-dialog-titlebar").css('background-color', 'white');
}

/*Action : Remove title bar of ui -widget - dialog*/
function removeDialogForGrayHeader() {
    $(".ui-dialog-titlebar").removeClass("ui-widget-header");
    $(".ui-dialog-titlebar").css('background-color', '#ededed');
}

/*Action : to reapply header changes*/
function reapplyDialogHeader() {
    $(".ui-dialog-titlebar").addClass("ui-widget-header");
    $(".ui-dialog-titlebar").css('background-color', '#dedede');
    $(".ui-dialog-titlebar").css('color', 'black');
}
/*Action : apply title bar of ui -widget - dialog-header*/
function applyClassForDialogHeader() {
    $(".ui-dialog-titlebar").css('background-color', '#dedede').css('background-image', 'none');
}
/*Action : apply title bar of ui -widget - dialog-header*/
function applyBlueClassForDialogHeader() {
    $(".ui-dialog-titlebar").css('background-color', '#0060A8').css('background-image', 'none');
    $(".ui-dialog-titlebar").css('color', '#FFFFFF').css('background-image', 'none');
}
/*Action : apply blue border for dialog*/
function applyBlueBorderForDialogHeader() {
    $(".ui-dialog").css('border-color', '#0060A8');
}

/*Action : to remove the default Close icon in dialog-header*/
function removeCloseIconForDialogHeader() {
    $(".ui-dialog-titlebar-close").hide();
}
/*Action : to apply the default Close icon in dialog-header*/
function applyCloseIconForDialogHeader() {
    $(".ui-dialog-titlebar-close").show();
}

/*Action :function to get the url based on folders selected*/
function getOrSetReferenceUrlFromFolders(isValueSet, urlToSet) {
    if (isValueSet) {
        //To-Do
    }
    else {
        return strUrlReferenceForQnInEditMode;
    }
    return false;
}
/*Action : function to remove attachment from app*/
function removeAttachmentFrmApp(guidOfAttachment) {
    var urlToRemoveAttachment = "../QuestionBank/RemoveAttachment?strGuId=" + encodeURIComponent(guidOfAttachment);
    doAjaxCall("POST", "", urlToRemoveAttachment, attachmentRemoveSuccess);
    return true;
}

function attachmentRemoveSuccess() {
    //To-Do
}

/*
* Action       : To check if dropdown value is null/empty/undefined
* Input params : value - value to check
* Return params: true if value is null/empty/undefined
*                            false otherwise
*/
function hasDropDownValue(value) {
    if (value == "" || value == null || value == undefined || value == DROPDOWN_SELECT || value == SelectNumericValue) {
        return true;
    }
    else {
        return false;
    }
}

function getListOfItemsForCompetencyFlexBox(strFilterText) {
    var urlForString = "../QuestionBank/GetCompetenciesForDropDown?strFilterText=" + strFilterText;
    doAjaxCall("POST", "", urlForString, this.successCompetencyFetch1);
}
function successCompetencyFetch1(result) {
    if (result.competencyStringListTemp != null) {
        competencyArray = result.competencyArray;
        competencyStringListTemp.results = eval('(' + result.competencyStringListTemp + ')');
        competencyStringListTemp.total = competencyStringListTemp.results.length;
        populateLinkedCompetencies();
    }
    return null;
}

function AssignDefaultTextValue(divId) {
    $('#' + divId + "_input").watermark(WATER_MARK_COMPETENCY, { className: 'watermark watermark-text' });
}

function initializeFlexBox(divId, lstOfItems) { 
    $('#' + divId).empty();
    $('#' + divId).flexbox(lstOfItems, {
        resultTemplate: '{name}',
        width: 600,
        paging: false,
        maxVisibleRows: 10,
        noResultsText: '',
        noResultsClass: '',
        matchClass: '',
        matchAny: true,
        doNotClearId: true
    });
    $('#' + divId + "_ctr").attr("style", "left: 0px; top: 22px; width: 600px; display: none;");
    $('#' + divId + "_input").watermark(WATER_MARK_COMPETENCY, { className: 'watermark watermark-text' });
    $('#LinkedCompetency_input').live('blur', function () {
        if ($('#LinkedCompetency_input').val() == "") {
            $('#LinkedCompetency_input').text = WATER_MARK_COMPETENCY;
            $('#LinkedCompetency_input').addClass('watermark watermark-text');
        } else {
            $('#LinkedCompetency_input').removeClass('watermark watermark-text');
        }
    });
}
function populateLinkedCompetencies() {
    initializeFlexBox('LinkedCompetency', competencyStringListTemp);
}

function initializeFlexBoxForCompetencyCategory(divId, lstOfItems, defaultValue) {
    lstOfItemsForComp = {};
    lstOfItemsForComp.results = lstOfItems;
    lstOfItemsForComp.total = lstOfItemsForComp.results.length;
    $('#' + divId).flexbox(lstOfItemsForComp, {
        initialValue: defaultValue,
        resultTemplate: '{name}',
        width: 290,
        paging: false,
        maxVisibleRows: 10,
        noResultsText: '',
        noResultsClass: '',
        matchClass: '',
        matchAny: true
        });
    $('#' + divId + "_input").live('keyup', function () {
        if (validateSpecialCharacters(this.value)) {
            var competencyCategoryItem = this.value;
            competencyCategoryItem = competencyCategoryItem.substring(0, competencyCategoryItem.length - 1);
            $('#' + divId + "_input").val(competencyCategoryItem);
            return false;
        }
        else {
            return true;
        }
    });
    $('#' + divId + "_ctr").attr("style", "left: 0px;top: 22px;width: 290px;");
    $('#' + divId + "_ctr").hide();
    $('#' + divId + "_input").watermark("Select or Type New", { className: 'watermark watermark-text' });
    $('#' + divId + "_input").live('blur', function () {
        if ($('#' + divId + "_input").val() == "") {
            $('#' + divId + "_input").text = "Select or Type New";
            $('#' + divId + "_input").addClass('watermark watermark-text');
        } else {
            $('#' + divId + "_input").removeClass('watermark watermark-text');
        }
    });
}

var isFilterValueSelectedForSearch = false;
var isFilterValueClearedForSearch = false;
var isFilterValueSelected = false;
function initializeFlexBoxForFilterByType(divId, lstOfItems) {
    lstOfItemsInQuestionType = {};
    lstOfItemsInQuestionType.results = lstOfItems;
    lstOfItemsInQuestionType.total = lstOfItemsInQuestionType.results.length;
    $('#' + divId).flexbox(lstOfItemsInQuestionType, {
        resultTemplate: '{name}',
        width: 147,
        paging: false,
        maxVisibleRows: 10,
        noResultsText: '',
        noResultsClass: '',
        matchClass: '',
        matchAny: true,
        onSelect: function () {
            isFilterValueSelected = true;
            isFilterValueSelectedForSearch = true;
            startAjaxLoader();
            isSearchPage = false;
            strListOfSelectedQuestionItems = "";
            questionBankDataTable.fnDraw();
        }
    });
    $('#' + divId + "_input").watermark("Filter by question type", { className: 'watermark watermark-text' });
    $('#FilterByQuestionType_input').live('blur', function () {
        if ($('#FilterByQuestionType_input').val() == "") {
            $('#FilterByQuestionType_input').text = "Filter by question type";
            $('#FilterByQuestionType_input').addClass('watermark watermark-text');
            isFilterValueSelected = false;
            isFilterValueSelectedForSearch = false;
            //            startAjaxLoader();
            //            questionBankDataTable.fnDraw();
        } else {
            $('#FilterByQuestionType_input').removeClass('watermark watermark-text');
        }
    });
    $('#FilterByQuestionType_input').live('keyup', function () {
        if (isFilterValueSelected) {
            if ($('#FilterByQuestionType_input').val() == "") {
                startAjaxLoader();
                strListOfSelectedQuestionItems = "";
                questionBankDataTable.fnDraw();
            }
        }
    });
}

/*Common function to initialize a tab*/
function getTabForContent(divId) {
    $("#" + divId).tabs({
        cache: false,
        spinner: ""
    });
}

function check_AddOrRemoveIfItemExistsInString(strItemsWithDelimiter, itemToCheckIfExists, strDelimiterToCheck, isAdd) {
    if (strItemsWithDelimiter != null) {
        if (strItemsWithDelimiter.toString().indexOf(itemToCheckIfExists) < 0) {
            var stringToAppend = isNullOrEmpty(strItemsWithDelimiter) ? (itemToCheckIfExists) : (strDelimiterToCheck + itemToCheckIfExists);
            strItemsWithDelimiter = (isNullOrEmpty(strItemsWithDelimiter) ? '' : strItemsWithDelimiter) + stringToAppend;
            return strItemsWithDelimiter;
        }
        else if (strItemsWithDelimiter.toString().indexOf(itemToCheckIfExists) >= 0 && !isAdd) {
            if (strItemsWithDelimiter.toString().indexOf(strDelimiterToCheck + itemToCheckIfExists) != -1) {
                strItemsWithDelimiter = strItemsWithDelimiter.replace(strDelimiterToCheck + itemToCheckIfExists, "");
            }
            else if (strItemsWithDelimiter.toString().indexOf(itemToCheckIfExists + strDelimiterToCheck) != -1) {
                strItemsWithDelimiter = strItemsWithDelimiter.replace(itemToCheckIfExists + strDelimiterToCheck, "");
            }
            else if (strItemsWithDelimiter.toString().indexOf(itemToCheckIfExists) != -1) {
                strItemsWithDelimiter = strItemsWithDelimiter.replace(itemToCheckIfExists, "");
            }
            return strItemsWithDelimiter;
        }
        return strItemsWithDelimiter;
    }
    return false;
}

/*Common function to disable a button*/
function disableAButton(buttonId) {
    $("#" + buttonId).removeClass('transaction-button').removeClass('navigation-button').removeClass('cancel-button').removeClass('remove-button');
    $("#" + buttonId).addClass('disabled-button');
    $("#" + buttonId + "LeftCurve").attr('src', '../Content/Images/Buttons/Button_left_curve_grey.jpg');
    $("#" + buttonId + "RightCurve").attr('src', '../Content/Images/Buttons/Button_right_curve_grey.jpg');
    $("#" + buttonId).attr('disabled', true);
    $("#" + buttonId).die('click').unbind('click');
}

function disableAButtonForForms(buttonId) {
    $("#" + buttonId).removeClass('transaction-button').removeClass('navigation-button').removeClass('cancel-button').removeClass('remove-button');
    $("#" + buttonId).addClass('disabled-button');
    $("#" + buttonId + "-LeftCurve").attr('src', '../Content/Images/Buttons/Button_left_curve_grey.jpg');
    $("#" + buttonId + "-RightCurve").attr('src', '../Content/Images/Buttons/Button_right_curve_grey.jpg');
    $("#" + buttonId).attr('disabled', true);
    $("#" + buttonId).die('click').unbind('click');
}

/*Common function to enable a button*/
function enableAButton(buttonId, type, classForMiddleCurve, functionNameToBind) {
    var leftCurve = "";
    var rightCurve = "";
    switch (type) {
        case GREY_BUTTON:
            leftCurve = "../Content/Images/Buttons/Button_left_curve_grey.jpg";
            rightCurve = "../Content/Images/Buttons/Button_right_curve_grey.jpg";
            break;
        case ORANGE_BUTTON:
            leftCurve = "../Content/Images/Buttons/Button_left_curve_orange.jpg";
            rightCurve = "../Content/Images/Buttons/Button_right_curve_orange.jpg";
            break;
        case BLUE_BUTTON:
            leftCurve = "../Content/Images/Buttons/Button_left_curve_blue.jpg";
            rightCurve = "../Content/Images/Buttons/Button_right_curve_blue.jpg";
            break;
        default:
            break;
    }
    $("#" + buttonId).removeClass('disabled-button');
    if (!$("#" + buttonId).hasClass(classForMiddleCurve)) {
        $("#" + buttonId).addClass(classForMiddleCurve);
    }
    $("#" + buttonId).attr('disabled', false);
    $("#" + buttonId).live('click', functionNameToBind);
    $("#" + buttonId + "LeftCurve").attr('src', leftCurve);
    $("#" + buttonId + "RightCurve").attr('src', rightCurve);
}

function enableAButtonForms(buttonId, type, classForMiddleCurve, functionNameToBind) {
    var leftCurve = "";
    var rightCurve = "";
    switch (type) {
        case GREY_BUTTON:
            leftCurve = "../Content/Images/Buttons/Button_left_curve_grey.jpg";
            rightCurve = "../Content/Images/Buttons/Button_right_curve_grey.jpg";
            break;
        case ORANGE_BUTTON:
            leftCurve = "../Content/Images/Buttons/Button_left_curve_orange.png";
            rightCurve = "../Content/Images/Buttons/Button_right_curve_orange.png";
            break;
        case BLUE_BUTTON:
            leftCurve = "../Content/Images/Buttons/Button_left_curve_blue.jpg";
            rightCurve = "../Content/Images/Buttons/Button_right_curve_blue.jpg";
            break;
        default:
            break;
    }
    $("#" + buttonId).removeClass('disabled-button');
    if (!$("#" + buttonId).hasClass(classForMiddleCurve)) {
        $("#" + buttonId).addClass(classForMiddleCurve);
    }
    $("#" + buttonId).attr('disabled', false);
    $("#" + buttonId).live('click', functionNameToBind);
    $("#" + buttonId + "-LeftCurve").attr('src', leftCurve);
    $("#" + buttonId + "-RightCurve").attr('src', rightCurve);
}

/*Common function to get control values - special characters handled*/
function getControlValueByElementId(controlName, controlType) {    
    switch (controlType) {
        case TEXTBOX_CONTROL:
            if ($("#" + controlName) != undefined && $("#" + controlName) != null && $("#" + controlName).length > 0) {
                var valueText = $("#" + controlName).val();
                if (valueText != "" && valueText != null && valueText != undefined) {
                    //valueOfText = valueOfText.replace(/\+/g, "%2B");
                    valueText = encodeSpecialSymbols(valueText);
                }
                return valueText;
            }
            else {
                return "";
            }
        case RADIOBUTTON_CONTROL:
            var valueOfText = "";
            if ($("#" + controlName) != undefined && $("#" + controlName) != null && $("#" + controlName).length > 0) {
                valueOfText = $("input[type=radio][name='" + controlName + "']:checked").val();
            }
            if (valueOfText == null || valueOfText == undefined) {
                valueOfText = "";
            }
            return valueOfText;
        case CHECKBOX_CONTROL:
            if ($("#" + controlName) != undefined && $("#" + controlName) != null && $("#" + controlName).length > 0) {
                var checkList = [];
                $("input[type=checkbox][name=" + controlName + "]:checked").each(function () {
                    checkList.push($(this).val());
                });
                return checkList;
            }
            else {
                return "";
            }
        case DROPDOWN_LIST:
            if ($("#" + controlName) != undefined && $("#" + controlName) != null && $("#" + controlName).length > 0) {
                if ($("#" + controlName).val() != DROPDOWN_RESET) {
                    return $("#" + controlName).val();
                }
                else {
                    return "";
                }
            }
            else {
                return "";
            }
        case LABEL_CONTROL:
            if ($("#" + controlName) != undefined && $("#" + controlName) != null && $("#" + controlName).length > 0) {
                return $("#" + controlName).text();
            }
            else {
                return "";
            }
    }
    return "";
}

function encodeSpecialSymbols(value) {
    if (value != undefined) {
        value = value.replace(/\\/g, "\\\\");
        value = value.replace(/\"/g, "\\\"");
        return encodeURIComponent(value);
    } else {
        return value;
    }
}

function breakWord(word, breakLength) {
    return word.replace(RegExp("(\\w{" + breakLength + "})(\\w)", "g"), function (all, text, char) {
        return text + "<br>" + char;
    });
}

/* to validate special characters*/
var strRegExpAlphabetNumber = /^[A-Za-z0-9 ]+$/;
function validateSpecialCharacters(value) {
    if (value != null && value != "") {
        return (strRegExpAlphabetNumber.test(value)) ? false : true;
    }
    return false;
}


function initializeFlexBoxForFilterByModules(divId, lstOfItems) {
    lstOfItemsForModule = {};
    lstOfItemsForModule.results = lstOfItems;
    lstOfItemsForModule.total = lstOfItemsForModule.results.length;
    $('#' + divId).flexbox(lstOfItemsForModule, {
        resultTemplate: '{name}',
        width: 150,
        paging: false,
        maxVisibleRows: 10,
        noResultsText: '',
        noResultsClass: '',
        matchClass: '',
        matchAny: true,
        onSelect: function () {
            isFilterValueSelected = true;
            isSearchPageForAssignment = false;
            isSourceFilterValueSelectedForSearch = false;
            startAjaxLoader();
            strListOfSelectedAssignmentItems = "";
            assignmentDataTable.fnDraw();
        }
    });
    $('#' + divId + "_input").watermark("Filter by Modules", { className: 'watermark watermark-text' });
    $('#filterByModule_input').live('blur', function () {
        if ($('#filterByModule_input').val() == "") {
            $('#filterByModule_input').text = "Filter by Modules";
            $('#filterByModule_input').addClass('watermark watermark-text');
            isFilterValueSelected = false;
            isSourceFilterValueSelectedForSearch = false;
        } else {
            $('#filterByModule_input').removeClass('watermark watermark-text');
        }
    });
    $('#filterByModule_input').live('keyup', function () {
        if (isFilterValueSelected) {
            if ($('#filterByModule_input').val() == "") {
                startAjaxLoader();
                strListOfSelectedAssignmentItems = "";
                assignmentDataTable.fnDraw();
            }
        }
    });
}

function setMaxLenthSearchText(controlId) {
    $("#" + controlId).attr("maxlength", MAXLENTH_SEARCHTEXT);
}


function initializeFlexBoxForFilterBySource(divId, lstOfItems) {
    lstOfItemsForSource = {};
    lstOfItemsForSource.results = lstOfItems;
    lstOfItemsForSource.total = lstOfItemsForSource.results.length;
    $('#' + divId).flexbox(lstOfItemsForSource, {
        resultTemplate: '{name}',
        width: 150,
        paging: false,
        maxVisibleRows: 15, 
        noResultsText: '',
        noResultsClass: '',
        matchClass: '',
        matchAny: true,
        onSelect: function () {
            isSourceFilterValueSelected = true;
            isSourceFilterValueSelectedForSearch = false;
            isSearchPageForSkillSet = false;
            startAjaxLoader();
            strListOfSelectedSkillSetItems = "";
            skillSetBuilderDataTable.fnDraw();
        }
    }); 
    $('#' + divId + "_input").watermark("Filter by Source", { className: 'watermark watermark-text' });
    $('#FilterBySource_input').live('blur', function () {
        if ($('#FilterBySource_input').val() == "") {
            $('#FilterBySource_input').text = "Filter by Sources";
            $('#FilterBySource_input').addClass('watermark watermark-text');
            isSourceFilterValueSelected = false;
            isSourceFilterValueSelectedForSearch = false;
            //            startAjaxLoader();
            //            questionBankDataTable.fnDraw();
        } else {
            $('#FilterBySource_input').removeClass('watermark watermark-text');
        }
    });
    $('#FilterBySource_input').live('keyup', function () {
        if (isSourceFilterValueSelected) {
            if ($('#FilterBySource_input').val() == "") {
                startAjaxLoader();
                strListOfSelectedSkillSetItems = "";
                skillSetBuilderDataTable.fnDraw();
            }
        }
    });
}

//Method to autohide/show the scroll for Datatable
function setScrollableTableHeight(tableId) {
    var tableObject = $("#" + tableId[0].id + "_wrapper");
    var contentDiv = tableObject.find("div.dataTables_scrollBody");
    
    var newStyleWithHeight=changeAttributeValue(contentDiv.attr("style"), "height",4,"");
    
    var headerRowObject = $("#" + tableId[0].id + "_wrapper .dataTables_scrollHeadInner >table");
    var headerRowWidth = getAttributeValue(headerRowObject.attr("style"), trimText("width"));

    if (tableId.selector.split("#")[1] != undefined);
    var outerContent = $("#" + tableId.selector.split("#")[1]);

    var bodyObject = $("#" + tableId[0].id + "_wrapper .dataTables_scrollBody >table");
    var newBodyStyle = changeAttributeValue(bodyObject.attr("style"), "width", 0, headerRowWidth);
    

    contentDiv.attr("style", newStyleWithHeight);
    bodyObject.attr("style", newBodyStyle);
    //IF IE value is 18, for FF value is 2px
    var browserDifferenceInWidth=18;
    if (GetBrowserType() == BROWSER_FF) {
        browserDifferenceInWidth = 0;
    }
    var newOuterBodySTyle = changeAttributeValue(outerContent.attr("style"), "width", 0, (parseInt(headerRowWidth.replace("px", "")) + browserDifferenceInWidth) + "px");
    outerContent.attr("style", newOuterBodySTyle);
}
function changeAttributeValue(styleString, stylAttributeToChange, valueToAdd,setNewValue) {
    var finalString = "";
    var styleArray = styleString.split(";");
    for (index = 0; index <= styleArray.length - 1; index++) {
        var currentAttribute = styleArray[index];
        var keyValuePair = currentAttribute.split(":");
        if (trimText(keyValuePair[0].toLowerCase()) == trimText(stylAttributeToChange)) {
            if (setNewValue == "") {
                currentAttribute = stylAttributeToChange + ":" + (parseInt(keyValuePair[1].replace("px", "")) + (valueToAdd)) + "px";
            }
            else {
                currentAttribute = stylAttributeToChange + ":" + setNewValue;
            }
        }
        currentAttribute = currentAttribute + ";";
        finalString = finalString + currentAttribute;
    }
    return finalString;
}
function getAttributeValue(styleString, stylAttributeToChange) {
    var styleArray = styleString.split(";");
 for (index = 0; index <= styleArray.length - 1; index++) {
     var currentAttribute = styleArray[index];
     var keyValuePair = currentAttribute.split(":");
     if (trimText(keyValuePair[0].toLowerCase()) == (stylAttributeToChange)) {
         //       currentAttribute = stylAttributeToChange + ":" + (parseInt(keyValuePair[1].replace("px", "")) + (valueToAdd)) + "px";
         return keyValuePair[1];
     }
 }
    return false;
}

function expandOrCollapseAnAccordion(objDiv) {
    var idOfAccordion = objDiv.id;
    var isSlidedUp = $("#accordion-img-" + idOfAccordion).hasClass("assignment-accordion-close");
    if (isSlidedUp) {
        $("#accordion-img-" + idOfAccordion).removeClass("assignment-accordion-close").addClass("assignment-accordion-open");
        $("#accordion-img-" + idOfAccordion).attr("src", "../../../Content/Images/arrow_black_section_down_normal.png");
        $("#accordion-body-" + idOfAccordion).slideDown();
    } else {
        $("#accordion-img-" + idOfAccordion).removeClass("assignment-accordion-open").addClass("assignment-accordion-close");
        $("#accordion-img-" + idOfAccordion).attr("src", "../../../Content/Images/arrow_black_section_right_normal.png");
        $("#accordion-body-" + idOfAccordion).slideUp();
    }
}

function expandAllAccordions() {
    $('img[id^=accordion-img-]').attr("src", "../../../Content/Images/arrow_black_section_down_normal.png");
    $('img[id^=accordion-img-]').removeClass("assignment-accordion-close").addClass("assignment-accordion-open");
    $('div[id^=accordion-body-]').slideDown();
}

function collapseAllAccordions() {
    $('img[id^=accordion-img-]').attr("src", "../../../Content/Images/arrow_black_section_right_normal.png");
    $('img[id^=accordion-img-]').removeClass("assignment-accordion-open").addClass("assignment-accordion-close");
    $('div[id^=accordion-body-]').slideUp();
}


//function to return broswer type in form of simOfficeConstants strings
function GetBrowserType() {
    var browserName = navigator.appName;

    if (browserName.indexOf("Microsoft") != -1) {
        return BROWSER_IE;
    }
    else if (browserName.indexOf("firefox") != -1) {
        return BROWSER_FF;
    }
    else if (browserName.indexOf("safari") != -1) {
        return BROWSER_SAFARI;
    }
    else if (browserName.indexOf("chrome") != -1) {
        return BROWSER_CHROME;
    }else {
        return "";
    }
}

// to set border for the table div when the table height increases
function setScrollableTableBorder(hasScroll, tableDivId) {    
    if (hasScroll) {
        $('#' + tableDivId + ' .dataTables_scrollBody').css('border-bottom', '1px solid #BBBBBB');
        $('#' + tableDivId + ' .dataTables_scrollBody').css('overflow', 'auto');
        $(".dataTables_scrollHead").css('border-bottom', '1px solid #cccccc');
    }
    else {
        $('#' + tableDivId + ' .dataTables_scrollBody').css('border-bottom', '0px');
        $('#' + tableDivId + ' .dataTables_scrollBody').css('overflow', 'hidden');
        $(".dataTables_scrollHead").css('border-bottom', '0px');
    }
}

function showLogOutConfirmDialog() {
    jConfirm("Are you sure you want to log off and close SimChart for the medical office?", "ALERT", function (isOk) {
        if (isOk) {
            startAjaxLoader();
            window.location.href = "../Account/LogoutSuccess";
            closeAjaxLoader();
            return false;
        }
        return false;
    });
}

function showLogOutConfirmDialogOverPdf() {
    if ($(".forms-pdf-content").length > 0 && $(".forms-pdf-content").is(':visible')) {        
        if (GetBrowserType() == BROWSER_IE) {
            startBackgound_Blur();
            $(".forms-pdf-content").hide();
            jConfirm("Are you sure you want to log off and close SimChart for the medical office?", "ALERT", function (isOk) {               
                if (isOk) {
                    startAjaxLoader();
                    window.location.href = "../Account/LogoutSuccess";
                    closeAjaxLoader();
                    return false;
                }
                else {
                    $(".forms-pdf-content").show();
                    closeBackgound_Blur();
                }
                return false;
            });
            return false;     
        }
    }
    showLogOutConfirmDialog();
    return false;
}
//To Adjust the height of Landing page according to screen resolution dynamically.
function adjustHeight() {
    setAdjustedHeightGridButtonContainer();
    setAdjustedHeightGridContainer();
    setAdjustedHeightInnerGridContainer();
}
function setAdjustedHeightGridButtonContainer() {
    var newHeight = calculateAdjustedHeight();
    $("div .dynamicHeight").css('height', newHeight);
}
//function calculateAdjustedHeight() {
//    var mainHeight = $('#main').height();
//    var setHeight = mainHeight - (mainHeight * 26 / 100);
//    return setHeight;
//}
function calculateAdjustedHeight() {
    var clearSpace = 23;
    var mainHeight = $('#main').height();
    var adminTabHeight = $('#ul_admin').height();
    var authoringTabHeight = $('#ul_authoring').height();
    var setContentHeight = mainHeight - adminTabHeight - authoringTabHeight - questionLandingHeader - clearSpace;
    return setContentHeight;
}
//To Adjust the height of Grid Container according to screen resolution dynamically.
function setAdjustedHeightGridContainer() {
    var newGridHeight = calculateAdjustedHeightGridContainer();
    $("div .grid-height").css('height', newGridHeight);
}
function calculateAdjustedHeightGridContainer() {
    var clearSpace = 5;
    var gridButtonContainerHeight = $("div .dynamicHeight").height();
    var setGridHeight = gridButtonContainerHeight - clearSpace;
    return setGridHeight;
}
//To Adjust the height of Inner rid Container according to screen resolution dynamically.
function setAdjustedHeightInnerGridContainer() {
    var newGridHeight = calculateAdjustedHeightInnerGridContainer();
    $("div .grid-inner-height").css('height', newGridHeight);
}
function calculateAdjustedHeightInnerGridContainer() {
    var clearSpace = 23;
    var gridContainerHeight = $("div .grid-height").height();
    var setGridHeight = gridContainerHeight - clearSpace;
    return setGridHeight;
}
//To Adjust the height of Data Table according to screen resolution dynamically.
function setInnerAdjustedHeight(innerDivId, buttonDivId) {
    var newHeight = calculateInnerAdjustedHeight(innerDivId, buttonDivId);
//    $("div .dataTables_scrollBody").css('height', newHeight);
    return newHeight;
}
function calculateInnerAdjustedHeight(innerDivId, buttonDivId) {
    var clearSpace = 2;
    var gridContainerHeight = $('#' + innerDivId).height();
    var buttonContainerHeight = $('#' + buttonDivId).height();
    var setInnerHeight = gridContainerHeight - buttonContainerHeight - tableInfoHeight - tableHeaderHeight - clearSpace;
    return setInnerHeight;
}

//To Adjust the height of SkillSet Step1 according to screen resolution dynamically.
function setContentHeightForAllBuilders() {
    var newContentHeight = calculateContentHeightForAllBuilders();
    $("div .holder-height").css('height', newContentHeight);
}
function calculateContentHeightForAllBuilders() {
    var clearSpace = 13;
    var mainHeight = $('#main').height();
    var adminTabHeight = $('#ul_admin').height();
    var authoringTabHeight = $('#ul_authoring').height();
    var setContentHeight = mainHeight - adminTabHeight - authoringTabHeight - clearSpace;
    return setContentHeight;
}
//To Adjust the height of SkillSet Step1 Right Holder according to screen resolution dynamically.
function setRightHolderHeight(headerDiv) {
    var newRightHolderHeight = calculateRightHolderHeight(headerDiv);
    $("div .right-content-height").css('height', newRightHolderHeight);
}
function calculateRightHolderHeight() {
    var clearSpace = $("div .clear-height-spacing").height();
    var holderHeight = $("div .holder-height").height();
    var setRightHolderHeight = holderHeight - pageHeaderHeight - (2*clearSpace);
    return setRightHolderHeight;
}
//To Adjust the height of SkillSet Step1 Right Holder according to screen resolution dynamically.
function setRightHolderInnerHeight(){
    var newRightHolderInnerHeight = calculateRightHolderInnerHeight();
    $("div .right-inner-content-height").css('height', newRightHolderInnerHeight);
}
function calculateRightHolderInnerHeight() {
    var clearSpace = $("div .clear-height-spacing").height();
    var rightInnerHeight = $("div .right-content-height").height();
    var setRightHolderInnerHeight = rightInnerHeight - assignmentTitleDivHeight - ButtonDivHeight - (clearSpace);
    return setRightHolderInnerHeight;
}
//To Adjust the height of Preview and Publish screen according to screen resolution dynamically.
function setPreviewPublishHeight() {
    var newPreviewPublishHeight = calculatePreviewPublishHeight();
    $("div .preview-publish-height").css('height', newPreviewPublishHeight);
}
function calculatePreviewPublishHeight() {
    var clearSpace = $("div .clear-height-spacing").height();
    var rightInnerHeight = $("div .right-content-height").height();
    var setPreviewPublishHeight = rightInnerHeight - ButtonDivHeight - clearSpace;
    return setPreviewPublishHeight;
}
//To Adjust the height of Assignment Step4 according to screen resolution dynamically.
function setStep4Height() {
    var newsetStep4Height = calculateStep4Height();
    $("div .step4-height").css('height', newsetStep4Height);
}
function calculateStep4Height() {
    var clearSpace = 14;
    var rightInnerHeight = $("div .right-content-height").height();
    var setStep4Height = rightInnerHeight - assignmentStep4TitleDivHeight - ButtonDivHeight - clearSpace;
    return setStep4Height;
}
//To Adjust the height of Assignment Step4 Patient List according to screen resolution dynamically.
function setStep4PatientListHeight() {
    var newsetStep4Height = calculateStep4PatientListHeight();
    $("div .patient-list-height").css('height', newsetStep4Height);
}
function calculateStep4PatientListHeight() {
    var clearSpace = 20;
    var rightInnerHeight = $("div .step4-height").height();
    var setStep4PatientListHeight = rightInnerHeight - clearSpace;
    return setStep4PatientListHeight;
}
//To Adjust the height of SkillSet Step1 Right Holder according to screen resolution dynamically.
function setQuestionHolderInnerHeight() {
    var newQuestionHolderInnerHeight = calculateQuestionHolderInnerHeight();
    $("div .question-inner-content-height").css('height', newQuestionHolderInnerHeight);
}
function calculateQuestionHolderInnerHeight() {
    var clearSpace = $("div .clear-height-spacing").height();
    var rightInnerHeight = $("div .right-content-height").height();
    var setQuestionHolderInnerHeight = rightInnerHeight - ButtonDivHeight - (1.5 * clearSpace);
    return setQuestionHolderInnerHeight;
} 
//To Adjust the height of Assignment Step3 inner divs without validation according to screen resolution dynamically.
function setAssignmentStep3InnerHeightWithoutValidation() {
    var newAssignmentStep3InnerHeightWithoutValidation = calculateAssignmentStep3InnerHeightWithoutValidation();
    $("div .step3-inner-height-without-validation").css('height', newAssignmentStep3InnerHeightWithoutValidation);
}
function calculateAssignmentStep3InnerHeightWithoutValidation() {
    var rightInnerContentHeight = $("div .right-inner-content-height").height();
    var setAssignmentStep3InnerHeight = rightInnerContentHeight - assignmentStep3InnerHeightWithoutValidation;
    return setAssignmentStep3InnerHeight;
}
//To Adjust the height of Assignment Step3 inner divs with validation according to screen resolution dynamically.
function setAssignmentStep3InnerHeightWithValidation() {
    var newAssignmentStep3InnerHeightWithValidation = calculateAssignmentStep3InnerHeightWithValidation();
    $("div .step3-inner-height-with-validation").css('height', newAssignmentStep3InnerHeightWithValidation);
}
function calculateAssignmentStep3InnerHeightWithValidation() {
    var rightInnerContentHeight = $("div .right-inner-content-height").height();
    var setAssignmentStep3InnerHeight = rightInnerContentHeight - assignmentStep3InnerHeightWithValidation;
    return setAssignmentStep3InnerHeight;
}
//To Adjust the height of Forms Repository according to screen resolution dynamically.
function setFormsRepositoryHeight() {
    var newFormsHeight = calculateFormsRepositoryHeight();
    $("div .forms-container-height").css('height', newFormsHeight);
}
function calculateFormsRepositoryHeight() {
    var clearSpace = 23;
    var mainHeight = $('#main').height();
    var adminTabHeight = $('#authoringTabs').height();
    var setFormsHeight = mainHeight - adminTabHeight - clearSpace;
    return setFormsHeight;
}
//To Adjust the height of Forms Inner Content according to screen resolution dynamically.
function setFormsContentHeight() {
    var newFormsContentHeight = calculateFormsContentHeight();
    $("div .forms-content-height").css('height', newFormsContentHeight);
}
function calculateFormsContentHeight() {
    var formsRepositoryHeight = $("div .forms-container-height").height();
    var formsHeaderHeight = $('#formsRepositoryHeader').height();
    var setFormsContentHeight = formsRepositoryHeight - formsBottomHeight - formsHeaderHeight;
    return setFormsContentHeight;
}
//To Adjust the height of Patient Records Access Request according to screen resolution dynamically.
function setPatientRecordsInnerContentHeight() {
    var newFormsInnerContentHeight = calculatePatientRecordsInnerContentHeight();
    $("div .PatientRecord-inner-content-height").css('height', newFormsInnerContentHeight);
}
function calculatePatientRecordsInnerContentHeight() {
    var formsRepositoryHeight = $("div .forms-content-height").height();
    var setPatientRecordsInnerContentHeight = formsRepositoryHeight - formsInnerHeaderHeight - 15;
    return setPatientRecordsInnerContentHeight;
}
//To Adjust the height of Forms Inner Content according to screen resolution dynamically.
function setFormsInnerContentHeight() {
    var newFormsInnerContentHeight = calculateFormsInnerContentHeight();
    $("div .forms-inner-content-height").css('height', newFormsInnerContentHeight);
}
function calculateFormsInnerContentHeight() {
    var formsRepositoryHeight = $("div .forms-content-height").height();
    var setFormsInnerContentHeight = formsRepositoryHeight - formsInnerHeaderHeight;
    return setFormsInnerContentHeight;
}
//To Adjust the height of Medical Forms Inner Content according to screen resolution dynamically.
function setMedicalFormInnerContentHeight() {
    var newFormsInnerContentHeight = calculateMedicalFormsInnerContentHeight();
    $("div .medical-form-inner-content-height").css('height', newFormsInnerContentHeight);
}
function calculateMedicalFormsInnerContentHeight() {
    var formsRepositoryHeight = $("div .forms-content-height").height();
    var setFormsInnerContentHeight = formsRepositoryHeight - formsInnerHeaderHeight - 5;
    return setFormsInnerContentHeight;
}
//To Adjust the height of Competency Landing page according to screen resolution dynamically.
function setCompetencyContainerAdjustedHeight() {
    var newHeight = calculateCompetencyContainerAdjustedHeight();
    $("div .competency-holder-height").css('height', newHeight);
}
function calculateCompetencyContainerAdjustedHeight() {
    var clearSpace = 20;
    var mainHeight = $('#main').height();
    var adminTabHeight = $('#ul_admin').height();
    var sectionSeperator = $("div .section-seperator-normal").height();
    var setContentHeight = mainHeight - adminTabHeight - sectionSeperator - ButtonDivHeight - clearSpace;
    return setContentHeight;
}
//To Adjust the height of Competency Data Table according to screen resolution dynamically.
function setCompetencyTableHeight() {
    var newCompetencyTableHeight = calculateCompetencyTableHeight();
    $("div .dataTables_scrollBody").css('height', newCompetencyTableHeight);
}
function calculateCompetencyTableHeight() {
    var clearSpace = 20;
    var competencyHolderHeight = $("div .competency-holder-height").height();
    var tableInfoHeight = $("div .dataTables_info").height();
    var tableHeaderHeight = $("div .dataTables_scrollHead").height();
    var competencyTableHeight = competencyHolderHeight - ButtonDivHeight - tableInfoHeight - tableHeaderHeight - clearSpace;
    return competencyTableHeight;
}
//To Adjust the height of Patient Step1 according to screen resolution dynamically.
function setPatientBuilderHeight() {
    var newPatientBuilderHeight = calculatePatientBuilderHeight();
    $("div .patient-holder-height").css('height', newPatientBuilderHeight);
}
function calculatePatientBuilderHeight() {
    var clearSpace = 30;
    var mainHeight = $('#main').height();
    var adminTabHeight = $('#ul_admin').height();
    var authoringTabHeight = $('#ul_authoring').height();
    var setContentHeight = mainHeight - adminTabHeight - authoringTabHeight - patientBuilderHeader - clearSpace;
    return setContentHeight;
}
//To Adjust the height of Patient Step1 content according to screen resolution dynamically.
function setPatientBuilderContentHeight() {
    var newPatientBuilderContentHeight = calculatePatientBuilderContentHeight();
    $("div .patient-content-height").css('height', newPatientBuilderContentHeight);
}
function calculatePatientBuilderContentHeight() {
    var clearSpace = 23;
    var patientHolder = $("div .patient-holder-height").height();
    var patientInnerHeader = $('#patientStep1InnerHeader').height();
    var setPatientBuilderContentHeight = patientHolder - ButtonDivHeight - patientInnerHeader - clearSpace;
    return setPatientBuilderContentHeight;
}
//To Adjust the height of search grids container according to screen resolution dynamically.
function setAdjustedHeightSearchResultsContainer() {
    var newHeight = calculateAdjustedHeightSearchResultsContainer();
    $("div .search-result-height").css('height', newHeight);
}
function calculateAdjustedHeightSearchResultsContainer() {
    var clearSpace = 20;
    var mainHeight = $('#main').height();
    var adminTabHeight = $('#ul_admin').height();
    var authoringTabHeight = $('#ul_authoring').height();
    var setContentHeight = mainHeight - adminTabHeight - authoringTabHeight - questionLandingHeader - clearSpace;
    return setContentHeight;
}
//To Adjust the height of search grid according to screen resolution dynamically.
function setAdjustedHeightSearchGrid() {
    var newHeight = calculateAdjustedHeightSearchGrid();
    //$("div .dataTables_scrollBody").css('height', newHeight);
    return newHeight;
}
function calculateAdjustedHeightSearchGrid() {
    var clearSpace = 84;
    var searchResult = $("div .search-result-height").height();
    var setSearchGridHeight = searchResult - clearSpace;
    return setSearchGridHeight;
}
//To Adjust the height of Patient Search grid container according to screen resolution dynamically.
function setAdjustedHeightPatientSearchResultsContainer() {
    var newHeight = calculateAdjustedHeightPatientSearchResultsContainer();
    $("div .patient-search-result-height").css('height', newHeight);
}
function calculateAdjustedHeightPatientSearchResultsContainer() {
    var clearSpace = 20;
    var mainHeight = $('#main').height();
    var adminTabHeight = $('#ul_admin').height();
    var authoringTabHeight = $('#ul_authoring').height();
    var setContentHeight = mainHeight - adminTabHeight - authoringTabHeight - questionLandingHeader - clearSpace;
    return setContentHeight;
}
//To Adjust the height of patient search grid according to screen resolution dynamically.
function setAdjustedHeightPatientSearchGrid() {
    var newHeight = calculateAdjustedHeightPatientSearchGrid();
    //$("div .dataTables_scrollBody").css('height', newHeight);
    return newHeight;
}
function calculateAdjustedHeightPatientSearchGrid() {
    var clearSpace = 84;
    var searchResult = $("div .patient-search-result-height").height();
    var setSearchGridHeight = searchResult - clearSpace;
    return setSearchGridHeight;
} 
