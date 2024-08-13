﻿using GINR;

Console.WriteLine("Hello, World!");

List<Movement> movements = new List<Movement>();
Console.WriteLine("Enter the number of decimal places to work to:");
var decimalPrecision = int.Parse(Console.ReadLine());
Console.WriteLine("Add a movement in the format \"quantity, item cost\" where quantity is a positive number for a receipt and a negative number for an issue.");
Console.WriteLine("Example: 5, 100, y");
while(true)
{
    
    Console.WriteLine("Add a movement:");
    var line = Console.ReadLine();
    if(line == "exit")
    {
        break;
    }
    var arguments = line.Split(',');
    var quantity = decimal.Parse(arguments[0]);
    decimal itemCost = 0;
    bool recalculateAverageCostForIssue = false;
    if (arguments.Length > 1)
    {
        if (arguments[1] != null)
        {
            itemCost = decimal.Parse(arguments[1]);

        }

        if (arguments.Length == 3 && arguments[2].Trim().ToLower() == "y")
        {
            recalculateAverageCostForIssue = true;
        }

        movements.Add(new Movement(movements.Count > 0 ? movements.Last() : new Movement(), quantity, itemCost, decimalPrecision, recalculateAverageCostForIssue));

    }
    else
    {
        if(movements.Count == 0)
        {
            Console.WriteLine("You must provide an item cost for the first movement.");
            continue;
        }
        if(quantity > 0)
        {
            Console.WriteLine("You must provide an item cost for a receipt.");
            continue;
        }
        else
        {
            movements.Add(new Movement(movements.Count > 0 ? movements.Last() : new Movement(), quantity, movements.Last().AverageCostAfter, decimalPrecision));
        }

    }

    

    Table table = new Table("Type", "Quantity", "Cost", "StkBefore", "StkAfter", "AveCost Before", "AveCost After", "NomValueBefore", "NomValueAfter", "GINRNomAdjustment", "ExpNomValueBeforeCurAdj", "CurRoundedNomAdjust", "NomValueAfAdjs", "PartValuation", "IsCorrect?");
    
    foreach (var movement in movements)
    {
       table.AddRow(movement.Type, movement.Quantity, movement.ItemCost, movement.StockQuantityBefore, movement.StockQuantityAfter, movement.AverageCostBefore, movement.AverageCostAfter, movement.NominalValueBefore, movement.NominalValueAfter, movement.GINRNominalAdjustment, movement.ExpectedNominalValue, movement.CurrencyRoundingNominalAdjustment, movement.NominalValueAfterAdjustment, Math.Round(movement.StockQuantityAfter * movement.AverageCostAfter, decimalPrecision,MidpointRounding.AwayFromZero), movement.IsCorrect );
    }

    table.Print();
}


