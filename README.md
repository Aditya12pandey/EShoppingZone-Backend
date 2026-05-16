<div align="center">

<img src="https://img.shields.io/badge/EShoppingZone-E--Commerce%20Platform-4f46e5?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0id2hpdGUiIGQ9Ik03IDEzYzEuNjYgMCAzLTEuMzQgMy0zUzguNjYgNyA3IDcgNCA4LjM0IDQgMTBzMS4zNCAxIDMgM3ptMCAyYy0yLjMzIDAtNyAxLjE3LTcgMy41VjIwaDJ2LTEuNWMwLS44NSAzLjE3LTIgNS0yIDEuMDMgMCAyLjY4LjM5IDMuOTkgMUw3IDE1em0xMC0yYzEuNjYgMCAzLTEuMzQgMy0zcy0xLjM0LTMtMy0zLTMgMS4zNC0zIDMgMS4zNCAzIDMgM3ptMCAyYy0yLjMzIDAtNyAxLjE3LTcgMy41VjIwaDJ2LTEuNWMwLS44NSAzLjE3LTIgNS0yIDEuODMgMCA1IDEuMTUgNSAyVjIwaDJ2LTEuNWMwLTIuMzMtNC42Ny0zLjUtNy0zLjV6Ii8+PC9zdmc+" />

# EShoppingZone

