# EcomApi

EcomApi é uma API desenvolvida em C# utilizando .NET 8, seguindo a arquitetura limpa. Ela gerencia produtos e categorias, utilizando MySQL como banco de dados.

## Tecnologias Utilizadas

- C#
- .NET 9
- MySQL
- Arquitetura Limpa (Clean Architecture)
- VS Code

## Estrutura do Projeto

A API segue uma estrutura baseada na Arquitetura Limpa:

```
EcomApi/
│-- Application/          # Lógica de negócio
│-- Domain/               # Entidades e interfaces
│-- InfraData/            # Persistência de dados
│-- Infrastructure.IoC/   # Injeção de dependência
│-- Web.Mvc/              # Controladores da API
│-- EcomApi.sln           # Solução do projeto
```

## Instalação e Configuração

1. Clone o repositório:
   ```sh
   git clone https://github.com/seu-usuario/EcomApi.git
   ```
2. Navegue até a pasta do projeto:
   ```sh
   cd EcomApi
   ```
3. Restaure as dependências:
   ```sh
   dotnet restore
   ```
4. Configure o banco de dados MySQL no arquivo `appsettings.json`.
5. Aplique as migrações:
   ```sh
   dotnet ef database update
   ```
6. Execute a API:
   ```sh
   dotnet run --project Web.Mvc
   ```

## Endpoints

A API fornece os seguintes endpoints:

### Produtos
- `GET /api/products` - Lista todos os produtos
- `POST /api/products` - Cria um novo produto
- `GET /api/products/{id}` - Retorna um produto pelo ID
- `PUT /api/products/{id}` - Atualiza um produto
- `DELETE /api/products/{id}` - Remove um produto

### Categorias
- `GET /api/categories` - Lista todas as categorias
- `POST /api/categories` - Cria uma nova categoria
- `GET /api/categories/{id}` - Retorna uma categoria pelo ID
- `PUT /api/categories/{id}` - Atualiza uma categoria
- `DELETE /api/categories/{id}` - Remove uma categoria

## Testes

Para rodar os testes, utilize o comando:
```sh
dotnet test
```

## Contribuição

1. Faça um fork do projeto
2. Crie um branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'Adicionando nova feature'`)
4. Envie para o branch principal (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## Licença

Este projeto está licenciado sob a MIT License.

