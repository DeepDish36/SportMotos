document.addEventListener("DOMContentLoaded", function () {
    const anunciosDropdown = document.getElementById("IdPeca");

    if (anunciosDropdown) {
        const marcaInput = document.getElementById("Marca");
        const modeloInput = document.getElementById("Modelo");
        const categoriaInput = document.getElementById("Categoria");
        const precoInput = document.getElementById("Preco");
        const precoHiddenInput = document.getElementById("PrecoHidden");
        const condicaoDropdown = document.getElementById("Condicao");
        const condicaoHiddenInput = document.getElementById("CondicaoHidden");

        anunciosDropdown.addEventListener("change", function () {
            const selectedAnuncioId = this.value;

            if (selectedAnuncioId) {
                fetch(`/CriarAnuncio/GetAnuncioPecaDetails?id=${selectedAnuncioId}`)
                    .then(response => response.json())
                    .then(data => {
                        marcaInput.value = data.marca;
                        modeloInput.value = data.modelo;
                        categoriaInput.value = data.categoria;
                        precoInput.value = data.preco;
                        precoHiddenInput.value = data.preco;
                        condicaoDropdown.value = data.condicao;
                        condicaoHiddenInput.value = data.condicao;
                    })
                    .catch(error => console.error('Erro ao buscar detalhes do anúncio de peça:', error));
            } else {
                marcaInput.value = "";
                modeloInput.value = "";
                categoriaInput.value = "";
                precoInput.value = "";
                precoHiddenInput.value = "";
                condicaoDropdown.value = "";
                condicaoHidden.value = "";
            }
        });
    } else {
        console.log("O script de dropdown foi ignorado porque não existe dropdown na página!");
    }
});
