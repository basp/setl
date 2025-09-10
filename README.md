# Setl
Simple ETL.

A heavily stripped down version of [RhinoETL](https://github.com/ayende/rhino-etl)
by [@ayende](https://github.com/ayende).

## Getting started
### Creating an `EtlProcess` class
Any *process* implementation needs to derive from `EtlProcess`. We'll define a `ProcessNumbers` process class.
```
internal class ProcessNumbers : EtlProcess
{
}
```

### Adding the *constructor*
Since this derives from `EtlProcess` it will need to pass in an `ILogger` to the base *constructor* (`.ctor`). Well define this `.ctor` in our `ProcessNumbers` class and it will just pass the `Ilogger` along to the `base` (`EtlProcess`).
```
internal class Example2
{
    internal class ProcessNumbers : EtlProcess
    {
        private readonly ILogger logger;
        
        public ProcessNumbers(
            ILogger logger, 
            IPipelineExecutor executor)
        : base(logger, executor)
        {        
            this.logger = logger;
        }

        protected override void Initialize()
        {
            throw new NotImplementedException();
        }
    }

    public static void Run(ILoggerFactory loggerFactory)
    {
    }
}
```

### Small step back
Every `EtlProcess` implementation, at the bare minimum, needs an `ILogger` instance to function. As do many other components of the framework.

In order to setup a `EtlProcess` that actually *does* something we need to *register* one or more `IOperation` instances. We don't have any yet and none are included *out-of-the-box*. So, the first thing we need to do is to define a few of those and then we'll come back to finish up our `ProcessNumbers` `EtlProcess` implementation.

## A bit about ETL
### Three types of operations
When dealing with an ETL pipeline there's usually three kinds of operations:

* Extract (`() -> A`)
* Transform (`A -> B`)
* Load (`B -> ()`)

#### Extract
An extract operation tends to *produce* information. It functions as a faucet that flows information into the system. In the example below, we have an extract function `E` that extracts a list of messages `M` of type `a` (i.e. `M<a>`).
```
E() -> M<a>[]
```

#### Transform
A transform operation is like an obstacle in the river. It will shape the data flowing through. In this example we have a transform function of `T<a,b>` which accepts a list of messages of type `M<a>` and *transforms* them to a list of messages of type `M<b>`.
```
T<a,b>(M<a>[]) -> M<b>[]
```

#### Load
The load operation is like a sink. It takes a bunch of data and magically makes it disappear. In this example, we have a load function of `L<b>` which takes a list of messages of type `M<b>` and *deals* with them (in some way or another).  
```
L<b>(M<b>[]) -> ()
```
> Whether it's actually persisting the messages somewhere or just ignoring them is unclear from the signature. But actually it doesn't matter. It could be an actual database on the other end or just some mock object pretending to be one. What's important is that this gives us a clear conceptual model to reason about these kinds of transformations.

#### Remarks
* This doesn't correspond nicely to CQRS and it doesn't have to. An ETL flow (by definition) is a *command* so the CQRS paradigm is not applicable here.
* In the real world those nice abstract signatures above can have additional events associated with them so they are actually a simplified view of reality.
> For example, a transformation is not only an operation where `M<A>` is transformed into `M<b>` but it can also cause events to be published into the world about the operation it's performing (i.e. monitoring, logging, validation and error collecting, etc.)
>
> This doesn't necessarily mess up our model but it is an aspect we need to keep in mind when actually engineering the data flow.

### Deriving from `AbstractOperation`
That should be enough theory for now. Let's actually implement some operations.

First we will need some kind of `IOperation` implementation. In most cases, the easiest way is to implement `AbstractOperation`. In this case we will setup a small list of `Foo` entities and use that as our source data.

```
internal class ExtractFakeData
{
    // A hard coded *fake-data* source
    private class ExtractFakeData : AbstractOperation
    {
        // A hard coded *fake-data* source
        private readonly List<Foo> sourceFoo =
        [
            new() { Id = 1, Name = "Foo_One" },
            new() { Id = 2, Name = "Foo_Two" },
            new() { Id = 3, Name = "Foo_Three" },
        ];

        // Mandatory `ILogger` ctor
        public ExtractFakeData(ILogger logger) : base(logger)
        {
        }
    
        // Custom name (optional)
        public override string Name => "extract-fake-data";


        // Yield our fake object data source as `Row` instances
        public override IEnumerable<Row> Execute(
            IEnumerable<Row> rows)
        {
            foreach (var foo in sourceFoo)
            {
                yield return Row.FromObject(foo);
            }
        }
    }
}
```
### A little bit of guidance:
* Defining the `.ctor` that takes an `ILogger` argument is **mandatory**.
* Overriding the `Name` property is *optional* (it will default to the dynamic name of the type).
* Overriding `Execute` is **strongly recommended** if you actually want to do something useful.

Everyting int **Setl** works on `Row` instances. So in order to get any kind of data into the system we need to convert it to rows. When dealing with objects this is quite easy, we can *usually* just use the `Row.FromObject` method to convert any object to a `Row` instance. 
> This is particularly useful when you're trying to ETL within a JSON heavy domain where entities are king and tables are pawns. Keep in mind though that there is some magic involved so it's good practice to keep your `Row` instances as simple and flat as possible (i.e. make use of well known and easily serializable values and don't put deep hierarchies in the fields of your rows).