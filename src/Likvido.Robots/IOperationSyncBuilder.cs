using System;

namespace Likvido.Robots
{
    public interface IOperationSyncBuilder : IOperationBuilder
    {
        Operation Build();
        void Run();
    }
}
