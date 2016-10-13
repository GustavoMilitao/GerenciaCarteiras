var timer = new Array();

$(document).ready(function () {

    referenciaAncora = '#div-grid-lista-carteiras';
    referenciaPaginacao = '#div-grid-lista-carteiras tbody';
    qtdPerPage = 20;

    programarAtualizacaoSaldoBitMiner();

    definirPaginacao("paginacao-carteiras");
    contexto["paginacao-carteiras"].paginacao.find(".break").empty();

    if ($alvo.length > qtdPerPage) {
        paginacaoPadrao('pt-br', 'paginacao-carteiras');
    }

    $('#formDefault').validate({
        submitHandler: function () {
            sair();
        }
    });
});


function sair() {
    chamadaAjaxPost(urlSair,
                {},
                function (data) {
                    if (data.sucesso) {
                        desativarProgramacaoAtualizacaoSaldoBitMiner();
                        redirecionarParaPagina(urlLoginHome, null);
                    }
                    else {
                        alert("Ocorreu um erro ao tentar fazer logout.");
                    }
                }, null, true);
}

function programarAtualizacaoSaldoBitMiner() {
    var listaCarteiras = JSON.parse($('#listaCarteiras').val());
    $.each(listaCarteiras, function (i, elemento) {
        chamadaAjaxPost(urlListarEnderecos, {
            accountId: elemento.id
        },
            function (data) {
                elemento.Addresses = data.listaEnderecos;
                if (elemento.Addresses.length > 0) {
                    timer.push(setInterval(function () {
                        chamadaAjaxPost(urlResponseAddress, {
                            address: elemento.Addresses[0].address
                        }, function (data) {
                            $('#valorBitMiner' + elemento.id).text($(data.response).find('#btnform b').text());
                        }, null, true)
                    }, 5000));
                }
            }, null, true);
    });
}

function desativarProgramacaoAtualizacaoSaldoBitMiner() {
    timer.forEach(function (elemento) {
        clearInterval(elemento);
    });
}