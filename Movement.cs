using System.Diagnostics.Contracts;

namespace GINR;

public class Movement
{
    public string Type { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal ItemCost { get; private set; }
    public decimal StockQuantityBefore { get; private set; }
    public decimal StockQuantityAfter { get; private set; }
    public decimal AverageCostBefore { get; private set; }
    public decimal AverageCostAfter { get; private set; }
    public decimal UnroundedAverageCostAfter { get; private set; }
    public decimal NominalValueBefore { get; private set; }
    public decimal NominalValueAfter { get; private set; }
    public decimal GINRNominalAdjustment { get; private set; }
    public decimal CurrencyRoundingNominalAdjustment { get; private set; }
    public decimal NominalValueAfterAdjustment { get; private set; }
    public decimal ExpectedNominalValue { get; private set; }
    public bool IsCorrect { get; private set; }
    public int DecimalPrecision { get; private set; } = 2;
    public bool RecalculateAverageCostForIssue { get; private set; }
    public Movement()
    {

    }
    public Movement(Movement previousMovement, decimal quantity, decimal itemCost, int decimalPrecision, bool recalculateAverageCostForIssue = false)
    {
        DecimalPrecision = decimalPrecision;
        Type = quantity > 0 ? "Receipt" : "Issue";
        Quantity = quantity;
        StockQuantityBefore = previousMovement.StockQuantityAfter;
        StockQuantityAfter = previousMovement.StockQuantityAfter + quantity;
        AverageCostBefore = previousMovement.AverageCostAfter;
        SetItemCost(itemCost, recalculateAverageCostForIssue);
        SetRecalculateAverageCostForIssue(quantity, recalculateAverageCostForIssue);
        CalculateNewAverageCosts();
        NominalValueBefore = previousMovement.NominalValueAfterAdjustment;
        NominalValueAfter = Math.Round((NominalValueBefore + Math.Round((Quantity * ItemCost), DecimalPrecision, MidpointRounding.AwayFromZero)), DecimalPrecision, MidpointRounding.AwayFromZero);
        CalculateGINRNominalAdjustment();
        CalculateCurrencyRoundingNominalAdjustment();
        NominalValueAfterAdjustment = NominalValueAfter + GINRNominalAdjustment + CurrencyRoundingNominalAdjustment;
        ExpectedNominalValue = Math.Round((StockQuantityAfter * UnroundedAverageCostAfter), DecimalPrecision, MidpointRounding.AwayFromZero);
        IsCorrect = NominalValueAfterAdjustment == Math.Round((StockQuantityAfter * AverageCostAfter), DecimalPrecision, MidpointRounding.AwayFromZero);
    }
    private decimal SetItemCost(decimal itemCost, bool recalculateAverageForIssue)
    {
        if (Quantity > 0) return ItemCost = itemCost;

        if (Quantity < 0 && recalculateAverageForIssue == true)
        {
            return ItemCost = itemCost;
        }
        if (Quantity < 0 && recalculateAverageForIssue == false)
        {
            return ItemCost = AverageCostBefore;
        }

        return ItemCost = itemCost;
    }
    private bool SetRecalculateAverageCostForIssue(decimal quantity, bool recalculateAverageCostForIssue)
    {
        if (quantity > 0) return RecalculateAverageCostForIssue = false;
        if (quantity < 0 && recalculateAverageCostForIssue == true) return RecalculateAverageCostForIssue = true;
        if (quantity < 0 && recalculateAverageCostForIssue == false) return RecalculateAverageCostForIssue = false;

        return RecalculateAverageCostForIssue = false;
    }
    private decimal CalculateNewAverageCosts()
    {
        if (StockQuantityBefore > 0)
        {
            if (StockQuantityAfter != 0)
            {
                UnroundedAverageCostAfter = ((StockQuantityBefore * AverageCostBefore) + Math.Round((Quantity * ItemCost), DecimalPrecision, MidpointRounding.AwayFromZero)) / StockQuantityAfter;
                AverageCostAfter = Math.Round(UnroundedAverageCostAfter, DecimalPrecision, MidpointRounding.AwayFromZero);
                return AverageCostAfter;
            }

            return AverageCostAfter = Math.Round(ItemCost, DecimalPrecision, MidpointRounding.AwayFromZero);

        }
        UnroundedAverageCostAfter = ItemCost;
        return AverageCostAfter = Math.Round(ItemCost, DecimalPrecision, MidpointRounding.AwayFromZero);
    }
    private decimal CalculateGINRNominalAdjustment()
    {
        if (StockQuantityBefore < 0)
        {
            return GINRNominalAdjustment = Math.Round((StockQuantityBefore * AverageCostAfter) - (StockQuantityBefore * AverageCostBefore), DecimalPrecision, MidpointRounding.AwayFromZero);
        }
        else
        {
            return GINRNominalAdjustment = 0;
        }

    }
    private decimal CalculateCurrencyRoundingNominalAdjustment()
    {
        decimal previousStockValue = Math.Round(StockQuantityBefore * AverageCostBefore, DecimalPrecision, MidpointRounding.AwayFromZero);
        decimal receiptValue = Math.Round(Quantity * ItemCost, DecimalPrecision, MidpointRounding.AwayFromZero);
        decimal stockValueByAddition = Math.Round(previousStockValue + receiptValue, DecimalPrecision, MidpointRounding.AwayFromZero);
        decimal stockValueAtRoundedAverage = Math.Round(StockQuantityAfter * AverageCostAfter, DecimalPrecision, MidpointRounding.AwayFromZero);

        return CurrencyRoundingNominalAdjustment = Math.Round((stockValueAtRoundedAverage - stockValueByAddition), DecimalPrecision, MidpointRounding.AwayFromZero) - GINRNominalAdjustment;

    }
}