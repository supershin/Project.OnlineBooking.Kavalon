var Utility = {
    init: function () {
        //$('.slideInDown').stopSlide();
        //Utility.RegisterStartModal();

        //แสดง register
        //Utility.controlActive($("#btn-register"));
        //Utility.controlVisible($("#frm-register"));

        // login อย่างเดียว       
        //$("#btn-register").hide();
        Utility.controlActive($("#btn-login"));
        Utility.controlVisible($("#frm-login"));

        $("#btn-register").click(() => {
            Utility.controlActive($("#btn-register"));
            Utility.controlVisible($("#frm-register"));
        });
        $("#btn-login").click(() => {
            Utility.controlActive($("#btn-login"));
            Utility.controlVisible($("#frm-login"));
        });

        $("#frm-register").submit(() => {
            Utility.SaveRegister();
            return false;
        });

        $("#frm-login").submit(() => {
            Utility.Authentication();
            return false;
        });
        $("#frm-forgot").submit(() => {
            Utility.ForgotPassword();
            return false;
        });

        $("#a-forgot").click(() => {
            Utility.DislayForgot();
        });

        $('.pass_show').append('<span class="ptxt">Show</span>');

        $(document).on('click', '.pass_show .ptxt', function () {
            $(this).text($(this).text() == "Show" ? "Hide" : "Show");
            $(this).prev().attr('type', function (index, attr) { return attr == 'password' ? 'text' : 'password'; });
        });
    },
    controlActive: function (active) {
        $("#btn-register").removeClass("grd-active");
        $("#btn-login").removeClass("grd-active");
        active.addClass("grd-active");
        //non.removeClass("grd-active");
    },
    controlVisible: function (active) {
        $("#frm-forgot").hide();
        $("#frm-login").hide();
        $("#frm-register").hide();
        active.fadeIn(100);
    },
    SaveRegister: function () {
        var accept = $("#chkAccept").prop('checked');
        if (!accept) {
            app.notify('warning', 'โปรดยอมรับเงื่อนไข และนโยบายความเป็นส่วนตัว');
            return false;
        }

        app.LoadWait(true);
        var formData = new FormData(document.getElementById("frm-register"));

        $.ajax({
            url: baseUrl + 'Register/SaveRegister',
            type: 'post',
            dataType: 'json',
            processData: false,
            contentType: false,
            success: function (res) {
                if (res.success) {
                    $('#frm-register').trigger("reset");
                    Utility.RegisterSuccessModal2();
                    //Utility.RegisterSuccessModal();
                    //if (res.isActivate)
                    //    setTimeout(() => {
                    //        //redirect to kave uni.verse
                    //        window.location = baseUrl + 'project/detail/7ee1b4d9-50dc-4cad-91e6-bf8b25e9cb9a';
                    //    }, 5000);
                }
                else {
                    app.notify('error', res.message);
                }
                app.LoadWait(false);
            },
            data: formData
        });

        return false;
    },
    Authentication: function () {
        app.LoadWait(true);
        var formData = new FormData(document.getElementById("frm-login"));

        $.ajax({
            url: baseUrl + 'Register/Authentication',
            type: 'post',
            dataType: 'json',
            processData: false,
            contentType: false,
            success: function (res) {
                if (res.success) {
                    if (res.activateDate == null)
                        Utility.RegisterSuccessModal2();
                    else {
                        window.location = baseUrl + 'project/detail/' + REDIRECT_PROJECT_ID;
                    }
                    //else if (res.returnUrl === null || res.returnUrl === "") {
                    //    //res.returnUrl = baseUrl + 'Project';
                    //    window.location = baseUrl + 'project/detail/97C0E5D4-3894-4318-A018-F48315878BDF'
                    //}
                    //window.location = res.returnUrl;                    
                }
                else {
                    app.notify('error', res.message);
                }
                app.LoadWait(false);
            },
            data: formData
        });

        return false;
    },
    RegisterSuccessModal: function () {

        //gtag('event', 'submit_register_onlinebooking', { 'send_to': 'UA-85057478-10' });

        let width = $(window).width();
        let height = $(window).height();
        //mobile or tablet device

        if (width < height) {
            $(".img-thankyou-register").attr('src', baseUrl + 'Resources/register/ASW_Kave-Universe_Online-Booking_ThankYou_Mobile.png');
            //$("#a-add-line").addClass("a-add-line-portrait");
            $("#a-close-page").addClass("a-close-page-portrait");
            //$(".img-line-icon").addClass("img-line-icon-portrait");
            $("#a-btn-close").addClass("a-btn-close-portrait");
        }
        else {
            $(".img-thankyou-register").attr('src', baseUrl + 'Resources/register/ASW_Kave-Universe_Online-Booking_ThankYou_Desktop.png');
            //$("#a-add-line").addClass("a-add-line-lanscape");
            $("#a-close-page").addClass("a-close-page-lanscape");
            //$(".img-line-icon").addClass("img-line-icon-lanscape");
            $("#a-btn-close").addClass("a-btn-close-lanscape");
        }

        $(".a-close-modal").click();
        $('#register-success-modal').modal();
    },
    RegisterStartModal: function () {

        //gtag('event', 'submit_register_onlinebooking', { 'send_to': 'UA-85057478-10' });

        let width = $(window).width();
        let height = $(window).height();
        //mobile or tablet device

        if (width < height) {
            //mobile
            $(".img-thankyou-register").attr('src', baseUrl + 'Resources/tives/Vault_Kaset.png');
            //$("#a-add-line").addClass("a-add-line-portrait");
            $("#a-close-page").addClass("a-close-page-portrait");
            //$(".img-line-icon").addClass("img-line-icon-portrait");
            $("#a-btn-close").addClass("a-btn-close-portrait");
        }
        else {
            //desktop
            $(".img-thankyou-register").attr('src', baseUrl + 'Resources/tives/Vault_Kaset.png');
            //$("#a-add-line").addClass("a-add-line-lanscape");
            $("#a-close-page").addClass("a-close-page-lanscape");
            //$(".img-line-icon").addClass("img-line-icon-lanscape");
            $("#a-btn-close").addClass("a-btn-close-lanscape");
        }

        $('#register-success-modal').modal();

        setTimeout(() => {
            $('#register-success-modal').modal('hide');
        }, 3000);
    },
    RegisterSuccessModal2: function () {
        $(".a-close-modal").click();
        $('#register-success-modal-2').modal();
    },
    DislayForgot: function () {
        $("#frm-register").hide();
        $("#frm-login").hide();
        $("#frm-forgot").fadeIn(1000);
    },
    ForgotPassword: function () {

        app.LoadWait(true);
        var formData = new FormData(document.getElementById("frm-forgot"));

        $.ajax({
            url: baseUrl + 'Register/ForgotPassword',
            type: 'post',
            dataType: 'json',
            processData: false,
            contentType: false,
            success: function (res) {
                if (res.success) {
                    $('#frm-forgot').trigger("reset");
                    $("#btn-login").click();
                    Utility.ForgotPasswordSuccessModal();
                }
                else {
                    app.notify('error', res.message);
                }
                app.LoadWait(false);
            },
            data: formData
        });

        return false;
    },
    ForgotPasswordSuccessModal: function () {
        $(".a-close-modal").click();
        $('#forgot-success-modal').modal();
    },
    SetLinkOnImg: function (lanscape) {
        if (lanscape) {
            $(".a-back-main").attr('style', 'left:75%;top:70%;transform:translate(-25%, -30%);');
        }
    }
};