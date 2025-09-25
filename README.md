# RobustFit

[![NuGet](https://img.shields.io/nuget/v/RobustFit.Core.svg)](https://www.nuget.org/packages/RobustFit.Core/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)]()

RobustFit is a .NET library for **ordinary least squares (OLS)** and **robust regression** analysis, designed to handle datasets that contain outliers and noise. It implements iteratively reweighted least squares (IRLS) methods with various robust loss functions.

```
 ____            _                     _     _____   _   _   
|  _ \    ___   | |__    _   _   ___  | |_  |  ___| (_) | |_ 
| |_) |  / _ \  | '_ \  | | | | / __| | __| | |_    | | | __|
|  _ <  | (_) | | |_) | | |_| | \__ \ | |_  |  _|   | | | |_ 
|_| \_\  \___/  |_.__/   \__,_| |___/  \__| |_|     |_|  \__|
```

<div align="center">
  <img src="src/RobustFit.Core/icon/icon.png" alt="RobustFit Logo" width="200">
</div>

## Table of Contents

- [RobustFit](#robustfit)
  - [Table of Contents](#table-of-contents)
  - [Features](#features)
  - [Installation](#installation)
    - [NuGet Package Manager](#nuget-package-manager)
    - [.NET CLI](#net-cli)
    - [Package Reference in .csproj](#package-reference-in-csproj)
  - [Project Structure](#project-structure)
  - [Building](#building)
    - [Prerequisites](#prerequisites)
    - [Build Commands](#build-commands)
    - [Creating NuGet Package (e.g. for local use)](#creating-nuget-package-eg-for-local-use)
  - [Running Tests](#running-tests)
  - [Running the Demo](#running-the-demo)
  - [Usage Examples](#usage-examples)
    - [Ordinary Least Squares (OLS) Regression](#ordinary-least-squares-ols-regression)
    - [Robust Regression with Huber Loss](#robust-regression-with-huber-loss)
  - [Documentation](#documentation)
  - [Contributing](#contributing)
  - [License](#license)

## Features

- **Ordinary Least Squares (OLS)** regression for standard linear models
- **Robust regression** with configurable loss functions:
  - **Huber loss** - balances efficiency and resistance to outliers
  - **Tukey (bisquare) loss** - completely nullifies the effect of extreme outliers
  - Support for custom loss functions through the `ILossFunction` interface
- **Iteratively Reweighted Least Squares (IRLS)** implementation for robust fitting
- Consistent API for fitting models and making predictions
- Support for single- and multi-feature datasets
- Comprehensive linear algebra utilities
- Full test coverage with MSTest

## Installation

### NuGet Package Manager

```
Install-Package RobustFit.Core
```

### .NET CLI

```bash
dotnet add package RobustFit.Core
```

### Package Reference in .csproj

```xml
<PackageReference Include="RobustFit.Core" Version="1.0.0" />
```

## Project Structure

```
RobustFit/
├── artifacts/               # NuGet packages
├── samples/                 # Example applications
│   └── RobustFit.Demo/      # Console demo application
├── src/                     # Source code
│   └── RobustFit.Core/      # Core library
│       ├── LossFunctions/   # Loss function implementations
│       ├── Models/          # Regression model implementations
│       └── Utils/           # Helper functions and utilities
└── tests/                   # Test projects
    └── RobustFit.UnitTests/ # Unit and integration tests
```

For detailed information about each component:
- [Core Library Documentation](src/README.md)
- [Demo Application Guide](samples/README.md)
- [Tests Documentation](tests/README.md)

## Building

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022, VS Code, or any .NET-compatible IDE

### Build Commands

Build the entire solution:

```bash
# From the root directory
dotnet build

# Build specific project
dotnet build src/RobustFit.Core
```

### Creating NuGet Package (e.g. for local use)

```bash
# From the src/RobustFit.Core directory
dotnet pack -c Release -o ../../artifacts
```

Alternatively, use the provided build script:

```bash
./build.sh
```

## Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate coverage report (requires ReportGenerator tool)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:tests/RobustFit.UnitTests/TestResults/*/coverage.cobertura.xml -targetdir:coveragereport
```

See the [Tests Documentation](tests/README.md) for more detailed information on testing.

## Running the Demo

The RobustFit.Demo project demonstrates the differences between OLS and robust regression methods when dealing with outliers.

```bash
# From the root directory
cd samples/RobustFit.Demo
dotnet run
```

Or using Visual Studio:
1. Set `RobustFit.Demo` as the startup project
2. Press F5 or click "Start Debugging"

See the [Demo Documentation](samples/README.md) for more details.

## Usage Examples

### Ordinary Least Squares (OLS) Regression

```csharp
using RobustFit.Core.Models;

// Prepare data
double[][] X = [
    [ 1.0, 2.0 ],
    [ 2.0, 3.0 ],
    [ 3.0, 4.0 ]
];
double[] y = [5.0, 7.0, 9.0];

// Create and fit model
var ols = new OLSRegressor();
ols.Fit(X, y);

// Make predictions
double prediction = ols.Predict([4.0, 5.0 ]);
Console.WriteLine($"OLS prediction: {prediction}");

// Access coefficients
Console.WriteLine($"Intercept: {ols.Coefficients[0]}");
Console.WriteLine($"Feature 1 coefficient: {ols.Coefficients[1]}");
Console.WriteLine($"Feature 2 coefficient: {ols.Coefficients[2]}");
```

### Robust Regression with Huber Loss

```csharp
using RobustFit.Core.Models;
using RobustFit.Core.LossFunctions;

// Create data with outliers
double[][] X = [
    [1.0],
    [2.0],
    [3.0],
    [4.0],
    [5.0] // This will be an outlier point
];
double[] y = [2.0, 4.0, 6.0, 8.0, 100.0]; // Last point is outlier

// Create loss function
var huberLoss = new HuberLoss(c: 1.345);

// Create and fit model
var robust = new RobustRegressor();
robust.Fit(X, y, huberLoss, maxIter: 100, tol: 1e-8);

// Make prediction
double prediction = robust.Predict([6.0]);
Console.WriteLine($"Robust prediction: {prediction}");
```

For more usage examples and API details, see the [Core Library Documentation](src/README.md).

## Documentation

- [Core Library Details](src/README.md)
- [Demo Application Guide](samples/README.md)
- [Testing Information](tests/README.md)

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue for any enhancements, bug fixes, or questions.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.


