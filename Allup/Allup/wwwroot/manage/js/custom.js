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

    $(document).on('click', '.deleteImageBtn', function (e) {
        e.preventDefault();
        let url = $(this).attr('href');
        fetch(url)
            .then(res => res.text())
            .then(data => {
                $('.imageContainer').html(data);
            })
    })
})