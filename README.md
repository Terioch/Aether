# Aether

### Video Demo: https://youtu.be/LWjSVAPWd4w

### Description:

Aether is an air quality monitoring application that provides real-time insights into air pollution levels worldwide.
Users can view an interactive map that defaults to their current location and highlights the Air Quality Index (AQI)
as well as a breakdown of different pollutants and their contribution to the overall air quality in that area.

The application is built with a .NET backend, a Next.js frontend and a PostsgreSQL database, designed to be performant, scalable, and data-driven.
Air quality data is loaded from the OpenWeatherMap air pollution API.

### Key Features

🗺️ Interactive Map Interface

- Displays a central marker based on the user’s current location or selected point
- Dynamically loads and displays neighbouring locations within the current map bounds

📊 Air Quality Index (AQI) Breakdown

- Shows overall air quality levels for each location
- Provides a clear breakdown of individual pollutants and their relative impact

⚡ Efficient Data Access

- All location coordinates are stored in the database
- Air quality readings are stored in the database and also cached in memory to minimise external API calls and improve performance

### Pollutant Data

Each location includes indexed readings for multiple air pollutants, allowing users to understand why air quality is good or poor in a given area.

Tracked pollutants include:

- Sulfur Dioxide (SO₂)
- Nitrogen Oxide (NO)
- Nitrogen Dioxide (NO₂)
- Particulate Matter 10 (PM₁₀)
- Particulate Matter 2.5 (PM₂.₅)
- Ozone (O₃)
- Carbon Monoxide (CO)
- Ammonia (NH₃)

These values are presented both individually and as part of the overall AQI calculation.
