$(document).ready(function () {
    //var listaCarteirasStr = listarTodosOsEnderecos();
    $('#btnRetirar').removeAttr('disabled');
    $('#btnListar').removeAttr('disabled');

    $('#btnSalvarEnderecos').on('click', function () {
        $('#btnRetirar').attr('disabled', 'disabled');
        $('#btnListar').attr('disabled', 'disabled');
        if ($('#inputCaminhoArquivo').val() != "") {
            chamadaAjaxPostSyncrona(urlSalvarEnderecos,
                {
                    listaCarteiras: $('#listaCarteiras').val(),
                    caminhoArquivo: $('#inputCaminhoArquivo').val()
                },
                function (data) {
                    if (data.sucesso) {
                        //alert("Foi retirado 0.005 BTC de todas as contas e o processamento está pendente.");
                        alert("O arquivo com os endereços foi salvo com sucesso em : " + $('#inputCaminhoArquivo').val());
                    }
                    else {
                        alert("Ocorreu um erro : " + data.mensagem);
                    }
                }, null, false);
        }
        else {
            alert("Digite um caminho");
        }
        $('#btnRetirar').removeAttr('disabled');
        $('#btnListar').removeAttr('disabled');
    });

    $('#btnRetirar').on('click', function () {
        $('#btnRetirar').attr('disabled', 'disabled');
        $('#btnListar').attr('disabled', 'disabled');
        if ($('#caminhoArquivoLer').val() != "") {
            chamadaAjaxPostSyncrona(urlRetirar,
                { enderecoArquivo: $('#caminhoArquivoLer').val() },
                function (data) {
                    if (data.sucesso) {
                        alert("Foi retirado 0.005 BTC de todas as contas e o processamento está pendente.");
                    }
                    else {
                        alert("Ocorreu um erro : " + data.mensagem);
                    }
                }, null, false);
        }
        else {
            alert("Digite um caminho");
        }
        $('#btnRetirar').removeAttr('disabled');
        $('#btnListar').removeAttr('disabled');
    });
});