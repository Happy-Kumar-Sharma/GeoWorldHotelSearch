# GeoWorldHotelSearch

A full-stack, production-grade hotel search engine built with ASP.NET Core 8.0 MVC. This application provides blazing-fast full-text search capabilities, a rich UI, and real-time analytics.

![GeoWorldHotelSearch Screenshot](screenshot.png)

## Features

- Full-text search with Elasticsearch
- Geospatial search capabilities
- Responsive UI with Bootstrap 5
- Admin dashboard with analytics
- RESTful API with Swagger documentation
- Data seeding for testing

## Setup Guide for Non-Technical Users

### Step 1: Install Required Software

1. **Install .NET 8.0 SDK**
   - Go to [https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
   - Download and install the .NET 8.0 SDK for your operating system (Windows, macOS, or Linux)
   - Verify installation by opening a command prompt/terminal and typing: `dotnet --version`

2. **Install PostgreSQL**
   - Go to [https://www.postgresql.org/download/](https://www.postgresql.org/download/)
   - Download and install PostgreSQL for your operating system
   - During installation:
     - Remember the password you set for the 'postgres' user
     - Keep the default port (5432)
     - Complete the installation

3. **Install Elasticsearch**
   - Go to [https://www.elastic.co/downloads/elasticsearch](https://www.elastic.co/downloads/elasticsearch)
   - Download and install Elasticsearch 7.x for your operating system
   - Start Elasticsearch service after installation

### Step 2: Download and Set Up the Project

1. **Download the Project**
   - Download the project as a ZIP file from the repository
   - Extract the ZIP file to a folder on your computer

2. **Configure the Database Connection**
   - Open the extracted folder
   - Find and open the file `appsettings.json` in a text editor
   - Update the connection string with your PostgreSQL password:
   ```
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=GeoWorldHotelSearch;Username=postgres;Password=YOUR_PASSWORD_HERE"
   }
   ```
   - Save the file

3. **Configure Elasticsearch**
   - In the same `appsettings.json` file, verify the Elasticsearch URL:
   ```
   "Elasticsearch": {
     "Url": "http://localhost:9200"
   }
   ```
   - Save the file if you made any changes

### Step 3: Run the Application

1. **Open a Command Prompt/Terminal**
   - Navigate to the extracted project folder
   - For example: `cd C:\Users\YourName\Downloads\GeoWorldHotelSearch`

2. **Create the Database**
   - Run the following command: `dotnet ef database update`
   - This will create the database and all required tables

3. **Start the Application**
   - Run the following command: `dotnet run`
   - Wait for the application to start
   - You should see a message like: "Now listening on: https://localhost:7034"

4. **Open the Application in a Web Browser**
   - Open your web browser
   - Go to: `https://localhost:7034`
   - You should see the GeoWorldHotelSearch homepage

### Step 4: Seed Sample Data

1. **Add Sample Hotels**
   - In the application, click on "Hotels" in the navigation menu
   - Click the "Seed Data" button
   - Enter the number of sample hotels you want to create (e.g., 1000)
   - Click "Generate"
   - Wait for the data generation to complete

### Step 5: Explore the Application

1. **Search for Hotels**
   - Use the search bar on the homepage to search for hotels
   - Try different search terms like "beach", "luxury", or city names

2. **View Hotel Details**
   - Click on a hotel to view its details
   - Explore the hotel information, amenities, and location map

3. **Explore the API**
   - Go to: `https://localhost:7034/swagger`
   - This shows all available API endpoints for programmatic access

## Troubleshooting

### Database Connection Issues
- Verify PostgreSQL is running
- Check that the connection string in `appsettings.json` has the correct password
- Make sure the PostgreSQL port (5432) is not blocked by a firewall

### Elasticsearch Issues
- Verify Elasticsearch is running by opening: `http://localhost:9200` in your browser
- You should see a JSON response with Elasticsearch version information
- If not, start the Elasticsearch service

### Application Won't Start
- Make sure you have the correct .NET SDK version installed
- Try running `dotnet restore` before `dotnet run`
- Check for error messages in the command prompt/terminal

## License

This project is licensed under the MIT License - see the LICENSE file for details.
