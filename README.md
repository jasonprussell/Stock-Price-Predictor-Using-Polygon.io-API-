# Stock Price Forecasting Application

## Overview

This application is designed to fetch historical stock price data and predict future prices using Machine Learning (ML) with the help of Microsoft's ML.NET library. It specifically uses the Singular Spectrum Analysis (SSA) time series forecasting model to make predictions about the future closing prices of a specified stock.

## Features

- Fetches historical stock price data from the Polygon.io API.
- Converts the JSON response into a .NET object model.
- Utilizes ML.NET to train a time series forecasting model.
- Predicts future stock prices along with confidence intervals.

## Requirements

- .NET 5.0 or later
- An API key from Polygon.io

## Setup

1. **API Key**: Obtain an API key from [Polygon.io](https://polygon.io/).
2. **Configuration**: Replace `YOUR_POLYGON_API_KEY` in the `Program.cs` file with your actual Polygon.io API key.

## Usage

- Compile and run the program using a .NET compatible IDE or the command line.
- By default, the program is set to forecast the closing prices of Apple Inc. (AAPL) stock. To forecast a different stock, change the `ticker` variable in the `Main` method.

## Components

### StockData Class

Represents the structure for storing historical stock data with properties for the Date and ClosePrice.

### StockPrediction Class

Used to hold the predicted stock prices and confidence intervals.

### PolygonResponse and Result Classes

Used for deserialization of the JSON response from the Polygon.io API.

### Main Method

The entry point of the application. It orchestrates the data fetching, model training, and prediction.

## Libraries and Tools

- **Microsoft.ML**: For machine learning model creation and prediction.
- **System.Net.Http**: For making HTTP requests to the Polygon.io API.
- **System.Text.Json**: For JSON parsing.

## Limitations and Considerations

- The accuracy of the predictions depends on the quality and quantity of the historical data.
- The model's parameters are pre-set in the code and might need adjustment for optimal performance on different datasets.
- The application is designed as a simple example and does not include error handling for all possible failure scenarios.

## Disclaimer

The predictions made by this program are purely for educational purposes and should not be used as financial advice.

## Contributions

Contributions to the project are welcome. Please ensure to follow the coding standards and add unit tests for any new features or bug fixes.
