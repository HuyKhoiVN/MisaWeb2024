//khai báo biến toàn cục
var formMode = "edit";
var employeeIdForUpdate = null;
var empForEdit = null;

$(document).ready(function () {
    //-------->1. Load dữ liệu từ JSON
    //-------->2. Hiển thị dữ liệu lên UI
    loadData();

    //-------->3. Gán các sự kiện
    //---3.1. Nhấn thêm mới:
    $("#btnAdd").click(function () {
        formMode = "add";
        //--hiển thị form thêm mới
        $("#dlgDialog").show();
        //--focus vào ô đầu tiên
        $("#txtEmployeeCode").focus();
    })

    //---3.2. ấn btn-close
    $(".m-dialog-close").click(function () {
        //ẩn form
        $("#dlgDialog").hide();
    })

    //add class
    
    //---3.3. double click vào 1 dòng trong table
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
    
                employeeIdForUpdate = employee.EmployeeId;
                empForEdit = employee;
    
                $(".m-table-action .btn-delete").on("click", function() {
                    try {
                        if (confirm(`Bạn có chắc chắn muốn xóa nhân viên ${employee.FullName} có mã là ${employee.EmployeeCode} không?`)) {
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

    //4. Thực hiện hành động trong form thêm mới
    $("#btnSave").click(function () {
        //---4.1. lấy dữ liệu từ form

        //họ tên, mã nhân viên k được trống
        //ngày sinh không lớn hơn ngày hiện tại
        //email đúng định dạng
        //sđt phải đúng định dạng

        let employeeCode = $("#txtEmployeeCode").val();
        let fullName = $("#txtFullName").val();
        let dateOfBirth = $("#dtDateOfBirth").val();
        let gender = $('input[name="gender"]:checked').val();
        let position = $("#cbxPositionId").attr('mvalue');
        let identityNumber = $("#txtIdentityNumber").val();
        let identityDate = $("#txtIdentityDate").val();
        let department = $("#cbxDepartmentId").attr('mvalue');
        let identityPlace = $("#txtIdentityPlace").val();
        let address = $("#txtAddress").val();
        let phoneNumber = $("#txtPhoneNumber").val();
        let landlinePhone = $("#txtLandlinePhone").val();
        let email = $("#txtEmail").val();
        let bankAccount = $("#txtBankAccount").val();
        let bankName = $("#txtBankName").val();
        let bankBranch = $("#txtBankBranch").val();

        //---4.2. Validate dữ liệu
        //------alert mã nv
        if(employeeCode == null || employeeCode === "") {
            alert("Mã nhân viên không được để trống");
            return false;
        }

        //------alert họ và tên
        if(fullName == null || fullName === "") {
            alert("Họ tên không được để trống");
            return false;
        }
        
        //------alert ngày sinh
        if(dateOfBirth) {
            dateOfBirth = new Date(dateOfBirth);
        }
        if(dateOfBirth > new Date()) {
            alert("Ngày không được phép lớn hơn ngày hiện tại");
            return false;
        }

        //-----alert nếu thông tin được điền chưa hợp lệ
        if ($(".m-input-error").length > 0) {
            alert("Vui lòng điền đúng các trường thông tin!");
            return false;
        }

        //-----hiển thị trạng thái lỗi validate khi không nhập các trường bắt buộc:
        $("input[required]").blur(function () {
            validateInputRequired(this);
        })

        //-----hiển thị trạng thái lỗi với email sai định dạng
        $("input[email]").blur(function () {
            //email regex
            let pattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;

            let errorMessage = "Email không đúng định dạng!";
            validateInput(this, pattern, errorMessage);
        })

        //-----hiển thị lỗi khi sdt sai định dạng
        $("#txtPhoneNumber").blur(function () {
            //sdt regex
            let pattern = /^\+?[0-9]*$/;

            let errorMessage = "Số điện thoại không đúng định dạng! Chỉ được phép chứa dấu + và số 0-9.";
            validateInput(this, pattern, errorMessage);
        })

        //---4.3. Build object và thêm dữ liệu
        let positionfix = "548dce5f-5f29-4617-725d-e2ec561b0f41";
        let departmentfix = "75cce5a6-4f7f-3671-3cf3-eb0223d0a4f7";

        //-----Khởi tạo đối tượng emp
        let employee = {
            "EmployeeCode": employeeCode,
            "FullName": fullName,
            "Gender": gender,
            "DateOfBirth": dateOfBirth,
            "PhoneNumber": phoneNumber,
            "Email": email,
            "PositionId": positionfix,
            "DepartmentId": departmentfix,
            "Address": address
        }
        
        //-----Hiện form loading, thực hiện truyền json emp
        $(".m-loading").show();
        //check formMode
        if (formMode == "add") {
            $.ajax({
                type: "POST",
                url: "https://cukcuk.manhnv.net/api/v1/Employees/",
                data: JSON.stringify(employee),
                dataType: "json",
                contentType: "application/json",
                success: function(response) {
                    //sau khi thực hiện thêm thì ẩn loading, ấn form chi tiết, load lại dữ liệu:
                    //hiển thị toast ms
                    showToast('success', 'Nhân viên đã được thêm.', ''); 
                    $("#dlgDialog").hide();
                    loadData();
                    
                },
                error: function(response) {
                    $(".m-loading").hide();
                    //console.log(response); // In ra respone
                    let errorMsg = 'Có lỗi xảy ra';
                    if (response.responseJSON && response.responseJSON.errors) {
                        // Lấy lỗi đầu tiên trong đối tượng errors
                        let firstErrorField = Object.keys(response.responseJSON.errors)[0];
                        errorMsg = response.responseJSON.errors[firstErrorField][0];
                    }
    
                    //hiển thị toast
                    showToast('error', errorMsg, '');
                }
            });
        }else {
            $.ajax({
                type: "PUT",
                url: `https://cukcuk.manhnv.net/api/v1/Employees/${employeeIdForUpdate}`,
                data: JSON.stringify(employee),
                dataType: "json",
                contentType: "application/json",
                success: function(response) {
                    //sau khi thực hiện thêm thì ẩn loading, ấn form chi tiết, load lại dữ liệu:
                    //hiển thị toast ms
                    showToast('success', 'Nhân viên đã được sửa.', ''); 
                    $("#dlgDialog").hide();
                    loadData();
                    
                },
                error: function(response) {
                    $(".m-loading").hide();
                    //console.log(response); // In ra respone
                    let errorMsg = 'Có lỗi xảy ra';
                    if (response.responseJSON && response.responseJSON.errors) {
                        // Lấy lỗi đầu tiên trong đối tượng errors
                        let firstErrorField = Object.keys(response.responseJSON.errors)[0];
                        errorMsg = response.responseJSON.errors[firstErrorField][0];
                    }
                    console.log(response);
        
                    //hiển thị toast
                    showToast('error', errorMsg, '');
                }
            });
        }
    })
})

//------------------Function-------------

//validate bắt buộc nhập
function validateInputRequired(input) {
    let value = $(input).val();
    if(value == null || value === "") {
        //set style cho ô nhập liệu
        $(input).addClass("m-input-error");

        //set thông tin tương ứng khi người dùng hover vào ô nhập liệu
        $(input).attr("title", "Thông tin này không được để trống!");

        return false;
    } else {
        //xóa style cho ô nhập liệu
        $(input).removeClass("m-input-error");
        $(input).removeAttr("title");

        return true;
    }
}

//validate với regex pattern
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
        //xóa style cho ô nhập liệu
        $(input).removeClass("m-input-error");
        $(input).removeAttr("title");
        return true;
    }
}

/*
    Load dữ liệu từ API
 */
function loadData() {
    //clear dữ liệu cũ
    $("#tableEmployee tbody").empty();

    //clear dialog
    ClearDialog();
    //1. Gọi API lấy dữ liệu
    //hiển thị loading
    $(".m-loading").show();
    $.ajax({
        type: "GET",
        url: "https://cukcuk.manhnv.net/api/v1/Employees",
        success: function (respone) {
            //khởi tạo biến đếm
            let index = 1;

            //duyệt từng đối tượng trong mảng
            for(const employee of respone) {
                //lấy thông tin cần thiết của từng đối tượng
                let employeeCode = employee.EmployeeCode;
                let fullName = employee.FullName;
                let genderName = employee.GenderName;
                let dob = employee.DateOfBirth;
                let email = employee.Email;
                let address = employee.Address;

                //định dạng dữ liệu
                // 1. Ngày tháng hiển thị ngày-tháng-năm
                if(dob) {
                    dob = new Date(dob);

                    //lấy ra ngày
                    let day = dob.getDate();
                    day = day < 10 ? `0${day}` : day; //nếu ngày <10 thì thêm 0 vào trước

                    //lấy ra tháng
                    let month = dob.getMonth() + 1;
                    month = month < 10 ? `0${month}` : month; //nếu tháng < 10 thì thêm 0 vào trước

                    //lấy ra năm
                    let year = dob.getFullYear();
                    //lấy giá trị ngày/tháng/năm
                    dob = `${day}/${month}/${year}`
                }else {
                    dob = "";
                }

                //2. Định dạng tiền tệ
                //lấy ra giá trị tiền tệ
                // let salary = employee.Salary;
                // salary = new Intl.NumberFormat('nv-VI', { style: 'currency', currency: 'VND' }).format(salary);

                //3. xử lý dữ liệu null cho giới tính, email, địa chỉ
                genderName != null ? genderName = genderName : genderName = ""; //nếu giới tính không null thì lấy giá trị, ngược lại thì không hiển thị
                email != null ? email = email : email = ""; //nếu email không null thì lấy
                address != null ? address = address : address = ""; //nếu địa chỉ không null thì lấy

                //4.thêm số 0 nếu index < 10
                index = index < 10 ? `0${index}` : index;

                //build chuỗi html
                var el = $(`<tr>      
                             <td class="text-align-left">
                                 ${index}
                                 <!-- <input type="checkbox"  name="" id="" checked> -->
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
                $(".m-loading").hide();
            }
        },
        error: function(respone) {
            debugger;
        }
    })
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

    employeeIdForUpdate = employee.EmployeeId;
    empForEdit = employee;
    dialogDisplay();
}

function dialogDisplay() {
    //hiển thị form
    $("#txtEmployeeCode").val(empForEdit.EmployeeCode);
    $("#txtFullName").val(empForEdit.FullName);
    $('input[name="gender"]:checked').val(empForEdit.Gender);
    $("#txtAddress").val(empForEdit.Address);
    $("#txtPhoneNumber").val(empForEdit.PhoneNumber);
    $("#txtEmail").val(empForEdit.Email);

    $("#dlgDialog").show();
}

/* Edit Employee */
function EditEmployee() {
    
}

/* Remove a Employee */
function RemoveEmployee(empId) {
    $.ajax({
        type: "DELETE",
        url: `https://cukcuk.manhnv.net/api/v1/Employees/${empId}`,
        success: function(response) {
            //sau khi thực hiện thêm thì ẩn loading, ấn form chi tiết, load lại dữ liệu:
            //hiển thị toast ms
            showToast('success', `Xoá thành công nhân viên ${empId}`, ''); 
            $("#dlgDialog").hide();
            loadData();
            
        },
        error: function(response) {
            $(".m-loading").hide();
            //console.log(response); // In ra respone
            let errorMsg = 'Có lỗi xảy ra';
            if (response.responseJSON && response.responseJSON.errors) {
                // Lấy lỗi đầu tiên trong đối tượng errors
                let firstErrorField = Object.keys(response.responseJSON.errors)[0];
                errorMsg = response.responseJSON.errors[firstErrorField][0];
            }

            //hiển thị toast
            showToast('error', errorMsg, '');
        }
    });
}
// Get by id
function GetEmpById(employeeIdForUpdate) {   
    $.ajax({
        type: "GET",
        url: `https://cukcuk.manhnv.net/api/v1/Employees/${employeeIdForUpdate}`,
        success: function(response) {
            //sau khi thực hiện thêm thì ẩn loading, ấn form chi tiết, load lại dữ liệu:
            //hiển thị toast ms
            showToast('success', 'Lấy thành công nhân viên', ''); 
            $("#dlgDialog").hide();
            loadData();
            
        },
        error: function(response) {
            $(".m-loading").hide();
            //console.log(response); // In ra respone
            let errorMsg = 'Có lỗi xảy ra';
            if (response.responseJSON && response.responseJSON.errors) {
                // Lấy lỗi đầu tiên trong đối tượng errors
                let firstErrorField = Object.keys(response.responseJSON.errors)[0];
                errorMsg = response.responseJSON.errors[firstErrorField][0];
            }

            //hiển thị toast
            showToast('error', errorMsg, '');
        }
    });
}