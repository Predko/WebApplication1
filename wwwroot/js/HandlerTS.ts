"use strict";

// Решение позаимствовано из:
// https://coderoad.ru/14267781/Сортировка-таблицы-HTML-с-JavaScript#49041392
// и
// из: https://coderoad.ru/14267781/Сортировка-таблицы-HTML-с-JavaScript#53880407

// Sort table.
function getCellValue(row: HTMLElement, indexTh: number) {
    return (row.children[indexTh] as HTMLElement).innerText || row.children[indexTh].textContent;
}

function getComparer(indexTh: number, asc: boolean) {
    function compareCells(v1, v2) {
        if (v1 !== '' && v2 !== '' && !isNaN(v1) && !isNaN(v2)) {
            return v1 - v2;
        }

        return v1.toString().localeCompare(v2);
    }

    let comparer = function (a: HTMLTableRowElement, b: HTMLTableRowElement) {
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
function sortOrderFromTh(th: HTMLTableCellElement) {
    let sortOrder;
    let resSortOrder;

    let indexTh: number = Array.prototype.slice.call(th.parentNode.children).indexOf(th);

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
    .forEach(th => th.addEventListener('click',
        (() => {
            let sortOrder = sortOrderFromTh(th);
            if (sortOrder === undefined) {
                return;
            }

            const indexTh: number = Array.prototype.slice.call(th.parentNode.children).indexOf(th);
            const table = th.closest('table');
            const tbody = table.querySelector('tbody');
            Array.prototype.slice.call(tbody.querySelectorAll('tr'))
                .sort(getComparer(indexTh, sortOrder))
                .forEach(tr => tbody.appendChild(tr));

            document.getElementById("divTable").scrollTop = 0;
        }
        )));

addEventListener("beforeunload", () => {
    // Save current scroll position.
    sessionStorage.setItem("currentScrollYPosition", String(document.getElementById("divTable").scrollTop));
}, false);

function restoreScrollPosition() {
    // Restore scroll position.
    let scrollTop: number = Number(sessionStorage.getItem("currentScrollYPosition")),
        divScrollContainer: HTMLElement = document.getElementById("divTable") as HTMLElement;

    let indexTh: number = Number(sessionStorage.getItem("Sort.ThIndex"));

    if (indexTh === undefined || indexTh === null) {
        indexTh = 0;
        sessionStorage.setItem("Sort.ThIndex", String(indexTh));
        sessionStorage.setItem("Sort.Order.Th" + indexTh, String(true));
    }

    let so: string = sessionStorage.getItem("Sort.Order.Th" + indexTh);
    let sortOrder: boolean = (so === undefined) ? undefined : so === 'true';

    SortTable(sortOrder, divScrollContainer, indexTh);

    if (scrollTop && divScrollContainer) {
        divScrollContainer.scrollTop = scrollTop;
    }
}


function SortTable(sortOrder: boolean, divScrollContainer: HTMLElement, indexTh) {
    if (sortOrder !== undefined) {
        const table: HTMLTableElement = divScrollContainer.querySelector('table');
        const tbody = table.querySelector('tbody');
        Array.prototype.slice.call(tbody.querySelectorAll('tr'))
            .sort(getComparer(indexTh, sortOrder))
            .forEach(tr => tbody.appendChild(tr));
    }
}

document.querySelector('table').onclick = (event) => {
    let cell: HTMLTableCellElement = event.target as HTMLTableCellElement;
    if (cell.tagName.toLowerCase() != 'td')
        return;
    let i: number = (cell.parentNode as HTMLTableRowElement).rowIndex;
    let j: number = cell.cellIndex;
    let currentTr: HTMLTableRowElement = cell.parentNode as HTMLTableRowElement;
    window.location.href = "customers/edit?customer=" + currentTr.id;
}

function resizebody() {
    let container = document.getElementById("divTable");
    let titleTable = document.getElementById("titleTable");

    let w = window,
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


// Добавляет контекстное меню к строке таблицы.

const menu:HTMLDivElement = document.querySelector("#context-menu");
const menuItems = menu.querySelectorAll(".context-menu__item");

let menuState = 0;

const taskItemClassName = "task";
let taskItemInContext;

const contextMenuClassName = "context-menu";
const contextMenuItemClassName = "context-menu__item";
const contextMenuLinkClassName = "context-menu__link";
const contextMenuActive = "context-menu--active";

(function ()
{
    init();

    // Инициализация.
    function init()
    {
        contextListener();
        clickListener();
        keyupListener();
    }

    // Обработчик события "contextmenu".
    // При правом клике на строке таблицы, отображает контекстное меню.
    // Если правый клик не на строке таблицы - скрывает.
    function contextListener()
    {
        document.addEventListener("contextmenu", function (e)
        {
            taskItemInContext = clickInsideElement(e, taskItemClassName);

            if (taskItemInContext)
            {
                e.preventDefault();
                toggleMenuOn();

                positionMenu(e);
            }
            else
            {
                taskItemInContext = null;
                toggleMenuOff();
            }
        });
    }

    // Проверяет, соответствует ли элемент(в аргументе события) указанному в переменной className классу.
    // Если да - возвращает элемент, если нет - false.
    function clickInsideElement(e, className)
    {
        let el = e.srcElement || e.target;

        if (el.classList.contains(className))
        {
            return el;
        }
        else
        {
            while (el = el.parentNode)
            {
                if (el.classList && el.classList.contains(className))
                {
                    return el;
                }
            }
        }

        return false;
    }

    // Показывает меню.
    function toggleMenuOn()
    {
        if (menuState !== 1)
        {
            menuState = 1;
            menu.classList.add(contextMenuActive);
        }
    }

    // Скрывает меню.
    function toggleMenuOff()
    {
        if (menuState !== 0)
        {
            menuState = 0;
            menu.classList.remove(contextMenuActive);
        }
    }

    // Обработчик клика мыши.
    // Если нажата левая кнопка не на контекстном меню - скрывает меню.
    function clickListener()
    {
        const leftBtn = 0;

        document.addEventListener("click", function (e)
        {
            const clickeElIsLink = clickInsideElement(e, contextMenuLinkClassName);

            if (clickeElIsLink)
            {
                e.preventDefault();
                menuItemListener(clickeElIsLink);
            }
            else
            {
                const button = e.which || e.button;
                if (button === 1)
                {
                    toggleMenuOff();
                }
            }
        });
    }

    function menuItemListener(link)
    {
        const currentTr: HTMLTableRowElement = taskItemInContext as HTMLTableRowElement;
        const action = "customers/" + link.getAttribute("data-action") + "?" + "customer=" + currentTr.id;

        window.location.href = action;

        console.log("Task ID - " + currentTr.id + "Task action - " + link.getAttribute("data-action"));

        toggleMenuOff();
    }

    // Обработчик нажатия клавиши на клавиатуре.
    // Если нажата клавиша ESC - скрывает меню.
    function keyupListener()
    {
        const ESC = 27;
        window.onkeyup = function (e)
        {
            if (e.keyCode === ESC)
            {
                toggleMenuOff();
            }
        }
    }

    // Позиционирование контекстного меню
    //-----------------------------------

    let menuPosition;   // {x , y}

    let menuPositionX: string;
    let menuPositionY: string;

    let menuWidth;      // Размеры 
    let menuHeight;     // контекстного меню.

    let windowWidth;
    let windowHeight;


    let clickCoords;
    let clickCoordsX;
    let clickCoordsY;


    // Закрываем контекстное меню при изменении размера окна.
    function resizeListener()
    {
        window.onresize = function (e)
        {
            toggleMenuOff();
        };
    }

    function positionMenu(e)
    {
        clickCoords = getPosition(e);
        clickCoordsX = clickCoords.x;
        clickCoordsY = clickCoords.y;

        menuWidth = menu.offsetWidth + 4;
        menuHeight = menu.offsetHeight + 4;

        windowWidth = window.innerWidth;
        windowHeight = window.innerHeight;

        menuPosition = getPosition(e);
        console.log(menuPosition);

        if ((windowWidth - clickCoordsX) < menuWidth)
        {
            menu.style.left = windowWidth - menuWidth + "px";
        }
        else
        {
            menu.style.left = clickCoordsX + "px";
        }

        if ((windowHeight - clickCoordsY) < menuHeight)
        {
            menu.style.top = windowHeight - menuHeight + "px";
        }
        else
        {
            menu.style.top = clickCoordsY + "px";
        }
    }

    // Определяем позицию клика.
    function getPosition(e: MouseEvent) {
        let posx = 0;
        let posy = 0;

        let me: MouseEvent;

        me = (!e) ? window.event as MouseEvent : me = e;

        if (me.pageX || me.pageY)
        {
            posx = me.pageX;
            posy = me.pageY;
        }
        else
        if (me.clientX || me.clientY)
        {
            posx = me.clientX + document.body.scrollLeft +
                document.documentElement.scrollLeft;
            posy = me.clientY + document.body.scrollTop +
                document.documentElement.scrollTop;
        }

        return {
            x: posx,
            y: posy
        }
    }
})();
