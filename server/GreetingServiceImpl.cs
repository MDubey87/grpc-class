using Greet;
using Grpc.Core;
using static Greet.GreetService;

namespace server
{
    public class GreetingServiceImpl : GreetServiceBase
    {
        public override Task<GreetingResponse> Greet(GreetingRequest request, ServerCallContext context)
        {
            var response = new GreetingResponse
            {
                Result = $"Hello, {request.Greeting.FirstName} {request.Greeting.LastName}!"
            };
            return Task.FromResult(response);
        }
        public override async Task GreetManyTimes(GreetingManyTimesRequest request, IServerStreamWriter<GreetingManyTimesResponse> responseStream, ServerCallContext context)
        {
            for (int i = 0; i < 5; i++)
            {
                var response = new GreetingManyTimesResponse
                {
                    Result = $"Hello, {request.Greeting.FirstName} {request.Greeting.LastName}! This is message {i + 1}."
                };
                await responseStream.WriteAsync(response);
                await Task.Delay(1000); // Simulate some delay
            }
        }
        public override async Task<LongGreetResponse> LongGreet(IAsyncStreamReader<LongGreetRequest> requestStream, ServerCallContext context)
        {
            var result = "";
            while (await requestStream.MoveNext())
            {
                var greeting = requestStream.Current.Greeting;
                result += $"Hello, {greeting.FirstName} {greeting.LastName}!\n";
            }
            return new LongGreetResponse { Result = result };
        }
        public override async Task GreetEveryOne(IAsyncStreamReader<GreetEveryOneRequest> requestStream, IServerStreamWriter<GreetEveryOneResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var greeting = requestStream.Current.Greeting;
                var result = $"Hello, {greeting.FirstName} {greeting.LastName}!";
                var response = new GreetEveryOneResponse
                {
                    Result = result
                };
                Console.WriteLine($"Received :{result}");
                await responseStream.WriteAsync(response);
            }
        }
        public override async Task<GreetingResponse> GreetWithDeadline(GreetingRequest request, ServerCallContext context)
        {
            for (int i = 0; i < 3; i++)
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Request was cancelled by the client.");
                    throw new RpcException(new Status(StatusCode.Cancelled, "Request was cancelled by the client."));
                }
                Thread.Sleep(1000); // Simulate some work
            }
            //await Task.Delay(300);
            var response = new GreetingResponse
            {
                Result = $"Hello, {request.Greeting.FirstName} {request.Greeting.LastName}!"
            };
            return response;
        }
    }
}
