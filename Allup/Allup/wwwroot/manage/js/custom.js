$(document).ready(function () {
    let isMain = $('#IsMain').is(':checked');
    if (isMain === true) {
        $('.imgContainer').removeClass('d-none');
        $('.parentContainer').addClass('d-none');
    } else {
        $('.imgContainer').addClass('d-none');
        $('.parentContainer').removeClass('d-none');
    }
    $('#IsMain').click(function () {
        let isMain = $(this).is(':checked');
        if (isMain === true) {
            $('.imgContainer').removeClass('d-none');
            $('.parentContainer').addClass('d-none');
        } else {
            $('.imgContainer').addClass('d-none');
            $('.parentContainer').removeClass('d-none');
        }
    })
})