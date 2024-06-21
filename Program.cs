using GINR;

Console.WriteLine("Hello, World!");

List<Movement> movements = new List<Movement>();
Console.WriteLine("Enter the number of decimal places to work to:");
var decimalPrecision = int.Parse(Console.ReadLine());
Console.WriteLine("Add a movement in the format \"quantity, item cost\" where quantity is a positive number for a receipt and a negative number for an issue.");
Console.WriteLine("Example: 5, 100");
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
    decimal itemCost;
    if(arguments.Length > 1)
    {
        itemCost = decimal.Parse(arguments[1]);
        movements.Add(new Movement(movements.Count > 0 ? movements.Last() : new Movement(), quantity, itemCost, decimalPrecision));
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


    Table table = new Table("Type", "Quantity", "Cost", "StkBefore", "StkAfter", "AveCost Before", "AveCost After", "NomValueBefore", "NomValueAfter", "GINRNomAdjustment", "CurRoundedNomAdjust", "ExpNomValueBeforeCurAdj", "PartValuation", "NomValueAfAdjs", "IsCorrect?");
    
    foreach (var movement in movements)
    {
       table.AddRow(movement.Type, movement.Quantity, movement.ItemCost, movement.StockQuantityBefore, movement.StockQuantityAfter, movement.AverageCostBefore, movement.AverageCostAfter, movement.NominalValueBefore, movement.NominalValueAfter, movement.GINRNominalAdjustment, movement.CurrencyRoundingNominalAdjustment, movement.ExpectedNominalValue, movement.StockQuantityAfter*movement.AverageCostAfter, movement.NominalValueAfterAdjustment, movement.IsCorrect );
    }

    table.Print();
}


