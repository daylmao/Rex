


<div align="center">

# 🦖 **Rex**
### *Where autodidacts learn, build, and grow together.*

![.NET 8](https://img.shields.io/badge/.NET%208.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-336791?style=for-the-badge&logo=postgresql&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white)
![Hangfire](https://img.shields.io/badge/Hangfire-E9573F?style=for-the-badge&logo=dotnet&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-1E90FF?style=for-the-badge&logo=dotnet&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-0db7ed?style=for-the-badge&logo=docker&logoColor=white)


---

### 🧡 *Built for the curious. Powered by collaboration.*
> “Learning is not a solo journey — it’s a shared adventure.”

</div>

---

## ⚡ **What is Rex?**

Rex is a **social learning platform** for autodidacts — people who learn by doing, exploring, and sharing.  
Users can form **topic-based study groups**, complete **challenges**, and grow together through **peer-to-peer learning**.

---

## 🎯 **Core Philosophy**

> *Every question sparks discovery, and every challenge builds wisdom.*

Rex is designed to transform the way self-learners connect:
- 💬 Collaborate through meaningful discussions  
- 🧠 Learn through thematic challenges  
- 🔥 Grow by sharing your insights  
- 🌍 Build knowledge communities that last  

---

## 🧩 **The Learning Ecosystem**

### 👥 Study Groups  
Create or join communities focused on your passion:
- 💻 Programming & Tech — JavaScript, AI, DevOps  
- 🎨 Creative Arts — Design, Music, Photography  
- 📈 Business — Marketing, Finance, Startups  
- 🔬 Science — Data Science, Physics, Biology  
- 🌍 Languages — English, Japanese, Culture  

### 🏆 Challenges  
Group leaders create interactive challenges to push members forward:  

```

📄 Challenge: "Build a REST API"
🎯 Goal: Create an authentication system
⏱️ Duration: 7 days
💬 Share: Code + your approach and lessons learned

```

### 🗣️ Knowledge Sharing  
Members explain not just *what* they did, but *how* they thought:
- 🧠 Reasoning & approach  
- 🛠️ Tools & stack used  
- 💡 Key takeaways  
- 🚫 Mistakes & lessons  
- 🚀 Next steps  

---

## 🏗️ **Architecture Overview**

> Built with **Clean Architecture** for scalability, modularity, and maintainability.

```

🦖 Rex Learning Platform/
│
├── 🧠 Rex.Domain/                     # Core business logic and entities
│   ├── ⚙️ Configurations/             # Entity configurations (EF Core)
│   ├── 🎲 Enum/                      # Domain enumerations
│   └── 👥 Models/                     # Entities (User, Group, Post, etc.)
│
├── 🎯 Rex.Application/                # Application logic (orchestration)
│   ├── 🎭 Abstractions/              # Abstractions (e.g., IEmailService)
│   ├── 🧠 Behavior/                  # Pipeline behaviors (MediatR)
│   ├── 📦 DTOs/                      # Data Transfer Objects
│   ├── 🤝 Helpers/                   # Helper classes
│   ├── 🔌 Interfaces/                # Contracts (Repository Interfaces, etc.)
│   ├── 🧩 Modules/                   # Logic by feature (Feature Sliced)
│   │   ├── Users/                    # (e.g., Commands, Queries, Handlers)
│   │   ├── Posts/                    # ...and so on for each module
│   │   └── ...
│   ├── 📄 Pagination/                # Pagination logic
│   ├── 🚀 Services/                  # Generic application services
│   ├── 🛠️ Utilities/                 # Utilities (Results, Errors)
│   └── 💉 DependencyInjection.cs      # Dependency Injection setup
│
├── 🏗️ Infrastructure/                 # External concerns (Database, APIs)
│   │
│   ├── 💾 Rex.Infrastructure.Persistence/
│   │   ├── 🗃️ Context/                 # EF Core DbContext
│   │   ├── 🔄 Migrations/              # Database migrations
│   │   ├── 📥 Repository/              # Repository implementations
│   │   ├── ⚙️ Services/                 # Persistence-related services
│   │   └── 💉 DependencyInjection.cs   # Injection setup
│   │
│   └── 🌐 Rex.Infrastructure.Shared/
│       ├── ⚙️ Services/                 # Implementations (Email, Auth)
│       └── 💉 DependencyInjection.cs   # Injection setup
│
└── 🌐 Rex.Presentation.Api/            # Presentation Layer (API)
    ├── 🎮 Controllers/               # API Endpoints
    ├── 🛡️ Filters/                    # Action filters (exceptions, etc.)
    ├── 🚧 Middlewares/               # Custom middlewares
    ├── 🔧 ServicesExtension/         # `IServiceCollection` extensions
    ├── 📜 appsettings.json           # Application configuration
    └── 🚀 Program.cs                 # Entry point and service registration

````

---

## 🛠️ **Tech Stack**

| Layer | Technology | Purpose |
|-------|-------------|----------|
| 🧠 **Backend** | ASP.NET Core 8 | Core API |
| 💾 **Database** | PostgreSQL + EF Core | Data management |
| ⚡ **Real-Time** | SignalR | Live group communication |
| ⏰ **Background Jobs** | Hangfire | Async task scheduling |
| 🚀 **Caching / Messaging** | Redis | Fast data exchange |
| 🔐 **Security** | JWT + Refresh Tokens | Authentication |
| 📊 **Logging** | Serilog + Seq | Observability |
| 🐳 **Containerization** | Docker Compose | Easy deployment |

---

## ⚙️ **Setup Guide**

<div align="center">

### 📋 **Prerequisites**
Make sure you have **Docker** and **Docker Compose** installed.

</div>

---

### 1️⃣ Copy environment template
```bash
cp .env.template .env
````

### 2️⃣ Configure your environment

Edit `.env` with your database, JWT, and Redis values.

### 3️⃣ Run the platform

```bash
docker compose up -d
```

---

<div align="center">

### 🦖 **Rex is ready to launch — let your learning adventure begin!**

</div>

---

## 🎮 **Key Features**

| For Learners                          | For Group Leaders                     |
| ------------------------------------- | ------------------------------------- |
| 🔍 Join topic-based study groups      | 🎨 Create and customize study groups  |
| 🏆 Participate in learning challenges | 🎯 Design challenges and set goals    |
| 💬 Share solutions and insights       | 📁 Provide curated learning resources |
| 🤝 Collaborate with peers             | 📊 Monitor progress and engagement    |


<div align="center">

## 🌟 **Project Vision**

Rex is a personal project born from a passion for **self-learning**, **community**, and **knowledge sharing**.  
It’s not open to external contributions, but you’re welcome to explore, get inspired, or follow its progress.

🦖 *Star this repo if you believe in the power of self-driven learning.*

---

### 🧡 **Rex — Where autodidacts become unstoppable.**

</div>
