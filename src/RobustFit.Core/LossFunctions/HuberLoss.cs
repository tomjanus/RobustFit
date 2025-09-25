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
        public HuberLoss(double c = 1.345)
        {
            this.c = c;
        }

        public double Psi(double r) =>
            Abs(r) <= c ? r : c * Math.Sign(r); // if residual is small use it, otherwise use c with the sign of r

        public double Weight(double r)
        {
            double absr = Abs(r);
            if (absr <= c) return 1.0;
            if (absr == 0.0) return 1.0; // Handle zero residual case
            return c / absr; // if residual is small weight = 1, otherwise weight = c / |r|
        }

        public double Loss(double r)
        {
            double absr = Abs(r);
            return absr <= c ? 0.5 * r * r : c * absr - 0.5 * c * c; // Huber loss function
        }

        /// <summary>
        /// Helper method for absolute value of a residual.
        /// </summary>
        private static double Abs(double r) => Math.Abs(r);
    }
}