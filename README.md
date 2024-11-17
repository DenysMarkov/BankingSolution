# Banking Solutin

## Features
- Account management: create, view, list accounts.
- Transactions: deposit, withdraw, transfer funds.
- RESTful API with Swagger documentation.

## Setup
1. Clone the repository.
2. Start the application:
dotnet run --project BankingSolution.API
3. Open Swagger UI: `http://localhost:5209/swagger`.

## Explanation of Choosing Domain-Driven Design for the Task
Domain-Driven Design (DDD)

Why is DDD suitable for this task?
Domain-Driven Design focuses on building a system around the core business logic and the domain model. For banking applications, key aspects include:

- Complex domain logic: Banking involves strict handling of monetary transactions, which must be accurately processed and validated (e.g., managing balances, validating transactions). DDD helps focus on such details.
- Clear structure: The clear separation into layers (Domain, Application, Infrastructure, API) simplifies code understanding and system evolution. For example, operations like deposits or transfers naturally fit into domain logic.
- Scalability: DDD simplifies adding new business features, such as loans or fee calculations, without breaking the existing architecture.

Examples of DDD in the solution:

- Entities: Account isolate business logic and ensure data integrity.
- Interfaces: IAccountRepository abstracts data access so that the domain layer does not depend on the infrastructure.