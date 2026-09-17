# SrbComercialWeb

Plataforma de e-commerce composta por uma API em .NET, um painel administrativo React e uma loja virtual React. O projeto pode ser executado localmente com Docker Compose.

## Aplicacoes

- `EcomApi`: API ASP.NET Core 8 com Dapper e PostgreSQL.
- `Ecom-admin`: painel administrativo em React, TypeScript, Vite e Material UI.
- `E-commerce/temperos-ecommerce`: loja virtual em React e Vite.
- `docker-compose.yml`: orquestracao da API, PostgreSQL e frontends.

## Pre-requisitos

- Docker Desktop com o Linux engine em execucao.
- Git.

Nao e necessario instalar .NET ou Node.js para executar o ambiente completo via Docker.

## Configuracao

1. Clone o repositorio:

```powershell
git clone https://github.com/SamuelRBatista/SrbComercialWeb.git
cd SrbComercialWeb
```

2. Crie o arquivo local de ambiente a partir do exemplo:

```powershell
Copy-Item .env.example .env
```

3. Edite `.env` e defina valores locais fortes:

```env
POSTGRES_PASSWORD=uma-senha-local
JWT_SECRET_KEY=uma-chave-jwt-longa-e-segura
```

O arquivo `.env` e ignorado pelo Git e nao deve ser publicado.

## Executar com Docker

Na raiz do projeto:

```powershell
docker compose up --build
```

Para executar em segundo plano:

```powershell
docker compose up --build -d
```

Para acompanhar os logs:

```powershell
docker compose logs -f api
```

Para parar os containers sem apagar os dados do banco:

```powershell
docker compose down
```

Para remover tambem o volume do PostgreSQL, apagando os dados locais:

```powershell
docker compose down -v
```

O script `EcomApi/Database/init.sql` cria as tabelas e insere dados demonstrativos quando o banco e inicializado.

## URLs locais

| Servico | URL |
| --- | --- |
| API | http://localhost:5124 |
| Swagger | http://localhost:5124/swagger |
| Painel administrativo | http://localhost:5173 |
| Loja virtual | http://localhost:5174 |
| PostgreSQL | localhost:5432 |

## Execucao sem Docker

### API

```powershell
Push-Location EcomApi\Web.Mvc
dotnet run
Pop-Location
```

### Painel administrativo

```powershell
Push-Location Ecom-admin
npm install
npm run dev
Pop-Location
```

### Loja virtual

```powershell
Push-Location E-commerce\temperos-ecommerce
npm install
npm run dev
Pop-Location
```

Para os frontends locais, a API deve estar acessivel em `http://localhost:5124`.

## Validacao

Build do painel administrativo:

```powershell
Push-Location Ecom-admin
npm run build
Pop-Location
```

Build da loja:

```powershell
Push-Location E-commerce\temperos-ecommerce
npm run build
Pop-Location
```

Build da API:

```powershell
Push-Location EcomApi\Web.Mvc
dotnet build
Pop-Location
```

## Estrutura

```text
SrbComercialWeb/
|-- EcomApi/
|   |-- Application/
|   |-- Domain/
|   |-- InfraData/
|   |-- Infrastructure.IoC/
|   |-- Web.Mvc/
|   |-- Database/init.sql
|   `-- Dockerfile
|-- Ecom-admin/
|   |-- src/
|   |-- Dockerfile
|   `-- nginx.conf
|-- E-commerce/
|   |-- temperos-ecommerce/
|   |-- Dockerfile
|   `-- nginx.conf
|-- docker-compose.yml
`-- .env.example
```

## Observacoes

- O banco usa um volume Docker chamado `postgres_data` para persistir os dados.
- Os dados demonstrativos sao inseridos de forma idempotente pelo script SQL.
- A API usa a rede interna do Compose para acessar o banco pelo hostname `postgres`.
