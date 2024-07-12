var Prebook = {
    init: () => {
        $("#btn-confirm-prebook").click(() => {
            Prebook.PaymentModal();
        });

        $("button[data-action='select-payment']").click(() => {
            $('#modal-prebook-payment').modal('hide');    
            setTimeout(() => {
                Prebook.TermPaymentModal();
            },500)                  
        });                

        $("div[data-action='payment-detail']").click(() => {
            Prebook.PaymentDetail();
        }); 

        $("#btn-accept-condition").click(() => {            
            $('#modal-term-payment-prebook').modal('hide');
            setTimeout(() => {
                Prebook.TransferModal();
            }, 500)    
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
        $('#modal-prebook-transfer').modal();
    },
    PaymentDetail: () => {
        $("#modal-prebook-payment-detail").modal();
    }
};