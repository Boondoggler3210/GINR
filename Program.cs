using GINR;

Console.WriteLine("Hello, World!");

List<Movement> movements = new List<Movement>();
Console.WriteLine("Enter the number of decimal places to work to:");
var decimalPrecision = int.Parse(Console.ReadLine());
Console.WriteLine("Add a movement in the format \"quantity, item cost, recalculate Average cost for this issue\" where quantity is a positive number for a receipt and a negative number for an issue.");
Console.WriteLine("Example: 5, 100, y");
while(true)
{
    
    Console.WriteLine("Add a movement:");
    var line = Console.ReadLine();
    if(String.IsNullOrWhiteSpace(line))
    {
        Console.WriteLine("Enter values in the form \"quantity, item cost, recalculate aveCost for this issue\"");
        continue;
    }
    if(line == "exit")
    {
        break;
    }
    var arguments = line.Split(',');

    decimal quantity; 
    if (decimal.TryParse(arguments[0], out quantity) == false)
    {
        Console.WriteLine("The quantity must be a number.");
        continue;
    }
    
    
    decimal itemCost;
    bool recalculateAverageCostForIssue = false;
    if (arguments.Length > 1)
    {
        if (decimal.TryParse(arguments[1], out itemCost) == false && quantity > 0)
        {
            Console.WriteLine("The item cost must be a number.");
            continue;
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


