using System;
using System.Threading.Tasks;

namespace Likvido.Robots
{
    public interface IOperationAsyncBuilder : IOperationBuilder
    {
        AsyncOperation Build();
        Task Run();
    }
}
