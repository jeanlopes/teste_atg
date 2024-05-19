using QuickFix;
using QuickFix.Fields;

namespace OrderGenerator
{
    public class OrderGeneratorApp : MessageCracker, IApplication
    {
        Session? _session = null;

        #region events

        public void FromAdmin(Message message, SessionID sessionID) { }

        public void ToAdmin(Message message, SessionID sessionID) { }

        public void FromApp(Message message, SessionID sessionID)
        {
            Console.WriteLine("IN:  " + message.ToString());
        }

        public void OnCreate(SessionID sessionID)
        {
            _session = Session.LookupSession(sessionID);
        }

        public void OnLogon(SessionID sessionID)
        {
            Console.WriteLine("Logon - " + sessionID.ToString());
        }

        public void OnLogout(SessionID sessionID)
        {
            Console.WriteLine("Logout - " + sessionID.ToString());
        }

        public void ToApp(Message message, SessionID sessionID)
        {
            Console.WriteLine();
            Console.WriteLine("OUT: " + message.ToString());
        }

        #endregion

        public void Run()
        {
            var count = 1;
            while (true)
            {
                SendRandomOrder(count);
                count++;
                Thread.Sleep(1000);
            }
        }

        private void SendRandomOrder(int count)
        {
            var m = QueryNewOrderSingle50(count);
            SendMessage(m);
        }

        private static QuickFix.FIX44.NewOrderSingle QueryNewOrderSingle50(int count)
        {
            QuickFix.FIX44.NewOrderSingle newOrderSingle =
                new(
                    GetClOrdID(count),
                    GetRamdomSymbol(),
                    GetRamdomSide(),
                    new TransactTime(DateTime.Now),
                    GetOrdType()
                );

            newOrderSingle.Set(GetRandomOrderQty());
            newOrderSingle.Set(GetRandomPrice());

            newOrderSingle.SetField(new HandlInst('1'));
            newOrderSingle.SetField(new TimeInForce('0'));

            return newOrderSingle;
        }

        private static int CalculateChecksum(string messageBody)
        {
            int checksum = 0;
            foreach (char c in messageBody)
            {
                checksum += (int)c;
            }

            checksum %= 256;
            return checksum;
        }

        private void SendMessage(Message m)
        {
            if (_session != null)
                _session.Send(m);
            else
            {
                Console.WriteLine("Can't send message: session not created.");
            }
        }

        private static OrdType GetOrdType()
        {
            return new OrdType(OrdType.MARKET);
        }

        private static ClOrdID GetClOrdID(int count)
        {
            return new ClOrdID(count.ToString());
        }

        private static Symbol GetRamdomSymbol()
        {
            string[] options = { "PETR4", "VALE3", "VIIA4" };
            var i = new Random().Next(0, 3);
            var choice = options[i];
            return new Symbol(choice);
        }

        private static Side GetRamdomSide()
        {
            char[] options = { Side.BUY, Side.SELL };
            var i = new Random().Next(0, 2);
            var choice = options[i];
            return new Side(choice);
        }

        private static OrderQty GetRandomOrderQty()
        {
            var num = new Random().Next(1, 100000);
            return new OrderQty(Convert.ToDecimal(num));
        }

        private static Price GetRandomPrice()
        {
            int cents = new Random().Next(1, 100000);
            double value = cents / 100.0;

            return new Price(Convert.ToDecimal(value));
        }
    }
}
