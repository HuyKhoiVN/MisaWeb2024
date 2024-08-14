// khai báo biến toàn cục
var formMode = "edit";
var employeeIdForUpdate = null;
var empForEdit = null;
let employeesData = [];

var currentPage = 1;
var pageSize = 10;
var totalPages = 10;
var totalEmployees = 1;



$(document).ready(function () {
    // -------->1. Load dữ liệu từ JSON
    // -------->2. Hiển thị dữ liệu lên UI
    GetTotal()

    // -------->3. Tìm kiếm theo tên/empCode
    $("#txtSearch").on("keyup", function() {
        let searchTerm = $(this).val().toLowerCase();
        let filteredData = employeesData.filter(employee => {
            return employee.employeeCode.toLowerCase().includes(searchTerm) ||
                   employee.fullName.toLowerCase().includes(searchTerm);
        });
        renderTable(filteredData);
    });

    // -------->Phân trang
    /*  1. Chọn số bản ghi: gán giá trị cho pageSize
        2. Bấm next/prev: Tăng, giảm giá trị currentPage
    */

    // Xử lý sự kiện khi chọn số bản ghi trên mỗi trang
    $(".m-paging-right .dropdown-item").click(function () {
        pageSize = parseInt($(this).text());
        currentPage = 1; // Reset lại trang hiện tại khi thay đổi số bản ghi/trang
        loadData();
    });

    // Xử lý sự kiện khi nhấn nút trang trước
    $("#btnPrev").click(function () {
        console.log(currentPage);
        if (currentPage > 1) {
            currentPage--;
            loadData(); // Hiển thị dữ liệu trang trước
        }
    });

    // Xử lý sự kiện khi nhấn nút trang sau
    $("#btnNext").click(function () {
        console.log(totalPages);
        if (currentPage < totalPages) {
            currentPage++;
            loadData(); // Hiển thị dữ liệu trang sau
        }
    });
    //------------------------------------------

    // --------->Thêm mới nhân viên
    $("#btnAdd").click(function () {
        formMode = "add";
        // --hiển thị form thêm mới
        $("#dlgDialog").show();
        // --focus vào ô đầu tiên
        $("#txtEmployeeCode").focus();
    })

    // ---3.2. ấn btn-close
    $(".m-dialog-close").click(function () {
        // ẩn form
        ClearDialog();
        $("#dlgDialog").hide();
    })
    // ----------------------------------------

    // add class
    
    // ---3.3. double click vào 1 dòng trong table
    $(".m-table").on("dblclick", "tr", rowDblClick);

    $(".m-table").on("click", "tr", function (event) {
        // Kiểm tra xem click có xảy ra trong vùng dứa 3 btn không
        if ($(event.target).closest('.m-table-action').length === 0) {
            // Nếu click không xảy ra trong vùng .m-table-action
    
            // Kiểm tra xem dòng đã có class row-selected?
            if ($(this).hasClass("row-selected")) {
                // Nếu đã có -> xoá đi
                $(this).removeClass("row-selected");
            } else {
                // Nếu chưa có, xoá hết các dòng khác và thêm class row-selected vào dòng được click
                $(this).siblings().removeClass("row-selected");
                $(this).addClass("row-selected");
    
                let employee = $(this).data("entity");
    
                employeeIdForUpdate = employee.employeeId;
                empForEdit = employee;
    
                $(".m-table-action .btn-delete").on("click", function() {
                    try {
                        if (confirm(`Bạn có chắc chắn muốn xóa nhân viên ${employee.fullName} có mã là ${employee.employeeCode} không?`)) {
                            RemoveEmployee(employeeIdForUpdate);
                        }
                    } catch (error) {
                        console.error("Error removing employee:", error);
                        alert("Đã xảy ra lỗi khi xóa nhân viên.");
                    }
                });
            
                $(".m-table-action .btn-edit").on("click", function() {
                    dialogDisplay();
                });
            }
        }
    });    

    
        // -----hiển thị trạng thái lỗi validate khi không nhập các trường bắt buộc:
        $("input[required]").blur(function () {
            let isValid = validateInputRequired(this);
            if (!isValid) {
                $(this).siblings(".m-error").html("Thông tin này không được để trống!");
            } else {
                $(this).siblings(".m-error").text("");
            }
        });
        

        // -----hiển thị trạng thái lỗi với email sai định dạng
        $("input[email]").blur(function () {
            // email regex
            let pattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;

            let errorMessage = "Email không đúng định dạng!";
            
            let isValid = validateInput(this, pattern, errorMessage);
            if (!isValid) {
                $(this).siblings(".m-error").html(errorMessage);
            } else {
                $(this).siblings(".m-error").text("");
            }
        })

        // -----hiển thị lỗi khi sdt sai định dạng
        $("#txtPhoneNumber").blur(function () {
            // sdt regex
            let pattern = /^\+?[0-9]*$/;

            let errorMessage = "Số điện thoại không đúng định dạng! Chỉ được phép chứa dấu + và số 0-9.";
            let isValid = validateInput(this, pattern, errorMessage);
            if (!isValid) {
                $(this).siblings(".m-error").html(errorMessage);
            } else {
                $(this).siblings(".m-error").text("");
            }
        })

    // ---------------------------------------------------------------
    // 4. Thực hiện hành động trong form thêm mới
    $("#btnSave").click(function () {
        // ---4.1. lấy dữ liệu từ form

        // họ tên, mã nhân viên k được trống
        // ngày sinh không lớn hơn ngày hiện tại
        // email đúng định dạng
        // sđt phải đúng định dạng

        let employeeCode = $("#txtEmployeeCode").val(); // employeeCode
        let fullName = $("#txtFullName").val(); // fullName
        let dateOfBirth = $("#dtDateOfBirth").val(); // dateOfBirth

        let gender = $('input[name="gender"]:checked').val(); // gender
        gender = parseInt(gender, 10); // chuyển đổi về int (hệ 10)

        let positionId = $("#cbxPositionId").attr('mvalue') || null; // positionId
        let identityNumber = $("#txtIdentityNumber").val(); // identityNumber
        let identityDate = $("#txtIdentityDate").val() || null; // identityDate
        let departmentId = $("#cbxDepartmentId").attr('mvalue') || null; // departmentId
        let identityPlace = $("#txtIdentityPlace").val(); // identityPlace
        let address = $("#txtAddress").val(); // address
        let phoneNumber = $("#txtPhoneNumber").val(); // phoneNumber
        let lanelineNumber = $("#txtLandlinePhone").val(); // lanelineNumber
        let email = $("#txtEmail").val(); // email
        let bankNumber = $("#txtBankAccount").val(); // bankNumber
        let bankName = $("#txtBankName").val(); // bankName
        let bankBranch = $("#txtBankBranch").val(); // bankBranch

        // ---4.2. Validate dữ liệu
        // ------alert mã nv
        if(employeeCode == null || employeeCode === "") {
            alert(resource["VI"].employeeNotEmpty);
            return false;
        }

        // ------alert họ và tên
        if(fullName == null || fullName === "") {
            alert("Họ tên không được để trống");
            return false;
        }
        
        // ------alert ngày sinh
        if(dateOfBirth) {
            dateOfBirth = new Date(dateOfBirth);
        }
        if(dateOfBirth > new Date()) {
            alert("Ngày không được phép lớn hơn ngày hiện tại");
            return false;
        }

        // Chuyển đổi lại dob
        dateOfBirth = $("#dtDateOfBirth").val() || null;

        // -----alert nếu thông tin được điền chưa hợp lệ
        if ($(".m-input-error").length > 0) {
            alert("Vui lòng điền đúng các trường thông tin!");
            return false;
        }

        // ---4.3. Build object và thêm dữ liệu
        let positionfix = "d4ca430a-464b-11ef-9fd0-00163e0c7f26";
        let departmentfix = "00eec814-464f-11ef-9fd0-00163e0c7f26";

        // -----Khởi tạo đối tượng emp
        let employee = {
            "employeeCode": employeeCode,
            "fullName": fullName,
            "gender": gender,
            "dateOfBirth": dateOfBirth,
            "phoneNumber": phoneNumber,
            "email": email,
            "identityNumber": identityNumber,
            "indentityDate": identityDate,
            "identityPlace": identityPlace,
            "address": address,
            "lanelineNumber": lanelineNumber,
            "bankNumber": bankNumber,
            "bankName": bankName,
            "bankBranch": bankBranch,
            "positionId": positionId,
            "departmentId": departmentId,
            "createdBy": "Huy Khoi",
            "modifiedBy": "Huy Khoi",
        };

        
        // -----Hiện form loading, thực hiện truyền json emp
        $(".m-loading").show();
        // check formMode
        if (formMode == "add") {
            $.ajax({
                type: "POST",
                url: "https://localhost:7177/api/v1/Employee",
                data: JSON.stringify(employee),
                dataType: "json",
                contentType: "application/json",
                success: function(response) {
                    // sau khi thực hiện thêm thì ẩn loading, ấn form chi tiết, load lại dữ liệu:
                    // hiển thị toast ms
                    showToast('success', 'Nhân viên đã được thêm.', ''); 
                    $("#dlgDialog").hide();
                    GetTotal();
                    
                },
                error: function(response) {
                    console.log(response);
                    console.log(employee);
                    console.log(response.responseJSON);
                    console.log(response.responseJSON.userMsg);
                    $(".m-loading").hide();
                    // console.log(response); // In ra response
                    let errorMsg = 'Có lỗi xảy ra';
                    if (response.responseJSON || response.responseJSON.errors) {
                        // Lấy lỗi đầu tiên trong đối tượng errors
                        errorMsg = response.responseJSON.userMsg;
                    }
    
                    // hiển thị toast
                    showToast('error', errorMsg, '');
                }
            });
        }else {
            $.ajax({
                type: "PUT",
                url: `https://localhost:7177/api/v1/Employee/${employeeIdForUpdate}`,
                data: JSON.stringify(employee),
                dataType: "json",
                contentType: "application/json",
                success: function(response) {
                    // sau khi thực hiện thêm thì ẩn loading, ấn form chi tiết, load lại dữ liệu:
                    // hiển thị toast ms
                    showToast('success', 'Nhân viên đã được sửa.', ''); 
                    $("#dlgDialog").hide();
                    loadData();
                    
                },
                error: function(response) {
                    $(".m-loading").hide();
                    // console.log(response); // In ra response
                    let errorMsg = 'Có lỗi xảy ra';
                    if (response.responseJSON || response.responseJSON.errors) {
                        // Lấy lỗi đầu tiên trong đối tượng errors
                        errorMsg = response.responseJSON.userMsg;
                    }
                    console.log(response);
        
                    // hiển thị toast
                    showToast('error', errorMsg, '');
                }
            });
        }
    })
})

