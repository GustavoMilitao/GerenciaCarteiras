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


function chamadaAjaxPost(url, parametros, callbackSucesso, callbackErro, exibirCarregando) {
    $.ajax({
        type: "POST",
        url: url,
        data: parametros,
        dataType: "json",
        traditional: true,
        success: function (args) {
                callbackSucesso(args);
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

function listarTodosOsEnderecos() {
    var listaCarteiras = JSON.parse($('#listaCarteiras').val());
    listaCarteiras.forEach(function (elemento, i) {
        document.getElementById('processandoConta').innerHTML = "Processando conta " + (i + 1) + " de " + listaCarteiras.length;
        chamadaAjaxPostSyncrona(urlListarEnderecos, {
            accountId: elemento.id
        },
            function (data) {
                elemento.Addresses = data.listaEnderecos;
            }, null, false)
    });
    document.getElementById('processandoConta').innerHTML = "";
    return JSON.stringify(listaCarteiras).replace(/"/g, '\'');
}

function download(data, filename, type) {
    var a = document.createElement("a"),
        file = new Blob([data], { type: type });
    if (window.navigator.msSaveOrOpenBlob) // IE10+
        window.navigator.msSaveOrOpenBlob(file, filename);
    else { // Others
        var url = URL.createObjectURL(file);
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        setTimeout(function () {
            document.body.removeChild(a);
            window.URL.revokeObjectURL(url);
        }, 0);
    }
}