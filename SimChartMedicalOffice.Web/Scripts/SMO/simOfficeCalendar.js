/*
  SimChart for Medical Office 
  simOfficeCalendar.js : Holds all the functions related to Calendar view and Exam Room View in Front Office Module
                          note: Full Calendar plugin is customized  based on Requirements for  SimChart for Medical Office 
*/
var VIEW_MORE_TEXT = "View All";
var SEARCH_VALIDATION_PATIENT = "You must select a patient to search";
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
             /*
             *                  Action : View more link click event handler
             * Input  parameters : From event click eventClick call back in intializeCalendarView function
                                             1) viewMoreDate - Date of the appointment from which view more is clicked
                                             2) xCoor- x-Coordinate of view-more link
                                             3) yCoor- y-Coordinate of view-more link
            */
    viewMoreForDayInMonth: function (viewMoreDate, xCoor, yCoor) {
        startAjaxLoader();
        var titleForPopup = dateFormat(new Date(viewMoreDate), "dddd, mmmm dd");
        var urlForViewMore = "../Appointment/GetViewMoreAppointments?dateString=" + convertDateToString(viewMoreDate);
        $("#view_more").load(urlForViewMore, function () {
            $('#view_more').dialog({
                autoOpen: false,
                modal: false,
                closeOnEscape: false,
                resizable: false,
                position: [xCoor, yCoor],
                open: function () {
                    applyClassForDialogHeader();
                },
                title: titleForPopup,
                width: 250
            });
            $("#view_more").dialog("open");
            closeAjaxLoader();
        });
    },
    /*
    *                  Action : To set the current filter value based on the LHS filters 
    * Input  parameters : From click of Filters Exam room view/Patient/Appointment Type
                                    1) filterChanged - relates to Filters Exam room view/Patient/Appointment Type- appEnum integer
    */
    setFilterTypeValue: function (filterChanged) {
        filterType = filterChanged;
    },
    /*
    *                  Action   : To get the current filter value based on the LHS filters 
    * Return  parameters : From click of Filters Exam room view/Patient/Appointment Type
                                     1) filterChanged - relates to Filters Exam room view/Patient/Appointment Type- appEnum integer
    */
    getFilterTypeValue: function () {
        return filterType;
    },
    /*
    *Action   : Ajax call to refresh/rendar events to calendar view 
    */
    getEventsForCalendar: function () {
        var urlForCalendarRefresh = "../Appointment/GetAppointments?filterType=" + calendarfilterType;
        doAjaxCall("POST", smoCalendar.getCalendarFilterCriteria(), urlForCalendarRefresh, smoCalendar.calendarRefreshSuccess);
    },
    /*
    *Action   : Ajax call to refresh/rendar events to exam room view 
    */
    getExamRoomEventsForCalendar: function () {
        var urlForExamViewCalendarRefresh = "../Appointment/GetAppointmentsForExamView";
        doAjaxCall("POST", smoExamViewCalendarFilter, urlForExamViewCalendarRefresh, smoCalendar.examViewcalendarRefreshSuccess);
    },
    /*
    *Action   : Sucess function for Ajax call to refresh/rendar events to exam room view 
    */
    examViewcalendarRefreshSuccess: function (events) {
        $('#calendarExamRoom').fullCalendar('removeEvents');
        $('#calendarExamRoom').fullCalendar('addEventSource', events.eventList);
        refreshdatePickerForExamView();
    },
    /*
    * Action   : Sucess function for Ajax call to refresh/rendar events to calendar view 
    */
    calendarRefreshSuccess: function (events) {
        $('#calendar').fullCalendar('removeEvents');
        $('#calendar').fullCalendar('addEventSource', events.eventList);
        refreshdatePickerForCalendarView();
    },
    /*
    * Action   : To show the Exam room view floor plan 
    */
    showOfficeFloorPlan: function () {
        $('#office-floor-image_inner_content').dialog({
            autoOpen: false,
            modal: true,
            closeOnEscape: false,
            resizable: false,
            open: function () {
                applyClassForDialogHeader();
            },
            title: 'Exam Room Layout',
            width: 670
        });
        $('#office-floor-image_inner_content').dialog("open");
    },
    /*
    * Action   : To get the Filter criteria for calendar view
    */
    getCalendarFilterCriteria: function () {
        return smoCalendarFilter;
    },
    /*
    *  Action   : To set default filter values for calendar view
    */
    setDefaultSettingsForCalendar: function () {
        smoCurrentCalendarView = calendarAgandaDay;
        smoCurrentViewStartDate = new Date();
        smoCurrentViewEndDate = new Date();
    },
    /*
    * Action   : To set default filter values for exam room view
    */
    setDefaultSettingsForExamView: function () {
        $('#calendarExamRoom').fullCalendar('changeView', calendarResourceDay);
        $('#calendarExamRoom').fullCalendar('gotoDate', new Date());
    },
    /*
    * Action   : To set default filter values for calendar view
    */
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
    /*
    * Action   : To get/set default filter values for calendar view
    */
    getExamRoomViewFilter: function () {
        smoExamViewCalendarFilter = {
            "StartDate": convertDateToString($('#calendarExamRoom').fullCalendar('getDate')),
            "EndDate": convertDateToString($('#calendarExamRoom').fullCalendar('getDate'))
        };
        smoCalendar.getExamRoomEventsForCalendar();
    },
    /*
    * Action   : To get/set  values based on the current view/state of  calendar/exam room view
    */
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
    /*
    *                  Action : To rendar the appointments/events in addNew/edit mode on click.
    * Input  parameters : From eventClick call back in intializeCalendarView function
                                   1) calEvent - calendarEvent object  
                                   2) jsEvent
                                   3) view - current view object  
    */
    editmodeAppointment: function (calEvent, jsEvent, view) {
        var appointmentUrl = calEvent.Url;
        var appointmentType = calEvent.AppointmentType;
        var appointmentDate = convertDateToString(calEvent.start);
        var patientName = calEvent.PatientName;
        var status = calEvent.Status;
        appointment.LoadNewAppointment(appointmentDate, appointmentUrl, appointmentType, patientName, status, calEvent.isRecurrence, calEvent.IsViewMode);
    },
    /*
    *                  Action : To rendar the appointments/events in edit mode on click.
    * Input  parameters : From eventClick call back in intializeCalendarView function
                                    1) calEvent - calendarEvent object  
                                    2) jsEvent
                                    3) view - current view object  
    */
    showAppointmentsOfADay: function (start, end, allDay) {
        var appointmentDate = "";
        var appointmentUrl = "";
        if (smoCalendar.getCurrentDateSelectedInCalendar() < new Date) {
            start = new Date;
        } else {
            start = smoCalendar.getCurrentDateSelectedInCalendar();
        }
        if (!isNullOrEmpty(start) && start != undefined) {
            appointmentDate = convertDateToString(start);
        }
        appointment.LoadNewAppointment(appointmentDate, appointmentUrl);
    },
    /*
    *                  Action : To rendar the appointments/events in edit mode on click.
    * Input  parameters : From eventClick call back in intializeCalendarView function
                                    1) calEvent - calendarEvent object  
                                    2) jsEvent
                                    3) view - current view object  
    */
    appointmentSearchPatients: function () {
        var patientsearchText = $("#searchByPatientCalendar_hidden").val();
        var patientNameText = $("#searchByPatientCalendar_input").val();
        patientsearchText = trimText(patientsearchText);
        var isSearchValid = true;
        if (isNullOrEmpty(patientsearchText)) {
            isSearchValid = false;
            jAlert(SEARCH_VALIDATION_PATIENT, ALERT_TITLE, function (isOk) { });
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
    /*
    * Action : To rendar appointments based on the month navigation
    */
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
    /*
    * Action : Cancel click in Patient search grid
    */
    cancelSearchPatientAppointmnet: function () {
        $("#calendar").show();
        $("#searchPatientForAppointment").hide();
        smoCalendar.resetCalendarFilterToDefault();
    },
    /*
    * Action : To reset calendar filters
    */
    resetCalendarFilterToDefault: function () {
        resetFilterByAppointmentType();
        resetExamRoomFilter();
        resetProviderFilter();
        resetSearchByPatient();
        smoCalendar.setDefaultDayViewForCalendar();
    },
    /*
    * Action : To reset calendar filters
    */
    setDefaultDayViewForCalendar: function () {
        $('#calendar').fullCalendar('changeView', calendarAgandaDay);
        $('#calendar').fullCalendar('gotoDate', new Date());
    },
    /*
    * Action : Ajax call to get the appointments selected in search grid
    */
    appointmentSelectedInPatientSearch: function () {
        $("input[type=radio][name='appointmentpatientSearch']:checked").each(function () {
            var urlForPatientVisitAppointment = "../Appointment/GetPatientVisitAppointment?appointmentUniqueIdentifierUrl=" + this.id;
            doAjaxCall("POST", "", urlForPatientVisitAppointment, smoCalendar.getPatientVisitAppointment);
        });
    },
    /*
    * Action : To load the appointments in popup from search grid
    */
    getPatientVisitAppointment: function (result) {
        if (result.PatientVisitPresent != null && result.PatientVisitPresent != "") {
            appointment.LoadNewAppointment(result.AppointmentDate.toString(), result.PatientVisitPresent.Url, PATIENTVISIT, result.PatientVisitPresent.Name, result.PatientVisitPresent.Status, result.PatientVisitPresent.IsRecurrence,result.IsViewMode);
        }
    }
};
/*
* Action           : To apply text background colors for the tooltips in calendar.
*/
function getColorForToolTip(className) {
    switch (className) {
        case "patient-visit":
            return "#A0C1E8";
        case "blocked-appointment":
            return "#E6B9B8";
        case "other-appointment":
            return "#D7E4BD";
    }
    return false;
}
/*
* Action           : Custom method for highlighting LHS calendar on click of different view
*/
function beforeShowDayCustomized(date) {
    var cssClass = "";
    switch (smoCalendarFilter.CalendarView) {
        case calendarAgandaWeek:
        case calendarMonthView:
            if (date >= convertStringToDate(smoCalendarFilter.StartDate) && date < convertStringToDate(smoCalendarFilter.EndDate)) {
                cssClass = calendarHighlightWeek;
            }
            break;
        case calendarAgandaDay:
            if (date.toDateString() == convertStringToDate(smoCalendarFilter.StartDate).toDateString()) {
                cssClass = calendarHighlightDay;
            }
            break;
    }
    return cssClass;
}
/*
* Action           : To apply text 6pm to the last slot in Full calendar.
*/
function apply6pmTimeLine() {
    $(".fc-slot39 th").html("6pm").css("padding-top", "15px");
}
/*
* Action           : To Refresh the LHS calendar for Calendar View in RHS
*/
function refreshdatePickerForCalendarView() {
    var startDate;
    var endDate;
    $("#dvDate").datepicker('destroy');
    $("#dvDate").datepicker({
        //set the class 'week-highlight' for the whole week
        firstDay: 0,
        beforeShowDay: function (date) {
            var cssClass = beforeShowDayCustomized(date);
            return [true, cssClass];
        },
        defaultDate: new Date(smoCalendarFilter.StartDate),
        onChangeMonthYear: function (year, month, inst) {
            if (inst.currentMonth != inst.selectedMonth) {
                var currentDate = $(this).datepicker('getDate');
                $('#calendar').fullCalendar('gotoDate', new Date(inst.selectedYear, inst.selectedMonth, inst.selectedDay));
            }
        },
        onSelect: function (dateText, inst) {
            startAjaxLoader();
            var currentDate = $(this).datepicker('getDate');
            startDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate() - currentDate.getDay());
            endDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate() - currentDate.getDay() + 6);
            $('.ui-datepicker-calendar').find('.ui-datepicker-current-day a').addClass('ui-state-active'); //Patient search click of day. check if calendar is available in UI. and change to day view
            if ($('#calendar')[0].style.display == 'none') {
                if (smoCalendar.getCurrentCalendarViewObject().name != 'agendaDay') {
                    $('#calendar').fullCalendar('changeView', 'agendaDay');
                }
                resetSearchByPatient();
                $("#searchPatientForAppointment").hide();
                $('#calendar').show();
            }
            $('#calendar').fullCalendar('gotoDate', currentDate);
            closeAjaxLoader();
        },
        selectWeek: true
    });
    $("#dvDate").datepicker("show");
    $('tr').find('td.week-highlight').children('a').addClass("ui-state-active");
    $('tr').find('td.day-highlight').children('a').addClass("ui-state-active day-highlight-a");
    closeAjaxLoader();
}
/*
* Action           : To Refresh the LHS calendar for Exam Room View in RHS
*/
function refreshdatePickerForExamView() {
    var startDate;
    var endDate;
    $("#dvDateExam").datepicker('destroy');
    $("#dvDateExam").datepicker({
        //set the class 'week-highlight' for the whole week
        firstDay: 0,
        defaultDate: new Date(smoExamViewCalendarFilter.StartDate),
        beforeShowDay: function (date) {
            var cssClass = '';
            if (date.toDateString() == smoCurrentExamViewDate.toDateString()) {
                cssClass = 'ui-state-active day-highlight';
            }
            return [true, cssClass];
        },
        onChangeMonthYear: function (year, month, inst) {
            if (inst.currentMonth != inst.selectedMonth) {
                var currentDate = $(this).datepicker('getDate');
                $('#calendarExamRoom').fullCalendar('gotoDate', new Date(inst.selectedYear, inst.selectedMonth, inst.selectedDay));
            }
        },
        onSelect: function (dateText, inst) {
            var currentDate = $(this).datepicker('getDate');
            startDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate() - currentDate.getDay());
            endDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate() - currentDate.getDay() + 6);
            smoCurrentExamViewDate = currentDate;
            $('#calendarExamRoom').fullCalendar('gotoDate', currentDate);
        },
        selectWeek: true
    });
    $('tr').find('td.week-highlight').children('a').addClass("ui-state-active");
    $('tr').find('td.day-highlight').children('a').addClass("ui-state-active day-highlight-a");
    $("#dvDateExam").datepicker("show");
    closeAjaxLoader();
}
/*
* Action           :  Initialize Exam Room View Calendar
*/
function initializeExamRoomView() {
    if (!$('#calendarExamRoom').hasClass('fc')) {
        //Exam Room View Calendar             
        $('#calendarExamRoom').fullCalendar({
            header: {
                left: 'prev,next',
                center: 'title',
                right: ''
            },
            defaultView: 'resourceDay',
            titleFormat: 'MMMM dd, yyyy',
            selectable: true,
            selectHelper: true,
            allDaySlot: false,
            ignoreTimezone: true,
            firstHour: 8,
            minTime: '8:00 am',
            maxTime: '6:00 pm',
            timeFormat: '',
            slotMinutes: 15,
            viewDisplay: function (view) {
                startAjaxLoader();
                smoCalendar.setCurrentExamRoomViewData(view);
                smoCalendar.getExamRoomViewFilter();
            },
            eventAfterRender: function (event, element, view) {
                $(element).find('.fc-event-inner.fc-event-skin').removeClass('fc-event-skin').addClass('event-provider-color');
            },
            eventClick: function (calEvent, jsView, view) { 
                    smoCalendar.editmodeAppointment(calEvent, jsView, view); 
            },
            eventRender: function (event, element) {
                element.qtip({
                    content: replaceHTMLBreak(event.tooltip),
                    position: {
                        target: 'mouse'
                    },
                    overwrite: false,
                    style: {
                        title: {
                            'color': 'Black'
                        },
                        background: getColorForToolTip(event.className[0]),
                        tip: true
                    },
                    show: {
                        when: 'mouseover',
                        fixed: true
                    },
                    hide: {
                        when: 'mouseout',
                        fixed: true
                    }
                });
            },
            contentHeight: 341,
            resources: resourceListForExamRoom
        });
        apply6pmTimeLine();
    } else {
        smoCalendar.getExamRoomEventsForCalendar();
    }
}
/*
* Action           :  Initialize Calendar View Calendar
*/
function initializeCalendarView() {
    if (!$('#calendar').hasClass('fc')) {
        $('#calendar').fullCalendar({
            header: {
                left: 'prev,next',
                center: 'title',
                right: 'agendaDay,agendaWeek,month'
            },
            defaultView: 'agendaDay',
            allDaySlot: false,
            buttonText: {
                month: 'Month',
                week: 'Week',
                day: 'Day'
            },
            firstHour: 8,
            minTime: '8:00am',
            maxTime: '6:00pm',
            selectable: true,
            ignoreTimezone: true,
            slotMinutes: 15,
            selectHelper: true,
            allDayDefault: false,
            titleFormat: {
                month: 'MMMM, yyyy',
                week: "MMMM d{ '&#8212;'[ MMMM] d, yyyy}",
                day: 'dddd, MMMM d, yyyy'
            },
            select: function (start, end, allDay) {
                if ((convertStringToDate(start) >= convertStringToDate(this.start)) && ((convertStringToDate(start) < convertStringToDate(this.end)))) 
                    smoCalendar.showAppointmentsOfADay(start, end, allDay);
                else 
                    return false;
            },
            eventClick: function (calEvent, jsView, view) {
                if (calEvent.title != VIEW_MORE_TEXT) {
                    smoCalendar.editmodeAppointment(calEvent, jsView, view);
                } else {
                    smoCalendar.viewMoreForDayInMonth(calEvent.start, jsView.pageX, jsView.pageY);
                }
            },
            eventRender: function (event, element) {
                if (event.title != VIEW_MORE_TEXT) {
                    element.qtip({
                        content: replaceHTMLBreak(event.tooltip),
                        position: {
                            target: 'mouse'
                        },
                        overwrite: false,
                        style: {
                            title: {
                                'color': 'Black'
                            },
                            background: getColorForToolTip(event.className[0]),
                            tip: true
                        },
                        show: {
                            when: 'mouseover',
                            fixed: true
                        },
                        hide: {
                            when: 'mouseout',
                            fixed: true
                        }
                    });
                }
            },
            timeFormat: '',
            eventAfterRender: function (event, element, view) {
                $(element).find('.fc-event-inner.fc-event-skin').removeClass('fc-event-skin').addClass('event-provider-color');
            },
            viewDisplay: function (view) {
                startAjaxLoader();
                smoCalendar.setCurrentViewFilterData(view);
                apply6pmTimeLine();
            },
            contentHeight: 341
        });
    }
}