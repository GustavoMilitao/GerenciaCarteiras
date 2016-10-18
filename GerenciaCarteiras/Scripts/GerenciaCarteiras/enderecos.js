$(document).ready(function () {
    $('#btnCriarEnderecos').on('click', function () {
        var listaCarteiras = JSON.parse($('#listaCarteiras').val());
        var primeiraCarteiraBTC;
        for (var i = 0; i < listaCarteiras.length; i++){
            if(listaCarteiras[i].currency == 'BTC'){
                primeiraCarteiraBTC = listaCarteiras[i];
                break;
            }
        }
        chamadaAjaxPost(urlCriarEnderecos,
            {
                idCarteira: primeiraCarteiraBTC.id,
                enderecoArquivo: $('#inputCaminhoArquivo').val(),
                quantidadeEnderecos: $('#qtdEnderecos').val()
            }, function (data) {
                if(data.sucesso){
                    alert("Endereços criados com sucesso e arquivo salvo em : " + $('#inputCaminhoArquivo').val());
                }
                else{
                    alert(data.mensagem);
                }
            }, null, false);
    });
});
