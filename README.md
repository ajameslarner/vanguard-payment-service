# Vanguard Payment Service

A modern .NET 8 payment processing service built with clean architecture principles, demonstrating enterprise-grade patterns and best practices.

## 🏗️ Architecture Overview

This solution implements **Domain-Driven Design (DDD)** with clean architecture principles, organizing code into distinct layers: 

- **Domain**: Core business entities, enums, and contracts

- **Infrastructure**: Data access, repositories, and external integrations

- **Common**: Application services, handlers, and business logic

- **Internal**: Web API controllers, middleware, and configuration

## 🎯 Technology Stack

### .NET 8 (LTS)

.NET 8 was chosen as the target framework because:

- **Long-Term Support (LTS)**: Provides 3 years of support, ensuring stability for production systems

- **Performance**: Significant performance improvements over previous versions

- **Modern C# Features**: Access to latest language features and syntax improvements 

- **Container Optimization**: Enhanced container performance and smaller image sizes

### Core Technologies

#### 🔄 **MediatR**
**Purpose**: Implements the mediator pattern for decoupled request/response handling

**Benefits**:
- Separates business logic from controller concerns
- Enables cross-cutting concerns through behaviours (logging, validation, etc.)
- Simplifies testing by providing clear command/query separation (CQRS)

```csharp
// Example usage in controller
var response = await _mediator.Send(new MakePaymentCommand(request));
```

#### 🗺️ **AutoMapper**
**Purpose**: Object-to-object mapping between DTOs and entities

**Benefits**:
- Reduces boilerplate mapping code
- Centralizes mapping configuration
- Type-safe transformations between layers

```csharp
// Automatic mapping from entity to model
return _mapper.Map<PaymentModel>(paymentEntity);
```

#### 🔐 **Azure AD/JWT Authentication**
**Purpose**: Enterprise-grade authentication and authorization

**Benefits**:
- Seamless integration with Microsoft identity platform
- JWT token-based authentication for stateless APIs
- Role-based access control (RBAC) capabilities

*Note: Currently commented out for demonstration purposes*

#### 📊 **OpenTelemetry (OTEL)**
**Purpose**: Observability and monitoring through distributed tracing

**Benefits**:
- Application performance monitoring (APM)
- Distributed request tracing across microservices
- Integration with popular monitoring platforms
- Custom metrics and logging instrumentation

```csharp
// Custom metric tracking
_meter.RecordPaymentSuccess();
_meter.RecordPaymentFailure();
```

#### 📚 **Swagger/OpenAPI**
**Purpose**: API documentation and interactive testing interface

**Benefits**:
- Auto-generated API documentation
- Interactive testing environment
- Client SDK generation capabilities
- Schema validation and examples

#### 🗄️ **Entity Framework Core (EFCore)**
**Purpose**: Object-relational mapping (ORM) and data access

**Benefits**:
- Code-first database development
- LINQ query capabilities
- Migration management
- Multiple database provider support (In-Memory for demo, easily configurable for SQL Server, PostgreSQL etc)

#### 🛡️ **Custom Middleware**
**Purpose**: Cross-cutting concerns and request/response processing

**Implementation**:
- **Exception Middleware**: Global error handling and logging
- **Headers Middleware**: Security headers and CORS configuration
- **HTTP Logging**: Request/response logging for audit trails

#### 🏥 **Health Checks**
**Purpose**: Application and dependency health monitoring

**Benefits**:
- Kubernetes/container orchestration integration
- Dependency health verification
- Automated failover and scaling decisions

#### 🎮 **Controllers vs Minimal APIs**
**Choice Rationale**: Choosing traditional controllers over minimal APIs because:

- **Enterprise Readiness**: Better suited for complex business applications
- **Attribute-Based Routing**: More explicit and maintainable routing configuration
- **Swagger Integration**: Superior documentation generation
- **Action Filters**: Easier implementation of cross-cutting concerns
- **Model Binding**: More robust parameter binding and validation

#### 🐳 **Containerization (Docker)**
**Purpose**: Consistent deployment and environment management

**Benefits**:
- Environment parity between development and production
- Easy scaling and orchestration
- Dependency isolation
- Cloud-native deployment readiness

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- Docker (optional)
- Visual Studio 2022 or VS Code

### Running the Application

1. **Clone the repository**
```bash
git clone <repository-url>
cd vanguard-payment-service
```

2. **Restore dependencies**
```bash
dotnet restore
```

3. **Run the application**
```bash
cd src/Internal
dotnet run
```

