[![Publish to nuget](https://github.com/Likvido/Likvido.Robots/workflows/Publish%20to%20nuget/badge.svg)](https://github.com/Likvido/Likvido.Robots/actions?query=workflow%3A%22Publish+to+nuget%22)
[![Nuget](https://img.shields.io/nuget/v/Likvido.Robots)](https://www.nuget.org/packages/Likvido.Robots/)
# Likvido.Robots
Library which helps with running simple tasks with telemetry and logging
# Usage
## An async execution
```
await new Robot("Name")
    .BuildOperation("operation")
    .SetConfigureServices((configuration, services) => { })
    .SetOnServiceProviderBuild(sp => { })
    .SetFunc(c => Task.CompletedTask)
    .SetPostExecute(() => Task.CompletedTask)
    .Run();
```
## A sync execution
```
new Robot("Name")
    .BuildOperation("operation")
    .SetConfigureServices((configuration, services) => { })
    .SetOnServiceProviderBuild(sp => { })
    .SetFunc(c => { })
    .SetPostExecute(() => { })
    .Run();
```
## An async reusable operation
```
AsyncOperation asyncOperation = new Robot("Name")
    .BuildOperation("operation")
    .SetConfigureServices((configuration, services) => { })
    .SetOnServiceProviderBuild(sp => { })
    .SetFunc(c => Task.CompletedTask)
    .SetPostExecute(() => Task.CompletedTask)
    .Build();
```
## A sync reusable operation
```
Operation operation = new Robot("Name")
    .BuildOperation("operation")
    .SetConfigureServices((configuration, services) => { })
    .SetOnServiceProviderBuild(sp => { })
    .SetFunc(c => { })
    .SetPostExecute(() => {})
    .Build();
```
