**Overview**
A data-driven web application that tracks Old School RuneScape (OSRS) item prices over time and visualizes historical trends. This project focuses on automated data collection, storage, and analysis, simulating a real-world data pipeline for tracking and analyzing time-series data.

**Key Features**
- Automated daily price updates using a background service
- Integration with external API for real-time item data
- Historical price tracking with interactive charts
- Role-based authentication (Admin / User)
- Dynamic tables and responsive UI

**Tech Stack**
- Backend: ASP.NET, C#
- Frontend: HTML, CSS, JavaScript
- Database: SQL
- Libraries: Chart.js, DataTables
- Tools: Git, GitHub

**How it Works**
- Users add items using a Game ID
- The application retrieves item data from an external API
- A background service runs every 24 hours to collect updated prices
- Price data is stored and linked to each item
- Users can view current prices and historical trends through graphs

**Data & Architecture**
- Designed relational data models linking Categories → Items → Price History
- Implemented automated data ingestion using scheduled background services
- Stored time-series pricing data for trend analysis
- Processed and visualized data using JavaScript charting libraries

**Access Control**
- Admin users: Full CRUD access to items and categories
- Authenticated users: Can view price history and trends
- Public users: Can view current item prices

**Resources Used**
- OSRS API: https://runescape.wiki/w/Application_programming_interface
- Chart.js: https://www.chartjs.org/
- DataTables: https://datatables.net/

