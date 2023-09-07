﻿$(document).ready(function () {
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

    $('.addBasket').click(function (e) {
        e.preventDefault();
        let url = $(this).attr('href');
        fetch(url)
            .then(res => res.text())
            .then(data => {
                $('.header-cart').html(data)
            });
    });

    $(document).on('click', '.loadMoreBtn', function (e) {
        e.preventDefault();
        let url = $(this).attr('href');
        let pageIndex = $(this).data('pageindex');
        let totalPage = $(this).data('maxpage');
        if (pageIndex > 0 && pageIndex < totalPage) {
            fetch(url + '?pageIndex=' + pageIndex)
                .then(res => res.text())
                .then(data => {
                    $('.productContainer').append(data)
                });
        } else if (pageIndex == totalPage) {
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
});