/// <summary>
/// Represents the Huber loss function, which is less sensitive to outliers than squared error loss.
/// </summary>
/// <remarks>
/// The Huber loss function is defined as:
/// 
/// <code>
/// L(r) = 0.5 * r^2              for |r| <= c
/// L(r) = c * |r| - 0.5 * c^2    for |r| >  c
/// </code>
/// 
/// * Psi(r) is the influence function calculated as a derivative of L(r) with respect to r, i.e. a loss gradient w.r.t. residual.
/// It behaves like the identity function for small residuals (|r| <= c) and is constant for large residuals (|r| > c).
/// In other words, it caps the influence of large residuals.
/// * Weight(r) is defined as Psi(r) / r, which is used in iteratively reweighted least squares (IRLS) algorithms.
/// It is used to express the weight assigned to each residual during optimization iterations in IRLS (Iteratively 
/// Reweighted Least Squares). 
///    If residual is small → weight = 1 (full influence). 
///    If residual is large → weight = c / |r| (downweighted)
/// </remarks>/// 
using System;
namespace RobustFit.Core.LossFunctions
{
    /// <summary>
    /// Represents the Huber loss function, which is less sensitive to outliers than squared error loss.
    /// </summary>
    /// <remarks>
    /// The Huber loss function is defined as:
    /// 
    /// <code>
    /// L(r) = 0.5 * r^2              for |r| <= c
    /// L(r) = c * |r| - 0.5 * c^2    for |r| >  c
    /// </code>
    /// 
    /// * Psi(r) is the influence function (gradient of L w.r.t. r).
    /// * Weight(r) is defined as Psi(r)/r, used in IRLS.
    /// </remarks>
    public class HuberLoss : ILossFunction
    {
        private readonly double c;

        /// <summary>
        /// Huber loss function
        /// </summary>
        /// <param name="c">Threshold where the loss switches from quadratic to linear.
        /// Default is 1.345 (95% efficiency at the normal distribution)</param>
        public HuberLoss(double c = 1.345)
        {
            if (c <= 0)
                throw new ArgumentException("The value of c (epsilon) must be positive", nameof(c));
            
            this.c = c;
        }

        public double TuningConstant => c;

        public double Loss(double r)
        {
            double absR = Math.Abs(r);
            if (absR <= c)
                return 0.5 * r * r;
            else
                return c * (absR - 0.5 * c);
        }

        public double Weight(double r)
        {
            double absR = Math.Abs(r);
            if (absR <= c)
                return 1.0;
            else
                return c / absR;
        }

        public double Psi(double r)
        {
            double absR = Math.Abs(r);
            if (absR <= c)
                return r;
            else
                return c * Math.Sign(r);
        }
    }
}