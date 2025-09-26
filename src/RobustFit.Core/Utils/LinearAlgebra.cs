using System;
using System.Linq;

namespace RobustFit.Core.Utils
{
    public static class LinearAlgebra
    {
        public static double[][] AddIntercept(double[][] X)
        {
            int n = X.Length; // number of rows
            int p = X[0].Length; // number of columns
            double[][] XA = new double[n][];
            for (int i = 0; i < n; i++)
            {
                XA[i] = new double[p + 1];
                XA[i][0] = 1.0;
                for (int j = 0; j < p; j++)
                    XA[i][j + 1] = X[i][j];
            }
            return XA;
        }

        public static double[] Residuals(double[][] XA, double[] y, double[] beta)
        {
            int n = y.Length;
            double[] r = new double[n];
            for (int i = 0; i < n; i++)
            {
                double pred = 0;
                for (int j = 0; j < beta.Length; j++)
                    pred += XA[i][j] * beta[j];
                r[i] = y[i] - pred;
            }
            return r;
        }

        public static double MAD(double[] residuals)
        {
            double[] absr = residuals.Select(r => Math.Abs(r)).ToArray();
            Array.Sort(absr);
            double median = absr[absr.Length / 2];
            //double mad = 1.4826 * median;
            double mad = median;
            if (mad < 1e-6) mad = 1e-6;
            return mad;
        }
        
        /// <summary>
        /// Calculates the median value of an array.
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The median value</returns>
        public static double Median(double[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException("Cannot compute median of empty array");
            
            // Create a copy to avoid modifying the original array
            double[] sortedValues = (double[])values.Clone();
            Array.Sort(sortedValues);
            
            int n = sortedValues.Length;
            if (n % 2 == 0)
            {
                // Even number of elements - average the two middle values
                return (sortedValues[n / 2 - 1] + sortedValues[n / 2]) / 2.0;
            }
            else
            {
                // Odd number of elements - return the middle value
                return sortedValues[n / 2];
            }
        }

        public static double[] OLS(double[][] XA, double[] y)
        {
            // Check dimensions
            if (XA.Length != y.Length)
                throw new ArgumentException($"Number of rows in X ({XA.Length}) must match length of y ({y.Length})");

            var XT = Transpose(XA);
            var XTX = Multiply(XT, XA);
            var XTy = Multiply(XT, y);
            return Solve(XTX, XTy);
        }

        public static double[] WLS(double[][] XA, double[] y, double[] w)
        {
            int n = y.Length;
            int p = XA[0].Length;
            double[][] Xw = new double[n][];
            double[] yw = new double[n];
            for (int i = 0; i < n; i++)
            {
                double wi = Math.Sqrt(w[i]);
                Xw[i] = new double[p];
                for (int j = 0; j < p; j++)
                    Xw[i][j] = XA[i][j] * wi;
                yw[i] = y[i] * wi;
            }
            return OLS(Xw, yw);
        }

        /// <summary>
        /// Performs regularized weighted least squares regression.
        /// </summary>
        /// <param name="X">The feature matrix.</param>
        /// <param name="y">The target vector.</param>
        /// <param name="weights">The weights for each observation.</param>
        /// <param name="alpha">Regularization strength (0 = no regularization).</param>
        /// <returns>The coefficient vector including intercept (if applicable).</returns>
        public static double[] RegularizedWLS(double[][] X, double[] y, double[] weights, double alpha)
        {
            if (X == null) throw new ArgumentNullException(nameof(X));
            if (y == null) throw new ArgumentNullException(nameof(y));
            if (weights == null) throw new ArgumentNullException(nameof(weights));
            if (X.Length != y.Length || X.Length != weights.Length) 
                throw new ArgumentException("X, y, and weights must have the same number of rows.");
            
            int n = X.Length;       // Number of observations
            int p = X[0].Length;    // Number of features (including intercept if present)
            
            // Create matrices for the system: (X'WX + αI)β = X'Wy
            double[][] XtWX = new double[p][];
            for (int i = 0; i < p; i++)
                XtWX[i] = new double[p];
            
            // Calculate X'WX
            for (int i = 0; i < p; i++)
            {
                for (int j = i; j < p; j++)  // Use symmetry to optimize
                {
                    double sum = 0.0;
                    for (int k = 0; k < n; k++)
                    {
                        sum += X[k][i] * weights[k] * X[k][j];
                    }
                    XtWX[i][j] = sum;
                    if (i != j) XtWX[j][i] = sum;  // Fill in symmetric element
                }
            }
            
            // Add regularization term (α*I) to X'WX
            for (int i = 0; i < p; i++)
            {
                XtWX[i][i] += alpha;
            }
            
            // Calculate X'Wy vector
            double[] XtWy = new double[p];
            for (int i = 0; i < p; i++)
            {
                double sum = 0.0;
                for (int k = 0; k < n; k++)
                {
                    sum += X[k][i] * weights[k] * y[k];
                }
                XtWy[i] = sum;
            }
            
            // Solve the system (X'WX + αI)β = X'Wy
            return SolveLinearSystem(XtWX, XtWy);
        }

        /// <summary>
        /// Solves a linear system of equations Ax = b using Cholesky decomposition.
        /// </summary>
        private static double[] SolveLinearSystem(double[][] A, double[] b)
        {
            int n = A.Length;
            
            // Create a copy of A to avoid modifying the input
            double[][] L = new double[n][];
            for (int i = 0; i < n; i++)
            {
                L[i] = new double[n];
                Array.Copy(A[i], L[i], n);
            }
            
            // Perform Cholesky decomposition: A = LL^T
            for (int i = 0; i < n; i++)
            {
                // Diagonal element
                double sum = L[i][i];
                for (int k = 0; k < i; k++)
                    sum -= L[i][k] * L[i][k];
                
                if (sum <= 0)
                    throw new InvalidOperationException("Matrix is not positive definite");
                
                L[i][i] = Math.Sqrt(sum);
                
                // Elements below diagonal in column i
                for (int j = i + 1; j < n; j++)
                {
                    sum = L[j][i];
                    for (int k = 0; k < i; k++)
                        sum -= L[i][k] * L[j][k];
                    
                    L[j][i] = sum / L[i][i];
                    L[i][j] = 0;  // Zero out upper triangle
                }
            }
            
            // Forward substitution to solve Ly = b
            double[] y = new double[n];
            for (int i = 0; i < n; i++)
            {
                double sum = b[i];
                for (int j = 0; j < i; j++)
                    sum -= L[i][j] * y[j];
                
                y[i] = sum / L[i][i];
            }
            
            // Back substitution to solve L^Tx = y
            double[] x = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                double sum = y[i];
                for (int j = i + 1; j < n; j++)
                    sum -= L[j][i] * x[j];
                
                x[i] = sum / L[i][i];
            }
            
            return x;
        }

