function loadDepartment() {
    const $dropdown = $('#dropdown-department');
    $.ajax({
        type: "GET",
        url: `https://localhost:7177/api/v1/Department`,
        success: function (response) {
            // Khởi tạo departments
            departments = response;
            
            // Hiển thị tên của phần tử đầu tiên
            if(departments.length > 0) {
                $("#cbxDepartmentId").text(departments[0].departmentName);        
                $("#cbxDepartmentId").attr('mvalue', departments[0].departmentId);
            }

            $.each(departments, function(index, department) {
                const $item = $('<div>', {
                    class: 'dropdown-item',
                    text: department.departmentName,
                    mvalue: department.departmentId,
                    click: function() {
                        selectItemNoIcon(this, 'combobox-container-department', 'cbxDepartmentId');
                    }
                });
                $dropdown.append($item);
            });
        },
        error: function(response) {
            console.error('Failed to load departments', response);
        }
    });
}

$(document).ready(function() {
    loadDepartment();
});
