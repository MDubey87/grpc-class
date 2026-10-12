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
            while(await requestStream.MoveNext())
            {
                var greeting = requestStream.Current.Greeting;
                result += $"Hello, {greeting.FirstName} {greeting.LastName}!\n";
            }
            return new LongGreetResponse { Result = result };
        }
    }
}
