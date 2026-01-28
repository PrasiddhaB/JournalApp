# Personal Journal Application

A secure, feature-rich journaling desktop application built with **.NET MAUI Blazor Hybrid** for Windows.

## 🎯 Features

### Core Journaling
- **One entry per day** - Enforced at database level
- **Rich-text/Markdown editor** with live preview
- **Auto-calculated word count**
- **System-generated timestamps** (CreatedAt, UpdatedAt)

### Mood Tracking
- **Primary mood** (required) - One main mood for analytics
- **Secondary moods** (optional) - Up to two additional moods
- **Categories**:
  - 🟢 **Positive**: Happy, Excited, Relaxed, Grateful, Confident
  - 🔵 **Neutral**: Calm, Thoughtful, Curious, Nostalgic, Bored
  - 🔴 **Negative**: Sad, Angry, Stressed, Lonely, Anxious

### Tags & Categorization
- **30+ pre-built tags**: Work, Health, Travel, Family, etc.
- **Custom user-defined tags**
- **Categories** for organizing entries

### Navigation
- **Calendar view** - Visual date navigation with entry indicators
- **Timeline view** - Paginated list of all entries
- **Search** by title or content
- **Filters** by date range, mood(s), and tag(s)

### Analytics Dashboard
All analytics are date-range filterable:
- 📊 **Mood distribution** (Positive/Neutral/Negative percentages)
- 😊 **Most frequent mood**
- 🔥 **Current streak** & **Longest streak**
- 📅 **Missed days** tracking
- 🏷️ **Most used tags** & Tag breakdown
- 📈 **Word count trends** over time

### Security & Privacy
- 🔐 **PIN/Password protection** (BCrypt hashed)
- 💾 **All data stored locally** in SQLite
- 🚫 **No cloud sync** - Your data stays on your device

### Export
- 📤 **Export to PDF** with date range selection
- Professional formatting with moods, tags, and content

### Customization
- 🌙 **Dark/Light theme** toggle
- Theme preference persisted locally

---

## 🛠️ Technology Stack

| Component | Technology |
|-----------|------------|
| Framework | .NET 10 MAUI Blazor Hybrid |
| UI | HTML, CSS, JavaScript (Blazor Components) |
| Database | SQLite + Entity Framework Core |
| Password Hashing | BCrypt.Net-Next |
| PDF Export | QuestPDF |
| Target Platform | Windows 10/11 Desktop |

---

## 📁 Project Structure

```
JournalApp/
├── App.xaml / App.xaml.cs        → MAUI application entry
├── MauiProgram.cs                → DI configuration & services
├── MainPage.xaml                 → Blazor WebView host
│
├── Data/
│   ├── JournalDbContext.cs       → EF Core context + configuration
│   ├── DbPathHelper.cs           → Local DB path management
│   ├── SeedData.cs               → Pre-built moods & tags
│   └── AppInitializer.cs         → Database initialization
│
├── Models/
│   ├── JournalEntry.cs           → Main entry entity
│   ├── Mood.cs                   → Mood with category
│   ├── Tag.cs                    → Tags (pre-built & custom)
│   ├── EntryTag.cs               → Many-to-many junction
│   ├── AppSettings.cs            → PIN, theme preferences
│   └── Dtos.cs                   → Filter, analytics DTOs
│
├── Repositories/
│   ├── IJournalRepository.cs     → CRUD & query contracts
│   ├── JournalRepository.cs      → SQLite implementation
│   ├── ISettingsRepository.cs    → Settings contracts
│   └── SettingsRepository.cs     → Settings implementation
│
├── Services/
│   ├── AuthService.cs            → PIN authentication
│   ├── ThemeService.cs           → Light/dark theme
│   ├── StreakService.cs          → Streak calculations
│   ├── AnalyticsService.cs       → Dashboard analytics
│   └── ExportPdfService.cs       → PDF generation
│
├── Pages/
│   ├── MainLayout.razor          → App shell & navigation
│   ├── Home.razor                → Dashboard & quick stats
│   ├── Today.razor               → Today's entry editor
│   ├── Entry.razor               → Edit entry for specific date
│   ├── Calendar.razor            → Calendar navigation
│   ├── Timeline.razor            → Paginated entry list
│   ├── Analytics.razor           → Charts & statistics
│   ├── Settings.razor            → PIN, theme, export
│   ├── Lock.razor                → PIN entry screen
│   └── Routes.razor              → Blazor routing
│
├── Components/
│   ├── EntryEditor.razor         → Markdown editor + preview
│   ├── MoodPicker.razor          → Primary + secondary moods
│   ├── TagPicker.razor           → Tag selection & creation
│   ├── EntryCard.razor           → Timeline entry display
│   ├── FilterBar.razor           → Search & filter controls
│   └── ConfirmDialog.razor       → Delete confirmations
│
└── wwwroot/
    ├── index.html                → Blazor host page
    └── css/app.css               → Complete stylesheet
```

