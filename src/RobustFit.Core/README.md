# RobustFit.Core

A lightweight .NET library for robust regression analysis, designed to provide reliable estimates in the presence of outliers and anomalies in datasets.

[![NuGet](https://img.shields.io/nuget/v/RobustFit.Core.svg)](https://www.nuget.org/packages/RobustFit.Core/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Features

- **OLS Regression**: Standard ordinary least squares implementation
- **Robust Regression**: IRLS (Iteratively Reweighted Least Squares) implementation
- **Multiple Loss Functions**:
  - Huber loss - balanced approach with tunable sensitivity to outliers
  - Tukey (bisquare) loss - complete outlier resistance for extreme cases
  - Custom loss function support via `ILossFunction` interface
- **Simple API**: Consistent interfaces for all models
- **Comprehensive Linear Algebra**: Matrix operations, residuals calculation, and more
- **Performance Optimized**: Efficient implementation for .NET applications

## Installation

```
Install-Package RobustFit.Core
```

Or with .NET CLI:

```
dotnet add package RobustFit.Core
```

## Quick Start

### Ordinary Least Squares (OLS)

```csharp
using RobustFit.Core.Models;

// Create and fit OLS model
var ols = new OLSRegressor();
ols.Fit(X, y);

// Make predictions
double prediction = ols.Predict(newDataPoint);
```

### Robust Regression with Huber Loss

```csharp
using RobustFit.Core.Models;
using RobustFit.Core.LossFunctions;

// Create loss function with tuning parameter
var huberLoss = new HuberLoss(c: 1.345);

// Create and fit robust model
var robust = new RobustRegressor();
robust.Fit(X, y, huberLoss, maxIter: 100, tol: 1e-6);

// Make predictions
double robustPrediction = robust.Predict(newDataPoint);
```

## Documentation and Source Code

- Full documentation and examples: [GitHub Wiki](https://github.com/tomjanus/RobustFit/wiki)
- Source code: [GitHub Repository](https://github.com/tomjanus/RobustFit)
- Sample application: See the demo project in the GitHub repository

## When to Use

- When your dataset contains outliers or anomalies
- When you need regression models that are resistant to extreme values
- When standard OLS regression produces poor results due to influential points

## License

This package is licensed under the [MIT License](https://opensource.org/licenses/MIT).

## Author

Tomasz Janus, University of Manchester

---

*RobustFit.Core is part of the RobustFit project, a suite of statistical tools for robust data analysis in .NET.*