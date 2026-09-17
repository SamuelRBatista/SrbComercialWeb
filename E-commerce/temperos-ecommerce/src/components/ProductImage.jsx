// src/components/ProductImage.jsx
import React, { useState } from 'react';

const ProductImage = ({ src, alt, category, className }) => {
  const [error, setError] = useState(false);
  const [loading, setLoading] = useState(true);

  // Emojis por categoria para fallback
  const getCategoryEmoji = (cat) => {
    const emojis = {
      'Pimentas': '🌶️',
      'Temperos em Pó': '🧂',
      'Desidratados': '🧄',
      'Ervas': '🌿',
      'Doces': '🍂',
      'Grãos': '🌾',
      'Sem Categoria': '📦'
    };
    return emojis[cat] || '📦';
  };

  // Cores por categoria para fallback
  const getCategoryColor = (cat) => {
    const colors = {
      'Pimentas': '#2d5016',
      'Temperos em Pó': '#cc3333',
      'Desidratados': '#f5a623',
      'Ervas': '#2e7d32',
      'Doces': '#8d6e63',
      'Grãos': '#5d4037',
      'Sem Categoria': '#4a7c2e'
    };
    return colors[cat] || '#4a7c2e';
  };

  if (error) {
    return (
      <div 
        className={`placeholder-image ${className || ''}`}
        style={{ 
          backgroundColor: getCategoryColor(category),
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          height: '100%',
          color: 'white',
          padding: '20px'
        }}
      >
        <span style={{ fontSize: '3rem' }}>{getCategoryEmoji(category)}</span>
        <span style={{ fontSize: '0.9rem', marginTop: '10px', textAlign: 'center' }}>
          {category || 'Produto'}
        </span>
      </div>
    );
  }

  return (
    <>
      {loading && (
        <div className="image-loading" style={{
          position: 'absolute',
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          background: '#f0f0f0',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center'
        }}>
          <span>🔄</span>
        </div>
      )}
      <img
        src={src}
        alt={alt}
        className={className}
        onError={() => setError(true)}
        onLoad={() => setLoading(false)}
        style={{ display: loading ? 'none' : 'block' }}
      />
    </>
  );
};

export default ProductImage;