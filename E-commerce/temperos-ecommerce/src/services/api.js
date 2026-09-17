// src/services/api.js

const API_BASE_URL = 'http://localhost:5124/api';
const API_BASE_URL_WITHOUT_API = 'http://localhost:5124'; // URL sem /api

// Função para lidar com erros da API
const handleResponse = async (response) => {
  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(errorData.message || `Erro ${response.status}: ${response.statusText}`);
  }
  return response.json();
};

// Função para construir URL da imagem corretamente
const buildImageUrl = (imagePath) => {
  if (!imagePath) return null;
  
  // Se já for uma URL completa, retorna ela
  if (imagePath.startsWith('http://') || imagePath.startsWith('https://')) {
    return imagePath;
  }
  
  // Remove barras duplicadas
  const cleanPath = imagePath.startsWith('/') ? imagePath : `/${imagePath}`;
  
  // Constrói a URL completa
  return `${API_BASE_URL_WITHOUT_API}${cleanPath}`;
};

// Função para adaptar os dados da API para o formato do frontend
export const adaptProductToFrontend = (apiProduct) => {
  // Construir URL completa da imagem
  let imageUrl = 'https://via.placeholder.com/400x300/4a7c2e/ffffff?text=Sem+Imagem';
  
  if (apiProduct.imageUrl) {
    const builtUrl = buildImageUrl(apiProduct.imageUrl);
    if (builtUrl) {
      imageUrl = builtUrl;
    }
  }

  // Se tiver imagens no array, usa a primeira
  if (apiProduct.images && apiProduct.images.length > 0) {
    const firstImage = apiProduct.images[0];
    if (firstImage.url || firstImage.imageUrl) {
      const imgUrl = firstImage.url || firstImage.imageUrl;
      const builtUrl = buildImageUrl(imgUrl);
      if (builtUrl) {
        imageUrl = builtUrl;
      }
    }
  }

  // Mapear status de estoque
  const inStock = apiProduct.stockQuantity > 0 && apiProduct.isActive;

  // Determinar categoria
  const categoryName = apiProduct.categoryName || 'Sem Categoria';

  return {
    id: apiProduct.id,
    name: apiProduct.name,
    description: apiProduct.description || apiProduct.shortDescription || 'Sem descrição disponível',
    price: apiProduct.price,
    image: imageUrl,
    category: categoryName,
    weight: apiProduct.weight ? `${apiProduct.weight}g` : 'Peso não informado',
    inStock: inStock,
    sku: apiProduct.sku,
    stockQuantity: apiProduct.stockQuantity,
    isActive: apiProduct.isActive,
    rating: apiProduct.averageRating || 0,
    totalReviews: apiProduct.totalReviews || 0,
    // Dados originais da API
    rawData: apiProduct
  };
};

// Função para adaptar múltiplos produtos
export const adaptProductsToFrontend = (apiProducts) => {
  if (!apiProducts || !Array.isArray(apiProducts)) {
    return [];
  }
  return apiProducts.map(adaptProductToFrontend);
};

// Serviço de API
export const api = {
  // Buscar todos os produtos
  getProducts: async () => {
    try {
      const response = await fetch(`${API_BASE_URL}/Products`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          // Se precisar de autenticação, adicione aqui:
          // 'Authorization': `Bearer ${token}`,
        },
      });
      return await handleResponse(response);
    } catch (error) {
      console.error('Erro ao buscar produtos:', error);
      throw error;
    }
  },

  // Buscar produto por ID
  getProductById: async (id) => {
    try {
      const response = await fetch(`${API_BASE_URL}/Products/${id}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      return await handleResponse(response);
    } catch (error) {
      console.error(`Erro ao buscar produto ${id}:`, error);
      throw error;
    }
  },

  // Buscar produtos por categoria
  getProductsByCategory: async (categoryId) => {
    try {
      const response = await fetch(`${API_BASE_URL}/Products/category/${categoryId}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      return await handleResponse(response);
    } catch (error) {
      console.error(`Erro ao buscar produtos da categoria ${categoryId}:`, error);
      throw error;
    }
  },

  // Criar novo pedido
  createOrder: async (orderData) => {
    try {
      const response = await fetch(`${API_BASE_URL}/Orders`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(orderData),
      });
      return await handleResponse(response);
    } catch (error) {
      console.error('Erro ao criar pedido:', error);
      throw error;
    }
  },

  // Buscar categorias
  getCategories: async () => {
    try {
      const response = await fetch(`${API_BASE_URL}/Categories`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      return await handleResponse(response);
    } catch (error) {
      console.error('Erro ao buscar categorias:', error);
      throw error;
    }
  },
};