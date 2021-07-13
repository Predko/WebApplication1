"use strict";
// Решение позаимствовано из:
// https://coderoad.ru/14267781/Сортировка-таблицы-HTML-с-JavaScript#49041392
// и
// из: https://coderoad.ru/14267781/Сортировка-таблицы-HTML-с-JavaScript#53880407
// Sort table.
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
    };
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
    var resSortOrder;
    var indexTh = Array.prototype.slice.call(th.parentNode.children).indexOf(th);
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
    sessionStorage.setItem("Sort.ThIndex", indexTh.toString());
    sessionStorage.setItem("Sort.Order.Th" + indexTh, resSortOrder);
    return resSortOrder;
}
document.querySelectorAll('th')
    .forEach(function (th) { return th.addEventListener('click', (function () {
    var sortOrder = sortOrderFromTh(th);
    if (sortOrder === undefined) {
        return;
    }
    var indexTh = Array.prototype.slice.call(th.parentNode.children).indexOf(th);
    var table = th.closest('table');
    var tbody = table.querySelector('tbody');
    Array.prototype.slice.call(tbody.querySelectorAll('tr'))
        .sort(getComparer(indexTh, sortOrder))
        .forEach(function (tr) { return tbody.appendChild(tr); });
    document.getElementById("divTable").scrollTop = 0;
})); });
addEventListener("beforeunload", function () {
    // Save current scroll position.
    sessionStorage.setItem("currentScrollYPosition", String(document.getElementById("divTable").scrollTop));
}, false);
function restoreScrollPosition() {
    // Restore scroll position.
    var scrollTop = Number(sessionStorage.getItem("currentScrollYPosition")), divScrollContainer = document.getElementById("divTable");
    var indexTh = Number(sessionStorage.getItem("Sort.ThIndex"));
    if (indexTh === undefined || indexTh === null) {
        indexTh = 0;
        sessionStorage.setItem("Sort.ThIndex", String(indexTh));
        sessionStorage.setItem("Sort.Order.Th" + indexTh, String(true));
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
        var table = divScrollContainer.querySelector('table');
        var tbody_1 = table.querySelector('tbody');
        Array.prototype.slice.call(tbody_1.querySelectorAll('tr'))
            .sort(getComparer(indexTh, sortOrder))
            .forEach(function (tr) { return tbody_1.appendChild(tr); });
    }
}
document.querySelector('table').onclick = function (event) {
    var cell = event.target;
    if (cell.tagName.toLowerCase() != 'td')
        return;
    var i = cell.parentNode.rowIndex;
    var j = cell.cellIndex;
    var currentTr = cell.parentNode;
    var action = "edit";
    var paramName = currentTr.closest('table').getAttribute("data-parameter");
    window.location.href = createPathWithNewParameter(action, paramName, currentTr.id);
};
// Создание адреса с параметрами.
function createPathWithNewParameter(action, param, value) {
    var url = new URL(window.location.href);
    var params = url.search;
    if (params === "" || params === null || params === undefined) {
        params = "?";
    }
    else {
        params += "&";
    }
    return url.pathname.concat("/", action, params, param + "=" + value);
}
function resizebody() {
    var container = document.getElementById("divTable");
    var titleTable = document.getElementById("titleTable");
    var w = window, d = document, e = d.documentElement, g = d.getElementsByTagName('body')[0], y = w.innerHeight || e.clientHeight || g.clientHeight, headerOffset = document.getElementById("header_body").offsetHeight, footerOffset = document.getElementById("footer_body").offsetHeight, stt = getComputedStyle(titleTable), titleTableOffset = titleTable.offsetHeight, styleBody = getComputedStyle(g);
    container.style.height = (y - headerOffset - footerOffset - titleTableOffset
        - parseInt(stt.paddingBottom)
        - parseInt(stt.paddingTop)
        - parseInt(styleBody.marginBottom)
        - parseInt(styleBody.marginTop)) + "px";
    restoreScrollPosition();
}
// Добавляет контекстное меню к строке таблицы.
var menu = document.querySelector("#context-menu");
var menuItems = menu.querySelectorAll(".context-menu__item");
var menuState = 0;
var taskItemClassName = "task";
var taskItemInContext;
var contextMenuClassName = "context-menu";
var contextMenuItemClassName = "context-menu__item";
var contextMenuLinkClassName = "context-menu__link";
var contextMenuActive = "context-menu--active";
(function () {
    init();
    // Инициализация.
    function init() {
        contextListener();
        clickListener();
        keyupListener();
    }
    // Обработчик события "contextmenu".
    // При правом клике на строке таблицы, отображает контекстное меню.
    // Если правый клик не на строке таблицы - скрывает.
    function contextListener() {
        document.addEventListener("contextmenu", function (e) {
            taskItemInContext = clickInsideElement(e, taskItemClassName);
            if (taskItemInContext) {
                e.preventDefault();
                toggleMenuOn();
                positionMenu(e);
            }
            else {
                taskItemInContext = null;
                toggleMenuOff();
            }
        });
    }
    // Проверяет, соответствует ли элемент(в аргументе события) указанному в переменной className классу.
    // Если да - возвращает элемент, если нет - false.
    function clickInsideElement(e, className) {
        var el = e.srcElement || e.target;
        if (el.classList.contains(className)) {
            return el;
        }
        else {
            while (el = el.parentNode) {
                if (el.classList && el.classList.contains(className)) {
                    return el;
                }
            }
        }
        return false;
    }
    // Показывает меню.
    function toggleMenuOn() {
        if (menuState !== 1) {
            menuState = 1;
            menu.classList.add(contextMenuActive);
        }
    }
    // Скрывает меню.
    function toggleMenuOff() {
        if (menuState !== 0) {
            menuState = 0;
            menu.classList.remove(contextMenuActive);
        }
    }
    // Обработчик клика мыши.
    // Если нажата левая кнопка не на контекстном меню - скрывает меню.
    function clickListener() {
        var leftBtn = 0;
        document.addEventListener("click", function (e) {
            var clickeElIsLink = clickInsideElement(e, contextMenuLinkClassName);
            if (clickeElIsLink) {
                e.preventDefault();
                menuItemListener(clickeElIsLink);
            }
            else {
                var button = e.which || e.button;
                if (button === 1) {
                    toggleMenuOff();
                }
            }
        });
    }
    function menuItemListener(link) {
        var currentTr = taskItemInContext;
        var action = currentTr.getAttribute("data-action");
        var paramName = currentTr.closest("table").getAttribute("data-parameter");
        var path = createPathWithNewParameter(link.getAttribute("data-action"), paramName, currentTr.id);
        window.location.href = path;
        toggleMenuOff();
    }
    // Обработчик нажатия клавиши на клавиатуре.
    // Если нажата клавиша ESC - скрывает меню.
    function keyupListener() {
        var ESC = 27;
        window.onkeyup = function (e) {
            if (e.keyCode === ESC) {
                toggleMenuOff();
            }
        };
    }
    // Позиционирование контекстного меню
    //-----------------------------------
    var menuPosition; // {x , y}
    var menuPositionX;
    var menuPositionY;
    var menuWidth; // Размеры 
    var menuHeight; // контекстного меню.
    var windowWidth;
    var windowHeight;
    var clickCoords;
    var clickCoordsX;
    var clickCoordsY;
    // Закрываем контекстное меню при изменении размера окна.
    function resizeListener() {
        window.onresize = function (e) {
            toggleMenuOff();
        };
    }
    function positionMenu(e) {
        clickCoords = getPosition(e);
        clickCoordsX = clickCoords.x;
        clickCoordsY = clickCoords.y;
        menuWidth = menu.offsetWidth + 4;
        menuHeight = menu.offsetHeight + 4;
        windowWidth = window.innerWidth;
        windowHeight = window.innerHeight;
        menuPosition = getPosition(e);
        console.log(menuPosition);
        if ((windowWidth - clickCoordsX) < menuWidth) {
            menu.style.left = windowWidth - menuWidth + "px";
        }
        else {
            menu.style.left = clickCoordsX + "px";
        }
        if ((windowHeight - clickCoordsY) < menuHeight) {
            menu.style.top = windowHeight - menuHeight + "px";
        }
        else {
            menu.style.top = clickCoordsY + "px";
        }
    }
    // Определяем позицию клика.
    function getPosition(e) {
        var posx = 0;
        var posy = 0;
        var me;
        me = (!e) ? window.event : me = e;
        if (me.pageX || me.pageY) {
            posx = me.pageX;
            posy = me.pageY;
        }
        else if (me.clientX || me.clientY) {
            posx = me.clientX + document.body.scrollLeft +
                document.documentElement.scrollLeft;
            posy = me.clientY + document.body.scrollTop +
                document.documentElement.scrollTop;
        }
        return {
            x: posx,
            y: posy
        };
    }
})();
//# sourceMappingURL=HandlerTS.js.map