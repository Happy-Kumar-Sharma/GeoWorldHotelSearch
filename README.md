<!-- filepath: /home/happy/learning/GeoWorldHotelSearch/README.md -->

<p align="center">
  <img src="https://raw.githubusercontent.com/Happy-Kumar-Sharma/GeoWorldHotelSearch/master/screenshot.png" alt="GeoWorldHotelSearch Screenshot" width="600"/>
</p>

<h1 align="center">GeoWorldHotelSearch</h1>

<p align="center">
  <b>Production-grade, blazing-fast hotel search engine with full-text and geospatial search, built on ASP.NET Core 8, PostgreSQL, and Elasticsearch.</b>
</p>

---

<p align="center">
  <a href="Views/Home/ProjectDocumentation.html" target="_blank" style="font-size:1.25rem;font-weight:bold;">
    📖 <b>View Full Project Documentation (HTML)</b>
  </a>
</p>

---

## 🚀 Features

- 🌍 Full-text & geospatial hotel search (Elasticsearch)
- 🗺️ Location-based filtering
- 📊 Admin dashboard with analytics
- 🏨 Hotel CRUD (Create, Read, Update, Delete)
- 📝 Booking functionality with validation
- 🧩 Modern, responsive UI (Bootstrap 5)
- 🔒 Secure, production-ready code
- 🧪 Data seeding for testing
- 🛠️ RESTful API with Swagger/OpenAPI docs

---

## 🧑‍💻 Technologies Used

- **Backend:** ASP.NET Core 8.0 MVC
- **Database:** PostgreSQL (with Entity Framework Core)
- **Search Engine:** Elasticsearch 7.x (NEST client)
- **Frontend:** Bootstrap 5, jQuery, Chart.js
- **API Docs:** Swagger/OpenAPI
- **Data Generation:** Bogus
- **Containerization:** Docker (optional)

---

## 📚 Prerequisites & Prior Knowledge

- **Required:**
  - Basic understanding of C# and .NET
  - Familiarity with using a terminal/command prompt
- **Recommended:**
  - Some experience with SQL databases (PostgreSQL)
  - Basic knowledge of REST APIs
  - (Optional) Docker basics for containerized setup

---

## 🛠️ Quick Start (for Any Level Developer)

### 1. Install Required Tools

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Elasticsearch 7.x](https://www.elastic.co/downloads/elasticsearch)
- [Docker](https://www.docker.com/) *(optional, for easy DB & ES setup)*

### 2. Clone & Configure

```bash
git clone https://github.com/Happy-Kumar-Sharma/GeoWorldHotelSearch.git
cd GeoWorldHotelSearch
```

- Edit `appsettings.json`:
  - Set your PostgreSQL password in `DefaultConnection`
  - Ensure Elasticsearch URL is correct (default: `http://localhost:9200`)

### 3. Database & Elasticsearch Setup

- **With Docker (Recommended for Beginners):**
  - Run: `cd elastic-start-local/elastic-start-local && ./start.sh`
  - This will start PostgreSQL and Elasticsearch containers
- **Manual:**
  - Start PostgreSQL and Elasticsearch services on your machine

### 4. Run Migrations & Seed Data

```bash
dotnet ef database update
```

- (Optional) Seed hotels via the web UI (Hotels > Seed Data)

### 5. Start the App

```bash
dotnet run
```
- Open [https://localhost:7034](https://localhost:7034) in your browser

---

## 🏗️ Project Structure

```
GeoWorldHotelSearch/
├── Controllers/         # MVC controllers (API, Hotels, Booking, etc.)
├── Data/                # EF Core DbContext
├── Models/              # Data models (Hotel, Booking, etc.)
├── Repositories/        # Data access layer
├── Services/            # Business logic
├── Views/               # Razor views (UI)
├── wwwroot/             # Static files (CSS, JS, images)
├── elastic-start-local/ # Docker scripts for local dev
├── Migrations/          # EF Core migrations
├── appsettings.json     # Configuration
└── ...
```

---

## 📊 Architecture & Flow Diagrams

All project diagrams are available in the `diagrams/` folder. These diagrams provide a visual overview of the architecture, technology stack, and key flows:

- **Project Architecture:**
  - `diagrams/project_architecture.png` — High-level structure and file relationships
- **Tools & Technology Stack:**
  - `diagrams/tools_technology_stack.png` — All major tools, frameworks, and how they interact
- **Hotel Search Flow:**
  - `diagrams/hotel_search_flow.png` — Step-by-step process of searching for hotels
- **Hotel Create Flow:**
  - `diagrams/hotel_create_flow.png` — Step-by-step process of adding a new hotel (including Elasticsearch indexing)
- **(Legacy) Combined Search/Add Flow:**
  - `diagrams/hotel_search_add_flow.png` — Both flows in a single diagram (for reference)

> For the PlantUML source files, see the corresponding `.puml` files in the same folder.

You can open these diagrams to quickly understand how the system works, how data flows, and how to extend or debug the application.

---

## 📝 Editing & Extending

- **Edit UI:** Change Razor files in `Views/`
- **Add/Change Models:** Edit files in `Models/` and run migrations if needed
- **Business Logic:** Update or add services in `Services/`
- **API/Controllers:** Add endpoints in `Controllers/`
- **Styling:** Edit CSS in `wwwroot/css/`
- **JS:** Add scripts in `wwwroot/js/`

### Hot Reload (for UI changes)
```bash
dotnet watch run
```

---

## 🤝 Contributing

1. Fork the repo & clone your fork
2. Create a new branch: `git checkout -b feature/your-feature`
3. Make your changes and commit: `git commit -am 'Add new feature'`
4. Push to your fork: `git push origin feature/your-feature`
5. Open a Pull Request on GitHub

---

## 🧑‍🔬 FAQ

**Q: Do I need to know Docker?**
- No, but it makes setup easier. Manual setup instructions are provided.

**Q: Can I use SQLite or MySQL?**
- The app is designed for PostgreSQL. You can adapt it, but some features (like JSON columns) are PostgreSQL-specific.

**Q: How do I reset the database?**
- Drop the DB in PostgreSQL, then run `dotnet ef database update` again.

**Q: How do I edit hotel info or amenities?**
- Use the web UI (Hotels > Edit) or update via the API.

**Q: How do I run tests?**
- (If tests are present) Run: `dotnet test`

---

## 🌟 Rolling Featured Hotels

- **Mark/Unmark as Featured:** Admins and users can mark any hotel as "Featured" directly from the hotel list, details, or edit screens.
- **Home Page Display:** Only featured hotels are shown in the "Featured Hotels" section on the home page. If no hotels are featured, a message is shown.
- **How to Use:**
  - On the Hotels list or details page, click the star button to mark/unmark a hotel as featured.
  - On the Edit Hotel page, use the "Mark as Featured Hotel" checkbox.
  - Changes are reflected instantly in the UI and on the home page.
- **API:** The backend exposes a `SetFeatured` action for toggling featured status.
- **Database:** The `Hotels` table includes an `IsFeatured` column (bool).

---

## 🛡️ Security

See [SECURITY.md](SECURITY.md) for security policy and reporting vulnerabilities.

---

## 🆘 Troubleshooting

- **Database Issues:** Ensure PostgreSQL is running and connection string is correct
- **Elasticsearch Issues:** Ensure ES is running at the configured URL
- **Port Conflicts:** Change ports in `appsettings.json` or Docker scripts
- **Other Issues:** Check logs in the terminal, or open an issue on GitHub

---

## 📄 License

MIT License. See [LICENSE](LICENSE) for details.
