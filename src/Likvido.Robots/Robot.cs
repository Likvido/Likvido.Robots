using System;

namespace Likvido.Robots
{
    public delegate void ReportError(IServiceProvider serviceProvider, string message, Exception e);

    public class Robot
    {
        public Robot(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public IOperationBuilder BuildOperation(string name)
        {
            return new OperationBuilder(Name, name);
        }
    }
}
