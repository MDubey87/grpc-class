using Calculator;
using Grpc.Core;
using static Calculator.CalculatorService;

namespace server
{
    public class CalculatorServiceImpl : CalculatorServiceBase
    {
        public override Task<SumResponse> Sum(SumRequest request, ServerCallContext context)
        {
            var response = new SumResponse
            {
                Result = request.Number1 + request.Number2
            };
            return Task.FromResult(response);
        }
    }
}
