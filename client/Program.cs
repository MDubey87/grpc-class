using Calculator;
using Greet;
using Grpc.Core;
using Sqrt;

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

//DoSimpleGreet(client);
//await DoManyGreetings(client);
//await DoLongGreet(client);
//await DoGreetEveryone(client);
//await SqrtServcie(channel);
await GeetingWithDeadline(client);
channel.ShutdownAsync().Wait();
Console.ReadKey();

static void DoSimpleGreet(GreetService.GreetServiceClient client)
{
    var greeting = new Greeting()
    {
        FirstName = "Clement",
        LastName = "Jean"
    };

    var request = new GreetingRequest() { Greeting = greeting };
    var response = client.Greet(request);

    Console.WriteLine(response.Result);
}
static async Task DoManyGreetings(GreetService.GreetServiceClient client)
{
    var greeting = new Greeting()
    {
        FirstName = "Clement",
        LastName = "Jean"
    };

    var request = new GreetingManyTimesRequest() { Greeting = greeting };
    var response = client.GreetManyTimes(request);

    while (await response.ResponseStream.MoveNext())
    {
        Console.WriteLine(response.ResponseStream.Current.Result);
        await Task.Delay(200);
    }
}
static async Task DoLongGreet(GreetService.GreetServiceClient client)
{
    var greeting = new Greeting()
    {
        FirstName = "Clement",
        LastName = "Jean"
    };

    var request = new LongGreetRequest() { Greeting = greeting };
    var stream = client.LongGreet();

    foreach (int i in Enumerable.Range(1, 10))
    {
        await stream.RequestStream.WriteAsync(request);
    }

    await stream.RequestStream.CompleteAsync();

    var response = await stream.ResponseAsync;

    Console.WriteLine(response.Result);
}
static async Task DoGreetEveryone(GreetService.GreetServiceClient client)
{
    var stream = client.GreetEveryOne();

    var responseReaderTask = Task.Run(async () =>
    {
        while (await stream.ResponseStream.MoveNext())
        {
            Console.WriteLine("Received : " + stream.ResponseStream.Current.Result);
        }
    });

    Greeting[] greetings =
    {
                new Greeting() { FirstName = "John", LastName = "Doe" },
                new Greeting() { FirstName = "Clement", LastName = "Jean" },
                new Greeting() { FirstName = "Patricia", LastName = "Hertz" }
            };

    foreach (var greeting in greetings)
    {
        Console.WriteLine("Sending : " + greeting.ToString());
        await stream.RequestStream.WriteAsync(new GreetEveryOneRequest()
        {
            Greeting = greeting
        });
    }

    await stream.RequestStream.CompleteAsync();
    await responseReaderTask;
}

static async Task GeetingWithDeadline(GreetService.GreetServiceClient client)
{
    var greeting = new Greeting()
    {
        FirstName = "Clement",
        LastName = "Jean"
    };
    try
    {
        var request = new GreetingRequest() { Greeting = greeting };
        var response = client.GreetWithDeadline(request,deadline:DateTime.UtcNow.AddMilliseconds(5000));
        Console.WriteLine(response.Result);
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
    {
        Console.WriteLine("Error: " + ex.Status.Detail);
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
    {
        Console.WriteLine("Error: " + ex.Status.Detail);
    }

}

static async Task SqrtServcie(Channel channel)
{
    var client = new SqrtService.SqrtServiceClient(channel);
    var request = new SqrtRequest() { Number = -1 };
    try
    {
        var response = await client.SqrtAsync(request);
        Console.WriteLine("Square root of {0} is: {1}", request.Number, response.Result);
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
    {
        Console.WriteLine("Error: " + ex.Status.Detail);
    }
}



