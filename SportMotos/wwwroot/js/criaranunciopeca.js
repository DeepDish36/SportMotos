document.addEventListener("DOMContentLoaded", function () {
    const pecasDropdown = document.getElementById("IdPeca");
    const marcaInput = document.getElementById("Marca");
    const modeloInput = document.getElementById("Modelo");
    const categoriaInput = document.getElementById("Categoria");
    const precoInput = document.getElementById("Preco");
    const precoHiddenInput = document.getElementById("PrecoHidden");
    const condicaoDropdown = document.getElementById("Condicao");
    const condicaoHiddenInput = document.getElementById("CondicaoHidden");

    pecasDropdown.addEventListener("change", function () {
        const selectedPecaId = this.value;

        if (selectedPecaId) {
            fetch(`/CriarAnuncio/GetPecaDetails?id=${selectedPecaId}`)
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
                .catch(error => console.error('Erro ao buscar detalhes da peça:', error));
        } else {
            marcaInput.value = "";
            modeloInput.value = "";
            categoriaInput.value = "";
            precoInput.value = "";
            precoHiddenInput.value = "";
            condicaoDropdown.value = "";
            condicaoHiddenInput.value = "";
        }
    });
});