4. **Access Swagger UI**
Navigate to: `https://localhost:7132/swagger` or `http://localhost:5050/swagger`

### 🏦 Seeded Test Accounts

The application comes with pre-seeded test accounts for demonstration:

*Note: The application uses an InMemory database for demonstration purposes*

| Account Number | Status | Balance | Allowed Payment Schemes | Description |
|---|---|---|---|---|
| `ACC1_disabled` | Disabled | £2,000,000.00 | None | Disabled account for testing error scenarios |
| `ACC2_enabled_bacs_fast` | Live | £2,000.00 | Bacs, FasterPayments | Multi-scheme account |
| `ACC3_enabled_bacs_chaps` | Live | £2,000.00 | Bacs, Chaps | High-value payment capable |
| `ACC4_enabled_bacs` | Live | £2,000.00 | Bacs only | Basic payment account |
| `ACC5_enabled_fast` | Live | £2,000.00 | FasterPayments only | Fast payment specialist |

### 📋 API Endpoints

#### **POST** `/api/payments` - Make Payment
Process a payment between accounts

**Request Body:**
```json
{
  "debtorAccountNumber": "ACC2_enabled_bacs_fast",
  "creditorAccountNumber": "ACC4_enabled_bacs",
  "amount": 100.50,
  "paymentScheme": 1
}
```

**Payment Schemes:**
- `0` - FasterPayments (Real-time payments)
- `1` - Bacs (Banker's Automated Clearing Services)
- `2` - Chaps (Same-day high-value payments)

#### **GET** `/api/payments/{transactionId}` - Get Payment
Retrieve payment details by transaction ID

#### **GET** `/api/accounts/{accountNumber}` - Get Account
Retrieve account information

#### **GET** `/health` - Health Check
Application health status

### 🧪 Test Scenarios

#### **Scenario 1: Successful Bacs Payment**
```json
POST /api/payments
{
  "debtorAccountNumber": "ACC2_enabled_bacs_fast",
  "creditorAccountNumber": "ACC4_enabled_bacs",
  "amount": 50.00,
  "paymentScheme": 1
}
```
Expected: ✅ Success - Both accounts support Bacs

#### **Scenario 2: Payment Scheme Not Supported**
```json
POST /api/payments
{
  "debtorAccountNumber": "ACC4_enabled_bacs",
  "creditorAccountNumber": "ACC5_enabled_fast",
  "amount": 50.00,
  "paymentScheme": 0
}
```
Expected: ❌ Failure - ACC4 doesn't support FasterPayments

#### **Scenario 3: Insufficient Funds**
```json
POST /api/payments
{
  "debtorAccountNumber": "ACC2_enabled_bacs_fast",
  "creditorAccountNumber": "ACC4_enabled_bacs",
  "amount": 5000.00,
  "paymentScheme": 1
}
```
Expected: ❌ Failure - Amount exceeds available balance

#### **Scenario 4: Disabled Account**
```json
POST /api/payments
{
  "debtorAccountNumber": "ACC1_disabled",
  "creditorAccountNumber": "ACC4_enabled_bacs",
  "amount": 50.00,
  "paymentScheme": 1
}
```
Expected: ❌ Failure - Source account is disabled

#### **Scenario 5: Successful High-Value Chaps Payment**
```json
POST /api/payments
{
  "debtorAccountNumber": "ACC3_enabled_bacs_chaps",
  "creditorAccountNumber": "ACC4_enabled_bacs",
  "amount": 1500.00,
  "paymentScheme": 2
}
```
Expected: ✅ Success - Debtor (ACC3) supports Chaps and has sufficient funds

### 🔧 Configuration

Key configuration in `appsettings.json` to switch database contexts:

```json
{
  "Database": {
    "Name": "Vanguard",
    "DataStoreType": "backup"  // "main" or "backup"
  }
}
```

### 🐳 Docker Deployment

Build and run with Docker:

```bash
docker build -t vanguard-payment-service .
docker run -p 8080:8080 vanguard-payment-service
```

### 🧪 Running Tests

```bash
cd tests/UnitTests
dotnet test
```

The test suite includes:
- Unit tests for payment strategies
- Service layer testing with mocking
- Controller testing
- Extension method validation
- Factory pattern verification

---

## 📚 Additional Resources

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/tags/domain%20driven%20design.html)
- [OpenTelemetry .NET Documentation](https://opentelemetry.io/docs/instrumentation/net/)
- [MediatR Documentation](https://github.com/jbogard/MediatR)