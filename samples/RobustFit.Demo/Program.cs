using RobustFit.Core.Models;
using RobustFit.Core.LossFunctions;
using Spectre.Console;

namespace RobustFit.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a fancy header
            AnsiConsole.Write(
                new FigletText("RobustFit").Centered().Color(Color.Blue)
            );

            AnsiConsole.Write(
                new Rule("[yellow]Original Equation: y = [bold]2.0[/] + [bold]3.0[/] * (Feature 1) + [bold]1.5[/] * (Feature 2) + noise[/]")
                    .RuleStyle("grey")
                    .Centered()
            );

            // Generate sample data with outliers
            var numOutliers = 10;
            var numPoints = 200;
            var (X, y) = GenerateDataWithOutliers(numPoints, numOutliers);
            // Test point for (single) prediction
            double[] testPoint = [2.0, -2.0];

            var dataPanel = new Panel(
                $"[green]Dataset Info[/]\n" +
                $"• Points: [bold]{numPoints}[/] (with [red]{numOutliers} outliers[/])\n" +
                $"• Features: [bold]{X[0].Length}[/]\n" +
                $"• Samples: [bold]{X.Length}[/]"
            ).Header("[blue]Data Summary[/]").BorderColor(Color.Green).NoBorder().Expand();
            AnsiConsole.Write(dataPanel);
            AnsiConsole.WriteLine();

            // Demo OLS Regressor
            AnsiConsole.Write(new Rule("[green]1. Ordinary Least Squares (OLS)[/]").RuleStyle("grey"));

            // Fit OLS model
            var ols = new OLSRegressor();
            ols.Fit(X, y);
            
            var olsTable = new Table()
                .AddColumn("[bold]Coefficient[/]")
                .AddColumn("[bold]Value[/]")
                .BorderColor(Color.Grey100);
            
            for (int i = 0; i < ols.Coefficients.Length; i++)
            {
                string coefName = i == 0 ? "Intercept" : $"Feature {i}";
                olsTable.AddRow(coefName, $"[cyan]{ols.Coefficients[i]:F3}[/]");
            }
            AnsiConsole.Write(olsTable);

            // Make predictions
            
            double olsPred = ols.Predict(testPoint);
            
            AnsiConsole.MarkupLine($"[yellow]Prediction for[/] [[{string.Join(", ", testPoint)}]]: [bold green]{olsPred:F3}[/]");
            AnsiConsole.WriteLine();

            // Demo Robust Regressor with Huber Loss
            AnsiConsole.Write(new Rule("[orange1]2. Robust Regression (Huber Loss)[/]\n").RuleStyle("grey"));
            
            var robust1 = new RobustRegressor();
            var robust2 = new RobustRegressor();
            var huberLoss = new HuberLoss(1.35);
            var tukeyLoss = new TukeyLoss(4.685);
            robust1.Fit(X, y, huberLoss, maxIter: 100, tol: 1e-6);

            var robustTable = new Table()
                .AddColumn("[bold]Coefficient[/]")
                .AddColumn("[bold]Value[/]")
                .BorderColor(Color.Grey100);
            
            for (int i = 0; i < robust1.Coefficients.Length; i++)
            {
                string coefName = i == 0 ? "Intercept" : $"Feature {i}";
                robustTable.AddRow(coefName, $"[orange1]{robust1.Coefficients[i]:F3}[/]");
            }
            AnsiConsole.Write(robustTable);

            double robustPred = robust1.Predict(testPoint);
            AnsiConsole.MarkupLine($"[yellow]Prediction for[/] [[{string.Join(", ", testPoint)}]]: [bold orange1]{robustPred:F3}[/]");
            AnsiConsole.WriteLine();


            // Demo Robust Regressor with Tukey Loss
            AnsiConsole.Write(new Rule("[red]3. Robust Regression (Tukey Loss)[/]\n").RuleStyle("grey"));
            robust2.Fit(X, y, tukeyLoss, maxIter: 100, tol: 1e-6);

            var robustTable2 = new Table()
                .AddColumn("[bold]Coefficient[/]")
                .AddColumn("[bold]Value[/]")
                .BorderColor(Color.Grey100);
            
            for (int i = 0; i < robust2.Coefficients.Length; i++)
            {
                string coefName = i == 0 ? "Intercept" : $"Feature {i}";
                robustTable2.AddRow(coefName, $"[orange1]{robust2.Coefficients[i]:F3}[/]");
            }
            AnsiConsole.Write(robustTable2);

            double robustPred2 = robust2.Predict(testPoint);
            AnsiConsole.MarkupLine($"[yellow]Prediction for[/] [[{string.Join(", ", testPoint)}]]: [bold orange1]{robustPred2:F3}[/]");
            AnsiConsole.WriteLine();

            // Compare results
            AnsiConsole.Write(new Rule($"[blue]4. Comparison of predictions for[/] [[{string.Join(", ", testPoint)}]]").NoBorder());

            var comparisonPanel = new Panel(
                $"[green]OLS Prediction:[/] [bold]{olsPred:F3}[/]\n" +
                $"[orange1]Robust Prediction (Huber Loss):[/] [bold]{robustPred:F3}[/]\n" +
                $"[red]Robust Prediction (Tukey Loss):[/] [bold]{robustPred2:F3}[/]")
                .Header("[blue]Model Comparison[/]")
                .BorderColor(Color.Blue)
                .Expand();
            AnsiConsole.Write(comparisonPanel);
            AnsiConsole.WriteLine();

            // Demo multiple predictions
            AnsiConsole.Write(new Rule("[grey]5. Batch Predictions[/]").NoBorder());
            
            double[][] testData = [
                [1.0, 0.5],
                [2.0, 1.0],
                [3.0, 1.5],
                [5.0, 1.0],
                [-1.0, -1.5],
                [-2.0, -1.0],
                [-3.0, -0.5]
            ];

            var olsPredictions = ols.Predict(testData);

            var predTable = new Table()
                .AddColumn("[bold]Input[/]")
                .AddColumn("[bold green]OLS[/]")
                .AddColumn("[bold orange1]Robust (Huber Loss)[/]")
                .AddColumn("[bold red]Robust (Tukey Loss)[/]");

            for (int i = 0; i < testData.Length; i++)
            {
                double pred_huber = robust1.Predict(testData[i]);
                double pred_tukey = robust2.Predict(testData[i]);

                predTable.AddRow(
                    $"[[{string.Join(", ", testData[i])}]]",
                    $"[green]{olsPredictions[i]:F3}[/]",
                    $"[orange1]{pred_huber:F3}[/]",
                    $"[red]{pred_tukey:F3}[/]");
            }
            AnsiConsole.Write(predTable.Expand());
        }

        static (double[][], double[]) GenerateDataWithOutliers(
            int nPoints = 100,
            int nOutliers = 10,
            int seed = 42)
        {
            var random = new Random(seed);

            double[][] X = new double[nPoints][];
            double[] y = new double[nPoints];

            for (int i = 0; i < nPoints; i++)
            {
                double x1 = random.NextDouble() * 4 - 2;
                double x2 = random.NextDouble() * 4 - 2;
                X[i] = [x1, x2];
                
                double noise = (random.NextDouble() - 0.5) * 0.5;
                y[i] = 2.0 + 3.0 * x1 + 1.5 * x2 + noise;
            }

            for (int i = nPoints - nOutliers; i < nPoints; i++)
            {
                y[i] += (random.NextDouble() - 0.5) * 80;
            }

            return (X, y);
        }
    }
}
