"use strict";


document.querySelector('table').onclick = (event) => {
    var cell = event.target;
    if (cell.tagName.toLowerCase() != 'td')
        return;
    var i = cell.parentNode.rowIndex;
    var j = cell.cellIndex;
    var currentTr = cell.parentNode;
    window.location.href = "customers?customer=" + currentTr.id;
}
