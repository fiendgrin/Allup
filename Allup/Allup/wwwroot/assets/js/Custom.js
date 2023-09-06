$(document).ready(function () {
    $('#SarchInput').keyup(() => {
        if (this.val().trim().length == 0) {
            $('#SearchBody').html('');
        }
    })

    $('#SearchBtn').click((e) => {
        e.preventDefault();
        let search = $("#SearchInput").val().trim();
        let categoryId = $("#CategoryId").val();
        let searcUrl = 'product/search?search=' + search + 'categoryId=' + categoryId

        if (search.length >= 3) {
            fetch(searcUrl)
                .then(res => res.text())
                .then(data => {
                    $('#SearchBody').html(data);
                    //for (var i = 0; i < data.length; i++) {
                    //    let item = ` <a class="d-block" href="#">
                    //                            <li class="d-block justify-content-between align-items-center">
                    //                                <img class="" style="width:100px" src="/assets/images/product/${data[i].mainImage}">
                    //                                <p>${data[i].title}</p>
                    //                                <span>$${data[i].price}</span>
                    //                            </li>
                    //                        </a>`;
                    //    item += item
                    //}
                })
        }

    })

    $('.modalBtn').click((e) => {
        e.preventDefault();

        let url = $(this).attr('href');
        fetch(url)
            .then((res) => res.json())
            .then((data) =>
            {
                $(".modal-content").html(data);

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

            })

    })

    $('.addBasket').click((e) =>
    {
        e.preventDefault();
        let url = $(this).attr('href');
        fetch(url)
            .then(res => res.json())
            .then(data =>
            {

            })
    })
})