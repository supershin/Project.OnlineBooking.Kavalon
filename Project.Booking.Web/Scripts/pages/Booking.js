
var Booking = {
    currentCustomerEdit: null,
    init: function () {
        $(".btn-cancel").unbind('click').click(function () {
            Booking.CancelBookingModal($(this).attr("data-id"));
        });

        $(".btn-payment").unbind('click').click(function () {
            Booking.GetTermPayment($(this).attr("data-id"));
            return false;
        });
    },
    initSaveBooking: function () {
        
        $("#btn-book-unit").unbind("click").click(e => {
            var unitID = $("#btn-book-unit").attr("data-id");
            Booking.ConfirmBookingModal(unitID);
        });

        $("#btn-book-back").click(() => {

            //$('.nav-tabs a[href="#detail-booking"]').tab('show');
            $("#a-detail-booking").click();
            $('html, body').animate({
                scrollTop: $("#div-menu").offset().top
            }, 1000);
        });

    },
    SaveBooking: function (unitID) {
        $('#btn-close').click();
        app.LoadWaitID("#div-unit-selected", true);

        $.post(baseUrl + 'booking/savebooking', { unitID: unitID }, function (res) {
            app.ajaxComplete(res,
                function () {                                       
                    $("#div-unit-selected").empty().html(res.htmlPartialUnitDetail);
                    //$("#span-quota").text(res.quota);
                    $("#div-my-quota").text(res.myQuota);
                    $("#div-used-quota").text(res.useQuota);
                    Booking.BookingSuccessModal();
                    //window.location = baseUrl + 'booking';
                },
                function () {
                    $("#div-unit-selected").empty().html(res.htmlPartialUnitDetail);
                    //$("#span-quota").text(res.quota);
                    $("#div-my-quota").text(res.myQuota);
                    $("#div-used-quota").text(res.useQuota);
                    if (res.message == "You do not have quota")
                        Booking.BookingErrorQuotaModal();
                    else if (res.message == "Unit not available")
                        Booking.BookingErrorUnitModal();
                    else app.notify('error', res.message);
                },false,false);
            
            app.LoadWaitID("#div-unit-selected", false);
        });
    },
    CancelBookingModal: function (bookingID) {

        $('#btn-confirm').unbind("click");

        $('#modal-confirm').modal({
            backdrop: 'static',
            keyboard: true
        });

        $('#btn-confirm').click(function (e) {
            Booking.SaveCancelBooking(bookingID);
            return false;
        });

    },
    ConfirmBookingModal: function (unitID) {

        $('#btn-confirm-book').unbind("click");

        $('#modal-confirm-book').modal({
            backdrop: 'static',
            keyboard: true
        });

        $('#btn-confirm-book').unbind('click').click(function (e) {
            Booking.SaveBooking(unitID);
            return false;
        });

    },
    SaveCancelBooking: function (bookingID) {
        app.LoadWait(true);
        $.post(baseUrl + 'booking/savecancelbooking', { bookingID: bookingID }, function (res) {
            app.ajaxComplete(res, function () {
                window.location.reload();
            }, () => {
                setTimeout(function () {
                    window.location.reload();
                }, 3000);
            }, false);
            app.LoadWait(false);
        });
    },
    GetTermPayment: function (bookingID) {
        app.LoadWaitID("#div-booking-unit-" + bookingID, true);
        $.post(baseUrl + 'booking/gettermpayment', { bookingID: bookingID }, function (res) {
            app.ajaxComplete(res, function () {
                $("#div-term-payment").empty().html(res.htmlTermPayment);
                Booking.TermPaymentModal();
            },
                function () {
                    $("#div-booking-detail-" + bookingID).empty().html(res.htmlDetail);
                    $("#div-booking-action-" + bookingID).empty().html(res.htmlAction);
                    $("#div-booking-customer-" + bookingID).empty().html(res.htmlCustomer);
                }, false);
            app.LoadWaitID("#div-booking-unit-" + bookingID, false);
        });
        return false;
    },
    TermPaymentModal: function () {
        $('#modal-term-payment').modal({
            backdrop: 'static',
            keyboard: true
        });
    },
    SetEnabledCustomer: function (bookingID) {
        $("input[data-id='" + bookingID + "']").removeClass('customer-disabled');
        $("input[data-id='" + bookingID + "']").addClass('customer-enabled');
        $("span[data-id='" + bookingID + "']").removeClass('sp-customer-enabled');
        $("span[data-id='" + bookingID + "']").addClass('sp-customer-disabled');
        $("#customer-action-" + bookingID).show();

        //set current customer edit to object json
        //Booking.currentCustomerEdit = app.serializeFormJSON($("#frm-customer-edit-" + bookingID));

        return false;
    },
    SetDisabledCustomer: function (bookingID) {
        $("input[data-id='" + bookingID + "']").removeClass('customer-enabled');
        $("input[data-id='" + bookingID + "']").addClass('customer-disabled');
        $("span[data-id='" + bookingID + "']").removeClass('sp-customer-disabled');
        $("span[data-id='" + bookingID + "']").addClass('sp-customer-enabled');
        $("#customer-action-" + bookingID).hide();

        //set current customer edit to element control
        //app.populate($("#frm-customer-edit-" + bookingID), Booking.currentCustomerEdit);        

        return false;
    },
    SaveBookingCustomer: function (bookingID) {
        var form = "frm-customer-edit-" + bookingID;

        app.LoadWaitID("#div-booking-unit-" + bookingID, true);
        var formData = new FormData(document.getElementById(form));
        formData.append("ID", bookingID);

        $.ajax({
            url: baseUrl + 'booking/SaveBookingCustomer',
            type: 'post',
            dataType: 'json',
            processData: false,
            contentType: false,
            success: function (res) {
                app.ajaxComplete(res, () => {
                    Booking.SetBookingData(res, bookingID);
                }, function () {
                    Booking.SetBookingData(res, bookingID);
                }, true);
                app.LoadWaitID("#div-booking-unit-" + bookingID, false);
            },
            data: formData
        });

        return false;
    },
    SetBookingData: function (res, bookingID) {
        if (res.refresh) {
            $("#div-booking-detail-" + bookingID).empty().html(res.htmlDetail);
            $("#div-booking-action-" + bookingID).empty().html(res.htmlAction);
            $("#div-booking-customer-" + bookingID).empty().html(res.htmlCustomer);
        }
    },
    BookingSuccessModal: function () {
        $(".a-close-modal").click();
        $('#booking-success-modal').modal();
    },
    BookingErrorQuotaModal: function () {
        $(".a-close-modal").click();
        $('#booking-error-quota-modal').modal();
    },
    BookingErrorUnitModal: function () {
        $(".a-close-modal").click();
        $('#booking-error-unit-modal').modal();
    }
};