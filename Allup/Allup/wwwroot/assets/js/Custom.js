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
                .then(res => res.json())
                .then(data => {
                    for (var i = 0; i < data.length; i++) {
                        let item = ` <a class="d-block" href="#">
                                                <li class="d-block justify-content-between align-items-center">
                                                    <img class="" style="width:100px" src="/assets/images/product/${data[i].mainImage}">
                                                    <p>${data[i].title}</p>
                                                    <span>$${data[i].price}</span>
                                                </li>
                                            </a>`;
                        item += item
                    }
                    $('#SearchBody').html(items);
                })
        }

    })
})