import { useState } from 'react';
import { ClientService } from '../../../../infrastructure/services/ecom/client/ClientService';
import { GetClientById } from '../../../../application/usesCases/ecom/client/GetClientById';
import type { Client } from '../../../../domain/entities/ecom/client/Client';

export function useClientById() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const getClientById = async (id: number): Promise<Client | null> => {
    setLoading(true);
    setError(null);
    try {
      const service = new ClientService();
      const getClientByIdUseCase = new GetClientById(service);
      const client = await getClientByIdUseCase.execute(id);
      return client;
    } catch (e) {
      setError('Erro ao buscar cliente');
      return null;
    } finally {
      setLoading(false);
    }
  };

  return { getClientById, loading, error };
}
