using System;
using RobustFit.Core.LossFunctions;
using RobustFit.Core.Utils;

namespace RobustFit.Core.Models
{
    public class RobustRegressor
    {
        public double[] Coefficients { get; private set; } = Array.Empty<double>();
        public bool FitIntercept { get; }

        public RobustRegressor(bool fitIntercept = true)
        {
            FitIntercept = fitIntercept;
        }

        public void Fit(double[][] X, double[] y, ILossFunction loss,
                        double alpha=0.0, int maxIter = 50, double tol = 1e-6,
                        string initialEstimate = "ols")
        {
            if (X == null) throw new ArgumentNullException(nameof(X));
            if (y == null) throw new ArgumentNullException(nameof(y));
            if (X.Length != y.Length) throw new ArgumentException("X and y must have the same number of rows.");
            if (loss == null) throw new ArgumentNullException(nameof(loss));

            int n = y.Length;
            var XA = FitIntercept ? LinearAlgebra.AddIntercept(X) : X;

            // Initial residuals and beta
            double[] beta;
            if (initialEstimate.Equals("ols", StringComparison.CurrentCultureIgnoreCase))
            {
                beta = LinearAlgebra.OLS(XA, y);
            }
            else if (initialEstimate.Equals("median", StringComparison.CurrentCultureIgnoreCase))
            {
                int p = X[0].Length + (FitIntercept ? 1 : 0);
                beta = new double[p];  // Initialize with zeros
                beta[0] = LinearAlgebra.Median(y);
            }
            else
            {
                int p = X[0].Length + (FitIntercept ? 1 : 0);
                beta = new double[p];  // Initialize all with zeros                
            }
            ;
            
            // Iteratively Reweighted Least Squares (IRLS)
            for (int iter = 0; iter < maxIter; iter++)
            {
                double[] residuals = new double[n];
                for (int i = 0; i < n; i++)
                    residuals[i] = y[i] - LinearAlgebra.DotProduct(XA[i], beta);

                double scale = LinearAlgebra.MAD(residuals) * 1.4826; // Consistency factor for normal distribution

                // Compute weights
                double[] w = new double[n];
                for (int i = 0; i < n; i++)
                    w[i] = loss.Weight(residuals[i] / scale);

                // Weighted least squares
                double[] newBeta;
                if (alpha == 0.0)
                    newBeta = LinearAlgebra.WLS(XA, y, w);
                else
                    newBeta = LinearAlgebra.RegularizedWLS(XA, y, w, alpha);

                // Convergence check
                double diff = LinearAlgebra.VectorNormDiff(beta, newBeta);
                beta = newBeta;
                if (diff < tol) break;
            }

            Coefficients = beta;
        }

        public void Fit(double[] X1D, double[] y, ILossFunction loss,
                        double alpha = 0.0, int maxIter = 50, double tol = 1e-6)
        {
            if (X1D == null) throw new ArgumentNullException(nameof(X1D));
            if (y == null) throw new ArgumentNullException(nameof(y));
            if (X1D.Length != y.Length) throw new ArgumentException("X1D and y must have the same length.");

            // Wrap 1D into 2D array
            double[][] X2D = Array.ConvertAll(X1D, v => new double[] { v });
            Fit(X2D, y, loss, alpha, maxIter, tol);
        }

        public double Predict(double[] x)
        {
            if (Coefficients == null || Coefficients.Length == 0)
                throw new InvalidOperationException("Model is not fitted. Call Fit(...) first.");

            double sum = FitIntercept ? Coefficients[0] : 0.0;
            int offset = FitIntercept ? 1 : 0;

            for (int j = 0; j < x.Length; j++)
                sum += Coefficients[j + offset] * x[j];
            return sum;
        }

        public double Predict(double x) => Predict(new double[] { x });

        public double[] Predict(double[][] X)
        {
            if (Coefficients == null || Coefficients.Length == 0)
                throw new InvalidOperationException("Model is not fitted. Call Fit(...) first.");

            double[] predictions = new double[X.Length];
            for (int i = 0; i < X.Length; i++)
                predictions[i] = Predict(X[i]);
            return predictions;
        }
    }
}