// ------------------Function-------------

// validate bắt buộc nhập
function validateInputRequired(input) {
    let value = $(input).val();
    if(value == null || value === "") {
        // set style cho ô nhập liệu
        $(input).addClass("m-input-error");

        // set thông tin tương ứng khi người dùng hover vào ô nhập liệu
        $(input).attr("title", "Thông tin này không được để trống!");

        return false;
    } else {
        // xóa style cho ô nhập liệu
        $(input).removeClass("m-input-error");
        $(input).removeAttr("title");

        return true;
    }
}

// validate với regex pattern
function validateInput(input, pattern, errorMessage) {
    let value = $(input).val();

    // Nếu giá trị null -> xóa class error nếu có
    if (value == null || value === "") {
        $(input).removeClass("m-input-error");
        $(input).removeAttr("title");
        return true;
    } 

    // Kiểm tra giá trị thẻ input với regex
    if (!pattern.test(value)) {
        $(input).addClass("m-input-error");
        $(input).attr("title", errorMessage);
        return false;
    } else {
        // xóa style cho ô nhập liệu
        $(input).removeClass("m-input-error");
        $(input).removeAttr("title");
        return true;
    }
}

/*
    Load dữ liệu từ API
    1. Gọi API với giá trị pageSize, currentPage (mặc định 10, 1)
    2. Gán giá trị employeesData cho data trả về
    3. Duyệt từng employee, gán giá trị cho các biến và truyền vào html
    4. Cho table append chuỗi html
 */
