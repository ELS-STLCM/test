/**
 * 
 */

function showControl(ctrlId) {
    $("#"+ctrlObj).show();
}
function hideControl(ctrlId) {
	$("#"+ctrlObj).hide();
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
    if ($("#" + divName+"_print") != undefined) {
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