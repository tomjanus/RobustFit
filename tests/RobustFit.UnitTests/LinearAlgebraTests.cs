using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobustFit.Core.Utils;
using System;
using System.Linq;

namespace RobustFit.UnitTests
{
    [TestClass]
    public class LinearAlgebraTests
    {
        private const double TOLERANCE = 1e-10;

        [TestMethod]
        public void AddIntercept_SimpleMatrix_AddsInterceptColumn()
        {
            // Arrange
            double[][] X = [
                [1.0, 2.0],
                [3.0, 4.0],
                [5.0, 6.0]
            ];

            // Act
            double[][] XA = LinearAlgebra.AddIntercept(X);

            // Assert
            Assert.AreEqual(3, XA.Length); // Same number of rows
            Assert.AreEqual(3, XA[0].Length); // One additional column

            // Check intercept column (first column should be all 1s)
            for (int i = 0; i < XA.Length; i++)
            {
                Assert.AreEqual(1.0, XA[i][0], TOLERANCE, $"Row {i}, intercept should be 1.0");
            }

            // Check original data is preserved
            Assert.AreEqual(1.0, XA[0][1], TOLERANCE);
            Assert.AreEqual(2.0, XA[0][2], TOLERANCE);
            Assert.AreEqual(3.0, XA[1][1], TOLERANCE);
            Assert.AreEqual(4.0, XA[1][2], TOLERANCE);
            Assert.AreEqual(5.0, XA[2][1], TOLERANCE);
            Assert.AreEqual(6.0, XA[2][2], TOLERANCE);
        }

        [TestMethod]
        public void AddIntercept_SingleColumn_CorrectDimensions()
        {
            // Arrange
            double[][] X = [
                [10.0],
                [20.0]
            ];

            // Act
            double[][] XA = LinearAlgebra.AddIntercept(X);

            // Assert
            Assert.AreEqual(2, XA.Length);
            Assert.AreEqual(2, XA[0].Length);
            Assert.AreEqual(1.0, XA[0][0], TOLERANCE);
            Assert.AreEqual(10.0, XA[0][1], TOLERANCE);
            Assert.AreEqual(1.0, XA[1][0], TOLERANCE);
            Assert.AreEqual(20.0, XA[1][1], TOLERANCE);
        }

        [TestMethod]
        public void Residuals_LinearModel_CorrectCalculation()
        {
            // Arrange: y = 1 + 2*x, so residuals should be y - (1 + 2*x)
            double[][] XA = [
                [1.0, 1.0], // intercept and x=1
                [1.0, 2.0], // intercept and x=2
                [1.0, 3.0]  // intercept and x=3
            ];
            double[] y = [3.5, 5.1, 6.9]; // Slightly off from perfect y = 1 + 2*x
            double[] beta = [1.0, 2.0]; // Perfect coefficients

            // Act
            double[] residuals = LinearAlgebra.Residuals(XA, y, beta);

            // Assert
            Assert.AreEqual(3, residuals.Length);
            Assert.AreEqual(0.5, residuals[0], TOLERANCE); // 3.5 - (1 + 2*1) = 0.5
            Assert.AreEqual(0.1, residuals[1], TOLERANCE); // 5.1 - (1 + 2*2) = 0.1
            Assert.AreEqual(-0.1, residuals[2], TOLERANCE); // 6.9 - (1 + 2*3) = -0.1
        }

        [TestMethod]
        public void Residuals_ZeroCoefficients_ReturnsOriginalY()
        {
            // Arrange
            double[][] XA = [
                [1.0, 2.0],
                [1.0, 3.0]
            ];
            double[] y = [5.0, 7.0];
            double[] beta = [0.0, 0.0];

            // Act
            double[] residuals = LinearAlgebra.Residuals(XA, y, beta);

            // Assert: residuals should equal y when all predictions are 0
            Assert.AreEqual(5.0, residuals[0], TOLERANCE);
            Assert.AreEqual(7.0, residuals[1], TOLERANCE);
        }

