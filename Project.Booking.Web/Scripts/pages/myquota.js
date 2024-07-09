const myquota = {
    init: () => {
        $('input[type=file]').change(function () {
            var t = $(this).val();
            var labelText = 'File : ' + t.substr(12, t.length);
            $(this).prev('label').text(labelText);
        })
    }
};