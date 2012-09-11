/*Holds all the functions related to Calendar*/
var VIEW_MORE_TEXT = "View All";
var SEARCH_VALIDATION_PATIENT="You must select a patient to search"
var smoCalendarFilter;
var smoCurrentCalendarView;
var smoCurrentDateSelected;
var smoCurrentViewStartDate;
var smoCurrentViewEndDate;
var calendarfilterType = 4;
var smoCurrentExamViewDate = new Date();
var calendarEventsList;
var calendarAppointmentType = 4;
var selectedMonth = "";
var smoExamViewCalendarFilter;
var smoCalendar = {
    showSmoCalendar: function () {

    },
    viewMoreForDayInMonth: function (viewMoreDate, xCoor, yCoor) {
        startAjaxLoader();
        var titleForPopup = dateFormat(new Date(viewMoreDate), "dddd, mmmm dd");
        var urlForViewMore = "../Appointment/GetViewMoreAppointments?dateString=" + convertDateToString(viewMoreDate);
        $("#view_more").load(urlForViewMore, function () {
            var $dialog = $('#view_more').dialog({
                autoOpen: false,
                modal: false,
                closeOnEscape: false,
                resizable: false,
                position: [xCoor, yCoor],
                open: function (event, ui) {
                    applyClassForDialogHeader();
                },
                title: titleForPopup,
                width: 250
            });
            $("#view_more").dialog("open");
            closeAjaxLoader();
        });
    },
    setFilterTypeValue: function (filterChanged) {
        filterType = filterChanged;
    },
    getFilterTypeValue: function () {
        return filterType;
    },
    getEventsForCalendar: function () {
        var urlForCalendarRefresh = "../Appointment/GetAppointments?filterType=" + calendarfilterType;
        doAjaxCall("POST", smoCalendar.getCalendarFilterCriteria(), urlForCalendarRefresh, smoCalendar.calendarRefreshSuccess);
    },
    getExamRoomEventsForCalendar: function () {
        var urlForExamViewCalendarRefresh = "../Appointment/GetAppointmentsForExamView";
        doAjaxCall("POST", smoExamViewCalendarFilter, urlForExamViewCalendarRefresh, smoCalendar.examViewcalendarRefreshSuccess);
    },
    examViewcalendarRefreshSuccess: function (events) {
        $('#calendarExamRoom').fullCalendar('removeEvents');
        $('#calendarExamRoom').fullCalendar('addEventSource', events.eventList);
        refreshdatePickerForExamView();
    },
    calendarRefreshSuccess: function (events) {
        $('#calendar').fullCalendar('removeEvents');
        $('#calendar').fullCalendar('addEventSource', events.eventList);
        refreshdatePickerForCalendarView();
    },
    showOfficeFloorPlan: function () {
        var $dialog = $('#office-floor-image_inner_content').dialog({
            autoOpen: false,
            modal: true,
            closeOnEscape: false,
            resizable: false,
            open: function (event, ui) {
                applyClassForDialogHeader();
            },
            title: 'Exam Room Layout',
            width: 670
        });
        $('#office-floor-image_inner_content').dialog("open");
    },
    getCalendarFilterCriteria: function () {
        return smoCalendarFilter;
    },
    setDefaultSettingsForCalendar: function () {
        smoCurrentCalendarView = "agendaDay";
        smoCurrentViewStartDate = new Date();
        smoCurrentViewEndDate = new Date();
        smoCalendar.setCalendarFilterCriteria();
    },
    setDefaultSettingsForExamView: function () {
        $('#calendarExamRoom').fullCalendar('changeView', 'resourceDay');
        $('#calendarExamRoom').fullCalendar('gotoDate', new Date());
    },
    setCalendarFilterCriteria: function () {
        smoCalendarFilter = {
            "CalendarView": smoCurrentCalendarView,
            "AppointmentType": calendarAppointmentType,
            "Patient": "patient1",
            "ExamRoom": $("#ExamRoomCalendar").val(),
            "ProviderId": $("#ProviderCalendar").val(),
            "StartDate": convertDateToString(smoCurrentViewStartDate),
            "EndDate": convertDateToString(smoCurrentViewEndDate),
            "CurrentDate": convertDateToString(smoCalendar.getCurrentDateSelectedInCalendar())
        };
        smoCalendar.getEventsForCalendar();
    },
    getExamRoomViewFilter: function () {
        smoExamViewCalendarFilter = {
            "StartDate": convertDateToString($('#calendarExamRoom').fullCalendar('getDate')),
            "EndDate": convertDateToString($('#calendarExamRoom').fullCalendar('getDate'))
        };
        smoCalendar.getExamRoomEventsForCalendar();
    },
    getCurrentCalendarViewObject: function () {
        return $('#calendar').fullCalendar('getView');
    },
    getCurrentDateSelectedInCalendar: function () {
        return $('#calendar').fullCalendar('getDate');
    },
    getCurrentDateSelectedInExamRoomCalendar: function () {
        return $('#calendar').fullCalendar('getDate');
    },
    setCurrentViewFilterData: function (viewObj) {
        smoCurrentCalendarView = viewObj.name;
        smoCurrentViewStartDate = viewObj.start;
        smoCurrentViewEndDate = viewObj.end;
        smoCurrentDateSelected = smoCalendar.getCurrentDateSelectedInCalendar();
        smoCalendar.setCalendarFilterCriteria();
    },
    setCurrentExamRoomViewData: function (viewObjExam) {
        smoCurrentExamViewDate = viewObjExam.start;
    },
    getCurrentExamRoomViewData: function () {
        return smoCurrentExamViewDate;
    },
    editmodeAppointment: function (calEvent, jsEvent, view) {
        var appointmentUrl = calEvent.Url;
        var appointmentType = calEvent.AppointmentType;
        var appointmentDate = convertDateToString(calEvent.start);
        var patientName = calEvent.PatientName;
        var status = calEvent.Status;
        appointment.LoadNewAppointment(appointmentDate, appointmentUrl, appointmentType, patientName, status, calEvent.isRecurrence);
    },
    showAppointmentsOfADay: function (start, end, allDay) {
        var appointmentDate = "";
        var appointmentUrl = "";
        if (!isNullOrEmpty(start) && start != undefined) {
            appointmentDate = convertDateToString(start);
        }
        appointment.LoadNewAppointment(appointmentDate, appointmentUrl);
    },
    appointmentSearchPatients: function () {
        var patientsearchText = $("#searchByPatientCalendar_hidden").val();
        var patientNameText = $("#searchByPatientCalendar_input").val();
        patientsearchText = trimText(patientsearchText);
        var isSearchValid = true;
        if (isNullOrEmpty(patientsearchText)) {
            isSearchValid = false;
            jAlert(SEARCH_VALIDATION_PATIENT, ALERT_TITLE, function (isOk) {
            });
        }
        if (isSearchValid) {
            startAjaxLoader();
            if (isNullOrEmpty(selectedMonth)) {
                selectedMonth = convertDateToString(smoCalendarFilter.CurrentDate);
            }

            var data = {
                patientName: patientNameText,
                searchPatientUniqueIndefier: patientsearchText,
                filteredDate: selectedMonth
            };
            $("#searchPatientForAppointment").load("../Appointment/SearchPatientInAppointment?searchPatientUniqueIndefier=", data, function () {
                $("#calendar").hide();
                $("#searchPatientForAppointment").show();
            });
        }
    },
    goToPreviousMonth: function () {
        if (selectedMonth != "") {
            selectedMonth = convertDateToString(convertStringToDate(selectedMonth).setMonth(convertStringToDate(selectedMonth).getMonth() - 1));
            smoCalendar.appointmentSearchPatients();
        }
    },
    goToNextMonth: function () {
        if (selectedMonth != "") {
            selectedMonth = convertDateToString(convertStringToDate(selectedMonth).setMonth(convertStringToDate(selectedMonth).getMonth() + 1));
            smoCalendar.appointmentSearchPatients();
        }
    },
    cancelSearchPatientAppointmnet: function () {
        $("#calendar").show();
        $("#searchPatientForAppointment").hide();
        smoCalendar.resetCalendarFilterToDefault();
    },
    resetCalendarFilterToDefault: function () {
        resetFilterByAppointmentType();
        resetExamRoomFilter();
        resetProviderFilter();
        resetSearchByPatient();
        smoCalendar.setDefaultSettingsForCalendar();
    }
}