### A Production-Grade E-Commerce Platform Built on Microservices

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-21-DD0031?style=flat-square&logo=angular)](https://angular.dev/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?style=flat-square&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Razorpay](https://img.shields.io/badge/Razorpay-Payment%20Gateway-02042B?style=flat-square&logo=razorpay&logoColor=white)](https://razorpay.com/)
[![NUnit](https://img.shields.io/badge/NUnit-53%20Tests-22c55e?style=flat-square&logo=dotnet)](https://nunit.org/)
[![Render](https://img.shields.io/badge/Render-Deployed-46E3B7?style=flat-square&logo=render&logoColor=white)](https://render.com/)
[![Neon](https://img.shields.io/badge/Neon-PostgreSQL-00E599?style=flat-square&logo=postgresql&logoColor=white)](https://neon.tech/)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)

<p>EShoppingZone is a modern, scalable e-commerce platform engineered with a full microservices architecture — enabling customers to browse products, manage carts, place orders, and pay via COD, E-Wallet, or Razorpay online payment at scale.</p>

[Architecture](#-system-architecture) · [Microservices](#-microservices-overview) · [API Docs](#-api-reference) · [Setup](#-getting-started) · [Testing](#-testing)

---

</div>

## 🚀 Live Demo

| Service         | URL                                                |
| --------------- | -------------------------------------------------- |
| **Frontend**    | https://eshoppingzone-frontend-bwfa.onrender.com   |
| **Profile API** | https://eshoppingzone-profile.onrender.com/swagger |
| **Product API** | https://eshoppingzone-product.onrender.com/swagger |
| **Cart API**    | https://eshoppingzone-cart.onrender.com/swagger    |
| **Order API**   | https://eshoppingzone-order.onrender.com/swagger   |
| **Wallet API**  | https://eshoppingzone-wallet.onrender.com/swagger  |

### Admin Credentials (seeded automatically)

```
Email:    admin@eshoppingzone.com
Password: Admin@123
```

---

## 📋 Table of Contents

- [Tech Stack](#-tech-stack)
- [Architecture Overview](#-system-architecture)
- [UML Diagrams](#-uml-diagrams)
  - [Architecture Diagram](#1-system-architecture-diagram)
  - [Database Schema](#2-database-schema-diagram)
  - [Design Flow](#3-design-flow-diagram)
  - [Order Payment Sequence](#4-order-payment-flow)
  - [Saga Pattern Flow](#5-saga-pattern--compensating-transaction)
  - [Inter-Service Communication](#6-inter-service-communication-map)
- [Microservices Overview](#-microservices-overview)
- [Core Features](#-core-features)
- [Database Schema](#-database-schema)
- [API Reference](#-api-reference)
- [Design Patterns](#-key-design-patterns)
- [Getting Started](#-getting-started)
- [Testing](#-testing)

---

## 🛠 Tech Stack

| Layer                  | Technology                               | Purpose                                   |
| ---------------------- | ---------------------------------------- | ----------------------------------------- |
| **Backend**            | ASP.NET Core 8 Web API                   | 5 independent microservices               |
| **Frontend**           | Angular 21                               | Single-page application                   |
| **Database**           | PostgreSQL 16 (Neon.tech)                | Per-service isolated databases            |
| **ORM**                | Entity Framework Core 8                  | Code-first migrations & data access       |
| **Auth**               | JWT Bearer (HS256)                       | Stateless authentication, shared secret   |
| **Password**           | PasswordHasher\<T\> (PBKDF2+HMAC-SHA256) | Secure password hashing                   |
| **Payment**            | Razorpay                                 | COD · E-Wallet · Online (UPI/Card)        |
| **HTTP Clients**       | IHttpClientFactory                       | Typed inter-service HTTP calls            |
| **Resilience**         | Saga Pattern (Choreography)              | Distributed transaction compensation      |
| **Validation**         | Data Annotations + Regex                 | Input validation across all DTOs          |
| **Exception Handling** | Global Exception Middleware              | Safe error logging + clean JSON responses |
| **Testing**            | NUnit + Moq + FluentAssertions           | 53 unit tests                             |
| **Deployment**         | Render.com                               | Cloud production environment              |

---

## 🏗 System Architecture

EShoppingZone follows a **Microservices Architecture** with these core principles:

| Pattern                                 | Applied Where                                   |
| --------------------------------------- | ----------------------------------------------- |
| ✅ **Microservices**                    | 5 independently deployable services             |
| ✅ **Repository + Service Layer**       | Clean separation in every microservice          |
| ✅ **Dependency Injection (AddScoped)** | All services via ASP.NET Core DI                |
| ✅ **JWT Shared Secret**                | Token validated locally in all 5 APIs           |
| ✅ **Saga Pattern (Choreography)**      | Wallet deduction + order save with compensation |
| ✅ **EF Core Transactions**             | Atomic balance mutations in Wallet API          |
| ✅ **Owned Entity (OwnsOne)**           | DeliveryAddress embedded in Orders table        |
| ✅ **JSON HasConversion**               | Dictionary/IList stored as JSON in Product      |
| ✅ **Global Exception Middleware**      | Logs internally, returns safe JSON to client    |
| ✅ **DB-Per-Service**                   | Strict data isolation across all 5 services     |

---

## 📐 UML Diagrams

### 1. System Architecture Diagram

> Full deployment view — Angular Frontend → 5 Microservices → PostgreSQL Databases

```mermaid
graph TB
    subgraph CLIENT["🌐 Client Layer"]
        WEB["Angular 21 Frontend\neshoppingzone-frontend-bwfa.onrender.com"]
    end

    subgraph INTERCEPTOR["⚡ Angular Layer"]
        INT["JWT Interceptor\nRole Guards (auth.guard / role.guard)"]
    end

    subgraph SERVICES["⚙️ Microservices Layer — Render.com"]
        PROFILE["Profile API\n:5001\nJWT · PasswordHasher"]
        PRODUCT["Product API\n:5002\nCatalogue · MerchantId FK"]
        CART["Cart API\n:5003\nCartId=UserId · CartTotal()"]
        ORDER["Order API\n:5004\nSaga · Razorpay · IHttpClientFactory"]
        WALLET["Wallet API\n:5005\nEWallet · Statements · Razorpay"]
    end

    subgraph INFRA["🗄️ Infrastructure — Neon.tech"]
        PG1[("profile db\nPostgreSQL")]
        PG2[("product db\nPostgreSQL")]
        PG3[("cart db\nPostgreSQL")]
        PG4[("order db\nPostgreSQL")]
        PG5[("wallet db\nPostgreSQL")]
    end

    subgraph PAY["💳 Payment Gateway"]
        RAZOR["Razorpay\nCOD · EWALLET · ONLINE"]
    end

    WEB --> INT
    INT --> PROFILE & PRODUCT & CART & ORDER & WALLET

    ORDER -->|"HTTP IHttpClientFactory\nSaga Pattern"| WALLET
    ORDER --> RAZOR
    WALLET --> RAZOR

    PROFILE --> PG1
    PRODUCT --> PG2
    CART --> PG3
    ORDER --> PG4
    WALLET --> PG5

    style WEB fill:#DD0031,color:#fff,stroke:#DD0031
    style INT fill:#c2185b,color:#fff,stroke:#c2185b
    style PROFILE fill:#059669,color:#fff,stroke:#059669
    style PRODUCT fill:#4f46e5,color:#fff,stroke:#4f46e5
    style CART fill:#d97706,color:#fff,stroke:#d97706
    style ORDER fill:#dc2626,color:#fff,stroke:#dc2626
    style WALLET fill:#0284c7,color:#fff,stroke:#0284c7
    style PG1 fill:#336791,color:#fff,stroke:#336791
    style PG2 fill:#336791,color:#fff,stroke:#336791
    style PG3 fill:#336791,color:#fff,stroke:#336791
    style PG4 fill:#336791,color:#fff,stroke:#336791
    style PG5 fill:#336791,color:#fff,stroke:#336791
    style RAZOR fill:#02042B,color:#fff,stroke:#02042B
```

---

### 2. Database Schema Diagram

> Entity relationships across all 5 isolated databases

```mermaid
erDiagram
    UserProfile ||--o{ Address : has
    UserProfile {
        int ProfileId PK
        string FullName
        string EmailId
        long MobileNumber
        string Role
        string Password
        string Gender
        string Image
        string About
        datetime DateOfBirth
    }
    Address {
        int AddressId PK
        string HouseNumber
        string StreetName
        string ColonyName
        string City
        string State
        string Pincode
        int ProfileId FK
    }
    ProductEntity {
        int ProductId PK
        string ProductType
        string ProductName
        string Category
        string Rating
        string Review
        string Image
        decimal Price
        string Description
        string Specification
        int MerchantId
    }
    CartEntity ||--o{ CartItemEntity : contains
    CartEntity {
        int CartId PK
        decimal TotalPrice
    }
    CartItemEntity {
        int CartItemId PK
        int ProductId
        string ProductName
        decimal Price
        int Quantity
        int CartId FK
    }
    OrderEntity {
        int OrderId PK
        datetime OrderDate
        int CustomerId
        int MerchantId
        decimal AmountPaid
        string ModeOfPayment
        string OrderStatus
        int Quantity
        string ProductName
        int ProductId
        string Address_FullName
        string Address_City
        string Address_Pincode
    }
    EWallet ||--o{ Statement : has
    EWallet {
        int WalletId PK
        decimal CurrentBalance
    }
    Statement {
        int StatementId PK
        string TransactionType
        decimal Amount
        datetime DateTime
        int OrderId
        string TransactionRemarks
        int WalletId FK
    }
```

---

### 3. Design Flow Diagram

> Complete user journey for all three roles

```mermaid
graph TD
    START(["Login / Register\nProfile API · JWT issued"])

    START --> CUSTOMER["👤 Customer\nCUSTOMER role"]
    START --> MERCHANT["🏪 Merchant\nMERCHANT role"]
    START --> ADMIN["🔑 Admin\nADMIN role (seeded)"]

    CUSTOMER --> BROWSE["Browse Products\nProduct API · public"]
    BROWSE --> CART["Add to Cart\nCart API · CartId=UserId"]
    CART --> CHECKOUT["Checkout\nAddress + Payment Mode"]
    CHECKOUT --> COD["COD\nPlace Order directly"]
    CHECKOUT --> EWALLET["E-Wallet\nDeduct from wallet balance"]
    CHECKOUT --> ONLINE["Online\nRazorpay Popup"]
    COD & EWALLET & ONLINE --> ORDER_PLACED["Order Placed\nOrder API · Saga Pattern"]
    ORDER_PLACED --> TRACK["Track Order\nPlaced→Shipped→Delivered"]
    CUSTOMER --> WALLET_DASH["Wallet Dashboard\nWallet API · statements"]

    MERCHANT --> ADD_PROD["Add Product\nProduct API · MerchantId FK"]
    ADD_PROD --> EDIT_PROD["Update / Delete\n403 if not owner"]
    MERCHANT --> VIEW_ORDERS["View Own Orders\nOrder API · MerchantId"]

    ADMIN --> MANAGE_USERS["Manage Users\nProfile API · all profiles"]
    ADMIN --> CHANGE_STATUS["Change Order Status\nPlaced→Shipped→Delivered→Cancelled"]
    ADMIN --> VIEW_WALLETS["View All Wallets\nWallet API · statements"]

    style START fill:#6c63ff,color:#fff,stroke:#6c63ff
    style CUSTOMER fill:#059669,color:#fff,stroke:#059669
    style MERCHANT fill:#d97706,color:#fff,stroke:#d97706
    style ADMIN fill:#dc2626,color:#fff,stroke:#dc2626
    style COD fill:#22c55e,color:#fff,stroke:#22c55e
    style EWALLET fill:#0284c7,color:#fff,stroke:#0284c7
    style ONLINE fill:#db2777,color:#fff,stroke:#db2777
    style ORDER_PLACED fill:#4f46e5,color:#fff,stroke:#4f46e5
```

---

### 4. Order Payment Flow

> Sequence diagram — how a customer places an order with Razorpay

```mermaid
sequenceDiagram
    actor Customer
    participant FE as Angular Frontend
    participant ORDER as Order API
    participant WALLET as Wallet API
    participant RAZOR as Razorpay

    Customer->>FE: Click "Place Order"
    FE->>ORDER: POST /api/orders/initiatePayment {amount, currency}
    ORDER->>RAZOR: Create Razorpay Order
    RAZOR-->>ORDER: {razorpayOrderId, keyId}
    ORDER-->>FE: {razorpayOrderId, keyId, amount}

    FE->>FE: Open Razorpay Checkout Popup

    alt Payment Successful
        Customer->>RAZOR: Pay via UPI / Card
        RAZOR-->>FE: {razorpayOrderId, paymentId, signature}
        FE->>ORDER: POST /api/orders/verifyAndPlace {signature, address, ...}
        ORDER->>ORDER: Utils.verifyPaymentSignature()
        ORDER->>ORDER: Save OrderEntity (ModeOfPayment = ONLINE)
        ORDER-->>FE: 200 OK — Order Placed ✅
    else Payment Cancelled
        Customer->>FE: Cancel
        FE-->>Customer: Nothing happens ✅
    end
```

---

### 5. Saga Pattern — Compensating Transaction

> How EShoppingZone handles distributed transaction failure between Order and Wallet APIs

```mermaid
sequenceDiagram
    actor Customer
    participant ORDER as Order API
    participant WALLET as Wallet API

    Customer->>ORDER: POST /api/orders/place (EWALLET)
    ORDER->>WALLET: POST /api/wallet/payMoney (IHttpClientFactory)

    alt Insufficient Balance
        WALLET-->>ORDER: 400 Bad Request
        ORDER-->>Customer: 400 — Insufficient wallet balance
        Note over ORDER: walletDeducted = false — No compensation needed
    else Wallet Deducted Successfully
        WALLET-->>ORDER: 200 OK
        Note over ORDER: walletDeducted = true
        ORDER->>ORDER: BeginTransactionAsync()
        ORDER->>ORDER: Save OrderEntity to DB

        alt Order Save Fails
            ORDER->>ORDER: RollbackAsync()
            Note over ORDER: walletDeducted == true → Compensate!
            ORDER->>WALLET: POST /api/wallet/refund (Compensating Transaction)
            WALLET->>WALLET: CurrentBalance += amount
            WALLET->>WALLET: CREDIT Statement persisted
            WALLET-->>ORDER: Refund Done ✅
            ORDER-->>Customer: 400 — Order failed, wallet refunded
        else Order Save Succeeds
            ORDER->>ORDER: CommitAsync()
            ORDER-->>Customer: 200 OK — Order Placed ✅
        end
    end
```

---

### 6. Inter-Service Communication Map

> All synchronous HTTP calls between services

```mermaid
graph LR
    CUSTOMER(["👤 Customer"])
    ORDER["Order API"]
    WALLET["Wallet API"]
    PROFILE["Profile API"]
    PRODUCT["Product API"]
    CART["Cart API"]
    RAZOR["Razorpay"]

    CUSTOMER -->|"JWT Bearer"| PROFILE
    CUSTOMER -->|"JWT Bearer"| PRODUCT
    CUSTOMER -->|"JWT Bearer"| CART
    CUSTOMER -->|"JWT Bearer"| ORDER
    CUSTOMER -->|"JWT Bearer"| WALLET

    ORDER -->|"HTTP IHttpClientFactory\nPOST /api/wallet/payMoney\nSaga: POST /api/wallet/refund"| WALLET
    ORDER -->|"POST /api/orders/initiatePayment\nPOST /api/orders/verifyAndPlace"| RAZOR
    WALLET -->|"POST /api/wallet/initiateTopUp\nPOST /api/wallet/verifyAndAdd"| RAZOR

    style CUSTOMER fill:#059669,color:#fff,stroke:#059669
    style ORDER fill:#dc2626,color:#fff,stroke:#dc2626
    style WALLET fill:#0284c7,color:#fff,stroke:#0284c7
    style PROFILE fill:#059669,color:#fff,stroke:#059669
    style PRODUCT fill:#4f46e5,color:#fff,stroke:#4f46e5
    style CART fill:#d97706,color:#fff,stroke:#d97706
    style RAZOR fill:#02042B,color:#fff,stroke:#02042B
```

**Synchronous (HTTP + IHttpClientFactory):**

```
Order.API   → Wallet.API   (POST /api/wallet/payMoney — EWALLET payment)
Order.API   → Wallet.API   (POST /api/wallet/refund — Saga compensation)
Order.API   → Razorpay     (initiatePayment + verifyAndPlace)
Wallet.API  → Razorpay     (initiateTopUp + verifyAndAdd)
```

---

## 📦 Microservices Overview

| Service         | Port | Responsibility                                                      |
| --------------- | ---- | ------------------------------------------------------------------- |
| **Profile.API** | 5001 | User registration, login, JWT generation, address management        |
| **Product.API** | 5002 | Product catalogue CRUD, category/type filtering, merchant ownership |
| **Cart.API**    | 5003 | Shopping cart, CartId=UserId, CartTotal() LINQ Sum                  |
| **Order.API**   | 5004 | Order lifecycle, Saga Pattern, Razorpay integration                 |
| **Wallet.API**  | 5005 | E-Wallet balance, atomic CREDIT/DEBIT, Razorpay top-up              |

---

## 🎯 Core Features

<details>
<summary><strong>👤 Profile & Auth</strong></summary>

- Register as Customer or Merchant via email/password
- Login with JWT token (24-hour expiry)
- Role-based access control — CUSTOMER / MERCHANT / ADMIN
- Admin seeded automatically at startup — no registration
- Profile management — bio, name, mobile, address
- Password hashing via PasswordHasher\<T\> (PBKDF2+HMAC-SHA256)
- Shared JWT secret — validated locally in all 5 APIs (no Profile API call per request)

</details>

<details>
<summary><strong>📦 Products</strong></summary>

- Add products with type, name, category, price, images, specifications
- MerchantId stored on entity — HTTP 403 if non-owner tries to update/delete
- Complex types (Dictionary, IList) stored as JSON via EF Core HasConversion
- Filter by category, type, or search by name
- Public access — no login required to browse

</details>

<details>
<summary><strong>🛒 Cart</strong></summary>

- CartId == UserId — O(1) cart lookup per authenticated user
- Add, remove, and clear cart items
- CartTotal() = `Items.Sum(i => i.Price * i.Quantity)`
- ReferenceHandler.IgnoreCycles prevents circular JSON serialization

</details>

<details>
<summary><strong>📋 Orders · 💳 Payments</strong></summary>

**Orders:** Place order with delivery address, view by customer/merchant, track status

**Payment Modes:**

- **COD** — Cash on Delivery, no payment at order time
- **EWALLET** — Deduct from internal wallet balance (Saga Pattern protects atomicity)
- **ONLINE** — Razorpay checkout popup (UPI, Card, Net Banking)

**Order Status Flow:** `Placed → Shipped → Delivered → Cancelled` (Admin only)

</details>

<details>
<summary><strong>💰 Wallet · 🔁 Saga Pattern</strong></summary>

**Wallet:** Create wallet (WalletId = UserId), add money manually or via Razorpay, view statements

**Saga Pattern (Choreography):**

- `walletDeducted` flag tracks whether wallet was debited
- If order save fails after wallet deduction → `POST /api/wallet/refund` called automatically
- All balance mutations wrapped in `BeginTransactionAsync()` for atomicity
- Every CREDIT/DEBIT persisted as Statement entity with remarks

</details>

---

## 🗄 Database Schema

Each microservice owns its own isolated PostgreSQL database — no shared DB, no cross-service joins.

| Service         | Database                | Key Tables                  | Notes                                               |
| --------------- | ----------------------- | --------------------------- | --------------------------------------------------- |
| **Profile.API** | `eshoppingzone_profile` | `UserProfiles`, `Addresses` | Email unique index, FK on Address                   |
| **Product.API** | `eshoppingzone_product` | `Products`                  | JSON columns for Dictionary/IList via HasConversion |
| **Cart.API**    | `eshoppingzone_cart`    | `Carts`, `CartItems`        | CartId = UserId, FK on CartItems                    |
| **Order.API**   | `eshoppingzone_order`   | `Orders`                    | DeliveryAddress as owned entity (OwnsOne)           |
| **Wallet.API**  | `eshoppingzone_wallet`  | `EWallets`, `Statements`    | FK Statements→EWallets                              |

---

## 🌐 API Reference

<details>
<summary><strong>🔑 Profile Endpoints — <code>/api/profiles</code></strong></summary>

| Method   | Endpoint                            | Auth  | Description               |
| -------- | ----------------------------------- | :---: | ------------------------- |
| `POST`   | `/api/profiles/register/customer`   |  ❌   | Register customer         |
| `POST`   | `/api/profiles/register/merchant`   |  ❌   | Register merchant         |
| `POST`   | `/api/profiles/login`               |  ❌   | Login — returns JWT token |
| `GET`    | `/api/profiles`                     | ADMIN | Get all profiles          |
| `GET`    | `/api/profiles/{id}`                |  ✅   | Get profile by ID         |
| `PUT`    | `/api/profiles/update`              |  ✅   | Update profile            |
| `DELETE` | `/api/profiles/{id}`                | ADMIN | Delete profile            |
| `POST`   | `/api/profiles/address`             |  ✅   | Add address               |
| `GET`    | `/api/profiles/address/{profileId}` |  ✅   | Get addresses             |

</details>

<details>
<summary><strong>📦 Product Endpoints — <code>/api/products</code></strong></summary>

| Method   | Endpoint                       |   Auth   | Description                                   |
| -------- | ------------------------------ | :------: | --------------------------------------------- |
| `GET`    | `/api/products`                |    ❌    | Get all products                              |
| `GET`    | `/api/products/{id}`           |    ❌    | Get by ID                                     |
| `GET`    | `/api/products/name/{name}`    |    ❌    | Search by name                                |
| `GET`    | `/api/products/category/{cat}` |    ❌    | Filter by category                            |
| `GET`    | `/api/products/type/{type}`    |    ❌    | Filter by type                                |
| `POST`   | `/api/products`                | MERCHANT | Add product                                   |
| `PUT`    | `/api/products`                | MERCHANT | Update product (owner only — 403 if mismatch) |
| `DELETE` | `/api/products/{id}`           | MERCHANT | Delete product (owner only)                   |

</details>

<details>
<summary><strong>🛒 Cart · 📋 Order · 💰 Wallet Endpoints</strong></summary>

**Cart — `/api/carts`**

| Method   | Endpoint                     |   Auth   | Description                   |
| -------- | ---------------------------- | :------: | ----------------------------- |
| `POST`   | `/api/carts/create`          | CUSTOMER | Create cart (CartId = UserId) |
| `GET`    | `/api/carts/{id}`            |    ✅    | Get cart by ID                |
| `GET`    | `/api/carts`                 |  ADMIN   | Get all carts                 |
| `POST`   | `/api/carts/addItem`         | CUSTOMER | Add item to cart              |
| `DELETE` | `/api/carts/removeItem/{id}` | CUSTOMER | Remove item from cart         |
| `DELETE` | `/api/carts/clear`           | CUSTOMER | Clear cart                    |

**Order — `/api/orders`**

| Method   | Endpoint                      |   Auth   | Description                    |
| -------- | ----------------------------- | :------: | ------------------------------ |
| `POST`   | `/api/orders/place`           | CUSTOMER | Place order (COD or EWALLET)   |
| `POST`   | `/api/orders/initiatePayment` | CUSTOMER | Initiate Razorpay order        |
| `POST`   | `/api/orders/verifyAndPlace`  | CUSTOMER | Verify signature + place order |
| `GET`    | `/api/orders`                 |  ADMIN   | Get all orders                 |
| `GET`    | `/api/orders/{id}`            |    ✅    | Get order by ID                |
| `GET`    | `/api/orders/customer/{id}`   | CUSTOMER | Get orders by customer         |
| `GET`    | `/api/orders/merchant/{id}`   | MERCHANT | Get orders by merchant         |
| `PUT`    | `/api/orders/status`          |  ADMIN   | Change order status            |
| `DELETE` | `/api/orders/{id}`            |    ✅    | Delete order                   |

**Wallet — `/api/wallet`**

| Method   | Endpoint                      |   Auth   | Description                          |
| -------- | ----------------------------- | :------: | ------------------------------------ |
| `POST`   | `/api/wallet/new`             | CUSTOMER | Create wallet                        |
| `GET`    | `/api/wallet/{id}`            |    ✅    | Get wallet by ID                     |
| `GET`    | `/api/wallet`                 |  ADMIN   | Get all wallets                      |
| `POST`   | `/api/wallet/addMoney`        | CUSTOMER | Add money manually                   |
| `POST`   | `/api/wallet/payMoney`        |    ✅    | Deduct from wallet                   |
| `POST`   | `/api/wallet/refund`          |    ✅    | Refund to wallet (Saga compensation) |
| `POST`   | `/api/wallet/initiateTopUp`   | CUSTOMER | Initiate Razorpay top-up             |
| `POST`   | `/api/wallet/verifyAndAdd`    | CUSTOMER | Verify + add money via Razorpay      |
| `GET`    | `/api/wallet/statements/{id}` |    ✅    | Get statements by wallet ID          |
| `GET`    | `/api/wallet/statements`      |  ADMIN   | Get all statements                   |
| `DELETE` | `/api/wallet/{id}`            |  ADMIN   | Delete wallet                        |

</details>

---

## 📊 Key Design Patterns

| Pattern                                      | Applied Where                                    |
| -------------------------------------------- | ------------------------------------------------ |
| Repository + Service Layer                   | All 5 APIs                                       |
| Dependency Injection (AddScoped)             | All services via ASP.NET Core DI                 |
| JWT Bearer (Shared Secret)                   | All 5 APIs — validated locally                   |
| EF Core Transactions (BeginTransactionAsync) | Wallet API + Order API                           |
| Owned Entity (OwnsOne)                       | Order API — DeliveryAddress embedded in Orders   |
| JSON HasConversion                           | Product API — Dictionary/IList as JSON strings   |
| Synchronous HTTP (IHttpClientFactory)        | Order API → Wallet API                           |
| Saga Pattern (Choreography)                  | Order API — wallet deduction + compensation      |
| Global Exception Middleware                  | All 5 APIs — logs internally, returns clean JSON |
| Data Annotations + Regex Validation          | All DTOs across all 5 APIs                       |

---

## 🔐 Security

- **JWT HS256** — Token signed with shared secret, 24-hour expiry
- **PasswordHasher\<T\>** — PBKDF2+HMAC-SHA256 password hashing
- **[Authorize(Roles)]** — Role-based protection on all sensitive endpoints
- **MerchantId Ownership** — HTTP 403 returned if merchant tries to edit another's product
- **Input Validation** — All DTOs validated with Data Annotations + Regex
- **EF Core Parameterized Queries** — Full SQL injection protection
- **Global Exception Middleware** — Stack traces never exposed to clients

---

## ⚡ Performance & Reliability

- **CartTotal LINQ** — `Items.Sum(i => i.Price * i.Quantity)` — computed in-memory, no round-trip
- **EF Core Transactions** — `BeginTransactionAsync()` for atomic wallet + statement operations
- **Saga Compensation** — Automatic refund if order save fails after wallet deduction
- **IgnoreCycles** — `ReferenceHandler.IgnoreCycles` prevents circular serialization in Cart + Wallet APIs
- **CartId = UserId** — O(1) cart lookup per user
- **Pagination** — List endpoints support pagination

---

## 📂 Project Structure

```
EShoppingZone/
│
├── EShoppingZone.Profile.API/
│   ├── Controllers/        ProfileController.cs
│   ├── Data/               ProfileDbContext.cs (seeds Admin)
│   ├── DTOs/               ProfileDtos.cs
│   ├── Entities/           UserProfile.cs, Address.cs
│   ├── Helpers/            JwtHelper.cs
│   ├── Middleware/         GlobalExceptionMiddleware.cs
│   ├── Repositories/       IProfileRepository.cs, ProfileRepository.cs
│   ├── Services/           IProfileService.cs, ProfileService.cs
│   ├── Program.cs
│   └── appsettings.json
│
├── EShoppingZone.Product.API/
│   ├── Entities/           ProductEntity.cs  ← renamed to avoid namespace conflict
│   ├── ...                 (same structure)
│   └── Program.cs
│
├── EShoppingZone.Cart.API/
│   ├── Entities/           CartEntity.cs, CartItemEntity.cs  ← renamed
│   ├── ...                 (same structure)
│   └── Program.cs          ← ReferenceHandler.IgnoreCycles
│
├── EShoppingZone.Order.API/
│   ├── Entities/           OrderEntity.cs, DeliveryAddress.cs  ← OwnsOne
│   ├── Services/           OrderService.cs  ← Saga Pattern + IHttpClientFactory
│   ├── ...                 (same structure)
│   └── Program.cs          ← AddHttpClient("WalletApi")
│
├── EShoppingZone.Wallet.API/
│   ├── Entities/           EWallet.cs, Statement.cs
│   ├── Services/           WalletService.cs  ← AddMoney, PayMoney, RefundMoney
│   ├── ...                 (same structure)
│   └── Program.cs          ← ReferenceHandler.IgnoreCycles
│
├── EShoppingZone.Profile.API.Tests/    → 11 tests ✅
├── EShoppingZone.Product.API.Tests/    → 10 tests ✅
├── EShoppingZone.Cart.API.Tests/       →  9 tests ✅
├── EShoppingZone.Order.API.Tests/      →  7 tests ✅
└── EShoppingZone.Wallet.API.Tests/     → 16 tests ✅
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 16+](https://www.postgresql.org/) or [Neon.tech](https://neon.tech) account
- [pgAdmin 4](https://www.pgadmin.org/) (optional)
- [Node.js 20+](https://nodejs.org/) (for frontend)

### Clone the Repository

```bash
git clone https://github.com/YOUR_USERNAME/EShoppingZone-Backend.git
cd EShoppingZone-Backend
```

### Configure Connection Strings

Update `appsettings.json` in each API project:

```json
{
  "ConnectionStrings": {
    "ProfileDb": "Host=localhost;Port=5432;Database=eshoppingzone_profile;Username=postgres;Password=YOUR_PASSWORD;"
  },
  "Jwt": {
    "Key": "EShoppingZone_SuperSecretKey_2026_DoNotShare_MinLength32Chars!",
    "Issuer": "EShoppingZone",
    "Audience": "EShoppingZoneClients",
    "ExpiryHours": "24"
  }
}
```

### Run Migrations

```powershell
cd EShoppingZone.Profile.API && dotnet ef migrations add InitialCreate && dotnet ef database update && cd ..
cd EShoppingZone.Product.API && dotnet ef migrations add InitialCreate && dotnet ef database update && cd ..
cd EShoppingZone.Cart.API    && dotnet ef migrations add InitialCreate && dotnet ef database update && cd ..
cd EShoppingZone.Order.API   && dotnet ef migrations add InitialCreate && dotnet ef database update && cd ..
cd EShoppingZone.Wallet.API  && dotnet ef migrations add InitialCreate && dotnet ef database update && cd ..
```

### Run All 5 APIs

Open 5 separate terminals:

```powershell
cd EShoppingZone.Profile.API && dotnet run --urls "http://localhost:5001"
cd EShoppingZone.Product.API && dotnet run --urls "http://localhost:5002"
cd EShoppingZone.Cart.API    && dotnet run --urls "http://localhost:5003"
cd EShoppingZone.Order.API   && dotnet run --urls "http://localhost:5004"
cd EShoppingZone.Wallet.API  && dotnet run --urls "http://localhost:5005"
```

### Swagger URLs

| API     | URL                           |
| ------- | ----------------------------- |
| Profile | http://localhost:5001/swagger |
| Product | http://localhost:5002/swagger |
| Cart    | http://localhost:5003/swagger |
| Order   | http://localhost:5004/swagger |
| Wallet  | http://localhost:5005/swagger |

---

## 🧪 Testing

```bash
# Run all 53 tests
cd EShoppingZone
dotnet test

# Run specific test project
dotnet test EShoppingZone.Wallet.API.Tests
```

### Test Results

```
EShoppingZone.Profile.API.Tests   → 11 / 11 ✅
EShoppingZone.Product.API.Tests   → 10 / 10 ✅
EShoppingZone.Cart.API.Tests      →  9 /  9 ✅
EShoppingZone.Order.API.Tests     →  7 /  7 ✅
EShoppingZone.Wallet.API.Tests    → 16 / 16 ✅
─────────────────────────────────────────────
Total                             → 53 / 53 ✅
```

| Test Project             | Coverage Area                                                                             |
| ------------------------ | ----------------------------------------------------------------------------------------- |
| `ProfileServiceTests.cs` | RegisterCustomer, RegisterMerchant, Login (valid/invalid), GetById, GetAll, Delete        |
| `ProductServiceTests.cs` | AddProduct, GetAll, GetById, GetByName, GetByCategory, Update, Delete                     |
| `CartServiceTests.cs`    | AddCart, GetCartById, GetAllCarts, CartTotal (multiple/empty/single), UpdateCart          |
| `OrderServiceTests.cs`   | PlaceOrder COD, Persists to DB, GetAllOrders, GetByCustomerId, ChangeStatus, Delete       |
| `WalletServiceTests.cs`  | AddWallet, AddMoney, PayMoney (sufficient/insufficient), RefundMoney, GetById, Statements |

Tests use:

- **NUnit** — test framework
- **Moq** — mocks Repository and DbContext (no real PostgreSQL needed)
- **FluentAssertions** — readable assertions
- **Microsoft.EntityFrameworkCore.InMemory** — in-memory DB for tests
- **ConfigureWarnings(InMemoryEventId.TransactionIgnoredWarning)** — suppresses transaction warning in Order + Wallet tests

---

## 🌍 Deployment

### Backend — Render.com ✅

| Service     | Render URL                                 |
| ----------- | ------------------------------------------ |
| Profile API | https://eshoppingzone-profile.onrender.com |
| Product API | https://eshoppingzone-product.onrender.com |
| Cart API    | https://eshoppingzone-cart.onrender.com    |
| Order API   | https://eshoppingzone-order.onrender.com   |
| Wallet API  | https://eshoppingzone-wallet.onrender.com  |

### Database — Neon.tech ✅

5 isolated PostgreSQL databases hosted on Neon.tech free tier.

### Frontend — Render.com ✅

https://eshoppingzone-frontend-bwfa.onrender.com

```bash
# Build for production
ng build --configuration production
# Output: dist/eshoppingzone-frontend/browser/
```

### Environment Variables (per service)

```
Jwt__Key             = EShoppingZone_SuperSecretKey_2026_DoNotShare_MinLength32Chars!
Jwt__Issuer          = EShoppingZone
Jwt__Audience        = EShoppingZoneClients
Jwt__ExpiryHours     = 24
ConnectionStrings__ProfileDb = Host=...;Database=eshoppingzone_profile;...
ServiceUrls__WalletApi       = https://eshoppingzone-wallet.onrender.com  (Order API only)
Razorpay__KeyId              = rzp_test_xxxx  (Order + Wallet API)
Razorpay__KeySecret          = xxxx           (Order + Wallet API)
```

---

## 📚 Resources

- [ASP.NET Core Docs](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core Docs](https://docs.microsoft.com/ef/core)
- [Razorpay Docs](https://razorpay.com/docs/)
- [Angular Docs](https://angular.dev/)
- [Neon.tech Docs](https://neon.tech/docs)
- [NUnit Docs](https://docs.nunit.org/)

---

<div align="center">

Made with ❤️ · EShoppingZone — Browse. Add to Cart. Order. Pay. Effortlessly.

</div>
