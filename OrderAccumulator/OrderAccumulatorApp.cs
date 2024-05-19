using QuickFix;
using QuickFix.Fields;

namespace OrderAccumulator
{
    public class OrderAccumulatorApp : MessageCracker, IApplication
    {
        private readonly List<QuickFix.FIX44.NewOrderSingle> _orders = new();
        private readonly Model.Account _account = new();

        public void EvaluateOrder(QuickFix.FIX44.NewOrderSingle order, SessionID s)
        {
            var res = _account.AddToBalance(order.Symbol, order.Price.Obj * order.OrderQty.Obj);
            if (res)
                SendOrderAccept(order, s);
            else
                SendOrderReject(order, s);
        }

        public void SendOrderAccept(QuickFix.FIX44.NewOrderSingle msg, SessionID s)
        {
            QuickFix.FIX44.ExecutionReport orderAccept =
                new(
                    new OrderID(),
                    new ExecID("0"),
                    new ExecType(ExecType.FILL),
                    new OrdStatus(OrdStatus.FILLED),
                    msg.Symbol,
                    msg.Side,
                    new LeavesQty(0),
                    new CumQty(msg.OrderQty.Obj),
                    new AvgPx(0)
                );

            try
            {
                Session.SendToTarget(orderAccept, s);
                _orders.Add(msg);
            }
            catch (SessionNotFound ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public void SendOrderReject(QuickFix.FIX44.NewOrderSingle msg, SessionID s)
        {
            var lastOrder = _orders.LastOrDefault();
            QuickFix.FIX44.OrderCancelReject ocj =
                new(
                    new OrderID(msg.ClOrdID.Obj),
                    msg.ClOrdID,
                    new OrigClOrdID(
                        lastOrder != null
                            ? lastOrder.ClOrdID.toStringField()
                            : new ClOrdID().toStringField()
                    ),
                    new OrdStatus(OrdStatus.REJECTED),
                    new CxlRejResponseTo(CxlRejResponseTo.ORDER_CANCEL_REQUEST)
                )
                {
                    CxlRejReason = new CxlRejReason(CxlRejReason.OTHER),
                    Text = new Text("Order rejected: User exceeded permitted exposure limit.")
                };

            try
            {
                Session.SendToTarget(ocj, s);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        #region eventos

        public void OnMessage(QuickFix.FIX44.NewOrderSingle message, SessionID sessionID)
        {
            EvaluateOrder(message, sessionID);
        }

        public void FromAdmin(Message message, SessionID sessionID)
        {
            Console.WriteLine("IN:  " + message);
        }

        public void FromApp(Message message, SessionID sessionID)
        {
            Console.WriteLine("IN:  " + message);
            Crack(message, sessionID);
        }

        public void OnCreate(SessionID sessionID) { }

        public void OnLogon(SessionID sessionID) { }

        public void OnLogout(SessionID sessionID) { }

        public void ToAdmin(Message message, SessionID sessionID)
        {
            Console.WriteLine("OUT:  " + message);
        }

        public void ToApp(Message message, SessionID sessionID)
        {
            Console.WriteLine("OUT: " + message);
        }

        #endregion
    }
}
