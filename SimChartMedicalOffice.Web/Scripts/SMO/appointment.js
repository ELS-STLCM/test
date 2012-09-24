var assignmentUniquePath;
var newAppointmentPatientJson;
var appointmentEditModeJson = "";
var appointmentEditURL = "";
var isAppointmentEdit = false;
var occurrenceTypeSelected = "";
var appointmentLoadUrl = "";
var appointmentLoadType = "";
var dialogLoadTitle = "";
var appointmentLoadDate = "";
var isAppointmentRecurrence = "";
var isSaveConflictAppointment = false;
var appointmentDurationEdit = 0;
var lstPatient = [];
var providerMasterList = [];
var providerMasterListOther = [];
var appointment = {
    commonFunction: {
        getAssignmentUniqueIdentifier: function () {
            return $("#assignmentUniqueIdentifierPath").data("assignmentIdentifier");
        },
        setAssignmentUniqueIdentifier: function (assignmentUniquePath) {
            setDivData($("#assignmentUniqueIdentifierPath"), "assignmentIdentifier", assignmentUniquePath);
        }
    },
    patientFunctions: {
        saveAppointmentPatient: function () {
            if (appointment.patientFunctions.validationOfNewPatient()) {
                var appointmentPatientJson;
                appointmentPatientJson = {
                    "FirstName": $("#patient_firstname").val(),
                    "LastName": $("#patient_lastname").val(),
                    "MiddleInitial": $("#patient_middleInitial").val(),
                    "DateOfBirth": $("#patient_dateaobirth").val(),
                    "Provider": $("#patientAppointmentProvider").val(),
                    "Insurance": $("#insurance").val(),
                    "IDNumber": $("#ID_number").val(),
                    "NameofPolicyHolder": $("#Name_policyholder").val(),
                    "GroupNumber": $("#group_number").val(),
                    "SSNofPolicyHolder": $("#SSN_policyholder").val()
                };
                var urlToSaveAppointmentPatient = "../Appointment/SaveNewPatientForAppointment?assignmentUniqueIdentifier=" + appointment.commonFunction.getAssignmentUniqueIdentifier();
                if (appointmentPatientJson != null) {
                    startAjaxLoader();
                    doAjaxCall("POST", appointmentPatientJson, urlToSaveAppointmentPatient, appointment.patientFunctions.saveSuccessOfNewPatient);
                    closeAjaxLoader();
                }
            }
        },
        validationOfNewPatient: function () {
            var isValid = false;
            var errorMessage = "<UL>";
            if (isNullOrEmpty($("#patient_firstname").val()) || isNullOrEmpty($("#patient_lastname").val()) ||
                isNullOrEmpty($("#patient_middleInitial").val()) || isNullOrEmpty($("#patient_dateaobirth").val()) ||
                    isNullOrEmpty($("#ID_number").val()) || isNullOrEmpty($("#Name_policyholder").val()) ||
                        isNullOrEmpty($("#group_number").val()) || isNullOrEmpty($("#SSN_policyholder").val()) ||
                            hasDropDownValue($("#insurance").val()) || hasDropDownValue($("#patientAppointmentProvider").val())) {
                errorMessage += "<LI>" + INPUT_REQUIRED + "</LI>";
                isValid = true;
            }
            if (isValid) {
                errorMessage += "</UL>";
                $("#validationSummaryforNewPatient")[0].innerHTML = errorMessage;
                $("#validationSummaryforNewPatient").focus();
                $("#validationSummaryforNewPatient").show();
                return false;
            } else {
                return true;
            }
        },
        cancel: function () {
            var status = "Are you sure you want to cancel? Your changes will not be saved.";
            jConfirm(status, 'Cancel', function (isCancel) {
                if (isCancel) {
                    $('#appointment_new_patient').dialog("close");
                    for (var radioCount = 0; radioCount < $('input:radio[name=PatientOption]').length; radioCount++) {
                        $('input:radio[name=PatientOption]')[radioCount].checked = false;
                    }
                    $('#appointment_load_view_content_view').dialog("open");
                }
            });
        },
        patientSelect: function () {
            if ($('input:radio[name=PatientOption]:checked').val() == 'createNewPatient') {
                $('#appointment_load_view_content_view').dialog("close");
                $("#dvVerifyPatient").hide();
                $("#appointment_new_patient").load("../SimOfficeCalendar/LoadNewPatient", function () {
                   $('#appointment_new_patient').dialog({
                        autoOpen: false,
                        modal: true,
                        closeOnEscape: false,
                        resizable: false,
                        open: function () {
                            applyClassForDialogHeader();
                        },
                        title: 'New Patient',
                        height: 330,
                        width: 720
                    });
                    $('#appointment_new_patient').dialog("open");
                });
            } else {
                $('#appointment_load_view_content_view').dialog("close");
                $("#appointment_search_patient").load("../Forms/FormsPatientSearch", function () {
                    $("#PatientSearch").dialog({
                        height: 450,
                        width: 850,
                        modal: true,
                        position: 'center',
                        resizable: false,
                        autoOpen: true,
                        closeOnEscape: false,
                        title: 'Patient Search',
                        close: function () {
                            reapplyDialogHeader();
                        },
                        open: function () {
                            $("#filterDateOfBirth").datepicker("hide");
                            applyBlueClassForDialogHeader();
                            appointment.patientFunctions.DisableSelectBtn("PatientSearch_BtnSelect");
                            removeCloseIconForDialogHeader(this);
                            applyBlueBorderForDialogHeader();
                        },
                        overlay: { opacity: 0.5, background: 'black' }
                    });
                });
            }
            $("#PatientSearch_BtnSelect").live('click', function () {
                // close PatientSearch dailog
                $("input[type=radio][name='selectPatient']:checked").each(function () {
                    appointment.patientFunctions.LoadPatientInfo(this.id);
                    appointment.patientFunctions.ClosePatientSearch();
                    $("#dvVerifyPatient").show();
                });
            });
            $("#PatientSearch_BtnCancel").live('click', function () {
                var status = "Are you sure you want to cancel? Your changes will not be saved.";
                jConfirm(status, 'Cancel', function (isCancel) {
                    if (isCancel) {
                        appointment.patientFunctions.ClosePatientSearch();
                        for (var radioCount = 0; radioCount < $('input:radio[name=PatientOption]').length; radioCount++) {
                            $('input:radio[name=PatientOption]')[radioCount].checked = false;
                        }
                        $('#appointment_load_view_content_view').dialog("open");
                    }
                });
            });

            // calender control events
            $("#filterDOBCalendarImage").live('click', function () {
                $("#filterDateOfBirth").datepicker();
                $("#filterDateOfBirth").datepicker("show");
            });
            $("#filterDateOfBirth").live('click', function () {
                $("#filterDateOfBirth").datepicker("hide");
            });

            //Search filter hide btn click
            $("#searchFilterHide").live('click', function () {
                $("div[id='searchFilter']").slideUp('fast');
                $("div[id='searchFilterShow']").show();
            });
            $("#searchFilterShow").live('click', function () {
                $(this).hide();
                $("div[id='searchFilter']").slideDown('fast');
            });

            // Filter button click event
            $("#fiterPatientButton").live('click', function () {
                appointment.patientFunctions.DisableSelectBtn("PatientSearch_BtnSelect");
                patientDataTable.fnDraw();
            });

            // Patient Select event
            $("input[type=radio][name=selectPatient]").live('change', function () {
                if ($("#PatientSearch_BtnSelect").is(":disabled")) {
                    enableAButtonForms("PatientSearch_BtnSelect", ORANGE_BUTTON, "navigation-button", "");
                }
            });
        },
        ClosePatientSearch: function () {
            $("FilterMRN").val("");
            $("FilterFirstName").val("");
            $("FilterLastName").val("");
            $("FilterDOB").val("");
            $("#PatientSearch_BtnSelect").die();
            $("#PatientSearch_BtnCancel").die();
            $("#fiterPatientButton").die();

            closeDialog("PatientSearch");
            $('#appointment_load_view_content_view').dialog("open");
        },
        LoadPatientInfo: function (patientGUID) {
            $.ajax({
                type: "POST",
                dataType: 'json',
                //data: JSON.stringify(priorAuthorizationRequestJson),
                url: "../Forms/LoadPatientInfo?patientGUID=" + patientGUID,
                contentType: AJAX_CONTENT_TYPE,
                error: function (result) {
                    if (result != null) {
                        jAlert(LOAD_ERROR, "Alert");
                    }
                },
                success: function (result) {
                    if (result != null) {
                        if (result.PatientInfo != null) {
                            newAppointmentPatientJson = {
                                "FirstName": result.PatientInfo.FirstName,
                                "LastName": result.PatientInfo.LastName,
                                "MiddleInitial": result.PatientInfo.MiddleInitial,
                                "Provider": result.PatientInfo.Provider,
                                "PatientIdentifier": result.PatientInfo.UniqueIdentifier
                            };
                            $(".ui-dialog-title-appointment_load_view_content_view").css('color', '#000000');
                            $("#Selected-patient-name").html(result.PatientInfo.LastName + ", " + result.PatientInfo.FirstName + " " + result.PatientInfo.MiddleInitial);
                            $("#ProviderList").val(result.PatientInfo.Provider);
                            $('#appointment_load_view_content_view').dialog("open");
                            $("#dvRemove").show();
                            $("#dvPatient").show();
                            $("#dvDateLoad").show();
                            $("#dvEmptyDiv").hide();
                        } else {
                            jAlert("Patient not found", "Error");
                        }
                    }
                }
            });
        },
        DisableSelectBtn: function (buttonId) {
            $("#" + buttonId).removeClass('transaction-button').removeClass('navigation-button').removeClass('cancel-button').removeClass('remove-button');
            $("#" + buttonId).addClass('disabled-button');
            $("#" + buttonId + "-LeftCurve").attr('src', '../Content/Images/Buttons/Button_left_curve_grey.jpg');
            $("#" + buttonId + "-RightCurve").attr('src', '../Content/Images/Buttons/Button_right_curve_grey.jpg');
            $("#" + buttonId).attr('disabled', true);
        },
        saveSuccessOfNewPatient: function (result) {
            if (result.PatientPresent) {
                jAlert(SAVED_MESSAGE, "Alert", function () {
                    newAppointmentPatientJson = {
                        "FirstName": result.Result.FirstName,
                        "LastName": result.Result.LastName,
                        "MiddleInitial": result.Result.MiddleInitial,
                        "Provider": result.Result.Provider,
                        "PatientIdentifier": result.Result.UniqueIdentifier
                    };
                    $('#searchByPatientCalendar_input').remove();
                    $('#searchByPatientCalendar_hidden').remove();
                    $('#searchByPatientCalendar_arrow').remove();
                    $('#searchByPatientCalendar_ctr').remove();
                    lstPatient = result.SearchPatientList;
                    waterMarkTextForCalendarSearchPatient();
                    $('#appointment_new_patient').dialog("close");
                    $("#Selected-patient-name").html(result.Result.LastName + ", " + result.Result.FirstName + " " + result.Result.MiddleInitial);
                    $("#ProviderList").val(result.Result.Provider);
                    $('#appointment_load_view_content_view').dialog("open");
                    $("#dvRemove").show();
                    $("#dvPatient").show();
                    $("#dvDateLoad").show();
                    $("#dvEmptyDiv").hide();
                });
            } else {
                $("#validationSummaryforNewPatient")[0].innerHTML = "Patient already exists";
                $("#validationSummaryforNewPatient").focus();
                $("#validationSummaryforNewPatient").show();
            }
        }
    },

    LoadNewAppointment: function (appointmentDate, appointmentUrl, appointmentType, patientName, status, isAppointmentRecurrenceOnEditMode,isViewMode) {
        if (!isNullOrEmpty(appointmentUrl)) {
            isAppointmentEdit = true;
        } else {
            {
                isAppointmentEdit = false;
            }
        }
        var dialogTitle = "";
        if ((!isNullOrEmpty(patientName)) && (appointmentType == PATIENTVISIT)) {
            dialogTitle = "Saved Appointment: " + patientName;
        } else if (appointmentType == BLOCK) {
            dialogTitle = "Saved Appointment: " + BLOCK;
        } else if (appointmentType == Other) {
            dialogTitle = "Saved Other Appointment";
        } else {
            dialogTitle = "New Appointment";
        }
        if (status != StatusCheckedOut && !isViewMode) {
            //            startAjaxLoader();
            if (isAppointmentRecurrenceOnEditMode) {
                appointmentLoadUrl = appointmentUrl;
                appointmentLoadType = appointmentType;
                dialogLoadTitle = dialogTitle;
                appointmentLoadDate = appointmentDate;
                isAppointmentRecurrence = isAppointmentRecurrenceOnEditMode;
                appointment.LoadRecurrencePopUp(EDIT_APPOINTMENT, OPEN_RECURRING_ITEM, false);
            } else {
                startAjaxLoader();
                appointment.LoadAppointmentPopup(appointmentUrl, appointmentType, dialogTitle, appointmentDate);
            }
            //            closeAjaxLoader();
        } else {
            startAjaxLoader();
            $("#appointment_load_view_readonly").load("../Appointment/LoadAppointmentInViewMode?appointmentUniqueIdentifierUrl=" + appointmentUrl + "&appointmentType=" + appointmentType, function () {
                $('#appointment_load_view_readonly').dialog({
                    autoOpen: false,
                    modal: true,
                    closeOnEscape: false,
                    resizable: false,
                    open: function () {
                        applyClassForDialogHeader();
                        applyCloseIconForDialogHeader(this);
                    },
                    title: dialogTitle,
                    height: 450,
                    width: 500
                });
                $('#appointment_load_view_readonly').dialog("open");
                closeAjaxLoader();
            });
        }
    },
    LoadAppointmentPopup: function (appointmentUrl, appointmentType, dialogTitle, appointmentDate) {
        $("#appointment_load_view_content_view").load("../Appointment/LoadAppointment?appointmentUniqueIdentifierUrl=" + appointmentUrl + "&appointmentType=" + appointmentType + "&occurenceType=" + appointment.GetOccurrenceType(), function () {
            $('#appointment_load_view_content_view').dialog({
                autoOpen: false,
                modal: true,
                closeOnEscape: false,
                resizable: false,
                open: function () {
                    applyClassForDialogHeader();
                    applyCloseIconForDialogHeader(this);
                },
                title: dialogTitle,
                height: 450,
                width: 500
            });
            $('#appointment_load_view_content_view').dialog("open");
            if (appointmentUrl == "") {
                $("#date").val(appointmentDate);
            }
            closeAjaxLoader();
        });
    },
    LoadExistingAppointment: function () {
        isAppointmentEdit = true;
        if (isAppointmentEdit) {
            $("#dvPatientSelection").hide();

        }
    },
    AppointmentTypeSelection: function () {
        if ($('input:radio[name=AppointmentType]:checked').val() == "PatientVisit") {
            $("#dvblock").hide();
            $("#dvVisitType").show();
            $("#dvPatient").hide();
            $("#dvPatientSelection").show();
            $("#dvOther").hide();
            $("#dvDateLoad").hide();
            $("#dvEmptyDiv").show();
            $("#VisitType").val("-Select-");
            for (var radioCount = 0; radioCount < $('input:radio[name=PatientOption]').length; radioCount++) {
                $('input:radio[name=PatientOption]')[radioCount].checked = false;
            }
        } else if ($('input:radio[name=AppointmentType]:checked').val() == "Block") {
            $("#dvblock").show();
            $("#dvVisitType").hide();
            $("#dvPatient").hide();
            $("#dvPatientSelection").hide();
            $("#dvOther").hide();
            $("#dvDateLoad").show();
            $("#dvEmptyDiv").hide();
            $("#attendeesDropDown").show();
            $("#attendeesCheckbox").hide();

        } else {
            $("#dvblock").hide();
            $("#dvVisitType").hide();
            $("#dvPatient").hide();
            $("#dvPatientSelection").hide();
            $("#dvOther").show();
            $("#dvDateLoad").show();
            $("#Other_textbox").hide();
            $("#dvEmptyDiv").hide();
            $("#attendeesDropDown").hide();
            $("#attendeesCheckbox").show();
            $("#OtherType").val("-Select-");
            $("#LocationOther").val("-Select-");
            $("#otherTextBlock").val("");
            $("input[type=checkbox][name='attendeesList']").each(function () {
                this.checked = false;
                this.disabled = false;
            });
        }
    },
    isRecurrence: function () {
        if ($('input:checkbox[name=chkRecurrence]:checked').val() == "IsChecked") {
            $("#dvRecurrence").show();
        } else {
            $("#dvRecurrence").hide();
            appointment.clearRecurrenceValues();
        }
    },
    clearRecurrenceValues: function () {
        $("#EndBy").val("");
        $("#txtOccurences").val("");
        for (var radioCountduration = 0; radioCountduration < $('input:radio[name=ReoccurenceDuration]').length; radioCountduration++) {
            $('input:radio[name=ReoccurenceDuration]')[radioCountduration].checked = false;
        }
        for (var radioCountPattern = 0; radioCountPattern < $('input:radio[name=ReoccurencePattern]').length; radioCountPattern++) {
            $('input:radio[name=ReoccurencePattern]')[radioCountPattern].checked = false;
        }
    },
    saveAppointment: function () {
        var strAppointmentType = $('input:radio[name=AppointmentType]:checked').val();
        if (appointment.ValidateAppointmentFields(strAppointmentType)) {
            $(".mandatory_field_highlight").removeClass("mandatory_field_highlight");
            $("#validationSummary").empty();
            $("#validationSummary").hide();
            var strAppointmentURL = "";
            var examRoomIdentifier = "";
            var patientIdentifier = "";
            var firstName = "";
            var lastName = "";
            var middleInitial = "";
            var recurrenceGroupJson = null;
            var type = "";
            var strOtherText = "";
            var noOfOccurences = 0;
            var status = 0;
            var isAllStaff = false;
            var providerIds = [];
            if (strAppointmentType == PATIENTVISIT) {
                examRoomIdentifier = $("#ExamRoom").val();
                patientIdentifier = newAppointmentPatientJson.PatientIdentifier;
                firstName = newAppointmentPatientJson.FirstName;
                lastName = newAppointmentPatientJson.LastName;
                middleInitial = newAppointmentPatientJson.MiddleInitial;
                type = $("#VisitType").val();
                strOtherText = "";
                //providerId = $("#ProviderList").val();
                providerIds.push($("#ProviderList").val());
                status = 1;
            } else if (strAppointmentType == BLOCK) {
                examRoomIdentifier = $("#BlockLocation").val();
                patientIdentifier = "";
                firstName = "";
                lastName = "";
                middleInitial = "";
                type = $("#BlockType").val();
                strOtherText = $("#otherText").val();
                //providerId = $("#BlockFor").val();
                if ($("#BlockFor").val() == AllStaffNumericValue) {
                    for (var iCount = 0; iCount < providerMasterList.length; iCount++) {
                        if (providerMasterList[iCount].Value != SelectNumericValue && providerMasterList[iCount].Value != AllStaffNumericValue) {
                            providerIds.push(providerMasterList[iCount].Value);
                        }
                    }
                    isAllStaff = true;
                } else {
                    providerIds.push($("#BlockFor").val());
                    isAllStaff = false;
                }
                status = 0;
            } else if (strAppointmentType == Other) {
                examRoomIdentifier = $("#LocationOther").val();
                patientIdentifier = "";
                firstName = "";
                lastName = "";
                middleInitial = "";
                type = $("#OtherType").val();
                strOtherText = $("#otherTextBlock").val();
                if ($('input:checkbox[name=attendeesList]:checked').val() == AllStaffNumericValue) {
                    for (var i = 0; i < providerMasterListOther.length; i++) {
                        if (providerMasterListOther[i].Value != SelectNumericValue && providerMasterListOther[i].Value != AllStaffNumericValue) {
                            providerIds.push(providerMasterListOther[i].Value);
                        }
                    }
                    isAllStaff = true;
                }
                else {
                    $("input[type=checkbox][name='attendeesList']:checked").each(function () {
                        providerIds.push(this.value);
                    });
                    isAllStaff = false;
                }
                //providerId = $("#otherAttendees").val();
                //                for (var count = 1001; count < 1005; count++) {
                //                    if ($('input:checkbox[id=' + count + ']')[0].checked == true) {
                //                        if (attendeeList != "") {
                //                            attendeeList = attendeeList.concat("$", count);
                //                        }
                //                        else {
                //                            attendeeList = count;
                //                        }
                //                    }
                //                }
            }
            if ($('input:checkbox[name=chkRecurrence]:checked').val() == "IsChecked") {
                if (!isNullOrEmpty($("#txtOccurences").val())) {
                    noOfOccurences = $("#txtOccurences").val();
                }
                recurrenceGroupJson = {
                    "Pattern": $('input:radio[name=ReoccurencePattern]:checked').val(),
                    "NumberOfOccurences": noOfOccurences,
                    "StartDateTime": $("#date").val() + " " + $("#StartTime").val(),
                    "EndDateTime": $("#date").val() + " " + $("#EndTime").val(),
                    "EndBy": (!isNullOrEmpty($("#EndBy").val())) ? $("#EndBy").val() + " " + $("#EndTime").val() : "",
                    "Description": $("#Description").val(),
                    "IsActive": true
                };
            }
            var appointmentJson = {
                "Type": type,
                "OtherText": strOtherText,
                "PatientIdentifier": patientIdentifier,
                "FirstName": firstName,
                "LastName": lastName,
                "MiddleInitial": middleInitial,
                "ProviderId": providerIds,
                "ExamRoomIdentifier": examRoomIdentifier,
                "IsInformationVerified": $('input:checkbox[name=verifyPatient]')[0].checked,
                "StartDateTime": $("#date").val() + " " + $("#StartTime").val(),
                "EndDateTime": $("#date").val() + " " + $("#EndTime").val(),
                "IsRecurrence": $('input:checkbox[name=chkRecurrence]')[0].checked,
                "Recurrence": recurrenceGroupJson,
                "ReasonForCancellation": !isNullOrEmpty($("#txtReason").val()) ? $("#txtReason").val() : "",
                "Description": $("#Description").val(),
                "IsActive": true,
                "Status": status,
                "IsAllStaffSelected": isAllStaff,
                "ChartTimeStamp": getClientTimeStampString(),
                "Signature": getLoginUserId(),
                "InactivatedBy":"",
                "InactiveTimeStamp":"",
                "ChartingRole":"",
                "ChartModifiedTimeStamp":"",
                "ChartModifiedBy":""
                //                "OtherAttendeesList": attendeeList
            };
            startAjaxLoader();
            var appointmentSaveUrl = "../Appointment/SaveAppointment?appointmentType=" + strAppointmentType + "&appointmentGuid=" + strAppointmentURL + "&occurenceType=" + appointment.GetOccurrenceType() + "&isSaveConflict=" + isSaveConflictAppointment;
            doAjaxCall("POST", appointmentJson, appointmentSaveUrl, this.successAppointment);
        } else {
            var errorMessage = "<UL class = 'validation_ul'>";
            errorMessage += "<LI>" + INPUT_REQUIRED + "</LI>";
            errorMessage += "</UL>";
            $("#validationSummary")[0].innerHTML = errorMessage;
            $("#validationSummary").focus();
            $("#validationSummary").show();
            $("#appointment_load_view_content_view").scrollTo("#validationSummary", 300);
        }
    },
    successAppointment: function (result) {
        closeAjaxLoader();
        result = eval('(' + result.Result + ')');
        if (result.Code == "Success") {
            smoCalendar.setCalendarFilterCriteria();
            jAlert(result.Description, ALERT_TITLE, function () {
                closeDialogWithoutDestroy("appointment_load_view_content_view");
                isSaveConflictAppointment = false;
                // to load the refresh method 
            });
        } else if (result.Code == "Error") {
            jAlert(result.Description, ALERT_TITLE, function () {
            });
        } else {
            jConfirm(result.Description, ALERT_TITLE, function (isOk) {
                if (isOk) {
                    isSaveConflictAppointment = true;
                    appointment.saveAppointment();
                }
            });
        }
    },
    //    successSaveAppointment: function (result) {
    //        result = eval('(' + result.Result + ')');
    //        if (result.Code == "Success") {
    //            smoCalendar.setCalendarFilterCriteria();
    //            jAlert(result.Description, ALERT_TITLE, function (isOk) {
    //                closeDialogWithoutDestroy("appointment_load_view_content_view");
    //                // to load the refresh method 
    //            });
    //        } else {
    //            jAlert(result.Description, ALERT_TITLE, function (isOk) {
    //            });
    //        }
    ////        var errorMessage = result.ErrorMessage;
    ////        if (isNullOrEmpty(errorMessage)) {
    ////            smoCalendar.setCalendarFilterCriteria();
    ////            if (true) {
    ////                jAlert("Saved", ALERT_TITLE, function (isOk) {
    ////                    closeDialogWithoutDestroy("appointment_load_view_content_view");
    ////                    // to load the refresh method 
    ////                });
    ////            } else {
    ////                jAlert("Failed");
    ////            }
    ////        } else {
    ////            jAlert(errorMessage, ALERT_TITLE, function (isOk) {
    ////            });
    ////        }
    //    },
    ValidateAppointmentFields: function (strAppointmentType) {
        if (strAppointmentType == PATIENTVISIT) {
            if ($("#VisitType").val() == "-Select-") {
                return false;
            }
            if (isNullOrEmpty(newAppointmentPatientJson)) {
                return false;
            }
            if (hasDropDownValue($("#ProviderList").val())) {
                return false;
            }
        }
        if (strAppointmentType == BLOCK) {
            if ($("#BlockType").val() == "-Select-") {
                return false;
            }
            if (hasDropDownValue($("#BlockFor").val())) {
                return false;
            }
            if ($("#BlockType").val() == Other) {
                if (isNullOrEmpty($("#otherText").val())) {
                    return false;
                }
            }
        }
        if (strAppointmentType == Other) {
            var attendeeNotSelect = false;
            if ($("#OtherType").val() == "-Select-") {
                return false;
            } else if ($("#OtherType").val() == "Other") {
                if ($("#otherTextBlock").val() == "") {
                    return false;
                }
            }
            if ($("#otherAttendees").val() == "-Select-") {
                return false;
            }
            //            for (var count = 1001; count < 1005; count++) {
            //                if ($('input:checkbox[id=' + count + ']')[0].checked == true) {
            //                    attendeeNotSelect == true;
            //                }
            //            }
            if (attendeeNotSelect) {
                return false;
            }
        }
        if ($("#StartTime").val() == "-Select-") {
            return false;
        }
        if ($("#EndTime").val() == "-Select-") {
            return false;
        }
        if (isNullOrEmpty($("#date").val())) {
            return false;
        }
        if (($('input:checkbox[name=chkRecurrence]:checked').val() == "IsChecked")) {
            if ($('input:radio[name=ReoccurencePattern]:checked').val() == undefined) {
                return false;
            }
            if ($('input:radio[name=ReoccurenceDuration]:checked').val() == undefined) {
                return false;
            }
            if ($('input:radio[name=ReoccurenceDuration]:checked').val() == "EndAfter") {
                if (isNullOrEmpty($("#txtOccurences").val())) {
                    return false;
                }
            }
            if ($('input:radio[name=ReoccurenceDuration]:checked').val() == "EndBy") {
                if (isNullOrEmpty($("#EndBy").val())) {
                    return false;
                }
            }
        }
        if (strAppointmentType == Other) {

        }
        return true;
    },
    RemovePatient: function () {
        newAppointmentPatientJson = "";
        $("#Selected-patient-name").html("");
        for (var radioCount = 0; radioCount < $('input:radio[name=PatientOption]').length; radioCount++) {
            $('input:radio[name=PatientOption]')[radioCount].checked = false;
        }
        $("#dvRemove").hide();
        $("#dvVerifyPatient").hide();
        $("#dvPatient").hide();
        $("#dvDateLoad").hide();
        $("#dvEmptyDiv").show();
    },
    CloseAppointment: function () {
        var status = "Are you sure you want to cancel? Your changes will not be saved.";
        jConfirm(status, 'Cancel', function (isCancel) {
            if (isCancel) {
                $('#appointment_load_view_content_view').dialog("close");
            }
        });
    },
    CloseCancelAppointment: function () {
        $('#appointment_cancel_view').dialog("close");
        var popupMode = $("input[type=hidden][id=AppointmentMode]").val();
        if (popupMode == DELETE_APPOINTMENT || popupMode == CANCEL_APPOINTMENT_TEXT) {
            $('#appointment_load_view_content_view').dialog("open");
        }
    },
    SubmitCancelledAppointment: function () {
        appointment.SetOccurrenceType();
        $('#appointment_cancel_view').dialog("close");
        var popupMode = $("input[type=hidden][id=AppointmentMode]").val();
        if (popupMode == EDIT_APPOINTMENT) {
            startAjaxLoader();
            appointment.LoadAppointmentPopup(appointmentLoadUrl, appointmentLoadType, dialogLoadTitle, appointmentLoadDate);
        } else if (popupMode == DELETE_APPOINTMENT) {
            appointment.DeleteAppointment();
        }
        //        if (isNullOrEmpty(appointmentEditURL)) {
        //            $('#appointment_load_view_content_view').dialog("open");
        //        } else {
        else {
            appointment.EditAppointment();
        }
        //        }
    },
    SetOccurrenceType: function () {
        if (isAppointmentRecurrence) {
            occurrenceTypeSelected = $('input:radio[name=CancelAppointment]:checked').val();
        } else {
            occurrenceTypeSelected = OCCURENCE_NONE;
        }

        return occurrenceTypeSelected;
    },
    GetOccurrenceType: function () {
        return occurrenceTypeSelected;
    },
    fnSaveAppointment: function () {
        if (!isAppointmentEdit) {
            appointment.saveAppointment();
        } else {
            if ($("#AppointmentStatus").val() == "4") {
                var appointmentStatus = $("#AppointmentStatus").val();
                if (isAppointmentRecurrence) {
                    appointment.LoadRecurrencePopUp(appointmentStatus, CANCEL_APPOINTMENT, true);
                } else {
                    var status = "Cancelling this appointment will remove it from the calendar";
                    jConfirm(status, 'Cancel', function (isOk) {
                        if (isOk) {
                            appointment.EditAppointment();
                        }
                    });
                }
                //                appointment.LoadRecurrencePopUp;
                //                var status = "Cancelling this appointment will remove it from the calendar";
                //                jConfirm(status, 'Cancel', function (isCancel) {
                //                    if (isCancel) {
                //                        //appointment.CancelAppointment;
                //                    }
                //                });
            } else {
                appointment.EditAppointment();
            }

        }
    },
    FormAppointmentJson: function () {
        var strAppointmentType = $("input[type=hidden][id=appointment-type-value]").val();
        var examRoomIdentifier = "";
        var patientIdentifier = "";
        var firstName = "";
        var lastName = "";
        var middleInitial = "";
        var recurrenceGroupJson = null;
        var type = "";
        var strOtherText = "";
        var noOfOccurences = 0;
        var appointmentStatus = 1;
        var strStatusLocation = 0;
        var providerIds = [];
        var isAllStaff = false;
        if (strAppointmentType == PATIENTVISIT) {
            examRoomIdentifier = $("#ExamRoom").val();
            patientIdentifier = appointmentEditModeJson.PatientIdentifier;
            firstName = appointmentEditModeJson.FirstName;
            lastName = appointmentEditModeJson.LastName;
            middleInitial = appointmentEditModeJson.MiddleInitial;
            type = $("#VisitType").val();
            strOtherText = "";
            providerIds.push($("#ProviderList").val());
            appointmentStatus = $("#AppointmentStatus").val();
            strStatusLocation = $("#StatusLocationList").val();
        } else if (strAppointmentType == BLOCK) {
            examRoomIdentifier = $("#BlockLocation").val();
            patientIdentifier = "";
            firstName = "";
            lastName = "";
            middleInitial = "";
            type = $("#BlockType").val();
            strOtherText = $("#otherText").val();
            if ($("#BlockFor").val() == AllStaffNumericValue) {
                for (var i = 0; i < providerMasterList.length; i++) {
                    if (providerMasterList[i].Value != SelectNumericValue && providerMasterList[i].Value != AllStaffNumericValue) {
                        providerIds.push(providerMasterList[i].Value);
                    }
                }
                isAllStaff = true;
            } else {
                providerIds.push($("#BlockFor").val());
                isAllStaff = false;
            }
        }
        else if (strAppointmentType == Other) {
            examRoomIdentifier = $("#LocationOther").val();
            patientIdentifier = "";
            firstName = "";
            lastName = "";
            middleInitial = "";
            type = $("#OtherType").val();
            strOtherText = $("#otherTextBlock").val();
            if ($('input:checkbox[name=attendeesList]:checked').val() == AllStaffNumericValue) {
                for (var i = 0; i < providerMasterListOther.length; i++) {
                    if (providerMasterListOther[i].Value != SelectNumericValue && providerMasterListOther[i].Value != AllStaffNumericValue) {
                        providerIds.push(providerMasterListOther[i].Value);
                    }
                }
                isAllStaff = true;
            }
            else {
                $("input[type=checkbox][name='attendeesList']:checked").each(function () {
                    providerIds.push(this.value);
                });
                isAllStaff = false;
            }
        }
        if ($('input:checkbox[name=chkRecurrence]:checked').val() == "IsChecked") {
            if (!isNullOrEmpty($("#txtOccurences").val())) {
                noOfOccurences = $("#txtOccurences").val();
            }
            recurrenceGroupJson = {
                "Pattern": $('input:radio[name=ReoccurencePattern]:checked').val(),
                "NumberOfOccurences": noOfOccurences,
                "StartDateTime": $("#date").val() + " " + $("#StartTime").val(),
                "EndDateTime": $("#date").val() + " " + $("#EndTime").val(),
                "EndBy": (!isNullOrEmpty($("#EndBy").val())) ? $("#EndBy").val() + " " + $("#EndTime").val() : "",
                "Description": $("#Description").val(),
                "IsActive": true
            };
        }
        var appointmentJson = {
            "Type": type,
            "OtherText": strOtherText,
            "PatientIdentifier": patientIdentifier,
            "FirstName": firstName,
            "LastName": lastName,
            "MiddleInitial": middleInitial,
            "ProviderId": providerIds,
            "ExamRoomIdentifier": examRoomIdentifier,
            "IsInformationVerified": $('input:checkbox[name=verifyPatient]')[0].checked,
            "StartDateTime": $("#date").val() + " " + $("#StartTime").val(),
            "EndDateTime": $("#date").val() + " " + $("#EndTime").val(),
            "IsRecurrence": $('input:checkbox[name=chkRecurrence]')[0].checked,
            "Recurrence": recurrenceGroupJson,
            "Description": $("#Description").val(),
            "IsActive": true,
            "Status": appointmentStatus,
            "StatusLocation": strStatusLocation,
            "ReasonForCancellation": !isNullOrEmpty($("#txtReason").val()) ? $("#txtReason").val() : "",
            "IsAllStaffSelected": isAllStaff,
            "ChartTimeStamp": "",
            "Signature": "",
            "InactivatedBy": getLoginUserId(),
            "InactiveTimeStamp": getClientTimeStampString(),
            "ChartingRole": "",
            "ChartModifiedTimeStamp": getClientTimeStampString(),
            "ChartModifiedBy": getLoginUserId()
        };
        return appointmentJson;
    },
    LoadRecurrencePopUp: function (mode, popUpTitle, isAppointmentOpen) {
        $("#appointment_cancel_view").load("../Appointment/DeleteCancelAppointment?cancelValue=" + mode, function () {
            $('#appointment_cancel_view').dialog({
                autoOpen: false,
                modal: true,
                closeOnEscape: false,
                resizable: false,
                open: function () {
                    applyClassForDialogHeader();
                    applyCloseIconForDialogHeader(this);
                },
                title: popUpTitle,
                height: 200,
                width: 250
            });
            if (isAppointmentOpen) {
                $('#appointment_load_view_content_view').dialog("close");
            }
            $('#appointment_cancel_view').dialog("open");
        });

    },
    DeleteAppointmentPopup: function () {
        if (isAppointmentRecurrence) {
            appointment.LoadRecurrencePopUp(DELETE_APPOINTMENT, DELETE_TITLE, true);
        } else {
            appointment.SetOccurrenceType();
            var status = "Deleting this appointment will remove it from the calendar";
            jConfirm(status, 'Cancel', function (isOk) {
                if (isOk) {
                    appointment.DeleteAppointment();
                }
            });
        }
    },
    DeleteAppointment: function () {
        var strAppointmentType = $("input[type=hidden][id=appointment-type-value]").val();
        var deleteAppointmentUrl = "../Appointment/DeleteAppointment?appointmentUrl=" + appointmentEditURL + "&recurrenceStatus=" + appointment.GetOccurrenceType() + "&appointmentType=" + strAppointmentType;
        doAjaxCall("POST", appointment.FormAppointmentJson(), deleteAppointmentUrl, this.successAppointment);
    },
    CancelAppointment: function () {
        var cancelAppointmentUrl = "../Appointment/CancelAppointment?appointmentUrl=" + appointmentEditURL + "&cancelAllAppintment=" + false + "appointmentType=" + strAppointmentType;
        doAjaxCall("POST", "", cancelAppointmentUrl, this.successCancelAppointment);
    },
    EditAppointment: function () {
        var strAppointmentUrl = appointmentEditURL;
        var strAppointmentType = $("input[type=hidden][id=appointment-type-value]").val();
        if (appointment.ValidateEditAppointmentFields(strAppointmentType)) {
            $(".mandatory_field_highlight").removeClass("mandatory_field_highlight");
            $("#validationSummary").empty();
            $("#validationSummary").hide();
            var examRoomIdentifier = "";
            var patientIdentifier = "";
            var firstName = "";
            var lastName = "";
            var middleInitial = "";
            var recurrenceGroupJson = null;
            var type = "";
            var strOtherText = "";
            var noOfOccurences = 0;
            var appointmentStatus = 1;
            var strStatusLocation = 0;
            var providerIds = [];
            var isAllStaff = false;
            if (strAppointmentType == PATIENTVISIT) {
                examRoomIdentifier = $("#ExamRoom").val();
                patientIdentifier = appointmentEditModeJson.PatientIdentifier;
                firstName = appointmentEditModeJson.FirstName;
                lastName = appointmentEditModeJson.LastName;
                middleInitial = appointmentEditModeJson.MiddleInitial;
                type = $("#VisitType").val();
                strOtherText = "";
                //providerId = $("#ProviderList").val();
                providerIds.push($("#ProviderList").val());
                appointmentStatus = $("#AppointmentStatus").val();
                strStatusLocation = $("#StatusLocationList").val();
            } else if (strAppointmentType == BLOCK) {
                examRoomIdentifier = $("#BlockLocation").val();
                patientIdentifier = "";
                firstName = "";
                lastName = "";
                middleInitial = "";
                type = $("#BlockType").val();
                strOtherText = $("#otherText").val();
                //providerId = $("#BlockFor").val();
                appointmentStatus = 0;
                if ($("#BlockFor").val() == AllStaffNumericValue) {
                    for (var i = 0; i < providerMasterList.length; i++) {
                        if (providerMasterList[i].Value != SelectNumericValue && providerMasterList[i].Value != AllStaffNumericValue) {
                            providerIds.push(providerMasterList[i].Value);
                        }
                    }
                    isAllStaff = true;
                } else {
                    providerIds.push($("#BlockFor").val());
                    isAllStaff = false;
                }
            }
            else if (strAppointmentType == Other) {
                examRoomIdentifier = $("#LocationOther").val();
                patientIdentifier = "";
                firstName = "";
                lastName = "";
                middleInitial = "";
                type = $("#OtherType").val();
                strOtherText = $("#otherTextBlock").val();
                if ($('input:checkbox[name=attendeesList]:checked').val() == AllStaffNumericValue) {
                    for (var i = 0; i < providerMasterListOther.length; i++) {
                        if (providerMasterListOther[i].Value != SelectNumericValue && providerMasterListOther[i].Value != AllStaffNumericValue) {
                            providerIds.push(providerMasterListOther[i].Value);
                        }
                    }
                    isAllStaff = true;
                }
                else {
                    $("input[type=checkbox][name='attendeesList']:checked").each(function () {
                        providerIds.push(this.value);
                    });
                    isAllStaff = false;
                }
            }
            if ($('input:checkbox[name=chkRecurrence]:checked').val() == "IsChecked") {
                if (!isNullOrEmpty($("#txtOccurences").val())) {
                    noOfOccurences = $("#txtOccurences").val();
                }
                recurrenceGroupJson = {
                    "Pattern": $('input:radio[name=ReoccurencePattern]:checked').val(),
                    "NumberOfOccurences": noOfOccurences,
                    "StartDateTime": $("#date").val() + " " + $("#StartTime").val(),
                    "EndDateTime": $("#date").val() + " " + $("#EndTime").val(),
                    "EndBy": (!isNullOrEmpty($("#EndBy").val())) ? $("#EndBy").val() + " " + $("#EndTime").val() : "",
                    "Description": $("#Description").val(),
                    "IsActive": true
                };
            }
            var appointmentJson = {
                "Type": type,
                "OtherText": strOtherText,
                "PatientIdentifier": patientIdentifier,
                "FirstName": firstName,
                "LastName": lastName,
                "MiddleInitial": middleInitial,
                "ProviderId": providerIds,
                "ExamRoomIdentifier": examRoomIdentifier,
                "IsInformationVerified": $('input:checkbox[name=verifyPatient]')[0].checked,
                "StartDateTime": $("#date").val() + " " + $("#StartTime").val(),
                "EndDateTime": $("#date").val() + " " + $("#EndTime").val(),
                "IsRecurrence": $('input:checkbox[name=chkRecurrence]')[0].checked,
                "Recurrence": recurrenceGroupJson,
                "Description": $("#Description").val(),
                "IsActive": true,
                "Status": appointmentStatus,
                "StatusLocation": !isNullOrEmpty(strStatusLocation) ? strStatusLocation : "0",
                "ReasonForCancellation": !isNullOrEmpty($("#txtReason").val()) ? $("#txtReason").val() : "",
                "IsAllStaffSelected": isAllStaff,
                "ChartTimeStamp": "",
                "Signature": "",
                "InactivatedBy": "",
                "InactiveTimeStamp": "",
                "ChartingRole": "",
                "ChartModifiedTimeStamp": getClientTimeStampString(),
                "ChartModifiedBy": getLoginUserId()
            };
            startAjaxLoader();
            var appointmentSaveUrl = "../Appointment/SaveAppointment?appointmentType=" + strAppointmentType + "&appointmentGuid=" + strAppointmentUrl + "&occurenceType=" + appointment.GetOccurrenceType() + "&isSaveConflict=" + isSaveConflictAppointment;
            doAjaxCall("POST", appointmentJson, appointmentSaveUrl, this.successAppointment);
        } else {
            var errorMessage = "<UL class = 'validation_ul'>";
            errorMessage += "<LI>" + INPUT_REQUIRED + "</LI>";
            errorMessage += "</UL>";
            $("#validationSummary")[0].innerHTML = errorMessage;
            $("#validationSummary").focus();
            $("#validationSummary").show();
            $("#appointment_load_view_content_view").scrollTo("#validationSummary", 300);
        }
    },
    ValidateEditAppointmentFields: function (strAppointmentType) {
        if (strAppointmentType == PATIENTVISIT) {
            if ($("#VisitType").val() == "-Select-") {
                return false;
            }
            if (hasDropDownValue($("#ProviderList").val())) {
                return false;
            }
        }
        if (strAppointmentType == BLOCK) {
            if ($("#BlockType").val() == "-Select-") {
                return false;
            }
            if (hasDropDownValue($("#BlockFor").val())) {
                return false;
            }
            if ($("#BlockType").val() == Other) {
                if (isNullOrEmpty($("#otherText").val())) {
                    return false;
                }
            }
        }
        if ($("#StartTime").val() == "-Select-") {
            return false;
        }
        if ($("#EndTime").val() == "-Select-") {
            return false;
        }
        if (isNullOrEmpty($("#date").val())) {
            return false;
        }
        if (($('input:checkbox[name=chkRecurrence]:checked').val() == "IsChecked")) {
            if ($('input:radio[name=ReoccurencePattern]:checked').val() == undefined) {
                return false;
            }
            if ($('input:radio[name=ReoccurenceDuration]:checked').val() == undefined) {
                return false;
            }
            if ($('input:radio[name=ReoccurenceDuration]:checked').val() == "EndAfter") {
                if (isNullOrEmpty($("#txtOccurences").val())) {
                    return false;
                }
            }
            if ($('input:radio[name=ReoccurenceDuration]:checked').val() == "EndBy") {
                if (isNullOrEmpty($("#EndBy").val())) {
                    return false;
                }
            }
        }
        if (strAppointmentType == Other) {

        }
        return true;
    },
    SetEndTimeInRecurrenceAppointmentEdit: function () {
        if (isAppointmentEdit) {
            var tempStartDate = convertStringToDate($("#date").val() + " " + $("#StartTime").val());
            var totalMinutes = (tempStartDate.getHours() * 60) + tempStartDate.getMinutes() + eval(appointmentDurationEdit);
            tempStartDate = convertStringToDate($("#date").val());
            tempStartDate.setMinutes(totalMinutes);
            var endTime = dateFormat(new Date(tempStartDate), JS_TIME_FORMAT);
            $("#EndTime").val(endTime);
        }
    }
};
