using QuickFix.Fields;

namespace OrderAccumulator.Model
{
    public class Account
    {
        public Account()
        {
            AccountList = new List<SymbolAccount>();
        }

        public List<SymbolAccount> AccountList { get; set; }

        public bool AddToBalance(Symbol symbol, decimal amount)
        {
            var existingAccount = AccountList.FirstOrDefault(a =>
                a.Symbol.getValue() == symbol.getValue()
            );

            if (existingAccount != null)
            {
                Console.WriteLine(">>>" + existingAccount.Symbol + " R$" + existingAccount.Balance);
                return existingAccount.AddToBalance(amount);
            }

            var newAccount = new SymbolAccount(symbol);
            if (newAccount.AddToBalance(amount))
            {
                Console.WriteLine(">>>" + newAccount.Symbol + " R$" + newAccount.Balance);
                AccountList.Add(newAccount);
                return true;
            }

            return false;
        }
    }
}
