//  --------------------------------------------------------------------------------------
// DependencyInjection.GettingStarted.ConsoleWriter.cs
// 2017/01/11
//  --------------------------------------------------------------------------------------

using System;

namespace DependencyInjection.GettingStarted
{
    public class ConsoleWriter : IConsoleWriter
    {
        readonly ISingletonDemo singletonDemo;

        public ConsoleWriter(ISingletonDemo singletonDemo)
        {
            this.singletonDemo = singletonDemo;
        }

        public void LogMessage(string message)
        {
            Console.WriteLine($"ConsoleWriter.LogMessage:  singletonDemo.ObjectId = {singletonDemo.ObjectId}");
            Console.WriteLine(message);
        }
    }
}