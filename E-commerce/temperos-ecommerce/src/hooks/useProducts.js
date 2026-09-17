// src/hooks/useProducts.js
import { useState, useEffect } from 'react';
import { api, adaptProductsToFrontend, adaptProductToFrontend } from '../services/api';

export const useProducts = () => {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchProducts = async () => {
    try {
      setLoading(true);
      setError(null);
      
      const data = await api.getProducts();
      const adaptedProducts = adaptProductsToFrontend(data);
      setProducts(adaptedProducts);
    } catch (err) {
      setError(err.message || 'Erro ao carregar produtos');
      console.error('Erro ao buscar produtos:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchProducts();
  }, []);

  return { products, loading, error, refetch: fetchProducts };
};

export const useProduct = (id) => {
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchProduct = async () => {
      if (!id) return;
      
      try {
        setLoading(true);
        setError(null);
        
        const data = await api.getProductById(id);
        const adaptedProduct = adaptProductToFrontend(data);
        setProduct(adaptedProduct);
      } catch (err) {
        setError(err.message || 'Erro ao carregar produto');
        console.error(`Erro ao buscar produto ${id}:`, err);
      } finally {
        setLoading(false);
      }
    };

    fetchProduct();
  }, [id]);

  return { product, loading, error };
};

export const useCategories = () => {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        setLoading(true);
        setError(null);
        
        const data = await api.getCategories();
        setCategories(data);
      } catch (err) {
        setError(err.message || 'Erro ao carregar categorias');
        console.error('Erro ao buscar categorias:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchCategories();
  }, []);

  return { categories, loading, error };
};