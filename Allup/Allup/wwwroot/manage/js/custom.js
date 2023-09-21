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
    $(document).on('click', '.setActiveBtn', function (e)
    {
        e.preventDefault();

        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Change Account Activity Status!'
        }).then((result) => {
            if (result.isConfirmed) {

                let url = $(this).attr('href');
                fetch(url)
                    .then(res => res.text())
                    .then(data => {
                        $('.listContainer').html(data);
                    })

                Swal.fire(
                    'Deleted!',
                    'Your Account Activity Status has been changed.',
                    'success'
                )
            }
        })
       
    })

    $(document).on('click', '.resetPasswordBtn', function (e)
    {
        e.preventDefault();

        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, reset it!'
        }).then((result) => {
            if (result.isConfirmed) {

                let url = $(this).attr('href');
                fetch(url)
                    .then(res => res.text())
                    .then(data => {
                        $('.listContainer').html(data);
                    })

                Swal.fire(
                    'Reset!',
                    'Your Password has been Reset.',
                    'success'
                )
            }
        })
       
    })
})