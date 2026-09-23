import axios from 'axios';
import type { Product } from '../../../../domain/entities/ecom/product/Product';
import type { IProductRepository } from '../../../../domain/repositories/ecom/product/IProductRepository';
import { API_BASE_URL } from '../../../../shared/config/api';

export class ProductService implements IProductRepository {
 
  private baseUrl = `${API_BASE_URL}/Products`;

  async getAll(): Promise<Product[]> {
    const res = await axios.get(this.baseUrl);
    return res.data;
  }

  async getById(id: number): Promise<Product> { 
    const res = await axios.get(`${this.baseUrl}/${id}`);
    return res.data;
  }

   async create(formData: FormData): Promise<Product> { 
    const response = await axios.post<Product>(this.baseUrl, formData);
    return response.data;
  }

   async update(formData: FormData): Promise<void> {
    const id = formData.get('id');
    if (!id) throw new Error('Id do produto não fornecido no FormData');

    try {
      await axios.put(`${this.baseUrl}/${id}`, formData);
    } catch (error) {
      if (axios.isAxiosError(error)) {
        const responseData = error.response?.data;
        const validationErrors = responseData?.Errors || responseData?.errors;
        const validationMessage = Array.isArray(validationErrors)
          ? validationErrors
              .map((item: { Mensagem?: string; message?: string }) => item.Mensagem || item.message || item)
              .join(' ')
          : undefined;
        throw new Error(
          responseData?.Error || responseData?.message || validationMessage || 'Não foi possível atualizar o produto.'
        );
      }

      throw error;
    }
  }

  async delete(id: number): Promise<void> {
    await axios.delete(`${this.baseUrl}/${id}`);
  }
}
