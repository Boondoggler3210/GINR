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
    public decimal UnroundedAverageCostAfter { get; set; }
    public decimal NominalValueBefore { get; set; }
    public decimal NominalValueAfter { get; set; }
    public decimal GINRNominalAdjustment { get; set; }
    public decimal CurrencyRoundingNominalAdjustment { get; set; }
    public decimal NominalValueAfterAdjustment { get; set; }   
    public decimal ExpectedNominalValue { get; set; }
    public bool IsCorrect => NominalValueAfterAdjustment == (StockQuantityAfter * AverageCostAfter);
    public int DecimalPrecision { get; set; } = 2;
    public Movement()
    {

    }

    public Movement(Movement previousMovement, decimal quantity, decimal itemCost, int decimalPrecision)
    {
        DecimalPrecision = decimalPrecision;
        Type = quantity > 0 ? "Receipt" : "Issue";
        Quantity = quantity;
        ItemCost = itemCost;
        StockQuantityBefore = previousMovement.StockQuantityAfter;
        StockQuantityAfter = previousMovement.StockQuantityAfter + quantity;
        AverageCostBefore = previousMovement.AverageCostAfter;
        
        CalculateAverageCost();

        NominalValueBefore = previousMovement.NominalValueAfterAdjustment;
        NominalValueAfter = Math.Round((NominalValueBefore + (quantity * itemCost)), DecimalPrecision, MidpointRounding.AwayFromZero);

        CalculateGINRNominalAdjustment();
        CalculateCurrencyRoundingNominalAdjustment();

        NominalValueAfterAdjustment = NominalValueAfter + GINRNominalAdjustment + CurrencyRoundingNominalAdjustment;

        ExpectedNominalValue = Math.Round((StockQuantityAfter * UnroundedAverageCostAfter), DecimalPrecision, MidpointRounding.AwayFromZero);

    }

    private decimal CalculateAverageCost()
    {
        if(StockQuantityBefore > 0)
        {
            if(StockQuantityAfter != 0)
            {
                UnroundedAverageCostAfter = ((StockQuantityBefore * AverageCostBefore) + (Quantity * ItemCost)) / StockQuantityAfter;
                AverageCostAfter = Math.Round(UnroundedAverageCostAfter, DecimalPrecision, MidpointRounding.AwayFromZero);
                return AverageCostAfter;
            }

            return AverageCostAfter = Math.Round(ItemCost, DecimalPrecision, MidpointRounding.AwayFromZero);

        }
        UnroundedAverageCostAfter = ItemCost;
        AverageCostAfter = Math.Round(ItemCost,DecimalPrecision, MidpointRounding.AwayFromZero);
        
        return AverageCostAfter;
    }
    private decimal CalculateGINRNominalAdjustment()
    {
        if(StockQuantityBefore < 0)
        {
            GINRNominalAdjustment = (StockQuantityBefore * AverageCostAfter) - (StockQuantityBefore * AverageCostBefore);
        }
        else
        {
            GINRNominalAdjustment = 0;
        }

        return Math.Round(GINRNominalAdjustment, DecimalPrecision, MidpointRounding.AwayFromZero);
    }

    private decimal CalculateCurrencyRoundingNominalAdjustment()
    {
  
        CurrencyRoundingNominalAdjustment = Math.Round(((StockQuantityAfter * AverageCostAfter) - (StockQuantityAfter * UnroundedAverageCostAfter)), DecimalPrecision, MidpointRounding.AwayFromZero);

        return CurrencyRoundingNominalAdjustment;
    }
}