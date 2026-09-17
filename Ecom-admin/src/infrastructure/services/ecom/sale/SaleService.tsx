import axios from 'axios';
import { API_BASE_URL } from '../../../../shared/config/api';

const baseUrl = `${API_BASE_URL}/Products`;

export async function sellProduct(productId: number, quantity: number, reason?: string, documentNumber?: string, userId?: number) {
  const payload = { quantity, reason, documentNumber, userId };
  const res = await axios.post(`${baseUrl}/${productId}/sell`, payload);
  return res.data;
}

export async function getMovements(productId: number, limit = 50) {
  const res = await axios.get(`${baseUrl}/${productId}/movements?limit=${limit}`);
  return res.data;
}
