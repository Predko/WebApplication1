"use strict";
// Решение позаимствовано из:
// https://coderoad.ru/14267781/Сортировка-таблицы-HTML-с-JavaScript#49041392
// и
// из: https://coderoad.ru/14267781/Сортировка-таблицы-HTML-с-JavaScript#53880407
// Sort table.

// Access the form element...
var form = document.getElementById("formId");
// ...and take over its submit event.
form.addEventListener("submit", function (event) {
    event.preventDefault();
    submitForm();
});
function submitForm() {
    var XHR = new XMLHttpRequest();
    // Bind the FormData object and the form element
    var FD = new FormData(form);
    // Define what happens on successful data submission
    XHR.addEventListener("load", function (event) {
        alert(XHR.response);
    });
    // Define what happens in case of error
    XHR.addEventListener("error", function (event) {
        alert('Oops! Something went wrong.');
    });
    // XHR.onprogress = function (event) {
    //     if (event.lengthComputable) {
    //         alert("\u041F\u043E\u043B\u0443\u0447\u0435\u043D\u043E " + event.loaded + " \u0438\u0437 " + event.total + " \u0431\u0430\u0439\u0442");
    //     }
    //     else {
    //         alert("\u041F\u043E\u043B\u0443\u0447\u0435\u043D\u043E " + event.loaded + " \u0431\u0430\u0439\u0442"); // если в ответе нет заголовка Content-Length
    //     }
    // };
    // Set up our request
    XHR.open("POST", "/customers/edit/submit");

    XHR.responseType = "json";
    
    // The data sent is what the user provided in the form
    XHR.send(FD);
}
//# sourceMappingURL=HandlerTS.js.map