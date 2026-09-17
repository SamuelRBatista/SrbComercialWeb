import axios from 'axios';
import type { Category } from '../../../../domain/entities/ecom/product/Category';
import type { ICategoryRepository } from '../../../../domain/repositories/ecom/product/ICategoryRepository';
import { API_BASE_URL } from '../../../../shared/config/api';

export class CategoryService implements ICategoryRepository {
  private baseUrl = `${API_BASE_URL}/category`;

  async getAll(): Promise<Category[]> {
    const res = await axios.get(this.baseUrl);
    return res.data;
  }
}