        [TestMethod]
        public void MAD_OddNumberOfElements_CorrectMedianAbsoluteDeviation()
        {
            // Arrange: residuals = [-2, -1, 0, 1, 2]
            // Absolute values = [0, 1, 1, 2, 2], median = 1
            // MAD = 1.4826 * 1 = 1.4826
            double[] residuals = [-2.0, -1.0, 0.0, 1.0, 2.0];

            // Act
            double mad = LinearAlgebra.MAD(residuals);

            // Assert
            Assert.AreEqual(1.4826 * 1.0, mad, TOLERANCE);
        }

        [TestMethod]
        public void MAD_EvenNumberOfElements_CorrectCalculation()
        {
            // Arrange: residuals = [-1, 0, 1, 2]
            // Absolute values = [0, 1, 1, 2], median of sorted = element at index length/2 = index 2 = 1
            double[] residuals = [-1.0, 0.0, 1.0, 2.0];

            // Act
            double mad = LinearAlgebra.MAD(residuals);

            // Assert
            Assert.AreEqual(1.4826 * 1.0, mad, TOLERANCE);
        }

        [TestMethod]
        public void MAD_AllZeros_ReturnsOne()
        {
            // Arrange - MAD returns 1.0 for all zeros to avoid division by zero in robust regression
            double[] residuals = [0.0, 0.0, 0.0, 0.0];

            // Act
            double mad = LinearAlgebra.MAD(residuals);

            // Assert
            Assert.AreEqual(1.0, mad, TOLERANCE);
        }

        [TestMethod]
        public void OLS_SimpleLinearRegression_CorrectSolution()
        {
            // Arrange: Solve y = XA * beta for y = 1 + 2*x
            double[][] XA = [
                [1.0, 1.0], // [intercept, x] for x=1
                [1.0, 2.0], // [intercept, x] for x=2
                [1.0, 3.0]  // [intercept, x] for x=3
            ];
            double[] y = [3.0, 5.0, 7.0]; // y = 1 + 2*x

            // Act
            double[] beta = LinearAlgebra.OLS(XA, y);

            // Assert
            Assert.AreEqual(2, beta.Length);
            Assert.AreEqual(1.0, beta[0], TOLERANCE); // Intercept
            Assert.AreEqual(2.0, beta[1], TOLERANCE); // Slope
        }

        [TestMethod]
        public void OLS_OverdeterminedSystem_LeastSquaresSolution()
        {
            // Arrange: More equations than unknowns, with some noise
            double[][] XA = [
                [1.0, 1.0],
                [1.0, 2.0],
                [1.0, 3.0],
                [1.0, 4.0]
            ];
            double[] y = [2.9, 5.1, 6.8, 9.2]; // Close to y = 1 + 2*x with noise

            // Act
            double[] beta = LinearAlgebra.OLS(XA, y);

            // Assert: Should be close to [1, 2]
            Assert.AreEqual(2, beta.Length);
            Assert.AreEqual(1.0, beta[0], 0.2); // Intercept approximately 1
            Assert.AreEqual(2.0, beta[1], 0.1); // Slope approximately 2
        }

        [TestMethod]
        public void WLS_UniformWeights_EquivalentToOLS()
        {
            // Arrange
            double[][] XA = [
                [1.0, 1.0],
                [1.0, 2.0],
                [1.0, 3.0]
            ];
            double[] y = [3.0, 5.0, 7.0];
            double[] uniformWeights = [1.0, 1.0, 1.0];

            // Act
            double[] olsBeta = LinearAlgebra.OLS(XA, y);
            double[] wlsBeta = LinearAlgebra.WLS(XA, y, uniformWeights);

            // Assert: WLS with uniform weights should equal OLS
            Assert.AreEqual(olsBeta.Length, wlsBeta.Length);
            for (int i = 0; i < olsBeta.Length; i++)
            {
                Assert.AreEqual(olsBeta[i], wlsBeta[i], TOLERANCE, $"Coefficient {i} should be equal");
            }
        }

