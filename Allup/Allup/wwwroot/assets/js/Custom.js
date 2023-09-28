$(document).ready(function () {
    //1.Search
    //2.Modal
    //3.BasketAndCart
    //4.LoadMore
    //5.NavBarActive
    //=====================================================
    //1.Search
    $('#searchInput').keyup(function () {
        if ($(this).val().trim().length == 0) {
            $('#searchBody').html('');
        }
    });

    $('#searchBtn').click(function (e) {
        e.preventDefault();
        let search = $(' #searchInput').val().trim();
        let categoryId = $('#categoryId').val();
        let searchUrl = 'product/search?search=' + search + '&' + 'categoryId=' + categoryId;

        if (search.length >= 3) {
            fetch(searchUrl)
                .then(res => res.text())
                .then(data => {
                    $('#searchBody').html(data);
                });
        }

    });

    //2.Modal
    $('.modalBtn').click(function (e) {
        e.preventDefault();

        let url = $(this).attr('href');
        fetch(url)
            .then((res) => res.text())
            .then((data) => {
                $('.modal-content').html(data);

                $('.quick-view-image').slick({
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    arrows: false,
                    dots: false,
                    fade: true,
                    asNavFor: '.quick-view-thumb',
                    speed: 400,
                });

                $('.quick-view-thumb').slick({
                    slidesToShow: 4,
                    slidesToScroll: 1,
                    asNavFor: '.quick-view-image',
                    dots: false,
                    arrows: false,
                    focusOnSelect: true,
                    speed: 400,
                });

            });

    });

    //3.BasketAndCart
    $(document).on('click', '.addBasket, .product-close', function (e) {
        e.preventDefault();
        let url = $(this).attr('href');
        fetch(url)
            .then(res => res.text())
            .then(data => {
                $('.header-cart').html(data)
            });
    });

    $(document).on('click', '.deleteProduct, .product-close', function (e) {
        e.preventDefault();
        let urlArr = $(this).attr('href').split('/');
        let Id = urlArr[urlArr.length - 1]
        let basketUrl = '/Basket/RemoveBasket/' + Id
        fetch(basketUrl)
            .then(res => res.text())
            .then(data => {
                $('.header-cart').html(data);
                $('#totalCart').html($('#totalBasket').html());
                $('#subTotalCart').html($('#subTotalBasket').html());
                $('#taxesCart').html($('#taxesBasket').html());

            });
        let cartUrl = '/Cart/RemoveCart/' + Id
        fetch(cartUrl)
            .then(res => res.text())
            .then(data => {
                $('#cartBody').html(data);
                $('#totalCart').html($('#totalBasket').html());
                $('#subTotalCart').html($('#subTotalBasket').html());
                $('#taxesCart').html($('#taxesBasket').html());


            });

    });

    //4.LoadMore
    $(document).on('click', '.loadMoreBtn', function (e) {
        e.preventDefault();
        let url = $(this).attr('href');
        console.log(url);
        let pageIndex = $(this).data('pageindex');
        console.log(pageIndex);

        let totalPage = $(this).data('maxpage');
        console.log(totalPage);

        if (pageIndex > 0 && (pageIndex + 1) < totalPage) {
            fetch(url + '?pageIndex=' + pageIndex)
                .then(res => res.text())
                .then(data => {
                    $('.productContainer').append(data)
                });
        } else if (pageIndex == (totalPage - 1)) {
            fetch(url + '?pageIndex=' + pageIndex)
                .then(res => res.text())
                .then(data => {
                    $('.productContainer').append(data)
                });
            $('.loadMoreBtn').remove();
        }
        pageIndex++;
        $('.loadMoreBtn').data("pageindex", pageIndex)
    });

    //5.NavBarActive

    let arr = $('#navBarMenuContent').children();

    $(window).on("load", function () {
        for (var i = 0; i < arr.length; i++) {
            let path = "/" + $(arr[i]).find("a:eq(0)").text();
            if (window.location.pathname == path) {
                $(arr[i]).addClass('active');
            } else {
                $(arr[i]).removeClass('active')
            }
            if (window.location.pathname == "/" && path == "/Home") {
                $(arr[i]).addClass('active');
            } else if (window.location.pathname == "/Product" && path == "/Shop") {
                $(arr[i]).addClass('active');

            }

        }
    });

    $('.addAddressBtn').click(function (e) {
        e.preventDefault();
        $('.addressForm').removeClass('d-none');
        $('.addAddressBtn').addClass('d-none');
        $('.AddressContainer').addClass('d-none');
    });

    $('.goBackBtn').click(function (e) {
        e.preventDefault();
        $('.addressForm').addClass('d-none');
        $('.addAddressBtn').removeClass('d-none');
        $('.AddressContainer').removeClass('d-none');
    });

    $('.editAddressBtn').click(function (e) {
        e.preventDefault();
        $('.addAddressBtn').addClass('d-none');
        $('.AddressContainer').addClass('d-none');
        $('.editAddressForm').removeClass('d-none');
        let url = $(this).attr('href');
        fetch(url)
            .then(res => res.text())
            .then(data => {
                $('.editAddressForm').html(data);
            })
    })
});