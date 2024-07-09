var countdown = document.getElementById("tiles"); // get tag element

CounterTimer();

var interv = setInterval(CounterTimer, 1000);

function CounterTimer() {
    if (leave > 0) {
        var day = Math.floor(leave / (60 * 60 * 24))
        var hour = Math.floor(leave / 3600) - (day * 24)
        var minute = Math.floor(leave / 60) - (day * 24 * 60) - (hour * 60)
        var second = Math.floor(leave) - (day * 24 * 60 * 60) - (hour * 60 * 60) - (minute * 60)

        hour = hour < 10 ? "0" + hour : hour;
        minute = minute < 10 ? "0" + minute : minute;
        second = second < 10 ? "0" + second : second;

        if (day <= 0 && hour <= 0 && minute <= 0 & second <= 0) {
            countdown.innerHTML = "";
            clearInterval(interv);
            window.location.reload();
        }
        countdown.innerHTML = "<span>" + day + "</span><span>" + hour + "</span><span>" + minute + "</span><span>" + second + "</span>";
        leave = leave - 1;   
    }
     
}

var countdown_app = {
    init: () => {
        //countdown_app.StartModal();
    },
    StartModal: function () {

        //gtag('event', 'submit_register_onlinebooking', { 'send_to': 'UA-85057478-10' });

        let width = $(window).width();
        let height = $(window).height();
        //mobile or tablet device

        if (width < height) {
            $(".img-thankyou-register").attr('src', baseUrl + 'Resources/countdown/ASW_Kave-Universe_Online-Booking_HowToBook.png');
            //$("#a-add-line").addClass("a-add-line-portrait");
            $("#a-close-page").addClass("a-close-page-portrait");
            //$(".img-line-icon").addClass("img-line-icon-portrait");
            $("#a-btn-close").addClass("a-btn-close-portrait");
        }
        else {
            $(".img-thankyou-register").attr('src', baseUrl + 'Resources/countdown/ASW_Kave-Universe_Online-Booking_HowToBook_WebBanner.png');
            //$("#a-add-line").addClass("a-add-line-lanscape");
            $("#a-close-page").addClass("a-close-page-lanscape");
            //$(".img-line-icon").addClass("img-line-icon-lanscape");
            $("#a-btn-close").addClass("a-btn-close-lanscape");
        }

        $('#register-success-modal').modal();

        setTimeout(() => {
            $('#register-success-modal').modal('hide');
        }, 5000);
    }
};