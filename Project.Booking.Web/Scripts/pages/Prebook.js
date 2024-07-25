let paymentTypeID;
var Prebook = {
    init: () => {
        $("#btn-confirm-prebook").click(() => {
            Prebook.PaymentModal();
        });

        $("button[data-action='select-payment']").click((e) => {
            paymentTypeID = $(e.currentTarget).attr('data-id');
            $('#modal-prebook-payment').modal('hide');

            if (paymentTypeID == PAYMENT_TYPE_CREDIT_ID) {
                setTimeout(() => {
                    Prebook.TermPaymentModal();
                }, 500)
            }
            else {
                setTimeout(() => {
                    Prebook.TransferModal();
                }, 500)
            }
        });

        $("div[data-action='payment-detail']").click(() => {
            Prebook.PaymentDetail();
        });

        $("#btn-accept-condition").click(() => {
            $('#modal-term-payment-prebook').modal('hide');
            Prebook.SaveProjectRegisterQuota();
            //setTimeout(() => {
            //    Prebook.TransferModal();
            //}, 500)    
        });
    },
    initPaymentTransfer: () => {
        $("#btn-payment-transfer").unbind('click').click(() => {
            paymentTypeID = PAYMENT_TYPE_TRANSFER_ID;
            Prebook.SaveProjectRegisterQuota();
        });

        $('input.decimal').autoNumeric({ aSep: ',', aNeg: '-', mDec: 2, mNum: 10 });

        $('#trans-date').pignoseCalendar({
            format: 'YYYY-MM-DD', // date format string. (2017-02-02)
            theme: 'light', // light, dark, blue
            //minDate: strDate
        });

    },
    TermPaymentModal: function () {
        $('#modal-term-payment-prebook').modal({
            backdrop: 'static',
            keyboard: true
        });
    },
    PaymentModal: function () {
        $('#modal-prebook-payment').modal();
    },
    TransferModal: function () {
        Prebook.getProjectQuota();
    },
    PaymentDetail: () => {
        $("#modal-prebook-payment-detail").modal();
    },
    SaveProjectRegisterQuota: () => {

        app.LoadWait(true);
        var amount = $('#trans-amount').autoNumeric('get').val();
        amount = amount.replace(/\,/g, '');

        var formData = new FormData($('#form-upload')[0]);
        formData.append("ProjectID", projectID);
        formData.append("ProjectQuotaID", $('input[name="rd"]:checked').val());
        formData.append("PaymentTypeID", paymentTypeID);
        formData.append("TransferDate", $('#trans-date').val());
        formData.append("Hours", $('#trans-hour').val());
        formData.append("Minutes", $('#trans-minute').val());
        formData.append("Amount", amount);

        $.ajax({
            url: baseUrl + 'Prebook/SaveProjectRegisterQuota',
            type: 'post',
            dataType: 'json',
            processData: false,
            contentType: false,
            cache: false,
            success: function (res) {
                app.ajaxComplete(res,
                    (res) => {
                        $("#div-prebook-list").empty().html(res.html);
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
    getProjectQuota: () => {
        app.LoadWait(true);
        let data = {
            ID: $('input[name="rd"]:checked').val(),
            ProjectID: projectID
        }
        $.post(baseUrl + 'Prebook/GetProjectQuota', data, (res) => {
            app.ajaxComplete(res, () => {
                $("#partial-payment-transfer-modal").empty().html(res.html);
                Prebook.initPaymentTransfer();
                $('#modal-prebook-transfer').modal();
            }, null, false);
            app.LoadWait(false);
        });
        return false
    },
    showImagePreview: (imageUploader, previewImage) => {

        if (imageUploader.files && imageUploader.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $(previewImage).attr('src', e.target.result);
            };

            reader.readAsDataURL(imageUploader.files[0]);
        } else {
            $("#imagePreview1").attr('src', '');
        }
    }
};