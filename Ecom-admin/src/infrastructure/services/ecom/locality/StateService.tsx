import axios from 'axios';
import type {State} from '../../../../domain/entities/ecom/locality/State';
import type { IStateRepository } from '../../../../domain/repositories/ecom/locality/IState.Repositories';
import { API_BASE_URL } from '../../../../shared/config/api';

export class StateService implements IStateRepository {
  private baseUrl = `${API_BASE_URL}/State`;

  async getAll(): Promise<State[]> {
    const res = await axios.get(this.baseUrl);
    return res.data;
  }
}
