document.addEventListener("DOMContentLoaded", function () {
    console.log("DOM fully loaded!");

    const btnMotos = document.getElementById("btnMotos");
    const btnPecas = document.getElementById("btnPecas");

    if (btnMotos && btnPecas) {
        btnMotos.addEventListener("click", function () {
            console.log("Motos clicado");
            document.getElementById("filtersMotos").style.display = "block";
            document.getElementById("filtersPecas").style.display = "none";
        });

        btnPecas.addEventListener("click", function () {
            console.log("Peças clicado");
            document.getElementById("filtersMotos").style.display = "none";
            document.getElementById("filtersPecas").style.display = "block";
        });
    } else {
        console.error("Botões de alternância não encontrados no DOM");
    }

    // Função auxiliar para preencher selects
    function preencherSelect(url, selectId) {
        fetch(url)
            .then(response => response.json())
            .then(data => {
                console.log(`Preenchendo ${selectId} com:`, data);
                const select = document.getElementById(selectId);
                if (select) {
                    data.forEach(item => {
                        const option = document.createElement("option");
                        option.value = item;
                        option.text = item;
                        select.appendChild(option);
                    });
                } else {
                    console.warn(`Select com ID ${selectId} não encontrado`);
                }
            })
            .catch(err => console.error(`Erro ao buscar ${url}:`, err));
    }

    preencherSelect('/Moto/GetMarcas', 'marcaSelect');
    preencherSelect('/Moto/GetEstilos', 'estiloSelect');
    preencherSelect('/Moto/GetAnos', 'anoDeSelect');
    preencherSelect('/Moto/GetAnos', 'anoAteSelect');

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
        const getValorValido = (id, invalidos = ["", "Marca", "Modelo", "Estilo", "Combustível", "de", "até"]) => {
            const valor = document.getElementById(id).value;
            return invalidos.includes(valor) ? null : valor;
        };

        const filtros = {
            estilo: getValorValido("estiloSelect"),
            marca: getValorValido("marcaSelect"),
            modelo: getValorValido("modeloSelect"),
            precoDe: getValorValido("precoDeSelect"),
            precoAte: getValorValido("precoAteSelect"),
            anoDe: getValorValido("anoDeSelect"),
            anoAte: getValorValido("anoAteSelect"),
            combustivel: getValorValido("combustivelSelect"),
            kmDe: getValorValido("kmDeSelect"),
            kmAte: getValorValido("kmAteSelect"),
        };

        // Limpar os filtros nulos para não poluir a query string
        const query = new URLSearchParams();
        for (const [chave, valor] of Object.entries(filtros)) {
            if (valor !== null) {
                query.append(chave, valor);
            }
        }

        fetch(`/Moto/GetAnuncios?${query.toString()}`)
            .then(response => response.json())
            .then(data => {
                const anunciosContainer = document.getElementById("anunciosContainer");
                anunciosContainer.innerHTML = ""; // Limpar resultados anteriores

                if (data.length === 0) {
                    anunciosContainer.innerHTML = `<p>Nenhum anúncio encontrado com os filtros aplicados.</p>`;
                } else {
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
                }

                document.getElementById("resultadosCount").innerText = `(${data.length} resultados)`;
            })
            .catch(error => {
                console.error("Erro ao buscar anúncios:", error);
            });
    }

    // Adicionar evento ao botão "Ver resultados"
    document.getElementById("btnVerResultados").addEventListener("click", buscarAnuncios);
});
