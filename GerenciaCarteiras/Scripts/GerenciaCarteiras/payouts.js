var s = new Array();
var qtd = 0;
var datat;


function allPayOuts() {
    //do {
    //    $.post("https://bitminer.io/payouts", { task: "morepayouts", offset: qtd },
    //                function (data) {
    //                    datat = data;
    //                    if (data) { qtd += 100; s.push(data); }
    //                });
    //} while (datat);
    var listaPaginasResponse;
    chamadaAjaxPostSyncrona(urlGetPayouts, { offset: 0 },
        function (data) {
            if (data.sucesso) {
                listaPaginasResponse = data.paginas;
            }
        }, null, false);
}


$(document).ready(function () {
    $('#btnRetirar').on('click', function () {
        allPayOuts();
    });
});
