# ExamiNation

**ExamiNation** is a powerful web application built with **.NET Core** that demonstrates modern architectural practices, including **Domain-Driven Design (DDD)**, **Identity**, and **Microservices**. The application supports test creation, role management, user authentication, password reset, and is fully containerized with Docker.

This project is ideal for showcasing clean architecture, security practices with **ASP.NET Identity**, and robust CI/CD pipelines for automated testing.

![image](https://github.com/user-attachments/assets/771820b0-fdc2-44d8-a79a-0f7a3f9c1406)


---

### Key Features:

- **User Management**: User registration, authentication, and password recovery using **ASP.NET Identity** with token-based authentication for API security.
- **Test Creation**: Users can create different types of tests such as **personality**, **IQ**, and more.
- **Role Management**: Admins and Test Creators can assign specific roles to users to control access and permissions within the application.
- **Global Exception Handling**: A centralized error handling system ensures consistent error responses throughout the application.
- **API Documentation**: Easily explore the application's API through **Swagger UI**.
- **Docker Support**: The application and **SQL Server** database are containerized using Docker for streamlined development and deployment.
- **CI/CD Pipeline**: Automated testing, building, and deployment using **GitHub Actions**.
- **Improved Performance**: Repository methods optimized with `AsNoTracking()` for better performance in read-only queries.
- **Seed Service**: The application includes a **Seed Service** that populates the database with initial test data and classifications based on scores, available in both **English** and **Spanish**.

---

### Technologies Used:

- **.NET Core**: The core framework for building cross-platform web applications.
- **ASP.NET Identity**: For user authentication, including registration, login, and password management.
- **Entity Framework Core**: For seamless data access and ORM functionality.
- **AutoMapper**: Simplifies object-to-object mapping, keeping code maintainable and clean.
- **Dependency Injection**: Implements loose coupling for better testability and scalability.
- **FluentAssertions**: A library for writing readable and expressive assertions in tests.
- **Docker**: Used to containerize both the application and the SQL Server database, ensuring a consistent environment for development.
- **CI/CD (GitHub Actions)**: Continuous Integration and Continuous Deployment pipeline for automated testing and deployment.
- **Swagger UI**: Auto-generated documentation for the application's API endpoints.
- **Nito.AsyncEx**: For enhancing asynchronous operations and simplifying concurrent task management.

---

### Setup and Installation:

Follow these steps to set up the application on your local machine using Docker:

#### Prerequisites:

1. **Docker**: Ensure Docker is installed and running.
2. **.NET SDK**: Ensure the .NET 8.0 SDK or later is installed on your machine.

#### Steps to Run the Application:

1. Clone the repository:

    ```bash
    git clone https://github.com/Taday97/ExamiNation.git
    cd ExamiNation
    ```

2. Build and start the Docker containers for the application and database:

    ```bash
    docker-compose up --build
    ```

    This will:
    - Build and start the application.
    - Set up the SQL Server database in a Docker container.
    - Automatically connect the application to the database.

3. **Access the Application**: Once the containers are running, navigate to `http://localhost:5000/swagger/index.html` to view the **Swagger UI**, which allows you to interact with the API.

---

### Seed Service for Initial Data:

The application includes a **Seed Service** that automatically populates the database with initial test data and classifications based on scores. This data is available in both **English** and **Spanish**. The `Seed` functionality allows you to:

1. Populate the **test questions**.
2. Add **classifications** according to the score ranges in both languages.
3. Seed both English and Spanish versions of the classifications.

To use the Seed service, simply run the application and the data will be added to the database upon startup.

---

### CI/CD Pipeline:

The project includes a CI/CD pipeline set up using **GitHub Actions**. This pipeline automatically:

1. **Builds and Tests** the application.
2. **Deploys** it (future configurations can be added for deployment to a server or cloud platform).

You can view the CI/CD configuration in the `.github/workflows` directory.
