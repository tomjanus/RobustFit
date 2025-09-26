 /// <summary>
 /// Defines the interface for loss functions used in robust fitting algorithms.
 /// Loss functions are used to reduce the influence of outliers in the data.
 /// The interface specifies two key methods: Psi and Weight.
 /// - Psi(r): The influence function, which is the derivative of the loss function $\rho(r)$ 
 ///    with respect to the residual $r$. This function determines how much influence each 
 ///    residual has on the parameter estimates. For example, in the Huber loss, $\psi(r)$ is 
 ///    linear for small residuals and constant for large residuals, limiting the influence of 
 ///    outliers.
 /// - Weight(r): The weight function used in Iteratively Reweighted Least Squares (IRLS),
 ///    calculated as psi(r)/r, where r is the residual. These weights are crucial for the iterative 
 ///    process, as they down-weight observations with large residuals (potential outliers) during 
 ///    each iteration of the fitting procedure. The weight function ensures that the algorithm is 
 ///    less sensitive to extreme values compared to ordinary least squares regression (OLS).
 /// </summary>
namespace RobustFit.Core.LossFunctions
{
    public interface ILossFunction
    {
        double Psi(double r);    // influence function (derivative of rho(r))
        double Weight(double r); // weight used in IRLS (psi(r)/r)
        double Loss(double r);   // loss function rho(r)
        /// <summary>
        /// Gets the tuning constant used by the loss function.
        /// </summary>
        double TuningConstant { get; }
    }
}