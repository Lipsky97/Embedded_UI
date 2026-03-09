# Embedded_UI
> [!IMPORTANT]
> This is a project in progress, which means some parts of the description talk about planned features.
## Description
This project was created to demonstrate my skills in developing embedded software. It collects environmental data from a BME280 sensor via the Raspberry Pi’s GPIO interface and displays the results in real time.

Users can view live readings—such as temperature changes over the past hour, updated every second—as well as generate historical charts from saved data. The interface is optimized for touchscreen use, with a default resolution of 600×1200, but can be used on any screen side. Includes configurable settings for units and display preferences.

The repository also includes a small Python utility that reads values from the sensor and broadcasts them to the .NET component of the project. All readings are stored in a local SQLite database for reliable data management, and when an internet connection is available, the system automatically backs up the database to ensure data safety.

## Implemented features
* Basic user interface
* Python utility to read and broadcast sensor values
* Real-time values display
* UI contains charts
* Read from local database and chart sensor readings

## Planned fatures
* Real-time charts updating every n amount of time
* Settings page
* Saving and retrieving chart configs and positions

