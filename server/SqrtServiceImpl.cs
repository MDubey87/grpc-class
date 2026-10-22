using Grpc.Core;
using Sqrt;
using static Sqrt.SqrtService;

namespace server
{
    public class SqrtServiceImpl : SqrtServiceBase
    {
        public override async Task<SqrtResponse> Sqrt(SqrtRequest request, Grpc.Core.ServerCallContext context)
        {
            var number = request.Number;
            if (number >= 0)
            {
                var result = Math.Sqrt(number);
                return new SqrtResponse { Result = result };
            }
            else
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Number must be positive"));
            }
        }
    }


}
