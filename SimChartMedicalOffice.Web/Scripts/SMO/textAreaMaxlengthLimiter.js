// Get all textareas that have a "maxlength" property. Now, and when later adding HTML using jQuery-scripting:
$('textarea').live('keyup blur', function () {
    // Store the maxlength and value of the field.
    var maxlength = 4096;
    var val = $(this).val();
    // Trim the field if it has content over the maxlength.
    if (val.length > maxlength) {
        $(this).val(val.slice(0, maxlength));
    }
});

// Get all textboxes that have a "maxlength" property. Now, and when later adding HTML using jQuery-scripting:
$('input[type="text"]').live('keyup blur', function () {
    // Store the maxlength and value of the field.
    var maxlength = 250;
    var val = $(this).val();
    // Trim the field if it has content over the maxlength.
    // CHECK for max length attr is present 
    if ($(this).attr('maxlength') == undefined) {
        // attribute does not exist
        if (val.length > maxlength) {
            $(this).val(val.slice(0, maxlength));
        }
    }
});