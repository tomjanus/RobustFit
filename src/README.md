# RobustFit.Core Library

## Overview
RobustFit.Core is a .NET library for robust regression analysis, designed for fitting statistical models that are resistant to outliers. The library implements iteratively reweighted least squares (IRLS) methods with various robust loss functions, making it suitable for real-world datasets where anomalies may exist.

[![NuGet](https://img.shields.io/nuget/v/RobustFit.Core.svg)](https://www.nuget.org/packages/RobustFit.Core/)

## Features

### Regression Models
- **OLSRegressor**: Standard Ordinary Least Squares regression
- **RobustRegressor**: Robust regression models resistant to outliers

### Loss Functions
- **HuberLoss**: Huber loss function (default c=1.345) for a balance between efficiency and robustness
- **TukeyLoss**: Tukey's bisquare loss function (default c=4.685) for complete resistance to outliers
- **ILossFunction**: Interface for creating custom loss functions

### Utility Functions
- **Linear Algebra**: Matrix operations, vector manipulations, residual calculation
- **Statistical Measures**: MAD (Median Absolute Deviation) calculations

## Installation

### NuGet Package Manager
```
Install-Package RobustFit.Core
```

### .NET CLI
```
dotnet add package RobustFit.Core
```

### Project Reference
```xml
<PackageReference Include="RobustFit.Core" Version="1.0.0" />
```

## Usage Examples

### Ordinary Least Squares Regression
```csharp
using RobustFit.Core.Models;

// Create and fit OLS model
var ols = new OLSRegressor();
ols.Fit(X, y);

// Make predictions
double olsPrediction = ols.Predict(testPoint);
```

### Robust Regression with Huber Loss
```csharp
using RobustFit.Core.Models;
using RobustFit.Core.LossFunctions;

// Create loss function
var huberLoss = new HuberLoss(c: 1.345);

// Create and fit robust model
var robust = new RobustRegressor();
robust.Fit(X, y, huberLoss, maxIter: 100, tol: 1e-6);

// Make predictions
double robustPrediction = robust.Predict(testPoint);
```

### Robust Regression with Tukey Loss
```csharp
using RobustFit.Core.Models;
using RobustFit.Core.LossFunctions;

// Create loss function
var tukeyLoss = new TukeyLoss(c: 4.685);

// Create and fit robust model
var robust = new RobustRegressor();
robust.Fit(X, y, tukeyLoss, maxIter: 100, tol: 1e-6);

// Make predictions
double robustPrediction = robust.Predict(testPoint);
```

### Custom Loss Functions
```csharp
using RobustFit.Core.LossFunctions;

// Implement custom loss function
public class CustomLoss : ILossFunction
{
    public double Psi(double r) => /* Your influence function */;
    public double Weight(double r) => /* Your weighting function */;
    public double Loss(double r) => /* Your loss function */;
}
```

## Performance Notes
- Robust regression is typically more computationally intensive than OLS
- Convergence is controlled by `maxIter` and `tol` parameters
- For large datasets, consider preprocessing to remove obvious outliers first

## Contributing
Contributions are welcome! Please feel free to submit a pull request or open an issue for any enhancements or bug fixes.

## License
This project is licensed under the MIT License. See the LICENSE file for more details.
