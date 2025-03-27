//Código para preencher os campos de Marca, Modelo, Preço e Condição ao selecionar uma moto no formulário de criação de anúncio

document.getElementById("IdMoto").addEventListener("change", function () {
    var motoId = this.value;
    if (motoId) {
        fetch(`/CriarAnuncio/GetMotoDetails/${motoId}`)
            .then(response => response.json())
            .then(data => {
                document.getElementById("Marca").value = data.marca;
                document.getElementById("Modelo").value = data.modelo;
                document.getElementById("Preco").value = data.preco;
                document.getElementById("PrecoHidden").value = data.preco || "0"; // Se estiver vazio, coloca "0"
                document.getElementById("Condicao").value = data.condicao;
                document.getElementById("CondicaoHidden").value = data.condicao;
            });
    } else {
        document.getElementById("Marca").value = "";
        document.getElementById("Modelo").value = "";
        document.getElementById("Preco").value = "";
        document.getElementById("PrecoHidden").value = "0"; // Evita erro de campo vazio
        document.getElementById("Condicao").value = "";
        document.getElementById("CondicaoHidden").value = "";
    }
});