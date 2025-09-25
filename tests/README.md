# RobustFit Tests

This directory contains unit tests and integration tests for the `RobustFit` library.

## Test Structure

The tests are organized as follows:

1. **LinearAlgebraTests.cs**
   - Tests for matrix operations
   - Vector manipulations
   - Mathematical utility functions

2. **LossFunctionTests.cs**
   - Huber loss function tests
   - Tukey loss function tests
   - Error weighting functions

3. **OLSTest.cs**
   - Ordinary Least Squares regression tests
   - Basic regression functionality
   - Multiple regression validation

4. **RobustFitTest.cs**
   - Tests for robust regression models
   - Handling of outliers in data
   - Comparison between regression methods

5. **IntegrationTests.cs**
   - End-to-end workflow testing
   - Performance with and without outliers
   - Comparative analysis between methods

## Requirements

- .NET 8.0 SDK or later
- MSTest.TestFramework (3.6.0)
- MSTest.TestAdapter (3.6.0)
- Microsoft.NET.Test.Sdk (17.11.1)

## Running the Tests

### From Command Line

```bash
# Navigate to the test project directory
cd tests/RobustFit.UnitTests

# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "ClassName=RobustFit.UnitTests.RobustFitTest"

# Run specific test method
dotnet test --filter "FullyQualifiedName=RobustFit.UnitTests.RobustFitTest.RobustRegressor_HuberLoss_HandlesOutliers"
```

### From Visual Studio

1. Open the `RobustFit.sln` solution in Visual Studio
2. Use the Test Explorer window to run or debug tests
   - View > Test Explorer
   - You can run all tests or select specific tests to run

## Test Output

Test results are stored in the `TestResults` directory and can be viewed in Visual Studio's Test Explorer or as XML reports in the output directory.

## Adding New Tests

When adding new tests:
1. Create test methods with clear naming conventions
2. Use `[TestClass]` attribute for test classes
3. Mark test methods with `[TestMethod]` attribute
4. Include appropriate assertions to validate functionality

## Code Coverage

To generate code coverage reports:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

This generates coverage data in the TestResults folder which can be converted to a report using tools like ReportGenerator.

### Viewing Coverage Reports

1. Install the ReportGenerator tool:
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

2. Generate HTML reports from coverage data:
```bash
reportgenerator -reports:TestResults/*/coverage.cobertura.xml -targetdir:coveragereport
```

3. Open the generated HTML report:
```bash
# On Linux
xdg-open coveragereport/index.html

# On Windows
start coveragereport/index.html

# On macOS
open coveragereport/index.html
```

The report provides detailed metrics on code coverage, including:
- Overall coverage percentage
- Line and branch coverage
- Uncovered code sections
- Coverage trends if generated regularly