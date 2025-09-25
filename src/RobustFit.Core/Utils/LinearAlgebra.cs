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
            double mad = 1.4826 * median;
            
            // Handle edge case where MAD is zero (perfect fit)
            return mad == 0.0 ? 1.0 : mad;
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
