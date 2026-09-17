import React from 'react';
import { useCart } from '../contexts/CartContext';

const Header = ({ onCartClick }) => {
  const { getTotalItems } = useCart();

  return (
    <header className="header">
      <div className="header-content">
        <div className="logo">
          <h1>🌿 Temperos Finos</h1>
          <span>Os melhores temperos para sua cozinha</span>
        </div>
        <button className="cart-button" onClick={onCartClick}>
          🛒 Carrinho
          {getTotalItems() > 0 && (
            <span className="cart-badge">{getTotalItems()}</span>
          )}
        </button>
      </div>
    </header>
  );
};

export default Header;