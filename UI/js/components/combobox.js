// Hàm chuyển đổi trạng thái hiển thị dropdown
function toggleDropdown(containerId, dropdownId) {
    const $dropdown = $('#' + dropdownId);
    const $comboboxContainer = $('#' + containerId);
    $dropdown.toggle(); // Chuyển đổi hiển thị giữa block và none
    if ($dropdown.is(':visible')) {
        $comboboxContainer.addClass('active');
    } else {
        $comboboxContainer.removeClass('active');
    }
}

// Hàm chọn một mục trong dropdown có icon
function selectItem(item, containerId, inputId) {
    const $item = $(item);
    const $comboboxInput = $('#' + inputId);
    $comboboxInput.html($item.text() + ' <i class="fas fa-chevron-down"></i>'); // Cập nhật nội dung của input

    const $selectedItem = $('#' + containerId + ' .dropdown-item.selected');
    $selectedItem.removeClass('selected'); // Bỏ chọn mục đã chọn trước đó

    $item.addClass('selected'); // Đánh dấu mục được chọn

    $comboboxInput.addClass('selected'); // Đánh dấu input là đã chọn

    toggleDropdown(containerId, $item.parent().attr('id')); // Chuyển đổi trạng thái dropdown
}

// Hàm chọn một mục trong dropdown không có icon trỏ xuống
/* 
    1. Lấy ra item được chọn (this), gán giá trị cho 2 attr: text, mvalue (id)
    2. Thêm class selected cho item
    3. Đóng dropdown
*/

function selectItemNoIcon(item, containerId, inputId) {
    const $item = $(item);
    const $comboboxInput = $('#' + inputId);
    $comboboxInput.html($item.text()); // Cập nhật nội dung của input
    $comboboxInput.attr('mvalue', $item.attr('mvalue')); // Gán thuộc tính mvalue

    const $selectedItem = $('#' + containerId + ' .dropdown-item.selected');
    $selectedItem.removeClass('selected'); // Bỏ chọn mục đã chọn trước đó

    $item.addClass('selected'); // Đánh dấu mục được chọn

    $comboboxInput.addClass('selected'); // Đánh dấu input là đã chọn

    toggleDropdown(containerId, $item.parent().attr('id')); // Chuyển đổi trạng thái dropdown
}

// Hàm lọc các mục trong dropdown
function filterItems(containerId, inputId) {
    const $comboboxInput = $('#' + inputId);
    const filter = $comboboxInput.val().toLowerCase(); // Lấy giá trị input và chuyển thành chữ thường
    const $items = $('#' + containerId + ' .dropdown-item');
    $items.each(function() {
        const $item = $(this);
        const text = $item.text().toLowerCase(); // Lấy nội dung mục và chuyển thành chữ thường
        $item.toggle(text.includes(filter)); // Hiển thị hoặc ẩn mục dựa trên bộ lọc
    });
}

// Đóng dropdown nếu người dùng nhấp ra ngoài
$(window).on('click', function(event) {
    const $containers = $('.combobox-container');
    $containers.each(function() {
        const $container = $(this);
        const $dropdown = $container.find('.dropdown');
        if ($dropdown.is(':visible') && !$container.is(event.target) && !$container.has(event.target).length) {
            $dropdown.hide(); // Ẩn dropdown
            $container.removeClass('active'); // Bỏ đánh dấu active
        }
    });
});

// Ngăn chặn sự kiện click của dropdown lan ra ngoài
$('.dropdown').on('click', function(event) {
    event.stopPropagation();
});
