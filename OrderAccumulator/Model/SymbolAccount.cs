using QuickFix.Fields;

namespace OrderAccumulator.Model
{
    public class SymbolAccount
    {
        public const int LIMIT = 1000000;

        public SymbolAccount(Symbol symbol)
        {
            Symbol = symbol;
        }

        public Symbol Symbol { get; private set; }
        public decimal Balance { get; private set; }

        public bool AddToBalance(decimal amount)
        {
            if (Balance + amount > LIMIT 
            || Balance + amount < (LIMIT * -1))
             return false;
            else
                Balance += amount;            
            return true;
        }
    }
}