        [TestMethod]
        public void WLS_ZeroWeight_IgnoresObservation()
        {
            // Arrange: Give zero weight to outlier
            double[][] XA = [
                [1.0, 1.0],
                [1.0, 2.0],
                [1.0, 3.0]
            ];
            double[] y = [3.0, 5.0, 100.0]; // Last point is outlier
            double[] weights = [1.0, 1.0, 0.0]; // Zero weight for outlier

            // Expected result: fit only first two points: y = 1 + 2*x
            double[][] XA_reduced = [
                [1.0, 1.0],
                [1.0, 2.0]
            ];
            double[] y_reduced = [3.0, 5.0];

            // Act
            double[] wlsBeta = LinearAlgebra.WLS(XA, y, weights);
            double[] expectedBeta = LinearAlgebra.OLS(XA_reduced, y_reduced);

            // Assert
            Assert.AreEqual(expectedBeta.Length, wlsBeta.Length);
            for (int i = 0; i < expectedBeta.Length; i++)
            {
                Assert.AreEqual(expectedBeta[i], wlsBeta[i], TOLERANCE, 
                    $"Coefficient {i}: WLS with zero weight should ignore outlier");
            }
        }

        [TestMethod]
        public void VectorNormDiff_IdenticalVectors_ReturnsZero()
        {
            // Arrange
            double[] a = [1.0, 2.0, 3.0];
            double[] b = [1.0, 2.0, 3.0];

            // Act
            double diff = LinearAlgebra.VectorNormDiff(a, b);

            // Assert
            Assert.AreEqual(0.0, diff, TOLERANCE);
        }

        [TestMethod]
        public void VectorNormDiff_OrthogonalVectors_CorrectEuclideanDistance()
        {
            // Arrange: a = (3, 0), b = (0, 4), distance = 5
            double[] a = [3.0, 0.0];
            double[] b = [0.0, 4.0];

            // Act
            double diff = LinearAlgebra.VectorNormDiff(a, b);

            // Assert
            Assert.AreEqual(5.0, diff, TOLERANCE); // sqrt(3^2 + 4^2) = 5
        }

        [TestMethod]
        public void VectorNormDiff_GeneralCase_CorrectCalculation()
        {
            // Arrange
            double[] a = [1.0, 2.0, 3.0];
            double[] b = [4.0, 6.0, 8.0];
            // Expected: sqrt((1-4)^2 + (2-6)^2 + (3-8)^2) = sqrt(9 + 16 + 25) = sqrt(50)

            // Act
            double diff = LinearAlgebra.VectorNormDiff(a, b);

            // Assert
            Assert.AreEqual(Math.Sqrt(50.0), diff, TOLERANCE);
        }

        [TestMethod]
        public void LinearAlgebra_IntegrationTest_FullWorkflow()
        {
            // Arrange: Test complete workflow similar to what RobustRegressor does
            double[][] X = [
                [1.0],
                [2.0],
                [3.0]
            ];
            double[] y = [2.1, 4.0, 5.9]; // Close to y = 2*x

            // Act: Simulate robust regression workflow
            var XA = LinearAlgebra.AddIntercept(X);
            var initialBeta = LinearAlgebra.OLS(XA, y);
            var residuals = LinearAlgebra.Residuals(XA, y, initialBeta);
            var scale = LinearAlgebra.MAD(residuals);
            
            // Create uniform weights for this test
            double[] weights = [1.0, 1.0, 1.0];
            var finalBeta = LinearAlgebra.WLS(XA, y, weights);
            
            var convergence = LinearAlgebra.VectorNormDiff(initialBeta, finalBeta);

            // Assert: Workflow completes without errors
            Assert.AreEqual(2, initialBeta.Length);
            Assert.AreEqual(3, residuals.Length);
            Assert.IsTrue(scale >= 0.0);
            Assert.AreEqual(2, finalBeta.Length);
            Assert.IsTrue(convergence >= 0.0);
            
            // With uniform weights, should be very close to OLS
            Assert.AreEqual(initialBeta[0], finalBeta[0], 0.01);
            Assert.AreEqual(initialBeta[1], finalBeta[1], 0.01);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))] // Expects ArgumentException for dimension mismatch
        public void LinearAlgebra_MismatchedDimensions_ThrowsException()
        {
            // Arrange: Mismatched dimensions
            double[][] XA = [
                [1.0, 1.0],
                [1.0, 2.0]
            ];
            double[] y = [1.0]; // Wrong size

            // Act: Should throw exception
            LinearAlgebra.OLS(XA, y);
        }
    }
}