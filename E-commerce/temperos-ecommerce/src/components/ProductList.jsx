import React, { useState } from 'react';
import ProductCard from './ProductCard';
import { useCategories, useProducts } from '../hooks/useProducts';

const ProductList = () => {
  const { products, loading, error } = useProducts();
  const { categories, loading: categoriesLoading } = useCategories();
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('');

  const productsWithCategories = products.map((product) => {
    const category = categories.find((item) => item.id === product.categoryId);

    return {
      ...product,
      category: category?.name || product.category || 'Sem Categoria',
    };
  });

  const categoryNames = [...new Set(productsWithCategories.map((product) => product.category))];

  // Filtrar produtos
  const filteredProducts = productsWithCategories.filter(product => {
    const matchesSearch = product.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         product.description.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesCategory = selectedCategory === '' || product.category === selectedCategory;
    return matchesSearch && matchesCategory;
  });

  // Estado de carregamento
  if (loading || categoriesLoading) {
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
          {categoryNames.map(category => (
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