---

## 🚀 Getting Started

### Prerequisites

1. **Visual Studio 2022** (17.8 or later)
2. **.NET 10 SDK** (Preview)
3. **MAUI Workload** installed:
   ```bash
   dotnet workload install maui
   ```

### Setup Instructions

1. **Create a new project in Visual Studio**:
   - File → New → Project
   - Search for ".NET MAUI Blazor Hybrid App"
   - Name it `JournalApp`
   - Select .NET 10 (or latest preview)

2. **Copy the project files**:
   - Replace/add all files from this template
   - Ensure the folder structure matches

3. **Install NuGet Packages**:
   ```bash
   dotnet add package Microsoft.EntityFrameworkCore.Sqlite
   dotnet add package BCrypt.Net-Next
   dotnet add package QuestPDF
   ```

4. **Build and Run**:
   - Set target to "Windows Machine"
   - Press F5 or click "Start"

### Database Location

The SQLite database is automatically created at:
```
C:\Users\{username}\AppData\Local\JournalApp\journal.db
```

PDF exports are saved to:
```
C:\Users\{username}\AppData\Local\JournalApp\Exports\
```

---

## 📸 Screenshots

The application includes:
- **Home Dashboard** - Quick stats and today's entry status
- **Today Editor** - Rich text editor with mood/tag selection
- **Calendar View** - Visual navigation with entry indicators
- **Timeline** - Searchable, filterable entry list
- **Analytics** - Charts and statistics
- **Settings** - Security, theme, and export options

---

## 🔒 Security Notes

- Passwords are hashed using **BCrypt** with work factor 12
- Session timeout is configurable (default: 30 minutes)
- All data remains **local** - never transmitted
- The database can be backed up by copying the `.db` file

---

## 📝 Database Schema

### JournalEntries
| Column | Type | Description |
|--------|------|-------------|
| Id | INT (PK) | Auto-increment |
| EntryDate | DATE | **Unique** - One per day |
| Title | VARCHAR(200) | Entry title |
| Content | TEXT | Markdown content |
| WordCount | INT | Auto-calculated |
| Category | VARCHAR(100) | Optional grouping |
| PrimaryMoodId | INT (FK) | Required mood |
| SecondaryMood1Id | INT (FK) | Optional mood |
| SecondaryMood2Id | INT (FK) | Optional mood |
| CreatedAt | DATETIME | System-generated |
| UpdatedAt | DATETIME | System-generated |

### Moods
| Column | Type | Description |
|--------|------|-------------|
| Id | INT (PK) | Auto-increment |
| Name | VARCHAR(50) | Mood name |
| Category | INT | 1=Positive, 2=Neutral, 3=Negative |
| Emoji | VARCHAR(10) | Display emoji |
| DisplayOrder | INT | UI ordering |

### Tags
| Column | Type | Description |
|--------|------|-------------|
| Id | INT (PK) | Auto-increment |
| Name | VARCHAR(50) | **Unique** tag name |
| IsPrebuilt | BOOL | System vs user-created |
| Color | VARCHAR(20) | Hex color code |
| CreatedAt | DATETIME | Creation timestamp |

### EntryTags (Junction)
| Column | Type | Description |
|--------|------|-------------|
| Id | INT (PK) | Auto-increment |
| JournalEntryId | INT (FK) | Entry reference |
| TagId | INT (FK) | Tag reference |

### AppSettings (Singleton)
| Column | Type | Description |
|--------|------|-------------|
| Id | INT (PK) | Always 1 |
| IsPasswordEnabled | BOOL | PIN protection enabled |
| PasswordHash | VARCHAR(100) | BCrypt hash |
| Theme | VARCHAR(20) | "light" or "dark" |
| SessionTimeoutMinutes | INT | Lock timeout |

---

## 🎨 Theming

The application supports **Light** and **Dark** themes:

```css
/* Light Theme (default) */
--bg-primary: #ffffff;
--text-primary: #1e293b;

/* Dark Theme */
--bg-primary: #0f172a;
--text-primary: #f1f5f9;
```

Theme preference is stored in the database and persists across sessions.

---

## 📚 Coursework Alignment

This project demonstrates:

1. **Clean Architecture** - Separation of concerns with Models, Repositories, Services
2. **Repository Pattern** - Abstraction over data access
3. **Dependency Injection** - Services registered in MauiProgram.cs
4. **Entity Framework Core** - ORM with SQLite
5. **Blazor Components** - Reusable UI with parameters and events
6. **Secure Password Storage** - BCrypt hashing
7. **PDF Generation** - QuestPDF document creation
8. **Theme System** - CSS variables with JavaScript state

---

## 📄 License

This is a coursework project. Use for educational purposes.

---

## 🤝 Contributing

This is a coursework submission. For questions, contact your instructor.
