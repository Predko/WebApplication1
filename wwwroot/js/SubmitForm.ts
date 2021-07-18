
// Access the form element...
const form = <HTMLFormElement>document.getElementById("formId");

// ...and take over its submit event.
form.addEventListener("submit", function (event)
{
    event.preventDefault();

    submitForm();
});

function submitForm()
{
    const XHR = new XMLHttpRequest();

    // Bind the FormData object and the form element
    const FD = new FormData(form);

    // Define what happens on successful data submission
    XHR.addEventListener("load", function (event)
    {
        alert(XHR.response);
    });

    // Define what happens in case of error
    XHR.addEventListener("error", function (event)
    {
        alert('Oops! Something went wrong.');
    });

    // Set up our request
    XHR.open("POST", "/customers/edit/submit");

    // The data sent is what the user provided in the form
    XHR.send(FD);
}

// XHR.onprogress = function (event)
    // {
    //     if (event.lengthComputable)
    //     {
    //         alert(`Получено ${event.loaded} из ${event.total} байт`);
    //     } else
    //     {
    //         alert(`Получено ${event.loaded} байт`); // если в ответе нет заголовка Content-Length
    //     }
    // }

//     // 1. Создаём новый XMLHttpRequest-объект
// let xhr = new XMLHttpRequest();

// // 2. Настраиваем его: POST-запрос по URL /article/.../load
// xhr.open('POST', '/customers/edit/submit');

// // 3. Отсылаем запрос
// xhr.send();

// // 4. Этот код сработает после того, как мы получим ответ сервера
// xhr.onload = function() {
//   if (xhr.status != 200) { // анализируем HTTP-статус ответа, если статус не 200, то произошла ошибка
//     alert(`Ошибка ${xhr.status}: ${xhr.statusText}`); // Например, 404: Not Found
//   } else { // если всё прошло гладко, выводим результат
//     alert(`Готово, получили ${xhr.response.length} байт`); // response -- это ответ сервера
//   }
// };

// xhr.onprogress = function(event) {
//   if (event.lengthComputable) {
//     alert(`Получено ${event.loaded} из ${event.total} байт`);
//   } else {
//     alert(`Получено ${event.loaded} байт`); // если в ответе нет заголовка Content-Length
//   }

// };

// xhr.onerror = function() {
//   alert("Запрос не удался");
// };

