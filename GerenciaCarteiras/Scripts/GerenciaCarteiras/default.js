var timer = new Array();

$(document).ready(function () {

    referenciaAncora = '#div-grid-lista-carteiras';
    referenciaPaginacao = '#div-grid-lista-carteiras tbody';
    qtdPerPage = 20;

    definirPaginacao("paginacao-carteiras");
    contexto["paginacao-carteiras"].paginacao.find(".break").empty();

    if ($alvo.length > qtdPerPage) {
        paginacaoPadrao('pt-br', 'paginacao-carteiras');
    }

    listarTodosOsEnderecos();
    atualizarSaldoBitMiner();

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


function atualizarSaldoBitMiner() {
    var listaCarteiras = JSON.parse($('#listaCarteiras').val());
    listaCarteiras.forEach(function (elemento) {
        timer.push(setTimeout(function () {
            chamadaAjaxPost(urlResponseAddress, {
                address: elemento.Addresses[0].address
            }, function (data) {
                $('#valorBitMiner' + elemento.id).text($(data.response).find('#btnform b').text());
            }, null, true)
        }, 2000));
    });
    atualizarSaldoBitMiner();
}

function desativarProgramacaoAtualizacaoSaldoBitMiner() {
    timer.forEach(function (elemento) {
        clearTimeout(elemento);
    });
}

function retirar(){
$('#widtherr').text('Wait...');
$('.deposit').hide();
$.post("",{"task":"withdraw","amount":$('#widthsum').val()},
function(data) {
    if(data==1){
        $('#widtherr').html('Withdraw confirmed and now pending.<br>Wait for processing.');
        setTimeout(function(){window.location.reload();},2000);
    }else{
        $('#widtherr').text(data);
    }
    $('.deposit').show();
});
}