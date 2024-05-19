using OrderAccumulator;
using QuickFix;

try
{
    var path = Directory.GetCurrentDirectory() + "\\simpleacc.cfg";
    SessionSettings settings = new(path);
    IApplication app = new OrderAccumulatorApp();
    IMessageStoreFactory storeFactory = new FileStoreFactory(settings);
    ILogFactory logFactory = new FileLogFactory(settings);
    IAcceptor acceptor = new ThreadedSocketAcceptor(app, storeFactory, settings, logFactory);

    acceptor.Start();
    Console.WriteLine("press <enter> to quit");
    Console.Read();
    acceptor.Stop();
}
catch (Exception e)
{
    Console.WriteLine("==FATAL ERROR==");
    Console.WriteLine(e.ToString());
}
