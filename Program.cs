// See https://aka.ms/new-console-template for more information
using System.ComponentModel;
using GINR;

Console.WriteLine("Hello, World!");

List<Movement> movements = new List<Movement>();

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
        movements.Add(new Movement(movements.Count > 0 ? movements.Last() : new Movement(), quantity, itemCost));
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
            movements.Add(new Movement(movements.Count > 0 ? movements.Last() : new Movement(), quantity, movements.Last().AverageCostAfter));
        }

    }


    Table table = new Table("Type", "Quantity", "Cost", "StkBefore", "StkAfter", "AveCost Before", "AveCost After", "NomValueBefore", "NomValueAfter", "NomAdjustment", "NomValueAfAdj", "ExpNomValue", "IsCorrect?");
    
    foreach (var movement in movements)
    {
       table.AddRow(movement.Type, movement.Quantity, movement.ItemCost.ToString("#.00"), movement.StockQuantityBefore, movement.StockQuantityAfter, movement.AverageCostBefore.ToString("#.00"), movement.AverageCostAfter.ToString("#.00"), movement.NominalValueBefore.ToString("#.00"), movement.NominalValueAfter, movement.NominalAdjustment.ToString("#.00"), movement.NominalValueAfterAdjustment.ToString("#.00"), movement.ExpectedNominalValue.ToString("#.00"), movement.IsCorrect);
    }

    table.Print();
}


