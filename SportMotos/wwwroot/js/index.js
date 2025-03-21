document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("btnMotos").addEventListener("click", function () {
        document.getElementById("filtersMotos").style.display = "block";
        document.getElementById("filtersPecas").style.display = "none";
    });

    document.getElementById("btnPecas").addEventListener("click", function () {
        document.getElementById("filtersMotos").style.display = "none";
        document.getElementById("filtersPecas").style.display = "block";
    });

    // Carregar marcas, estilos, anos e quilómetros ao carregar a página
    fetch('/Moto/GetMarcas')
        .then(response => response.json())
        .then(data => {
            const marcaSelect = document.getElementById("marcaSelect");
            data.forEach(marca => {
                const option = document.createElement("option");
                option.value = marca;
                option.text = marca;
                marcaSelect.appendChild(option);
            });
        });

    fetch('/Moto/GetEstilos')
        .then(response => response.json())
        .then(data => {
            const estiloSelect = document.getElementById("estiloSelect");
            data.forEach(estilo => {
                const option = document.createElement("option");
                option.value = estilo;
                option.text = estilo;
                estiloSelect.appendChild(option);
            });
        });

    fetch('/Moto/GetAnos')
        .then(response => response.json())
        .then(data => {
            const anoDeSelect = document.getElementById("anoDeSelect");
            const anoAteSelect = document.getElementById("anoAteSelect");
            data.forEach(ano => {
                const optionDe = document.createElement("option");
                optionDe.value = ano;
                optionDe.text = ano;
                anoDeSelect.appendChild(optionDe);

                const optionAte = document.createElement("option");
                optionAte.value = ano;
                optionAte.text = ano;
                anoAteSelect.appendChild(optionAte);
            });
        });

    // Ativar select de modelos quando uma marca de moto for selecionada
    document.getElementById("marcaSelect").addEventListener("change", function () {
        const modeloSelect = document.getElementById("modeloSelect");
        modeloSelect.innerHTML = '<option disabled hidden selected>Modelo</option>'; // Limpar modelos anteriores
        if (this.value !== "Marca") {
            modeloSelect.disabled = false;
            fetch(`/Moto/GetModelos?marca=${this.value}`)
                .then(response => response.json())
                .then(data => {
                    data.forEach(modelo => {
                        const option = document.createElement("option");
                        option.value = modelo;
                        option.text = modelo;
                        modeloSelect.appendChild(option);
                    });
                });
        } else {
            modeloSelect.disabled = true;
        }
    });

    // Ativar select de modelos quando uma marca de peça for selecionada
    document.getElementById("pecaMarcaSelect").addEventListener("change", function () {
        const pecaModeloSelect = document.getElementById("pecaModeloSelect");
        pecaModeloSelect.innerHTML = '<option disabled hidden selected>Modelo</option>'; // Limpar modelos anteriores
        if (this.value !== "Marca") {
            pecaModeloSelect.disabled = false;
            fetch(`/Moto/GetModelosPecas?marca=${this.value}`)
                .then(response => response.json())
                .then(data => {
                    data.forEach(modelo => {
                        const option = document.createElement("option");
                        option.value = modelo;
                        option.text = modelo;
                        pecaModeloSelect.appendChild(option);
                    });
                });
        } else {
            pecaModeloSelect.disabled = true;
        }
    });

    // Função para buscar e exibir os anúncios filtrados
    function buscarAnuncios() {
        const estilo = document.getElementById("estiloSelect").value;
        const marca = document.getElementById("marcaSelect").value;
        const modelo = document.getElementById("modeloSelect").value;
        const precoDe = document.getElementById("precoDeSelect").value;
        const precoAte = document.getElementById("precoAteSelect").value;
        const anoDe = document.getElementById("anoDeSelect").value;
        const anoAte = document.getElementById("anoAteSelect").value;
        const combustivel = document.getElementById("combustivelSelect").value;
        const kmDe = document.getElementById("kmDeSelect").value;
        const kmAte = document.getElementById("kmAteSelect").value;

        const query = new URLSearchParams({
            estilo, marca, modelo, precoDe, precoAte, anoDe, anoAte, combustivel, kmDe, kmAte
        }).toString();

        fetch(`/Moto/GetAnuncios?${query}`)
            .then(response => response.json())
            .then(data => {
                const anunciosContainer = document.getElementById("anunciosContainer");
                anunciosContainer.innerHTML = ""; // Limpar anúncios anteriores
                data.forEach(anuncio => {
                    const anuncioDiv = document.createElement("div");
                    anuncioDiv.className = "anuncio";
                    anuncioDiv.innerHTML = `
                        <h3>${anuncio.titulo}</h3>
                        <p>Marca: ${anuncio.marca}</p>
                        <p>Modelo: ${anuncio.modelo}</p>
                        <p>Preço: €${anuncio.preco}</p>
                        <p>Ano: ${anuncio.ano}</p>
                        <p>Combustível: ${anuncio.combustivel}</p>
                        <p>Quilometragem: ${anuncio.quilometragem}</p>
                    `;
                    anunciosContainer.appendChild(anuncioDiv);
                });

                // Atualizar contagem de resultados
                document.getElementById("resultadosCount").innerText = `(${data.length} resultados)`;
            });
    }

    // Adicionar evento ao botão "Ver resultados"
    document.getElementById("btnVerResultados").addEventListener("click", buscarAnuncios);
});
