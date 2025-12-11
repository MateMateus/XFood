# 🥗 xFood — Sistema de Gestão para Delivery

> **Status:** Em Desenvolvimento 🚧

O **xFood** é uma solução backend modular e robusta desenvolvida em **.NET**, projetada para gerenciar o ecossistema de delivery e restaurantes. O projeto adota os princípios da **Clean Architecture** (Arquitetura Limpa) e **DDD (Domain-Driven Design)** para assegurar escalabilidade, testabilidade e facilidade de manutenção a longo prazo.

-----

## 🏛 Arquitetura e Design

O projeto foi construído com foco na separação de responsabilidades, garantindo que as regras de negócio não dependam de tecnologias externas (como bancos de dados ou frameworks de UI).

A solução é dividida em 4 camadas principais:

| Camada | Responsabilidade |
| :--- | :--- |
| **🔸 Domain** | O núcleo do sistema. Contém as Entidades, Agregados, Value Objects e Interfaces de Repositório. É independente de qualquer tecnologia externa. |
| **🔸 Application** | Contém os casos de uso (Use Cases), DTOs, Validadores e Mappers. Orquestra o fluxo de dados entre o mundo externo e o Domínio. |
| **🔸 Infrastructure** | Implementa as interfaces definidas no Domínio. Aqui residem o EF Core, acesso a dados, integrações com APIs de terceiros e configurações de e-mail/filas. |
| **🔸 Web** | A camada de entrada. Contém a API RESTful, Controllers, Middlewares, configuração de DI e tratamento de exceções globais. |

-----

## 🚀 Tecnologias Utilizadas

  * **Linguagem:** C\#
  * **Framework:** .NET (6.0+)
  * **ORM:** Entity Framework Core
  * **Banco de Dados:** SQL Server (Padrão)
  * **Arquitetura:** Clean Architecture / DDD
  * **API:** RESTful

-----

## 📂 Estrutura do Projeto

A organização das pastas reflete diretamente a arquitetura em camadas:

```bash
xFood/
│── xFood.sln                # Solução principal
│── global.json              # Configuração da versão do .NET SDK
│
├── xFood.Domain/            # 🧠 Núcleo: Entidades e Regras de Negócio
├── xFood.Application/       # 💼 Casos de uso, Services e DTOs
├── xFood.Infrastructure/    # 💾 Banco de dados, Migrations e Integrações
└── xFood.Web/               # 🌐 API, Controllers e Endpoints
```

-----

## ⚙️ Como Executar o Projeto

Siga os passos abaixo para rodar a aplicação em ambiente de desenvolvimento.

### Pré-requisitos

  * [.NET SDK](https://dotnet.microsoft.com/download) instalado.
  * Uma instância de banco de dados (SQL Server ou LocalDB).
  * Ferramenta de linha de comando do EF Core (`dotnet tool install --global dotnet-ef`).

### 1\. Clonar e Restaurar

Clone o repositório e baixe as dependências do NuGet:

```bash
git clone https://github.com/SEU-USUARIO/xFood.git
cd xFood
dotnet restore
```

### 2\. Configurar Banco de Dados

O projeto utiliza **Entity Framework Core**. Você precisa aplicar as migrations para criar o banco de dados.

Certifique-se de que a *Connection String* no `appsettings.json` (dentro de `xFood.Web`) está apontando para o seu servidor local.

```bash
# Criar a migration inicial (caso não exista)
dotnet ef migrations add InitialCreate -p xFood.Infrastructure -s xFood.Web

# Aplicar as alterações no banco de dados
dotnet ef database update -p xFood.Infrastructure -s xFood.Web
```

### 3\. Rodar a Aplicação

Compile e execute o projeto da API:

```bash
cd xFood.Web
dotnet run
```

A API estará disponível em: `https://localhost:5001` ou `http://localhost:5000`.

-----

## 📌 Funcionalidades Principais

O sistema prevê as seguintes funcionalidades de negócio:

  - [ ] **Gestão de Restaurantes:** Cadastro e configuração de estabelecimentos.
  - [ ] **Cardápio Digital:** Gestão de produtos, categorias e preços.
  - [ ] **Gestão de Pedidos:** Fluxo completo desde a criação até a entrega.
  - [ ] **Clientes:** Cadastro e gerenciamento de perfis de usuários.
  - [ ] **Entregas:** Integração e controle logístico (a implementar).

-----

## 🛠️ Contribuição

Contribuições são bem-vindas\! Se você quiser melhorar o projeto:

1.  Faça um **Fork** do projeto.
2.  Crie uma **Branch** para sua feature (`git checkout -b feature/nova-feature`).
3.  Faça o **Commit** (`git commit -m 'Adiciona nova feature'`).
4.  Faça o **Push** (`git push origin feature/nova-feature`).
5.  Abra um **Pull Request**.

-----

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](https://www.google.com/search?q=LICENSE) para mais detalhes.
