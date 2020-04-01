var selectionsCounter = 0;

function packData() {
    var selections = new Array();

    for (i = 0; i < selectionsCounter; i++) {
        var element = document.getElementById('selection-' + i.toString()).childNodes[0];
        selections.push(element.value);
    }

    document.getElementsByName('encodedData')[0].value = btoa(JSON.stringify(selections));

    document.getElementById('selectionsForm').submit();
}

function addSelection() {
    // Create a container contains text input
    var container = document.createElement('div');
    container.className = "uk-margin";
    container.id = 'selection-' + selectionsCounter.toString();

    // Create text input with UIKit style
    var inputArea = document.createElement('input');
    inputArea.className = 'uk-input';
    inputArea.type = 'text';
    inputArea.placeholder = 'Input selection content here';

    // Group elements
    container.appendChild(inputArea);

    // Add to page
    document.getElementById('selections').appendChild(container);

    selectionsCounter++;
}
