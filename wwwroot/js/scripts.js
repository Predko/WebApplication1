"use strict";

addEventListener("beforeunload", () => {
    // Save current scroll position.
    sessionStorage.setItem("currentScrollYPosition", document.getElementById("divTable").scrollTop);
}, false);

function restoreScrollPosition() {
    // Restore scroll position.
    var scrollTop = sessionStorage.getItem("currentScrollYPosition"),
        divScrollContainer = document.getElementById("divTable");

    if (scrollTop && divScrollContainer) {
        divScrollContainer.scrollTop = scrollTop;
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

