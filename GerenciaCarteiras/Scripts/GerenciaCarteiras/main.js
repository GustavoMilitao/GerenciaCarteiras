function redirecionarParaPagina(urlPaginaDestino, dados) {
    var form = $('#formRedirecionamento');

    if (dados != undefined) {
        for (i = 0; i < dados.length; i++) {
            form.append('<input hidden class="input-hidden" name="' + dados[i].nome + '" value="' + dados[i].valor + '" />');
        }
    }

    form.attr('action', urlPaginaDestino);

    form.submit();
}


function chamadaAjax(url, parametros, callbackSucesso, callbackErro, exibirCarregando) {
    $.ajax({
        type: "POST",
        url: url,
        data: parametros,
        dataType: "json",
        traditional: true,
        success: function (args) {
            if (args.expirou) {
                exibirModalInformativo("Sua sessão expirou. Você será redirecionado para a tela de login", function () { location = args.urlSessaoExpirada; });
            }
            else {
                callbackSucesso(args);
            }
        },
        beforeSend: function () {
            if (exibirCarregando == undefined)
                ExibirModalCarregando();
        },
        complete: function () {
            if (exibirCarregando == undefined)
                EsconderModalCarregando();
        },
        error: function (reqObj, tipoErro, mensagemErro) {
            var jsonValue = { MensagemDeErro: 'Ocorreu um erro ao executar a ação.' };
            try {
                jsonValue = jQuery.parseJSON(reqObj.responseText);
            }
            catch (excecao) {
            }
            exibirModalInformativo(jsonValue.MensagemDeErro);
            if (reqObj.state() != 'rejected') {
                if (callbackErro) {
                    callbackErro(tipoErro, mensagemErro);
                }
            }
        }
    });
}

function chamadaAjaxGetGatheringWalletAmmount(url, cookies, campoAtualizar) {
    //$.ajax({
    //    type: "GET",
    //    url: url,
    //    async: true,
    //    beforeSend: function (xhr) {
    //        if(cookies){
    //            xhr.setRequestHeader('Cookie', cookies);
    //        }
    //    },
    //    xhrFields: {
    //        withCredentials: (cookies != null && cookies && cookies != undefined)
    //    },
    //    success: function (response) {
    //        if (response.responseText && response.responseText != undefined) {
    //            var html = response.responseText();
    //            campoAtualizar.val($(html).find('#btnform b').val());
    //        }
    //    },

    //});
    var xhttp;
    if (window.XMLHttpRequest) {
        xhttp = new XMLHttpRequest();
    } else {
        // code for IE6, IE5
        xhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xhttp.withCredentials = true;
    xhttp.open("GET", url, true);
    xhttp.setRequestHeader("Cookie", cookies);
    xhttp.send();
    if (xhttp.responseText && xhttp.responseText != undefined) {
        var html = response.responseText();
        campoAtualizar.val($(html).find('#btnform b').val());
    }
}

function ExibirModalCarregando() {
    $('#modalCarregando').modal('show');
}

function EsconderModalCarregando() {
    $('#modalCarregando').modal('hide');
}

function exibirModalInformativo(mensagem, callbackClick) {
    $('#modalInformativo #mensagemInformativo').text(mensagem);
    $('#modalInformativo').modal({ backdrop: 'static', keyboard: false });
    $('#modalInformativo').modal('show');

    if (callbackClick != undefined) {
        $('#btnModalInformativo').click(callbackClick);
        $('#btnModalInformativoFechar').click(callbackClick);
    }
}