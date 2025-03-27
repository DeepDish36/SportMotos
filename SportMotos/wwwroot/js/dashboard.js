//Código utilizado no Dashboard.cshtml

// Inicializar DataTables
$(function () {
    $('#motosTable').DataTable({
        language: {
            url: "/datatable/langconfig.json"
        }
    });
    $('#pecasTable').DataTable({
        language: {
            url: "/datatable/langconfig.json"
        }
    });
    $('#anuncioMotosTable').DataTable({
        language: {
            url: "/datatable/langconfig.json"
        }
    });
    $('#anuncioPecasTable').DataTable({
        language: {
            url: "/datatable/langconfig.json"
        }
    });
    $('#pedidosTable').DataTable({
        language: {
            url: "/datatable/langconfig.json"
        }
    });
});

// Modal
function showDeleteModal(id, tipo) {
    let mensagem = "";
    let url = "";

    // Define a mensagem e a URL com base no tipo
    switch (tipo) {
        case "moto":
            mensagem = "Tem certeza de que deseja excluir esta moto?";
            url = `/CriarAnuncio/ExcluirMoto/${id}`;
            break;
        case "peca":
            mensagem = "Tem certeza de que deseja excluir esta peça?";
            url = `/Pecas/ExcluirPeca/${id}`;
            break;
        case "anuncioMoto":
            mensagem = "Tem certeza de que deseja excluir este anúncio de moto?";
            url = `/Anuncios/ExcluirAnuncioMoto/${id}`;
            break;
        case "anuncioPeca":
            mensagem = "Tem certeza de que deseja excluir este anúncio de peça?";
            url = `/Anuncios/ExcluirAnuncioPeca/${id}`;
            break;
    }

    // Atualiza a modal com a mensagem correta
    document.getElementById("deleteMessage").innerText = mensagem;

    // Define a ação do botão de confirmação
    document.getElementById("confirmDeleteButton").onclick = function () {
        window.location.href = url; // Redireciona para a exclusão
    };

    // Exibe a modal
    var deleteModal = new bootstrap.Modal(document.getElementById("confirmDeleteModal"));
    deleteModal.show();
}

// Gráfico de Estatísticas
document.getElementById("mes").addEventListener("change", function () {
    let mesSelecionado = this.value;
    fetch(`/Dashboard/GetEstatisticas?mes=${mesSelecionado}`)
        .then(response => response.json())
        .then(data => {
            atualizarGrafico(data);
        });
});

function atualizarGrafico(data) {
    let ctx = document.getElementById("vendasChart").getContext("2d");
    new Chart(ctx, {
        type: "bar", // ou "line"
        data: {
            labels: ["Clientes", "Anúncios Vendidos"],
            datasets: [{
                label: `Dados de ${data.mes}`,
                data: [data.clientes, data.anunciosVendidos],
                backgroundColor: ["blue", "green"]
            }]
        }
    });
}

// Mostrar Secções
function showSection(sectionId) {
    var sections = document.querySelectorAll(".dashboard-section");
    sections.forEach(section => {
        section.style.display = "none";
    });
    document.getElementById(sectionId).style.display = "block";
}

var ctxVendas = document.getElementById("vendasChart").getContext("2d");
var vendasChart = new Chart(ctxVendas, {
    type: "line",
    data: {
        labels: JSON.parse('@Html.Raw(Json.Serialize(ViewBag.VendasUltimosMesesLabels))'),
        datasets: [{
            label: "Vendas (€)",
            data: JSON.parse('@Html.Raw(Json.Serialize(ViewBag.VendasUltimosMesesData))'),
            borderColor: "#007bff",
            backgroundColor: "rgba(0, 123, 255, 0.2)",
            fill: true
        }]
    }
});
