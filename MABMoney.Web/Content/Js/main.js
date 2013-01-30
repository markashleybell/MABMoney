$(function () {
    $('.date-picker').datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true
    });

    $('#net-worth').popover({
        title: 'Accounts',
        placement: 'bottom',
        html: true,
        content: $('#net-worth-content').html()
    });
});