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
    var itemCost = decimal.Parse(arguments[1]);

    movements.Add(new Movement(movements.Count > 0 ? movements.Last() :new Movement(), quantity, itemCost));
    
    Table table = new Table("Type", "Quantity", "Cost", "StkBefore", "StockAfter", "AveCost Before", "AveCost After", "NomValue Before", "NomValue After", "NomAdjustment", "NomValueAfAdj", "Expected NomValue", "IsCorrect?");
    
    foreach (var movement in movements)
    {
       table.AddRow(movement.Type, movement.Quantity, movement.ItemCost, movement.StockQuantityBefore, movement.StockQuantityAfter, movement.AverageCostBefore, movement.AverageCostAfter, movement.NominalValueBefore, movement.NominalValueAfter, movement.NominalAdjustment, movement.NominalValueAfterAdjustment, movement.ExpectedNominalValue, movement.IsCorrect);
    }

    table.Print();
}


