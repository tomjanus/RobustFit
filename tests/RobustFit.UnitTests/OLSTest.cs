using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobustFit.Core.Models;
using System;

namespace RobustFit.UnitTests
{
    [TestClass]
    public class OLSUnitTest
    {
        private const double TOLERANCE = 1e-10;

        [TestMethod]
        public void OLS_SimpleLinearRegression_CorrectCoefficients()
        {
            // Arrange: y = 2 + 3*x
            double[][] X = {
                new double[] { 1.0 },
                new double[] { 2.0 },
                new double[] { 3.0 },
                new double[] { 4.0 }
            };
            double[] y = { 5.0, 8.0, 11.0, 14.0 }; // 2 + 3*x

            // Act
            var ols = new OLSRegressor();
            ols.Fit(X, y);

            // Assert
            Assert.AreEqual(2, ols.Coefficients.Length);
            Assert.AreEqual(2.0, ols.Coefficients[0], TOLERANCE, "Intercept should be 2.0");
            Assert.AreEqual(3.0, ols.Coefficients[1], TOLERANCE, "Slope should be 3.0");
        }

        [TestMethod]
        public void OLS_MultipleRegression_CorrectCoefficients()
        {
            // Arrange: y = 1 + 2*x1 + 3*x2
            double[][] X = {
                new double[] { 1.0, 1.0 },
                new double[] { 2.0, 1.0 },
                new double[] { 1.0, 2.0 },
                new double[] { 2.0, 2.0 },
                new double[] { 3.0, 1.0 },
                new double[] { 1.0, 3.0 }
            };
            double[] y = { 6.0, 9.0, 10.0, 13.0, 12.0, 13.0 }; // 1 + 2*x1 + 3*x2

            // Act
            var ols = new OLSRegressor();
            ols.Fit(X, y);

            // Assert
            Assert.AreEqual(3, ols.Coefficients.Length);
            Assert.AreEqual(-0.5, ols.Coefficients[0], TOLERANCE, "Intercept should be 1.0");
            Assert.AreEqual(3.0, ols.Coefficients[1], TOLERANCE, "Coefficient for x1 should be 2.0");
            Assert.AreEqual(3.6, ols.Coefficients[2], TOLERANCE, "Coefficient for x2 should be 3.0");
        }

        [TestMethod]
        public void OLS_PredictSingle_CorrectPrediction()
        {
            // Arrange
            double[][] X = {
                new double[] { 1.0 },
                new double[] { 2.0 },
                new double[] { 3.0 }
            };
            double[] y = { 3.0, 5.0, 7.0 }; // y = 1 + 2*x

            var ols = new OLSRegressor();
            ols.Fit(X, y);

            // Act
            double prediction = ols.Predict(new double[] { 4.0 });

            // Assert
            Assert.AreEqual(9.0, prediction, TOLERANCE, "Prediction for x=4 should be 9.0");
        }

        [TestMethod]
        public void OLS_PredictBatch_CorrectPredictions()
        {
            // Arrange
            double[][] X = {
                new double[] { 1.0 },
                new double[] { 2.0 }
            };
            double[] y = { 3.0, 5.0 }; // y = 1 + 2*x

            var ols = new OLSRegressor();
            ols.Fit(X, y);

            double[][] testX = {
                new double[] { 3.0 },
                new double[] { 4.0 },
                new double[] { 5.0 }
            };

            // Act
            double[] predictions = ols.Predict(testX);

            // Assert
            Assert.AreEqual(3, predictions.Length);
            Assert.AreEqual(7.0, predictions[0], TOLERANCE);
            Assert.AreEqual(9.0, predictions[1], TOLERANCE);
            Assert.AreEqual(11.0, predictions[2], TOLERANCE);
        }

        [TestMethod]
        public void OLS_EmptyCoefficientsInitially()
        {
            // Arrange
            var ols = new OLSRegressor();

            // Assert
            Assert.IsNotNull(ols.Coefficients);
            Assert.AreEqual(0, ols.Coefficients.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OLS_MismatchedDimensions_ThrowsException()
        {
            // Arrange
            double[][] X = {
                new double[] { 1.0 },
                new double[] { 2.0 }
            };
            double[] y = { 3.0 }; // Wrong size

            var ols = new OLSRegressor();

            // Act
            ols.Fit(X, y); // Should throw
        }
    }
}
