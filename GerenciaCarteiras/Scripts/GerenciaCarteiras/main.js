﻿function redirecionarParaPagina(urlPaginaDestino, dados) {
    var form = $('#formRedirecionamento');

    if (dados != undefined) {
        for (i = 0; i < dados.length; i++) {
            form.append('<input hidden class="input-hidden" name="' + dados[i].nome + '" value="' + dados[i].valor + '" />');
        }
    }

    form.attr('action', urlPaginaDestino);

    form.submit();
}


function chamadaAjaxPost(url, parametros, callbackSucesso, callbackErro, exibirCarregando) {
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

function chamadaAjaxGet(url, parametros, callbackSucesso, callbackErro, exibirCarregando) {
    $.ajax({
        type: "GET",
        url: url,
        data: parametros,
        crossDomain: true,
        dataType: 'jsonp',
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

function chamadaAjaxPostSyncrona(url, parametros, callbackSucesso, callbackErro, exibirCarregando) {
    $.ajax({
        type: "POST",
        url: url,
        data: parametros,
        dataType: "json",
        traditional: true,
        async: false,
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