// src/components/ProductCard.jsx
import React from 'react';
import { useCart } from '../contexts/CartContext';
import ProductImage from './ProductImage';

const ProductCard = ({ product }) => {
  const { addToCart } = useCart();

  return (
    <div className="product-card">
      <div className="product-image">
        <ProductImage 
          src={product.image}
          alt={product.name}
          category={product.category}
        />
      </div>
      <div className="product-info">
        <span className="product-category">{product.category}</span>
        <h3>{product.name}</h3>
        <p className="product-description">{product.description}</p>
        <div className="product-details">
          <span className="product-weight">📦 {product.weight}</span>
          {/* <span className="product-stock">
            {product.inStock ? '✅ Em estoque' : '❌ Indisponível'}
          </span> */}
        </div>
        <div className="product-footer">
          <span className="product-price">R$ {product.price.toFixed(2)}</span>
          <button 
            className="add-to-cart"
            onClick={() => addToCart(product)}
            disabled={!product.inStock}
          >
            Adicionar ao Carrinho
          </button>
        </div>
      </div>
    </div>
  );
};

export default ProductCard;