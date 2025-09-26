/// <summary>
/// Represents the Tukey loss function, also known as bisquare loss.
/// This loss function gives less weight to large residuals.
/// </summary>
/// <remarks>
/// The Tukey loss function is defined as:
/// 
/// <code>
/// L(r) = (c^2 / 6) * [1 - (1 - (r/c)^2)^3]   for |r| <= c
/// L(r) = (c^2 / 6)                           for |r| >  c
/// <code>
/// 
/// * Psi(r) is the influence function calculated as a derivative of L(r) with respect to r.
/// It behaves like the identity function for small residuals <c>(|r| <= c)</c> and goes to zero for large residuals <c>(|r| > c)</c>.
/// * Weight(r) is defined as Psi(r) / r, which is used in iteratively reweighted least squares (IRLS) algorithms.
/// It is used to express the weight assigned to each residual during optimization iterations in IRLS (Iteratively 
/// Reweighted Least Squares). 
///    If residual is small → weight = <c>(1 - (r/c)^2)^2</c> (full influence for very small residuals, decreasing to 0 as |r| approaches c). 
///    If residual is large → weight = 0 (no influence)
/// </remarks>

using System;

namespace RobustFit.Core.LossFunctions
{
    /// <summary>
    /// Represents the Tukey loss function (bisquare loss).
    /// This loss function gives progressively less weight to large residuals,
    /// completely ignoring those beyond the tuning constant c.
    /// </summary>
    public class TukeyLoss : ILossFunction
    {

        /// <summary>
        /// The tuning constant for the Tukey loss function.  The default value is 4.685.
        /// </summary>
        private readonly double c;
        private readonly double c2; // store c^2 for efficiency

        /// <param name="c">
        /// Tuning constant. The default value (4.685) achieves ~95% efficiency
        /// under Gaussian errors while being robust to outliers.
        /// </param>
        public TukeyLoss(double c = 4.685)
        {
            this.c = c;
            c2 = c * c;
        }

        public double TuningConstant => c;

        /// <summary>
        /// Computes the psi function (derivative of the loss function) for a given residual.
        /// </summary>
        /// <param name="r">The residual.</param>
        /// <returns>The value of the psi function for the given residual.</returns>
        public double Psi(double r)
        {
            double r2 = r * r;
            if (r2 <= c2)
            {
                double t = 1 - r2 / c2;
                return r * t * t;
            }
            return 0.0;
        }

        /// <summary>
        /// Computes the weight for a given residual.
        /// </summary>
        /// <param name="r">The residual.</param>
        /// <returns>The weight for the given residual.</returns>
        public double Weight(double r)
        {
            double r2 = r * r;
            if (r2 <= c2)
            {
                double t = 1 - r2 / c2;
                return t * t;
            }
            return 0.0;
        }

        public double WeightAlt(double r)
        {
            double w;
            if (Math.Abs(r) <= 1.0)
                w = Math.Pow(1.0 - r * r, 2);
            else
                w = 0.0;
            return w;
        }

        /// <summary>
        /// Computes the loss value L(r).
        /// Not required for IRLS but useful for evaluation and diagnostics.
        /// </summary>
        public double Loss(double r)
        {
            double r2 = r * r;
            if (r2 <= c2)
            {
                double t = 1.0 - r2 / c2;
                return c2 / 6.0 * (1.0 - t * t * t);
            }
            return c2 / 6.0;
        }
    }
}
