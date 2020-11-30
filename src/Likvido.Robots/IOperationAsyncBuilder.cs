using System;
using System.Threading.Tasks;

namespace Likvido.Robots
{
    public interface IOperationAsyncBuilder : IOperationBuilder
    {
        Func<Task> Build();
        Task Run();
    }
}
