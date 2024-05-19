using OrderGenerator;
using QuickFix;

try
{
    var path = Directory.GetCurrentDirectory() + "\\tradeclient.cfg";
    SessionSettings settings = new(path);
    OrderGeneratorApp application = new();
    IMessageStoreFactory storeFactory = new FileStoreFactory(settings);
    ILogFactory logFactory = new ScreenLogFactory(settings);

    QuickFix.Transport.SocketInitiator initiator = new QuickFix.Transport.SocketInitiator(
        application,
        storeFactory,
        settings,
        logFactory
    );

    initiator.Start();
    application.Run();
    initiator.Stop();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    Console.WriteLine(e.StackTrace);
}
Environment.Exit(1);