function loadData() {
    // clear dữ liệu cũ
    $("#tableEmployee tbody").empty();

    // clear dialog
    ClearDialog();

    // 1. Gọi API lấy dữ liệu
    // hiển thị loading
    $(".m-loading").show();
    $.ajax({
        type: "GET",
        url: `https://localhost:7177/api/v1/Employee/getpaging?pageSize=${pageSize}&pageIndex=${currentPage}`,
        success: function (response) {
            // khởi tạo biến đếm
            employeesData = response;
            
            totalPages = Math.ceil(totalEmployees / pageSize);
            renderTable(response);
            $(".m-loading").hide();
        },
        error: function(response) {
            debugger;
        }
    })
}

function renderTable(data) {
    $("#tableEmployee tbody").empty();
     let index = 1 + (currentPage - 1) * pageSize;
    // let startIndex = (currentPage - 1) * pageSize;
    // let endIndex = startIndex + pageSize;
    // let pageData = data.slice(startIndex, endIndex); // Lấy dữ liệu trang hiện tại
    
    for (const employee of data) {
        let employeeCode = employee.employeeCode;
        let fullName = employee.fullName;
        let genderName = employee.genderName;
        let dob = employee.dateOfBirth;
        let email = employee.email;
        let address = employee.address;

        if (dob) {
            dob = new Date(dob);
            let day = dob.getDate();
            day = day < 10 ? `0${day}` : day;
            let month = dob.getMonth() + 1;
            month = month < 10 ? `0${month}` : month;
            let year = dob.getFullYear();
            dob = `${day}/${month}/${year}`;
        } else {
            dob = "";
        }

        genderName = genderName || "";
        email = email || "";
        address = address || "";
        index = index < 10 ? `0${index}` : index;

        var el = $(`<tr>      
                     <td class="text-align-left">
                         ${index}
                     </td>
                     <td class="text-align-left">${employeeCode}</td>
                     <td class="text-align-left">${fullName}</td>
                     <td class="text-align-left">${genderName}</td>
                     <td class="text-align-center">${dob}</td>
                     <td class="text-align-left">${email}</td>
                     <td class="text-align-left col-address">
                         <div class="addrInfor">
                                ${address}
                         </div>
                         <div class="m-table-action">
                             <button id="btnEdit" class="btn-edit">
                                 <i class="fas fa-edit"></i>
                             </button>
                             <button id="btnCopy" class="btn-copy">
                                 <i class="fas fa-copy"></i>
                             </button>
                             <button id="btnDelete" class="btn-delete">
                                 <i class="fa fa-times"></i>
                             </button>
                         </div>
                     </td>
                 </tr>`);
        el.data("entity", employee);

        $("#tableEmployee tbody").append(el);
        index++;
    }
}

