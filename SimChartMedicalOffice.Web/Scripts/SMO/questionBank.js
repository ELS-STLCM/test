var questionTypeLoaded = -1;
var qnOrAnsImageRefIdNameTemp = "";
var maxLenghtOfAnswers = -1;
var qnGuidInEditMode = "";
var isEditMode = false;
var isLinkedCompetencyselected = false;
var strUrlReferenceForSave = "";
var lstCompetency;
var strListOfSelectedQuestionItems = [];
var competencyStringListTemp = {};
var competencyArray;
var blankCount = 0;
var textCount = 0;
var isSearchPage = false;
var questionNewTextToSave;
var questionBank = {
    /*Common functions used in question bank module*/
    commonFunctions: {
        /*Search actions*/
        loadFolder: function (folderUrl, folderIdentifier) {
            setDivData($("#questionBankTab"), "currentFolder", folderUrl);
            setDivData($("#questionBankTab"), "currentFolderIdentifier", folderIdentifier);
            startAjaxLoader();
            $("#authoringQuestionBank").load("../Authoring/QuestionBankLanding", function () {
                closeAjaxLoader();
            });
        },
        searchByText: function (isSearchButtonClicked) {
            var searchText = $("#searchByText").val();
            var searchQuestionType = $("#FilterByQuestionType_hidden").val();
            if (isSearchPage) {
                searchQuestionType = $("#FilterByQuestionTypeForSearch_hidden").val();
            }
            searchText = trimText(searchText);
            var isSearchValid = true;
            if (isNullOrEmpty(searchText) || searchText.length < 2) {
                if (isSearchButtonClicked == true) {
                    jAlert(SEARCH_VALIDATION, ALERT_TITLE, function () {
                    });
                    return false;
                }
                else {
                    searchText = "";
                }
            }

            if (isSearchValid) {
                startAjaxLoader();
                strListOfSelectedQuestionItems = "";
                if (isFilterValueClearedForSearch) {
                    searchQuestionType = "";
                }
                var questionSearchContent = "../QuestionBank/GiveSearchResults?strSearchText=" + encodeURIComponent(searchText) + "&strQuestionType=" + searchQuestionType;
                $("#authoringQuestionBank").load(questionSearchContent, function () {
                    //closeAjaxLoader();
                });
            }
            return false;
        },
        /*Template dropdown change function*/
        dropDownChange: function () {
            questionTypeLoaded = $("#QuestionBankList").val();
            var strQuestionType = "../QuestionBank/LoadQuestionType?strQuestionType=" + questionTypeLoaded;
            //Check if the dropdown value is '-Select-'
            if (questionTypeLoaded == 1) {
                $("#noTempleteSelected").css('display', 'block');
                $("#content_questions").hide();
                $("#rationaleLinkedCompetency").hide();
            }
            else {
                //To show loader on seletion of the dropdowns
                $("#RationaleForCorrectAnswer").val("");
                $("#LinkedCompetency_input").val("");
                $("#LinkedCompetency_hidden").val("");
                $("#content_questions").show();
                $("#noTempleteSelected").css('display', 'none');
                questionBank.commonFunctions.hideOrShowQuestionBankContent(false);
                //To show page after page loads in ajax
                $("#questionTypes").load(strQuestionType, function () {
                    questionBank.commonFunctions.hideOrShowQuestionBankContent(true);
                    if (questionTypeLoaded == 2) {
                        $("#rationaleLinkedCompetency").hide();
                        disableAButton("btnSaveQuestion");
                    }
                    else {
                        enableAButton("btnSaveQuestion", BLUE_BUTTON, "transaction-button", questionBank.commonFunctions.saveQuestion);
                        $("#rationaleLinkedCompetency").show();
                    }
                    if (questionTypeLoaded == 4) {
                        $("#rationale_correct_ans").hide();
                    }
                    else {
                        $("#rationale_correct_ans").show();
                    }

                });
                questionBank.commonFunctions.setmaxLenghtOfAnswers(questionTypeLoaded);
            }
        },
        hideOrShowQuestionBankContent: function (isShow) {
            if (isShow) {
                $("#questionTypesLoading").addClass('hide-content');
                $("#questionLoaded").removeClass('hide-content');
            } else {
                $("#questionTypesLoading").removeClass('hide-content');
                $("#questionLoaded").addClass('hide-content');
            }
        },
        enableRadioOnTextChange: function (text) {
            var number = text.name.split("_")[1];
            if (text.value != "") {
                $("#Answer" + number).attr('disabled', false);
            }
            else {
                $("#Answer" + number).attr('disabled', true);
                $("#Answer" + number).attr('checked', false);
            }
        },
        unCheckRadioOnRemovingImage: function (imageContentId) {
            if (questionTypeLoaded == "6") {
                var answerOptionNumber = imageContentId.charAt(imageContentId.length - 1);
                if ($("#AnswerOption_" + answerOptionNumber).val() == "") {
                    $("#Answer" + answerOptionNumber).attr('checked', false);
                    $("#Answer" + answerOptionNumber).attr('disabled', true);
                }
            }
        },
        getGuidValueForQuestionUI: function () {
            return qnGuidInEditMode;
        },
        /*To set max lenght for answer options for forming json*/
        setmaxLenghtOfAnswers: function (questionTypeLoaded) {
            switch (questionTypeLoaded) {
                case "8":
                    {
                        maxLenghtOfAnswers = 2;
                        break;
                    }
                case "7":
                    {
                        maxLenghtOfAnswers = 1;
                        break;
                    }
                case "6":
                    {
                        maxLenghtOfAnswers = 5;
                        break;
                    }
                case "3":
                    {
                        maxLenghtOfAnswers = 10;
                        break;
                    }
                case "4":
                    {
                        maxLenghtOfAnswers = 4;
                        break;
                    }
                case "9":
                    {
                        maxLenghtOfAnswers = 8;
                        break;
                    }
                case "5":
                    {
                        maxLenghtOfAnswers = 10;
                        break;
                    }
            }
        },
        /*Json formation for Question*/
        formJsonForQuestion: function () {
            var questionTextToSave = getControlValueByElementId("QuestionText", TEXTBOX_CONTROL);
            if (questionTypeLoaded == "9") {
                questionNewTextToSave = getControlValueByElementId("QuestionTextNew", TEXTBOX_CONTROL);
            }
            else {
                questionNewTextToSave = getControlValueByElementId("QuestionTextAreaNew", TEXTBOX_CONTROL);
            }
            var orientationFormat = $("#BlankOrientation").val();
            if (questionTypeLoaded == "4") {
                questionTextToSave = "";
                textCount = 0;
                blankCount = 0;
                questionTextToSave = questionBank.commonFunctions.getQuestionForFillIn(orientationFormat, "dynamicRenderDiv");
                questionNewTextToSave = questionBank.commonFunctions.getQuestionForFillIn(orientationFormat, "newQuestionTextDivFillIn");
            }
            var questionObj = {
                "QuestionText": questionTextToSave,
                "QuestionType": questionTypeLoaded,
                "Rationale": getControlValueByElementId("RationaleForCorrectAnswer", TEXTBOX_CONTROL),
                "BlankOrientation": $("#BlankOrientation").val(),
                "CompetencyReferenceGUID": $("input[type=hidden][id=LinkedCompetency_hidden]").val(),
                "QuestionImageReference": questionBank.commonFunctions.getOrSetImageReference(true, "QNIMAGE", "NA"),
                "CorrectOrder": (questionTypeLoaded == "3") ? questionBank.commonFunctions.getCorrectOrderOptions(maxLenghtOfAnswers) : "",
                "AnswerOptions": questionBank.commonFunctions.getAnswerOptions(maxLenghtOfAnswers),
                "NoOfLabels": $("#NoOfLabels").val(),
                "IsActive": true,
                "IsAutoSave": false
            };
            return questionObj;
        },
        getQuestionForFillIn: function (orientationFormat, divId) {
            textCount = 0;
            blankCount = 0;
            var questionTextToSave = "";
            var orientationControls = orientationFormat.split(" ");
            if ($("div[id^=" + divId + "]") != undefined && $("div[id^=" + divId + "]").length > 0) {
                for (var i = 0; i < orientationControls.length; i++) {

                    if (!isNullOrEmpty($("#" + divId + "text1").val())) {
                        if (orientationControls[i] == "(Blank)") {
                            blankCount = blankCount + 1;
                            questionTextToSave = questionTextToSave + FILL_IN_THE_BLANKS_BLANK;
                        }
                        else if (orientationControls[i] == "(Text)") {
                            textCount = textCount + 1;
                            questionTextToSave = questionTextToSave + $("#" + divId + "text" + textCount).val();


                        }
                    }
                }
            }
            return questionTextToSave;
        },
        /*To get/set image References for answers/questions*/
        getOrSetImageReference: function (isValueReturn, qnOrAnsImageRefIdName, imageRefId) {
            switch (qnOrAnsImageRefIdName) {
                case "QNIMAGE":
                    {
                        if (isValueReturn)
                            return getImageRefData("qnImageRefId");
                        else
                            loadImageToImageDiv(imageRefId, "qnImageRefId", true);
                        break;
                    }
                case "ANSIMAGE1":
                    {
                        if (isValueReturn)
                            return getImageRefData("answerRefId1");
                        else
                            loadImageToImageDiv(imageRefId, "answerRefId1", true);
                        $("#Answer1").attr('disabled', false);
                        break;
                    }
                case "ANSIMAGE2":
                    {
                        if (isValueReturn)
                            return getImageRefData("answerRefId2");
                        else
                            loadImageToImageDiv(imageRefId, "answerRefId2", true);
                        $("#Answer2").attr('disabled', false);
                        break;
                    }
                case "ANSIMAGE3":
                    {
                        if (isValueReturn)
                            return getImageRefData("answerRefId3");
                        else
                            loadImageToImageDiv(imageRefId, "answerRefId3", true);
                        $("#Answer3").attr('disabled', false);
                        break;
                    }
                case "ANSIMAGE4":
                    {
                        if (isValueReturn)
                            return getImageRefData("answerRefId4");
                        else
                            loadImageToImageDiv(imageRefId, "answerRefId4", true);
                        $("#Answer4").attr('disabled', false);
                        break;
                    }
                case "ANSIMAGE5":
                    {
                        if (isValueReturn)
                            return getImageRefData("answerRefId5");
                        else
                            loadImageToImageDiv(imageRefId, "answerRefId5", true);
                        $("#Answer5").attr('disabled', false);
                        break;
                    }
            }
            return false;
        },
        /*To get answeroption list */
        getAnswerOptions: function (lenghtOfAnswers) {
            var answerOptionList = [];
            if (questionTypeLoaded == "9") {
                var answerList = [];
                $('select#AnswerOptions').find('option').each(function () {
                    answerList.push($(this).val());
                });
                for (var iCount = 1; iCount <= answerList.length; iCount++) {
//                    var imgId = "ANSIMAGE" + iCount;
                    var answerOptions = {
                        "AnswerImageReference": null,
                        "Rationale": null,
                        "AnswerText": encodeSpecialSymbols(answerList[iCount - 1]),
                        "AnswerSequence": iCount,
                        "IsCorrectAnswer": false
                    };
                    answerOptionList.push(answerOptions);
                }
            }
            else if (questionTypeLoaded != "3") { //Answer options doesnt exists for correct order
                for (var iIndex = 1; iIndex <= lenghtOfAnswers; iIndex++) {
                    var imgId = "ANSIMAGE" + iIndex;
                    if (($("#AnswerOption_" + iIndex).val() != "") || (questionBank.commonFunctions.getOrSetImageReference(true, imgId, "NA") != "" && questionBank.commonFunctions.getOrSetImageReference(true, imgId, "NA") != undefined)) {
                        var answerOption = {
                            "AnswerImageReference": questionBank.commonFunctions.getOrSetImageReference(true, imgId, "NA"),
                            "Rationale": null,
                            "AnswerText": getControlValueByElementId("AnswerOption_" + iIndex, TEXTBOX_CONTROL),
                            "MachingText": getControlValueByElementId("AnswerMatch_" + iIndex, TEXTBOX_CONTROL),
                            "AnswerSequence": iIndex,
                            "IsCorrectAnswer": (maxLenghtOfAnswers == 1) ? true : questionBank.commonFunctions.checkIfCorrectAnswer("Answer" + iIndex)
                        };
                        answerOptionList.push(answerOption);
                    }
                }
            }
            return answerOptionList;
        },
        getCorrectOrderOptions: function (lenghtOfAnswers) {
            var correctOrderList = [];
            for (var iIndex = 1; iIndex <= lenghtOfAnswers; iIndex++) {
                var correctOrderItem = getControlValueByElementId("AnswerOption_" + iIndex, TEXTBOX_CONTROL);
                if (!isNullOrEmpty(correctOrderItem)) {
                    correctOrderList.push(correctOrderItem);
                }
            }
            return correctOrderList;
        },
        checkCorrectOrderConsecutiveAnswers: function (lenghtOfAnswers) {
            for (var iIndex = 1; iIndex <= lenghtOfAnswers; iIndex++) {
                var correctOrderItemPrev = $("#AnswerOption_" + (iIndex - 1)).val();
                var correctOrderItem = $("#AnswerOption_" + iIndex).val();
                if ($("#AnswerOption_" + (iIndex - 1)).length > 0 && isNullOrEmpty(correctOrderItemPrev) && !isNullOrEmpty(correctOrderItem)) {
                    return true;
                }
            }
            return false;
        },
        /*To check if the answeroption is a correct answer */
        checkIfCorrectAnswer: function (valueToCheck) {
            return ($('input[type=radio][id=' + valueToCheck + ']').is(' :checked'));
        },
        setCorrectAnswer: function (strCorrectAnswerValue) {
            $('input[type=radio][id=' + strCorrectAnswerValue + ']').attr('checked', true);
        },
        /*function to removeattachment */
        removeAttachment: function (idMetadataContent) {
            // !!!important!!!!: Just removing the meta data in page. Image should be removed only on click of save.
            jConfirm(IMAGE_REMOVE, "Remove Image", function (isOk) {
                if (isOk) {
//                    var guidOfImg = getImageRefData(idMetadataContent);
                    removeMetaDataForImage(idMetadataContent);
                    $("#" + idMetadataContent).attr('src', "../Content/Images/Image_Div.png");
                }
            });
        },
        /*Ajax call to save question */
        saveQuestion: function () {
            if (questionBank.commonFunctions.validateQuestionBankFields(questionTypeLoaded, false)) {
                var questionJson = questionBank.commonFunctions.formJsonForQuestion();
                var urlToSaveQn = "../QuestionBank/SaveQuestion?questionUrlReference=" + questionBank.commonFunctions.getUrlReferenceForQuestionSave() + "&folderIdentifier=" + getDivData("questionBankTab", "currentFolderIdentifier") + "&isEditMode=" + isEditMode + "&isNewQuestion=" + false + "&isExistingQuestion=" + false + "&questionGuid=" + "" + "&authoringType=" + 0 + "&authoringUrl=" + "&questionNewTextToSave=" + questionNewTextToSave;
                if (questionJson != null) {
                    startAjaxLoader();
                    doAjaxCall("POST", questionJson, urlToSaveQn, questionBank.commonFunctions.saveQuestionSuccess);
                }
            }
        },
        returnToQuestionBank: function () {
            startAjaxLoader();
            $("#authoringQuestionBank").load("../Authoring/QuestionBankLanding");
            closeAjaxLoader();
        },
        cancel: function () {
            var status = "Are you sure you want to cancel? Your changes will not be saved.";
            jConfirm(status, 'Cancel', function (isCancel) {
                if (isCancel) {
                    startAjaxLoader();
                    $("#authoringQuestionBank").load("../Authoring/QuestionBankLanding");
                    closeAjaxLoader();
                }
            });
        },
        saveQuestionSuccess: function () {
            jAlert(SAVED_MESSAGE, "Alert", function (isOk) {
                if (isOk) {
                    $("#authoringQuestionBank").load("../QuestionBank/QuestionBank");
                }
            });
            closeAjaxLoader();
        },
        getUrlReferenceForQuestionSave: function () {
            if (isNullOrEmpty(strUrlReferenceForSave)) {
                return getDivData("questionBankTab", "currentFolder");
            } else {
                return strUrlReferenceForSave;
            }
        },
        loadQuestionInEditMode: function (strUrlOfQuestion, iQuestionType) {
            //startAjaxLoader();
            var loadQuestionPage = "../QuestionBank/QuestionBank";
            $("#authoringQuestionBank").load(loadQuestionPage, function () {
                //closeAjaxLoader();
                questionBank.commonFunctions.hideOrShowQuestionBankContent(false);
                $("#questionHolder #questionTypes").load("../QuestionBank/RenderQuestionInEditMode?questionQuid=" + strUrlOfQuestion + "&iQuestionType=" + iQuestionType + "&folderType=1", function () {
                    enableAButton("btnSaveQuestion", BLUE_BUTTON, "transaction-button", questionBank.commonFunctions.saveQuestion);
                    questionBank.commonFunctions.hideOrShowQuestionBankContent(true);
                });
                $("#rationaleLinkedCompetency").css('display', 'block');
                $("#qn_select_template").css('display', 'none');
                $("#noTempleteSelected").css('display', 'none');
            });

        },
        loadPageInEditMode: function (qnImgToLoadInEditMode, strRationale, strCompetencyLinked, strCorrectAnswerValue, questionUrl, isEditModeFlag, linkedCompetencyGuid, questionTypeLoadedFlag) {
            strUrlReferenceForSave = !isNullOrEmpty(questionUrl) ? questionUrl : "";
            isEditMode = isEditModeFlag;
            if (!isNullOrEmpty(questionTypeLoadedFlag)) {
                questionTypeLoaded = questionTypeLoadedFlag;
            }
            questionBank.commonFunctions.setmaxLenghtOfAnswers(questionTypeLoaded);
            if (!isNullOrEmpty(qnImgToLoadInEditMode)) {
                questionBank.commonFunctions.getOrSetImageReference(false, "QNIMAGE", qnImgToLoadInEditMode);
            }
            else {
                $("#qnImageRefId_view").removeClass("link select-hand").addClass("disabled-text");
                $("#qnImageRefId_view").removeData("imgRef");
                $("#qnImageRefId_view").die('click');
                $("#qnImageRefId_view").attr('src', "");
            }
            if (questionTypeLoaded == 4) {
                $("#rationale_correct_ans").hide();
            }
            else {
                $("#rationale_correct_ans").show();
            }
            if (!isNullOrEmpty(strRationale)) {
                $("#RationaleForCorrectAnswer").val(strRationale);
            }
            if (!isNullOrEmpty(strCompetencyLinked)) {
                isLinkedCompetencyselected = true;
                $("input[type=hidden][id=LinkedCompetency_hidden]").val(linkedCompetencyGuid);
                $("#LinkedCompetency_input").val(strCompetencyLinked);
                $("#LinkedCompetency_input").removeClass('watermark watermark-text');
            }
            if (!isNullOrEmpty(strCorrectAnswerValue)) {
                questionBank.commonFunctions.setCorrectAnswer(strCorrectAnswerValue);
            }
        },
        loadAnswerImages: function (ansImageGuid, answerCount) {
            if (!isNullOrEmpty(ansImageGuid)) {
                var imgId = "ANSIMAGE" + answerCount;
                questionBank.commonFunctions.getOrSetImageReference(false, imgId, ansImageGuid);
            }
        },
        loadDynamicQuestionTextInEditMode: function (questiontext, divId) {

            var questionTextItems = questiontext.split(FILL_IN_THE_BLANKS_BLANK);
            questionTextItems = $.grep(questionTextItems, function (item) { return item.length != 0; });
            for (var count = 1; count <= textCount; count++) {
                $("#" + divId + "text" + count).val($.trim(questionTextItems[count - 1]));
            }
        },
        enableValidRaioButtons: function () {
            for (var count = 1; count <= 5; count++) {
                if ($("#AnswerOption_" + count).val() != "") {
                    $("#Answer" + count).attr('disabled', false);
                }
            }
        },
        /*Common functions for validating the controls used in question bank module*/
        validateQuestionBankFields: function (questionTypeLoaded, isNewQuestion) {
//            var maxAnswersLength = -1;
            var isValid = false;
            var isInputRequired = false;
            var answerTextCount = 0;
            var answerMatchCount = 0;
            var errorMessage = "<UL class = 'validation_ul'>";
            var selectedCompetency = $("#LinkedCompetency_input").val();
            if (!isNullOrEmpty(selectedCompetency)) {
                if (jQuery.inArray(selectedCompetency, competencyArray) >= 0) {
                    isLinkedCompetencyselected = true;
                }
                else
                    isLinkedCompetencyselected = false;
            } else {
                isLinkedCompetencyselected = false;
            }
            if (questionTypeLoaded == "5") {
                if ((isNullOrEmpty($("#QuestionText").val())) || !isLinkedCompetencyselected) {
                    errorMessage += "<LI>" + INPUT_REQUIRED + "</LI>";
                    isInputRequired = false;
                    isValid = true;
                }
            }
            if ((isNullOrEmpty($("#QuestionText").val()) && questionTypeLoaded != "4" && questionTypeLoaded != "5") || (questionTypeLoaded != "5" && !isLinkedCompetencyselected) || (questionTypeLoaded == "9" && hasDropDownValue($("#NoOfLabels").val()))) {
                isInputRequired = true;
                isValid = true;
            }
            else if (!($('input[name=CorrectAnswer]').is(':checked')) && questionTypeLoaded != "7" && questionTypeLoaded != "3" && questionTypeLoaded != "9" && questionTypeLoaded != "4" && questionTypeLoaded != "5") {
                isValid = true;
                errorMessage += "<LI>" + MUST_CORRECT_ANS + "</LI>";
            }
            else {
                switch (questionTypeLoaded) {
                    case "7":
                        if (isNullOrEmpty($("#AnswerOption_1").val())) {
                            isInputRequired = true;
                        }
                        break;
                    case "8":
                        break;
                    case "9":
                        var answerList = [];
                        $('select#AnswerOptions').find('option').each(function () {
                            answerList.push($(this).val());
                        });
                        if (answerList.length != $("#NoOfLabels").val()) {
                            errorMessage += "<LI>" + NO_OF_LABELS_MISMATCH + "</LI>";
                            isValid = true;
                        }
                        if (questionBank.commonFunctions.getOrSetImageReference(true, "QNIMAGE", "NA") == "" || questionBank.commonFunctions.getOrSetImageReference(true, "QNIMAGE", "NA") == undefined) {
                            isInputRequired = true;
                            isValid = true;
                        }
                        break;
                    case "5":
                        for (var answerOptionCounts = 1; answerOptionCounts <= maxLenghtOfAnswers; answerOptionCounts++) {
                            if (!isNullOrEmpty($("#AnswerOption_" + answerOptionCounts).val())) {
                                answerTextCount = answerTextCount + 1;
                            }
                        }
                        for (var answerOption = 1; answerOption <= maxLenghtOfAnswers; answerOption++) {
                            if (!isNullOrEmpty($("#AnswerMatch_" + answerOption).val())) {
                                answerMatchCount = answerMatchCount + 1;
                            }
                            else if (questionBank.commonFunctions.getOrSetImageReference(true, "ANSIMAGE" + answerOption, "NA") != "" && questionBank.commonFunctions.getOrSetImageReference(true, "ANSIMAGE" + answerOption, "NA") != undefined) {
                                answerMatchCount = answerMatchCount + 1;
                            }
                        }
                        if ((answerTextCount < 2) || (answerMatchCount < 2)) {
                            errorMessage += "<LI>" + INCORRECT_ANSWERS + "</LI>";
                            isValid = true;
                        }
                        else if (answerTextCount != answerMatchCount) {
                            errorMessage += "<LI>" + ITEM_MISMATCH + "</LI>";
                            isValid = true;
                        }
                        break;
                    default:
                        for (var answerOptionCount = 1; answerOptionCount <= maxLenghtOfAnswers; answerOptionCount++) {
                            if (!isNullOrEmpty($("#AnswerOption_" + answerOptionCount).val())) {
                                answerTextCount = answerTextCount + 1;
                            }
                            else if (questionTypeLoaded == "6") {
                                if (questionBank.commonFunctions.getOrSetImageReference(true, "ANSIMAGE" + answerOptionCount, "NA") != "" && questionBank.commonFunctions.getOrSetImageReference(true, "ANSIMAGE" + answerOptionCount, "NA") != undefined) {
                                    answerTextCount = answerTextCount + 1;
                                }
                            }
                        }
                        if (answerTextCount <= 2) {
                            if (questionTypeLoaded != "3") {
                                errorMessage += "<LI>" + INPUT_ANSWERS_REQUIRED + "</LI>";
                            }
                            else {
                                errorMessage += "<LI>" + INPUT_ANSWER_REQUIRED_CORRECTORDER + "</LI>";
                            }
                            isValid = true;
                            break;
                        }
                        if (questionTypeLoaded == "4") {
                            if (!hasDropDownValue($("#BlankOrientation").val())) {
                                for (var countQuestion = 1; countQuestion <= textCount; countQuestion++) {
                                    if (isNullOrEmpty($("#dynamicRenderDivtext" + countQuestion).val())) {
                                        isInputRequired = true;
                                        isValid = true;
                                        break;
                                    }
                                }
                            }
                            else {
                                isInputRequired = true;
                                isValid = true;
                                break;
                            }
                            if (!($('input[name=CorrectAnswer]').is(':checked'))) {
                                isValid = true;
                                errorMessage += "<LI>" + MUST_CORRECT_ANS + "</LI>";
                                break;
                            }
                        }
                        break;
                }
            }
            if (isNewQuestion && isNewQuestionRequired) {
                if (questionTypeLoaded != "4") {
                    if (questionTypeLoaded == "9") {
                        if (isNullOrEmpty($("#QuestionTextNew").val())) {
                            isInputRequired = true;
                            isValid = true;
                        }
                    }
                    else {
                        if (isNullOrEmpty($("#QuestionTextAreaNew").val())) {
                            isInputRequired = true;
                            isValid = true;
                        }
                    }
                    //                    else {
                    //                        if ($("#QuestionTextNew").val() == $("#QuestionText").val()) {
                    //                            errorMessage += "<LI>" + QUESTION_NAME_EXISTS + "</LI>";
                    //                            isValid = true;
                    //                        }
                    //                    }
                }
                if (questionTypeLoaded == "4") {
                    if (!hasDropDownValue($("#BlankOrientation").val())) {
                        for (var count = 1; count <= textCount; count++) {
                            if (isNullOrEmpty($("#newQuestionTextDivFillIntext" + count).val())) {
                                isInputRequired = true;
                                isValid = true;
                                break;
                            }
                        }
                    }
                    else {
                        isInputRequired = true;
                        isValid = true;
                    }
                    var questionTextToSave = questionBank.commonFunctions.getQuestionForFillIn($("#BlankOrientation").val(), "dynamicRenderDiv");
                    var questionNewTextToSaveTemp = questionBank.commonFunctions.getQuestionForFillIn($("#BlankOrientation").val(), "newQuestionTextDivFillIn");
                    //                    if (questionTextToSave == questionNewTextToSaveTemp) {
                    //                        errorMessage += "<LI>" + QUESTION_NAME_EXISTS + "</LI>";
                    //                        isValid = true;
                    //                    }
                }

            }
            if (isInputRequired) {
                errorMessage += "<LI>" + INPUT_REQUIRED + "</LI>";
                isValid = true;
            }
            if (questionTypeLoaded == "3" && !isValid) {
                isValid = questionBank.commonFunctions.checkCorrectOrderConsecutiveAnswers(10);
                errorMessage += "<LI>" + CORRECT_SEQUENCE + "</LI>";
            }
            if (isValid) {
                errorMessage += "</UL>";
                $("#validationSummary")[0].innerHTML = errorMessage;
                $("#validationSummary").show();
                $("#questionTypes").scrollTo("#validationSummary", 100);
                $("#QuestionTemplateSlide").scrollTo("#validationSummary", 100);
                $("#mainStep3SkillSet").focus();
                $("#validationSummary").focus();
                $("#qn-type-holder-all-questions").scrollTo('#qn_select_template', 100);
                return false;
            } else {
                $("#validationSummary")[0].innerHTML = "";
                $("#validationSummary").hide();
                return true;
            }

        },

        gridOperations: {
            questionItemChanged: function (obj) {
                var idOfQuestionItem = obj.id;
                strListOfSelectedQuestionItems = check_AddOrRemoveIfItemExistsInString(strListOfSelectedQuestionItems, idOfQuestionItem, 'Ø', obj.checked);
            }
        },

        fillInTheBlank: {
            renderDynamicControls: function (orientationFormat, divId) {
                textCount = 0;
                blankCount = 0;
                $("#" + divId).empty();
                var orientationControls = orientationFormat.split(" ");
                for (var i = 0; i < orientationControls.length; i++) {
                    $("#" + divId).append("&nbsp;");
                    if (orientationControls[i] == "(Blank)") {
                        blankCount = blankCount + 1;
                        $("#" + divId).append("&nbsp;<label id='blank" + blankCount + "'>____________________</label>&nbsp;");
                    }
                    else if (orientationControls[i] == "(Text)") {
                        textCount = textCount + 1;
                        $("#" + divId).append($('<input>').attr({ type: 'text', name: 'text' + textCount, id: divId + 'text' + textCount, style: 'width:160px;font-size:11px' }));
                    }
                }
            },
            loadQuestionInEditMode: function () {

            }
        },
        labellingQuestion: {
            appendDataInSelectList: function () {
                var labelTextEntered = $("#labelBlank").val();
                if (labelTextEntered != "") {
                    $("#AnswerOptions").append("<option title=" + encodeHTML(labelTextEntered) + ">" + labelTextEntered + "</option>");
                    $("#labelBlank").val("");
                }
            },

            removeDataInSelectList: function () {
                $("#AnswerOptions > option:selected").each(function () {
                    $(this).remove();
                });
            },

            loadAnswerListInEditMode: function (answerArray) {
                if (answerArray != undefined) {
                    for (var count = 0; count < answerArray.length; count++) {
                        $("#AnswerOptions").append("<option>" + answerArray[count] + "</option>");
                    }
                }
            }
        }
    }
};