"use strict";
function loadMainContent(url) {
    var mainContainer = document.querySelector('#main-container');
    var customerId = mainContainer.getAttribute("data-customer-id");
    var formData = new FormData();
    formData.append('customer', customerId);
    fetch(url, {
        method: 'POST',
        //mode: 'cors', // no-cors, *cors, same-origin
        //cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
        //credentials: 'same-origin', // include, *same-origin, omit
        headers: {
            // 'Content-Type': 'application/json'
            'Content-Type': 'application/x-www-form-urlencoded',
        },
        //redirect: 'follow', // manual, *follow, error
        //referrerPolicy: 'no-referrer', // no-referrer, *client
        body: formData //`{customer: ${customerId}}` // body data type must match "Content-Type" header
    })
        .then(function (response) {
        if (response.status === 200) {
            response.text()
                .then(function (text) {
                mainContainer.innerHTML = text;
                scriptsInit();
                resizebody();
            });
        }
        else {
            throw new Error('Что-то пошло не так на API сервере.');
        }
    })
        .then(function (response) {
        console.debug(response);
        // ...
    }).catch(function (error) {
        console.error(error);
    });
}
//# sourceMappingURL=loadMainContent.js.map