/* Clear dữ liệu */
function ClearDialog() {
    // Xoá các thông tin trong form sau khi thêm thành công
    $('.employee-form').find('input[type=text], input[type=number], input[type=date]').val('');
    $('.gender-radio input[type=radio]').prop('checked', false);
}

/* Clcik double vào 1 dòng */
function rowDblClick() {
    formMode = "edit";
    let employee = $(this).data("entity");

    employeeIdForUpdate = employee.employeeId;
    empForEdit = employee;
    dialogDisplay();
}

function dialogDisplay() {
    // hiển thị form
    // Hiển thị các thông tin nhân viên lên form
    $("#txtEmployeeCode").val(empForEdit.employeeCode);
    $("#txtFullName").val(empForEdit.fullName);
    
    // Chuyển định dạng dateOffBirth chỉ lấy yyyy/mm/dd
    var dob = empForEdit.dateOfBirth;
    if (typeof dob === 'string' && dob !== null) {
        dob = dob.split('T')[0];
    }

    $("#dtDateOfBirth").val(dob);

    // Chọn radio button theo giá trị của empForEdit.gender
    $('input[name="gender"][value="' + empForEdit.gender + '"]').prop("checked", true);

    $("#txtIdentityNumber").val(empForEdit.identityNumber);

    var idd = empForEdit.indentityDate;
    if (typeof idd === 'string' && idd !== null) {
        idd = idd.split('T')[0];
    }

    $("#txtIdentityDate").val(idd);
    $("#txtIdentityPlace").val(empForEdit.identityPlace);
    $("#txtAddress").val(empForEdit.address);
    $("#txtPhoneNumber").val(empForEdit.phoneNumber);
    $("#txtLandlinePhone").val(empForEdit.lanelineNumber);
    $("#txtEmail").val(empForEdit.email);
    $("#txtBankAccount").val(empForEdit.bankNumber);
    $("#txtBankName").val(empForEdit.bankName);
    $("#txtBankBranch").val(empForEdit.bankBranch);

    updateSelectedItem('cbxDepartmentId', 'dropdown-department', empForEdit.departmentId);
    updateSelectedItem('cbxPositionId', 'dropdown-position', empForEdit.positionId);

    // Hiển thị dialog
    $("#dlgDialog").show();
}

