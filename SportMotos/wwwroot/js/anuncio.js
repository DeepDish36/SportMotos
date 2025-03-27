//JS para a página Anuncio.cshtml

$(function () {
    $('#marcaDropdown').on('change', function () {
        var marcaSelecionada = $(this).val();
        if (marcaSelecionada) {
            $('#modeloDropdown').prop('disabled', false);
            // Carregar modelos com base na marca selecionada
            // Exemplo: $('#modeloDropdown').html('<option value="Modelo1">Modelo1</option><option value="Modelo2">Modelo2</option>');
        } else {
            $('#modeloDropdown').prop('disabled', true).val('');
        }
    });
});

function filterAnuncios() {
    var marca = $('#marcaDropdown').val();
    var modelo = $('#modeloDropdown').val();
    var preco = $('#precoDropdown').val();
    var genero = $('#generoDropdown').val();
    var cor = $('#corDropdown').val();

    $('.anuncio-card').each(function () {
        var card = $(this);
        var cardMarca = card.data('marca');
        var cardModelo = card.data('modelo');
        var cardPreco = card.data('preco');
        var cardGenero = card.data('genero');
        var cardCor = card.data('cor');

        var showCard = true;

        if (marca && cardMarca !== marca) {
            showCard = false;
        }
        if (modelo && cardModelo !== modelo) {
            showCard = false;
        }
        if (preco) {
            var precoRange = preco.split('-');
            var precoMin = parseFloat(precoRange[0]);
            var precoMax = precoRange[1] ? parseFloat(precoRange[1]) : Infinity;
            if (cardPreco < precoMin || cardPreco > precoMax) {
                showCard = false;
            }
        }
        if (genero && cardGenero !== genero) {
            showCard = false;
        }
        if (cor && cardCor !== cor) {
            showCard = false;
        }

        if (showCard) {
            card.show();
        } else {
            card.hide();
        }
    });
}

function sortAnuncios() {
    var sortBy = $('#ordenarPorDropdown').val();
    var anunciosContainer = $('#anunciosContainer');
    var anuncios = $('.anuncio-card').get();

    anuncios.sort(function (a, b) {
        var aData = $(a).data();
        var bData = $(b).data();

        if (sortBy === 'precoAsc') {
            return aData.preco - bData.preco;
        } else if (sortBy === 'precoDesc') {
            return bData.preco - aData.preco;
        } else if (sortBy === 'data') {
            return new Date(bData.dataPublicacao) - new Date(aData.dataPublicacao);
        } else {
            return 0;
        }
    });

    $.each(anuncios, function (index, anuncio) {
        anunciosContainer.append(anuncio);
    });
}