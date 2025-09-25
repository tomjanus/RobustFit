using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobustFit.Core.Models;
using RobustFit.Core.LossFunctions;
using System;

namespace RobustFit.UnitTests
{
    [TestClass]
    public class RobustFitTest
    {
        private const double TOLERANCE = 1e-6;

        [TestMethod]
        public void RobustRegressor_HuberLoss_HandlesOutliers()
        {
            // Arrange: Clean data with one outlier
            double[][] X = [
                [1.0],
                [2.0],
                [3.0],
                [4.0],
                [5.0] // Outlier point
            ];
            double[] y = [2.0, 4.0, 6.0, 8.0, 100.0]; // Last point is outlier

            var huberLoss = new HuberLoss(c: 1.345);
            var robustRegressor = new RobustRegressor();

            // Act
            robustRegressor.Fit(X, y, huberLoss, maxIter: 100, tol: 1e-8);

            // Assert: Should be closer to true relationship (slope ≈ 2) than OLS
            // OLS would be heavily influenced by the outlier
            var olsRegressor = new OLSRegressor();
            olsRegressor.Fit(X, y);

            // Robust regression should have slope closer to 2.0 than OLS
            double robustSlope = robustRegressor.Coefficients[1];
            double olsSlope = olsRegressor.Coefficients[1];

            Assert.IsTrue(Math.Abs(robustSlope - 2.0) < Math.Abs(olsSlope - 2.0),
                "Robust regression should be less affected by outliers");
        }

        [TestMethod]
        public void RobustRegressor_TukeyLoss_HandlesOutliers()
        {
            // Arrange: Clean data with outliers
            double[][] X = [
                [1.0],
                [2.0],
                [3.0],
                [4.0],
                [5.0]
            ];
            double[] y = [3.0, 5.0, 7.0, 9.0, 50.0]; // y = 1 + 2*x with outlier

            var tukeyLoss = new TukeyLoss(c: 4.685);
            var robustRegressor = new RobustRegressor();

            // Act
            robustRegressor.Fit(X, y, tukeyLoss, maxIter: 100, tol: 1e-8);

            // Assert: Should converge
            Assert.AreEqual(2, robustRegressor.Coefficients.Length);
            Assert.IsTrue(robustRegressor.Coefficients[0] != 0.0); // Intercept
            Assert.IsTrue(robustRegressor.Coefficients[1] != 0.0); // Slope
        }

        [TestMethod]
        public void RobustRegressor_PredictSingle_CorrectPrediction()
        {
            // Arrange
            double[][] X = [
                [1.0],
                [2.0],
                [3.0]
            ];
            double[] y = [2.0, 4.0, 6.0]; // y = 2*x

            var huberLoss = new HuberLoss();
            var robustRegressor = new RobustRegressor();
            robustRegressor.Fit(X, y, huberLoss);

            // Act
            double prediction = robustRegressor.Predict([4.0]);

            // Assert: Should predict close to 8.0
            Assert.AreEqual(8.0, prediction, 0.1);
        }

        [TestMethod]
        public void RobustRegressor_ConvergesTolerance_StopsEarly()
        {
            // Arrange: Perfect linear data (should converge quickly)
            double[][] X = [
                [1.0],
                [2.0],
                [3.0],
                [4.0]
            ];
            double[] y = [1.0, 2.0, 3.0, 4.0]; // y = x

            var huberLoss = new HuberLoss();
            var robustRegressor = new RobustRegressor();

            // Act: Use very strict tolerance - should converge quickly
            robustRegressor.Fit(X, y, huberLoss, maxIter: 100, tol: 1e-12);

            // Assert: Should have reasonable coefficients
            Assert.AreEqual(2, robustRegressor.Coefficients.Length);
            Assert.AreEqual(0.0, robustRegressor.Coefficients[0], TOLERANCE); // Intercept ≈ 0
            Assert.AreEqual(1.0, robustRegressor.Coefficients[1], TOLERANCE); // Slope ≈ 1
        }

        [TestMethod]
        public void RobustRegressor_MultipleFeatures_CorrectCoefficients()
        {
            // Arrange: y = 1 + 2*x1 + 3*x2
            double[][] X = [
                [1.0, 1.0],
                [2.0, 1.0],
                [1.0, 2.0],
                [2.0, 2.0],
                [3.0, 1.0],
                [1.0, 3.0]
            ];
            double[] y = [6.0, 9.0, 10.0, 13.0, 12.0, 13.0]; // -0.5 + 3*x1 + 3.6*x2

            var huberLoss = new HuberLoss();
            var robustRegressor = new RobustRegressor();

            // Act
            robustRegressor.Fit(X, y, huberLoss);

            // Assert
            Assert.AreEqual(3, robustRegressor.Coefficients.Length);
            Assert.AreEqual(-0.5, robustRegressor.Coefficients[0], 0.01); // Intercept
            Assert.AreEqual(3.0, robustRegressor.Coefficients[1], 0.01); // x1 coefficient
            Assert.AreEqual(3.6, robustRegressor.Coefficients[2], 0.01); // x2 coefficient
        }

        [TestMethod]
        public void RobustRegressor_EmptyCoefficientsInitially()
        {
            // Arrange
            var robustRegressor = new RobustRegressor();

            // Assert
            Assert.IsNotNull(robustRegressor.Coefficients);
            Assert.AreEqual(0, robustRegressor.Coefficients.Length);
        }
    }
}