function updateSelectedItem(containerId, dropdownId, value) {
    // Xóa class selected từ tất cả các dropdown-item khác
    $('#' + dropdownId + ' .dropdown-item').removeClass('selected');

    // Tìm dropdown-item tương ứng với giá trị được chọn
    var $selectedItem = $('#' + dropdownId + ' .dropdown-item[mvalue="' + value + '"]');
    
    if ($selectedItem.length > 0) {
        // Cập nhật text và mvalue cho combobox
        $('#' + containerId).text($selectedItem.text());
        $('#' + containerId).attr('mvalue', value);

        // Thêm class selected cho dropdown-item được chọn
        $selectedItem.addClass('selected');
    }
}

/* Remove a Employee */
function RemoveEmployee(empId) {
    $.ajax({
        type: "DELETE",
        url: `https://localhost:7177/api/v1/Employee/${empId}`,
        success: function(response) {
            // sau khi thực hiện thêm thì ẩn loading, ấn form chi tiết, load lại dữ liệu:
            // hiển thị toast ms
            showToast('success', `Xoá thành công nhân viên ${empId}`, ''); 
            $("#dlgDialog").hide();
            GetTotal();
            
        },
        error: function(response) {
            $(".m-loading").hide();
            // console.log(response); // In ra response
            let errorMsg = 'Có lỗi xảy ra';
            if (response.responseJSON && response.responseJSON.errors) {
                // Lấy lỗi đầu tiên trong đối tượng errors
                let firstErrorField = Object.keys(response.responseJSON.errors)[0];
                errorMsg = response.responseJSON.errors[firstErrorField][0];
            }

            // hiển thị toast
            showToast('error', errorMsg, '');
        }
    });
}
// Get by id
function GetEmpById(employeeIdForUpdate) {   
    $.ajax({
        type: "GET",
        url: `https://localhost:7177/api/v1/Employee/${employeeIdForUpdate}`,
        success: function(response) {
            // sau khi thực hiện thêm thì ẩn loading, ấn form chi tiết, load lại dữ liệu:
            // hiển thị toast ms
            showToast('success', 'Lấy thành công nhân viên', ''); 
            $("#dlgDialog").hide();
            loadData();
            
        },
        error: function(response) {
            $(".m-loading").hide();
            // console.log(response); // In ra response
            let errorMsg = 'Có lỗi xảy ra';
            if (response.responseJSON && response.responseJSON.errors) {
                // Lấy lỗi đầu tiên trong đối tượng errors
                let firstErrorField = Object.keys(response.responseJSON.errors)[0];
                errorMsg = response.responseJSON.errors[firstErrorField][0];
            }

            // hiển thị toast
            showToast('error', errorMsg, '');
        }
    });
}

// Lấy tổng số nhân viên
function GetTotal() {
    $.ajax({
        type: "GET",
        url: `https://localhost:7177/api/v1/Employee/GetTotalEmployeeCount`,
        success: function (response) {
            totalEmployees = response;
            $(".m-paging .m-paging-left").html(`Tổng: ${totalEmployees}`);
            loadData();
        },
        error: function(response) {
            showToast("error", response, "");
        }
    })
}