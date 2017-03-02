Date.prototype.toDateInputValue = (function() {
    var local = new Date(this);
    local.setMinutes(this.getMinutes() - this.getTimezoneOffset());
    return local.toJSON().slice(0,10);
});


$(function(){
    $('select').material_select();
    $('#current-day').val(new Date().toDateInputValue());
});
