//--------------------------------
//DEFINIR VARIÁVEIS DESSA SESSÃO PARA PAGINAÇÃO
var referenciaAncora;
var referenciaPaginacao;
var qtdPerPage;
var contexto;
//--------------------------------

var pageAtivo;
var $alvo;
var qtdAlvo;
var $paginacao;
var $setaAnterior;
var $setaProximo;
var maxPage;


function definirPaginacao(nomeContexto) {
    pageAtivo = 1;
    $alvo = $(referenciaPaginacao + ' tr');
    qtdAlvo = $alvo.length;

    if (nomeContexto !== undefined)
        $paginacao = $('.' + nomeContexto);
    else
        $paginacao = $('.pagination');

    $setaAnterior = $paginacao.find('.btn-prev');
    $setaProximo = $paginacao.find('.btn-next');
    maxPage = Math.ceil(qtdAlvo / qtdPerPage);

    if (nomeContexto !== undefined)
        SalvarContexto(nomeContexto);
}

function showPage(page, element) {

    thisPage = page;

    if (typeof element !== undefined)
        CarregarContexto(element);

    pageAtivo = parseInt(thisPage);
    $alvo.hide();

    $alvo.slice((pageAtivo - 1) * qtdPerPage, pageAtivo * qtdPerPage).css('display', 'table-row');
    $paginacao.find('a').removeClass('selected');
    $paginacao.find('a').eq(pageAtivo).addClass('selected');

    $setaAnterior.removeClass("disabled");
    $setaProximo.removeClass("disabled");

    if (pageAtivo == 1) {
        $setaAnterior.addClass("disabled");
    }
    else if (pageAtivo == maxPage) {
        $setaProximo.addClass("disabled");
    }

    if (element !== undefined)
        SalvarContexto(element);
}

function paginacaoPadrao(codigoCultura, element) {

    if (typeof element !== undefined)
        CarregarContexto(element)

    var _anterior = '';
    var _proximo = '';
    if (codigoCultura == 'en-us') {
        _anterior = 'Previous';
        _proximo = 'Next';
    }
    else {
        _anterior = 'Anterior';
        _proximo = 'Pr\u00F3ximo';
    }

    $paginacao.find(".break").append('<a title="' + _anterior + '" class="btn btn-out btn-prev disabled">' + _anterior + '</a>');
    for (var i = 1; i <= maxPage ; i++) {
        if (i == 1) {
            $paginacao.find('.break').append('<a id="row-' + i + '" class="pager selected">' + i + '</a>');
        }
        else {
            $paginacao.find('.break').append('<a id="row-' + i + '" class="pager">' + i + '</a>');
        }
    }
    $paginacao.find(".break").append('<a title="' + _proximo + '" class="btn btn-out btn-next">' + _proximo + '</a>');

    $setaAnterior = $paginacao.find('.btn-prev');
    $setaProximo = $paginacao.find('.btn-next');
    $paginacao.find('a.pager').on('click', function (e) {
        e.preventDefault();
        showPage($(this).text(), element);
    });

    $setaAnterior.click(function (e) {
        if (typeof element !== undefined) {
            pageAtivo = contexto[element].pageAtivo;
            maxPage = contexto[element].maxPage;
        }

        e.preventDefault();
        if (pageAtivo > 1) {
            showPage(pageAtivo - 1, element);
        }
    });

    $setaProximo.click(function (e) {
        if (typeof element !== undefined) {
            pageAtivo = contexto[element].pageAtivo;
            maxPage = contexto[element].maxPage;
        }

        e.preventDefault();
        if (pageAtivo < maxPage) {
            showPage(pageAtivo + 1, element);
        }
    });

    showPage(1, element);
}

function SalvarContexto(nomeContexto) {
    contexto[nomeContexto] = {
        alvo: $alvo,
        qtdAlvo: qtdAlvo,
        paginacao: $paginacao,
        setaAnterior: $setaAnterior,
        setaProximo: $setaProximo,
        maxPage: maxPage,
        pageAtivo: pageAtivo
    }
}

function CarregarContexto(nomeContexto) {
    if (nomeContexto !== undefined) {
        referenciaAncora = contexto[nomeContexto].referenciaAncora;
        referenciaPaginacao = contexto[nomeContexto].referenciaPaginacao;
        pageAtivo = contexto[nomeContexto].pageAtivo;
        $alvo = contexto[nomeContexto].alvo;
        qtdAlvo = contexto[nomeContexto].qtdAlvo;
        $paginacao = $('.' + nomeContexto);
        $setaAnterior = $paginacao.find('.btn-prev');
        $setaProximo = $paginacao.find('.btn-next');
        maxPage = contexto[nomeContexto].maxPage;
        referenciaAncora = contexto[nomeContexto].referenciaAncora;
        referenciaPaginacao = contexto[nomeContexto].referenciaPaginacao;
    }
}