var isCompetencySave = false;
var isCompetencyDelete = false;
var strListOfSelectedCompetencyItems = "";
var competency = {
    /*Common functions used in competency module*/
    commonFunctions: {
        /*Ajax call to save competency */
        searchCompetency: function () {
            var searchText = $("#competencySearchByText").val();
            searchText = trimText(searchText);
            var isSearchValid = true;
            if (!isNullOrEmpty(searchText) && !(searchText.length >= 2)) {
                isSearchValid = false;
                jAlert(SEARCH_VALIDATION, ALERT_TITLE, function () {
                });
            }
            if (isSearchValid) {
                startAjaxLoader();
                competencyDataTable.fnDraw();
                closeAjaxLoader();
            }
        },
        saveCompetency: function (competencyUrlReference, isEditMode) {
            if (this.validateCompetencyFields()) {
                var competencyJson = this.formJsonForCompetency();
                var urlToSaveCompetency = "../Competency/SaveCompetency?competencyUrlReference=" + competencyUrlReference + "&isEditMode=" + isEditMode;
                if (competencyJson != null) {
                    startAjaxLoader();
                    doAjaxCall("POST", competencyJson, urlToSaveCompetency, this.successFunctionForsaveCompetency);

                }
            }
        },
        /*sucess function for competency after saving*/
        successFunctionForsaveCompetency: function () {
            jAlert(SAVED_MESSAGE, "Alert", function (isOk) {
                if (isOk) {
                    isCompetencySave = true;
                    closeDialog("competency_content");
                    $("#showCreateNewCompetency").empty();
                    $('#competency_content').empty();
                    startAjaxLoader();
                    competencyDataTable.fnDraw();
                    closeAjaxLoader();
                    isCompetencySave = false;
                    //                    $("#smoAdmin").load("../Competency/LoadMaster?iReferenceOfMasterToLoad=1", function () {
                    //                        closeAjaxLoader();
                    //                    });
                }
            });
        },
        /*cancel function for competency*/
        cancelCompetency: function () {
            jConfirm(CANCEL_MESSAGE, "Cancel", function (isOk) {
                if (isOk) {
                    closeDialog("competency_content");
                    $("#showCreateNewCompetency").empty();
                    $('#competency_content').empty();
                    $("#smoAdmin").load("../Competency/LoadMaster?iReferenceOfMasterToLoad=1");
                }
            });
        },

        /*Json formation for Competency*/
        formJsonForCompetency: function () {
            var competencyObj = {
                "Name": $("#Name").val(),
                "Focus": $("#CompetencyFocus").val(),
                "Category": $("#CompetencyCategory_input").val(),
                "IsActive": true,
                "Sources": this.formSourceList(),
                "Notes": $("#Notes").val(),
                "CAAHEP": $("#CAAHEP").val(),
                "ABHES": $("#ABHES").val(),
                "Url": "",
                "UniqueIdentifier": ""
            };
            return competencyObj;
        },
        /*List formation for Competency Source*/
        formSourceList: function () {
            var sourceListJson = [];
            $("input[type=checkbox][name^='CompetencySourceCheckBox']:checked").each(function () {
                var competencySource = $(this)[0].value;
                var numberValue = "";
                if ((competencySource == CAAHEP_COMPETENCY_SOURCE)) {
                    numberValue = $("#CAAHEP").val();
                }
                if ((competencySource == "ABHES")) {
                    numberValue = $("#ABHES").val();
                }
                var competencySourcesJson = {
                    "Name": competencySource,
                    "Number": numberValue,
                    "IsActive": false
                };
                sourceListJson.push(competencySourcesJson);
            });
            return sourceListJson;
        },
        /*validation for competency*/
        validateCompetencyFields: function () {
            var isInputRequired = false;
            var isCaahepNumberRequired = false;
            var isAbhesNumberRequired = false;
//            var isNumberRequired = false;
            var isValid = false;
            var isSourceRequired = false;
            var errorMessage = "<UL>";
            if (isNullOrEmpty($("#CompetencyCategory_input").val()) || validateSpecialCharacters($("#CompetencyCategory_input").val()) || isNullOrEmpty($("#Name").val()) || hasDropDownValue($("#CompetencyFocus").val())) {
                isInputRequired = true;
                isValid = true;
            }
            if (isInputRequired) {
                errorMessage += "<LI>" + INPUT_REQUIRED + "</LI>";
            }
            //error message for special character validation on competency name
            //            if (validateSpecialCharacters($("#CompetencyCategory_input").val())) {
            //                errorMessage += "<LI>" + "Category should not contain special characters" + "</LI>";
            //                isValid = true;
            //            }
            if (isNullOrEmpty($("#CAAHEP").val()) && isNullOrEmpty($("#ABHES").val())) {
                $("input[type=checkbox][name^='CompetencySourceCheckBox']").each(function () {
                    var competencySource = $(this)[0].value;
                    if (($('input[name=CompetencySourceCheckBox]').is(':checked'))) {
                        if (competencySource == CAAHEP_COMPETENCY_SOURCE && this.checked) {
                            isCaahepNumberRequired = true;
                            isValid = true;
                        }
                        if (competencySource == ABHES_COMPETENCY_SOURCE && this.checked) {
                            isAbhesNumberRequired = true;
                            isValid = true;
                        }
                    }
                    else {
                        isSourceRequired = true;
                        if (!isInputRequired) {
                            isInputRequired = true;
                            errorMessage += "<LI>" + INPUT_REQUIRED + "</LI>";
                        }
                        isValid = true;
                    }
                });
                if (!isCaahepNumberRequired && !isAbhesNumberRequired) {
                    errorMessage += "<LI>" + CAAHEP_COMPETENCY_SOURCE + " # " + " or " + ABHES_COMPETENCY_SOURCE + " # " + COMPETENCY_IS_REQUIRED + "</LI>";
                    isValid = true;
                }
                else {
                    if (isCaahepNumberRequired) {
                        errorMessage += "<LI>" + CAAHEP_COMPETENCY_SOURCE + " # " + COMPETENCY_IS_REQUIRED + "</LI>";
                        isValid = true;
                    }
                    if (isAbhesNumberRequired) {
                        errorMessage += "<LI>" + ABHES_COMPETENCY_SOURCE + " # " + COMPETENCY_IS_REQUIRED + "</LI>";
                        isValid = true;
                    }
                }
            }
            else {
                if (isNullOrEmpty($("#CAAHEP").val())) {
                    $("input[type=checkbox][name^='CompetencySourceCheckBox']").each(function () {
                        var competencySource = $(this)[0].value;
                        if (competencySource == CAAHEP_COMPETENCY_SOURCE && this.checked) {
                            errorMessage += "<LI>" + CAAHEP_COMPETENCY_SOURCE + " # " + COMPETENCY_IS_REQUIRED + "</LI>";
                            isValid = true;
                        }
                    });
                }
                else {
                    $("input[type=checkbox][name^='CompetencySourceCheckBox']").each(function () {
                        var competencySource = $(this)[0].value;
                        if (competencySource == CAAHEP_COMPETENCY_SOURCE && !this.checked) {
                            errorMessage += "<LI>" + "Source " + CAAHEP_COMPETENCY_SOURCE + COMPETENCY_IS_REQUIRED + "</LI>";
                            isValid = true;
                        }

                    });
                }

                if (isNullOrEmpty($("#ABHES").val())) {
                    $("input[type=checkbox][name^='CompetencySourceCheckBox']").each(function () {
                        var competencySource = $(this)[0].value;
                        if (competencySource == ABHES_COMPETENCY_SOURCE && this.checked) {
                            errorMessage += "<LI>" + ABHES_COMPETENCY_SOURCE + " # " + COMPETENCY_IS_REQUIRED + "</LI>";
                            isValid = true;
                        }

                    });
                }
                else {
                    $("input[type=checkbox][name^='CompetencySourceCheckBox']").each(function () {
                        var competencySource = $(this)[0].value;
                        if (competencySource == ABHES_COMPETENCY_SOURCE && !this.checked) {
                            errorMessage += "<LI>" + "Source " + ABHES_COMPETENCY_SOURCE + COMPETENCY_IS_REQUIRED + "</LI>";
                            isValid = true;
                        }
                    });
                }
            }


            if (isValid) {
                errorMessage += "</UL>";
                $("#validationSummary")[0].innerHTML = errorMessage;
                $("#validationSummary").focus();
                $("#validationSummary").show();
                return false;
            }
            else {
                $("#validationSummary")[0].innerHTML = "";
                $("#validationSummary").hide();
                return true;
            }
        },
        /*  to show competency pop up*/
        showCompetency: function (titleText) {
            $("#validationSummary").hide();
            $('#competency_content').dialog({
                autoOpen: true,
                modal: true,
                closeOnEscape: false,
                resizable: false,
                open: function () {
                    $('#image_view_load_inner_content').css('overflow', 'hidden');
                    removeDialogForGrayHeader();

                },
                close: function () {
                    $(this).dialog('destroy').remove();
                },
                beforeClose: function () {
                    reapplyDialogHeader();
                },
                title: '<div class="header-text">' + titleText + '</div>',
                width: 530
            });
        },
        /* Ajax call to add source*/
        funSourceLstAdd: function () {
            if (!isNullOrEmpty($("#Source").val())) {
                $("#validationSummary")[0].innerHTML = "";
                $("#validationSummary").hide();
                var competencySourcesJson = {
                    "Name": $("#Source").val()
                };
                var urlToSaveSource = "../Competency/SaveCompetencySources";
                if (competencySourcesJson != null) {
                    startAjaxLoader();
                    doAjaxCall("POST", competencySourcesJson, urlToSaveSource, this.successSource);
                    closeAjaxLoader();
                }
            }
            else {
                var errormessage = "<UL> <LI>" + INPUT_REQUIRED + "</LI> </UL>";
                $("#validationSummary")[0].innerHTML = errormessage;
                $("#validationSummary").focus();
                $("#validationSummary").show();
            }

        },
        /* success function after source added */
        successSource: function (result) {
            $("#Source").val("");
            competency.commonFunctions.loadSourceList(result.sourceList);
        },

        /* to add source to the list */
        loadSourceList: function (sourceList) {
            $("#sourceList").empty();
            if (sourceList != null) {
                for (var indexSource = 0; indexSource < sourceList.length; indexSource++) {
                    var check = '<input type="checkbox" name="CompetencySourceCheckBox" id="CompetencySourceCheckBox_' + indexSource + '" value="' + sourceList[indexSource] + '" /> <label for="CompetencySourceCheckBox_' + indexSource + '">' + sourceList[indexSource] + '</label>' + '<br />';
                    $("#sourceList").append(check);
                }
            }
        },

        /* to load  competency popup */
        loadCompetency: function (competencyUrl) {
            var titleText = (isNullOrEmpty(competencyUrl) ? "Create New Competency" : "Edit/Delete Competency");
            startAjaxLoader();
            $("#showCreateNewCompetency").empty();
            $('#competency_content').empty();
            $("#showCreateNewCompetency").load("../Competency/LoadCompetency?competencyGuid=" + competencyUrl, function () {
                competency.commonFunctions.showCompetency(titleText);
                initializeFlexBoxForCompetencyCategory('CompetencyCategory', categoryList, competencyCategory);
            });
            closeAjaxLoader();
        },

        /* to retain seleceted checkboxes in pagination for competency grid */
        competencyItemChanged: function (obj) {
            var idOfCompetencyItem = obj.id;

            strListOfSelectedCompetencyItems = check_AddOrRemoveIfItemExistsInString(strListOfSelectedCompetencyItems, idOfCompetencyItem, 'Ø', obj.checked);
            //competencyDataTable.fnDraw(false);
            //            if (!isNullOrEmpty(strListOfSelectedCompetencyItems)) {
            //                enableAButton("btnDeleteCompetency", BLUE_BUTTON, "transaction-button", competency.commonFunctions.deleteCompetency);
            //            }
        },

        /* to delete competency */
        deleteCompetency: function () {
            if (isNullOrEmpty(strListOfSelectedCompetencyItems)) {
                jAlert(SELECT_ITEM_MESSAGE, ALERT_TITLE);
            }
            else {
                this.deleteCompetencyAjaxCall("");
            }
        },
        /*success function after deleting competency */
        deleteSuccess: function () {
            jAlert(DELETE_MESSAGE, ALERT_TITLE_DELETE, function (isOk) {
                if (isOk) {
                    closeDialog("competency_content");
                    $("#showCreateNewCompetency").empty();
                    $('#competency_content').empty();
                    //$("#smoAdmin").load("../Competency/LoadMaster?iReferenceOfMasterToLoad=1");
                    isCompetencyDelete = true;
                    startAjaxLoader();
                    competencyDataTable.fnDraw();
                    strListOfSelectedCompetencyItems = [];
                    closeAjaxLoader();
                    isCompetencyDelete = false;


                }
            });
        },
        /*Ajax call to delete competency */
        deleteCompetencyAjaxCall: function (strUniqueIdentifier) {
            var selectedCompetencyItems = "";
            if (!(isNullOrEmpty(strUniqueIdentifier))) {
                selectedCompetencyItems = strUniqueIdentifier;
            }
            else {
                selectedCompetencyItems = strListOfSelectedCompetencyItems;
            }
            jConfirm(DELETE_COMPETENCY_MESSAGE, ALERT_TITLE_DELETE, function (isOk) {
                if (isOk) {
                    startAjaxLoader();
                    var urlToDeleteCompetency = "../Competency/DeleteCompetency";
                    doAjaxCall("POST", selectedCompetencyItems, urlToDeleteCompetency, competency.commonFunctions.deleteSuccess);
                    closeAjaxLoader();
                }
            });
        }
    }
};

        