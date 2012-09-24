/**
 * Numeric Validations
 */
//regular expression for three digit number
var threeDigitNumber = /^\+?\d*$/;
var threeDigitPositiveDecimalNumber = /^\+?\d*?([\.]\d{1,2})?$/;
var threeDigitDecimal = /^\d*?([\.])\d{2}$/;
var decimaldigit = /^\+?\d*?([\.]\d*)?$/;
var temperatureFahrenheit = /^\d*?([\.]\d{1})?$/;
var postiveNegativeInteger = /^(\+|-)?\d+$/;
var alphaNumeric = /^[a-zA-Z0-9\s]/;
var numeric = /^[0-9]/;

//Function to validate the Integers
function IsValidNumeric(value) {
    if (value != null && value != "") {
        if (value.search(threeDigitNumber) == -1) {
            return false;
        }
        else {
            return true;
        }
    }
    return true;
}
function isValidPositiveNegativeInteger(value){
if (value != null && value != "") {
        if (value.search(postiveNegativeInteger) == -1) {
            return false;
        }
        else {
            return true;
        }
    }
    return true;    
}

//Function to validate Decimal value
function IsValidDecimal(value) {
    if (value.search(threeDigitDecimal) == -1) {
        return false;
    }
    else {
        return true;
    }
}


//Function to validate Positive Decimal value
function IsValidPositiveNumeric(value) {
    if (value != null && value != "") {
        if (value.search(threeDigitPositiveDecimalNumber) == -1) {
            return false;
        }
        else {
            return true;
        }
    }
    return true;
}

//Function to validate numeric numbers on key press.
function isNumeric(event) {
    var inputString = '';
    if (event != undefined) {
        var evt = (event.which == undefined) ? event.keyCode : event.which;
        if (evt == 8 || evt == 0) {
            return true;
        }
        inputString = String.fromCharCode(evt);
        if (numeric.test(inputString)) {
            return true;
        }
        else {
            return false;
        }
    }
    return false;
}


//Function to validate alphanumeric numbers on key press.
function isAlphaNumeric(event) {    
    var inputString = '';
    if (event != undefined) {
        var evt = (event.which == undefined) ? event.keyCode : event.which;
        if (evt == 8 || evt == 0) {
            return true;
        }        
        inputString = String.fromCharCode(evt);                   
        if (alphaNumeric.test(inputString)) {
            return true;
        }
        else {
            return false;
            }  
    }
    return false;
}

function removeNonAlphaNumeric() {
    var initVal = $(this).val();
    outputVal = initVal.replace(/[^0-9a-zA-Z]/g, "");
    if (initVal != outputVal) {
        $(this).val(outputVal);
    }
}

function removeNonNumeric() {
    var initVal = $(this).val();
    outputVal = initVal.replace(/[^0-9]/g, "");
    if (initVal != outputVal) {
        $(this).val(outputVal);
    }
}
