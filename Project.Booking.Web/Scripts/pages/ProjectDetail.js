var notifyHub;
var ProjectDetail = {
    init: function () {
        $("#BuildID").change(function () {
            ProjectDetail.BuildChange();
        });

        $("#a-introduction").click(() => {
            $('#modal-introduction').modal();
        });

        $("#a-view-available").click(() => {
            ProjectDetail.getUnitAvailable(); 
            return false;
        });

        //$('#trans-date').pignoseCalendar({
        //    format: 'YYYY-MM-DD', // date format string. (2017-02-02)
        //    theme: 'light', // light, dark, blue
        //    //minDate: strDate
        //});

        //$('#btn-save-transfer').click(() => {
        //    ProjectDetail.saveTransferPayment();
        //    return false;
        //});
        //ProjectDetail.bindDeletePayment();
    },
    initHub: function () {
        //Set Client SignalR
        notifyHub = app.initHub();
        notifyHub.client.sendUnitStatus = function (obj) {
            //console.log(obj);            
            //alert(obj.UnitStatusColor);
            $("g[data-id='" + obj.UnitID + "']").css('fill', obj.UnitStatusColor);
            $("g[data-id='" + obj.UnitID + "']").attr('unit-status', obj.UnitStatusColor);
        };

    },
    initGallery: function () {
        $(".filter-button").click(function () {

            $(".filter-button").removeClass("active");
            $(this).addClass('active');
            var value = $(this).attr('data-filter');

            if (value == "all") {
                //$('.filter').removeClass('hidden');
                $('.filter').show('1000');
            }
            else {
                //            $('.filter[filter-item="'+value+'"]').removeClass('hidden');
                //            $(".filter").not('.filter[filter-item="'+value+'"]').addClass('hidden');
                $(".filter").not('.' + value).fadeOut();
                $('.filter').filter('.' + value).fadeIn();

            }

        });

        $(".filter-button[data-order-by='1']").click();
        $(".filter-button[data-order-by='1']").addClass("active");
    },
    initFloorPlan: function () {
        //alert($('.zoomHolder').css('height'));
        $("#FloorID").unbind('change').change(function () {
            ProjectDetail.FloorChange();
        });

        $(".a9s-annotation").on('click', function (event) {
            //console.log(this);
            ProjectDetail.SetUnitStatusColor();
            var id = $(this).attr('data-id');
            $(this).css('fill', 'rgb(0 0 255 / 62%)');
            ProjectDetail.GetUnitDetail(id);
        });

        ProjectDetail.SetUnitStatusColor();


    },
    bindDeletePayment: () => {
        $(".lb-remove").unbind('click').click((e) => {
            let id = $(e.currentTarget).attr('data-id');
            ProjectDetail.confirmDeletePayment(id);
        });
    },
    BuildChange: function () {
        var data = {
            projectID: projectID,
            buildID: $("#BuildID").val()
        };

        $.post(baseUrl + 'Project/GetFloorList', data, function (res) {
            app.ajaxComplete(res, function () {
                $("#div-floor").empty().html(res.htmlFloorSelectList);
                $("#div-floorplan").html(res.htmlFloorPlan);
            }, null, false);
        });
    },
    FloorChange: function () {

        var data = {
            projectID: projectID,
            buildID: $("#BuildID").val(),
            floorID: $("#FloorID").val()
        };

        $.post(baseUrl + 'Project/GetFloorList', data, function (res) {

            app.ajaxComplete(res, function () {
                $("#div-floorplan").html(res.htmlFloorPlan);

            }, null, false);
        });
    },
    SetUnitStatusColor: function () {
        $(".a9s-annotation").each(function () {
            var fillColor = $(this).attr('unit-status');
            $(this).css('fill', fillColor);
        });
    },
    GetUnitDetail: function (ID) {
        app.LoadWaitID("#div-unit-selected", true);
        $.post(baseUrl + 'Project/GetUnitDetail', { ID: ID }, function (res) {
            app.ajaxComplete(res, function () {
                $("#div-unit-selected").html(res.htmlUnitDetail);
                $('html, body').animate({
                    scrollTop: $("#div-unit-selected").offset().top - 100
                }, 1000);
            }, null, false);
            app.LoadWaitID("#div-unit-selected", false);
        });
        return false;
    },
    saveTransferPayment: () => {
        app.LoadWait(true);
        var amount = $('#trans-amount').autoNumeric('get').val();
        amount = amount.replace(/\,/g,'');        
        var formData = new FormData($('#form-upload')[0]);
        formData.append("ProjectID", projectID);
        formData.append("TransferDate", $('#trans-date').val());
        formData.append("Amount", amount);
        $.ajax({
            url: baseUrl + 'Project/SaveTransferPayment',
            type: 'post',
            dataType: 'json',
            processData: false,
            contentType: false,
            cache: false,
            success: function (res) {
                app.ajaxComplete(res,
                    function () {
                        $("#div-trans-resource").empty().html(res.html);
                        $("#trans-date").val('');
                        $("#trans-amount").val('');
                        $("#file-resource").val('');
                        $("#file-resource-display").text('Click Attach a file :');
                        ProjectDetail.bindDeletePayment();
                    }, null);
                app.LoadWait(false);
            },
            error: function (xhr, status, error) {
                window.location.reload();
            },
            data: formData
        });
        return false;
    },
    confirmDeletePayment: (id) => {
        $("#btn-confirm").unbind('click').click(() => {
            ProjectDetail.saveDeletePayment(id);
        });

        $('#modal-confirm').modal({
            backdrop: 'static',
            keyboard: true
        });
    },
    saveDeletePayment: function (id) {
        $('#modal-confirm').modal('hide');
        app.LoadWait(true);
        $.post(baseUrl + 'Project/saveDeletePayment', { ID: id, ProjectID: projectID }, function (res) {
            app.ajaxComplete(res,
                function () {
                    ProjectDetail.setDeletePayment(res);
                }, function () {
                    ProjectDetail.setDeletePayment(res);
                });
            app.LoadWait(false);
        });
    },
    setDeletePayment: (res) => {
        $("#div-trans-resource").empty().html(res.html);
        ProjectDetail.bindDeletePayment();
    },
    getUnitAvailable: () => {
        app.LoadWait(true);
        $.post(baseUrl + 'Project/GetUnitAvailable', { ProjectID: projectID }, function (res) {
            app.ajaxComplete(res, function () {
                ProjectDetail.setUnitAvailable(res);
            }, null, false);
            app.LoadWait(false)
        });
        return false;
    },
    setUnitAvailable: (res) => {
        $("#partial-unit-available").empty().html(res.html);

        $("button[data-action='view-unit']").unbind('click').click((e) => {
            let buildID = $(e.currentTarget).attr('build-id');
            let floorID = $(e.currentTarget).attr('floor-id');
            $("#BuildID").val(buildID);
            app.LoadWait(true);
            ProjectDetail.BuildChange();
            $('#modal-available').modal('hide');
            setTimeout(() => {                
                $("#FloorID").val(floorID);
                ProjectDetail.FloorChange();               
                app.LoadWait(false);
            },3000);
       
        });

        $('#modal-available').modal();
    }
};