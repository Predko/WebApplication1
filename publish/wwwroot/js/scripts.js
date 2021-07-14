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
// Решение позаимствовано из:
// https://coderoad.ru/14267781/Сортировка-таблицы-HTML-с-JavaScript#49041392
// и
// из: https://coderoad.ru/14267781/Сортировка-таблицы-HTML-с-JavaScript#53880407


function getCellValue(row, indexTh) {
    return row.children[indexTh].innerText || row.children[indexTh].textContent;
}

function getComparer(indexTh, asc) {
    function compareCells(v1, v2) {
        if (v1 !== '' && v2 !== '' && !isNaN(v1) && !isNaN(v2)) {
            return v1 - v2;
        }

        return v1.toString().localeCompare(v2);
    }

    var comparer = function (a, b) {
        return compareCells(getCellValue(asc ? a : b, indexTh), getCellValue(asc ? b : a, indexTh));
    }

    return comparer;
}

// Извлекает порядок сортировки из атрибута data-sort-order
// и возвращает true  - если текущее значение сортировки "ascending"
//              false - если "descending"
//              undefined - "disabled" - сортировка запрещена.
// Меняет значение сортировки на противоположное.
// Сохраняет порядок сортировки для столбца в сессионном хранилище.
function sortOrderFromTh(th) {
    var sortOrder;
    let resSortOrder;

    var indexTh = Array.from(th.parentNode.children).indexOf(th);

    sortOrder = sessionStorage.getItem("Sort.Order.Th" + indexTh);

    if (sortOrder == undefined || sortOrder == null) {
        switch (th.dataset.sortOrder) {
            case "none":
            case "ascending":
                resSortOrder = true;
                th.dataset.sortOrder = "descending";
                break;
            case "descending":
                resSortOrder = false;
                th.dataset.sortOrder = "ascending";
                break;
            case "disabled":
                return undefined;
        }
    }
    else {
        resSortOrder = (sortOrder === 'false');
    }

    sessionStorage.setItem("Sort.ThIndex", indexTh);
    sessionStorage.setItem("Sort.Order.Th" + indexTh, resSortOrder);

    return resSortOrder;
}

document.querySelectorAll('th')
    .forEach(th => th.addEventListener('click',
        (() => {
            var sortOrder = sortOrderFromTh(th);
            if (sortOrder === undefined) {
                return;
            }

            const table = th.closest('table');
            const tbody = table.querySelector('tbody');
            Array.from(tbody.querySelectorAll('tr'))
                .sort(getComparer(Array.from(th.parentNode.children).indexOf(th), sortOrder))
                .forEach(tr => tbody.appendChild(tr));

            document.getElementById("divTable").scrollTop = 0;
        }
        )));


