//  --------------------------------------------------------------------------------------
// DependencyInjection.GettingStarted.SingletonDemo.cs
// 2017/01/11
//  --------------------------------------------------------------------------------------

using System;

namespace DependencyInjection.GettingStarted
{
    public class SingletonDemo : ISingletonDemo
    {
        public SingletonDemo()
        {
            ObjectId = Guid.NewGuid();
        }

        public Guid ObjectId { get; }
    }
}