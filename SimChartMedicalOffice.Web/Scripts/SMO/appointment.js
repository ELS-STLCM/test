var assignmentUniquePath;
var newAppointmentPatientJson;
var appointmentEditModeJson = "";
var appointmentEditURL = "";
var isAppointmentEdit = false;
var occurrenceTypeSelected = "";
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
            var errormessage = "";
            var isValid = false;
            errorMessage = "<UL>";
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
            }
            else {
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
                    var $dialog = $('#appointment_new_patient').dialog({
                        autoOpen: false,
                        modal: true,
                        closeOnEscape: false,
                        resizable: false,
                        open: function (event, ui) {
                            applyClassForDialogHeader();
                        },
                        title: 'New Patient',
                        height: 330,
                        width: 720
                    });
                    $('#appointment_new_patient').dialog("open");
                });
            }
            else {
                $('#appointment_load_view_content_view').dialog("close");
                $("#appointment_search_patient").load("../Forms/FormsPatientSearch", function () {
                    $("#PatientSearch").dialog({ height: 450,
                        width: 850,
                        modal: true,
                        position: 'center',
                        resizable: false,
                        autoOpen: true,
                        closeOnEscape: false,
                        title: 'Patient Search',
                        close: function (event, ui) {
                            reapplyDialogHeader();
                        },
                        open: function (event, ui) {
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
                        }
                        else {
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
                jAlert(SAVED_MESSAGE, "Alert", function (isOk) {
                    newAppointmentPatientJson = {
                        "FirstName": result.Result.FirstName,
                        "LastName": result.Result.LastName,
                        "MiddleInitial": result.Result.MiddleInitial,
                        "Provider": result.Result.Provider,
                        "PatientIdentifier": result.Result.UniqueIdentifier
                    };
                    $('#appointment_new_patient').dialog("close");
                    $("#Selected-patient-name").html(result.Result.LastName + ", " + result.Result.FirstName + " " + result.Result.MiddleInitial);
                    $("#ProviderList").val(result.Result.Provider);
                    $('#appointment_load_view_content_view').dialog("open");
                    $("#dvRemove").show();
                    $("#dvPatient").show();
                    $("#dvDateLoad").show();
                    $("#dvEmptyDiv").hide();
                });
            }
            else {
                $("#validationSummaryforNewPatient")[0].innerHTML = "Patient already exists";
                $("#validationSummaryforNewPatient").focus();
                $("#validationSummaryforNewPatient").show();
            }
        }
    },

    LoadNewAppointment: function (appointmentDate, appointmentUrl, appointmentType, patientName, status, isAppointmentRecurrenceOnEditMode) {
        if (!isNullOrEmpty(appointmentUrl)) {
            isAppointmentEdit = true;
        } else {
            {
                isAppointmentEdit = false;
            }
        }
        if (isAppointmentRecurrenceOnEditMode) {
            appointment.LoadRecurrencePopUp("edit", OPEN_RECURRING_ITEM);
            appointment.SetOccurrenceType();
        }
        
        var dialogTitle = "";
        if ((!isNullOrEmpty(patientName)) && (appointmentType == PATIENTVISIT)) {
            dialogTitle = "Saved Appointment: " + patientName;
        }
        else if (appointmentType == BLOCK) {
            dialogTitle = "Saved Appointment: " + BLOCK;
        }
        else {
            dialogTitle = "New Appointment";
        }
        if (status != StatusCheckedOut) {
            startAjaxLoader();
            $("#appointment_load_view_content_view").load("../Appointment/LoadAppointment?appointmentUniqueIdentifierUrl=" + appointmentUrl + "&appointmentType=" + appointmentType, function () {
                var $dialog = $('#appointment_load_view_content_view').dialog({
                    autoOpen: false,
                    modal: true,
                    closeOnEscape: false,
                    resizable: false,
                    open: function (event, ui) {
                        applyClassForDialogHeader();
                        applyCloseIconForDialogHeader(this);
                    },
                    title: dialogTitle,
                    height: 450,
                    width: 500
                });
                $('#appointment_load_view_content_view').dialog("open");
                $("#date").val(appointmentDate);
                closeAjaxLoader();
            });
        } else {
            startAjaxLoader();
            $("#appointment_load_view_readonly").load("../Appointment/LoadAppointmentInViewMode?appointmentUniqueIdentifierUrl=" + appointmentUrl + "&appointmentType=" + appointmentType, function () {
                var $dialog = $('#appointment_load_view_readonly').dialog({
                    autoOpen: false,
                    modal: true,
                    closeOnEscape: false,
                    resizable: false,
                    open: function (event, ui) {
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
    LoadExistingAppointment: function (calendarEventProxy) {
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
        }
        else if ($('input:radio[name=AppointmentType]:checked').val() == "Block") {
            $("#dvblock").show();
            $("#dvVisitType").hide();
            $("#dvPatient").hide();
            $("#dvPatientSelection").hide();
            $("#dvOther").hide();
            $("#dvDateLoad").show();
            $("#dvEmptyDiv").hide();
        }
        else {
            $("#dvblock").hide();
            $("#dvVisitType").hide();
            $("#dvPatient").hide();
            $("#dvPatientSelection").hide();
            $("#dvOther").show();
        }
    },
    isRecurrence: function () {
        if ($('input:checkbox[name=chkRecurrence]:checked').val() == "IsChecked") {
            $("#dvRecurrence").show();
        }
        else {
            $("#dvRecurrence").hide();
        }
    },
    saveAppointment: function () {
        var strAppointmentType = $('input:radio[name=AppointmentType]:checked').val();
        if (appointment.ValidateAppointmentFields(strAppointmentType)) {
            $(".mandatory_field_highlight").removeClass("mandatory_field_highlight");
            $("#validationSummary").empty();
            $("#validationSummary").hide();
            var strAppointmentURL = "";
            var patientJson = {};
            var providerId = 0;
            var examRoomIdentifier = "";
            var patientIdentifier = "";
            var firstName = "";
            var lastName = "";
            var middleInitial = "";
            var recurrenceGroupJson = null;
            var type;
            var strOtherText;
            var noOfOccurences = 0;
            if (strAppointmentType == PATIENTVISIT) {
                examRoomIdentifier = $("#ExamRoom").val();
                patientIdentifier = newAppointmentPatientJson.PatientIdentifier;
                firstName = newAppointmentPatientJson.FirstName;
                lastName = newAppointmentPatientJson.LastName;
                middleInitial = newAppointmentPatientJson.MiddleInitial;
                type = $("#VisitType").val();
                strOtherText = "";
                providerId = $("#ProviderList").val();
            }
            else if (strAppointmentType == BLOCK) {
                examRoomIdentifier = $("#BlockLocation").val();
                patientIdentifier = "";
                firstName = "";
                lastName = "";
                middleInitial = "";
                type = $("#BlockType").val();
                strOtherText = $("#otherText").val();
                providerId = $("#BlockFor").val();
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
                    "EndBy": $("#EndBy").val(),
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
                "ProviderId": providerId,
                "ExamRoomIdentifier": examRoomIdentifier,
                "IsInformationVerified": $('input:checkbox[name=verifyPatient]')[0].checked,
                "StartDateTime": $("#date").val() + " " + $("#StartTime").val(),
                "EndDateTime": $("#date").val() + " " + $("#EndTime").val(),
                "IsRecurrence": $('input:checkbox[name=chkRecurrence]')[0].checked,
                "Recurrence": recurrenceGroupJson,
                "Description": $("#Description").val(),
                "IsActive": true,
                "Status": "1"

            };

            var appointmentSaveUrl = "../Appointment/SaveAppointment?appointmentType=" + strAppointmentType + "&appointmentGuid=" + strAppointmentURL;
            doAjaxCall("POST", appointmentJson, appointmentSaveUrl, this.successSaveAppointment);
        }
        else {
            var errorMessage = "<UL>";
            errorMessage += "<LI>" + INPUT_REQUIRED + "</LI>";
            errorMessage += "</UL>";
            $("#validationSummary")[0].innerHTML = errorMessage;
            $("#validationSummary").focus();
            $("#validationSummary").show();
            $("#assignment-Metadata-Main-Content").scrollTo("#validationSummary", 300);
        }
    },
    successSaveAppointment: function (result) {
        var errorMessage = result.ErrorMessage;
        if (isNullOrEmpty(errorMessage)) {
            smoCalendar.setCalendarFilterCriteria();
            if (true) {
                jAlert("Saved", ALERT_TITLE, function (isOk) {
                    closeDialogWithoutDestroy("appointment_load_view_content_view");
                    // to load the refresh method 
                });
            } else {
                jAlert("Failed");
            }
        } else {
            jAlert(errorMessage, ALERT_TITLE, function (isOk) {
            });
        }
    },
    ValidateAppointmentFields: function (strAppointmentType) {
        if (strAppointmentType == PATIENTVISIT) {
            if ($("#VisitType").val() == "-Select-") {
                return false;
            }
            if (isNullOrEmpty(newAppointmentPatientJson)) {
                return false;
            }
            if (isNullOrEmpty($("#ProviderList").val())) {
                return false;
            }

        }
        if (strAppointmentType == BLOCK) {
            if ($("#BlockType").val() == "-Select-") {
                return false;
            }
            if ($("#BlockFor").val() == "-Select-") {
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
        $('#appointment_load_view_content_view').dialog("close");
    },
    CloseCancelAppointment: function () {
        $('#appointment_cancel_view').dialog("close");
        $('#appointment_load_view_content_view').dialog("open");
    },
    SubmitCancelledAppointment: function () {
        $('#appointment_cancel_view').dialog("close");
        $('#appointment_load_view_content_view').dialog("open");
        appointment.SetOccurrenceType();
    },
    SetOccurrenceType: function () {
        occurrenceTypeSelected = $('input:radio[name=CancelAppointment]:checked').val();
        return occurrenceTypeSelected;
    },
    GetOccurrenceType: function () {
        return occurrenceTypeSelected;
    },
    fnSaveAppointment: function () {
        if (!isAppointmentEdit) {
            appointment.saveAppointment();
        }
        else {
            if ($("#AppointmentStatus").val() == "4") {
                var appointmentStatus = $("#AppointmentStatus").val();
                appointment.LoadRecurrencePopUp(appointmentStatus, CANCEL_APPOINTMENT);
                //                var status = "Cancelling this appointment will remove it from the calendar";
                //                jConfirm(status, 'Cancel', function (isCancel) {
                //                    if (isCancel) {
                //                        //appointment.CancelAppointment;
                //                    }
                //                });
            }
            else {
                appointment.EditAppointment();
            }

        }
    },
    LoadRecurrencePopUp: function (appointmentStatus, popUpTitle) {
        $("#appointment_cancel_view").load("../Appointment/DeleteCancelAppointment?cancelValue=" + appointmentStatus, function () {
            var $dialog = $('#appointment_cancel_view').dialog({
                autoOpen: false,
                modal: true,
                closeOnEscape: false,
                resizable: false,
                open: function (event, ui) {
                    applyClassForDialogHeader();
                    applyCloseIconForDialogHeader(this);
                },
                title: popUpTitle,
                height: 200,
                width: 250
            });
            $('#appointment_load_view_content_view').dialog("close");
            $('#appointment_cancel_view').dialog("open");
        });

    },
    CancelAppointment: function () {
        var cancelAppointmentUrl = "../Appointment/CancelAppointment?appointmentUrl=" + appointmentEditURL + "&cancelAllAppintment=" + false + "appointmentType=" + strAppointmentType;
        doAjaxCall("POST", "", cancelAppointmentUrl, this.successCancelAppointment);
    },
    EditAppointment: function () {
        var strAppointmentUrl = appointmentEditURL;
        var strAppointmentType = $("#Selected-appointment-type").html();
        if (appointment.ValidateEditAppointmentFields(strAppointmentType)) {
            $(".mandatory_field_highlight").removeClass("mandatory_field_highlight");
            $("#validationSummary").empty();
            $("#validationSummary").hide();
            var patientJson = {};
            var providerId = 0;
            var examRoomIdentifier = "";
            var patientIdentifier = "";
            var firstName = "";
            var lastName = "";
            var middleInitial = "";
            var recurrenceGroupJson = null;
            var type;
            var strOtherText;
            var noOfOccurences = 0;
            var appointmentStatus = 1;
            var strStatusLocation = 0;
            if (strAppointmentType == PATIENTVISIT) {
                examRoomIdentifier = $("#ExamRoom").val();
                patientIdentifier = appointmentEditModeJson.PatientIdentifier;
                firstName = appointmentEditModeJson.FirstName;
                lastName = appointmentEditModeJson.LastName;
                middleInitial = appointmentEditModeJson.MiddleInitial;
                type = $("#VisitType").val();
                strOtherText = "";
                providerId = $("#ProviderList").val();
                appointmentStatus = $("#AppointmentStatus").val();
                strStatusLocation = $("#StatusLocationList").val();
            }
            else if (strAppointmentType == BLOCK) {
                examRoomIdentifier = $("#BlockLocation").val();
                patientIdentifier = "";
                firstName = "";
                lastName = "";
                middleInitial = "";
                type = $("#BlockType").val();
                strOtherText = $("#otherText").val();
                providerId = $("#BlockFor").val();
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
                    "EndBy": $("#EndBy").val(),
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
                "ProviderId": providerId,
                "ExamRoomIdentifier": examRoomIdentifier,
                "IsInformationVerified": $('input:checkbox[name=verifyPatient]')[0].checked,
                "StartDateTime": $("#date").val() + " " + $("#StartTime").val(),
                "EndDateTime": $("#date").val() + " " + $("#EndTime").val(),
                "IsRecurrence": $('input:checkbox[name=chkRecurrence]')[0].checked,
                "Recurrence": recurrenceGroupJson,
                "Description": $("#Description").val(),
                "IsActive": true,
                "Status": appointmentStatus,
                "StatusLocation": strStatusLocation
            };

            var appointmentSaveUrl = "../Appointment/SaveAppointment?appointmentType=" + strAppointmentType + "&appointmentGuid=" + strAppointmentUrl;
            doAjaxCall("POST", appointmentJson, appointmentSaveUrl, this.successSaveAppointment);
        }
        else {
            var errorMessage = "<UL>";
            errorMessage += "<LI>" + INPUT_REQUIRED + "</LI>";
            errorMessage += "</UL>";
            $("#validationSummary")[0].innerHTML = errorMessage;
            $("#validationSummary").focus();
            $("#validationSummary").show();
            $("#assignment-Metadata-Main-Content").scrollTo("#validationSummary", 300);
        }
    },
    ValidateEditAppointmentFields: function (strAppointmentType) {
        if (strAppointmentType == PATIENTVISIT) {
            if ($("#VisitType").val() == "-Select-") {
                return false;
            }
            if (isNullOrEmpty($("#ProviderList").val())) {
                return false;
            }
        }
        if (strAppointmentType == BLOCK) {
            if ($("#BlockType").val() == "-Select-") {
                return false;
            }
            if ($("#BlockFor").val() == "-Select-") {
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
    }
}
