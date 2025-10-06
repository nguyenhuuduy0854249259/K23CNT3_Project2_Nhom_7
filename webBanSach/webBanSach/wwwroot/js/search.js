$(document).ready(function () {
    const $input = $("#searchInput");
    const $suggestions = $("#searchSuggestions");

    $input.on("keyup", function () {
        const keyword = $(this).val().trim();
        if (keyword.length < 1) {
            $suggestions.hide();
            return;
        }

        $.ajax({
            url: '/Sach/SearchSuggestions',
            type: 'GET',
            data: { keyword: keyword },
            success: function (data) {
                if (data.length === 0) {
                    $suggestions.hide();
                    return;
                }

                let html = '<ul class="list-group list-group-flush">';
                data.forEach(item => {
                    html += `
                        <li class="list-group-item list-group-item-action d-flex align-items-center suggestion-item"
                            data-url="/Sach/Details/${item.maSach}">
                            <img src="/images/sach/${item.hinhAnh ?? 'no-image.png'}"
                                class="me-2 rounded" style="width:40px;height:50px;object-fit:cover;">
                            <div>
                                <div class="fw-semibold">${item.tenSach}</div>
                                <div class="text-danger">${item.giaBan.toLocaleString()} đ</div>
                            </div>
                        </li>`;
                });
                html += '</ul>';
                $suggestions.html(html).show();
            }
        });
    });

    $(document).on("click", ".suggestion-item", function () {
        window.location.href = $(this).data("url");
    });

    $(document).click(function (e) {
        if (!$(e.target).closest("#searchForm").length) {
            $suggestions.hide();
        }
    });
});
