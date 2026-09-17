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
    const response = await axios.post<Product>(this.baseUrl, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });      
    return response.data;
  }

   async update(formData: FormData): Promise<void> {
    const id = formData.get('id');
    if (!id) throw new Error('Id do produto não fornecido no FormData');

    await axios.put(`${this.baseUrl}/${id}`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  }

  async delete(id: number): Promise<void> {
    await axios.delete(`${this.baseUrl}/${id}`);
  }
}
