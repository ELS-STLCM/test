var ENTER_FOLDER_NAME = "Please enter folder name";
var ENTER_FOLDER_RENAME = "Please rename folder";
var DELETE_FOLDER_MESSAGE = "Are you sure you want to delete this folder?";
var ALERT_TITLE = "Message";
var DELETE = "Delete";
var RENAME = "Rename";
var rearrangedSubFolderList = new Array();
var folderCount;

function loadFolderTools(folderPanel) {
//    var subset = $("#" + folderPanel);
    $("#deleteFolder_" + folderPanel).attr('src', '../Content/Images/FolderIcons/icon_folder-delete_disable.png');
    $("#deleteFolder_" + folderPanel).attr('disabled', true);
    $("#deleteFolder_" + folderPanel).unbind('click');
    $("#renameFolder_" + folderPanel).attr('src', '../Content/Images/FolderIcons/icon_folder-rename_disable.png');
    $("#renameFolder_" + folderPanel).attr('disabled', true);
    $("#renameFolder_" + folderPanel).unbind('click');
}
function folderHasContent(folder,folderType) {
    var hasContent = false;
    if (folderType == "questionBankTab") {
    if (folder != null && (folder.SubFolders != null || folder.QuestionItems != null)) {
        hasContent = true;
        }
    }
    else{
    if (folder != null && (folder.SubFolders != null || folder.Patients != null)) {
        hasContent = true;
        }
    }
    return hasContent;
}

function deHighlightTheDivs() {
    $("#folderListContent div").css("background", "none");
}

function hightLightTheDivSelected(folderId) {
    deHighlightTheDivs();
    $("#folderDiv_" + folderId).css("background", "#EEEEEE");
}

function triggerFolderSelection(hasContent, folderId, folderUrl, folderName, folderPanel) {
    hightLightTheDivSelected(folderId);
//    setDivData(getDivObject(folderPanel), "currentFolder", folderUrl);
//    setDivData(getDivObject(folderPanel), "currentFolderIdentifier", folderId);
    setDivData(getDivObject(folderPanel), "selectedFolderName", folderName);
    setDivData(getDivObject(folderPanel), "selectedFolderUrl", folderUrl);
    setDivData(getDivObject(folderPanel), "selectedFolderId", folderId);
//    var subset = $("#" + folderPanel);
    $("#renameFolder_" + folderPanel).attr('src', '../Content/Images/FolderIcons/icon_folder-rename_normal.png');
    $("#renameFolder_" + folderPanel).attr('disabled', false);
    $("#renameFolder_" + folderPanel).bind('click', function () {
        renameSelectedFolder(getDivData(folderPanel, "selectedFolderId"), getDivData(folderPanel, "selectedFolderUrl"), getDivData(folderPanel, "selectedFolderName"), folderPanel);
    });
    
    if (hasContent) {
        $("#deleteFolder_" + folderPanel).attr('src', '../Content/Images/FolderIcons/icon_folder-delete_disable.png');
        $("#deleteFolder_" + folderPanel).attr('disabled', true);
        $("#deleteFolder_" + folderPanel).unbind('click');
    }
    else {        
        $("#deleteFolder_" + folderPanel).attr('src', '../Content/Images/FolderIcons/icon_folder-delete_normal.png');
        $("#deleteFolder_" + folderPanel).attr('disabled', false);
        $("#deleteFolder_" + folderPanel).bind('click', function () {
            deleteFolder(getDivData(folderPanel, "selectedFolderId"), getDivData(folderPanel, "selectedFolderUrl"), folderPanel);
        });
    }
    return false;
}


function activateFolderTools(folderPanel, folderCount) {
    if (folderCount > 1) {
        $("#rearrangeFolders_" + folderPanel).attr('src', '../Content/Images/FolderIcons/icon_folder-swap_normal.png');
        $("#rearrangeFolders_" + folderPanel).attr('disabled', false);
    }
    else {
        $("#rearrangeFolders_" + folderPanel).attr('src', '../Content/Images/FolderIcons/icon_folder-swap_disable.png');
        $("#rearrangeFolders_" + folderPanel).attr('disabled', true);
    }
}
function getFolderType(currentTabSelected){
    switch(currentTabSelected){
        case "questionBankTab":
            return 1;
        case "practiceTab":
            return 2;
        case "assignmentBuilderTab":
            return 3;
        case "skillSetBuilderTab":
            return 4;   
        default: return 0;
    }
}

