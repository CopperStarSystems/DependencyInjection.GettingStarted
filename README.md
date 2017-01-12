## Introduction 

Dependency Injection (DI) is an established design pattern describing how objects acquire their dependencies.
  This pattern is often facilitated by an Inversion of Control (IoC) container, which is used at runtime 
  to resolve and inject dependencies as objects are instantiated.

### So what do I gain from using DI/IoC?
DI and IoC provide a host of benefits to medium and large-scale projects:
* DI significantly reduces coupling between components and their dependencies
* DI encourages engineers to use [Composition vs. Inheritance](https://en.wikipedia.org/wiki/Composition_over_inheritance), resulting in more flexible implementations
* IoC enables substitution of alternate implementations via configuration, and in some cases even at runtime.
* DI easily supports N-Tier architectures

### Are there reasons I wouldn't want to use DI/IoC?
Although DI/IoC are awesome, they're not always the best choice.  Honestly I've found that it's best to start with DI in mind at least (i.e. injecting dependencies manually), and if the application grows to the point where manual injection becomes painful, introduce an IoC container, but don't just introduce one because you can.




## So Let's Get Down to it Already!

> **Prerequisites:**  I'll keep the tools to a minimum, but you'll definitely need a development environment of some sort.  If you're just starting out, I recommend [Visual Studio Community Edition](https://www.visualstudio.com/vs/community/), a free development environment for applications targeting the .NET framework.

> **There are many available IoC containers**:
> 
> It's important to note that while this tutorial uses [Castle Windsor](https://github.com/castleproject/Windsor/blob/master/docs/README.md) as an IoC container, there are several other frameworks that offer similar functionality:
>* SimpleIoc
>* AutoFac
>* Microsoft Unity (not to be confused with Unity Game Engine)
>* Ninject
> 
> Each framework offers slightly different advantages and features, but in the end they share most core concepts, including Type Registration, Resolution, and Injection.

### Create the example project
1.  Open Visual Studio
2.  Select `File -> New -> Project...`
3.  Select "Console Application", assign a name (I'll be using DependencyInjection.GettingStarted), and click Ok.

Once Visual Studio finishes creating the project, we'll have an ultra-simple application shell, but it doesn't do anything particularly useful yet - if you execute it, it will simply exit immediately.

> **Note:**  For simplicity's sake, this example is a basic console application rather than GUI-based.  The same principles would apply in a GUI application using DI/IoC.

### Install Castle Windsor
Castle Windsor is a third-party IoC container, meaning that it is not part of the .NET framework itself.  We will be using the NuGet Package Manager to install and reference the Castle components:
1. In Visual Studio, go to `Tools -> NuGet Package Manager -> Package Manager Console`
2. By default, the package manager console will open at the bottom of the Visual Studio window:

![NuGet Package Manager Console](https://cdn.filestackcontent.com/zHtOgzQSq4mZvWtfU2nQ "NuGet Package Manager Console")

3.  In the console window, type `Install-Package Castle.Windsor` and hit return

Visual Studio will go out to the [NuGet Package Repository](http://nuget.org), download the package to your local computer, and add a reference to the required Castle.Core and Castle.Windsor libraries.

**And now, we're almost ready to do some cool stuff!**

## Creating our Composition Root:
> **Note:**  The Composition Root is the root component for our application.  It is usually created during application bootstrapping and serves as a container for all other application logic.  For example, in a MVVM application, the MainViewModel is often considered the composition root.

> In this project, we'll call our main component CompositionRoot for clarity's sake.

#### Stub out the Composition Root class
1.  In Visual Studio Solution Explorer, right-click on the sample project and select `Add -> New Item`.
2.  In the Add New Item dialog, select "Interface" and call it ICompositionRoot
3.  Repeat steps 1 and 2, but select "Class" in step 2 and call it CompositionRoot
4.  Open the CompositionRoot class and replace the default code with this:

```c#
//  --------------------------------------------------------------------------------------
// DependencyInjection.GettingStarted.CompositionRoot.cs
// 2017/01/11
//  --------------------------------------------------------------------------------------

namespace DependencyInjection.GettingStarted
{
    public class CompositionRoot : ICompositionRoot
    {
        // We will do fancier stuff here in just a bit...
    }
}
```

#### Register the CompositionRoot with Castle Windsor
Ok, so that's all lovely and everything, but how does Castle know about these types?  The answer is simple:  Type Registration.  In order for the container to 'know about' and fulfill requests for types, each type must be registered with the Windsor Container.

To register our new type with Windsor, go to Program.cs and update the code as below:
```c#
//  --------------------------------------------------------------------------------------
// DependencyInjection.GettingStarted.Program.cs
// 2017/01/11
//  --------------------------------------------------------------------------------------

using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace DependencyInjection.GettingStarted
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new WindsorContainer();

            // Register the CompositionRoot type with the container
            container.Register(Component.For<ICompositionRoot>().ImplementedBy<CompositionRoot>());

            // Resolve an object of type ICompositionRoot (ask the container for an instance)
            // This is analagous to calling new() in a non-IoC application.
            var root = container.Resolve<ICompositionRoot>();
            
            // Wait for user input so they can check the program's output.
            Console.ReadLine();
        }
    }
}
```

> **Q:  So what did we actually just do?** 

> **A:** We established the infrastructure for our application, specifically:
* We created a CompositionRoot to contain other application logic
* We registered ICompositionRoot with the Windsor container and mapped it to its implementing CompositionRoot type
* We asked the WindsorContainer to give us an instance of ICompositionRoot and it news up a CompositionRoot and gives it to us.

> **Q:  So how do we even know this is working?**

>**A:** Well, right now the only way we can tell is that Windsor doesn't blow up when we run the application (comment out the `container.Register` call and run the app - it will fail spectacularly).  Honestly, this is one of the most challenging parts of using IoC - if resolution fails, it fails at runtime and usually throws a complex exception.
>
>Anyhow, let's go add some functionality to CompositionRoot to let it log some data to the console...

#### Adding an Internal Logging Feature to CompositionRoot
Let's add a simple feature to CompositionRoot so we can verify that an instance is actually being created for us by Castle Windsor:

Open up ICompositionRoot.cs and update the code as follows:
```c#
//  --------------------------------------------------------------------------------------
// DependencyInjection.GettingStarted.ICompositionRoot.cs
// 2017/01/11
//  --------------------------------------------------------------------------------------

namespace DependencyInjection.GettingStarted
{
    public interface ICompositionRoot
    {
        void LogMessage(string message);
    }
}
```
Likewise, open up CompositionRoot.cs and update the code as follows:
```c#
//  --------------------------------------------------------------------------------------
// DependencyInjection.GettingStarted.CompositionRoot.cs
// 2017/01/11
//  --------------------------------------------------------------------------------------

using System;

namespace DependencyInjection.GettingStarted
{
    public class CompositionRoot : ICompositionRoot
    {
        public void LogMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
```

Effectively, we just added a method that will log a given message to the console.  Now let's move over to 
our app startup again, and edit the code to log a message after we've resolved the composition root:
```c#
//  --------------------------------------------------------------------------------------
// DependencyInjection.GettingStarted.Program.cs
// 2017/01/11
//  --------------------------------------------------------------------------------------

using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace DependencyInjection.GettingStarted
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new WindsorContainer();

            // Register the CompositionRoot type with the container
            container.Register(Component.For<ICompositionRoot>().ImplementedBy<CompositionRoot>());

            // Resolve an object of type ICompositionRoot (ask the container for an instance)
            // This is analagous to calling new() in a non-IoC application.
            var root = container.Resolve<ICompositionRoot>();

            root.LogMessage("Hello from my very first resolved class!");

            // Wait for user input so they can check the program's output.
            Console.ReadLine();
        }
    }
}
```
Press `F5` to run the application and you should see "Hello from my very first resolved class!" displayed on the console.

> **Q:  So this is fun and exciting and all, but I'm really struggling to see the value here...**

> **A:** That's OK, we haven't done anything fancy just yet, so let's move on and create a reusable logging service, eh?

## Making our logging function reusable:
So now we've seen the very basics of registering types, but let's take it a step further:  Let's extract out the console writing functionality so that we can easily reuse it in other classes (and unit test it too!)

### Extract the logging functionality
Similar to the prior example, we want to create an interface and a corresponding class.  Let's call them IConsoleWriter and ConsoleWriter, respectively
Update the IConsoleWriter code like so:

```c#
//  --------------------------------------------------------------------------------------
// DependencyInjection.GettingStarted.IConsoleWriter.cs
// 2017/01/11
//  --------------------------------------------------------------------------------------

namespace DependencyInjection.GettingStarted
{
    public interface IConsoleWriter
    {
        void LogMessage(string message);
    }
}
```

Update the ConsoleWriter code like so:
```c#
//  --------------------------------------------------------------------------------------
// DependencyInjection.GettingStarted.ConsoleWriter.cs
// 2017/01/11
//  --------------------------------------------------------------------------------------

using System;

namespace DependencyInjection.GettingStarted
{
    public class ConsoleWriter : IConsoleWriter
    {
        public void LogMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
```
So, at this point, we have a class that knows how to write to the console, that we can now inject into other classes.
>**Note:**  This is a simple example of the [Single Responsibility Principle](https://en.wikipedia.org/wiki/Single_responsibility_principle) - this class has one responsibility: Writing to the console.

### Wiring up the new logger into our composition root
So now that we've extracted the console logging functionality into a separate class, let's look at how to use that class from Windsor:

First, start by opening CompositionRoot.cs and updating the code as below:
```c#
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

        public CompositionRoot(IConsoleWriter consoleWriter)
        {
            this.consoleWriter = consoleWriter;
            consoleWriter.LogMessage("Hello from CompositionRoot Constructor!");
        }

        public void LogMessage(string message)
        {
            consoleWriter.LogMessage(message);
        }
    }
}
```
Ok, so nothing really groundbreaking there, we just told CompositionRoot that it needs an instance of IConsoleWriter in order to do its work and added a call into our logger component from the constructor. Let's bring on the magic!

Open up Program.cs and add another registration for IConsoleWriter, mapped to ConsoleWriter:

```c#
//  --------------------------------------------------------------------------------------
// DependencyInjection.GettingStarted.Program.cs
// 2017/01/11
//  --------------------------------------------------------------------------------------

using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace DependencyInjection.GettingStarted
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new WindsorContainer();

            // Register the CompositionRoot type with the container
            container.Register(Component.For<ICompositionRoot>().ImplementedBy<CompositionRoot>());
            container.Register(Component.For<IConsoleWriter>().ImplementedBy<ConsoleWriter>());

            // Resolve an object of type ICompositionRoot (ask the container for an instance)
            // This is analagous to calling new() in a non-IoC application.
            var root = container.Resolve<ICompositionRoot>();

            root.LogMessage("Hello from my very first resolved class!");

            // Wait for user input so they can check the program's output.
            Console.ReadLine();
        }
    }
}
```

Now, when you execute the program, the output should look like:
```
Hello from CompositionRoot Constructor!
Hello from my very first resolved class!
```
>**Double-Take:** What just happened there?  We never gave an IConsoleWriter to CompositionRoot, so how on earth did it get ahold of one?  I mean, if I wrote this code:
>```c#
> var x = new CompositionRoot();
>```
>it wouldn't even compile, because CompositionRoot's constructor **clearly** requires an IConsoleWriter, right?!?
>
>**A:** This is your first taste of Castle Windsor's magic.  At a basic level, this is what happens when we make the call to `container.Resolve<ICompositionRoot>()`:
>* Windsor looks up ICompositionRoot in its internal registry and sees that it is mapped to the CompositionRoot concrete type.
>* Windsor looks at the CompositionRoot type and sees that its constructor has a dependency on IConsoleWriter.
>* Windsor repeats the process for IConsoleWriter, seeing that ConsoleWriter has no constructor dependencies
>* Windsor creates an instance of ConsoleWriter (the mechanics aren't important at this level)
>* Windsor creates an instance of CompositionRoot, supplying the ConsoleWriter it created in the prior step
>* Windsor returns the created CompositionRoot instance, which is fully configured with the ConsoleWriter it requires for its work.
> 
> This automated dependency injection is one of the most valuable features offered by Castle Windsor and other IoC frameworks.


## Conclusion:
Dependency Injection and Inversion of Control are powerful tools available to modern software engineers.  The Dependency Injection design pattern enables flexible components, composed themselves of smaller components, while Inversion of Control frees the engineer from having to manually track, instantiate, inject, and destroy object instances.

Although not covered in this tutorial, adhering to the DI pattern also significantly improves engineers' ability to unit test their code in isolation from other subsystems, focusing primarily on a class' interactions with its dependencies.

In this tutorial, we have scratched the very surface of DI and IoC, focusing only on the most fundamental of concepts.  The Castle Windsor library is very mature and highly-extensible, offering a host of features not covered in this specific tutorial.  Keep an eye out for future tutorials where I'll cover:
* The Castle Windsor TypedFactoryFacility:  Easily resolve types that have runtime dependencies!
* How (and why) to resolve multiple types mapped to the same interface
* Customizing Castle Windsor injection behavior
* Automating Castle Container bootstrapping using Reflection, Attributes, and Conventions


## Digging Deeper:  Object Lifestyles
Another important concept managed by IoC containers is that of object lifetimes.  In most applications of any realistic complexity, there may be tens of thousands of objects created and used over the lifetime of a session.
In many cases, these objects are needed only for a very short time and should be released for garbage collection as soon as possible.  In other cases, you may need to ensure that only one instance of a specific type is created and that same instance is used everywhere that type is needed.

Castle Windsor uses the metaphor of "Lifestyles" to describe object lifetimes.  For example, the first scenario above corresponds to a "Transient" lifestyle, while the second corresponds to a "Singleton" lifestyle.  [Castle offers several other lifestyle options for specialized scenarios](https://github.com/castleproject/Windsor/blob/master/docs/lifestyles.md).

>**I'm not going to cover it in depth in this tutorial ,but let's take a first look at how we can use these Lifestyles to control Windsor's behavior:**

### Introduce and register a singleton
As usual, we'll start by adding an interface and a class, let's call them ISingletonDemo and SingletonDemo respectively.
Open ISingletonDemo and update the code like so:

```c#
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
```

We're basically just defining an interface that has a unique identifier so we can tell different instances apart.

Likewise, open the SingletonDemo class and update the code like so:

```c#
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
```

>Note that we assign a unique ObjectId to each instance when it is created.

We also need to register the interface/type with Windsor.  For now, let's just use the default (Singleton) Lifestyle:

```
//  --------------------------------------------------------------------------------------
// DependencyInjection.GettingStarted.Program.cs
// 2017/01/11
//  --------------------------------------------------------------------------------------

using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace DependencyInjection.GettingStarted
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new WindsorContainer();

            // Register the CompositionRoot type with the container
            container.Register(Component.For<ICompositionRoot>().ImplementedBy<CompositionRoot>());
            container.Register(Component.For<IConsoleWriter>().ImplementedBy<ConsoleWriter>());
            container.Register(Component.For<ISingletonDemo>().ImplementedBy<SingletonDemo>());

            // Resolve an object of type ICompositionRoot (ask the container for an instance)
            // This is analagous to calling new() in a non-IoC application.
            var root = container.Resolve<ICompositionRoot>();

            root.LogMessage("Hello from my very first resolved class!");

            // Wait for user input so they can check the program's output.
            Console.ReadLine();
        }
    }
}
``` 

And now, let's add a dependency on this component to both our CompositionRoot and ConsoleWriter components
>Note:  We don't have to do it this way, I'm just trying to illustrate a component that is used in multiple places and how that behavior works in Transient vs. Singleton Lifestyles.

Add the dependency to ConsoleWriter by specifying an ISingletonDemo in the constructor:

```c#
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
            Console.WriteLine("ConsoleWriter.LogMessage:  singletonDemo.InvocationCount={0}",
                              singletonDemo.InvocationCount);
            Console.WriteLine(message);
        }
    }
}
```

Likewise with CompositionRoot - Add a constructor dependency on ISingletonDemo:

```c#
//  --------------------------------------------------------------------------------------
// DependencyInjection.GettingStarted.CompositionRoot.cs
// 2017/01/11
//  --------------------------------------------------------------------------------------

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
            singletonDemo.DoSomeWork();
            var msg = $"CompositionRoot.LogMessage:  singletonDemo.InvocationCount={singletonDemo.InvocationCount}";
            consoleWriter.LogMessage(msg);
            consoleWriter.LogMessage(message);
        }
    }
}
```

Now when you execute the application, the output will look like:

```
ConsoleWriter.LogMessage:  singletonDemo.ObjectId = 1c87be72-b1c5-41e8-bb63-a71c72b3f214
Hello from CompositionRoot Constructor!
CompositionRoot.LogMessage:  singletonDemo.ObjectId = 1c87be72-b1c5-41e8-bb63-a71c72b3f214
ConsoleWriter.LogMessage:  singletonDemo.ObjectId = 1c87be72-b1c5-41e8-bb63-a71c72b3f214
Hello from my very first resolved class!
```

>Notice that all the ObjectIds are the same, telling us that the same SingletonDemo instance is being used in both CompositionRoot and ConsoleWriter.

### From Singleton to Transient
Ok, but sometimes we don't want objects to live forever, instead we'd like a shiny new instance each time a particular type is requested.  Fortunately, the Transient Lifestyle has us covered!

To use a type as Transient, we have to tell Castle that's what we want, so let's go edit Program.cs

```c#
//  --------------------------------------------------------------------------------------
// DependencyInjection.GettingStarted.Program.cs
// 2017/01/11
//  --------------------------------------------------------------------------------------

using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace DependencyInjection.GettingStarted
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new WindsorContainer();

            // Register the CompositionRoot type with the container
            container.Register(Component.For<ICompositionRoot>().ImplementedBy<CompositionRoot>());
            container.Register(Component.For<IConsoleWriter>().ImplementedBy<ConsoleWriter>());
            container.Register(Component.For<ISingletonDemo>().ImplementedBy<SingletonDemo>().LifestyleTransient());

            // Resolve an object of type ICompositionRoot (ask the container for an instance)
            // This is analagous to calling new() in a non-IoC application.
            var root = container.Resolve<ICompositionRoot>();

            root.LogMessage("Hello from my very first resolved class!");

            // Wait for user input so they can check the program's output.
            Console.ReadLine();
        }
    }
}
```

>Note:  All we did there was add `.LifestyleTransient()` to the ISingletonDemo registration.

Now when we execute the application, the output should resemble:

```
ConsoleWriter.LogMessage:  singletonDemo.ObjectId = c83ae1c0-4cb7-4bbf-a8a3-b462a98ff36c
Hello from CompositionRoot Constructor!
CompositionRoot.LogMessage:  singletonDemo.ObjectId = ec2c9635-1e58-4044-9086-8c3f88ccf620
ConsoleWriter.LogMessage:  singletonDemo.ObjectId = c83ae1c0-4cb7-4bbf-a8a3-b462a98ff36c
Hello from my very first resolved class!
```

>Note that the ObjectIds are now different depending on whether they're the instance from ConsoleWriter or CompositionRoot
---

## Glossary of Terms
During this tutorial I use some terms specific to DI that may be unfamiliar to the reader:

**Dependency:**  A dependency is something (usually another class) that a class relies upon to do its work, e.g.:

```
public class someClass{
    // SomeClass depends upon SomeService, therefore SomeService is a dependency of SomeClass.
    public SomeClass(SomeService service){
        // ...do some work with service...
    }
}
```

**Injection:**  Injection refers to various ways to provide dependencies to classes from outside the class itself.  There are three primary injection methods:  Constructor, Property, and Setter.  This tutorial will focus on using Castle Windsor's constructor injection approach.  Below is an example of code that is **not** using dependency injection.  Contrast this with the code in the prior block, which is using Constructor Injection:

```
// Example of bad (but commonly seen in example code) approach to dependency management
public class someClass{
    // SomeClass depends upon SomeService
    public SomeClass(){
        // Notice that we're instantiating service here, using the new keyword.
        // This is the direct opposite of DI/IoC.
        var service = new SomeService();
        // ...do some work with service...
    }
}
```
**By implementing the code as above, we have lost significant value:**
* We can no longer inject a fake instance of SomeService in for unit testing purposes
* SomeClass is now very tightly coupled to SomeService
* We can no longer swap out service implementations at runtime if needed
* If SomeService's constructor dependencies change, we will also need to change SomeClass to match
* SomeClass' dependencies are now hidden - if we need to know what SomeClass depends on, we have to open the code and examine the entire code file.

**Dependency Injection (DI):** A design pattern in which classes specify their dependencies so they can be provided from external sources rather than creating them directly;  Although DI can be implemented manually, in most applications of moderate or greater size an Inversion of Control (IoC) container is used to facilitate the tracking, instantiation, lifetime, and injection of registered types and instances.  

>*It's important to remember that DI is a dev-time concept, while an IoC container is a runtime tool that facilitates DI by injecting dependencies at runtime as needed.*

**Inversion of Control (IoC) Container:** IoC containers are runtime tools that perform several duties related to type resolution:
* Acts as a registry of known types (i.e. application-specific types, etc.)
* Can (in most cases) instantiate and provide types to fulfill dependencies (Type Resolution)
* Tracks and controls lifetime of objects instantiated by the container

**Bootstrapping:**  Bootstrapping (no relation to Bootstrap CSS) is the process of starting up an application, and it typically involves several steps even when not using IoC.  Generally, when using an IoC container, the bootstrapping process contains the following steps:
* Configure Windsor Container:  In this step, we tell the Windsor Container what types we want it to know about (so it can resolve them), and how we want their lifetime to be managed (i.e. a new instance for every request, one application-wide instance, etc.)
* Resolve the Composition Root:  In a DI application, we use the concept of "Composition Root" to represent the main object resolved at runtime.  Normally, all other objects are considered logical children to this root, for example:

```
// Imagine this is a WPF/MVVM project.  MainViewModel is the ViewModel that represents the
// entire application (i.e. MainView is the application shell)...
public class MainViewModel : IMainViewModel
{
    // Note:  PropertyChanged notifications, etc. omitted for brevity.
    public MainViewModel(ISomeViewModel otherViewModel){
        this.SomeOtherViewModel = otherViewModel;
    }

   public ISomeViewModel SomeOtherViewModel{get;set;}
}


// ...In your app startup code...
// Container configuration omitted, assume that IMainViewModel and ISomeViewModel are registered.
IWindsorContainer container = ConfigureContainer();

// Ask Windsor for an IMainViewModel instance:
IMainViewModel viewModel = container.Resolve<IMainViewModel>();

```
So how does this all play out?  

MainViewModel is the application's Composition Root (because we decided it would be), so we configure the container and then ask it to give us an instance of IMainViewModel.  At that point, Castle sees that IMainViewModel depends on ISomeViewModel, so it also resolves an instance of ISomeViewModel and then gives it to the MainViewModel constructor.

At first it might seem like magic, but if your architecture is correctly designed, you should only have to call container.Resolve<T> in your app startup code - Windsor will take care of the rest!


**[ServiceLocator Antipattern](http://blog.ploeh.dk/2010/02/03/ServiceLocatorisanAnti-Pattern/):**  I'm pointing this out because it is a pattern often seen in poorly-written example code for DI.  In effect, ServiceLocator requires you to pass around an IoC container so that classes can resolve types themselves, effectively circumventing the value of DI and IoC.  It's easiest to recognize when you see calls to Container.Resolve<T>() in places other than your bootstrap code.





