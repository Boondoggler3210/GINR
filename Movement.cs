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
    public bool IsCorrect { get; set; }
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
        NominalValueAfter = Math.Round((NominalValueBefore + Math.Round((Quantity * ItemCost), DecimalPrecision, MidpointRounding.AwayFromZero)), DecimalPrecision, MidpointRounding.AwayFromZero);

        CalculateGINRNominalAdjustment();
        CalculateCurrencyRoundingNominalAdjustment();

        NominalValueAfterAdjustment = NominalValueAfter + GINRNominalAdjustment + CurrencyRoundingNominalAdjustment;

        ExpectedNominalValue = Math.Round((StockQuantityAfter * UnroundedAverageCostAfter), DecimalPrecision, MidpointRounding.AwayFromZero);
        IsCorrect =  NominalValueAfterAdjustment == Math.Round((StockQuantityAfter * AverageCostAfter), DecimalPrecision, MidpointRounding.AwayFromZero);

    }

    private decimal CalculateAverageCost()
    {
        if(StockQuantityBefore > 0)
        {
            if(StockQuantityAfter != 0)
            {
                UnroundedAverageCostAfter = ((StockQuantityBefore * AverageCostBefore) + Math.Round((Quantity * ItemCost),DecimalPrecision,MidpointRounding.AwayFromZero)) / StockQuantityAfter;
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
            GINRNominalAdjustment = Math.Round((StockQuantityBefore * AverageCostAfter) - (StockQuantityBefore * AverageCostBefore), DecimalPrecision, MidpointRounding.AwayFromZero);

            //GINRNominalAdjustment = (Math.Round(StockQuantityBefore * AverageCostAfter, DecimalPrecision, MidpointRounding.AwayFromZero)) - (Math.Round(StockQuantityBefore * AverageCostBefore, DecimalPrecision,MidpointRounding.AwayFromZero));
        }
        else
        {
            GINRNominalAdjustment = 0;
        }

        return Math.Round(GINRNominalAdjustment, DecimalPrecision, MidpointRounding.AwayFromZero);
    }

    private decimal CalculateCurrencyRoundingNominalAdjustment()
    {
        decimal previousStockValue = Math.Round(StockQuantityBefore * AverageCostBefore, DecimalPrecision, MidpointRounding.AwayFromZero);
        decimal receiptValue = Math.Round(Quantity * ItemCost, DecimalPrecision, MidpointRounding.AwayFromZero);
        decimal stockValueByAddition = Math.Round(previousStockValue + receiptValue, DecimalPrecision, MidpointRounding.AwayFromZero);

        decimal stockValueAtRoundedAverage = Math.Round(StockQuantityAfter * AverageCostAfter, DecimalPrecision, MidpointRounding.AwayFromZero);
        
        CurrencyRoundingNominalAdjustment = Math.Round((stockValueAtRoundedAverage - stockValueByAddition), DecimalPrecision, MidpointRounding.AwayFromZero) - GINRNominalAdjustment;
        
        return CurrencyRoundingNominalAdjustment;
        
    }

}