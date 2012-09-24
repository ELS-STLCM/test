var strListOfSelectedAssignmentItems = [];
var selectedSkillSetGuid = "";
var selectedQuestionGuidsToAdd = [];
var lstOfItems = {};
var competencyFiltertext = "";
var selectedQuestionGuid = "";
var skillsetUrl = "";
var selectedReferenceQuestionGuid = "";
var assignmentUrl = "";
var assignmentUniqueIdentifier = "";
var selectedSkillSets_Filter = [];
var unSelectedSkillSetsForRemove = [];
var CompetencyList = [];
var isSaveAndProceedStep1 = false;
var selectedItemSequenceNumber = "";
var SelectedSkillSetList = [];
var ListofPublishedSkillsets = [];
var isModuleFilterValueSelectedForSearch = false;
var isModuleFilterValueClearedForSearch = false;
var isSearchPageForAssignment = false;
var isSelectedItemAQuestion = true;
var assignBuilder = {
    /*Common functions used in question bank module*/
    commonFunctions: {
        /*Search actions*/
        gridOperations: {
            AssignmentItemChanged: function (obj) {
                var idOfassignItem = obj.id;
                strListOfSelectedAssignmentItems = check_AddOrRemoveIfItemExistsInString(strListOfSelectedAssignmentItems, idOfassignItem, 'Ø', obj.checked);
            }
        },
        setAssignmentUniqueIdentifier: function (uniqueIdentifier) {
            setDivData(getDivObject("assignmentBuilderTab"), "assignmentUniqueIdentifier", uniqueIdentifier);
        },
        getAssignmentUniqueIdentifier: function () {
            return getDivData("assignmentBuilderTab", "assignmentUniqueIdentifier");
        },
        returnToAssignment: function () {
            startAjaxLoader();
            $("#authoringAssignmentBuilder").load("../Authoring/AssignmentBuilderLanding");
            closeAjaxLoader();
        },
        searchByText: function () {
            var searchText = $("#searchByTextAssign").val();
            var searchModule = $("#filterByModule_input").val();
            if (isSearchPageForAssignment) {
                searchModule = $("#filterByModuleForSearch_input").val();
            }

            searchText = trimText(searchText);
            var isSearchValid = true;
            if (isNullOrEmpty(searchText) || searchText.length < 2) {
                isSearchValid = false;
                jAlert(SEARCH_VALIDATION, ALERT_TITLE, function () {
                });
            }
            if (isSearchValid) {
                startAjaxLoader();
                strListOfSelectedAssignmentItems = "";
                var assignmentSearchContent = "../AssignmentBuilder/GiveSearchResults?strSearchText=" + encodeURIComponent(searchText) + "&strModule=" + encodeURIComponent(searchModule);
                $("#authoringAssignmentBuilder").load(assignmentSearchContent, function () {
                    //closeAjaxLoader();
                });
            }
        },
        LoadStep1OrStep5: function (assignmentId, isPublished) {
            this.setAssignmentUniqueIdentifier(assignmentId);
            if (!isPublished) {
                assignBuilder.redirectionFunctions.loadStep1OfAssignmentBuilder();
            }
            else {
                assignBuilder.redirectionFunctions.loadStep5OfAssignmentBuilder();
            }
        },
        LoadUnselectedSkillsets: function (unselectedSkillsetList) {
            var competencyDivs = "";
            if (unselectedSkillsetList != null && unselectedSkillsetList.length > 0) {
                for (var compCount = 0; compCount < unselectedSkillsetList.length; compCount++) {
                    competencyDivs += "<div class='grid_32 bottom-only-border' data-questioncount='" + unselectedSkillsetList[compCount].QuestionCount + "' data-sequencenumber='" + unselectedSkillsetList[compCount].SequenceNumber + "' data-skillseturl='" + unselectedSkillsetList[compCount].Url + "' data-skillsettitle ='" + unselectedSkillsetList[compCount].SkillSetTitle + "' data-guid ='" + unselectedSkillsetList[compCount].UniqueIdentifier + "' id='skillSet_" + unselectedSkillsetList[compCount].UniqueIdentifier + "'><input type='checkbox' class='grid_3' name='SelectSkillSet' id='SelectSkillSet_" + compCount + "' onclick='skillSet.SkillSetMetadata.HighlightCompetency(this);'/><label id='lblSkillset_" + compCount + "' class='dual-list align grid_25' for='SelectSkillSet_" + compCount + "'>" + unselectedSkillsetList[compCount].SkillSetTitle + "</label><a href='#' onclick=assignBuilder.commonFunctions.showCompetencies('" + unselectedSkillsetList[compCount].Url + "')>View</a><div class='clear-height-spacing clear'></div></div>";
                }
            }
            $("#SkillSetUnselectedList").empty();
            if (competencyDivs != "") {
                $("#SkillSetUnselectedList").append(competencyDivs);
            }
        },
        showCompetencies: function (skillsetUrl) {
            var urlCompetencies = "../AssignmentBuilder/GetCompetenciesForASkillSet?skillsetUniqueIdentifier=" + skillsetUrl;
            doAjaxCall("POST", "", urlCompetencies, assignBuilder.commonFunctions.successCompetencyListFetch);
        },
        successCompetencyListFetch: function (result) {
            if (result.Result != null) {
                CompetencyList = result.Result;
                $("#LoadCompetencyContent").show();
                $("#LoadCompetencyContent").dialog({
                    autoOpen: true,
                    modal: true,
                    closeOnEscape: false,
                    position: 'center',
                    resizable: false,
                    title: 'Linked Competencies',
                    height: 250,
                    width: 450,
                    open: function () {
                        applyClassForDialogHeader();
                        if (CompetencyList != null && CompetencyList != "") {
                            $("#competenciesLoaded").empty();
                            $("#competenciesLoaded").append(CompetencyList);
                        }
                    },
                    overlay: { opacity: 0.5, background: 'black' }
                });
            }
        },
        initializeFlexBoxForCompetencyList: function (divId, lstOfItems) {
            $('#' + divId).flexbox(lstOfItems, {
                resultTemplate: '{name}',
                width: 190,
                paging: false,
                maxVisibleRows: 10,
                noResultsText: '',
                noResultsClass: '',
                matchClass: '',
                matchAny: true,
                onSelect: function () {
                    startAjaxLoader();
                    competencyFiltertext = $('#' + divId + "_hidden").val();
                    assignBuilder.commonFunctions.FilterSkillSetsOnSelectedCompetency();
                    closeAjaxLoader();
                }
            });
            $('#' + divId + "_input").watermark("Search by name or number", { className: 'watermark watermark-text' });
        },
        GetMaxSeqNum: function () {
            var maxValue = 1;
            if (selectSkillsetList.length == 0) {
                return maxValue;
            }
            for (var listIndex = 0; listIndex < selectSkillsetList.length; listIndex++) {
                if (maxValue < selectSkillsetList[listIndex].SequenceNumber) {
                    maxValue = selectSkillsetList[listIndex].SequenceNumber;
                }
            }
            return maxValue + 1;
        },
        AddCompetencies: function () {
            $("#SkillSetUnselectedList").contents().find("input[name='SelectSkillSet']:checked").each(function () {
                $(this).attr('checked', false);
                $(this).parent().removeClass("competency-highlight");
//                var listindex = selectSkillsetList.length;
                var selectedskillset = {
                    "Guid": getDivData($(this).parent()[0].id, "guid"),
                    "SequenceNumber": assignBuilder.commonFunctions.GetMaxSeqNum(),
                    "SkillSetTitle": (getDivData($(this).parent()[0].id, "skillsettitle")),
                    "SkillSetUrl": getDivData($(this).parent()[0].id, "skillseturl")
                };
                selectSkillsetList.push(selectedskillset);
                $("#SkillSetUnselectedList #" + $(this).parent()[0].id).remove();
            });
            assignBuilder.commonFunctions.RefreshSelectedListBox();
        },
        RefreshSelectedListBox: function () {
            $("#SkillSetSelectedList").empty();
            assignBuilder.commonFunctions.SortSkillsetList();
            for (var selectedIndex = 0; selectedIndex < selectSkillsetList.length; selectedIndex++) {
                if (selectedItemSequenceNumber == selectSkillsetList[selectedIndex].SequenceNumber) {
                    $("#SkillSetSelectedList").append("<option selected='selected' Guid = '" + selectSkillsetList[selectedIndex].Guid + "' SequenceNumber = '" + selectSkillsetList[selectedIndex].SequenceNumber + "' SkillSetUrl = '" + selectSkillsetList[selectedIndex].SkillSetUrl + "'>" + selectSkillsetList[selectedIndex].SkillSetTitle + "</option>");
                } else {
                    $("#SkillSetSelectedList").append("<option Guid = '" + selectSkillsetList[selectedIndex].Guid + "' SequenceNumber = '" + selectSkillsetList[selectedIndex].SequenceNumber + "' SkillSetUrl = '" + selectSkillsetList[selectedIndex].SkillSetUrl + "'>" + selectSkillsetList[selectedIndex].SkillSetTitle + "</option>");
                }
            }
        },
        LoadSavedSkillSets: function (savedSkillSets) {
            $("#SkillSetSelectedList").empty();
            selectSkillsetList = [];
            for (var skillCount = 1; skillCount <= savedSkillSets.length; skillCount++) {
                var skillSetUrl = savedSkillSets[skillCount - 1].Url;
                var skillSetTitle = savedSkillSets[skillCount - 1].SkillSetTitle;
                //$("#SkillSetSelectedList").append("<option SequenceNumber = '" + savedSkillSets[skillCount - 1].SequenceNumber + "' SkillSetUrl = '" + skillSetUrl + "' QuestionCount = '" + savedSkillSets[skillCount - 1].QuestionCount + "'>" + skillSetTitle + "</option>");
                $("#SkillSetSelectedList").append("<option Guid='" + savedSkillSets[skillCount - 1].UniqueIdentifier + "' SequenceNumber = '" + savedSkillSets[skillCount - 1].SequenceNumber + "' SkillSetUrl = '" + skillSetUrl + "' QuestionCount = '" + savedSkillSets[skillCount - 1].QuestionCount + "'>" + skillSetTitle + "</option>");
                var selectedskillset = {
                    "Guid": savedSkillSets[skillCount - 1].UniqueIdentifier,
                    "SequenceNumber": savedSkillSets[skillCount - 1].SequenceNumber,
                    "SkillSetTitle": skillSetTitle,
                    "SkillSetUrl": skillSetUrl
                };
                selectSkillsetList.push(selectedskillset);
            }
        },
        UpdateNextSequenceNumber: function (currentSeqNum, currentIndex) {
            var listcount = selectSkillsetList.length;
            var minseqNum = selectSkillsetList[listcount - 1].SequenceNumber;
            for (var i = listcount - 1; i >= 0; i--) {
                if (selectSkillsetList[i].SequenceNumber <= minseqNum && selectSkillsetList[i].SequenceNumber > currentSeqNum) {
                    minseqNum = selectSkillsetList[i].SequenceNumber;
                } else {
                    {
                        selectSkillsetList[currentIndex].SequenceNumber = minseqNum;
                        selectSkillsetList[currentIndex + 1].SequenceNumber = currentSeqNum;
                        selectedItemSequenceNumber = minseqNum;
                        return;
                    }
                }
            }
        },
        UpdatePrevSequenceNumber: function (currentSeqNum, currentIndex) {
            var listcount = selectSkillsetList.length;
            var maxseqNum = selectSkillsetList[0].SequenceNumber;
            for (var i = 0; i <= listcount - 1; i++) {
                if (selectSkillsetList[i].SequenceNumber >= maxseqNum && selectSkillsetList[i].SequenceNumber < currentSeqNum) {
                    maxseqNum = selectSkillsetList[i].SequenceNumber;
                } else {
                    selectSkillsetList[currentIndex].SequenceNumber = maxseqNum;
                    selectSkillsetList[currentIndex - 1].SequenceNumber = currentSeqNum;
                    selectedItemSequenceNumber = maxseqNum;
                    return;
                }
            }
        },
        SwapSkillsets: function (action) {
            $("#SkillSetSelectedList > option:selected").each(function () {
                var selectedseqnum = parseInt(this.getAttribute("SequenceNumber"));
                var selectedskillsetindex = parseInt(this.index);
                if (action == "downArrow") {
                    if (selectedskillsetindex < (selectSkillsetList.length - 1)) {
                        assignBuilder.commonFunctions.UpdateNextSequenceNumber(selectedseqnum, selectedskillsetindex);
                    }
                } else if (action == "upArrow") {
                    if (selectedskillsetindex > 0) {
                        assignBuilder.commonFunctions.UpdatePrevSequenceNumber(selectedseqnum, selectedskillsetindex);
                    }
                }
                assignBuilder.commonFunctions.RefreshSelectedListBox();
            });
        },
        SortSkillsetList: function () {
            selectSkillsetList.sort(function (a, b) {
                var a1 = a.SequenceNumber, b1 = b.SequenceNumber;
                if (a1 == b1) return 0;
                return a1 > b1 ? 1 : -1;
            });
        },
        GetUnselectSkillSetsFromDiv: function () {
            unSelectedSkillSetsForRemove = [];
            $("#SkillSetUnselectedList").contents().find("input[name='SelectSkillSet']").each(function () {
                var divIdForSkillset = $(this).parent()[0].id;
                var unselectedSkillSet = { "Url": getDivData(divIdForSkillset, "skillseturl"),
                    "SkillSetTitle": getDivData(divIdForSkillset, "skillsettitle"),
                    "QuestionCount": getDivData(divIdForSkillset, "questioncount"),
                    "UniqueIdentifier": getDivData(divIdForSkillset, "guid"),
                    "SequenceNumber": getDivData(divIdForSkillset, "sequencenumber")
                };
                unSelectedSkillSetsForRemove.push(unselectedSkillSet);
                //$("#SkillSetUnselectedList #" + $(this).parent()[0].id).remove();
            });
        },
        RemoveCompetencies: function () {
            $("#SkillSetSelectedList > option:selected").each(function () {
                assignBuilder.commonFunctions.GetUnselectSkillSetsFromDiv();
                var sequenceNumber = this.getAttribute("SequenceNumber");
                var skillSetUrl = this.getAttribute("SkillSetUrl");
                var skillSetTitle = $(this).val();
                var questionCount = this.getAttribute("QuestionCount");
                var dataGuid = this.getAttribute("Guid");
                var unselectedSkillSet = { "Url": skillSetUrl, "SkillSetTitle": skillSetTitle, "QuestionCount": questionCount, "UniqueIdentifier": dataGuid, "SequenceNumber": sequenceNumber };
                unSelectedSkillSetsForRemove.push(unselectedSkillSet);
                $(this).remove();
                var arrayIndex = 0;
                for (var listIndex = 0; listIndex < selectSkillsetList.length; listIndex++) {
                    if (selectSkillsetList[listIndex].SequenceNumber == sequenceNumber) {
                        arrayIndex = listIndex;
                        break;
                    }
                }
                selectSkillsetList.splice(arrayIndex, 1);
                assignBuilder.commonFunctions.LoadUnselectedSkillsets(unSelectedSkillSetsForRemove);
            });
        },
        FilterSkillSetsOnSelectedCompetency: function () {
            var urlQuestionBank = "../AssignmentBuilder/FilterSkillSetsBasedOnCompetencies?guidOfCompetency=" + competencyFiltertext;
            doAjaxCall("POST", "", urlQuestionBank, assignBuilder.commonFunctions.successSkillSetFetch);
        },
        successSkillSetFetch: function (result) {
            if (result.Result != null) {
                assignBuilder.commonFunctions.FilterSearchListBasedOnSelectedList(result.Result);
                //assignBuilder.commonFunctions.LoadUnselectedSkillsets(result.Result);
            }
            return null;
        },
        FilterSearchListBasedOnSelectedList: function (filteredList) {
            var skillSetIndex = 0;
            for (var selectedSkillCount = 0; selectedSkillCount < selectSkillsetList.length; selectedSkillCount++) {
                var selectedSkillSetUrl = selectSkillsetList[selectedSkillCount].SkillSetUrl;
                if (selectedSkillSetUrl != null && selectedSkillSetUrl != "") {
                    for (var innerCount = 0; innerCount < filteredList.length; innerCount++) {
                        if (selectedSkillSetUrl == filteredList[innerCount].Url) {
                            skillSetIndex = innerCount;
                            break;
                        }
                    }
                    filteredList.splice(skillSetIndex, 1);
                }
            }
            assignBuilder.commonFunctions.LoadUnselectedSkillsets(filteredList);
        },
        validateAssignmentFields: function () {
            var isValid = true;
            var skillSetsValid = false;
            var isModuleValid = false;
            var isPatientValid = true;
            var isResourceValid = true;
            var errorMessage = "<UL>";
            assignBuilder.commonFunctions.GetSelectedSkillSetsList();
            if (selectedSkillSets_Filter.length > 0) {
                skillSetsValid = true;
            }
            if (isNullOrEmpty($("#AssignmentTitle").val())) {
                isValid = false;
            }
            var moduleList = assignBuilder.commonFunctions.GetModuleList();
            if (moduleList != null && moduleList.length > 0) {
                isModuleValid = true;
            }
            //            if (isNullOrEmpty($('input[type=checkbox]:checked').val()) || ($('input[type=checkbox]:checked').val() != undefined )) {
            //                isValid = false;
            //            }
            if ($('input:radio[name=PatientOption]:checked').val() == "createNewPatient") {
                isPatientValid = patient.pageOperations.validatePatientFields();
            }
            isResourceValid = assignBuilder.commonFunctions.validateAssignmentResources();
            if (!isValid || !isModuleValid || !skillSetsValid || !isPatientValid || !isResourceValid) {
                //$("#assignment-Metadata-Main-Content").removeClass('.skill-set-MetaData-without-validation ').addClass('skill-set-MetaData-with-validation');
                errorMessage += "<LI>" + INPUT_REQUIRED + "</LI>";
                errorMessage += "</UL>";
                $("#validationSummary")[0].innerHTML = errorMessage;
                $("#validationSummary").focus();
                $("#validationSummary").show();
                $("#assignment-Metadata-Main-Content").scrollTo("#validationSummary", 300);
                closeAjaxLoader();
                return false;
            }
            else {
                return true;
            }
        },
        getPatientJson: function () {
            var patientJson = {};
            if ($('input:radio[name=PatientOption]:checked').val() == "createNewPatient") {
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
                    "IsAssignmentPatient": true
                };
            }
            return patientJson;
        },
        validateAssignmentResources: function () {
            var isResourcesValid = true;
            for (var resourceCountIndex = 1; resourceCountIndex < resourceCount; resourceCountIndex++) {
                if ($("#resourceDetail_" + resourceCountIndex).length > 0) {
                    if (isNullOrEmpty($("#" + resourceCountIndex + "_authorName").val())
                        || isNullOrEmpty($("#" + resourceCountIndex + "_resourceTitle").val())
                            || isNullOrEmpty($("#" + resourceCountIndex + "_resourceEdition").val())
                                || isNullOrEmpty($("#" + resourceCountIndex + "_resourceChapter").val())
                                    || isNullOrEmpty($("#" + resourceCountIndex + "_resourcePageRange").val())) {
                        isResourcesValid = false;
                    } else {
                        isResourcesValid = true;
                    }
                }
            }
            return isResourcesValid;
        },
        getResourceList: function () {
            var resourceList = [];
            var sequenceCount = 0;
            for (var resourceCountIndex = 1; resourceCountIndex < resourceCount; resourceCountIndex++) {
                // to check if the div with current resourceCountIndex is present or removed
                if ($("#resourceDetail_" + resourceCountIndex).length > 0) {
                    var resource = {
                        "Author": $("#" + resourceCountIndex + "_authorName").val(),
                        "Title": $("#" + resourceCountIndex + "_resourceTitle").val(),
                        "Edition": $("#" + resourceCountIndex + "_resourceEdition").val(),
                        "Chapter": $("#" + resourceCountIndex + "_resourceChapter").val(),
                        "PageRange": $("#" + resourceCountIndex + "_resourcePageRange").val(),
                        "SequenceNumber": sequenceCount
                    };
                    resourceList.push(resource);
                    sequenceCount++;
                }
            }
            return (resourceList.length > 0 ? resourceList : null);
        },
        GetSelectedSkillSetsList: function () {
            selectedSkillSets_Filter = [];
            for (var skillCount = 0; skillCount < selectSkillsetList.length; skillCount++) {
                var skillSetObject = { "SkillSetIdentifier": selectSkillsetList[skillCount].SkillSetUrl,
                    "SequenceNumber": selectSkillsetList[skillCount].SequenceNumber
                };
                selectedSkillSets_Filter.push(skillSetObject);
            }
        },
        GetModuleList: function () {
            var focusList = [];
            $("input[type='checkbox'][name=Module]:checked").each(function () {
                focusList.push($(this).val());
            });
            return (focusList.length > 0 ? focusList : null);
        },
        SaveAssignment: function (saveAndProceed) {
            if (saveAndProceed != null & saveAndProceed != undefined) {
                isSaveAndProceedStep1 = saveAndProceed;
            }
            if (assignBuilder.commonFunctions.validateAssignmentFields()) {
                startAjaxLoader();
                this.HideValidation();
                assignBuilder.commonFunctions.GetSelectedSkillSetsList();
                var strduration;
                strduration = assignBuilder.commonFunctions.GetAssignmentDuration();
                var assignmentJson = {
                    "Title": encodeSpecialSymbols($.trim($("#AssignmentTitle").val())),
                    "Module": assignBuilder.commonFunctions.GetModuleList(),
                    "Duration": strduration,
                    "Keywords": encodeSpecialSymbols($.trim($("#AssignmentKeywords").val())),
                    "Patient": assignBuilder.commonFunctions.getPatientJson(),
                    "PatientReference": $("input[type=hidden][id=filterByPatientName_hidden]").val(),
                    "Resources": assignBuilder.commonFunctions.getResourceList(),
                    "SkillSets": selectedSkillSets_Filter
                };
                //var asignmentUrl = getDivData("assignmentBuilderTab", "currentFolder");
                var folderUrl = "";
                var isEditMode = false;
                var currentFolderUrl = getDivData("assignmentBuilderTab", "currentFolder");
                var currentFolderIdentifier = getDivData("assignmentBuilderTab", "currentFolderIdentifier");
                if (this.getAssignmentUniqueIdentifier() != "") {
                    folderUrl = this.getAssignmentUniqueIdentifier();
                    isEditMode = true;
                }
                else if (currentFolderUrl != "" && currentFolderIdentifier != "") {
                    folderUrl = currentFolderUrl + "/" + currentFolderIdentifier;
                }

                var assignmentSaveUrl = "../AssignmentBuilder/SaveAssignmentMetadata?folderUrl=" + folderUrl + "&isEditMode=" + isEditMode;
                doAjaxCall("POST", assignmentJson, assignmentSaveUrl, assignBuilder.commonFunctions.AssignmentSaveSuccess);
            }
        },
        GetAssignmentDuration: function () {
            var strduration;
            var strdurationhrs = $("#AssignmentDurationHrs").val();
            if (strdurationhrs == "-Select-") {
                strdurationhrs = "";
            } else {
                strdurationhrs = "0" + strdurationhrs;
            }
            var strdurationmns = $("#AssignmentDurationMns").val();
            if (strdurationmns == "-Select-") {
                strdurationmns = "";
            }
            strduration = strdurationhrs + ":" + strdurationmns;
            if (strduration == ":") {
                strduration = "";
            }
            if (strdurationhrs == "" && strdurationmns != "") {
                strduration = "00" + ":" + strdurationmns;
            }
            if (strdurationhrs != "" && strdurationmns == "") {
                strduration = strdurationhrs + ":" + "00";
            }
            return strduration;
        },
        LoadAssignmentInEditMode: function (assignmentToEdit) {
            //load title
            if (assignmentToEdit.Title != null && assignmentToEdit.Title != "") {
                $("#AssignmentTitle").val(assignmentToEdit.Title);
            }

            //load module
            if (assignmentToEdit.Module != null && assignmentToEdit.Module.length > 0) {
                $("input[type='checkbox'][name=Module]").each(function () {
                    if ($.inArray($(this).val(), assignmentToEdit.Module) >= 0) {
                        $(this).attr('checked', true);
                    }
                });
            }

            //load keywords 
            if (assignmentToEdit.Keywords != null) {
                $("#AssignmentKeywords").val(assignmentToEdit.Keywords);
            }

            // load duration
            if (assignmentToEdit.Duration != null && assignmentToEdit.Duration != "") {
                var duration = assignmentToEdit.Duration.split(':');
                var hrs = duration[0].split("0", 2);
                $("#AssignmentDurationHrs").val(hrs[1]);
                $("#AssignmentDurationMns").val(duration[1]);
            }
            else {
                $("#AssignmentDurationHrs").val(DROPDOWN_SELECT);
                $("#AssignmentDurationMns").val(DROPDOWN_SELECT);
            }

            //load patient
            if (assignmentToEdit.Patient != null) {
                if (assignmentToEdit.Patient.ParentReferenceGuid != null && assignmentToEdit.Patient.ParentReferenceGuid != "") {
                    $("#selectPatient").attr('checked', true);
                    savedPatientValue = assignmentToEdit.Patient.LastName + ", " + assignmentToEdit.Patient.FirstName + " " + assignmentToEdit.Patient.MiddleInitial;
                    assignment.commonFunctions.patientRadioSelection();

                    //                    $("input[type=hidden][id=filterByPatientName_hidden]").val(assignmentToEdit.Patient.ParentReferenceGuid);
                    //                    $("#filterByPatientName").val(assignmentToEdit.Patient.LastName + ", " + assignmentToEdit.Patient.FirstName + " " + assignmentToEdit.Patient.MiddleInitial);
                    //                    $("#filterByPatientName").removeClass('watermark watermark-text');
                }
                else {
                    $("#createNewPatient").attr('checked', true);
                    savedPatientValue = "";
                    assignment.commonFunctions.patientRadioSelection();

                    $("#firstName").val(assignmentToEdit.Patient.FirstName);
                    $("#lastName").val(assignmentToEdit.Patient.LastName);
                    $("#middleInitial").val(assignmentToEdit.Patient.MiddleInitial);
                    $("#ageInYears").val(assignmentToEdit.Patient.AgeInYears);
                    $("#ageInMonths").val(assignmentToEdit.Patient.AgeInMonths);
                    $("#ageInDays").val(assignmentToEdit.Patient.AgeInDays);
                    $("#dayOfBirth").val(assignmentToEdit.Patient.DateOfBirth);
                    //$('input:radio[id=' + assignmentToEdit.Patient.Sex + ']').attr('checked', true);
                    $("input[type=radio][id='" + assignmentToEdit.Patient.Sex + "Sex']").attr('checked', true);
                    patient.pageOperations.getOrSetImageReference(false, "patientImage", assignmentToEdit.Patient.UploadImage),
                    $("#medicalRecordNumber").val(assignmentToEdit.Patient.MedicalRecordNumber);
                    $("#officeType").val(assignmentToEdit.Patient.OfficeType);
                    $("#provider").val(assignmentToEdit.Patient.Provider);

                }
            }

            //load resources

            if (assignmentToEdit.Resources != null && assignmentToEdit.Resources.length > 0) {
                for (var resCount = 0; resCount < assignmentToEdit.Resources.length; resCount++) {
                    addResource();
                    $('input:checkbox[name=addResource]').attr('checked', true);
                    $("#addButton").show();
                    $("#" + (resCount + 1) + "_authorName").val(assignmentToEdit.Resources[resCount].Author);
                    $("#" + (resCount + 1) + "_resourceTitle").val(assignmentToEdit.Resources[resCount].Title);
                    $("#" + (resCount + 1) + "_resourceEdition").val(assignmentToEdit.Resources[resCount].Edition);
                    $("#" + (resCount + 1) + "_resourceChapter").val(assignmentToEdit.Resources[resCount].Chapter);
                    $("#" + (resCount + 1) + "_resourcePageRange").val(assignmentToEdit.Resources[resCount].PageRange);
                }
            }

        },
        AssignmentSaveSuccess: function (result) {
            assignBuilder.commonFunctions.setAssignmentUniqueIdentifier(result.assignmentUrl);
            if (result.messageToReturn != "") {
                closeAjaxLoader();
                if (isSaveAndProceedStep1) {
                    isSaveAndProceedStep1 = false;
                    //reset page global variables
                    assignBuilder.commonFunctions.ResetStep1();

                    assignBuilder.redirectionFunctions.loadStep2OfAssignmentBuilder();
                }
                else {

                    jAlert("Saved", ALERT_TITLE, function () {

                    });
                }
            } else {
                closeAjaxLoader();
                jAlert("Failed");
            }
            return false;
        },
        HideValidation: function () {
            $("#validationSummary").empty();
            $("#validationSummary").hide();
        },
        ResetStep1: function () {
            // resetting global variables used in step1
            selectSkillsetList = [];
        },
        getListOfCompetencyForAssignmentFlexbox: function () {
            var urlForString = "../AssignmentBuilder/GetCompetenciesForFlexBox?assignmentUrl=" + assignBuilder.commonFunctions.getAssignmentUniqueIdentifier();
            doAjaxCall("POST", "", urlForString, assignBuilder.commonFunctions.initializeFlexBoxForAssignmentQnBank);

        },
        initializeFlexBoxForAssignmentQnBank: function (result) {
            var competencyQuestionStringListTemp1 = {};
            if (result.Result != null) {
                competencyQuestionStringListTemp1.results = result.Result;
                competencyQuestionStringListTemp1.total = competencyQuestionStringListTemp1.results.length;
                initializeFlexBox("LinkedCompetency", competencyQuestionStringListTemp1);
            }
                return null;
        }
    },

    redirectionFunctions: {
        loadStep3OfAssignmentBuilder: function () {
            startAjaxLoader();
            $("#authoringAssignmentBuilder").load("../AssignmentBuilder/AssignmentBuilder", function () {
                $("#divToLoadSteps").load("../AssignmentBuilder/LoadAssignmentStep3?assignmentUrl=" + assignBuilder.commonFunctions.getAssignmentUniqueIdentifier() + "&selectedAttempts=" + "" + "&selectedPassRate=" + "", function () {
                    //closeAjaxLoader();
                });
            });
        },
        loadStep1OfAssignmentBuilder: function () {
            savedQuestionsCount = [];
            startAjaxLoader();
            $("#authoringAssignmentBuilder").load("../AssignmentBuilder/AssignmentBuilder", function () {
                $("#divToLoadSteps").load("../AssignmentBuilder/LoadAssignmentMetaData?assignmentUniqueIdentifier=" + assignBuilder.commonFunctions.getAssignmentUniqueIdentifier(), function () {
                    closeAjaxLoader();
                });
            });
        },
        loadStep2OfAssignmentBuilder: function () {
            startAjaxLoader();
            $("#authoringAssignmentBuilder").load("../AssignmentBuilder/AssignmentBuilder", function () {
                $("#divToLoadSteps").load("../AssignmentBuilder/ReditectToOrientation", function () {
                    assignment.commonFunctions.getAssignment(assignBuilder.commonFunctions.getAssignmentUniqueIdentifier());
                    closeAjaxLoader();
                });
            });
        },
        loadStep4OfAssignmentBuilder: function () {
            startAjaxLoader();
            $("#authoringAssignmentBuilder").load("../AssignmentBuilder/AssignmentBuilder", function () {
                $("#divToLoadSteps").load("../AssignmentBuilder/LoadAssignmentOfficeSettings", function () {
                    closeAjaxLoader();
                });
            });
        },
        loadStep5OfAssignmentBuilder: function () {
            startAjaxLoader();
            $("#authoringAssignmentBuilder").load("../AssignmentBuilder/AssignmentBuilder", function () {
                $("#divToLoadSteps").load("../AssignmentBuilder/PreviewAndPublishStep5?assignmentUrl=" + assignBuilder.commonFunctions.getAssignmentUniqueIdentifier(), function () {
                    closeAjaxLoader();
                });
            });
        }
    },

    step3Actions: {
        revertToOriginal: function () {
            var url = "../AssignmentBuilder/RevertQuestionToOriginal?questionUrl=" + UrlOfSelectedQuestion + "&questionGuid=" + strQuestionGuid + "&skillSetGuid=" + selectedSkillSetGuid + "&isSelectedItemQuestion=" + isSelectedItemAQuestion;
            doAjaxCall("POST", "", url, assignBuilder.step3Actions.OnSuccessRevertQuestion);
        },

        OnSuccessRevertQuestion: function () {
            //savedQuestionsCount.splice(strQuestionGuid, 1);
            for (var count = 0; count < savedQuestionsCount.length; count++) {
                if (strQuestionGuid == savedQuestionsCount[count].id) {
                    savedQuestionsCount.splice(count, 1);
                }
            }
            startAjaxLoader();
            $("#divToLoadSteps").load("../AssignmentBuilder/LoadAssignmentStep3?assignmentUrl=" + assignBuilder.commonFunctions.getAssignmentUniqueIdentifier() + "&selectedAttempts=" + $("#NoOfAttemptsAllowed").val() + "&selectedPassRate=" + $("#AssignmentPassRate_input").val());
            closeAjaxLoader();
        }
    }
}