function getCurrentTab(folderType) {
    switch (folderType) {
        case 1:
            return "questionBankTab";
        case 2:
            return "practiceTab";
        case 3:
            return "assignmentBuilderTab";
        case 4:
            return "skillSetBuilderTab";
        default: return "";
    }
}

//function to add a new folder into db
function createFolder(currentTabSelected) {
    jPrompt('Name', '', 'Enter folder name', function (folderName) {
        if (folderName != undefined && folderName != undefined) {
            if ($.trim(folderName) != "" && $.trim(folderName) != undefined) {
                startAjaxLoader();
                var folderSequenceNumber = parseInt(getDivData(currentTabSelected, "MaxSequenceNumber")) + 1;
                    var folderJson = {
                        "Name": encodeSpecialSymbols($.trim(folderName)),
                        "SequenceNumber": folderSequenceNumber
                    };
                    var urlForFolders = '../QuestionBank/CreateFolder?folderType=' + getFolderType(currentTabSelected) + '&folderUrl=' + getDivData(currentTabSelected, "currentFolder") + '&folderGuid=' + getDivData(currentTabSelected, "currentFolderIdentifier") + '&parentFolderName=' +  getDivData(currentTabSelected, "currentFolder") + '&currentTab=' + currentTabSelected;
                    $.ajax({
                        url: urlForFolders,
                        type: "POST",
                        data: JSON.stringify(folderJson),
                        dataType: 'json',
                        asynch: false,
                        error: function (result) {
                            if (result != null) {
                                alert('Error');
                            }
                        },
                        success: function (result) {
                            if (result.messageToReturn == "") {
                                setDivData(getDivObject(currentTabSelected), "NewAddedFolder", result.newFolderId);
                                renderFolders(result.folderGuid, currentTabSelected, result.folderUrl);
                            }
                            else {
                                jAlert(result.messageToReturn, ALERT_TITLE, function () {
                                    addFolder();
                                });
                            }
                            closeAjaxLoader();
                        }
                    });
                    return false;
            }
            else {
                jAlert(ENTER_FOLDER_NAME, ALERT_TITLE, function () {
                    addFolder();
                });
            }
        }
    });
}

//function to add a new folder into db
function renameSelectedFolder(selectedFolderIdentifier, parentFolderIdentifier, oldFolderName, currentTabSelected) {
    jPrompt('Name', $.trim(oldFolderName), 'Rename folder', function (folderName) {
        if (folderName != undefined && folderName != undefined) {
            if ($.trim(folderName) != "" && $.trim(folderName) != undefined) {
                var folderJson = {
                    "Name": encodeSpecialSymbols($.trim(folderName))
                };
                startAjaxLoader();
                $.ajax({
                    url: '../QuestionBank/RenameFolder?folderType=' + getFolderType(currentTabSelected) + '&folderUrl=' + getDivData(currentTabSelected, "selectedFolderUrl") + '&folderGuid=' + getDivData(currentTabSelected, "selectedFolderId") + '&parentFolderName=' + getDivData(currentTabSelected, "currentFolderIdentifier") + '&currentTab=' + currentTabSelected + '&folderUrlOfParent=' + getDivData(currentTabSelected, "currentFolder"),
                    type: "POST",
                    data: JSON.stringify(folderJson),
                    asynch: false,
                    error: function (result) {
                        if (result != null) {
                            alert('Error');
                        }
                    },
                    success: function (result) {
                        if (result.messageToReturn == "") {
                            result.folderUrl = (result.folderUrl == undefined) ? "" : result.folderUrl;
                            result.folderGuid = (result.folderGuid == undefined) ? "" : result.folderGuid;
                            renderFolders(getDivData(currentTabSelected, "currentFolderIdentifier"), currentTabSelected, getDivData(currentTabSelected, "currentFolder"));
                        }
                        else {
                            jAlert(result.messageToReturn, ALERT_TITLE, function () {
                                renameFolder(selectedFolderIdentifier, parentFolderIdentifier, oldFolderName, currentTabSelected);
                            });
                        }
                        closeAjaxLoader();
                    }
                });
                return false;
            }
            else {
                jAlert(ENTER_FOLDER_RENAME, ALERT_TITLE, function () {
                    renameFolder(selectedFolderIdentifier, parentFolderIdentifier, oldFolderName, currentTabSelected);
                });
            }
        }
    });
}
//function to add a new folder into db
function deleteSelectedFolder(selectedFolderIdentifier, parentFolderIdentifier, currentTabSelected) {
    jConfirm(DELETE_FOLDER_MESSAGE, 'Delete', function (isCancel) {
        if (isCancel) {
            startAjaxLoader();
            $.ajax({
                url: '../QuestionBank/DeleteFolder?folderType=' + getFolderType(currentTabSelected) + '&folderUrl=' + getDivData(currentTabSelected, "selectedFolderUrl") + '&folderGuid=' + getDivData(currentTabSelected, "selectedFolderId"),
                type: "POST",
                asynch: false,
                error: function (result) {
                    if (result != null) {
                        alert('Error');
                    }
                },
                success: function (result) {
                    result.folderUrl = (result.folderUrl == undefined) ? "" : result.folderUrl;
                    result.folderGuid = (result.folderGuid == undefined) ? "" : result.folderGuid;
                    renderFolders(getDivData(currentTabSelected, "currentFolderIdentifier"), currentTabSelected, getDivData(currentTabSelected, "currentFolder"));
                    closeAjaxLoader();
                }
            });
            return false;
        }
    });
}

