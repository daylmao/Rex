

## 🦖 **What is Rex?**

Rex is a **social platform designed specifically for autodidacts** - self-directed learners who thrive on curiosity and peer collaboration. It's where passionate learners create **thematic study groups**, tackle **knowledge challenges**, and share their learning journeys with like-minded individuals.

### 🎯 **Core Philosophy**
> *"Learning is not a solo journey - it's a collaborative adventure where every question sparks discovery and every challenge builds wisdom."*

## ✨ **The Learning Ecosystem**

### 👥 **Study Groups**
Users can create and join topic-focused learning communities:
- 💻 **Programming & Tech** - JavaScript, Python, AI/ML, DevOps
- 🎨 **Creative Arts** - Digital design, photography, music production
- 📈 **Business & Finance** - Entrepreneurship, investing, marketing
- 🔬 **Science & Research** - Data science, biology, physics
- 🌍 **Languages & Culture** - Spanish, Japanese, cultural studies
- *...and any topic imaginable*

### 🏆 **Challenge System**
Group leaders create engaging challenges that members can tackle:

```
📄 Challenge Example: "Build a REST API"
├── 🎯 Objective: Create a user authentication API
├── ⏱️ Duration: 7 days
├── 📁 Resources: Documentation links, tutorials
└── 💬 Solution Sharing: Code + explanation of approach
```

### 🗣️ **Knowledge Sharing**
Members don't just submit solutions - they **explain their thinking**:
- 🔍 **Problem-solving approach**
- 🛠️ **Tools and resources used**
- 💡 **Lessons learned**
- 🚫 **Challenges faced**
- 🚀 **Next steps for improvement**

## 🏛️ **Architecture**

*Built with Clean Architecture principles for scalability and maintainability*

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
```

## 🛠️ **Tech Stack**

| Component | Technology | Purpose |
|-----------|------------|----------|
| 🧠 **Backend** | .NET 8 + ASP.NET Core Web API | Robust learning platform |
| 💾 **Database** | PostgreSQL + Entity Framework Core | Scalable data management |
| 🔄 **Infrastructure** | Docker Compose | Development environment |
| 📊 **Monitoring** | Serilog + Seq | Structured logging and insights |
| 🔐 **Security** | JWT + Refresh Tokens | Secure authentication system |
| 🗃️ **Database Tools** | pgAdmin | Database administration |

---

<div align="center">

# 🚀 **Getting Started**

*Launch Rex in just a few simple steps*

</div>

<br>

<div align="center">

### 📋 **Prerequisites**

Make sure you have **Docker Desktop** installed

</div>

<br>

---

<div align="center">

## 🎆 **Environment Setup**

*Configure Rex for your learning adventure*

</div>

<br>

<div align="center">

<table>
<tr>
<td width="33%" align="center">

### 1️⃣
<img src="https://img.shields.io/badge/Copy-Template-FF9500?style=for-the-badge&logo=files&logoColor=white" alt="Copy">

**Copy Environment Template**

```bash
cp .env.template .env
```

</td>
<td width="33%" align="center">

### 2️⃣
<img src="https://img.shields.io/badge/Configure-Variables-4CAF50?style=for-the-badge&logo=edit&logoColor=white" alt="Edit">

**Edit Configuration**

Open `.env` file and fill in your local environment values

</td>
<td width="33%" align="center">

### 3️⃣
<img src="https://img.shields.io/badge/Launch-Rex-2196F3?style=for-the-badge&logo=rocket&logoColor=white" alt="Launch">

**Start the Platform**

```bash
docker compose up -d
```

</td>
</tr>
</table>

</div>

<br>

<div align="center">



### 🦖 **Welcome to Rex - Let the learning begin!**

</div>

---

## 🎮 **Key Features**

<table>
<tr>
<td width="50%">

### 📚 **For Learners**
- 🔍 Join study groups by interest
- 🏆 Participate in learning challenges
- 💬 Share solutions and explanations
- 🤝 Connect with fellow autodidacts

</td>
<td width="50%">

### 👑 **For Group Leaders**
- 🎨 Create themed study groups
- 🎯 Design engaging challenges
- 📁 Curate learning resources
- 📊 Monitor group progress
- ⚙️ Manage group dynamics

</td>
</tr>
</table>



---

<div align="center">
  <strong>Built by learners, for learners 📚</strong><br>
  <sub>Rex - Where autodidacts become unstoppable 🦖</sub><br><br>
  <em>"The best way to learn is to teach, and the best way to teach is to challenge."</em>
</div>
