//  --------------------------------------------------------------------------------------
// DependencyInjection.GettingStarted.CompositionRoot.cs
// 2017/01/11
//  --------------------------------------------------------------------------------------

using System;

namespace DependencyInjection.GettingStarted
{
    public class CompositionRoot : ICompositionRoot
    {
        readonly IConsoleWriter consoleWriter;
        readonly ISingletonDemo singletonDemo;

        public CompositionRoot(IConsoleWriter consoleWriter, ISingletonDemo singletonDemo)
        {
            this.consoleWriter = consoleWriter;
            this.singletonDemo = singletonDemo;
            consoleWriter.LogMessage("Hello from CompositionRoot Constructor!");
        }

        public void LogMessage(string message)
        {
            Console.WriteLine($"CompositionRoot.LogMessage:  singletonDemo.ObjectId = {singletonDemo.ObjectId}");

            consoleWriter.LogMessage(message);
        }
    }
}