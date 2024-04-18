using System.Diagnostics.Contracts;

namespace GINR;

public class Movement
{
    public string Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal ItemCost { get; set; }
    public decimal StockQuantityBefore { get; set; }
    public decimal StockQuantityAfter { get; set; }    
    public decimal AverageCostBefore { get; set; }
    public decimal AverageCostAfter { get; set; }
    public decimal NominalValueBefore { get; set; }
    public decimal NominalValueAfter { get; set; }
    public decimal NominalAdjustment { get; set; }
    public decimal NominalValueAfterAdjustment { get; set; }   
    public decimal ExpectedNominalValue { get; set; }
    public bool IsCorrect => NominalValueAfterAdjustment == ExpectedNominalValue;
    public Movement()
    {

    }

    public Movement(Movement previousMovement, decimal quantity, decimal itemCost)
    {
        Type = quantity > 0 ? "Receipt" : "Issue";
        Quantity = quantity;
        ItemCost = itemCost;
        StockQuantityBefore = previousMovement.StockQuantityAfter;
        StockQuantityAfter = previousMovement.StockQuantityAfter + quantity;
        AverageCostBefore = previousMovement.AverageCostAfter;
        
        CalculateAverageCost();

        NominalValueBefore = previousMovement.NominalValueAfterAdjustment;
        NominalValueAfter = NominalValueBefore + (quantity * itemCost);

        CalculateNominalAdjustment();

        NominalValueAfterAdjustment = NominalValueAfter + NominalAdjustment;

        ExpectedNominalValue = StockQuantityAfter * AverageCostAfter;

    }

    private decimal CalculateAverageCost()
    {
        if(StockQuantityBefore > 0)
        {
            if(StockQuantityAfter != 0)
            {
                AverageCostAfter = ((StockQuantityBefore * AverageCostBefore) + (Quantity * ItemCost)) / StockQuantityAfter;
            }
            else
            {
                AverageCostAfter = ItemCost;
            }

        }
        else
        {
            AverageCostAfter = ItemCost;
        }

        return AverageCostAfter;
    }
    private decimal CalculateNominalAdjustment()
    {
        if(StockQuantityBefore < 0)
        {
            NominalAdjustment = (StockQuantityBefore * AverageCostAfter) - (StockQuantityBefore * AverageCostBefore);
        }
        else
        {
            NominalAdjustment = 0;
        }

        return NominalAdjustment;
    }
}