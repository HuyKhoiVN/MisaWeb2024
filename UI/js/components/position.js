function loadPosition() {
    const $dropdown = $('#dropdown-position');
    $.ajax({
        type: "GET",
        url: `https://localhost:7177/api/Position`,
        success: function (response) {
            // Khởi tạo positions
            positions = response;
            
            // Hiển thị tên của phần tử đầu tiên
            if(positions.length > 0) {
                $("#cbxPositionId").text(positions[0].positionName);        
                $("#cbxPositionId").attr('mvalue', positions[0].positionId);
            }

            $.each(positions, function(index, position) {
                const $item = $('<div>', {
                    class: 'dropdown-item',
                    text: position.positionName,
                    mvalue: position.positionId,
                    click: function() {
                        selectItemNoIcon(this, 'combobox-container-position', 'cbxPositionId');
                    }
                });
                $dropdown.append($item);
            });
        },
        error: function(response) {
            console.error('Failed to load positions', response);
        }
    });
}

$(document).ready(function() {
    loadPosition();
});
