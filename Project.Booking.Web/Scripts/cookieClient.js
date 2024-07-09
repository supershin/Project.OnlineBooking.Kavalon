var CookieClient = {
    init: function () {
        //Set Cookies
        let cookiePopup = $("#cookie-accept-popup");        
        if (getCookie('visited-booking') === undefined || getCookie('visited-booking') === '') {
            cookiePopup.show();
        }

        $("#acceptCookie").on('click touch', function () {
            setCookie();
            cookiePopup.fadeOut(320);
        });

        $("#notAcceptCookie").on('click touch', function () {
            cookiePopup.fadeOut(320);
        });

    }
};

function getCookie(name) {
    let value = "; " + document.cookie;
    let parts = value.split("; " + name + "=");
    if (parts.length === 2) return parts.pop().split(";").shift();
}

function setCookie() {
    if (getCookie('visited-booking') === undefined || getCookie('visited-booking') === '') {
        //console.log('cookie was not set');
        //cookiePopup.show();
        let now = new Date();
        let visitedTime = new Date();
        now.setMonth(now.getMonth() + 1);

        document.cookie = "visited-booking=" + visitedTime.toUTCString() + "; expires=" + now.toUTCString() + "; path=/";
    }
}

function delCookie(name) {
    document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
}