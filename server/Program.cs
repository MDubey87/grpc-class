using Calculator;
using Greet;
using Grpc.Core;
using server;

int Port = 50051;
Server? server = null;
try
{
    server = new Server
    {
        Services = { GreetService.BindService(new GreetingServiceImpl()), CalculatorService.BindService(new CalculatorServiceImpl()) },
        Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
    };
    server.Start();
    Console.WriteLine("Server is listening on port " + Port);
    Console.ReadKey();
}
catch (IOException ex)
{
    Console.WriteLine("Error starting server: " + ex.Message);
    throw;
}
finally
{
    if (server != null)
    {
        server.ShutdownAsync().Wait();
        Console.WriteLine("Server has been shut down.");
    }
}