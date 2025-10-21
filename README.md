# 🛡️ DragoTactical - Comprehensive Security Solutions

Welcome to **DragoTactical**, a cutting-edge web application that bridges the gap between a stagnant company and a live interactive web application. The application we have built for our client provides comprehensive cybersecurity and physical security solutions to protect our clients organizations from both cyber and physical threats.

## 🌟 About DragoTactical

DragoTactical is a professional security services company that offers a complete suite of protection solutions. Our website serves as both a marketing platform and a client engagement system, allowing potential clients to learn about our clients services and submit inquiries directly through our secure contact forms.

### What DragoTactical Offers

- **🔒 Cybersecurity Services**: Risk assessments, penetration testing, network security, data protection, incident response, and more
- **👮 Physical Security Services**: On-site security personnel, VIP protection, surveillance systems, access control, and emergency response
- **📊 Risk Management**: Comprehensive security audits, consulting, and project management
- **💼 Consultancy**: Virtual Cyber Assistants (VCA) and Virtual CISOs for strategic guidance

## 🚀 Getting Started

### Prerequisites

Before you begin, ensure you have the following installed on your development machine:

- **.NET 8.0 SDK** or later
- **Visual Studio 2022** (recommended) or **Visual Studio Code**
- **Git** for version control
- **SQLite** (included with .NET, no separate installation needed)

### 📥 Installation

1. Within the repository, click on the "<> Code" drop down on the far right next to the "Go to file" and "+" buttons.
2. On the Local tab, click on the last option: "Download ZIP".
3. Once the zip file has downloaded, open your local file explorer.
4. Go to your Downloads.
5. Click on the "DragoTactical.zip" folder, should be most recent in Downloads.
6. Extract the files and store the project in the location of choice.
7. Navigate to Visual Studio 2022 or Visual Studio Code.
8. To open the project, click File > Open > Choose the project folder "DragoTactical".

### 🔧 Build and Run

To run the application:
1. **Restore NuGet packages** (if using command line):
   ```bash
   dotnet restore
   ```

2. **Set up the database**:
   
   The project uses Entity Framework migrations to manage the database schema. You have two options:
   
   **Option A - Automatic (Recommended for first-time setup):**
   - Simply run the application - the database will be created automatically on first run
   
   **Option B - Manual (if you want to ensure migrations are applied):**
   ```bash
   dotnet ef database update
   ```
   
   **Note**: The database file (`DragoTactical.db`) will be created in the project root directory and is pre-populated with sample data including service categories and available services.

3. **Run the application**:
   - **Visual Studio**: Press `F5` or click the "Start" button
   - **Command Line**: Run `dotnet run`
   - **Visual Studio Code**: Press `Ctrl+F5` to run without debugging


## 🛠️ Technologies Used

### Backend Technologies
- **ASP.NET Core 8.0** - Modern web framework
- **Entity Framework Core 9.0** - Object-relational mapping
- **SQLite** - Lightweight database for development and production
- **C# 12** - Latest C# language features

### Frontend Technologies
- **HTML5** - Semantic markup
- **CSS3** - Modern styling with custom properties and flexbox/grid
- **JavaScript** - Client-side interactivity
- **Bootstrap 5** - Responsive UI framework
- **jQuery** - DOM manipulation and AJAX

### Security Features
- **Content Security Policy (CSP)** - XSS protection
- **Anti-Forgery Tokens** - CSRF protection
- **Rate Limiting** - DDoS and abuse prevention
- **Secure Headers** - Additional security layers
- **Input Validation** - Server-side validation
- **SQL Injection Protection** - Entity Framework parameterized queries

### Development Tools
- **Microsoft.CST.DevSkim** - Security code analysis
- **Entity Framework Tools** - Database migrations
- **ASP.NET Core Identity** - Authentication (configured but not used in current version)

## 📝 How to Submit Forms

The DragoTactical website includes several contact forms that allow potential clients to request information about their services. Here's how the form submission process works:

### Available Forms

1. **Contact Us Form** 
   - General inquiries and service requests
   - Includes fields for personal information, company details, and service selection

2. **Service-Specific Forms**
   - **Cybersecurity Services**: For requesting cybersecurity services
   - **Physical Security Services**: For requesting Physical security services

### Form Submission Process

1. **Fill out the form** with your details:
   - Personal information (Name, Email, Phone)
   - Company information (Company Name, Location)
   - Service selection (choose from available services)
   - Additional message (optional)

2. **Follow-up** - the company will:
   - Review your submission within 24 hours
   - Contact you via email or phone
   - Provide detailed information about requested services

### Form Security Features

- **CSRF Protection**: All forms include anti-forgery tokens
- **Rate Limiting**: Prevents spam and abuse
- **Input Validation**: Server-side validation for all fields
- **Secure Storage**: Data is encrypted and stored securely
- **Privacy Protection**: Personal information is handled according to privacy standards

## 🗄️ Database Structure

The application uses SQLite with Entity Framework Core. The database includes:

### Tables
- **Categories**: Service categories (Physical, Cybersecurity)
- **Services**: Individual services offered by DragoTactical
- **FormSubmissions**: Client inquiries and contact form data

### Sample Data
The database is pre-populated with:
- 2 service categories
- 16 different services (8 physical, 8 cybersecurity)


## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Built with ❤️ by Cybernetic Solutions**

*Protecting your digital and physical assets with cutting-edge security solutions.*
