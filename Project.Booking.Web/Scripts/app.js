$.ajaxSetup({
    cache: false
});

var app = {
    init: function () {
        $(".carousel").swipe({
            swipe: function (event, direction, distance, duration, fingerCount, fingerData) {
                if (direction == 'left') $(this).carousel('next');
                if (direction == 'right') $(this).carousel('prev');
            },
            allowPageScroll: "vertical"
        });

        $('input.integer').autoNumeric({ aSep: ',', aNeg: '-', vMin: -9999999999, mNum: 10, aPad: false });
        $('input.decimal').autoNumeric({ aSep: ',', aNeg: '-', mDec: 2, mNum: 10 });
        
    },
    initHub: function () {
        var notifyHub = $.connection.notifyHub;
        console.log(notifyHub);
        $.connection.hub.start().done(function () {
            console.log("Notify Hub Start");
        });
        return notifyHub;
    },
    LoadWait: function (FlagShow) {
        if (FlagShow) {

            $('body').waitMe({

                //none, rotateplane, stretch, orbit, roundBounce, win8,
                //win8_linear, ios, facebook, rotation, timer, pulse,
                //progressBar, bouncePulse or img
                effect: 'win8_linear',
                //place text under the effect (string).
                text: '',
                //background for container (string).
                bg: 'rgb(190 190 242 / 70%)',
                //color for background animation and text (string).
                color: '#000',
                //change width for elem animation (string).
                sizeW: '',
                //change height for elem animation (string).
                sizeH: '',
                // url to image
                source: '',
                // callback
                onClose: function () { }
            });
        }
        else {
            $('body').waitMe('hide');
        }
    },
    LoadWaitID: function (ID, FlagShow) {

        if (FlagShow) {

            $(ID).waitMe({

                //none, rotateplane, stretch, orbit, roundBounce, win8,
                //win8_linear, ios, facebook, rotation, timer, pulse,
                //progressBar, bouncePulse or img
                effect: 'win8_linear',
                //place text under the effect (string).
                text: '',
                //background for container (string).
                bg: 'rgb(190 190 242 / 70%)',
                //color for background animation and text (string).
                color: '#000',
                //change width for elem animation (string).
                sizeW: '',
                //change height for elem animation (string).
                sizeH: '',
                // url to image
                source: '',
                // callback
                onClose: function () { }
            });
        }
        else {
            $(ID).waitMe('hide');
        }
    },
    notify: function (type, message) {
        if (type === 'error') {
            $.growl.error({ message: message, duration: 6000, location: 'tc' });
        }
        else if (type === 'success') {
            $.growl.notice({ title: type, message: message, duration: 2000, location: 'tc' });
        }
        else if (type === 'warning') {
            $.growl.warning({ message: message, duration: 6000, location: 'tc' });
        }
    },
    ajaxComplete: function (res, callbackSuccess, callbackError, successAlert = true, errorAlert = true) {
        if (res.success) {
            if (successAlert) {
                app.notify('success', res.message);
            }
            if (typeof callbackSuccess === 'function') {
                callbackSuccess();
            }
        }
        else {
            if (res.sessionTimeOut) {
                app.notify('warning', res.message);
                setTimeout(function () {
                    window.location.reload();
                }, 3000);
            }
            else {
                if (typeof callbackError === 'function') {
                    callbackError();
                }
                if (errorAlert) {
                    app.notify('error', res.message);
                }                
            }
        }
    },
    ajaxVerifySession: function (res) {
        if (res.sessionTimeOut) {
            app.notify('warning', res.message);
            setTimeout(function () {
                window.location.reload();
            }, 3000);
        }
    },
    Numbers: function (e) {
        var keynum;
        var keychar;
        var numcheck;
        if (window.event) {// IE
            keynum = e.keyCode;
        }
        else if (e.which) {// Netscape/Firefox/Opera
            keynum = e.which;
        }
        if (keynum == 13 || keynum == 8 || typeof (keynum) == "undefined") {
            return true;
        }
        keychar = String.fromCharCode(keynum);
        numcheck = /^[0-9]$/;
        return numcheck.test(keychar);
    },
    serializeFormJSON: function (form) {
        var o = {};
        var a = form.serializeArray();
        $.each(a, function () {
            if (o[this.name]) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    },
    populate:function(frm, data) {
        $.each(data, function (key, value) {
            var ctrl = $('[name=' + key + ']', frm);
            switch (ctrl.prop("type")) {
                case "radio": case "checkbox":
                    ctrl.each(function () {
                        if ($(this).attr('value') == value) $(this).attr("checked", value);
                    });
                    break;
                default:
                    ctrl.val(value);
            }
        });
    }
};