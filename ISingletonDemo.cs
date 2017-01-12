//  --------------------------------------------------------------------------------------
// DependencyInjection.GettingStarted.ISingletonDemo.cs
// 2017/01/11
//  --------------------------------------------------------------------------------------

using System;

namespace DependencyInjection.GettingStarted
{
    public interface ISingletonDemo
    {
        Guid ObjectId { get; }
    }
}