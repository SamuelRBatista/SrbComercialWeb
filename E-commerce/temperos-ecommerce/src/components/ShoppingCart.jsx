import React from 'react';
import { useCart } from '../contexts/CartContext';

const ShoppingCart = ({ onClose }) => {
  const { cartItems, total, removeFromCart, updateQuantity, clearCart } = useCart();

  if (cartItems.length === 0) {
    return (
      <div className="cart-overlay" onClick={onClose}>
        <div className="cart-modal" onClick={e => e.stopPropagation()}>
          <div className="cart-header">
            <h2>🛒 Carrinho</h2>
            <button className="close-cart" onClick={onClose}>✕</button>
          </div>
          <div className="empty-cart">
            <span>🛍️</span>
            <p>Seu carrinho está vazio</p>
            <button className="continue-shopping" onClick={onClose}>
              Continuar Comprando
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="cart-overlay" onClick={onClose}>
      <div className="cart-modal" onClick={e => e.stopPropagation()}>
        <div className="cart-header">
          <h2>🛒 Carrinho ({cartItems.length} itens)</h2>
          <button className="close-cart" onClick={onClose}>✕</button>
        </div>

        <div className="cart-items">
          {cartItems.map(item => (
            <div key={item.id} className="cart-item">
              <img src={item.image} alt={item.name} className="cart-item-image" />
              <div className="cart-item-info">
                <h4>{item.name}</h4>
                <span className="cart-item-weight">{item.weight}</span>
                <span className="cart-item-price">R$ {item.price.toFixed(2)}</span>
              </div>
              <div className="cart-item-actions">
                <div className="quantity-control">
                  <button 
                    onClick={() => updateQuantity(item.id, item.quantity - 1)}
                    className="quantity-btn"
                  >
                    -
                  </button>
                  <span className="quantity">{item.quantity}</span>
                  <button 
                    onClick={() => updateQuantity(item.id, item.quantity + 1)}
                    className="quantity-btn"
                  >
                    +
                  </button>
                </div>
                <button 
                  onClick={() => removeFromCart(item.id)}
                  className="remove-item"
                >
                  🗑️
                </button>
              </div>
            </div>
          ))}
        </div>

        <div className="cart-footer">
          <div className="cart-total">
            <span>Total:</span>
            <strong>R$ {total.toFixed(2)}</strong>
          </div>
          <div className="cart-actions">
            <button className="clear-cart" onClick={clearCart}>
              Limpar Carrinho
            </button>
            <button className="checkout-btn">
              Finalizar Compra
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ShoppingCart;