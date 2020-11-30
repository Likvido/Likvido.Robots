using System;

namespace Likvido.Robots
{
    public interface IOperationSyncBuilder : IOperationBuilder
    {
        Action Build();
        void Run();
    }
}
