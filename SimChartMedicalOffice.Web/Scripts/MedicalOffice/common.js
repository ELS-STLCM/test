/**
* Reusable functions : Functions from SimChart that can be used in SimOffice
*/

/*
* Action       : common method to do ajax Calls
* Input params : typeOfCall - either GET/POST
data  - object/list.
urlForAjax - URL of call
successFunction - function name to be called after successful ajax
*/
function doAjaxCall(typeOfCall, data, urlForAjax, successFunction) {
    $.ajax({
        type: typeOfCall,
        dataType: 'json',
        data: JSON.stringify(data),
        url: urlForAjax,
        contentType: "application/json; charset=utf-8",
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
            window.location.href = "TO-DO";
        }
    }
    jAlert("Error Message");
}

/*
* Action       : To check if value is null/empty/undefined
* Input params : value - value to check
* Return params: true if value is null/empty/undefined
* 				  false otherwise
*/
function isNullOrEmpty(value) {
    if (value == "" || value == null || value == undefined) {
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
* 				  classToAdd - class to be added
* 				  
*/
function addClassToObj(objId, classToAdd) {
    $("#" + objId).addClass(classToAdd);
}

/*
* Action       : To remove class to a dom
* Input params : objId - Object id of dom.
* 				  classToRemove - class to be removed
* 				  
*/
function removeClassToObj(objId, classToRemove) {
    $("#" + objId).removeClass(classToRemove);
}

/*
* Action       : To check if dom has specific class
* Input params : objId - Object id of dom.
* 				  classToRemove - Class to check
* Return params: true if has the class else false.
*/
function checkIfDomHasClass(objId, classToCheck) {
    $("#" + objId).hasClass(classToRemove);
}


/*
* Action       : To display a string with ellipsis(...)
* Input params : actualString - string to check
* 				  maxLengthToDisplay - MaxLength for ellipsis
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
* Action       : To get system time stamp
*/
function getSystemTimeStamp() {
    return new Date(), JS_FORMAT;
}
function getSystemTimeStampString() {
    return dateFormat(new Date(), JS_FORMAT);
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

/******************************Numeric Validations*******************************************/
/**
* Numeric Validations
*/
//regular expression for three digit number
var threeDigitNumber = /^\+?\d*$/;
var threeDigitPositiveDecimalNumber = /^\+?\d*?([\.]\d{1,2})?$/;
var threeDigitDecimal = /^\d*?([\.])\d{2}$/;
var decimaldigit = /^\+?\d*?([\.]\d*)?$/;
var temperatureFahrenheit = /^\d*?([\.]\d{1})?$/;
var postiveNegativeInteger = /^(\+|-)?\d+$/;

//Function to validate the Integers
function IsValidNumeric(value) {
    if (value != null && value != "") {
        if (value.search(threeDigitNumber) == -1) {
            return false;
        }
        else {
            return true;
        }
    }
    return true;
}
function isValidPositiveNegativeInteger(value) {
    if (value != null && value != "") {
        if (value.search(postiveNegativeInteger) == -1) {
            return false;
        }
        else {
            return true;
        }
    }
    return true;
}

//Function to validate Decimal value
function IsValidDecimal(value) {
    if (value.search(threeDigitDecimal) == -1) {
        return false;
    }
    else {
        return true;
    }
}


//Function to validate Positive Decimal value
function IsValidPositiveNumeric(value) {
    if (value != null && value != "") {
        if (value.search(threeDigitPositiveDecimalNumber) == -1) {
            return false;
        }
        else {
            return true;
        }
    }
    return true;
}

//Function to validate numeric numbers on key press.
function isNumeric(event) {
    if (event.keyCode >= 48 && event.keyCode <= 57) {
        return true;
    }
    else {
        event.keyCode = 0;
        return false;
    }
}


/******************************************jQuery Helper*******************************************/
/**
* 
*/

function showControl(ctrlId) {
    $("#" + ctrlObj).show();
}
function hideControl(ctrlId) {
    $("#" + ctrlObj).hide();
}

function disableTextBox(txtboxObject) {
    $(txtboxObject).attr("readOnly", true);
}

function enableTextBox(txtboxObject) {
    $(txtboxObject).attr("readOnly", false);
    $(txtboxObject).removeAttr("readOnly");
}

function disableTextArea(txtAreaObject) {
    $(txtAreaObject).attr("readOnly", true);
}


function enableTextArea(txtAreaObject) {
    $(txtAreaObject).attr("readOnly", false);
}

function disableDropdown(dropdownObject) {
    $(dropdownObject).attr("disabled", true);
}

function enableDropdown(dropdownObject) {
    $(dropdownObject).attr("disabled", false);
}


function clearAndDisableAllInputControls(panelElementId) {
    $("#" + panelElementId + " :input").val("");
    $("#" + panelElementId + " :input").attr('disabled', true);
    $("#" + panelElementId + " :input[type='text']").removeAttr("disabled");
    $("#" + panelElementId + " :input[type='text']").attr('readonly', true);
}
function enableAllInputControls(panelElementId) {
    $("#" + panelElementId + " :input").attr('disabled', false);
    $("#" + panelElementId + " :input[type='text']").removeAttr("disabled");
    $("#" + panelElementId + " :input[type='text']").attr('readonly', false);
}
function disabledAllInputControls(panelElementId) {
    $("#" + panelElementId + " :input").attr('disabled', true);
    $("#" + panelElementId + " :input[type='text']").removeAttr("disabled");
    $("#" + panelElementId + " :input[type='text']").attr('readonly', true);
}

function setBackgroundWhite(obj) {
    $(obj).css("background-color", "#FFFFFF");
}

function setBackgroundGrey(obj) {
    $(obj).css("background-color", "#e2e2e2");
}

function enableTimeEntry(timeEntryObj) {
    $(timeEntryObj).timeEntry({ show24Hours: true, spinnerImage: '' });
}

function disableTimeEntry(timeEntryObj) {
    $(timeEntryObj).timeEntry('destroy');
}



function getControlValueByElementId(controlName, controlType) {
    switch (controlType) {
        case TEXTBOX_CONTROL:
            if ($("#" + controlName) != undefined && $("#" + controlName) != null) {
                var valueOfText = $("#" + controlName).val();
                if (valueOfText != "" && valueOfText != null && valueOfText != undefined) {
                    //valueOfText = valueOfText.replace(/\+/g, "%2B");
                    valueOfText = encodeSpecialSymbols(valueOfText);
                }
                return valueOfText;
            }
            else {
                return "";
            }
        case RADIOBUTTON_CONTROL:
            var valueOfText = "";
            if ($("#" + controlName) != undefined && $("#" + controlName) != null) {
                valueOfText = $("input[type=radio][name='" + controlName + "']:checked").val();
            }
            if (valueOfText == null || valueOfText == undefined) {
                valueOfText = "";
            }
            return valueOfText;
            break;
        case CHECKBOX_CONTROL:
            if ($("#" + controlName) != undefined && $("#" + controlName) != null) {
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
            if ($("#" + controlName) != undefined && $("#" + controlName) != null) {
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
            if ($("#" + controlName) != undefined && $("#" + controlName) != null) {
                return $("#" + controlName).text();
            }
            else {
                return "";
            }
    }
}

var TEXTAREA_CONTROL = "TextArea";

//This function is to retrieve the values of the text controls in body doc screens.
function getValueFromTextControls(controlName, controlType) {
    switch (controlType) {
        case TEXTBOX_CONTROL:
            if ($("#" + controlName) != undefined && $("#" + controlName) != null) {
                var valueOfText = $("input[name^='" + controlName + "']").val();
                if (valueOfText != "" && valueOfText != null && valueOfText != undefined) {
                    valueOfText = encodeSpecialSymbols(valueOfText);
                }
                return valueOfText;
            }
            else {
                return "";
            }
        case TEXTAREA_CONTROL:
            if ($("#" + controlName) != undefined && $("#" + controlName) != null) {
                var valueOfText = $("textarea[name='" + controlName + "']").val();
                if (valueOfText != "" && valueOfText != null && valueOfText != undefined) {
                    valueOfText = encodeSpecialSymbols(valueOfText);
                }
                return valueOfText;
            }
            else {
                return "";
            }
    }
}



function assignValuesToinputControls(controlName, value) {
    if ($("#" + controlName) != undefined) {
        if (!IsNullOrEmpty(value)) {
            $("#" + controlName).val(decodeSpecialSymbols(value));
        }
        else {
            $("#" + controlName).val("");
        }
    }
}
function assignValueToPrintDiv(divName, value, defaultValue) {
    if ($("#" + divName + "_print") != undefined) {
        if (!IsNullOrEmpty(value)) {
            $("#" + divName + "_print").html(value);
        }
        else {
            $("#" + divName + "_print").html(defaultValue);
        }
    }
}

function assignValueToCheckBoxList(checkboxName, otherTextBox, value) {
    var isOtherSelected = true;
    var chkObject = ("input[type=checkbox][name=" + checkboxName + "]");
    if ($(chkObject) != undefined && !isNullOrEmpty(value)) {
        $(chkObject + ":unchecked").each(function () {
            if ($(this).val() == value) {
                $(this).attr('checked', true);
                this.setAttribute('checked', 'checked');
                isOtherSelected = false;
            }
            if ($(this).val() == "Other" && isOtherSelected) {
                $(this).attr('checked', true);
                this.setAttribute('checked', 'checked');
                if (otherTextBox != "" && $("#" + otherTextBox + "") != undefined) {
                    if (value != "/$" + $(this).val() + "/$") {
                        $("#" + otherTextBox + "").val(value);
                    }
                    enableTextBox("#" + otherTextBox);
                }
            }
        });
    }
}

function assignValueToRadio(radioName, otherTextBox, value) {
    var isOtherSelected = true;
    var radioObject = ("input[type=radio][name=" + radioName + "]");
    if ($(radioObject) != undefined && !IsNullOrEmpty(value)) {
        $(radioObject + ":unchecked").each(function () {
            if ($(this).val() == value) {
                $(this).attr('checked', true);
                this.setAttribute('checked', 'checked');
                isOtherSelected = false;
            }
            if ($(this).val() == "Other" && isOtherSelected) {
                $(this).attr('checked', true);
                this.setAttribute('checked', 'checked');
                if (otherTextBox != "" && $("#" + otherTextBox + "") != undefined) {
                    if (value != "/$" + $(this).val() + "/$") {
                        $("#" + otherTextBox + "").val(value);
                    }
                    enableTextBox("#" + otherTextBox);
                }
            }
        });
    }
}


/*********************************Special Character Handling***************************************/
/**
* Special Character Handling
*/
function enquote(jsonString) {
    return jsonString.replace(/\n/g, "\\n").replace(/\r/g, "\\r").replace(/\t/g, "\\t");
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
function decodeSpecialSymbols(value) {
    if (isNaN(value)) {
        if (value.indexOf(" ") == -1) {
            value = decodeURIComponent(value);
            value = value.replace(/\\\\/g, "\\");
            value = value.replace(/\\\"/g, "\"");
            return value;
        }
        else {
            value = value.replace(/%2B/g, "+");
            value = value.replace(/%25/g, "%");
            return value;
        }
    }
    else {
        return value;
    }
}

function decodeSpecialHTMLSymbols(value) {
    //debugger;
    value = decodeSpecialSymbols(value);
    return value.replace(/\n/g, "<br/>");
}

function setControlHTMLValueByElementId(controlName, value) {
    var controlInstance = $(controlName);
    controlInstance.html(decodeSpecialSymbols(value));
}

function setControlStringValueByElementId(controlName, value) {
    var controlInstance = $(controlName);
    controlInstance.text(value);
}

function replaceDoubleQuotes(stringValue) {
    return stringValue.replace(/"/g, '\\"');
}
function replaceDoubleQuotesInBackPack(stringValue) {
    stringValue = stringValue.replace(/\\/g, "\\\\");
    stringValue = stringValue.replace(/\“/g, "\"");
    stringValue = stringValue.replace(/\”/g, "\"");
    stringValue = stringValue.replace(/\"/g, "\\\\&quot;");
    return encodeURIComponent(stringValue);
}
function restoreDoubleQuotes(stringValue) {
    return stringValue.replace(/\\"/g, '\"');
}

function encodeHTML(stringValue) {
    return stringValue.split('&').join('&amp;').split('<').join('&lt;').split('"').join('&quot;').split("'").join('&#39;');
}

function replaceHTMLBreak(stringValue) {
    return stringValue.replace(/\n/g, "<br/>");
}

function replaceSingleAndDoubleQuotes(stringValue) {

    return stringValue.replace(/"/g, '\"')
                      .replace(/\'/g, "\'");
}
function replaceSpecialCharacters(stringValue) {

    stringValue = replaceSingleAndDoubleQuotes(stringValue);
    return stringValue.replace(/\n/g, "\\n");
}
