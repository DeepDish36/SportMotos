// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Example starter JavaScript for disabling form submissions if there are invalid fields

//Não dá submit ao formulário se os campos não forem válidos
(() => {
    'use strict'

    // Fetch all the forms we want to apply custom Bootstrap validation styles to
    const forms = document.querySelectorAll('.needs-validation')

    // Loop over them and prevent submission
    Array.from(forms).forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault()
                event.stopPropagation()
            }

            form.classList.add('was-validated')
        }, false)
    })
})()

// Passar o ID do cliente pela URL para aceder à sua cesta
document.addEventListener("DOMContentLoaded", function () {
    const cestaButton = document.getElementById("cestaButton");

    if (cestaButton) {
        const clienteId = getClienteId();
        cestaButton.href = `/Carrinho/Cesta?idCliente=${clienteId}`;
    }
});

// Lista para armazenar os produtos (guarda os itens para mostrar na UI)
let cart = [];

// Função para obter o ID do cliente
function getClienteId() {
    // Obtém o ID do cliente logado a partir de um endpoint de autenticação
    return fetch('/Login/ObterClienteLogado')
        .then(response => {
            if (!response.ok) {
                throw new Error('Erro ao obter o cliente logado');
            }
            return response.json();
        })
        .then(data => data.clienteId)
        .catch(error => {
            console.error("Erro ao obter o ID do cliente logado:", error);
            return null; // Retorna null se houver erro
        });
}

// Carregar os dados do carrinho na BD via Fetch API
function loadCartFromServer() {
    const clienteId = getClienteId(); // Obtém o ID do cliente logado

    fetch(`/Carrinho/ObterCarrinho?idCliente=${parseInt(clienteId)}`)
        .then(response => response.json())
        .then(data => {
            cart = data; // Atualiza a variável global do carrinho
            updateCartUI(); // Atualiza a interface
        })
        .catch(error => console.error("Erro ao carregar carrinho:", error));
}


// Função para atualizar a UI do carrinho 
function updateCartUI() {
    let cartContainer = document.getElementById("cartItems");
    cartContainer.innerHTML = "";

    let totalPrice = 0;
    let totalQuantity = 0;

    if (cart.length === 0) {
        cartContainer.innerHTML = `<p class="empty-cart-message"><b>Carrinho vazio!</b></p>`;
    } else {
        cart.forEach(item => {
            totalQuantity += item.quantity;
            totalPrice += item.price * item.quantity;

            let cartItem = document.createElement("div");
            cartItem.classList.add("cart-item");
            cartItem.innerHTML = `
                <img src="${item.image}" alt="Produto">
                <div class="cart-item-details">
                    <p><strong>€${item.price.toFixed(2)}</strong></p>
                    <p>${item.brand} - ${item.name}</p>
                    <p>Qtd.: ${item.quantity}</p>
                </div>
                <button class="remove-item" onclick="removeFromCart(${item.id})">
                    🗑️
                </button>
                <hr>
            `;
            cartContainer.appendChild(cartItem);
        });
    }

    document.getElementById("cartItemCount").textContent = totalQuantity;
    document.getElementById("subtotalPrice").textContent = `€${totalPrice.toFixed(2)}`;
    document.getElementById("totalPrice").textContent = `€${totalPrice.toFixed(2)}`;
}

// Remover item do carrinho
function removeFromCart(productId) {
    cart = cart.filter(item => item.id !== productId);

    // Remover do carrinho
    fetch(`/Carrinho/RemoverItem?idCliente=${getClienteId()}&idPeca=${productId}`, {
        method: "DELETE"
    }).then(() => updateCartUI())
        .catch(error => console.error("Erro ao remover item:", error));
}

// 🔥 Função para alternar a exibição do carrinho
function toggleCart() {
    let cartDropdown = document.getElementById("cartDropdown");

    if (cartDropdown.style.display === "block") {
        cartDropdown.style.display = "none";
    } else {
        cartDropdown.style.display = "block";
        loadCartFromServer(); 
    }
}

// 🔥 Fecha o carrinho ao clicar fora
document.addEventListener("click", function (event) {
    let cartDropdown = document.getElementById("cartDropdown");
    let cartButton = document.getElementById("cartButton");

    if (cartDropdown && cartButton && !cartDropdown.contains(event.target) && !cartButton.contains(event.target)) {
        cartDropdown.style.display = "none";
    }
});
