$(function() {
    $('#inputUsername').focusout(function() {
        $.ajax('IsUsernameFree', { data: { username: $(this).val() } }).done(function(data) {
            $('#inputUsername').parents(".form-group").addClass("has-" + (data.response ? "success" : "error"));
            $('#btn-signup').prop('disabled', !data.response);
        });
    });
})