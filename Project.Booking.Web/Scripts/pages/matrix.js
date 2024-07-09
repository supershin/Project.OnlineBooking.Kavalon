var notifyHub;
const matrix = {
    init: () => {
        $(".unit-item").click((e) => {
            let unitID = $(e.currentTarget).attr("id");
            matrix.getUnit(unitID);
        });
    },
    initHub: function () {
        //Set Client SignalR
        notifyHub = app.initHub();
        notifyHub.client.sendUnitStatus = function (obj) {
            //alert('test');
            //console.log(obj);
            //alert(JSON.stringify(obj));
            //alert(obj.UnitStatusColor);
            if (obj.UnitStatusName == 'ว่าง') {
                $("#" + obj.UnitID).css('color', 'black');
                obj.UnitStatusColor = "white";
            }
            else {
                $("#" + obj.UnitID).css('color', 'transparent');
            }
            $("#" + obj.UnitID).css('background-color', obj.UnitStatusColor);
        };
    },
    getUnit: (id) => {
        app.LoadWait(true);
        $.ajax({
            url: baseUrl + 'Matrix/GetUnit',
            type: 'post',
            data: { ID: id },
            success: function (res) {
                app.ajaxComplete(res,
                    function () {
                        $("#div-unit-modal").empty().html(res.html);
                        $('button[data-action="btn-save-unit-bookint"]').unbind('click').click((e) => {
                            let unitID = $(e.currentTarget).attr("unit-id");
                            matrix.saveUnitBooking(unitID);
                        });
                        $('#modal-unit').modal();
                    },
                    function () {
                        $("#div-unit-selected").empty().html(res.htmlPartialUnitDetail);
                    }, false, false);
                app.LoadWait(false);
            },
            error: function (xhr, status, error) {
                window.location.reload();
            }
        });
        return false;
    }
};