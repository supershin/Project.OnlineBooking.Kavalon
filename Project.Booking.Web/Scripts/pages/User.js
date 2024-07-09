var User = {
    init: () => {
        $("#frm-user").submit(() => {
            User.SaveUserProfile();
            return false;
        });

        $('.pass_show').append('<span class="ptxt">Show</span>');

        $(document).on('click', '.pass_show .ptxt', function () {            
            $(this).text($(this).text() == "Show" ? "Hide" : "Show");
            $(this).prev().prev().prev().attr('type', function (index, attr) { return attr == 'password' ? 'text' : 'password'; });
        });  

    },
    SaveUserProfile: () => {
        app.LoadWait(true);
        var formData = new FormData(document.getElementById("frm-user"));

        $.ajax({
            url: baseUrl + 'User/SaveUserProfile',
            type: 'post',
            dataType: 'json',
            processData: false,
            contentType: false,
            success: function (res) {
                app.ajaxComplete(res,
                    function () {
                        app.notify('warning', res.message);
                        setTimeout(function () {
                            window.location = baseUrl + 'register/logout';
                        }, 3000);                                              
                    },
                    null, false);
              
                app.LoadWait(false);
            },
            data: formData
        });

        return false;
    }
};