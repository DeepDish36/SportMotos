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

// Variável global do carrinho
let cart = [];

// 🔥 Função para obter o ID do cliente (simulando login)
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

// 🔥 Função para carregar o carrinho do BD via API
function loadCartFromServer() {
    const clienteId = getClienteId();

    fetch(`/Carrinho/ObterCarrinho?idCliente=${clienteId}`)
        .then(response => response.json())
        .then(data => {
            cart = data; // Atualiza o carrinho com os itens da BD
            updateCartUI(); // Atualiza a interface
        })
        .catch(error => console.error("Erro ao carregar carrinho:", error));
}

// 🔥 Função para atualizar a interface do carrinho lateral
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

// 🔥 Função para remover item do carrinho
function removeFromCart(productId) {
    cart = cart.filter(item => item.id !== productId);

    // 🔥 Remover do BD
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
        loadCartFromServer(); // 🔥 Carregar os itens reais
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
