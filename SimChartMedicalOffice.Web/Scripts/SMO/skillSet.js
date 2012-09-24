var strListOfSelectedSkillSetItems = [];
var questionType = "";
var strQuestionGuid = "";
var strParentReferenceGuidOfQuestion = "";
var strSkillSetUrl = "";
var savedQuestionsCount = [];
var isSearchPage = false;
var isContentChanged = false;
var selectedSources_Filter = [];
var selectedCompetencyListTemp = [];
var competencySearchText_Filter = "";
var competencyQuestionSearchFiltertext = "";
var selectedQuestionTypeFilterText = "";
var selectedCompetencies_Filter = [];
var selectedCompetencyQuestion_Filter = [];
var selectedQuestionTemplateList = [];
var selectQuestionOrderList = []; // select question Order list to save 
var QuestionList = [];
var UrlOfSelectedQuestion = "";
var competencyQuestionStringListTemp = {};
var questionTypeList = {};
var isCompetencyFilterValueSelected = false;
var selectedQuestionSwap = "";
var selectedQuestionDestinationOrderSeqNo = "";
var isNewQuestionRequired = false;
var isProceedToStep4Valid = false;
var isSkillSetEditMode = false;
var questionBankCompetencyList = [];
var isSkillStructureContentChange = false;
var IsSkillStructureEditMode = false;
var isSourceFilterValueSelected = false;
var isSourceFilterValueSelectedForSearch = false;
var isSourceFilterValueClearedForSearch = false;
var isSearchPageForSkillSet = false;
var skillSet = {
    /*Common functions used in skillSet module*/
    commonFunctions: {
        getListOfCompetencyForSkillSetFlexBoxInQnBank: function () {
            var urlForString = "../SkillSet/GetCompetencyForSkillSetFlexBox?uniqueIdentifier=" + skillSet.commonFunctions.getSkillSetUniqueIdentifier();
            doAjaxCall("POST", "", urlForString, skillSet.commonFunctions.initializeFlexBoxForSkillSetQnBank);

        },
        initializeFlexBoxForSkillSetQnBank: function (result) {
            var competencyQuestionStringListTemp1 = {};
            if (result.competencyStringListTemp != null) {
                //competencyArray = result.competencyArray;
                competencyQuestionStringListTemp1.results = eval('(' + result.competencyStringListTemp + ')');
                competencyQuestionStringListTemp1.total = competencyQuestionStringListTemp1.results.length;
                competencyArray = result.competencyArray;
                initializeFlexBox("LinkedCompetency", competencyQuestionStringListTemp1);
            }
            return null;
        },
        getSkillSetUniqueIdentifier: function () {
            return $("#authoringSkillSetBuilder").data("skillSetIdentifier");
        },
        setSkillSetUniqueIdentifier: function (uniqueIdentifier) {
            setDivData($("#authoringSkillSetBuilder"), "skillSetIdentifier", uniqueIdentifier);
        },
        clearSkillSetUniqueIdentifier: function () {
            setDivData($("#authoringSkillSetBuilder"), "skillSetIdentifier", "");
        },
        loadImageForAStep: function (stepInfo) {
            var imgSrc = "";
            switch (stepInfo) {
                case "1":
                    imgSrc = "../../Content/Images/Step1_Of_4.png";
                    break;
                case "2":
                    imgSrc = "../../Content/Images/Step2_Of_4.png";
                    break;
                case "3":
                    imgSrc = "../../Content/Images/Step3_Of_4.png";
                    break;
                case "4":
                    imgSrc = "../../Content/Images/Step4Of4.png";
                    break;
                default:
            }
            $("#img_for_skillset").attr("src", imgSrc).removeClass("hide-content").addClass("show-content");
        },
        gridOperations:
            {
                skillSetItemChanged: function (obj) {
                    var idOfSkillSetItem = obj.id;
                    strListOfSelectedSkillSetItems = check_AddOrRemoveIfItemExistsInString(strListOfSelectedSkillSetItems, idOfSkillSetItem, 'Ø', obj.checked);
                }
            },
        searchByText: function () {
            var searchText = $("#searchByTextSSB").val();
            var searchSource = $("#FilterBySource_input").val();
            if (isSearchPageForSkillSet) {
                searchSource = $("#FilterBySourceForSearch_input").val();
            }
            searchText = trimText(searchText);
            var isSearchValid = true;
            if (isNullOrEmpty(searchText) || searchText.length < 2) {
                if (!(searchText == "" && isSearchPage)) {
                    isSearchValid = false;
                    jAlert(SEARCH_VALIDATION, ALERT_TITLE, function () {
                    });
                }
            }
            if (isSearchValid) {
                startAjaxLoader();
                strListOfSelectedSkillSetItems = "";
                var skillSetSearchContent = "../SkillSet/GiveSearchResults?strSearchText=" + encodeURIComponent(searchText) + "&strSource=" + encodeURIComponent(searchSource);
                $("#authoringSkillSetBuilder").load(skillSetSearchContent, function () {
                });
            }
        },
        loadQuestionInEditModeForSkillSet: function (strUrlOfQuestion, iQuestionType, questionGuid) {
            questionType = iQuestionType;
            strQuestionGuid = questionGuid;
            questionBank.commonFunctions.hideOrShowQuestionBankContent(false);
            $("#questionTypes").load("../QuestionBank/RenderQuestionInEditMode?questionQuid=" + strUrlOfQuestion + "&iQuestionType=" + iQuestionType + "&folderType=4", function () {
                questionBank.commonFunctions.hideOrShowQuestionBankContent(true);
                if (iQuestionType == 2 || iQuestionType == 5) {
                    $("#rationaleLinkedCompetency").hide();
                } else {
                    $("#rationaleLinkedCompetency").show();
                }
                if (iQuestionType == 4) {
                    $("#rationale_correct_ans").hide();
                } else {
                    $("#rationale_correct_ans").show();
                }
                isContentChanged = false;
                skillSet.commonFunctions.loadQuestionForNewQuestion();
                $('#LinkedCompetency_ctr').attr("style", "left: 0px;top: 22px;width: 368px;display:none;");
                $('#LinkedCompetency_input').attr("style", "width: 368px;");
                $('#competencySuggest').removeClass("competency-suggest");
            });
        },
        getUrlReferenceForQuestionSave: function () {
            return $("#authoringSkillSetBuilder").data("strUrlReferenceForQnSave");

        },
        setUrlReferenceForQuestionSave: function (strUrlReferenceForQnSave) {
            setDivData($("#authoringSkillSetBuilder"), "strUrlReferenceForQnSave", strUrlReferenceForQnSave);
        },
        getNewQuestionTextForSkillSetToQB: function (questionType, isNewQuestion) {
            if (isNewQuestion) {
                if (questionType == "4") {
                    questionNewTextToSave = questionBank.commonFunctions.getQuestionForFillIn($("#BlankOrientation").val(), "newQuestionTextDivFillIn");
                    if (isNullOrEmpty(questionNewTextToSave)) {
                        questionNewTextToSave = questionBank.commonFunctions.getQuestionForFillIn($("#BlankOrientation").val(), "dynamicRenderDiv");
                    }
                }
                else if (questionType == "9") {
                    questionNewTextToSave = getControlValueByElementId("QuestionTextNew", TEXTBOX_CONTROL);
                }
                else {
                    questionNewTextToSave = getControlValueByElementId("QuestionTextAreaNew", TEXTBOX_CONTROL);
                }
            } else {
                questionNewTextToSave = "";
            }
        },
        saveSkillSetQuestion: function () {
            var isNewQuestionChecked = false;
            var isNewQuestion = false;
            var isExistingQuestion = false;
            questionBank.commonFunctions.setmaxLenghtOfAnswers(questionType);
            if (($('input[name=AddQuestionToQuestionBank]').is(':checked'))) {
                isNewQuestionChecked = true;
            }
            if (questionBank.commonFunctions.checkIfCorrectAnswer("QuestionToQuestionBankNew") && isNewQuestionChecked) {
                isNewQuestion = true;
            }
            if (questionBank.commonFunctions.checkIfCorrectAnswer("QuestionToQuestionBankExisting") && isNewQuestionChecked) {
                isExistingQuestion = true;
                isNewQuestionRequired = false;
                isNewQuestion = false;
            }
            if (questionBank.commonFunctions.validateQuestionBankFields(questionType, isNewQuestion)) {
                var questionJson = questionBank.commonFunctions.formJsonForQuestion();
                skillSet.commonFunctions.getNewQuestionTextForSkillSetToQB(questionType, isNewQuestion);
                var urlToSaveQn = "../QuestionBank/SaveQuestion?questionUrlReference=" + skillSet.commonFunctions.getUrlReferenceForQuestionSave() + "&folderIdentifier=" + "" + "&isEditMode=" + true + "&isNewQuestion=" + isNewQuestion + "&isExistingQuestion=" + isExistingQuestion + "&questionGuid=" + strParentReferenceGuidOfQuestion + "&authoringType=" + 1 + "&authoringUrl=" + skillSet.commonFunctions.getSkillSetUniqueIdentifier() + "&questionNewTextToSave=" + questionNewTextToSave;
                if (questionJson != null) {
                    startBackgound_Blur();
                    doAjaxCall("POST", questionJson, urlToSaveQn, skillSet.commonFunctions.saveSkillSetQuestionSuccess);
                    closeBackgound_Blur();
                }
            }
            else {
                $("#QuestionTemplateSlide").scrollTo("#validationSummary", 300);
            }
        },
        saveSkillSetQuestionSuccess: function (result) {
            if (result.isQuestionNameAlreadyExists) {
                jAlert(QUESTION_NAME_EXISTS, "Alert", function () {
                    return false;
                });
            }
            else {
                var alertMessageToBeDisplayed = SAVED_MESSAGE;
                if (result.isQuestionSavedToQuestionBank != null) {
                    if (result.isQuestionSavedToQuestionBank) {
                        alertMessageToBeDisplayed = QUESTION_SAVED_MASTER;
                    }
                }
                var questionCount;
                isProceedToStep4Valid = result.isProceedToStep4Valid;
                jAlert(alertMessageToBeDisplayed, "Alert", function (isOk) {
                    if (isOk) {
                        if (result.questionResult != null) {
                            startBackgound_Blur();
                            isContentChanged = false;
                            $("#skillSetContent").load("../SkillSet/LoadQuestionsForSkillSet?skillSetUrl=" + skillSet.commonFunctions.getSkillSetUniqueIdentifier(), function () {
                                closeBackgound_Blur();
                            });
                            for (indexGuid = 0; indexGuid < savedQuestionsCount.count; indexGuid++) {
                                if (strQuestionGuid != savedQuestionsCount[indexGuid].id) {
                                    questionCount = {
                                        id: strQuestionGuid
                                    };
                                    break;
                                }
                            }
                            savedQuestionsCount.push(questionCount);
                        }
                    }
                });
            }
        },
        cancel: function () {
            var status = "Are you sure you want to cancel? Your changes will not be saved.";
            jConfirm(status, 'Cancel', function (isCancel) {
                if (isCancel) {
                    startBackgound_Blur();
                    $("#skillSetContent").load("../SkillSet/LoadQuestionsForSkillSet?skillSetUrl=" + skillSet.commonFunctions.getSkillSetUniqueIdentifier());
                    closeBackgound_Blur();
                }
            });
        },

        loadStep3SkillSet: function (skillSetUrl) {
            strSkillSetUrl = skillSetUrl;

            $("#QuestionTemplateSlide").addClass('question-slide-out-skillset');
            $("#rationaleLinkedCompetency").hide();
            $("#QuestionBankSlideButton").hide();
            $("#questiontosave").hide();

            skillSet.commonFunctions.loadImageForAStep("3");
            $("#content_skillset_changed :input").live('change', function () {
                isContentChanged = true;
            });
            $('#QuestionToQuestionBankNew').click(function () {

                $("#newQuestionDiv").show();
                skillSet.commonFunctions.loadQuestionForNewQuestion();
            });
            $('#QuestionToQuestionBankExisting').click(function () {
                $('#QuestionTextAreaNew').val("");
                $("#newQuestionDiv").hide();
                //isNewQuestionRequired = false;
            });
            if (QuestionList.length > 0) {
                for (var indexSource = 0; indexSource < QuestionList.length; indexSource++) {
                    var questionList = '<div style="height:10px;">&nbsp;</div>' + '<div id="' + QuestionList[indexSource].UniqueIdentifier + '" class="grid_32 clear-background standard-text select-hand"  onclick=skillSet.commonFunctions.contentChanged("' + QuestionList[indexSource].ParentReferenceGuid + '","' + QuestionList[indexSource].UniqueIdentifier + '","' + QuestionList[indexSource].QuestionType + '","' + QuestionList[indexSource].Url + '","' + QuestionList[indexSource].CompetencyReferenceGUID + '")>' + QuestionList[indexSource].QuestionText + '</div>' + '<br />';
                    $("#QuestionList").append(questionList);
                }
                skillSet.commonFunctions.loadQuestions(QuestionList[0].ParentReferenceGuid, QuestionList[0].UniqueIdentifier, QuestionList[0].QuestionType, QuestionList[0].Url, QuestionList[0].CompetencyReferenceGUID);
            }
        },
        contentChanged: function (parentReferenceGuid, divId, questionType, questionUrl, competencyGuid) {
            if (!isContentChanged) {
                skillSet.commonFunctions.loadQuestions(parentReferenceGuid, divId, questionType, questionUrl, competencyGuid);
            } else {
                var status = "Are you sure you want to proceed? Your changes will not be saved.";
                jConfirm(status, 'Proceed', function (isCancel) {
                    if (isCancel) {
                        skillSet.commonFunctions.loadQuestions(parentReferenceGuid, divId, questionType, questionUrl, competencyGuid);
                    }
                });
            }
        },
        addQuestionToQbClickFunction: function () {
            if ($('#AddQuestionToQuestionBank').is(':checked')) {
                $('#QuestionTextAreaNew').val("");
                $("#questiontosave").show();
                if (isNewQuestionRequired) {
                    $("#newQuestionDiv").show();
                    $("#QuestionToQuestionBankNew").attr("checked", true);
                } else {
                    $("#newQuestionDiv").hide();
                }
            }
            else {
                $("#QuestionToQuestionBankExisting").attr("checked", false);
                $("#questiontosave").hide();
                $("#newQuestionDiv").hide();

            }
        },
        loadQuestions: function (parentReferenceGuid, divId, questionType, questionUrl, competencyGuid) {

            strParentReferenceGuidOfQuestion = parentReferenceGuid;
            $(".clear-background").removeClass("question-highlight");
            $("#" + divId).addClass("question-highlight");
            skillSet.commonFunctions.setUrlReferenceForQuestionSave(questionUrl);
            skillSet.commonFunctions.loadQuestionInEditModeForSkillSet(questionUrl, questionType, divId);
            if (isNullOrEmpty(competencyGuid)) {
                $("#QuestionBankLabel").hide();
                $("#AddQuestionToQuestionBank_lbl").html("Add the question to the Question Bank");
                isNewQuestionRequired = false;
                $('#AddQuestionToQuestionBank').unbind("click").bind("click", skillSet.commonFunctions.addQuestionToQbClickFunction);
                $("#newQuestion").hide();
                $("#existingQuestion").hide();
                $("#newQuestionDiv").hide();
            } else {
                $("#QuestionBankLabel").html("Selecting the following option will update the master Question Bank");
                $("#AddQuestionToQuestionBank_lbl").html("Add the edited question to the Question Bank");
                if (isNullOrEmpty(parentReferenceGuid)) {
                    $('#AddQuestionToQuestionBank').unbind("click");
                    isNewQuestionRequired = false;
                } else {
                    isNewQuestionRequired = true;
                    $('#AddQuestionToQuestionBank').unbind("click").bind("click", skillSet.commonFunctions.addQuestionToQbClickFunction);
                }
                $("#newQuestion").show();
                $("#newQuestionDiv").hide();

                $("#existingQuestion").show();
            }
            if ($('#AddQuestionToQuestionBank').is(':checked')) {
                $('#AddQuestionToQuestionBank').attr('checked', false);
                $('#QuestionToQuestionBankExisting').attr('checked', false);
                $("#questiontosave").hide();
                $("#newQuestionDiv").hide();
            }
        },
        proceedToStep4: function () {
            if (isProceedToStep4Valid) {
                skillSet.commonFunctions.loadStep4(skillSet.commonFunctions.getSkillSetUniqueIdentifier());
            }
            else {
                var status = "All question templates must be configured in order to proceed to the next step.";
                jAlert(status, 'Message');
            }
        },
        loadQuestionForNewQuestion: function () {
            switch (questionType) {
                case "8":
                case "7":
                case "6":
                case "3":
                    {
                        $("#newQuestionTextDivLabeling").hide();
                        $("#newQuestionTextDivMultiple").show();
                        $("#newQuestionTextDivFillIn").hide();
                        break;
                    }
                case "5":
                    {
                        $("#newQuestionTextDivLabeling").hide();
                        $("#newQuestionTextDivMultiple").show();
                        $("#newQuestionTextDivFillIn").hide();
                        break;
                    }
                case "4":
                    {
                        $("#newQuestionTextDivLabeling").hide();
                        $("#newQuestionTextDivMultiple").hide();
                        questionBank.commonFunctions.fillInTheBlank.renderDynamicControls(blankOrientation, "newQuestionTextDivFillIn");
                        $("#newQuestionTextDivFillIn").show();
                        break;
                    }
                case "9":
                    {
                        $("#newQuestionTextDivMultiple").hide();
                        $("#newQuestionTextDivLabeling").show();
                        $("#newQuestionTextDivFillIn").hide();
                        break;
                    }
            }
        },

        cancelForskillset: function () {

            var status = "Are you sure you want to cancel? Your changes will not be saved.";
            jConfirm(status, 'Cancel', function (isCancel) {
                if (isCancel) {
                    startBackgound_Blur();
                    $("#skillSetContent").load("../SkillSet/LoadQuestionsForSkillSet?skillSetUrl=" + skillSet.commonFunctions.getSkillSetUniqueIdentifier());
                    closeBackgound_Blur();
                }
            });
        },
        backToStep2: function () {

            if (!isContentChanged) {
                // skillSet.commonFunctions.loadQuestions(divID, questionType, questionUrl, competencyGuid);
            }
            else {
                var status = "Are you sure you want to proceed? Your changes will not be saved.";
                jConfirm(status, 'Proceed', function (isCancel) {
                    if (isCancel) {
                        //  skillSet.commonFunctions.loadQuestions(divID, questionType, questionUrl, competencyGuid);
                    }
                });
            }

        },
        loadLandingPage: function () {
            startAjaxLoader();
            $("#authoringSkillSetBuilder").load("../SkillSet/SkillSetBuilderLanding", function () {
                closeAjaxLoader();
            });
        },
        loadStep1: function (skillSetUrl) {
            startAjaxLoader();
            skillSet.commonFunctions.setSkillSetUniqueIdentifier(skillSetUrl);
            $("#authoringSkillSetBuilder").load("../SkillSet/SkillSetBuilder", function () {
                $("#skillSetContent").load("../SkillSet/LoadCompetenciesForSkillSet?skillSetUrl=" + skillSetUrl, function () {
                    closeAjaxLoader();
                });
            });
        },
        cancelStep1: function () {
            var skillSetUrl = skillSet.commonFunctions.getSkillSetUniqueIdentifier();
            var status = "Are you sure you want to cancel? Your changes will not be saved.";
            jConfirm(status, 'Cancel', function (isCancel) {
                if (isCancel) {
                    skillSet.commonFunctions.loadStep1(skillSetUrl);
                }
            });
        },
        loadStep2: function (skillSetUrl) {
            if (skillSetUrl == undefined || skillSetUrl == "") {
                skillSetUrl = skillSet.commonFunctions.getSkillSetUniqueIdentifier();
            }

            $("#authoringSkillSetBuilder").load("../SkillSet/SkillSetBuilder", function () {
                $("#skillSetContent").load("../SkillSet/LoadSkillSetStepTwo?skillSetUrl=" + skillSetUrl, function () {

                });
            });
        },
        loadStep3: function (skillSetUrl) {
            startBackgound_Blur();
            $("#authoringSkillSetBuilder").load("../SkillSet/SkillSetBuilder", function () {
                $("#skillSetContent").load("../SkillSet/LoadQuestionsForSkillSet?skillSetUrl=" + skillSetUrl, function () {
                    closeBackgound_Blur();
                });
            });
        },
        loadStep4: function (skillSetUrl) {
            startAjaxLoader();
            $("#authoringSkillSetBuilder").load("../SkillSet/SkillSetBuilder", function () {
                $("#skillSetContent").load("../SkillSet/PreviewAndPublishStep4?skillSetUrl=" + skillSetUrl, function () {
                    closeAjaxLoader();
                });
            });
        },
        previewPublish: {
            publish: function () {
                jConfirm(PUBLISH_MESSAGE, ALERT_TITLE, function (isOk) {
                    if (isOk) {
                        startAjaxLoader();
                        var urlpublish = "../SkillSet/PublishASkillSet?skillSetUrl=" + skillSet.commonFunctions.getSkillSetUniqueIdentifier();
                        doAjaxCall("POST", "", urlpublish, skillSet.commonFunctions.previewPublish.publishSuccess);
                    }
                });
            },
            publishSuccess: function (ajaxResult) {
                if (ajaxResult != null && ajaxResult.Result) {
                    jAlert(SKILLSETPUBLISHED_MESSAGE, ALERT_TITLE, function (isOk) {
                        if (isOk) {
                            skillSet.commonFunctions.previewPublish.publishedSettings();
                            closeAjaxLoader();
                        }
                    });
                }
            },
            publishedSettings: function () {
                disableAButton("btnPublish");
                $("#backToStep3SkillSet").hide();
                $("#previewStep4SkillSet").removeClass("prefix_9").addClass("prefix_10");
            },
            backToStep3: function () {
                startBackgound_Blur();
                $("#skillSetContent").load("../SkillSet/LoadQuestionsForSkillSet?skillSetUrl=" + skillSet.commonFunctions.getSkillSetUniqueIdentifier(), function () {
                    closeBackgound_Blur();
                });
            },
            preview: function () { }
        },
        swapQuestions: function (action) {
            var sourceUrl = "";
            var destinationUrl = "";
            var isValid = "";
            startAjaxLoader();
            for (var indexSource = 0; indexSource < QuestionList.length; indexSource++) {
                if (strQuestionGuid == QuestionList[indexSource].UniqueIdentifier) {
                    isValid = ((action == "upArrow" && indexSource != 0) || (action == "downArrow" && indexSource != (QuestionList.length - 1))) ? true : false;
                    if (isValid) {
                        sourceUrl = QuestionList[indexSource].Url;
                        destinationUrl = (action == "upArrow") ? QuestionList[indexSource - 1].Url : QuestionList[indexSource + 1].Url;
                    }
                }
            }
            if (isValid) {
                var urlSwap = "../SkillSet/SwapQuestionsForSkillSet?sourceUrl=" + sourceUrl + "&destinationUrl=" + destinationUrl + "&skillSetUrl=" + strSkillSetUrl;
                doAjaxCall("POST", "", urlSwap, this.successSwap);
            }
            closeAjaxLoader();
        },

        successSwap: function (result) {
//            var sourceIndex = 0;
            if (result.questionList != null) {
                $("#QuestionList").empty();
                //                for (var indexSource = 0; indexSource < result.questionList.length; indexSource++) {
                //                    sourceIndex = (result.strSourceUrl == QuestionList[indexSource].Url) ? indexSource : sourceIndex;
                //                    var questionList = '<div style="height:5px;">&nbsp;</div>' + '<div id="' + QuestionList[indexSource].UniqueIdentifier + '" class="grid_32 clear-background standard-text header-text select-hand"  onclick=skillSet.commonFunctions.contentChanged("' + QuestionList[indexSource].UniqueIdentifier + '","' + QuestionList[indexSource].QuestionType + '","' + QuestionList[indexSource].Url + '","' + QuestionList[indexSource].CompetencyReferenceGUID + '")>' + QuestionList[indexSource].QuestionText + '</div>' + '<br />';
                //                    $("#QuestionList").append(questionList);
                //                }
                //                skillSet.commonFunctions.loadQuestions(result.questionList[sourceIndex].UniqueIdentifier, result.questionList[sourceIndex].QuestionType, result.questionList[sourceIndex].Url, result.questionList[sourceIndex].CompetencyReferenceGUID);
                //                closeAjaxLoader();
                startBackgound_Blur();
                $("#skillSetContent").load("../SkillSet/LoadQuestionsForSkillSet?skillSetUrl=" + skillSet.commonFunctions.getSkillSetUniqueIdentifier(), function () {
                    closeBackgound_Blur();
                });
            }
        }
    },
    SkillSetMetadata:
        {
            SaveSkillSet: function () {

                if (skillSet.SkillSetMetadata.Validate()) {
                    startAjaxLoader();
                    skillSet.SkillSetMetadata.GetSelectedCompetencyList();
                    var skillSetJson = {
                        "SkillSetTitle": encodeSpecialSymbols($.trim($("#SkillSetTitle").val())),
                        "Focus": skillSet.SkillSetMetadata.GetFocusList(),
                        "Competencies": selectedCompetencies_Filter
                    };
                    var skillSetUrl = skillSet.commonFunctions.getSkillSetUniqueIdentifier();
                    var folderIdentifier = getDivData("skillSetBuilderTab", "currentFolderIdentifier");
                    var folderUrl = getDivData("skillSetBuilderTab", "currentFolder");
                    folderIdentifier = (!isNullOrEmpty(folderUrl) && !isNullOrEmpty(folderIdentifier)) ? (folderUrl + "/" + folderIdentifier) : "";
                    var skillSetSaveUrl = "../SkillSet/SaveSkillSet?skillSetUrl=" + (isNullOrEmpty(skillSetUrl) ? "" : skillSetUrl) + "&isEditMode=" + isSkillSetEditMode + "&folderIdentifier=" + folderIdentifier;
                    doAjaxCall("POST", skillSetJson, skillSetSaveUrl, skillSet.SkillSetMetadata.SkillSetSaveSuccessProceed);
                }

            },
            Validate: function () {
                var errorMessage = "<UI>";
                skillSet.SkillSetMetadata.GetSelectedCompetencyList();
                var selectedCompetencyListTemp = selectedCompetencies_Filter;
                skillSet.SkillSetMetadata.HideValidation();
                if (($.trim($("#SkillSetTitle").val()) == "") || (selectedCompetencyListTemp != undefined && selectedCompetencyListTemp.length == 0)) {
                    errorMessage += "<LI>" + INSUFFICIENT_INFO + "</LI>";
                    errorMessage += "</UI>";
                    $("#validationSummary")[0].innerHTML = errorMessage;
                    $("#validationSummary").show();
                    $("#skillSet-Metadata-Main-Content").scrollTop(0);
                    return false;
                }
                return true;
            },
            HideValidation: function () {
                $(".mandatory_field_highlight").removeClass("mandatory_field_highlight");
                $("#validationSummary").empty();
                $("#validationSummary").hide();
            },
            SkillSetSaveSuccess: function (result) {
                skillSet.commonFunctions.setSkillSetUniqueIdentifier(result.uniqueIdentifier);
                isSkillSetEditMode = true;
                if (result.messageToReturn != "") {
                    jAlert("Saved", ALERT_TITLE, function () {
                        skillSet.commonFunctions.loadStep2(skillSet.commonFunctions.getSkillSetUniqueIdentifier());
                    });
                } else {
                    jAlert("Failed");
                }
            },
            SkillSetSaveSuccessProceed: function (result) {
                skillSet.commonFunctions.setSkillSetUniqueIdentifier(result.uniqueIdentifier);
                isSkillSetEditMode = true;
                if (result.messageToReturn != "") {
                    skillSet.commonFunctions.loadStep2(skillSet.commonFunctions.getSkillSetUniqueIdentifier());
                } else {
                    jAlert("Failed");
                }
            },
            SaveOnlySkillSet: function () {
                if (skillSet.SkillSetMetadata.Validate()) {
                    startAjaxLoader();
                    skillSet.SkillSetMetadata.GetSelectedCompetencyList();
                    var skillSetJson = {
                        "SkillSetTitle": encodeSpecialSymbols($.trim($("#SkillSetTitle").val())),
                        "Focus": skillSet.SkillSetMetadata.GetFocusList(),
                        "Competencies": selectedCompetencies_Filter
                    };
                    var skillSetUrl = skillSet.commonFunctions.getSkillSetUniqueIdentifier();
                    var folderIdentifier = getDivData("skillSetBuilderTab", "currentFolderIdentifier");
                    var folderUrl = getDivData("skillSetBuilderTab", "currentFolder");
                    folderIdentifier = (!isNullOrEmpty(folderUrl) && !isNullOrEmpty(folderIdentifier)) ? (folderUrl + "/" + folderIdentifier) : "";
                    var skillSetSaveUrl = "../SkillSet/SaveSkillSet?skillSetUrl=" + (isNullOrEmpty(skillSetUrl) ? "" : skillSetUrl) + "&isEditMode=" + isSkillSetEditMode + "&folderIdentifier=" + folderIdentifier;
                    doAjaxCall("POST", skillSetJson, skillSetSaveUrl, skillSet.SkillSetMetadata.SkillSetOnlySaveSuccess);
                }
            },
            SkillSetOnlySaveSuccess: function (result) {
                skillSet.commonFunctions.setSkillSetUniqueIdentifier(result.uniqueIdentifier);
                isSkillSetEditMode = true;
                if (result.messageToReturn != "") {
                    jAlert("Saved", ALERT_TITLE, function () {
                        closeAjaxLoader();
                    });
                } else {
                    jAlert("Failed");
                }
            },
            GetFocusList: function () {
                var focusList = [];
                $("input[type='checkbox'][name=SkillSetFocus]:checked").each(function () {
                    focusList.push($(this).val());
                });
                return (focusList.length > 0 ? focusList : null);
            },
            AddCompetencies: function () {
                $("#competencyUnselectedList").contents().find("input[name='SelectSkillSetCompetency']:checked").each(function () {
                    $(this).attr('checked', false);
                    $(this).parent().removeClass("competency-highlight");
                    $("#competencySelectedList").append($(this).parent());
                    $("#competencyUnselectedList").removeClass($(this).parent());
                });
            },
            getSelectedCompetencies: function () {
                var lstOfSelectedCompetenciesToRemove = [];
                $("#competencySelectedList").contents().find("input[name='SelectSkillSetCompetency']:checked").each(function () {
                    var competencyGuid = this.id.split("SelectSkillSetCompetency_")[1];
                    lstOfSelectedCompetenciesToRemove.push(competencyGuid);
                });
                return lstOfSelectedCompetenciesToRemove;
            },
            removeAndCloseCompetencyDialog: function () {
                skillSet.SkillSetMetadata.removeAndAppendCompetencies();
                closeAjaxLoader();
                $('#competency_skillSet_load_view_content_view').dialog("close");
            },
            SkillSetRemoveCompetencies: function (resultData) {
                var lstOfQuestions = resultData.lstOfSkillSetQuestion;
                var strQuestionList = resultData.stringOfQuestions;
                if (lstOfQuestions != null && lstOfQuestions.length > 0) {

                    $('#competency_skillSet_load_view_content').html(strQuestionList);

                    $('#competency_skillSet_load_view_content_view').dialog({
                        autoOpen: false,
                        modal: true,
                        closeOnEscape: false,
                        resizable: false,
                        open: function () {
                            applyClassForDialogHeader();
                        },
                        title: 'Message',
                        minHeight: 100,
                        minWidth: 450
                    });
                    $('#competency_skillSet_load_view_content_view').dialog("open");

                } else {
                    jConfirm("Are you sure you want to remove this competency?", ALERT_TITLE, function (isOk) {
                        if (isOk) {
                            skillSet.SkillSetMetadata.removeAndAppendCompetencies();

                        }
                        closeAjaxLoader();
                    });
                }
            },
            RemoveCompetencies: function () {
                if (skillSet.SkillSetMetadata.getSelectedCompetencies() != null && skillSet.SkillSetMetadata.getSelectedCompetencies().length > 0) {
                    if (isSkillSetEditMode) {
                        var removeCompetenciesUrl = "../SkillSet/CheckIfQuestionsPresentForListOfCompetencies?skillSetIdentifier=" + skillSet.commonFunctions.getSkillSetUniqueIdentifier();
                        doAjaxCall("POST", skillSet.SkillSetMetadata.getSelectedCompetencies(), removeCompetenciesUrl, skillSet.SkillSetMetadata.SkillSetRemoveCompetencies);
                    }
                    else {
                        skillSet.SkillSetMetadata.removeAndAppendCompetencies();
                    }
                }
            },
            removeAndAppendCompetencies: function () {
                $("#competencySelectedList").contents().find("input[name='SelectSkillSetCompetency']:checked").each(function () {
                    $(this).attr('checked', false);
                    $(this).parent().removeClass("competency-highlight");
                    $("#competencyUnselectedList").append($(this).parent());
                    $("#competencySelectedList").removeClass($(this).parent());
                });
            },
            RefreshCompetencySearch: function () {
//                var abc = "";
                skillSet.SkillSetMetadata.GetSelectedCompetencyList();
                var isSearchValid = true;
                if (isNullOrEmpty(competencySearchText_Filter)) {
                    if (!(competencySearchText_Filter == "" && isSearchPage)) {
                        isSearchValid = false;
                        jAlert(SEARCH_VALIDATION, ALERT_TITLE, function () {
                        });
                    }
                }
                if (isSearchValid) {
                    var skillSetProxyObj = {
                        "FilterSourceList": selectedSources_Filter,
                        "SelectedCompetencyList": selectedCompetencies_Filter,
                        "competencyText": competencySearchText_Filter
                    };
                    var skillSetSaveUrl = "../SkillSet/GetFilteredCompetencyList";
                    doAjaxCall("POST", skillSetProxyObj, skillSetSaveUrl, skillSet.SkillSetMetadata.SearchSuccess);
                }
            },
            RefreshCompetencyCheckbox: function () {
                skillSet.SkillSetMetadata.GetSelectedCompetencyList();
                selectedSources_Filter = [];
                $("input[type=checkbox][name='SkillSetSources']:checked").each(function () {
                    selectedSources_Filter.push($(this).val());
                });
                var skillSetProxyObj = {
                    "FilterSourceList": selectedSources_Filter,
                    "SelectedCompetencyList": selectedCompetencies_Filter,
                    "competencyText": competencySearchText_Filter
                };
                var skillSetSaveUrl = "../SkillSet/GetFilteredCompetencyList";
                doAjaxCall("POST", skillSetProxyObj, skillSetSaveUrl, skillSet.SkillSetMetadata.SearchSuccess);
            },
            SearchSuccess: function (result) {
                if (result.filteredCompetencyList != null) {
                    skillSet.SkillSetMetadata.LoadUnselectedCompetencies(result.filteredCompetencyList);
                }
            },
            //            loadSKillSetInEditMode: function (skillSetUrl) {
            //                startAjaxLoader();
            //                $("#authoringSkillSetBuilder").empty();
            //                $("#authoringSkillSetBuilder").load("../SkillSet/LoadCompetenciesForSkillSet?skillSetUrl=" + skillSetUrl);
            //                closeAjaxLoader();
            //            },
            GetSelectedCompetencyList: function () {
                selectedCompetencies_Filter = [];
                //$("#competencySelectedList").contents().find("div[id^='skillSetCompetency']").each(function () {
                $("#competencySelectedList > div[id^='skillSetCompetency']").each(function () {
                    selectedCompetencies_Filter.push(this.id.split('_')[1]);
                });
            },
            LoadUnselectedCompetencies: function (unselectedCompetenciesList) {
                var competencyDivs = "";
                if (unselectedCompetenciesList != null && unselectedCompetenciesList.length > 0) {
                    for (var compCount = 0; compCount < unselectedCompetenciesList.length; compCount++) {
                        competencyDivs += "<div class='grid_32 bottom-only-border' id='skillSetCompetency_" + unselectedCompetenciesList[compCount].id + "'><input type='checkbox' name='SelectSkillSetCompetency' class='grid_2' id='SelectSkillSetCompetency_" + unselectedCompetenciesList[compCount].id + "' onclick='skillSet.SkillSetMetadata.HighlightCompetency(this);'/><div class='grid_28 align-div-inline'><label class='dual-list align' for='SelectSkillSetCompetency_" + unselectedCompetenciesList[compCount].id + "'>" + unselectedCompetenciesList[compCount].name + "</label></div><div class='clear-height-spacing clear'></div></div>";
                    }
                }
                $("#competencyUnselectedList").empty();
                if (competencyDivs != "") {
                    $("#competencyUnselectedList").append(competencyDivs);
                }
            },
            LoadSelectedCompetencies: function (selectedCompetenciesList) {
                var competencyDivs = "";
                if (selectedCompetenciesList != null && selectedCompetenciesList.length > 0) {
                    for (var compCount = 0; compCount < selectedCompetenciesList.length; compCount++) {
                        competencyDivs += "<div class='grid_32 bottom-only-border' id='skillSetCompetency_" + selectedCompetenciesList[compCount].id + "'><input type='checkbox' name='SelectSkillSetCompetency' class='grid_2' id='SelectSkillSetCompetency_" + selectedCompetenciesList[compCount].id + "' onclick='skillSet.SkillSetMetadata.HighlightCompetency(this);'/><div class='grid_28 align-div-inline'><label class='dual-list align' for='SelectSkillSetCompetency_" + selectedCompetenciesList[compCount].id + "'>" + selectedCompetenciesList[compCount].name + "</label></div><div class='clear-height-spacing clear'></div></div>";
                    }
                }
                $("#competencySelectedList").empty();
                if (competencyDivs != "") {
                    $("#competencySelectedList").append(competencyDivs);
                }
            },
            HighlightCompetency: function (obj) {
                if ($(obj).is(":checked")) {
                    $(obj).parent().addClass("competency-highlight");
                }
                else {
                    $(obj).parent().removeClass("competency-highlight");
                }
            },
            OnPageLoad: function () { }
        },

    stepTwoSkillStructure: {
        getFlexBoxForFilterByQuestionType: function () {
            var urlQuestionTemplate = "../QuestionBank/GetQuestionType";
            doAjaxCall("POST", "", urlQuestionTemplate, this.successFlexBoxForFilterByQuestionType);
        },
        getListOfCompetencyForSkillSetFlexBox: function () {
            var urlForString = "../SkillSet/GetCompetencyForSkillSetFlexBox?uniqueIdentifier=" + skillSet.commonFunctions.getSkillSetUniqueIdentifier();
            doAjaxCall("POST", "", urlForString, this.successCompetencyFetch);

        },
        successFlexBoxForFilterByQuestionType: function (result) {
            if (result.questionList != null) {
                //competencyArray = result.questionNameArray;
                var resultList = eval('(' + result.questionList + ')');
                questionTypeList.results = resultList;
                questionTypeList.total = questionTypeList.results.length;
                skillSet.stepTwoSkillStructure.initializeFlexBoxForFilterByQuestionType(questionTypeList);
            }
            return null;

        },
        successCompetencyFetch: function (result) {
            if (result.competencyStringListTemp != null) {
                //competencyArray = result.competencyArray;
                competencyQuestionStringListTemp.results = eval('(' + result.competencyStringListTemp + ')');
                competencyQuestionStringListTemp.total = competencyQuestionStringListTemp.results.length;
                skillSet.stepTwoSkillStructure.initializeFlexBoxForCompetencySkill(competencyQuestionStringListTemp);
            }

            return null;
        },
        initializeFlexBoxForFilterByQuestionType: function (lstOfItems) {
            var divId = "FilterByQuestionType";
            $('#' + divId).flexbox(lstOfItems, {
                resultTemplate: '{name}',
                width: 175,
                paging: false,
                maxVisibleRows: 10,
                noResultsText: '',
                noResultsClass: '',
                matchClass: '',
                matchAny: true,
                onSelect: function () {
                    startAjaxLoader();
                    selectedQuestionTypeFilterText = $('#' + divId).val();
                    skillSet.stepTwoSkillStructure.getQuestionBank();
                }
            });
            $('#' + divId + "_ctr").attr("style", "left: 0px;top: 22px;width: 175px;");
            $('#' + divId + "_ctr").hide();
            $('#' + divId + "_input").watermark(WATER_MARK_COMPETENCY_STEPTWO, { className: 'watermark watermark-text' });
            $('#' + divId + '').live('blur', function () {
                if ($('#' + divId + '_input').val() == "") {
                    $('#' + divId + '_input').text = WATER_MARK_COMPETENCY_STEPTWO;
                    $('#' + divId + '_input').addClass('watermark watermark-text');
                } else {
                    $('#' + divId + '_input').removeClass('watermark watermark-text');
                }
            });
            $('#' + divId + '_input').live('keyup', function () {
                if ($('#FilterByQuestionType').val() == "") {
                    startAjaxLoader();
                    skillSet.stepTwoSkillStructure.getQuestionBank();
                    closeAjaxLoader();
                }
            });
        },
        initializeFlexBoxForCompetencySkill: function (lstOfItems) {
            var divId = "LinkedCompetencyQuestion";
            $('#' + divId).flexbox(lstOfItems, {
                resultTemplate: '{name}',
                width: 175,
                paging: false,
                //maxVisibleRows: 10,
                noResultsText: '',
                noResultsClass: '',
                matchClass: '',
                matchAny: true,
                onSelect: function () {
                    startAjaxLoader();
                    isCompetencyFilterValueSelected = true;
                    competencyQuestionSearchFiltertext = $('#' + divId + "_hidden").val();
                    skillSet.stepTwoSkillStructure.getQuestionBank();
                }
            });
            $('#' + divId + "_ctr").attr("style", "left: 0px;top: 22px;width: 175px;max-height:150px;overflow-y:auto");
            $('#' + divId + "_ctr").hide();
            $('#' + divId + "_input").watermark(WATER_MARK_COMPETENCY, { className: 'watermark watermark-text' });
            $('#' + divId + '').live('blur', function () {
                if ($('#' + divId + '_input').val() == "") {
                    $('#' + divId + '_input').text = WATER_MARK_COMPETENCY;
                    $('#' + divId + '_input').addClass('watermark watermark-text');
                    isCompetencyFilterValueSelected = false;
                } else {
                    $('#' + divId + '_input').removeClass('watermark watermark-text');
                }
            });
            $('#' + divId + '_input').live('keyup', function () {
                if (isCompetencyFilterValueSelected) {
                    if ($('#' + divId + '_input').val() == "") {
                        startAjaxLoader();
                        skillSet.stepTwoSkillStructure.getQuestionBank();
                        closeAjaxLoader();
                    }
                }
            });
        },
        getQuestionTemplate: function () {
            var urlQuestionTemplate = "../QuestionBank/GetQuestionType";
            doAjaxCall("POST", "", urlQuestionTemplate, this.successQuestionTemplate);
        },
        successQuestionTemplate: function (result) {
            skillSet.stepTwoSkillStructure.LoadQuestionTemplate(result.ResultList);
        },
        LoadQuestionTemplate: function (questionTemplateList) {
            if (questionTemplateList != undefined) {
                $("#loadQuestionTemplate").empty();
                for (var index = 0; index < questionTemplateList.length; index++) {
                    $("#loadQuestionTemplate").append("<option id='questionTemplate_" + questionTemplateList[index].id + "' TypeOfQuestion = '" + questionTemplateList[index].name + "' QuestionTypeId = '" + questionTemplateList[index].id + "'>" + "  " + questionTemplateList[index].name + "</option>");
                }
            }
        },
        getQuestionBank: function () {
            skillSet.stepTwoSkillStructure.RefreshCompetencyQuestionSearch();
        },
        successQuestionBank: function (result) {

            skillSet.stepTwoSkillStructure.LoadQuestionBank(result);
        },
        LoadQuestionBank: function (skillSetQuestionList) {
            if (skillSetQuestionList != undefined) {
                if (skillSetQuestionList.CompetencyQuestionList != undefined) {
                    questionBankCompetencyList = skillSetQuestionList.CompetencyQuestionList;
                    skillSet.stepTwoSkillStructure.loadCompetencyQuestionBankList(skillSetQuestionList.CompetencyQuestionList, false);
                }
                //                if (skillSetQuestionList.SkillQuestionList != undefined) {
                //                    selectQuestionOrderList = skillSetQuestionList.SkillQuestionList;
                //                    skillSet.stepTwoSkillStructure.loadSkillQuestionList(skillSetQuestionList.SkillQuestionList);
                //                }
            }
        },
        loadCompetencyQuestionBankList: function (competencyQuestionList, isFromRemove) {
            var placeHolderId = "#loadQuestionBankCheckBoxDiv";
            var controlName = "competencyQuestionBankCheckBox";
            if (!isFromRemove) {
                $(placeHolderId).empty();
            }
            if (competencyQuestionList != null && competencyQuestionList.length > 0) {
                for (var compCount = 0; compCount < competencyQuestionList.length; compCount++) {
                    var valueId = "competencyQuestionBankCheckBox_" + competencyQuestionList[compCount].UniqueIdentifier;
                    var strValue = competencyQuestionList[compCount].Text;
//                    var checkedStatusBool = false;
                    var comptencyQuestion = "";
                    comptencyQuestion += "<div class='grid_32 bottom-only-border' id='skillSetCompetencyQuestion_" + competencyQuestionList[compCount].UniqueIdentifier + "' >";
                    comptencyQuestion += "<input type='checkbox' IsQuestionFromTemplate='false' OrderSequenceNumber='' ParentReferenceGuid='" + competencyQuestionList[compCount].ParentReferenceGuid + "' TypeOfQuestion='" + competencyQuestionList[compCount].TypeOfQuestion + "' UniqueIdentifier='" + competencyQuestionList[compCount].UniqueIdentifier + "' name='" + controlName + "' id='" + valueId + "' value ='" + strValue + "' Url='" + competencyQuestionList[compCount].Url + "' class='grid_2' onclick='skillSet.SkillSetMetadata.HighlightCompetency(this);' />";
                    comptencyQuestion += "<div class='grid_28 align-div-inline'><label for='" + valueId + "'>" + strValue + "</label></div>";
                    comptencyQuestion += "<div class='clear-height-spacing clear'></div></div>";
                    $(placeHolderId).append(comptencyQuestion);
                }
                closeAjaxLoader();
            }
            else {
                $(placeHolderId).empty();
                closeAjaxLoader();
            }

        },
        loadSkillQuestionList: function () {
        },
        addQuestion: function () {
            skillSet.stepTwoSkillStructure.GetQuestionOrderList();
            var selectedQuestionOrderCount = selectQuestionOrderList.length;
            this.GetSelectedQuestionTemplate();
            if (selectedQuestionTemplateList != []) {
                for (var indexTemplate = 0; indexTemplate < selectedQuestionTemplateList.length; indexTemplate++) {
                    $("#loadQuestionOrderList").append("<option IsQuestionFromTemplate='true' TemplateSequenceNumber='" + selectedQuestionTemplateList[indexTemplate].TemplateSequenceNumber + "' OrderSequenceNumber='" + (++selectedQuestionOrderCount) + "'  TypeOfQuestion = '" + selectedQuestionTemplateList[indexTemplate].TypeOfQuestion + "' QuestionTypeId='" + selectedQuestionTemplateList[indexTemplate].QuestionTypeId + "'  UniqueIdentifier = '" + selectedQuestionTemplateList[indexTemplate].QuestionTypeId + "' text='" + selectedQuestionTemplateList[indexTemplate].QuestionTypeId + "' title='" + selectedQuestionTemplateList[indexTemplate].QuestionTemplateValue + "'>" + selectedQuestionTemplateList[indexTemplate].QuestionTemplateValue + "</option>");
                }
            }
            this.ClearSelectedQuestionTemplate();
            this.GetSelectedCompetencyQuestionCheckBox();
            if (selectedCompetencyQuestion_Filter != []) {
                for (var index = 0; index < selectedCompetencyQuestion_Filter.length; index++) {
                    $("#loadQuestionOrderList").append("<option IsQuestionFromTemplate='false' ParentReferenceGuid='" + selectedCompetencyQuestion_Filter[index].ParentReferenceGuid + "' Url='" + selectedCompetencyQuestion_Filter[index].Url + "' OrderSequenceNumber='" + (++selectedQuestionOrderCount) + "' TypeOfQuestion ='" + selectedCompetencyQuestion_Filter[index].TypeOfQuestion + "' UniqueIdentifier = '" + selectedCompetencyQuestion_Filter[index].UniqueIdentifier + "' text='" + selectedCompetencyQuestion_Filter[index].Text + "' title='" + selectedCompetencyQuestion_Filter[index].Text + "'>" + selectedCompetencyQuestion_Filter[index].Text + "</option>");
                }

                skillSet.stepTwoSkillStructure.RemoveCheckedQuestionCompetencies();
            }
        },
        RemoveCheckedQuestionCompetencies: function () {
            $("#loadQuestionBankCheckBoxDiv").contents().find("input[name='competencyQuestionBankCheckBox']:checked").each(function () {
                $(this).attr('checked', false);
                $(this).parent().removeClass("competency-highlight");
                $("#loadQuestionBankCheckBoxDiv #" + $(this).parent()[0].id).remove();
            });

        },
        RemoveQuestionOrderList: function () {
            var appenToCheckBoxQuestionFromSelectList = [];
            $("#loadQuestionOrderList > option:selected").each(function () {
                var isFromQuestionTemplate = eval(this.getAttribute("IsQuestionFromTemplate"));
                var uniqueIdentifier = this.getAttribute("UniqueIdentifier");
                var typeOfQuestion = this.getAttribute("TypeOfQuestion");
                var parentReferenceGuid = this.getAttribute("ParentReferenceGuid");
                var textValue = this.value;
                var url = this.getAttribute("Url");
                if (url != "" && !isFromQuestionTemplate) {
                    var removedQuestion = {
                        UniqueIdentifier: uniqueIdentifier,
                        TypeOfQuestion: typeOfQuestion,
                        Text: textValue,
                        ParentReferenceGuid: parentReferenceGuid,
                        Url: url
                    };
                    appenToCheckBoxQuestionFromSelectList.push(removedQuestion);
                }
                $(this).remove();
            });
            if (appenToCheckBoxQuestionFromSelectList.length > 0) {
                skillSet.stepTwoSkillStructure.loadCompetencyQuestionBankList(appenToCheckBoxQuestionFromSelectList, true);
            }
            skillSet.stepTwoSkillStructure.RearrangeQuestionOrderSequenceNumber();
        },
        RearrangeQuestionOrderSequenceNumber: function () {
            var selectedQuestionOrderCount = 0;
            $('select#loadQuestionOrderList').find('option').each(function () {
                $(this).attr({
                    OrderSequenceNumber: ++selectedQuestionOrderCount
                });
            });
        },
        ClearSelectedQuestionTemplate: function () {
            $("#loadQuestionTemplate option:selected").removeAttr("selected");
        },
        ClearSelectedQuestionBankCheckBox: function () {
            // to do 
        },
        GetMaxNumberQuestionType: function (questionTypeId) {
            var isMatchFound = false;
            var maxQuestionTypeId = 1;
            if (selectQuestionOrderList.length > 0) {
                for (var rowIndex = 0; rowIndex < selectQuestionOrderList.length; rowIndex++) {
                    if (eval(selectQuestionOrderList[rowIndex].IsQuestionFromTemplate) && questionTypeId == eval(selectQuestionOrderList[rowIndex].QuestionTypeId)) {
                        if (eval(maxQuestionTypeId) <= eval(selectQuestionOrderList[rowIndex].TemplateSequenceNumber)) {
                            isMatchFound = true;
                            maxQuestionTypeId = selectQuestionOrderList[rowIndex].TemplateSequenceNumber;
                        }
                    }
                }
                return (isMatchFound) ? ++maxQuestionTypeId : maxQuestionTypeId;
            }
            return maxQuestionTypeId;
        },
        GetQuestionOrderList: function () {
            selectQuestionOrderList = [];
            var orderSequenceNumberCount = 0;
            $('select#loadQuestionOrderList').find('option').each(function () {
                var isQuestionFromTemplate = eval(this.getAttribute("IsQuestionFromTemplate"));
                var questionTypeId = "";
//                var TypeOfQuestion = this.getAttribute("TypeOfQuestion");
                if (isQuestionFromTemplate) {
                    questionTypeId = this.getAttribute("QuestionTypeId");
                }
                var templateSequenceNumber = this.getAttribute("TemplateSequenceNumber");
                if (templateSequenceNumber == null || templateSequenceNumber == "") {
                    templateSequenceNumber = "0";
                }
                var selectedQuestionOrder = {
                    "Text": $(this).val(),
                    "IsQuestionFromTemplate": isQuestionFromTemplate,
                    "TypeOfQuestion": questionTypeId,
                    "QuestionTypeId": questionTypeId,
                    "UniqueIdentifier": this.getAttribute("UniqueIdentifier"),
                    "OrderSequenceNumber": ++orderSequenceNumberCount,
                    "TemplateSequenceNumber": templateSequenceNumber,
                    "ParentReferenceGuid": this.getAttribute("ParentReferenceGuid"),
                    "Url": this.getAttribute("Url")
                };
                selectQuestionOrderList.push(selectedQuestionOrder);
            });
        },
        LoadQuestionOrderList: function () {
            $('select#loadQuestionOrderList option').each(function (index, option) {
                $(option).remove();
            });
            var selectedOption = "";
            for (var qusOrderCount = 0; qusOrderCount < selectQuestionOrderList.length; qusOrderCount++) {
                if (selectedQuestionDestinationOrderSeqNo != "" && selectQuestionOrderList[qusOrderCount].OrderSequenceNumber == selectedQuestionDestinationOrderSeqNo) {
                    selectedOption = "selected";
                }
                if (eval(selectQuestionOrderList[qusOrderCount].IsQuestionFromTemplate)) {
                    if (selectedOption != "") {
                        $("#loadQuestionOrderList").append("<option selected='" + selectedOption + "' IsQuestionFromTemplate='true' Url='" + selectQuestionOrderList[qusOrderCount].Url + "'  TemplateSequenceNumber='" + selectQuestionOrderList[qusOrderCount].TemplateSequenceNumber + "' OrderSequenceNumber='" + selectQuestionOrderList[qusOrderCount].OrderSequenceNumber + "' TypeOfQuestion='" + selectQuestionOrderList[qusOrderCount].TypeOfQuestion + "' QuestionTypeId='" + selectQuestionOrderList[qusOrderCount].QuestionTypeId + "' ParentReferenceGuid='" + selectQuestionOrderList[qusOrderCount].ParentReferenceGuid + "'  UniqueIdentifier = '" + selectQuestionOrderList[qusOrderCount].QuestionTypeId + "' Text='" + selectQuestionOrderList[qusOrderCount].Text + "' title='" + selectQuestionOrderList[qusOrderCount].Text + "' >" + selectQuestionOrderList[qusOrderCount].Text + "</option>");
                    } else {
                        $("#loadQuestionOrderList").append("<option IsQuestionFromTemplate='true' Url='" + selectQuestionOrderList[qusOrderCount].Url + "'   TemplateSequenceNumber='" + selectQuestionOrderList[qusOrderCount].TemplateSequenceNumber + "' OrderSequenceNumber='" + selectQuestionOrderList[qusOrderCount].OrderSequenceNumber + "' TypeOfQuestion='" + selectQuestionOrderList[qusOrderCount].TypeOfQuestion + "' QuestionTypeId='" + selectQuestionOrderList[qusOrderCount].QuestionTypeId + "' ParentReferenceGuid='" + selectQuestionOrderList[qusOrderCount].ParentReferenceGuid + "'  UniqueIdentifier = '" + selectQuestionOrderList[qusOrderCount].QuestionTypeId + "' Text='" + selectQuestionOrderList[qusOrderCount].Text + "' title='" + selectQuestionOrderList[qusOrderCount].Text + "' >" + selectQuestionOrderList[qusOrderCount].Text + "</option>");
                    }
                }
                else {
                    if (selectedOption != "") {
                        $("#loadQuestionOrderList").append("<option selected='" + selectedOption + "' IsQuestionFromTemplate='false' Url='" + selectQuestionOrderList[qusOrderCount].Url + "' OrderSequenceNumber='" + selectQuestionOrderList[qusOrderCount].OrderSequenceNumber + "' TypeOfQuestion ='" + selectQuestionOrderList[qusOrderCount].TypeOfQuestion + "' ParentReferenceGuid='" + selectQuestionOrderList[qusOrderCount].ParentReferenceGuid + "'  UniqueIdentifier = '" + selectQuestionOrderList[qusOrderCount].UniqueIdentifier + "' Text='" + selectQuestionOrderList[qusOrderCount].Text + "' title='" + selectQuestionOrderList[qusOrderCount].Text + "' >" + selectQuestionOrderList[qusOrderCount].Text + "</option>");
                    } else {
                        $("#loadQuestionOrderList").append("<option IsQuestionFromTemplate='false' Url='" + selectQuestionOrderList[qusOrderCount].Url + "' OrderSequenceNumber='" + selectQuestionOrderList[qusOrderCount].OrderSequenceNumber + "' TypeOfQuestion ='" + selectQuestionOrderList[qusOrderCount].TypeOfQuestion + "' ParentReferenceGuid='" + selectQuestionOrderList[qusOrderCount].ParentReferenceGuid + "' UniqueIdentifier = '" + selectQuestionOrderList[qusOrderCount].UniqueIdentifier + "' Text='" + selectQuestionOrderList[qusOrderCount].Text + "' title='" + selectQuestionOrderList[qusOrderCount].Text + "' >" + selectQuestionOrderList[qusOrderCount].Text + "</option>");
                    }
                }
                selectedOption = "";
            }
        },
        skillStructureSwapQuestions: function (action) {
            skillSet.stepTwoSkillStructure.GetQuestionOrderList();
            if (selectQuestionOrderList != [] && selectQuestionOrderList.length > 0) {
                var selectQuestionOrderCount = selectQuestionOrderList.length;
                var isMultleSelect = false;
                var isAnyOneSelected = false;
                var multileCount = 0;

                $("#loadQuestionOrderList > option:selected").each(function () {
                    isAnyOneSelected = true;
                    if (multileCount == 0) {
                        var isQuestionFromTemplate = eval(this.getAttribute("IsQuestionFromTemplate"));
                        var questionTypeId = "";
                        var typeOfQuestion = this.getAttribute("TypeOfQuestion");
                        if (isQuestionFromTemplate) {
                            questionTypeId = this.getAttribute("QuestionTypeId");
                        }
                        selectedQuestionSwap = {
                            "Text": $(this).val(),
                            "IsQuestionFromTemplate": isQuestionFromTemplate,
                            "QuestionTypeId": questionTypeId,
                            "TypeOfQuestion": typeOfQuestion,
                            "UniqueIdentifier": this.getAttribute("UniqueIdentifier"),
                            "ParentReferenceGuid": this.getAttribute("ParentReferenceGuid"),
                            "OrderSequenceNumber": this.getAttribute("OrderSequenceNumber"),
                            "TemplateSequenceNumber": this.getAttribute("TemplateSequenceNumber"),
                            "Url": this.getAttribute("Url")
                        };
                    }
                    multileCount = multileCount + 1;
                });
                (multileCount > 1) ? isMultleSelect = true : isMultleSelect = false;
                if (isAnyOneSelected && !isMultleSelect) {
                    var selectedIndexOrderSequenceNumber = "";
                    if (action == "downArrow") {
                        if (selectedQuestionSwap.OrderSequenceNumber != selectQuestionOrderCount) {
                            selectedIndexOrderSequenceNumber = selectQuestionOrderList[(selectedQuestionSwap.OrderSequenceNumber) - 1].OrderSequenceNumber;
                            selectQuestionOrderList[(selectedQuestionSwap.OrderSequenceNumber) - 1].OrderSequenceNumber = selectedIndexOrderSequenceNumber + 1;
                            selectedQuestionDestinationOrderSeqNo = selectedIndexOrderSequenceNumber + 1;
                            selectQuestionOrderList[(selectedIndexOrderSequenceNumber + 1) - 1].OrderSequenceNumber = selectQuestionOrderList[selectedIndexOrderSequenceNumber - 1].OrderSequenceNumber - 1;
                            skillSet.stepTwoSkillStructure.SortQuestionOrderList();
                        }
                    }
                    else if (action == "upArrow") {
                        if (selectedQuestionSwap.OrderSequenceNumber != 1) {
                            selectedIndexOrderSequenceNumber = selectQuestionOrderList[(selectedQuestionSwap.OrderSequenceNumber) - 1].OrderSequenceNumber;
                            selectQuestionOrderList[(selectedQuestionSwap.OrderSequenceNumber) - 1].OrderSequenceNumber = selectedIndexOrderSequenceNumber - 1;
                            selectedQuestionDestinationOrderSeqNo = selectedIndexOrderSequenceNumber - 1;
                            selectQuestionOrderList[(selectedIndexOrderSequenceNumber - 1) - 1].OrderSequenceNumber = selectQuestionOrderList[selectedIndexOrderSequenceNumber - 1].OrderSequenceNumber + 1;
                            skillSet.stepTwoSkillStructure.SortQuestionOrderList();
                        }
                    }
                }
            }

        },
        SortQuestionOrderList: function () {
            selectQuestionOrderList.sort(function (a, b) {
                var a1 = a.OrderSequenceNumber, b1 = b.OrderSequenceNumber;
                if (a1 == b1) return 0;
                return a1 > b1 ? 1 : -1;
            });
            skillSet.stepTwoSkillStructure.LoadQuestionOrderList();
        },
        GetSelectedQuestionTemplate: function () {
            selectedQuestionTemplateList = [];
            $("#loadQuestionTemplate > option:selected").each(function () {
                var questionTypeId = this.getAttribute("QuestionTypeId");
                var typeOfQuestion = this.getAttribute("TypeOfQuestion");
                var templateSequenceNumber = skillSet.stepTwoSkillStructure.GetMaxNumberQuestionType(questionTypeId);
                var selectedQuestion = {
                    "QuestionTypeId": questionTypeId,
                    "QuestionTemplateValue": this.value + " Template " + templateSequenceNumber,
                    "TemplateSequenceNumber": templateSequenceNumber,
                    "TypeOfQuestion": typeOfQuestion
                };
                selectedQuestionTemplateList.push(selectedQuestion);
            });

        },
        SaveSkillStructure: function (action) {
            if (skillSet.stepTwoSkillStructure.Validate()) {
                if (isSkillStructureContentChange) {
                    var skillSetUniqueIdentifier = skillSet.commonFunctions.getSkillSetUniqueIdentifier();
                    var urlSaveSkillStructure = "../SkillSet/SaveSkillStructure?uniqueIdentifierUrl=" + skillSetUniqueIdentifier;
                    if (action == "1") {
                        startAjaxLoader();
                        doAjaxCall("POST", selectQuestionOrderList, urlSaveSkillStructure, this.successSaveSkillStructure1);
                        closeAjaxLoader();
                    }
                    if (action == "2") {
                        doAjaxCall("POST", selectQuestionOrderList, urlSaveSkillStructure, this.successSaveSkillStructure2);
                    }
                }
                else {
                    skillSet.commonFunctions.loadStep3(skillSet.commonFunctions.getSkillSetUniqueIdentifier());
                }
            }

        },
        Validate: function () {
            var errorMessage = "<UI>";
            skillSet.stepTwoSkillStructure.GetQuestionOrderList();
            skillSet.stepTwoSkillStructure.HideValidation();
            if ((selectQuestionOrderList.length == 0)) {
                errorMessage += "<LI>" + ATLEAST_ONE_VALIDATION + "</LI>";
                errorMessage += "</UI>";
                $("#validationSummary")[0].innerHTML = errorMessage;
                $("#validationSummary").show();
                $("#SkillSetBuilderSkillStructureDiv").scrollTop(0);
                return false;
            }
            return true;
        },
        HideValidation: function () {
            $(".mandatory_field_highlight").removeClass("mandatory_field_highlight");
            $("#validationSummary").empty();
            $("#validationSummary").hide();
        },
        successSaveSkillStructure1: function () {
            if (true) {
                jAlert("Saved", ALERT_TITLE, function () {
                    skillSet.commonFunctions.loadStep2(skillSet.commonFunctions.getSkillSetUniqueIdentifier());
                });
            }
        },
        successSaveSkillStructure2: function () {
            if (true) {
                skillSet.commonFunctions.loadStep3(skillSet.commonFunctions.getSkillSetUniqueIdentifier());
            }
        },
        GetSelectedCompetencyQuestionCheckBox: function () {
            selectedCompetencyQuestion_Filter = [];
            $("input[type=checkbox][name^='competencyQuestionBankCheckBox']:checked").each(function () {
                var selectedQuestion = {
                    //"QuestionGuid": this.id.split('_')[1],
                    "UniqueIdentifier": this.getAttribute("UniqueIdentifier"),
                    "TypeOfQuestion": this.getAttribute("TypeOfQuestion"),
                    "Text": this.value,
                    "Url": this.getAttribute("Url"),
                    "IsQuestionFromTemplate": eval(this.getAttribute("IsQuestionFromTemplate")),
                    "OrderSequenceNumber": this.getAttribute("OrderSequenceNumber"),
                    "ParentReferenceGuid": this.getAttribute("ParentReferenceGuid")
                };
                selectedCompetencyQuestion_Filter.push(selectedQuestion);
            });
        },
        RefreshCompetencyQuestionSearch: function () {
            selectedQuestionTypeFilterText = $("#FilterByQuestionType_input").val();
            competencyQuestionSearchFiltertext = $("#LinkedCompetencyQuestion_input").val() != "" ? $("#LinkedCompetencyQuestion_hidden").val() : "";
            //skillSet.stepTwoSkillStructure.GetSelectedCompetencyQuestionList(); // this is for question order list to do 
            var skillSetUniqueIdentifier = skillSet.commonFunctions.getSkillSetUniqueIdentifier();
//            var selectedQuestionType = ""; // to do fiter by type
            var skillSetQuestionProxyObj = {
                "UniqueIdentifier": skillSetUniqueIdentifier,
                "FilterQuestionType": selectedQuestionTypeFilterText,
                "Questions": selectQuestionOrderList,
                "competencyText": competencyQuestionSearchFiltertext
            };
            var urlQuestionBank = "../SkillSet/GetJsonCompetencyQuestionListInSkillSet";
            doAjaxCall("POST", skillSetQuestionProxyObj, urlQuestionBank, this.successQuestionBank);
        },
        cancelForskillsetStructure: function () {
            var status = "Are you sure you want to cancel? Your changes will not be saved.";
            jConfirm(status, 'Cancel', function (isCancel) {
                if (isCancel) {
                    startAjaxLoader();
                    skillSet.commonFunctions.loadStep2(skillSet.commonFunctions.getSkillSetUniqueIdentifier());
                    closeAjaxLoader();
                }
            });
        }
    }
};
