# SimpleExecutionLogger

## Table of Contents
* [Description](#description)
* [Installation](#installation)
* [Usage](#usage)
* [Configuration](#configuration)
* [API](#api)
	* [API Examples](#api-examples)
* [Contacts](#contacts)
* [License](#license)

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

:link: Link to NuGet package: https://www.nuget.org/packages/SimpleExecutionLogger/

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

## Configuration

`ExecutionLogger` class has several public properties that can be additionally configured for your logger instance
_before_ your start logging:

| Property  | Data Type | Description | Default value |
| ------------- | ------------- | ------------- | ------------- |
|`TabulationPrefix`|string|String, specifying a tab symbol or custom string used for nested methods indentation|`"\t"`|
|`MethodStartedLogPrefix`|string|Prefix added to the log when entering a method (at the beginning of the method execution)|`" >> Method "`|
|`MethodEndedLogPrefix`|string|Prefix added to the log when exiting from a method (at the end of the method execution, when returning from method)|`" << Method "`|
|`MethodStepLogPrefix`|string|Prefix added to the log when logging the specific step within executing method|`" Method "`|
|`LoggerNameFormat`|string|Format for printing the name of the logger instance. Custom user string, specifying the format, **must** contain only single formatting element `{0}`|`"[{0}] "`|
|`EnableLoggerName`|bool|Enable printing the logger instance name during logging. Set to false in order to hide the logger name|`true`|
|`StepNameFormat`|string|Format for printing the name of the method step. Custom user string, specifying the format, **must** contain only single formatting element `{0}`|`"[Step: {0}]: "`|
|`StartAtString`|string|Custom user string, specifying the phrase for start of method logging|`" start at "`|
|`EndAtString`|string|Custom user string, specifying the phrase for end of method logging|`" end at "`|
|`AtString`|string|Custom user string, specifying the phrase for start of method step execution|`" at "`|
|`MethodNameQuoteString`|string|String used for wrapping the name of the logged method|`"'"`|
|`StepNameSeparator`|string|Separator string between the name of the logged method and the name of the logged step inside this method|`": "`|
|`MethodExecutionDurationFormat`|string|String specifying the format of showing the execution duration of the logged method, **must** contain only single formatting element `{0}`|`", duration: {0} ms"`|
|`MethodStepDurationFormat`|string|A string that specifies the format for displaying the time elapsed from the start of a method's execution to the step within that method and the time delta between that step and the previous step, if any. **Must** contain only two formatting elements `{0}` and `{1}`|`", elapsed from start: {0} ms, delta={1} ms"`|

If you want to change some of these available properties to your custom values before starting the logging process,
you can set the desired values for your logger instance before the first call to any `StartMethod` call for `ExecutionLogger` logger instance.

For example, you can place your custom configuration in the static constructor of your class in case your logger instance is defined
as a static variable of your logged class:

```cs
using SimpleExecutionLogger;

public class MyAmazingClass {
	// Static logger instance for this class
	private static ExecutionLogger logger = new ExecutionLogger("My Amazing Logger");

	static MyAmazingClass() {
		logger.TabulationPrefix = "  ";
		logger.MethodStartedLogPrefix = " Started method ";
		logger.MethodEndedLogPrefix = " Exited from method ";
		logger.LoggerNameFormat = "(Logger: '{0}')";
		logger.StartAtString = " at ";
		logger.EndAtString = " at ";        
	}

	// ... other code in MyAmazingClass ...
}
```

Now if you run the example application we considered above, you'll get the output like this:
```
Hello, I'm in MyAmazingClass!

Collected logs:
(Logger: 'My Amazing Logger') Started method 'Main' at 09.12.2023 11:46:50
  (Logger: 'My Amazing Logger') Started method 'MyMethod1' at 09.12.2023 11:46:50
    (Logger: 'My Amazing Logger') Started method 'MyMethod2' at 09.12.2023 11:46:50
      (Logger: 'My Amazing Logger') Started method 'OtherMethodN' at 09.12.2023 11:46:50
      (Logger: 'My Amazing Logger') Exited from method 'OtherMethodN' at 09.12.2023 11:46:50, duration: 0 ms
    (Logger: 'My Amazing Logger') Exited from method 'MyMethod2' at 09.12.2023 11:46:50, duration: 0 ms
  (Logger: 'My Amazing Logger') Exited from method 'MyMethod1' at 09.12.2023 11:46:50, duration: 0 ms
(Logger: 'My Amazing Logger') Exited from method 'Main' at 09.12.2023 11:46:50, duration: 14 ms
```

In case your logger instance is defined as a _non-static field_ in the logged class, a good place to configure custom
values for available properties is the constructor of your logged class.

Let's say we have another class `MyAnotherAmazingClass` where logger instance is now defined as a
non-static class variable. In this case we add configuration for custom properties to the non-static
constructor of the this logged class:

```cs
public class MyAnotherAmazingClass {
	// Non-static logger instance for this class
	private ExecutionLogger logger = new ExecutionLogger("Another Amazing Logger");

	// Non-static class constructor
	public MyAnotherAmazingClass() {
		logger.TabulationPrefix = "  ";
		logger.MethodStartedLogPrefix = " Started method ";
		logger.MethodEndedLogPrefix = " Exited from method ";
		logger.LoggerNameFormat = "(Logger: '{0}')";
		logger.StartAtString = " at ";
		logger.EndAtString = " at ";
	}

	// ... other code in MyAnotherAmazingClass ...
}
```

## API
Besides `StartMethod()`, `EndMethod()` and `GetLog()` there are some other public methods in the `ExecutionLogger` class that
can be useful for managing the logging process.

Below is the list of other available methods in the `ExecutionLogger` class:

| Method  | Description |
| ------------- | ------------- |
|`void ClearLog()`|Performs full cleaning for the current logger instance: clears the current log and clears the stack of executing methods|
|`void ClearLoggedMethodsStack()`|Clears the stack, containing logged methods. Also resets the current method level to 0 value (indentation/tabulation level)|
|`void ClearAll()`|Performs full cleaning for the current logger instance: clears the current log and clears the stack of executing methods|
|`void LogMethodStep(string stepDescription)`|Adds to log the specified step (the description of executed operation) within the execution of the current method.|
|`void LogMethodStep(string? stepName, string stepDescription)`|Adds to log the specified step (the description of executed operation) within the execution of the current method|

To better understand how these additional methods can be used, let's look at some examples.

### API Examples

#### LogMethodStep()
You can add some additional logical steps in your logged methods in case these steps also need to be added to the final log.
For this purpose methods `void LogMethodStep(string stepDescription)` and `void LogMethodStep(string? stepName, string stepDescription)` are
used.

Let's consider the following example and improve a bit our `MyAmazingClass` example class:
```cs
using SimpleExecutionLogger;

public class MyAmazingClass {
	// Static logger instance for this class
	private static ExecutionLogger logger = new ExecutionLogger("My Amazing Logger");

	static MyAmazingClass() {
		logger.TabulationPrefix = "  ";
		logger.MethodStartedLogPrefix = " Started method ";
		logger.MethodEndedLogPrefix = " Exited from method ";
		logger.LoggerNameFormat = "(Logger: '{0}')";
		logger.StartAtString = " at ";
		logger.EndAtString = " at ";
	}
        
	// Class constructor
	public MyAmazingClass() {
		// ... some initialization code ...
	}

	public static void Main() {
		logger.StartMethod();

		Console.WriteLine("Hello, I'm in MyAmazingClass!");
		Console.WriteLine();

		// calling MyMethod1 with corresponding step logging
		logger.LogMethodStep("Calling 'MyMethod1'...");
		MyMethod1();

		logger.EndMethod();

		string log = logger.GetLog();

		Console.WriteLine("Collected logs:");
		Console.WriteLine(log);
	}

	private static void MyMethod1() {
		logger.StartMethod();

		logger.LogMethodStep("Entering the cycle with 'i' counter...");
		for (int i = 0; i < 10; i++) {
			logger.LogMethodStep(i.ToString(), "The value of i = " + i);
			Thread.Sleep(10);
		}

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
In this example we have added a call to `LogMethodStep` method in our `Main` method:
```cs
		// calling MyMethod1 with corresponding step logging
		logger.LogMethodStep("Calling 'MyMethod1'...");
		MyMethod1();
```
Next, we have added a simple `for` loop into `MyMethod1` and inside this loop we've added
the logging of step for every cycle, passing `i.ToString()` as a step name and `"The value of i = " + i` expression
as a step description:
```cs
	private static void MyMethod1() {
		logger.StartMethod();

		logger.LogMethodStep("Entering the cycle with 'i' counter...");
		for (int i = 0; i < 10; i++) {
			logger.LogMethodStep(i.ToString(), "The value of i = " + i);
			Thread.Sleep(10);
		}

		// now calling MyMethod2 from MyMethod1
		MyMethod2();

		logger.EndMethod();
	}
```

Now let's run the application and see what the final log of its execution will look like:
```
Hello, I'm in MyAmazingClass!

Collected logs:
(Logger: 'My Amazing Logger') Started method 'Main' at 09.12.2023 12:28:34
  (Logger: 'My Amazing Logger') Method 'Main': Calling 'MyMethod1'... at 09.12.2023 12:28:34, elapsed from start: 15 ms, delta=0 ms
  (Logger: 'My Amazing Logger') Started method 'MyMethod1' at 09.12.2023 12:28:34
    (Logger: 'My Amazing Logger') Method 'MyMethod1': Entering the cycle with 'i' counter... at 09.12.2023 12:28:34, elapsed from start: 0 ms, delta=0 ms
    (Logger: 'My Amazing Logger') Method 'MyMethod1': [Step: 0]: The value of i = 0 at 09.12.2023 12:28:34, elapsed from start: 0 ms, delta=0 ms
    (Logger: 'My Amazing Logger') Method 'MyMethod1': [Step: 1]: The value of i = 1 at 09.12.2023 12:28:35, elapsed from start: 24 ms, delta=24 ms
    (Logger: 'My Amazing Logger') Method 'MyMethod1': [Step: 2]: The value of i = 2 at 09.12.2023 12:28:35, elapsed from start: 39 ms, delta=15 ms
    (Logger: 'My Amazing Logger') Method 'MyMethod1': [Step: 3]: The value of i = 3 at 09.12.2023 12:28:35, elapsed from start: 54 ms, delta=15 ms
    (Logger: 'My Amazing Logger') Method 'MyMethod1': [Step: 4]: The value of i = 4 at 09.12.2023 12:28:35, elapsed from start: 70 ms, delta=16 ms
    (Logger: 'My Amazing Logger') Method 'MyMethod1': [Step: 5]: The value of i = 5 at 09.12.2023 12:28:35, elapsed from start: 86 ms, delta=16 ms
    (Logger: 'My Amazing Logger') Method 'MyMethod1': [Step: 6]: The value of i = 6 at 09.12.2023 12:28:35, elapsed from start: 102 ms, delta=16 ms
    (Logger: 'My Amazing Logger') Method 'MyMethod1': [Step: 7]: The value of i = 7 at 09.12.2023 12:28:35, elapsed from start: 118 ms, delta=16 ms
    (Logger: 'My Amazing Logger') Method 'MyMethod1': [Step: 8]: The value of i = 8 at 09.12.2023 12:28:35, elapsed from start: 133 ms, delta=15 ms
    (Logger: 'My Amazing Logger') Method 'MyMethod1': [Step: 9]: The value of i = 9 at 09.12.2023 12:28:35, elapsed from start: 148 ms, delta=15 ms
    (Logger: 'My Amazing Logger') Started method 'MyMethod2' at 09.12.2023 12:28:35
      (Logger: 'My Amazing Logger') Started method 'OtherMethodN' at 09.12.2023 12:28:35
      (Logger: 'My Amazing Logger') Exited from method 'OtherMethodN' at 09.12.2023 12:28:35, duration: 0 ms
    (Logger: 'My Amazing Logger') Exited from method 'MyMethod2' at 09.12.2023 12:28:35, duration: 0 ms
  (Logger: 'My Amazing Logger') Exited from method 'MyMethod1' at 09.12.2023 12:28:35, duration: 164 ms
(Logger: 'My Amazing Logger') Exited from method 'Main' at 09.12.2023 12:28:35, duration: 185 ms
```

Now note that thanks to the fact that we have added logging of individual steps inside methods, in 
the log we have information about each such step, as well as an indication of the duration of this step, in milliseconds.

#### ClearLog()

`ExecutionLogger` class has internal string buffer to hold the full log. In case you think that this internal buffer is
overloaded, you can reset this buffer by calling `ClearLog` method. It will clear this internal buffer, but note
that you'll not be able to access the collected records in the log after calling this method:
```cs
	// ... some code where we have already printed the collected log 
	// and don't require anymore the internal buffer for log ...
	logger.ClearLog();
```
#### ClearLoggedMethodsStack()

This method is used to clear the current internal indentation level for methods stack (i.e. reset it to 0 value)
and clear the internal stack holding the information about the sequence of methods calls (methods stack).
> [!NOTE]
> Please note that `ClearLoggedMethodsStack` method _does not clear_ the internal string buffer which
> stores the full log
```cs
	// reset the internal methods stack for logger instance
	// and indentation level for methods to 0 value
	logger.ClearLoggedMethodsStack();
```

#### ClearAll()

Method `ClearAll` just calls to methods considered above: `ClearLog` and `ClearLoggedMethodsStack`,
so call it when you want to clear both the internal string buffer and methods stack:
```cs
	// we're calling ClearAll() at the moment when we want to fully reset the
	// state of the logger instance:
	logger.ClearAll();
```

## Contacts

:envelope: If you liked the library and have suggestions for improving functionality, please send your feedback and proposals 
to [allineed.ru@gmail.com](mailto:allineed@gmail.com)

You can also contact me on Telegram: [https://t.me/maxdamascus](https://t.me/maxdamascus)

## License

[MIT License](/LICENSE.txt)

