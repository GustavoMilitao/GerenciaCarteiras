$(document).ready(function () {
    var listaCarteirasStr = $('#listaCarteiras').val();
    listarTodosOsEnderecos();
    $('#btnRetirar').removeAttr('disabled');
    $('#btnListar').removeAttr('disabled');
    $('#btnRetirar').on('click', function () {
        chamadaAjaxPost(urlRetirar,
            { listaCarteirasString: listaCarteirasStr },
            function (data) {
                if (data.sucesso) {
                    alert("Foi retirado 0.005 BTC de todas as contas e o processamento está pendente.");
                }
                else{
                    alert("Ocorreu um erro : " + data.mensagem);
                }
            }, null, false);
    })
});