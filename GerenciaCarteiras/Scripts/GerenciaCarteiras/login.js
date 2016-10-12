function conectar() {
    var apiKeyVal = $('#apiKey').val();
    var apiSecretVal = $('#apiSecret').val();
    if (apiKeyVal && apiSecretVal) {
        chamadaAjax(urlLogin,
                    {
                        apiKey: apiKeyVal,
                        apiSecret: apiSecretVal
                    },
                    function (data) {
                        if (data.sucesso) {
                            redirecionarParaPaginaDefault(apiKeyVal, apiSecretVal, data.urlListar)
                        }
                        else {
                            alert("Dados da conta incorretos ou vazios");
                        }
                    }, null, true);
    }
}


function queryStringGrid(apiKey, apiSecret) {
    var params = {
        'apiKey': apiKey,
        'apiSecret': apiSecret,
    }
    var query = $.param(params);
    return query;
}


$(document).ready(function () {
    $('#formLogin').validate({
        rules: {
            chaveAPI: {
                required: true,
                minlength: 1
            },
            segredoAPI: {
                required: true,
                minlength: 1
            },
            messages: {
                chaveAPI: mensagemChaveAPIObrigatorio,
                segredoAPI: mensagemSegredoAPIObrigatorio,
            },
        },
        submitHandler: function () {
            conectar();
        }
    });
});

function redirecionarParaPaginaDefault(apiKey, apiSecret, urlListar) {
    var arrayObjeto = new Array();

    var objetoApiKey = { nome: "apiKey", valor: apiKey };
    var objetoApiSecret = { nome: "apiSecret", valor: apiSecret };

    arrayObjeto.push(objetoApiKey);
    arrayObjeto.push(objetoApiSecret);

    redirecionarParaPagina(urlListar, arrayObjeto);
}