        public static double VectorNormDiff(double[] a, double[] b)
        {
            double sum = 0;
            for (int i = 0; i < a.Length; i++)
                sum += (a[i] - b[i]) * (a[i] - b[i]);
            return Math.Sqrt(sum);
        }

        // Minimal linear algebra
        private static double[][] Transpose(double[][] A)
        {
            int n = A.Length, m = A[0].Length;
            double[][] T = new double[m][];
            for (int j = 0; j < m; j++)
            {
                T[j] = new double[n];
                for (int i = 0; i < n; i++) T[j][i] = A[i][j];
            }
            return T;
        }

        private static double[][] Multiply(double[][] A, double[][] B)
        {
            int n = A.Length, m = B[0].Length, p = A[0].Length;
            double[][] C = new double[n][];
            for (int i = 0; i < n; i++)
            {
                C[i] = new double[m];
                for (int j = 0; j < m; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < p; k++)
                        sum += A[i][k] * B[k][j];
                    C[i][j] = sum;
                }
            }
            return C;
        }

        public static double DotProduct(double[] a, double[] b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vectors must be of the same length");
                
            double sum = 0.0;
            for (int i = 0; i < a.Length; i++)
                sum += a[i] * b[i];
            return sum;
        }

        private static double[] Multiply(double[][] A, double[] v)
        {
            int n = A.Length, p = A[0].Length;

            // Check dimensions
            if (v.Length != p)
                throw new ArgumentException($"Matrix columns ({p}) must match vector length ({v.Length})");

            double[] res = new double[n];
            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < p; j++) sum += A[i][j] * v[j];
                res[i] = sum;
            }
            return res;
        }

        private static double[] Solve(double[][] A, double[] b)
        {
            // Basic Gaussian elimination
            int n = b.Length;
            double[][] M = new double[n][];
            for (int i = 0; i < n; i++)
            {
                M[i] = new double[n + 1];
                for (int j = 0; j < n; j++) M[i][j] = A[i][j];
                M[i][n] = b[i];
            }
            for (int i = 0; i < n; i++)
            {
                double pivot = M[i][i];
                for (int j = i; j <= n; j++) M[i][j] /= pivot;
                for (int k = i + 1; k < n; k++)
                {
                    double factor = M[k][i];
                    for (int j = i; j <= n; j++) M[k][j] -= factor * M[i][j];
                }
            }
            double[] x = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                double sum = M[i][n];
                for (int j = i + 1; j < n; j++) sum -= M[i][j] * x[j];
                x[i] = sum;
            }
            return x;
        }
    }
}
