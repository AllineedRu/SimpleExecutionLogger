# SimpleExecutionLogger

## Table of Contents
* [Description](#description)
* [Usage](#usage)

## Description
**SimpleExecutionLogger** is a small library that allows you to add logging of methods and their execution steps to your code. 
You can track how long it took to execute the methods of interest and get a text log containing a 
stack of calls to methods for which logging is enabled.

In order to start logging the execution of methods in the target class of interest, it is enough to create 
a logger instance in it and add calls to start logging entry into and exit from the method of interest. 
You can also add custom steps inside the method with a description of what happens in that step, and this 
information will also end up in the final log.

To better understand the call stack, the logger provides the ability to indent nested methods that are called 
from a parent method: thus, when reading the final log, you can understand which parts of the code take the most 
time and optimize your code in case it is required.

It is recommended to perform logging at the stage of development and debugging of your application to find 
bottlenecks or problem areas that may not be optimal in terms of execution time. At the final stage of working on the code, 
it is recommended to remove method logging and the logger instance from the class being debugged, when the optimization 
is already completed, since logging also imposes additional overhead at runtime.

## Installation

To install the library you will need to install `SimpleExecutionLogger` NuGet package for your project or
build from sources and add a link to the library from your project.

## Usage

Let's say you have a console application and its main class `MyAmazingClass` containing the `public static void Main()` method
as its entry point. And this class needs to be logged and instrumented, i.e. you need to understand
how much time methods `Main`, `MyMethod1`, `MyMethod2` and `OtherMethodN` have been executing:

```cs
public class MyAmazingClass {

	// Class constructor
	public MyAmazingClass() {
		// ... some initialization code ...
	}

	// Main entry point of your class's logic
	public static void Main() {
		Console.WriteLine("Hello, I'm in MyAmazingClass!");
		Console.WriteLine();

		// calling MyMethod1
		MyMethod1();
	}

	private void MyMethod1() {
		// ... some code ...

		// now calling MyMethod2 from MyMethod1
		MyMethod2();
	}

	private void MyMethod2() {
		// ... some code ...

		// lastly, we are calling OtherMethodN from MyMethod2
		OtherMethodN();
	}

	// ... other methods of MyAmazingClass ...

	private void OtherMethodN() {
		// ... other instructions ... 
	}
}
```

This is where the **SimpleExecutionLogger** library can help you. First of all, import the required `SimpleExecutionLogger` namespace:
```cs
using SimpleExecutionLogger;

public class MyAmazingClass {
	// ...
}
```

Now you can create a logger variable in your class (this variable is of `ExecutionLogger` type) and
specify the desired name for your logger in the constructor call for `ExecutionLogger` instance:
```cs
using SimpleExecutionLogger;

public class MyAmazingClass {

	// Static logger instance for MyAmazingClass
	private static ExecutionLogger logger = new ExecutionLogger("My Amazing Logger");

	// Class constructor
	public MyAmazingClass() {
		// ... some initialization code ...
	}

	// ...
}
```


And that's it! Now you have logger instance for `MyAmazingClass` class and one can add some logging to those methods of your class you want 
to analyze, log & trace. Let's add logging to our methods `Main`, `MyMethod1`, `MyMethod2` and `OtherMethodN`:
```cs
public class MyAmazingClass {
	// Static logger instance for MyAmazingClass
	private static ExecutionLogger logger = new ExecutionLogger("My Amazing Logger");

	// Class constructor
	public MyAmazingClass() {
		// ... some initialization code ...
	}

	// Main entry point of your class's logic
	public static void Main() {
		logger.StartMethod();

		Console.WriteLine("Hello, I'm in MyAmazingClass!");
		Console.WriteLine();

		// calling MyMethod1
		MyMethod1();

        logger.EndMethod();
	}

	private static void MyMethod1() {
		logger.StartMethod();
		
		// ... some instructions here ...

		// now calling MyMethod2 from MyMethod1
		MyMethod2();

		logger.EndMethod();
	}

	private static void MyMethod2() {
		logger.StartMethod();
		
		// ... some instructions here ...

		// lastly, we are calling OtherMethodN from MyMethod2
		OtherMethodN();

		logger.EndMethod();
	}

	// ... other methods of MyAmazingClass ...

	private static void OtherMethodN() {
		logger.StartMethod();

		// ... other instructions ... 

		logger.EndMethod();
	}
}
```
Now all of the methods are tracked by the `logger` variable and when your class's starts executing
from `Main` method all the subsequent calls to other methods will be logged and execution 
time for every logged method will be collected as well.

_But wait... How can we access and view the collected logs now?_

It's easy, all you need to do is just to choose the right place for accessing the collected logs.
In our example case a good place for getting the collected logs is the last lines of the `Main` method,
after `logger.EndMethod()` call:
```cs
	// ...

	public static void Main() {
		logger.StartMethod();

		Console.WriteLine("Hello, I'm in MyAmazingClass!");
		Console.WriteLine();

		// calling MyMethod1
		MyMethod1();

		logger.EndMethod();

		// Get the logs collected by 'logger' instance
		string log = logger.GetLog();

		// Print the logs to the console
		Console.WriteLine("Collected logs:");
		Console.WriteLine(log);
	}

	// ...
```
Now if you start your application you should see the output like this:

```
Hello, I'm in MyAmazingClass!

Collected logs:
[My Amazing Logger]  >> Method 'Main' start at 09.12.2023 2:32:35
        [My Amazing Logger]  >> Method 'MyMethod1' start at 09.12.2023 2:32:35
                [My Amazing Logger]  >> Method 'MyMethod2' start at 09.12.2023 2:32:35
                        [My Amazing Logger]  >> Method 'OtherMethodN' start at 09.12.2023 2:32:35
                        [My Amazing Logger]  << Method 'OtherMethodN' end at 09.12.2023 2:32:35, duration: 0 ms
                [My Amazing Logger]  << Method 'MyMethod2' end at 09.12.2023 2:32:35, duration: 0 ms
        [My Amazing Logger]  << Method 'MyMethod1' end at 09.12.2023 2:32:35, duration: 0 ms
[My Amazing Logger]  << Method 'Main' end at 09.12.2023 2:32:35, duration: 15 ms
```

With the help of our `logger` instance now we've got a full information about the methods that
have been called by our application! Note that for every method we also have a timestamp when this method
has been started and the timestamp when it has been finished. The duration for every logged method 
has been also collected. For example, we can see that our `Main` method has been executing for **15 milliseconds**.
The rest of the methods `MyMethod1`, `MyMethod2`, `OtherMethodN` were too fast because they are empty in our short
example.