function ShowLoader() {
    $('#loaderModal').show();
}

function HideLoader() {
    $('#loaderModal').hide();
}

function ShowPopupMsg(message) {
    $('#msgDiv').text("");
    $('#msgDiv').text(message);
    $('#messageModal').show();
}

function HidePopupMsg() {
    $('#messageModal').hide();
}

//$(".datepicker").datepicker();
//$(".datepicker1").datepicker();
