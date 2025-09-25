using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobustFit.Core.LossFunctions;
using System;

namespace RobustFit.UnitTests
{
    [TestClass]
    public class LossFunctionTests
    {
        private const double TOLERANCE = 1e-10;

        #region HuberLoss Tests

        [TestClass]
        public class HuberLossTests
        {
            [TestMethod]
            public void HuberLoss_DefaultConstructor_UsesCorrectTuningConstant()
            {
                // Act
                var huber = new HuberLoss();

                // Assert: Default c should be 1.345
                double smallResidual = 0.5; // |r| < c
                Assert.AreEqual(smallResidual, huber.Psi(smallResidual), TOLERANCE);
                Assert.AreEqual(1.0, huber.Weight(smallResidual), TOLERANCE);
            }

            [TestMethod]
            public void HuberLoss_SmallResiduals_LinearBehavior()
            {
                // Arrange
                var huber = new HuberLoss(c: 2.0);
                double[] smallResiduals = { -1.5, -0.5, 0.0, 0.5, 1.5 };

                foreach (double r in smallResiduals)
                {
                    // Act & Assert: For |r| <= c, Psi(r) = r
                    Assert.AreEqual(r, huber.Psi(r), TOLERANCE, $"Psi({r}) should equal {r}");
                    
                    // For |r| <= c, Weight(r) = 1
                    Assert.AreEqual(1.0, huber.Weight(r), TOLERANCE, $"Weight({r}) should be 1.0");
                    
                    // For |r| <= c, Loss(r) = 0.5 * r^2
                    Assert.AreEqual(0.5 * r * r, huber.Loss(r), TOLERANCE, $"Loss({r}) should be {0.5 * r * r}");
                }
            }

            [TestMethod]
            public void HuberLoss_LargeResiduals_ConstantBehavior()
            {
                // Arrange
                var huber = new HuberLoss(c: 1.0);
                double[] largeResiduals = { -5.0, -2.0, 2.0, 5.0 };

                foreach (double r in largeResiduals)
                {
                    // Act & Assert: For |r| > c, Psi(r) = c * sign(r)
                    double expectedPsi = 1.0 * Math.Sign(r);
                    Assert.AreEqual(expectedPsi, huber.Psi(r), TOLERANCE, $"Psi({r}) should be {expectedPsi}");
                    
                    // For |r| > c, Weight(r) = c / |r|
                    double expectedWeight = 1.0 / Math.Abs(r);
                    Assert.AreEqual(expectedWeight, huber.Weight(r), TOLERANCE, $"Weight({r}) should be {expectedWeight}");
                    
                    // For |r| > c, Loss(r) = c * |r| - 0.5 * c^2
                    double expectedLoss = 1.0 * Math.Abs(r) - 0.5 * 1.0 * 1.0;
                    Assert.AreEqual(expectedLoss, huber.Loss(r), TOLERANCE, $"Loss({r}) should be {expectedLoss}");
                }
            }

            [TestMethod]
            public void HuberLoss_ZeroResidual_CorrectValues()
            {
                // Arrange
                var huber = new HuberLoss(c: 1.345);

                // Act & Assert
                Assert.AreEqual(0.0, huber.Psi(0.0), TOLERANCE);
                Assert.AreEqual(1.0, huber.Weight(0.0), TOLERANCE); // Should handle division by zero
                Assert.AreEqual(0.0, huber.Loss(0.0), TOLERANCE);
            }

            [TestMethod]
            public void HuberLoss_BoundaryValues_CorrectBehavior()
            {
                // Arrange
                double c = 2.0;
                var huber = new HuberLoss(c: c);

                // Act & Assert: At boundary |r| = c
                Assert.AreEqual(c, huber.Psi(c), TOLERANCE);
                Assert.AreEqual(-c, huber.Psi(-c), TOLERANCE);
                Assert.AreEqual(1.0, huber.Weight(c), TOLERANCE);
                Assert.AreEqual(1.0, huber.Weight(-c), TOLERANCE);
            }
        }

