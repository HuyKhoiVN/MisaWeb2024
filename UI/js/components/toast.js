function showToast(type, message, helpMsg) {
    let toastClass, toastIconClass, toastTitle;
    
    if (type === 'success') {
        toastClass = 'm-toast-success';
        toastIconClass = 'fa fa-check-circle';
        toastTitle = 'Thành công!';
    } else if (type === 'error') {
        toastClass = 'm-toast-error';
        toastIconClass = 'fas fa-exclamation-circle';
        toastTitle = 'Lỗi!';
    }

    let toastHtml = `
        <div class="m-toast-item ${toastClass}">
            <div class="m-toast-icon"><i class="${toastIconClass}"></i></div>
            <div class="m-toast-text">
                <span class="m-toast-text-message">
                    <span class="m-toast-text-title">${toastTitle}</span>
                    <span class="m-toast-text-detail">${message}</span>
                </span>
                <span class="m-toast-text-link">${helpMsg}</span>
            </div>
            <div class="m-toast-close"><i class="fas fa-times"></i></div>
        </div>`;
    
    let $toastMsg = $(toastHtml);
    $("#toastBox").append($toastMsg);
    $("#toastBox").show();

    // Click vào nút đóng để đóng toast-item
    $toastMsg.find('.m-toast-close').on('click', function() {
        $toastMsg.remove();
        if ($("#toastBox").children().length === 0) {
            $("#toastBox").hide();
        }
    });

    setTimeout(function () {
        $toastMsg.remove();
        if ($("#toastBox").children().length === 0) {
            $("#toastBox").hide();
        }
    }, 1500); 
}