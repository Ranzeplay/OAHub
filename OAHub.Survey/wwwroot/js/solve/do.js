function packData() {
    var array = new Array();

    var all = $('#question-body')[0].children;
    for (var i = 0; i < all.length; i++) {
        var element = all.item(i);
        switch (element.getAttribute('question-type')) {
            case 'singleSelect':
                array.push(packSingleSelectData(element.getAttribute('id').split('-')[1]));
                break;
            case 'multiSelect':
                array.push(packMultiSelectData(element.getAttribute('id').split('-')[1]));
                break;
            case 'blankFill':
                array.push(packBlankFillData(element.getAttribute('id').split('-')[1]));
                break;
            case 'check':
                array.push(packCheckData(element.getAttribute('id').split('-')[1]));
                break;
            default:
        }
    }


    document.getElementsByName('encodedData')[0].value = btoa(JSON.stringify(array));
    document.getElementById('answerForm').submit();
}

function packCheckData(questionId) {
    var element = document.getElementsByName('ch-' + questionId)[0];
    if (element != null) {
        if (element.checked) {
            return btoa(questionId + '-' + 'True');
        } else {
            return btoa(questionId + '-' + 'False');
        }
    }
    else {
        throw new Error('Question not found!');
    }
}

function packBlankFillData(questionId) {
    var element = document.getElementsByName('bf-' + questionId)[0];
    if (element != null) {
        return btoa(questionId + '-' + btoa(element.value));
    }
    else {
        throw new Error('Question not found!');
    }
}

function packMultiSelectData(questionId) {
    var selectedIndexes = new Array();

    var element = document.getElementById(questionId + '-ms-selections');
    if (element != null) {
        document.getElementsByName('ms-' + questionId).forEach(sub => {
            if (sub.checked) {
                selectedIndexes.push(sub.getAttribute('index'));
            }
        });
    } else {
        throw new Error('Question not found!');
    }

    return btoa(questionId + '-' + btoa(JSON.stringify(selectedIndexes)));
}

function packSingleSelectData(questionId) {
    var index;
    var element = document.getElementById(questionId + '-ss-selections');
    if (element != null) {
        document.getElementsByName('ss-' + questionId).forEach(sub => {
            if (sub.checked) {
                index = sub.getAttribute('index');
            }
        });
    } else {
        throw new Error('Question not found!');
    }

    return btoa(questionId + '-' + index);
}