//function to Swap/rearrange folders
function rearrangeFolders(currentTabSelected) {
    var folderUrl = getDivData(currentTabSelected, "currentFolder");
    //restting selected folder id if any from div data
    setDivData(getDivObject(currentTabSelected), "selectedFolderId", "");
    removeSwapDivFromOtherTabs(currentTabSelected);
    var swapPopUp = "../QuestionBank/GetSubFoldersForSwap?parentFolderIdentifier=" + getDivData(currentTabSelected, "currentFolderIdentifier") + "&folderType=" + getFolderType(currentTabSelected) + "&folderUrl=" + (isNullOrEmpty(folderUrl) ? "" : folderUrl);

    $("#swapFolder").load(swapPopUp, function () {
        initializeRearrangeDialog();

        //define all dialog click events
        $("#Rearrange_BtnCancel").live('click', function () {
            var status = "Are you sure you want to cancel? Your changes will not be saved.";
            jConfirm(status, 'Cancel', function (isCancel) {
                if (isCancel) {
                    closeDialog("rearrangeFoldersDialog");
                }
            });
            $("#Rearrange_BtnOk").die();
            $("#Rearrange_BtnCancel").die();
        });

        $("#Rearrange_BtnOk").live('click', function () {
            getRearrangedFoldersList(currentTabSelected);
        });
    });
}

function removeSwapDivFromOtherTabs(currentTab) {
    switch (currentTab) {
        case "questionBankTab":
            // remove breadCrumb divs from all other tabs to avoid dupilcate data
            RemoveOtherTabSwapDivs("authoringPractice");
            RemoveOtherTabSwapDivs("authoringSkillSetBuilder");
            RemoveOtherTabSwapDivs("authoringAssignmentBuilder");
            break;
        case "practiceTab":
            // remove breadCrumb divs from all other tabs to avoid dupilcate data
            RemoveOtherTabSwapDivs("authoringQuestionBank");
            RemoveOtherTabSwapDivs("authoringSkillSetBuilder");
            RemoveOtherTabSwapDivs("authoringAssignmentBuilder");
            break;
        case "assignmentBuilderTab":
            // remove breadCrumb divs from all other tabs to avoid dupilcate data
            RemoveOtherTabSwapDivs("authoringQuestionBank");
            RemoveOtherTabSwapDivs("authoringSkillSetBuilder");
            RemoveOtherTabSwapDivs("authoringPractice");
            break;
        case "skillSetBuilderTab":
            // remove breadCrumb divs from all other tabs to avoid dupilcate data
            RemoveOtherTabSwapDivs("authoringQuestionBank");
            RemoveOtherTabSwapDivs("authoringPractice");
            RemoveOtherTabSwapDivs("authoringAssignmentBuilder");
            break;

        //add other tab cases 
    }
}

