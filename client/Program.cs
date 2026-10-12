using Calculator;
using Greet;
using Grpc.Core;

const string target = "127.0.0.1:50051";
Channel channel = new Channel(target, ChannelCredentials.Insecure);
channel.ConnectAsync().ContinueWith((task) =>
{
    if (task.Status == TaskStatus.RanToCompletion)
    {
        Console.WriteLine("Client connected to server at " + target);
    }
}).Wait();
var client = new GreetService.GreetServiceClient(channel);
var greeting = new Greeting
{
    FirstName = "John",
    LastName = "Doe"
};

var request = new LongGreetRequest
{
    Greeting = greeting
};
var stream = client.LongGreet();

foreach(var i in Enumerable.Range(0, 5))
{
    await stream.RequestStream.WriteAsync(request);
    await Task.Delay(1000); // Simulate some delay
}

await stream.RequestStream.CompleteAsync();
var response = await stream.ResponseAsync;
Console.WriteLine(response.Result);

//var request = new GreetingManyTimesRequest
//{
//    Greeting = greeting
//};
//using (var call = client.GreetManyTimes(request))
//{
//    while (await call.ResponseStream.MoveNext())
//    {
//        var response = call.ResponseStream.Current;
//        Console.WriteLine(response.Result);
//    }
//}

//Console.WriteLine("====== Calling Calulator Service ======");
//var client = new CalculatorService.CalculatorServiceClient(channel);
//var calulatorRequest = new SumRequest
//{
//    Number1 = 10,
//    Number2 = 20
//};
//var response = client.Sum(calulatorRequest);
//Console.WriteLine(response.Result);

channel.ShutdownAsync().Wait();
Console.ReadKey();