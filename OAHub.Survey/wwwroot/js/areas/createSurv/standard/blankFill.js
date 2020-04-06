function packData() {
    if (document.getElementById('displayType').checked) {
        document.getElementsByName('encodedData')[0].value = btoa('true');
    } else {
        document.getElementsByName('encodedData')[0].value = btoa('false');
    }
    document.getElementById('blankFillForm').submit();
}