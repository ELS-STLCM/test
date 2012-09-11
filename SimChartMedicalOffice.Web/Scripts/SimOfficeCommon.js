function encodeSpecialSymbols(value) {
    if (value != undefined) {
        value = value.replace(/\\/g, "\\\\");
        value = value.replace(/\"/g, "\\\"");
        return encodeURIComponent(value);
    } else {
        return value;
    }
}