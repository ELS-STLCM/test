/**
 * Special Character Handling
 */
function enquote(jsonString) {
    return jsonString.replace(/\n/g, "\\n").replace(/\r/g, "\\r").replace(/\t/g, "\\t");
}
function encodeSpecialSymbols(value) {
    if (value != undefined) {
        value = value.replace(/\\/g, "\\\\");
        value = value.replace(/\"/g, "\\\"");
        return encodeURIComponent(value);
    } else {
    return value;
    }
}
function decodeSpecialSymbols(value) {
    if (isNaN(value)) {
        if (value.indexOf(" ") == -1) {
            value = decodeURIComponent(value);
            value = value.replace(/\\\\/g, "\\");
            value = value.replace(/\\\"/g, "\"");
            return value;
        }
        else {
            value = value.replace(/%2B/g, "+");
            value = value.replace(/%25/g, "%");
            return value;
        }
    }
    else {
        return value;
    }
}

function decodeSpecialHTMLSymbols(value) {
    //debugger;
    value = decodeSpecialSymbols(value);
    return value.replace(/\n/g,"<br/>");
}

function setControlHTMLValueByElementId(controlName, value) {
    var controlInstance = $(controlName);
    controlInstance.html(decodeSpecialSymbols(value));
}

function setControlStringValueByElementId(controlName, value) {
    var controlInstance = $(controlName);
    controlInstance.text(value);    
}

function replaceDoubleQuotes(stringValue) {    
    return stringValue.replace(/"/g, '\\"');
}
function replaceDoubleQuotesInBackPack(stringValue) {
    stringValue = stringValue.replace(/\\/g, "\\\\");
    stringValue = stringValue.replace(/\“/g, "\"");
    stringValue = stringValue.replace(/\”/g, "\"");
    stringValue = stringValue.replace(/\"/g, "\\\\&quot;");
    return encodeURIComponent(stringValue);
}
function restoreDoubleQuotes(stringValue) {    
    return stringValue.replace(/\\"/g,'\"');
}

function encodeHTML(stringValue) {
    return stringValue.split('&').join('&amp;').split('<').join('&lt;').split('"').join('&quot;').split("'").join('&#39;').split(" ").join('&#32;');
}

function replaceHTMLBreak(stringValue) {
    return stringValue.replace(/\n/g, "<br/>");
}

function replaceSingleAndDoubleQuotes(stringValue) {
    
    return stringValue.replace(/"/g, '\"')
                      .replace(/\'/g, "\'");
}
function replaceSpecialCharacters(stringValue) {
    stringValue = replaceSingleAndDoubleQuotes(stringValue);
    stringValue=stringValue.replace("&quot;", "\"").replace("&amp;", "&");
    return stringValue.replace(/\n/g, "\\n");
}