function RemoveOtherTabSwapDivs(tabName) {
    $("#" + tabName + " #swapFolder").remove();
}

function getRearrangedFoldersList(currentTabSelected) {
    $("#Rearrange_BtnOk").die();
    $("#Rearrange_BtnCancel").die();
    var sequenceNumber = folderCount;
    $("#rearrangeFoldersDialog").contents().find('div#sortable').each(function () {
        $(this).find('div').each(function () {

            var current = ($(this)[0].id.split('+')[1]).toString();
            var folder = {
                "UniqueIdentifier": current,
                "SequenceNumber": sequenceNumber
            };
            rearrangedSubFolderList.push(folder);
            sequenceNumber = sequenceNumber - 1;
        });
    });

    updateSubFolderSequenceNumber(currentTabSelected);
}


function successFunctionForSubFolderSwap(result) {
    rearrangedSubFolderList = new Array();
    closeDialog("rearrangeFoldersDialog");
    folderCount = 0;
    if (result != null && result.folderType != null) {
        var currentTabSelected = getCurrentTab(result.folderType);
        var folderUrl = getDivData(currentTabSelected, "currentFolder");
        renderFolders(getDivData(currentTabSelected, "currentFolderIdentifier"), currentTabSelected, isNullOrEmpty(folderUrl) ? "" : folderUrl);
    }

}

function updateSubFolderSequenceNumber(currentTabSelected) {
    var folderUrl = getDivData(currentTabSelected, "currentFolder");
    var reArrangeSubFoldersUrl = "../QuestionBank/RearrangeSubFolderSequenceNumber?parentFolderIdentifier=" + getDivData(currentTabSelected, "currentFolderIdentifier") + "&folderType=" + getFolderType(currentTabSelected) + "&folderUrl=" + (isNullOrEmpty(folderUrl) ? "" : folderUrl);
    doAjaxCall("POST", rearrangedSubFolderList, reArrangeSubFoldersUrl, successFunctionForSubFolderSwap);
}

function initializeRearrangeDialog()
{
    $("#rearrangeFoldersDialog").dialog({ height: 400,
        width: 720,
        modal: true,
        position: 'center',
        resizable: false,
        autoOpen: true,
        closeOnEscape: false,
        title: 'Rearrange Folders',
        open: function (event, ui) {
            //$("#filterDateOfBirth").datepicker("hide");
            applyBlueClassForDialogHeader();
            //forms.PatientSearch.DisableSelectBtn("PatientSearch_BtnSelect");
            //                    disableAButtonForForms("PatientSearch_BtnSelect");
            //removeCloseIconForDialogHeader(this);
            applyBlueBorderForDialogHeader();
        },
        close: function (event, ui) {
            $("#Rearrange_BtnCancel").die();
            $("#Rearrange_BtnOk").die();
            $(this).dialog('destroy').remove();
        },
        overlay: { opacity: 0.5, background: 'black' }
    });    
}

function getDivObject(divIdentifier) {
    return $("#"+ divIdentifier);
}
function getSelectedFolderUrl(parentIdentifier, selectedFolderName) {
    return parentIdentifier + "/SubFolders/" + selectedFolderName;
}
function getDivData(divIdentifier, dataParameter) {
    return ((getDivObject(divIdentifier)).data(dataParameter) != undefined) ? ((getDivObject(divIdentifier)).data(dataParameter)) : "";
}
function setDivData(divObject, dataName, data) {
    if (divObject != undefined && divObject != null) {
        divObject.data(dataName, data);
    }
}

function getDataGridPopulated(folderType) {
    switch (folderType) {
    case "questionBankTab":
        $('#questionBank_grid_display #questionBankGrid').dataTable().fnDraw();
        break;
    case "practiceTab":
        $('#patientBuilder_grid_display #patientBuilderGrid').dataTable().fnDraw();
        break;
    case "assignmentBuilderTab":
        $('#Assignment_grid_display #AssignmentGrid').dataTable().fnDraw();
        break;
    case "skillSetBuilderTab":
        $('#skillSetBuilder_grid_display #SkillSetBuilderGrid').dataTable().fnDraw();
        break;
    default :
        return 0;
    }
}

