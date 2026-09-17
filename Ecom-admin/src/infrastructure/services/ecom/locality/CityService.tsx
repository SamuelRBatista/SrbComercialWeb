import axios from 'axios';

import type {City} from '../../../../domain/entities/ecom/locality/City';
import type { ICityRepository } from '../../../../domain/repositories/ecom/locality/ICityRepositories';
import { API_BASE_URL } from '../../../../shared/config/api';

export class CityService implements ICityRepository {

  private baseUrl = `${API_BASE_URL}/City`;

  async getAll(): Promise<City[]> {
    const res = await axios.get(this.baseUrl);
    return res.data;
  }

  async getByStateId(stateId: number): Promise<City[]> {
    const all = await this.getAll();
    return all.filter(city => city.stateId === stateId);
  }
}