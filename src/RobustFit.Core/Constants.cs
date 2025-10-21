// Types of regressors supported by the package
namespace RobustFit.Core
{
    // Define the enum with your fixed set of values
    public enum RegressorType
    {
        OLS,        // Default value is 0
        HuberLoss,  // Value is 1
        TukeyLoss   // Value is 2
    }
}