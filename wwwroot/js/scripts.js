"use strict";

addEventListener("beforeunload", () => {
    // Save current scroll position.
    sessionStorage.setItem("currentScrollYPosition", document.getElementById("divTable").scrollTop);
}, false);

function restoreScrollPosition() {
    // Restore scroll position.
    var scrollTop = sessionStorage.getItem("currentScrollYPosition"),
        divScrollContainer = document.getElementById("divTable");

    var indexTh = sessionStorage.getItem("Sort.ThIndex");

    if (indexTh === undefined || indexTh === null) {
        indexTh = 0;
        sessionStorage.setItem("Sort.ThIndex", indexTh);
        sessionStorage.setItem("Sort.Order.Th" + indexTh, true);
    }

    var so = sessionStorage.getItem("Sort.Order.Th" + indexTh);
    var sortOrder = (so === undefined) ? undefined : so === 'true';

    SortTable(sortOrder, divScrollContainer, indexTh);

    if (scrollTop && divScrollContainer) {
        divScrollContainer.scrollTop = scrollTop;
    }
}


function SortTable(sortOrder, divScrollContainer, indexTh) {
    if (sortOrder !== undefined) {
        const table = divScrollContainer.querySelector('table');
        const tbody = table.querySelector('tbody');
        Array.from(tbody.querySelectorAll('tr'))
            .sort(getComparer(indexTh, sortOrder))
            .forEach(tr => tbody.appendChild(tr));
    }
}

document.querySelector('table').onclick = (event) => {
    var cell = event.target;
    if (cell.tagName.toLowerCase() != 'td')
        return;
    var i = cell.parentNode.rowIndex;
    var j = cell.cellIndex;
    var currentTr = cell.parentNode;
    window.location.href = "customers?customer=" + currentTr.id;
}

function resizebody() {
    var container = document.getElementById("divTable");
    var titleTable = document.getElementById("titleTable");

    var w = window,
        d = document,
        e = d.documentElement,
        g = d.getElementsByTagName('body')[0],
        y = w.innerHeight || e.clientHeight || g.clientHeight,
        headerOffset = document.getElementById("header_body").offsetHeight,
        footerOffset = document.getElementById("footer_body").offsetHeight,
        stt = getComputedStyle(titleTable),
        titleTableOffset = titleTable.offsetHeight,
        styleBody = getComputedStyle(g);

    container.style.height = (y - headerOffset - footerOffset - titleTableOffset
        - parseInt(stt.paddingBottom)
        - parseInt(stt.paddingTop)
        - parseInt(styleBody.marginBottom)
        - parseInt(styleBody.marginTop)) + "px";

    restoreScrollPosition();
}

