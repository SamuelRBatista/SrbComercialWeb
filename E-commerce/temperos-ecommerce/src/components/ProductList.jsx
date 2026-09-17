import React, { useState } from 'react';
import ProductCard from './ProductCard';
import { useProducts } from '../hooks/useProducts';

const ProductList = () => {
  const { products, loading, error } = useProducts();
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('');

  // Extrair categorias únicas dos produtos (adaptado da API)
  const categories = [...new Set(products.map(p => p.category).filter(Boolean))];

  // Filtrar produtos
  const filteredProducts = products.filter(product => {
    const matchesSearch = product.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         product.description.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesCategory = selectedCategory === '' || product.category === selectedCategory;
    return matchesSearch && matchesCategory;
  });

  // Estado de carregamento
  if (loading) {
    return (
      <div className="loading-container">
        <div className="loading-spinner"></div>
        <p>Carregando produtos...</p>
      </div>
    );
  }

  // Estado de erro
  if (error) {
    return (
      <div className="error-container">
        <p>❌ {error}</p>
        <button onClick={() => window.location.reload()} className="retry-btn">
          Tentar Novamente
        </button>
      </div>
    );
  }

  return (
    <div className="product-list">
      <div className="filters">
        <input
          type="text"
          placeholder="🔍 Buscar temperos..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="search-input"
        />
        <select
          value={selectedCategory}
          onChange={(e) => setSelectedCategory(e.target.value)}
          className="category-filter"
        >
          <option value="">Todas as categorias</option>
          {categories.map(category => (
            <option key={category} value={category}>{category}</option>
          ))}
        </select>
      </div>

      <div className="products-count">
        {filteredProducts.length} produto(s) encontrado(s)
      </div>

      <div className="products-grid">
        {filteredProducts.map(product => (
          <ProductCard key={product.id} product={product} />
        ))}
      </div>

      {filteredProducts.length === 0 && (
        <div className="no-products">
          <p>Nenhum produto encontrado</p>
        </div>
      )}
    </div>
  );
};

export default ProductList;