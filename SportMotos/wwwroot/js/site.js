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

let cart = [];

function toggleCart() {
    let cartDropdown = document.getElementById("cartDropdown");
    // Alterna a exibição do carrinho
    if (cartDropdown.style.display === "block") {
        cartDropdown.style.display = "none";
    } else {
        cartDropdown.style.display = "block";
    }
}

// Fecha o carrinho ao clicar fora
document.addEventListener("click", function (event) {
    let cartDropdown = document.getElementById("cartDropdown");
    let cartButton = document.getElementById("cartButton");

    // Se o clique não for dentro do carrinho ou do botão, fecha o carrinho
    if (!cartDropdown.contains(event.target) && !cartButton.contains(event.target)) {
        cartDropdown.style.display = "none";
    }
});

// Função para remover um item do carrinho
function removeFromCart(productId) {
    cart = cart.filter(item => item.id !== productId);
    updateCartUI();
}


function addToCart(product) {
    let existingProduct = cart.find(item => item.id === product.id);
    if (existingProduct) {
        existingProduct.quantity++;
    } else {
        cart.push({ ...product, quantity: 1 });
    }
    updateCartUI();
}

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


// Simulação de produtos (remova depois de testar)
setTimeout(() => {
    addToCart({ id: 1, name: "Capacete Moto", brand: "AGV", price: 99.99, image: "https://via.placeholder.com/50" });
    addToCart({ id: 2, name: "Luvas Racing", brand: "Alpinestars", price: 39.99, image: "https://via.placeholder.com/50" });
}, 2000);