        #endregion

        #region TukeyLoss Tests

        [TestClass]
        public class TukeyLossTests
        {
            [TestMethod]
            public void TukeyLoss_DefaultConstructor_UsesCorrectTuningConstant()
            {
                // Act
                var tukey = new TukeyLoss();

                // Assert: Default c should be 4.685
                double smallResidual = 1.0; // |r| < c
                double r2 = smallResidual * smallResidual;
                double c2 = 4.685 * 4.685;
                double t = 1 - r2 / c2;
                
                Assert.AreEqual(smallResidual * t * t, tukey.Psi(smallResidual), TOLERANCE);
                Assert.AreEqual(t * t, tukey.Weight(smallResidual), TOLERANCE);
            }

            [TestMethod]
            public void TukeyLoss_SmallResiduals_QuadraticBehavior()
            {
                // Arrange
                var tukey = new TukeyLoss(c: 4.0);
                double[] smallResiduals = { -2.0, -1.0, 0.0, 1.0, 2.0 };

                foreach (double r in smallResiduals)
                {
                    // Act & Assert: For |r| <= c
                    double r2 = r * r;
                    double c2 = 16.0; // c = 4, so c^2 = 16
                    double t = 1 - r2 / c2;
                    
                    double expectedPsi = r * t * t;
                    double expectedWeight = t * t;
                    double expectedLoss = c2 / 6.0 * (1.0 - t * t * t);
                    
                    Assert.AreEqual(expectedPsi, tukey.Psi(r), TOLERANCE, $"Psi({r}) incorrect");
                    Assert.AreEqual(expectedWeight, tukey.Weight(r), TOLERANCE, $"Weight({r}) incorrect");
                    Assert.AreEqual(expectedLoss, tukey.Loss(r), TOLERANCE, $"Loss({r}) incorrect");
                }
            }

            [TestMethod]
            public void TukeyLoss_LargeResiduals_ZeroBehavior()
            {
                // Arrange
                var tukey = new TukeyLoss(c: 2.0);
                double[] largeResiduals = { -5.0, -3.0, 3.0, 5.0 };

                foreach (double r in largeResiduals)
                {
                    // Act & Assert: For |r| > c, all functions should return 0 or constant
                    Assert.AreEqual(0.0, tukey.Psi(r), TOLERANCE, $"Psi({r}) should be 0");
                    Assert.AreEqual(0.0, tukey.Weight(r), TOLERANCE, $"Weight({r}) should be 0");
                    
                    // Loss should be constant c^2/6 for |r| > c
                    double expectedLoss = 4.0 / 6.0; // c^2/6 = 4/6
                    Assert.AreEqual(expectedLoss, tukey.Loss(r), TOLERANCE, $"Loss({r}) should be constant");
                }
            }

            [TestMethod]
            public void TukeyLoss_ZeroResidual_CorrectValues()
            {
                // Arrange
                var tukey = new TukeyLoss(c: 4.685);

                // Act & Assert
                Assert.AreEqual(0.0, tukey.Psi(0.0), TOLERANCE);
                Assert.AreEqual(1.0, tukey.Weight(0.0), TOLERANCE);
                Assert.AreEqual(0.0, tukey.Loss(0.0), TOLERANCE);
            }

            [TestMethod]
            public void TukeyLoss_BoundaryValues_CorrectBehavior()
            {
                // Arrange
                double c = 3.0;
                var tukey = new TukeyLoss(c: c);

                // Act & Assert: At boundary |r| = c
                Assert.AreEqual(0.0, tukey.Psi(c), TOLERANCE);
                Assert.AreEqual(0.0, tukey.Psi(-c), TOLERANCE);
                Assert.AreEqual(0.0, tukey.Weight(c), TOLERANCE);
                Assert.AreEqual(0.0, tukey.Weight(-c), TOLERANCE);
                
                // Loss should be c^2/6 at boundary
                double expectedLoss = c * c / 6.0;
                Assert.AreEqual(expectedLoss, tukey.Loss(c), TOLERANCE);
                Assert.AreEqual(expectedLoss, tukey.Loss(-c), TOLERANCE);
            }
        }

