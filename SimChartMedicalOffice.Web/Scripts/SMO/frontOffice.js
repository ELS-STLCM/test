var patientMedicalRecordRequestLst = [];
var forms = {
    EnableSaveBtn: function (formValue) {
        enableAButtonForms("savePatientRecord-" + formValue, ORANGE_BUTTON, "navigation-button", "");
    },
    DisableSaveBtn: function (formValue) {
        forms.PatientSearch.DisableSelectBtn("savePatientRecord-" + formValue);
    },
    Cancel: function () {
        forms.PatientRecordsAccessForm.ResetPage();
    },
    ReferralForm: {
        Save: function (formValue) {
            var referralJson;
            var patientGUID = getDivData("referralContent", "patientGUID");
            var isUpdate = true;
            var uniqueIdentifier = null;
            //setDivData(getDivObject("referralContent"), "PatientName", $("#patientName").val());
            if (isUpdate) {
                uniqueIdentifier = getDivData("referralContent", "uniqueIdentifier");
                // if uniqueIdentifier not set , set it as null to save a new form
                if (uniqueIdentifier == "") {
                    uniqueIdentifier = null;
                }
            }
            referralJson = {
                "Id": "0",
                "DiagnosisOrCode": encodeSpecialSymbols($("#diagnosisOrCode").val()),
                "SurgicalProcedureDate": encodeSpecialSymbols($("#surgicalProcedureOrDate").val()),
                "SurgicalClinicalInformation": encodeSpecialSymbols($("#clinicalInformationOrSymptoms").val()),
                "PreviousClinicalTreatments": encodeSpecialSymbols($("#previousClinicalTreatments").val()),
                "PatientName": encodeSpecialSymbols($("#patientName").val()),
                "Address": encodeSpecialSymbols($("#patientAddress").val()),
                "PatientPhone": encodeSpecialSymbols($("#patientPhone").val()),
                "AlternateContact": encodeSpecialSymbols($("#alternateContact").val()),
                "HealthId": encodeSpecialSymbols($("#healthId").val()),
                "DateOfBirth": encodeSpecialSymbols($("#dateOfBirthReferral").val()),
                "Allergies": encodeSpecialSymbols($("#allergies").val()),
                "Medications": encodeSpecialSymbols($("#medications").val()),
                "IsDiabetic": $("input:radio[name=diabetic]:checked").val(),
                "PlaceOfService": encodeSpecialSymbols($("#placeOfService").val()),
                "NumberOfVisits": encodeSpecialSymbols($("#numberOfVisits").val()),
                "AddressOfService": encodeSpecialSymbols($("#serviceAddress").val()),
                "LengthOfStay": encodeSpecialSymbols($("#lengthOfStay").val()),
                "ReferringProvider": encodeSpecialSymbols($("#referringProvider").val()),
                "Phone": encodeSpecialSymbols($("#providerPhone").val()),
                "NameToPrint": encodeSpecialSymbols($("#providerName").val()),
                "NpiNumber": encodeSpecialSymbols($("#NPINumber").val()),
                "Signature": encodeSpecialSymbols($("#signature").val()),
                "Date": encodeSpecialSymbols($("#providingDate").val()),
                "FamilyPhysicianName": encodeSpecialSymbols($("#familyPhysicianName").val()),
                "IsSameReferringPhysician": ($("input[type=checkbox][name=sameAsReferringPhysician]:checked").length == 0 ? "No" : "Yes"),
                "FormInitiatedBy": encodeSpecialSymbols($("#formInitiatedBy").val()),
                "AuthorizationNumber": encodeSpecialSymbols($("#authorizationNumber").val()),
                "EffectiveDate": encodeSpecialSymbols($("#effectiveDate").val()),
                "ExpirationDate": encodeSpecialSymbols($("#expirationDate").val()),
                "NameAndPosition": encodeSpecialSymbols($("#referrerNamePosition").val()),
                "PhoneOfInitiatedPerson": encodeSpecialSymbols($("#referrerPhone").val()),
                "SignatureOfInitiatedPerson": encodeSpecialSymbols($("#referrerSignature").val()),
                "DateInitiated": encodeSpecialSymbols($("#referrerSignatureDate").val()),
                "PatientReferenceId": patientGUID,
                "UniqueIdentifier": uniqueIdentifier
            };
            $.ajax({
                type: "POST",
                dataType: 'json',
                data: JSON.stringify(referralJson),
                url: "../Forms/SaveReferralForm",
                contentType: AJAX_CONTENT_TYPE,
                error: function (result) {
                    if (result != null) {
                        jAlert(SAVE_ERROR, "Alert");
                    }
                },
                success: function (result) {
                    if (result != null) {
                        forms.ReferralForm.ResetPage();
                        var patientName = getDivData("referralContent", "PatientName");
                        forms.DisableSaveBtn(formValue);
                        forms.ReferralForm.DisableControls();
                        setDivData(getDivObject("referralContent"), "uniqueIdentifier", "");
                        setDivData(getDivObject("referralContent"), "patientGUID", "");
                        refreshFrontOfficeTab(7, formValue, patientName);
                    }
                },
                traditional: true
            });
            return false;
        },
        LoadPatientInfo: function (formValue, patientGUID) {
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
                            $("#patientName").val(result.PatientInfo.LastName + ", " + result.PatientInfo.FirstName + " " + result.PatientInfo.MiddleInitial);
                            // setting div data PatientName
                            setDivData(getDivObject("referralContent"), "PatientName", result.PatientInfo.FirstName + " " + result.PatientInfo.LastName);
                            $("#dateOfBirthReferral").val(result.PatientInfo.DateOfBirth);
                            forms.EnableSaveBtn(formValue);
                            forms.ReferralForm.EnableControls();
                            //alert('PriorAuthorizationRequest Info Saved!');
                        }
                        else {
                            jAlert("Patient not found", "Error");
                        }
                    }
                },
                traditional: true
            });
        },
        LoadReferralForm: function (formValue, patientGUID) {
            $.ajax({
                type: "POST",
                dataType: 'json',
                //data: JSON.stringify(priorAuthorizationRequestJson),
                url: "../Forms/LoadReferralForm?patientGUID=" + patientGUID,
                contentType: AJAX_CONTENT_TYPE,
                error: function (result) {
                    if (result != null) {
                        jAlert(LOAD_ERROR, "Alert");
                    }
                },
                success: function (result) {
                    if (result != null) {
                        if (result.ReferralFormObj != null) {
                            forms.ReferralForm.PopulateValuesToControls(result.ReferralFormObj);
                            forms.EnableSaveBtn(formValue);
                            forms.ReferralForm.EnableControls();
                            setDivData(getDivObject("referralContent"), "uniqueIdentifier", result.ReferralFormObj.UniqueIdentifier);
                        }
                        // if no forms are present call patient info
                        else {
                            forms.ReferralForm.LoadPatientInfo(formValue, patientGUID);
                        }
                    }
                },
                traditional: true
            });
        },
        PopulateValuesToControls: function (formValues) {
            $("#diagnosisOrCode").val(formValues.DiagnosisOrCode);
            $("#surgicalProcedureOrDate").val(formValues.SurgicalProcedureDate);
            $("#clinicalInformationOrSymptoms").val(formValues.SurgicalClinicalInformation);
            $("#previousClinicalTreatments").val(formValues.PreviousClinicalTreatments);
            $("#patientName").val(formValues.PatientName);
            $("#patientAddress").val(formValues.Address);
            $("#patientPhone").val(formValues.PatientPhone);
            $("#alternateContact").val(formValues.AlternateContact);
            $("#healthId").val(formValues.HealthId);
            $("#dateOfBirthReferral").val(formValues.DateOfBirth);
            $("#allergies").val(formValues.Allergies);
            $("#medications").val(formValues.Medications);
            $('input:radio[id=diabetic' + formValues.IsDiabetic + ']').attr('checked', true);
            $("#placeOfService").val(formValues.PlaceOfService);
            $("#numberOfVisits").val(formValues.NumberOfVisits);
            $("#serviceAddress").val(formValues.AddressOfService);
            $("#lengthOfStay").val(formValues.LengthOfStay);
            $("#referringProvider").val(formValues.ReferringProvider);
            $("#providerPhone").val(formValues.Phone);
            $("#providerName").val(formValues.NameToPrint);
            $("#NPINumber").val(formValues.NpiNumber);
            $("#signature").val(formValues.Signature);
            $("#providingDate").val(formValues.Date);
            $("#familyPhysicianName").val(formValues.FamilyPhysicianName);
            $("input:checkbox[id=diabetic#sameAsReferringPhysician]").attr("checked", formValues.IsSameReferringPhysician != null ? (formValues.IsSameReferringPhysician.toLowerCase() == "yes" ? true : false) : false);
            $("#formInitiatedBy").val(formValues.FormInitiatedBy);
            $("#authorizationNumber").val(formValues.AuthorizationNumber);
            $("#effectiveDate").val(formValues.EffectiveDate);
            $("#expirationDate").val(formValues.ExpirationDate);
            $("#referrerNamePosition").val(formValues.NameAndPosition);
            $("#referrerPhone").val(formValues.PhoneOfInitiatedPerson);
            $("#referrerSignature").val(formValues.SignatureOfInitiatedPerson);
            $("#referrerSignatureDate").val(formValues.DateInitiated);
        },
        Print: function () {
            var formID = getDivData("referralContent", "uniqueIdentifier");
            if (formID != "") {
                var patientGUID = getDivData("referralContent", "patientGUID");
                window.open("../Forms/FilledReferralFormPrint?patientGUID=" + patientGUID + "&formID=" + formID);
            }
            else {
                window.open("../Forms/EmptyReferralFormPrint");
            }
        },
        ResetPage: function () {
            $("#diagnosisOrCode").val("");
            $("#surgicalProcedureOrDate").val("");
            $("#clinicalInformationOrSymptoms").val("");
            $("#previousClinicalTreatments").val("");
            $("#patientName").val("");
            $("#patientAddress").val("");
            $("#patientPhone").val("");
            $("#alternateContact").val("");
            $("#healthId").val("");
            $("#dateOfBirthReferral").val("");
            $("#allergies").val("");
            $("#medications").val("");
            $('input:radio[name=diabetic]').attr('checked', false);
            $("#placeOfService").val("");
            $("#numberOfVisits").val("");
            $("#serviceAddress").val("");
            $("#lengthOfStay").val("");
            $("#referringProvider").val("");
            $("#providerPhone").val("");
            $("#providerName").val("");
            $("#NPINumber").val("");
            $("#signature").val("");
            $("#providingDate").val("");
            $("#familyPhysicianName").val("");
            $("input:checkbox[id=diabetic#sameAsReferringPhysician]").attr("checked", false);
            $("#formInitiatedBy").val("");
            $("#authorizationNumber").val("");
            $("#effectiveDate").val("");
            $("#expirationDate").val("");
            $("#referrerNamePosition").val("");
            $("#referrerPhone").val("");
            $("#referrerSignature").val("");
            $("#referrerSignatureDate").val("");
            $("#referralContent").scrollTop(0);
            forms.ReferralForm.DisableControls();
        },
        DisableControls: function () {
            $("#diagnosisOrCode").attr('disabled', 'disabled');
            $("#surgicalProcedureOrDate").attr('disabled', 'disabled');
            $("#clinicalInformationOrSymptoms").attr('disabled', 'disabled');
            $("#previousClinicalTreatments").attr('disabled', 'disabled');
            $("#patientName").attr('disabled', 'disabled');
            $("#patientAddress").attr('disabled', 'disabled');
            $("#patientPhone").attr('disabled', 'disabled');
            $("#alternateContact").attr('disabled', 'disabled');
            $("#healthId").attr('disabled', 'disabled');
            $("#dateOfBirthReferral").attr('disabled', 'disabled');
            $("#allergies").attr('disabled', 'disabled');
            $("#medications").attr('disabled', 'disabled');
            $('input:radio[name=diabetic]').attr('disabled', true);
            $("#placeOfService").attr('disabled', 'disabled');
            $("#numberOfVisits").attr('disabled', 'disabled');
            $("#serviceAddress").attr('disabled', 'disabled');
            $("#lengthOfStay").attr('disabled', 'disabled');
            $("#referringProvider").attr('disabled', 'disabled');
            $("#providerPhone").attr('disabled', 'disabled');
            $("#providerName").attr('disabled', 'disabled');
            $("#NPINumber").attr('disabled', 'disabled');
            $("#signature").attr('disabled', 'disabled');
            $("#providingDate").attr('disabled', 'disabled');
            $("#familyPhysicianName").attr('disabled', 'disabled');
            $("input:checkbox[id=diabetic#sameAsReferringPhysician]").attr("disabled", true);
            $("#formInitiatedBy").attr('disabled', 'disabled');
            $("#authorizationNumber").attr('disabled', 'disabled');
            $("#effectiveDate").attr('disabled', 'disabled');
            $("#expirationDate").attr('disabled', 'disabled');
            $("#referrerNamePosition").attr('disabled', 'disabled');
            $("#referrerPhone").attr('disabled', 'disabled');
            $("#referrerSignature").attr('disabled', 'disabled');
            $("#referrerSignatureDate").attr('disabled', 'disabled');
        },
        EnableControls: function () {
            $("#diagnosisOrCode").attr('disabled', false);
            $("#surgicalProcedureOrDate").attr('disabled', false);
            $("#clinicalInformationOrSymptoms").attr('disabled', false);
            $("#previousClinicalTreatments").attr('disabled', false);
            $("#patientName").attr('disabled', false);
            $("#patientAddress").attr('disabled', false);
            $("#patientPhone").attr('disabled', false);
            $("#alternateContact").attr('disabled', false);
            $("#healthId").attr('disabled', false);
            $("#dateOfBirthReferral").attr('disabled', false);
            $("#allergies").attr('disabled', false);
            $("#medications").attr('disabled', false);
            $('input:radio[name=diabetic]').attr('disabled', false);
            $("#placeOfService").attr('disabled', false);
            $("#numberOfVisits").attr('disabled', false);
            $("#serviceAddress").attr('disabled', false);
            $("#lengthOfStay").attr('disabled', false);
            $("#referringProvider").attr('disabled', false);
            $("#providerPhone").attr('disabled', false);
            $("#providerName").attr('disabled', false);
            $("#NPINumber").attr('disabled', false);
            $("#signature").attr('disabled', false);
            $("#providingDate").attr('disabled', false);
            $("#familyPhysicianName").attr('disabled', false);
            $("input:checkbox[id=diabetic#sameAsReferringPhysician]").attr("disabled", false);
            $("#formInitiatedBy").attr('disabled', false);
            $("#authorizationNumber").attr('disabled', false);
            $("#effectiveDate").attr('disabled', false);
            $("#expirationDate").attr('disabled', false);
            $("#referrerNamePosition").attr('disabled', false);
            $("#referrerPhone").attr('disabled', false);
            $("#referrerSignature").attr('disabled', false);
            $("#referrerSignatureDate").attr('disabled', false);
        }
    },
    PatientRecordsAccessForm: {
        //For adding the PatientRecordsAccessForm row to the DB.
        funPatientMedicalRecordRequest: function () {
            patientMedicalRecordRequestLst = [];
            $("input[type=checkbox][name^='MedicalRecordType']:checked").each(function () {
                var recordRequestId = encodeSpecialSymbols($(this).val());
                var recordRequestValue = encodeSpecialSymbols(($(this).val() == "Other") ? $("#MedicalRecordTypeOther").val() : $(this).val());
                var recordRequestJson = {
                    "Id": recordRequestId,
                    "Value": recordRequestValue
                };
                patientMedicalRecordRequestLst.push(recordRequestJson);
            });
        },
        Save: function (formValue) {
            if (forms.PatientRecordsAccessForm.Validate()) {
                var patientRecordsAccessFormJson;
                var patientGUID = getDivData("patient_records_access_main_content", "patientGUID");
                var isUpdate = true;
                var uniqueIdentifier = null;
                //setDivData(getDivObject("patient_records_access_main_content"), "PatientName", $("#PatientName").val());
                if (isUpdate) {
                    uniqueIdentifier = getDivData("patient_records_access_main_content", "uniqueIdentifier");
                    // if uniqueIdentifier not set , set it as null to save a new form
                    if (uniqueIdentifier == "") {
                        uniqueIdentifier = null;
                    }
                }
                patientRecordsAccessFormJson = {
                    "Address": encodeSpecialSymbols($("#Address").val()),
                    "PatientMedicalRecordRequest": patientMedicalRecordRequestLst,
                    "Charge": encodeSpecialSymbols($("#Charge").val()),
                    "CompletedBy": encodeSpecialSymbols($("#CompletedBy").val()),
                    "DateCompleted": encodeSpecialSymbols($("#DateCompleted").val()),
                    "EmergencyPatientCity": encodeSpecialSymbols($("#City").val()),
                    "EmergencyPatientDob": encodeSpecialSymbols($("#DOB").val()),
                    "EmergencyPatientAddress": encodeSpecialSymbols($("#MailingAddress").val()),
                    "EmergencyPatientName": encodeSpecialSymbols($("#PatientName").val()),
                    "EmergencyPatientState": encodeSpecialSymbols($("#State").val()),
                    "EmergencyPatientZipCode": encodeSpecialSymbols($("#ZipCode").val()),
                    "MedicalRecordPeriodFrom": encodeSpecialSymbols($("#MedicalRecordFrom").val()),
                    "MedicalRecordPeriodTo": encodeSpecialSymbols($("#MedicalRecordTo").val()),
                    "Phone": encodeSpecialSymbols($("#Phone").val()),
                    "ReasonforDisclosure": encodeSpecialSymbols($("#ReasonForDisclosure").val()),
                    "ReleasingTo": encodeSpecialSymbols($("#ReleasingTo").val()),
                    "RequestExpiryDate": encodeSpecialSymbols($("#RequestExpire").val()),
                    "Signature": encodeSpecialSymbols($("#Signature").val()),
                    "SignatureDate": encodeSpecialSymbols($("#SignatureDate").val()),
                    "WitnessSignature": encodeSpecialSymbols($("#WitnessSignature").val()),
                    "WitnessSignatureDate": encodeSpecialSymbols($("#WitnessSignatureDate").val()),
                    "PatientReferenceId": patientGUID,
                    "UniqueIdentifier": uniqueIdentifier
                };
                $.ajax({
                    type: "POST",
                    dataType: 'json',
                    data: JSON.stringify(patientRecordsAccessFormJson),
                    url: '../Forms/SavePatientRecordsAccessForm',
                    contentType: AJAX_CONTENT_TYPE,
                    error: function (result) {
                        if (result != null) {
                            jAlert(SAVE_ERROR, "Alert");
                        }
                    },
                    success: function (result) {
                        if (result != null) {
                            forms.DisableSaveBtn(formValue);
                            forms.PatientRecordsAccessForm.ResetPage();
                            var patientName = getDivData("patient_records_access_main_content", "PatientName");
                            setDivData(getDivObject("patient_records_access_main_content"), "uniqueIdentifier", "");
                            setDivData(getDivObject("patient_records_access_main_content"), "patientGUID", "");
                            refreshFrontOfficeTab(7, formValue, patientName);
                        }
                    },
                    traditional: true
                });
                return false;
            }
        },
        LoadPatientInfo: function (formValue, patientGUID) {
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
                            $("#PatientName").val(result.PatientInfo.LastName + ", " + result.PatientInfo.FirstName + " " + result.PatientInfo.MiddleInitial);
                            // setting div data PatientName
                            setDivData(getDivObject("patient_records_access_main_content"), "PatientName", result.PatientInfo.FirstName + " " + result.PatientInfo.LastName);
                            $("#DOB").val(result.PatientInfo.DateOfBirth);
                            forms.EnableSaveBtn(formValue);
                            forms.PatientRecordsAccessForm.EnableControls();
                            //alert('PriorAuthorizationRequest Info Saved!');
                        }
                        else {
                            jAlert("Patient not found", "Error");
                        }
                    }
                },
                traditional: true
            });
        },
        LoadPatientRecordAccess: function (formValue, patientGUID) {
            forms.PatientRecordsAccessForm.HideValidation();
            $.ajax({
                type: "POST",
                dataType: 'json',
                //data: JSON.stringify(priorAuthorizationRequestJson),
                url: "../Forms/LoadPatientRecordsAccessForm?patientGUID=" + patientGUID,
                contentType: AJAX_CONTENT_TYPE,
                error: function (result) {
                    if (result != null) {
                        jAlert(LOAD_ERROR, "Alert");
                    }
                },
                success: function (result) {
                    if (result != null) {
                        if (result.PatientRecordsAccess != null) {
                            $("#Address").val(result.PatientRecordsAccess.Address);
                            $("#City").val(result.PatientRecordsAccess.EmergencyPatientCity);
                            $("#DOB").val(result.PatientRecordsAccess.EmergencyPatientDob);
                            $("#MailingAddress").val(result.PatientRecordsAccess.EmergencyPatientAddress);
                            $("#PatientName").val(result.PatientRecordsAccess.EmergencyPatientName);
                            $("#State").val(result.PatientRecordsAccess.EmergencyPatientState);
                            $("#MedicalRecordFrom").val(result.PatientRecordsAccess.MedicalRecordPeriodFrom);
                            $("#MedicalRecordTo").val(result.PatientRecordsAccess.MedicalRecordPeriodTo);
                            $("#Phone").val(result.PatientRecordsAccess.Phone);
                            $("#ZipCode").val(result.PatientRecordsAccess.EmergencyPatientZipCode);
                            $("#ReasonForDisclosure").val(result.PatientRecordsAccess.ReasonforDisclosure);
                            $("#ReleasingTo").val(result.PatientRecordsAccess.ReleasingTo);
                            $("#RequestExpire").val(result.PatientRecordsAccess.RequestExpiryDate);
                            $("#Signature").val(result.PatientRecordsAccess.Signature);
                            $("#SignatureDate").val(result.PatientRecordsAccess.SignatureDate);
                            $("#WitnessSignature").val(result.PatientRecordsAccess.WitnessSignature);
                            $("#WitnessSignatureDate").val(result.PatientRecordsAccess.WitnessSignatureDate);
                            $("#Charge").val(result.PatientRecordsAccess.Charge);
                            $("#DateCompleted").val(result.PatientRecordsAccess.DateCompleted);
                            $("#CompletedBy").val(result.PatientRecordsAccess.CompletedBy);
                            //$("input[type=checkbox][name^='MedicalRecordType']").each(function() {
                            if (result.PatientRecordsAccess.PatientMedicalRecordRequest != null) {
                                for (var i = 0; i < result.PatientRecordsAccess.PatientMedicalRecordRequest.length; i++) {

                                    $("input[type=checkbox][id='MedicalRecordType_" + result.PatientRecordsAccess.PatientMedicalRecordRequest[i].Id + "']").attr('checked', true);
                                    if (result.PatientRecordsAccess.PatientMedicalRecordRequest[i].Id == "Other") {
                                        $("#MedicalRecordTypeOther").val(result.PatientRecordsAccess.PatientMedicalRecordRequest[i].Value);
                                    }

                                }
                            }
                            forms.EnableSaveBtn(formValue);
                            forms.PatientRecordsAccessForm.EnableControls();
                            setDivData(getDivObject("patient_records_access_main_content"), "uniqueIdentifier", result.PatientRecordsAccess.UniqueIdentifier);
                        }
                        else {
                            //if no Forms present for Patient show default patient info
                            forms.PatientRecordsAccessForm.LoadPatientInfo(formValue, patientGUID);
                        }
                    }
                },
                traditional: true
            });
        },
        ResetPage: function () {
            $("#Address").val("");
            $("#City").val("");
            $("#DOB").val("");
            $("#MailingAddress").val("");
            $("#PatientName").val("");
            $("#State").val("");
            $("#MedicalRecordFrom").val("");
            $("#MedicalRecordTo").val("");
            $("#Phone").val("");
            $("#ZipCode").val("");
            $("#ReasonForDisclosure").val("");
            $("#ReleasingTo").val("");
            $("#RequestExpire").val("");
            $("#Signature").val("");
            $("#SignatureDate").val("");
            $("#WitnessSignature").val("");
            $("#WitnessSignatureDate").val("");
            $("input[type=checkbox][name^='MedicalRecordType']:checked").prop("checked", false);
            $("#MedicalRecordTypeOther").val("");
            $("#Charge").val("");
            $("#DateCompleted").val("");
            $("#CompletedBy").val("");
            forms.PatientRecordsAccessForm.DisableControls();
            $("#patient_records_access_main_content").scrollTop(0);
        },
        Print: function () {
            var formID = getDivData("patient_records_access_main_content", "uniqueIdentifier");
            if (formID != "") {
                var patientGUID = getDivData("patient_records_access_main_content", "patientGUID");
                window.open("../Forms/FilledPatientRecordsAccessFormPrint?patientGUID=" + patientGUID + "&formID=" + formID);
            }
            else {
                window.open("../Forms/EmptyPatientRecordsAccessFormPrint");
            }
        },
        Validate: function () {
            var errorMessage = "<UI>";
            forms.PatientRecordsAccessForm.HideValidation();
            if ($.trim($("#DateCompleted").val()) == "" || $.trim($("#CompletedBy").val()) == "" || $.trim($("#Charge").val()) == "") {
                errorMessage += "<LI>" + INSUFFICIENT_INFO + "</LI>";
                errorMessage += "</UI>";
                //                $("#DateCompleted").addClass("mandatory_field");
                //                $("#CompletedBy").addClass("mandatory_field");
                //                $("#Charge").addClass("mandatory_field");
                //                $(".mandatory_field").live('keyup', function () {
                //                    $("#"+this.name+"_label").removeClass("mandatory_field_highlight");
                //                });

                //                $("#DateCompleted_label").addClass("mandatory_field_highlight");
                //                $("#CompletedBy_label").addClass("mandatory_field_highlight");
                //                $("#Charge_label").addClass("mandatory_field_highlight");

                $("#validationSummary")[0].innerHTML = errorMessage;
                $("#validationSummary").show();
                $("#patient_records_access_main_content").scrollTop(0);
                return false;
            }
            return true;
        },
        HideValidation: function () {
            $(".mandatory_field_highlight").removeClass("mandatory_field_highlight");
            $("#validationSummary").empty();
            $("#validationSummary").hide();
        },
        OnPageLoad: function (formValue) {
            // if Student mode -- needs to be dynamic
            var isStudent = true;
            $('#MedicalRecordTypeOther').attr('disabled', true);
            $("input[type=checkbox][id=MedicalRecordType_Other]").click(function () {
                if ($(this).attr('checked')) {
                    $('#MedicalRecordTypeOther').attr('disabled', false);
                }
                else {
                    $('#MedicalRecordTypeOther').val("");
                    $('#MedicalRecordTypeOther').attr('disabled', true);
                }
            });
            if (isStudent) {
                $("#Address").attr('disabled', 'disabled');
                $("#City").attr('disabled', 'disabled');
                $("#DOB").attr('disabled', 'disabled');
                $("#MailingAddress").attr('disabled', 'disabled');
                $("#PatientName").attr('disabled', 'disabled');
                $("#State").attr('disabled', 'disabled');
                $("#ZipCode").attr('disabled', 'disabled');
                $("#MedicalRecordFrom").attr('disabled', 'disabled');
                $("#MedicalRecordTo").attr('disabled', 'disabled');
                $("#Phone").attr('disabled', 'disabled');
                $("#ReasonForDisclosure").attr('disabled', 'disabled');
                $("#ReleasingTo").attr('disabled', 'disabled');
                $("#RequestExpire").attr('disabled', 'disabled');
                $("#Signature").attr('disabled', 'disabled');
                $("#SignatureDate").attr('disabled', 'disabled');
                $("#WitnessSignature").attr('disabled', 'disabled');
                $("#WitnessSignatureDate").attr('disabled', 'disabled');
                $("input[type=checkbox][name^='MedicalRecordType']").attr('disabled', true);
            }
            forms.PatientRecordsAccessForm.HideValidation();
            forms.PatientRecordsAccessForm.DisableControls();
            forms.PatientSearch.DisableSelectBtn("savePatientRecord-" + formValue);
        },
        DisableControls: function () {
            $("#Charge").attr('disabled', 'disabled');
            $("#DateCompleted").attr('disabled', 'disabled');
            $("#CompletedBy").attr('disabled', 'disabled');
        },
        EnableControls: function () {
            $("#Charge").attr('disabled', false);
            $("#DateCompleted").attr('disabled', false);
            $("#CompletedBy").attr('disabled', false);
        }

    },
    PriorauthorizationRequestForm: {
        Save: function (formValue) {
            var patientGUID = getDivData("prior_authorization_request_main_content", "patientGUID");
            var priorAuthorizationRequestJson;
            var isUpdate = true;
            var uniqueIdentifier = null;
            //setDivData(getDivObject("prior_authorization_request_main_content"), "PatientName", $("#memberName").val());
            if (isUpdate) {
                uniqueIdentifier = getDivData("prior_authorization_request_main_content", "uniqueIdentifier");
                // if uniqueIdentifier not set , set it as null to save a new form
                if (uniqueIdentifier == "") {
                    uniqueIdentifier = null;
                }
            }
            priorAuthorizationRequestJson = {
                "Id": "0",
                "MemberName": encodeSpecialSymbols($("#memberName").val()),
                "MemberId": encodeSpecialSymbols($("#memberId").val()),
                "DateOfBirth": encodeSpecialSymbols($("#dateOfBirth").val()),
                "OrderingPhysician": encodeSpecialSymbols($("#orderingPhysician").val()),
                "PhysicianAddress": encodeSpecialSymbols($("#physicianAddress").val()),
                "Provider": encodeSpecialSymbols($("#provider").val()),
                "ProviderFax": encodeSpecialSymbols($("#providerFax").val()),
                "ProviderPhone": encodeSpecialSymbols($("#providerPhone").val()),
                "ProviderContactNumber": encodeSpecialSymbols($("#providerContactNumber").val()),
                "ProviderPlaceOfService": encodeSpecialSymbols($("#placeOfService").val()),
                "ServiceRequested": encodeSpecialSymbols($("#serviceRequested").val()),
                "ServiceFrequency": encodeSpecialSymbols($("#serviceFrequency").val()),
                "DiagonsisOrICD9Code8": encodeSpecialSymbols($("#diagnosisICD9Code8").val()),
                "ProcedureCPT4Codes": encodeSpecialSymbols($("#procedureCPT4Codes").val()),
                "InjuryRelated": encodeSpecialSymbols($('input:radio[name=injuryRelated]:checked').val()),
                "StartingServiceDate": encodeSpecialSymbols($("#startingServiceDates").val()),
                "EndingServiceDate": encodeSpecialSymbols($("#endingServiceDates").val()),
                "DateOfInjury": encodeSpecialSymbols($("#dateOfInjuryRelated").val()),
                "WorkerCompentationRelated": encodeSpecialSymbols($('input:radio[name=workersCompensationRelated]:checked').val()),
                "WorkerCompDateOfInjury": encodeSpecialSymbols($("#dateOfInjuryWorkersCompensationRelated").val()),
                "AuthorizationNumber": encodeSpecialSymbols($("#authorizationNumber").val()),
                "EffectiveDate": encodeSpecialSymbols($("#effectiveDate").val()),
                "ExpiryDate": encodeSpecialSymbols($("#expirationDate").val()),
                "PatientReferenceId": patientGUID,
                "UniqueIdentifier": uniqueIdentifier
            };
            $.ajax({
                type: "POST",
                dataType: 'json',
                data: JSON.stringify(priorAuthorizationRequestJson),
                url: "../Forms/SavePriorAuthorizationRequestForm",
                contentType: AJAX_CONTENT_TYPE,
                error: function (result) {
                    if (result != null) {
                        jAlert(SAVE_ERROR, "Alert");
                    }
                },
                success: function (result) {
                    if (result != null) {
                        forms.PriorauthorizationRequestForm.ResetPage();
                        forms.DisableSaveBtn(formValue);

                        var patientName = getDivData("prior_authorization_request_main_content", "PatientName");
                        forms.PriorauthorizationRequestForm.DisableControls();
                        setDivData(getDivObject("prior_authorization_request_main_content"), "uniqueIdentifier", "");
                        setDivData(getDivObject("prior_authorization_request_main_content"), "patientGUID", "");
                        refreshFrontOfficeTab(7, formValue, patientName);
                    }
                },
                traditional: true
            });
            return false;
        },
        LoadPatientInfo: function (formValue, patientGUID) {
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
                            $("#memberName").val(result.PatientInfo.LastName + ", " + result.PatientInfo.FirstName + " " + result.PatientInfo.MiddleInitial);
                            // setting div data PatientName
                            setDivData(getDivObject("prior_authorization_request_main_content"), "PatientName", result.PatientInfo.FirstName + " " + result.PatientInfo.LastName);
                            $("#dateOfBirth").val(result.PatientInfo.DateOfBirth);
                            forms.EnableSaveBtn(formValue);
                            forms.PriorauthorizationRequestForm.EnableControls();
                            //alert('PriorAuthorizationRequest Info Saved!');
                        }
                        else {
                            jAlert("Patient not found", "Error");
                        }
                    }
                },
                traditional: true
            });
        },
        LoadPrioAuthorizationRequestForm: function (formValue, patientGUID) {
            $.ajax({
                type: "POST",
                dataType: 'json',
                //data: JSON.stringify(priorAuthorizationRequestJson),
                url: "../Forms/LoadPriorAuthorizationRequestForm?patientGUID=" + patientGUID,
                contentType: AJAX_CONTENT_TYPE,
                error: function (result) {
                    if (result != null) {
                        jAlert(LOAD_ERROR, "Alert");
                    }
                },
                success: function (result) {
                    if (result != null) {
                        if (result.PriorAuthorizationRequest != null) {
                            $("#memberName").val(result.PriorAuthorizationRequest.MemberName);
                            $("#memberId").val(result.PriorAuthorizationRequest.MemberId);
                            $("#dateOfBirth").val(result.PriorAuthorizationRequest.DateOfBirth);
                            $("#orderingPhysician").val(result.PriorAuthorizationRequest.OrderingPhysician);
                            $("#physicianAddress").val(result.PriorAuthorizationRequest.PhysicianAddress);
                            $("#provider").val(result.PriorAuthorizationRequest.Provider);
                            $("#providerFax").val(result.PriorAuthorizationRequest.ProviderFax);
                            $("#providerPhone").val(result.PriorAuthorizationRequest.ProviderPhone);
                            $("#providerContactNumber").val(result.PriorAuthorizationRequest.ProviderContactNumber);
                            $("#placeOfService").val(result.PriorAuthorizationRequest.ProviderPlaceOfService);
                            $("#serviceRequested").val(result.PriorAuthorizationRequest.ServiceRequested);
                            $("#serviceFrequency").val(result.PriorAuthorizationRequest.ServiceFrequency);
                            $("#diagnosisICD9Code8").val(result.PriorAuthorizationRequest.DiagonsisOrICD9Code8);
                            $("#procedureCPT4Codes").val(result.PriorAuthorizationRequest.ProcedureCPT4Codes);
                            $("#startingServiceDates").val(result.PriorAuthorizationRequest.StartingServiceDate);
                            $("#endingServiceDates").val(result.PriorAuthorizationRequest.EndingServiceDate);
                            $("#dateOfInjuryRelated").val(result.PriorAuthorizationRequest.DateOfInjury);
                            $("#dateOfInjuryWorkersCompensationRelated").val(result.PriorAuthorizationRequest.WorkerCompDateOfInjury);
                            $("#authorizationNumber").val(result.PriorAuthorizationRequest.AuthorizationNumber);
                            $("#effectiveDate").val(result.PriorAuthorizationRequest.EffectiveDate);
                            $("#expirationDate").val(result.PriorAuthorizationRequest.ExpiryDate);
                            $('input:radio[id=injuryRelated' + result.PriorAuthorizationRequest.InjuryRelated + ']').attr('checked', true);
                            $('input:radio[id=workersCompensationRelated' + result.PriorAuthorizationRequest.WorkerCompentationRelated + ']').attr('checked', true);

                            forms.EnableSaveBtn(formValue);
                            forms.PriorauthorizationRequestForm.EnableControls();
                            setDivData(getDivObject("prior_authorization_request_main_content"), "uniqueIdentifier", result.PriorAuthorizationRequest.UniqueIdentifier);
                        }
                        else {
                            //if no Forms present for Patient show default patient info
                            forms.PriorauthorizationRequestForm.LoadPatientInfo(formValue, patientGUID);
                        }
                    }
                },
                traditional: true
            });
        },
        Print: function () {
            var formID = getDivData("prior_authorization_request_main_content", "uniqueIdentifier");
            if (formID != "") {
                var patientGUID = getDivData("prior_authorization_request_main_content", "patientGUID");
                window.open("../Forms/FilledPriorAuthorizationRequestPrint?patientGUID=" + patientGUID + "&formID=" + formID);
            }
            else {
                window.open("../Forms/EmptyPriorAuthorizationRequestPrint");
            }
        },
        ResetPage: function () {
            $("#memberName").val("");
            $("#memberId").val("");
            $("#dateOfBirth").val("");
            $("#orderingPhysician").val("");
            $("#physicianAddress").val("");
            $("#provider").val("");
            $("#providerFax").val("");
            $("#providerPhone").val("");
            $("#providerContactNumber").val("");
            $("#placeOfService").val("");
            $("#serviceRequested").val("");
            $("#serviceFrequency").val("");
            $("#diagnosisICD9Code8").val("");
            $("#procedureCPT4Codes").val("");
            $("#startingServiceDates").val("");
            $("#endingServiceDates").val("");
            $("#dateOfInjuryRelated").val("");
            $("#dateOfInjuryWorkersCompensationRelated").val("");
            $("#authorizationNumber").val("");
            $("#effectiveDate").val("");
            $("#expirationDate").val("");
            $('input:radio[name=injuryRelated]').attr('checked', false);
            $('input:radio[name=workersCompensationRelated]').attr('checked', false);
            forms.PriorauthorizationRequestForm.DisableControls();
            $("#prior_authorization_request_main_content").scrollTop(0);
        },
        DisableControls: function () {
            $("#memberName").attr('disabled', 'disabled');
            $("#memberId").attr('disabled', 'disabled');
            $("#dateOfBirth").attr('disabled', 'disabled');
            $("#orderingPhysician").attr('disabled', 'disabled');
            $("#physicianAddress").attr('disabled', 'disabled');
            $("#provider").attr('disabled', 'disabled');
            $("#providerFax").attr('disabled', 'disabled');
            $("#providerPhone").attr('disabled', 'disabled');
            $("#providerContactNumber").attr('disabled', 'disabled');
            $("#placeOfService").attr('disabled', 'disabled');
            $("#serviceRequested").attr('disabled', 'disabled');
            $("#serviceFrequency").attr('disabled', 'disabled');
            $("#diagnosisICD9Code8").attr('disabled', 'disabled');
            $("#procedureCPT4Codes").attr('disabled', 'disabled');
            $("#startingServiceDates").attr('disabled', 'disabled');
            $("#endingServiceDates").attr('disabled', 'disabled');
            $("#dateOfInjuryRelated").attr('disabled', 'disabled');
            $("#dateOfInjuryWorkersCompensationRelated").attr('disabled', 'disabled');
            $("#authorizationNumber").attr('disabled', 'disabled');
            $("#effectiveDate").attr('disabled', 'disabled');
            $("#expirationDate").attr('disabled', 'disabled');
            $('input:radio[name=injuryRelated]').attr('disabled', true);
            $('input:radio[name=workersCompensationRelated]').attr('disabled', true);
        },
        EnableControls: function () {
            $("#memberName").attr('disabled', false);
            $("#memberId").attr('disabled', false);
            $("#dateOfBirth").attr('disabled', false);
            $("#orderingPhysician").attr('disabled', false);
            $("#physicianAddress").attr('disabled', false);
            $("#provider").attr('disabled', false);
            $("#providerFax").attr('disabled', false);
            $("#providerPhone").attr('disabled', false);
            $("#providerContactNumber").attr('disabled', false);
            $("#placeOfService").attr('disabled', false);
            $("#serviceRequested").attr('disabled', false);
            $("#serviceFrequency").attr('disabled', false);
            $("#diagnosisICD9Code8").attr('disabled', false);
            $("#procedureCPT4Codes").attr('disabled', false);
            $("#startingServiceDates").attr('disabled', false);
            $("#endingServiceDates").attr('disabled', false);
            $("#dateOfInjuryRelated").attr('disabled', false);
            $("#dateOfInjuryWorkersCompensationRelated").attr('disabled', false);
            $("#authorizationNumber").attr('disabled', false);
            $("#effectiveDate").attr('disabled', false);
            $("#expirationDate").attr('disabled', false);
            $('input:radio[name=injuryRelated]').attr('disabled', false);
            $('input:radio[name=workersCompensationRelated]').attr('disabled', false);
        }
    },
    NoticeOfPrivacyPractice: {
        Save: function (formValue) {
            var noticeOfPrivacyPracticeJson;
            noticeOfPrivacyPracticeJson = {
                "FormName": "NoticeOfPrivacyPractice"
            };
            $.ajax({
                type: "POST",
                dataType: 'json',
                data: JSON.stringify(noticeOfPrivacyPracticeJson),
                url: "../Forms/SaveNoticeOfPrivacyPractice",
                contentType: AJAX_CONTENT_TYPE,
                error: function (result) {
                    if (result != null) {
                        alert('Error');
                    }
                },
                success: function (result) {
                    if (result != null) {
                        var patientName = getDivData("notice_form_main_content", "PatientName");
                        setDivData(getDivObject("notice_form_main_content"), "uniqueIdentifier", "");
                        setDivData(getDivObject("notice_form_main_content"), "patientGUID", "");
                        refreshFrontOfficeTab(7, formValue, patientName);
                    }
                },
                traditional: true
            });
            return false;
        },
        LoadPatientInfo: function (formValue, patientGUID) {
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
                            setDivData(getDivObject("notice_form_main_content"), "PatientName", result.PatientInfo.FirstName + " " + result.PatientInfo.LastName);
                            enableAButtonForms("savePatientRecord-1", ORANGE_BUTTON, "navigation-button", "");
                            //alert('PriorAuthorizationRequest Info Saved!');
                        }
                        else {
                            jAlert("Patient not found", "Error");
                        }
                    }
                },
                traditional: true
            });
        }
    },
    PatientBillofRights: {
        Save: function (formValue) {
            var billOfRightsJson;
            billOfRightsJson = {
                "FormName": "PatientBillofRights"
            };
            $.ajax({
                type: "POST",
                dataType: 'json',
                data: JSON.stringify(billOfRightsJson),
                url: "../Forms/SavePatientBillofRights",
                contentType: AJAX_CONTENT_TYPE,
                error: function (result) {
                    if (result != null) {
                        alert('Error');
                    }
                },
                success: function (result) {
                    if (result != null) {
                        var patientName = getDivData("billOfRights", "PatientName");
                        setDivData(getDivObject("billOfRights"), "uniqueIdentifier", "");
                        setDivData(getDivObject("billOfRights"), "patientGUID", "");
                        refreshFrontOfficeTab(7, formValue, patientName);
                    }
                },
                traditional: true
            });
            return false;
        },
        LoadPatientInfo: function (formValue, patientGUID) {
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
                            setDivData(getDivObject("billOfRights"), "PatientName", result.PatientInfo.FirstName + " " + result.PatientInfo.LastName);
                            enableAButtonForms("savePatientRecord-3", ORANGE_BUTTON, "navigation-button", "");
                            //alert('PriorAuthorizationRequest Info Saved!');
                        }
                        else {
                            jAlert("Patient not found", "Error");
                        }
                    }
                },
                traditional: true
            });
        }
    },
    PatientSearch: {
        Initialize: function () {
            $("#PatientSearch").dialog({ height: 450,
                width: 850,
                modal: true,
                position: 'center',
                resizable: false,
                autoOpen: true,
                closeOnEscape: false,
                title: 'Patient Search',
                open: function (event, ui) {
                    $("#filterDateOfBirth").datepicker("hide");
                    applyBlueClassForDialogHeader();
                    forms.PatientSearch.DisableSelectBtn("PatientSearch_BtnSelect");
                    //                    disableAButtonForForms("PatientSearch_BtnSelect");
                    removeCloseIconForDialogHeader(this);
                    applyBlueBorderForDialogHeader();
                },
                overlay: { opacity: 0.5, background: 'black' }
            });
        },
        LoadPatientSearch: function (formValue) {
            $("#PatientSearchGrid").load("../Forms/FormsPatientSearch", function () {
                //fix for iframe coming above dialog for read-only forms
                forms.PatientSearch.HideOrShowIFrameBeforeDialog();
                forms.PatientSearch.Initialize();

                //reset current form
                forms.PatientSearch.ResetFormBeforePatientSearch(formValue);
            });
            //various click events
            // Patient Select and Cancel
            $("#PatientSearch_BtnSelect").live('click', function () {
                // close PatientSearch dailog
                forms.PatientSearch.SelectPatient(formValue);

                //fix for reshowing the hidden iFrame for read-only forms
                forms.PatientSearch.HideOrShowIFrameBeforeDialog();

                forms.PatientSearch.ClosePatientSearch();
            });
            $("#PatientSearch_BtnCancel").live('click', function () {
                var status = "Are you sure you want to cancel? Your changes will not be saved.";
                jConfirm(status, 'Cancel', function (isCancel) {
                    if (isCancel) {
                        //fix for reshowing the hidden iFrame for read-only forms
                        forms.PatientSearch.HideOrShowIFrameBeforeDialog();

                        forms.PatientSearch.ClosePatientSearch();
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
                forms.PatientSearch.DisableSelectBtn("PatientSearch_BtnSelect");
                patientDataTable.fnDraw();
            });

            // Patient Select event
            $("input[type=radio][name=selectPatient]").live('change', function () {
                if ($("#PatientSearch_BtnSelect").is(":disabled")) {
                    enableAButtonForms("PatientSearch_BtnSelect", ORANGE_BUTTON, "navigation-button", "");
                }
            });
        },
        HideOrShowIFrameBeforeDialog: function () {
            if (GetBrowserType() == BROWSER_IE) {
                //fix for iframe coming above dialog for read-only forms
                if ($(".forms-pdf-content").length > 0 && $(".forms-pdf-content").is(':visible')) {
                    $(".forms-pdf-content").hide();
                }
                else {
                    $(".forms-pdf-content").show();
                }
            }
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
        },
        SelectPatient: function (formValue) {
            $("input[type=radio][name='selectPatient']:checked").each(function () {
                forms.PatientSearch.LoadPatientFormType(formValue, this.id);
            });
        },
        LoadPatientFormType: function (formValue, patientGUID) {
            switch (formValue) {
                case '1':
                    //setDivData(getDivObject("referralContent"), "patientGUID", patientGUID);
                    forms.NoticeOfPrivacyPractice.LoadPatientInfo(formValue, patientGUID);
                    break;
                case '2':
                    setDivData(getDivObject("referralContent"), "patientGUID", patientGUID);
                    // if saved form data needed loadForm else Patient info 
                    //forms.ReferralForm.LoadReferralForm(formValue, patientGUID);
                    forms.ReferralForm.LoadPatientInfo(formValue, patientGUID);
                    break;
                case '3':
                    //setDivData(getDivObject("referralContent"), "patientGUID", patientGUID);
                    //forms.ReferralForm.LoadReferralForm(formValue, patientGUID);
                    forms.PatientBillofRights.LoadPatientInfo(formValue, patientGUID);
                    break;
                case '5':
                    setDivData(getDivObject("patient_records_access_main_content"), "patientGUID", patientGUID);
                    // if saved form data needed loadForm else Patient info 
                    //forms.PatientRecordsAccessForm.LoadPatientRecordAccess(formValue, patientGUID);
                    forms.PatientRecordsAccessForm.LoadPatientInfo(formValue, patientGUID);
                    break;
                case '6':
                    setDivData(getDivObject("prior_authorization_request_main_content"), "patientGUID", patientGUID);
                    // if saved form data needed loadForm else Patient info 
                    //forms.PriorauthorizationRequestForm.LoadPrioAuthorizationRequestForm(formValue, patientGUID);
                    forms.PriorauthorizationRequestForm.LoadPatientInfo(formValue, patientGUID);
                    break;
                default:
            }
        },
        ResetFormBeforePatientSearch: function (formValue) {
            switch (formValue) {
                case '2':
                    setDivData(getDivObject("referralContent"), "patientGUID", "");
                    forms.ReferralForm.ResetPage();
                    break;
                case '5':
                    setDivData(getDivObject("patient_records_access_main_content"), "patientGUID", "");
                    forms.PatientRecordsAccessForm.ResetPage();
                    break;
                case '6':
                    setDivData(getDivObject("prior_authorization_request_main_content"), "patientGUID", "");
                    forms.PriorauthorizationRequestForm.ResetPage();
                    break;
                default:
            }
        },
        //        EnableSelectBtn: function () {
        //            $("#PatientSearch_BtnSelect").removeClass('disabled-text');
        //            $("#PatientSearch_BtnSelect").attr('disabled', false);
        //        },
        DisableSelectBtn: function (buttonId) {
            $("#" + buttonId).removeClass('transaction-button').removeClass('navigation-button').removeClass('cancel-button').removeClass('remove-button');
            $("#" + buttonId).addClass('disabled-button');
            $("#" + buttonId + "-LeftCurve").attr('src', '../Content/Images/Buttons/Button_left_curve_grey.jpg');
            $("#" + buttonId + "-RightCurve").attr('src', '../Content/Images/Buttons/Button_right_curve_grey.jpg');
            $("#" + buttonId).attr('disabled', true);
        }

    }
};
    
    