using System;
using RobustFit.Core.Utils;

namespace RobustFit.Core.Models
{
    public class OLSRegressor
    {
        public double[] Coefficients { get; private set; } = Array.Empty<double>();
        public bool FitIntercept { get; }

        public OLSRegressor(bool fitIntercept = true)
        {
            FitIntercept = fitIntercept;
        }

        public void Fit(double[][] X, double[] y)
        {
            if (X == null) throw new ArgumentNullException(nameof(X));
            if (y == null) throw new ArgumentNullException(nameof(y));
            if (X.Length != y.Length) throw new ArgumentException("X and y must have the same number of rows.");

            var XA = FitIntercept ? LinearAlgebra.AddIntercept(X) : X;
            Coefficients = LinearAlgebra.OLS(XA, y);
        }

        public void Fit(double[] X1D, double[] y)
        {
            if (X1D == null) throw new ArgumentNullException(nameof(X1D));
            if (y == null) throw new ArgumentNullException(nameof(y));
            if (X1D.Length != y.Length) throw new ArgumentException("X1D and y must have the same length.");

            // Wrap 1D into 2D array
            double[][] X2D = Array.ConvertAll(X1D, v => new double[] { v });
            Fit(X2D, y);
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
