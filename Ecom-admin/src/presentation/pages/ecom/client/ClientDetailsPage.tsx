import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import styles from './styles';

import { useStates } from '../../../../shared/hooks/ecom/locality/useStates';
import { useCities } from '../../../../shared/hooks/ecom/locality/useCities';
import { useAppContext } from '../../../../shared/contexts/ContextProvider'; // Importa o contexto
import type { Client } from '../../../../domain/entities/ecom/client/Client';
import SidebarLayout from '../../../layouts/components/SidebarLayout';

const ClientDetailsPage = () => {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const { client } = useAppContext(); // pega do contexto

  const [clientData, setClientData] = useState<Client | null>(null);
  const [loading, setLoading] = useState(true);

  const { states } = useStates();
  const { cities } = useCities(clientData?.stateId ?? 0);

  useEffect(() => {
    const fetchClient = async () => {
      if (!id) return;
      setLoading(true);
      try {
        const data = await client.getClientById(Number(id));
        setClientData(data);
      } catch (error) {
        console.error('Erro ao buscar cliente:', error);
        setClientData(null);
      } finally {
        setLoading(false);
      }
    };

    fetchClient();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id]); // só id como dependência

  if (loading) return <p>Carregando detalhes...</p>;
  if (!clientData) return <p>Cliente não encontrado.</p>;

  const stateName = states.find((state) => state.id === clientData.stateId)?.name || '';
  const cityName = cities.find((city) => city.id === clientData.cityId)?.name || '';

  return (
    <SidebarLayout isCollapsed={false}>
      <div style={styles.cadastroFormContainer}>
        <h2 style={styles.title}>Detalhes do Cliente</h2>

        <div style={styles.formRow}>
          <label>Nome:</label>
          <span>{clientData.name}</span>

          <label>Cpf:</label>
          <span>{clientData.cpf}</span>
        </div>

        <div style={styles.formRow}>
          <label>E-mail:</label>
          <span>{clientData.email}</span>

          <label>Telefone:</label>
          <span>{clientData.phoneNumber}</span>

          <label>Cep:</label>
          <span>{clientData.zipCode}</span>

          <label>Endereço:</label>
          <span>{clientData.address}</span>

          <label>Bairro:</label>
          <span>{clientData.neighborhood}</span>
        </div>

        <div style={styles.formRow}>
          <label>Estado:</label>
          <span>{stateName}</span>

          <label>Cidade:</label>
          <span>{cityName}</span>
        </div>

        <div style={styles.formRow}>
          <button
            type="button"
            onClick={() => navigate('/panel/client')}
            style={styles.btnCancel}
          >
            Voltar
          </button>
        </div>
      </div>
    </SidebarLayout>
  );
};

export default ClientDetailsPage;
