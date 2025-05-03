document.addEventListener("DOMContentLoaded", function () {
    const anunciosDropdown = document.getElementById("IdMoto");

    if (anunciosDropdown) {
        const marcaInput = document.getElementById("Marca");
        const modeloInput = document.getElementById("Modelo");
        const precoInput = document.getElementById("Preco");
        const precoHiddenInput = document.getElementById("PrecoHidden");
        const condicaoDropdown = document.getElementById("Condicao");
        const condicaoHiddenInput = document.getElementById("CondicaoHidden");
        const tituloInput = document.getElementById("Titulo");
        const descricaoTextarea = document.getElementById("Descricao");

        anunciosDropdown.addEventListener("change", function () {
            const selectedAnuncioId = this.value;

            if (selectedAnuncioId) {
                fetch(`/CriarAnuncio/GetAnuncioMotoDetails?id=${selectedAnuncioId}`)
                    .then(response => response.json())
                    .then(data => {
                        tituloInput.value = data.titulo;
                        descricaoTextarea.value = data.descricao;
                        marcaInput.value = data.marca;
                        modeloInput.value = data.modelo;
                        precoInput.value = data.preco;
                        precoHiddenInput.value = data.preco;
                        condicaoDropdown.value = data.condicao;
                        condicaoHiddenInput.value = data.condicao;
                    })
                    .catch(error => console.error('Erro ao buscar detalhes do anúncio de moto:', error));
            } else {
                tituloInput.value = "";
                descricaoTextarea.value = "";
                marcaInput.value = "";
                modeloInput.value = "";
                precoInput.value = "";
                precoHiddenInput.value = "";
                condicaoDropdown.value = "";
                condicaoHiddenInput.value = "";
            }
        });
    } else {
        console.log("✅ O script de dropdown foi ignorado porque não existe dropdown na página!");
    }
});
