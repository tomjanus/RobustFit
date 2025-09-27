# RobustFit Demonstration

```
 ____            _                     _     _____   _   _   
|  _ \    ___   | |__    _   _   ___  | |_  |  ___| (_) | |_ 
| |_) |  / _ \  | '_ \  | | | | / __| | __| | |_    | | | __|
|  _ <  | (_) | | |_) | | |_| | \__ \ | |_  |  _|   | | | |_ 
|_| \_\  \___/  |_.__/   \__,_| |___/  \__| |_|     |_|  \__|
```

## Running the Demo

### Prerequisites
- .NET 8.0 SDK or later
- Visual Studio 2022, VS Code, or any .NET-compatible IDE

### Steps to Run

1. **From the command line:**
   ```bash
   # Navigate to the demo project directory
   cd RobustFit.Demo
   
   # Build and run the project
   dotnet run
   ```

2. **From Visual Studio:**
   - Open the `RobustFit.sln` solution file in Visual Studio
   - Set `RobustFit.Demo` as the startup project
   - Press F5 or click the "Start Debugging" button

The demo application will generate sample data with outliers and demonstrate the differences between Ordinary Least Squares (OLS) and Robust Regression methods using both Huber and Tukey loss functions.

## Library Demo

### Data Summary  

**Dataset Info**  
- Points: 200 (with 10 outliers)  
- Features: 2  
- Samples: 200  

**Equation used to generate data:**  
`y = 2.0 + 3.0 * (Feature 1() + 1.5 * (Feature 2) + noise`  
Where `Feature 1` and `Feature 2` are two predictor variables (features) and `noise` is an additive
noise term drawn from a continuous uniform distribution on the interval [-0.25, +0.25].

---

### 1. Ordinary Least Squares (OLS)

```
┌─────────────┬───────┐
│ Coefficient │ Value │
├─────────────┼───────┤
│ Intercept   │ 1.563 │
│ Feature 1   │ 2.924 │
│ Feature 2   │ 1.844 │
└─────────────┴───────┘
Prediction for [2, -2]: 3.724
```

---

### 2. Robust Regression (Huber Loss)

```
┌─────────────┬───────┐
│ Coefficient │ Value │
├─────────────┼───────┤
│ Intercept   │ 1.975 │
│ Feature 1   │ 3.006 │
│ Feature 2   │ 1.506 │
└─────────────┴───────┘
Prediction for [2, -2]: 4.974
```

---

### 3. Robust Regression (Tukey Loss)

```
┌─────────────┬───────┐
│ Coefficient │ Value │
├─────────────┼───────┤
│ Intercept   │ 1.983 │
│ Feature 1   │ 3.009 │
│ Feature 2   │ 1.505 │
└─────────────┴───────┘
Prediction for [2, -2]: 4.991
```

---

### 4. Comparison of Predictions for [2, -2]

```
┌─Model Comparison─────────────────────────────────────────────────────────────┐
│ OLS Prediction: 3.724                                                        │
│ Robust Prediction (Huber Loss): 4.974                                        │
│ Robust Prediction (Tukey Loss): 4.991                                        │
└──────────────────────────────────────────────────────────────────────────────┘
```

---

### 5. Batch Predictions

```
┌─────────────────────────────┬───────────────────┬───────────────────────────┬───────────────────────────┐
│ Input                       │ OLS               │ Robust (Huber Loss)       │ Robust (Tukey Loss)       │
├─────────────────────────────┼───────────────────┼───────────────────────────┼───────────────────────────┤
│ [1, 0.5]                    │ 5.409             │ 5.734                     │ 5.745                     │
│ [2, 1]                      │ 9.254             │ 9.493                     │ 9.506                     │
│ [3, 1.5]                    │ 13.100            │ 13.251                    │ 13.267                    │
│ [5, 1]                      │ 18.026            │ 18.510                    │ 18.533                    │
│ [-1, -1.5]                  │ -4.126            │ -3.290                    │ -3.283                    │
│ [-2, -1]                    │ -6.128            │ -5.542                    │ -5.539                    │
│ [-3, -0.5]                  │ -8.130            │ -7.795                    │ -7.796                    │
└─────────────────────────────┴───────────────────┴───────────────────────────┴───────────────────────────┘
```

---

## License

MIT License. See [LICENSE](LICENSE) for details.