        #endregion

        #region Comparative Tests

        [TestClass]
        public class LossFunctionComparisonTests
        {
            [TestMethod]
            public void LossFunctions_WeightConsistency_PsiDividedByR()
            {
                // Arrange
                var huber = new HuberLoss(c: 1.5);
                var tukey = new TukeyLoss(c: 4.0);
                double[] residuals = [-2.0, -0.5, 0.1, 0.5, 2.0];

                foreach (double r in residuals)
                {
                    if (Math.Abs(r) > 1e-12) // Avoid division by zero
                    {
                        // Act & Assert: Weight(r) should equal Psi(r) / r
                        double huberExpected = huber.Psi(r) / r;
                        double tukeyExpected = tukey.Psi(r) / r;
                        
                        Assert.AreEqual(huberExpected, huber.Weight(r), TOLERANCE, 
                            $"Huber Weight({r}) != Psi({r})/r");
                        Assert.AreEqual(tukeyExpected, tukey.Weight(r), TOLERANCE, 
                            $"Tukey Weight({r}) != Psi({r})/r");
                    }
                }
            }

            [TestMethod]
            public void LossFunctions_SymmetryProperty_EvenFunctions()
            {
                // Arrange
                var huber = new HuberLoss(c: 1.2);
                var tukey = new TukeyLoss(c: 3.5);
                double[] residuals = [0.5, 1.0, 2.0, 3.0];

                foreach (double r in residuals)
                {
                    // Act & Assert: Loss and Weight should be symmetric (even functions)
                    Assert.AreEqual(huber.Loss(r), huber.Loss(-r), TOLERANCE, 
                        $"Huber Loss not symmetric at r={r}");
                    Assert.AreEqual(huber.Weight(r), huber.Weight(-r), TOLERANCE, 
                        $"Huber Weight not symmetric at r={r}");
                        
                    Assert.AreEqual(tukey.Loss(r), tukey.Loss(-r), TOLERANCE, 
                        $"Tukey Loss not symmetric at r={r}");
                    Assert.AreEqual(tukey.Weight(r), tukey.Weight(-r), TOLERANCE, 
                        $"Tukey Weight not symmetric at r={r}");
                    
                    // Psi should be antisymmetric (odd function)
                    Assert.AreEqual(huber.Psi(r), -huber.Psi(-r), TOLERANCE, 
                        $"Huber Psi not antisymmetric at r={r}");
                    Assert.AreEqual(tukey.Psi(r), -tukey.Psi(-r), TOLERANCE, 
                        $"Tukey Psi not antisymmetric at r={r}");
                }
            }

            [TestMethod]
            public void LossFunctions_MonotonicityProperty_WeightsDecrease()
            {
                // Arrange
                var huber = new HuberLoss(c: 1.0);
                var tukey = new TukeyLoss(c: 2.0);
                double[] increasingResiduals = [0.1, 0.5, 1.0, 1.5, 2.0, 3.0];

                // Act & Assert: Weights should be non-increasing for increasing |r|
                for (int i = 1; i < increasingResiduals.Length; i++)
                {
                    double r1 = increasingResiduals[i - 1];
                    double r2 = increasingResiduals[i];
                    
                    Assert.IsTrue(huber.Weight(r2) <= huber.Weight(r1), 
                        $"Huber weights not decreasing: Weight({r2}) > Weight({r1})");
                    Assert.IsTrue(tukey.Weight(r2) <= tukey.Weight(r1), 
                        $"Tukey weights not decreasing: Weight({r2}) > Weight({r1})");
                }
            }
        }

        #endregion
    }
}