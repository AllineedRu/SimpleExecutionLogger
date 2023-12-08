/*
Copyright 2023 Maksim Abramkin

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace SimpleExecutionLogger {
    /// <summary>
    /// [RU] Логгер исполнения методов, позволяющий хранить стек вызовов и время исполнения методов с возможностью получения текста лога.<br/>
    /// [EN] Execution logger for method, allows to store the stack of method calls and execution time of methods and access the log text.
    /// </summary>
    public class ExecutionLogger {
        /// <summary>
        /// [RU] Название текущего экземпляра логгера<br/>
        /// [EN] The name of the current logger instance
        /// </summary>
        private readonly string loggerName;

        /// <summary>
        /// [RU] Уровень вложенности для текущего исполняемого метода<br/>
        /// [EN] The nesting level for currently executed method
        /// </summary>
        private int methodLevel;

        /// <summary>
        /// [RU] Стек вызовов исполняемых методов<br/>
        /// [EN] The stack for executing methods
        /// </summary>
        private readonly Stack<ExecutionInfo> executionStack = new Stack<ExecutionInfo>();

        /// <summary>
        /// [RU] Лог с записями об исполнении методов<br/>
        /// [EN] The log holding the records about methods execution
        /// </summary>
        private readonly StringBuilder executionInfoBuilder = new StringBuilder();

        /// <summary>
        /// [RU] Название текущего вызывающего метода<br/>
        /// [EN] The name of the current caller method
        /// </summary>
        private string? currentCallerMethodName;

        /// <summary>
        /// [RU] Название данного экземпляра логгера<br/>
        /// [EN] The name of the current logger instance
        /// </summary>
        public string LoggerName {
            get {
                return loggerName;
            }
        }

        /// <summary>
        /// [RU] Уровень отступа для текущего исполняемого метода<br/>
        /// [EN] Tabulation level for currently executing method
        /// </summary>
        public int MethodLevel {
            get {
                return methodLevel;
            }
        }

        /// <summary>
        /// [RU] Строка, задающая символ табуляции или пользовательская строка, используемая для отступов для вложенных методов<br/>
        /// [EN] String, specifying a tab symbo or custom string used for nested methods indentation.
        /// </summary>
        public string TabulationPrefix { get; set; } = "\t";

        /// <summary>
        /// [RU] Префикс, добавляемый к логу при входе в метод (начале исполнения метода)<br/>
        /// [EN] Prefix added to the log when entering a method (at the beginning of the method execution) 
        /// </summary>
        public string MethodStartedLogPrefix { get; set; } = " >> Method ";

        /// <summary>
        /// [RU] Префикс, добавляемый к логу при логировании шага внутри исполняемого метода.<br/>
        /// [EN] Prefix added to the log when logging the specific step within executing method.
        /// </summary>
        public string MethodStepLogPrefix { get; set; } = " Method ";

        /// <summary>
        /// [RU] Префикс, добавляемый к логу при выходе из метода (завершении исполнения метода, возврате из метода)<br/>
        /// [EN] Prefix added to the log when exiting from a method (at the end of the method execution, when returning from method)
        /// </summary>
        public string MethodEndedLogPrefix { get; set; } = " << Method ";

        /// <summary>
        /// [RU] Формат для вывода названия экземпляра логгера. Пользовательская строка, задающая формат, обязательно должна содержать единственный элемент форматирования {0}.<br/>
        /// [EN] Format for printing the name of the logger instance. Custom user string, specifying the format, must contain only single formatting element {0}.
        /// </summary>
        public string LoggerNameFormat { get; set; } = "[{0}] ";

        /// <summary>
        /// [RU] Разрешить вывод названия экземпляра логгера при логировании. Установите в false, чтобы не отображать название логгера.<br/>
        /// [EN] Enable printing the logger instance name during logging. Set to false in order to hide the logger name.
        /// </summary>
        public bool EnableLoggerName { get; set; } = true;

        /// <summary>
        /// [RU] Формат для вывода названия шага внутри метода. Пользовательская строка, задающая формат, обязательно должна содержать единственный элемент форматирования {0}.<br/>
        /// [EN] Format for printing the name of the method step. Custom user string, specifying the format, must contain only single formatting element {0}.
        /// </summary>
        public string StepNameFormat { get; set; } = "[Step: {0}]: ";

        /// <summary>
        /// [RU] Пользовательская строка, задающая фразу начала логирования метода.<br/>
        /// [EN] Custom user string, specifying the phrase for start of method logging.
        /// </summary>
        public string StartAtString { get; set; } = " start at ";

        /// <summary>
        /// [RU] Пользовательская строка, задающая фразу окончания логирования метода.<br/>
        /// [EN] Custom user string, specifying the phrase for end of method logging. 
        /// </summary>
        public string EndAtString { get; set; } = " end at ";

        /// <summary>
        /// [RU] Пользовательская строка, задающая фразу для начала выполнения шага внутри метода.<br/>
        /// [EN] Custom user string, specifying the phrase for start of method step execution.
        /// </summary>
        public string AtString { get; set; } = " at ";

        /// <summary>
        /// [RU] Строка, используемая для обрамления названия логируемого метода.<br/>
        /// [EN] String used for wrapping the name of the logged method.
        /// </summary>
        public string MethodNameQuoteString { get; set; } = "'";

        /// <summary>
        /// [RU] Строка-разделитель между именем логируемого метода и названием логируемого шага внутри этого метода.<br/>
        /// [EN] Separator string between the name of the logged method and the name of the logged step inside this method.
        /// </summary>
        public string StepNameSeparator { get; set; } = ": ";

        /// <summary>
        /// [RU] Строка, задающая формат для отображения длительности выполнения логируемого метода, обязательно должна содержать единственный элемент форматирования {0}.<br/>
        /// [EN] String specifying the format of showing the execution duration of the logged method, must contain only single formatting element {0}.
        /// </summary>
        public string MethodExecutionDurationFormat { get; set; } = ", duration: {0} ms";

        /// <summary>
        /// [RU] Строка, задающая формат для отображения времени, прошедшего от начала выполнения метода до шага внутри этого метода и дельту времени между этим шагом и предыдущим шагом, если он имел место быть. Обязательно должна содержать два элемента форматирования {0} и {1}.<br/>
        /// [EN] A string that specifies the format for displaying the time elapsed from the start of a method's execution to the step within that method and the time delta between that step and the previous step, if any. Must contain two formatting elements {0} and {1}.
        /// </summary>
        public string MethodStepDurationFormat { get; set; } = ", elapsed from start: {0} ms, delta={1} ms";

        /// <summary>
        /// [RU] Конструктор, создаёт новый экземпляр логгера с заданным именем.<br/>
        /// [EN] Constructor, creates a new instance of logger with the specified name.
        /// </summary>
        /// <param name="loggerName">[RU] Имя для нового экземпляра логгера;<br/>[EN] The name for newly created logger</param>
        public ExecutionLogger(string loggerName) {
            this.loggerName = loggerName;
        }

        /// <summary>
        /// [RU] Начинает логирование исполнения для текущего метода. Предполагается, что данный метод вызывается в начале метода и является первой (или одной из первых) инструкций в логируемом методе.<br/>
        /// [EN] Starts logging of execution of the current method. It is assumed that this method is called at the very beginning of the method and is a first statement (or one of the first statements) in the logged method.
        /// </summary>
        public void StartMethod() {
            StackTrace stackTrace = new StackTrace();
            if (stackTrace.FrameCount < 2) {
                return;
            }

            StackFrame? stackFrame = stackTrace.GetFrame(1);
            if (stackFrame == null) {
                return;
            }

            MethodBase? methodBase = stackFrame.GetMethod();
            if (methodBase == null) {
                return;
            }

            currentCallerMethodName = methodBase.Name;

            ExecutionInfo executionInfo = new ExecutionInfo(currentCallerMethodName);
            executionInfo.Start();

            AddMethodStartToLog(executionInfo);

            executionStack.Push(executionInfo);

            methodLevel++;
        }

        /// <summary>
        /// [RU] Добавляет время старта исполнения метода в лог.<br/>
        /// [EN] Adds to the log the start time of method execution.
        /// </summary>
        /// <param name="executionInfo">[RU] Экземпляр ExecutionInfo, содержащий информацию об исполнении<br/>[EN] ExecutionInfo instance containing information about execution</param>
        private void AddMethodStartToLog(ExecutionInfo executionInfo) {
            AddTabsToLog();
            executionInfoBuilder
                .Append(EnableLoggerName ? string.Format(LoggerNameFormat, LoggerName) : string.Empty)
                .Append(MethodStartedLogPrefix)
                .Append(MethodNameQuoteString)
                .Append(executionInfo.MethodName)
                .Append(MethodNameQuoteString)
                .Append(StartAtString)
                .AppendLine(executionInfo.StartTimestamp.ToString());
        }

        /// <summary>
        /// [RU] Добавляет к логу информацию о завершении метода.<br/>
        /// [EN] Adds to the log the information about the end of method execution.
        /// </summary>
        /// <param name="executionInfo">[RU] Экземпляр ExecutionInfo, содержащий информацию об исполнении<br/>[EN] ExecutionInfo instance containing information about execution</param>
        private void AddMethodEndToLog(ExecutionInfo executionInfo) {
            AddTabsToLog();
            executionInfoBuilder
                .Append(EnableLoggerName ? string.Format(LoggerNameFormat, LoggerName) : string.Empty)
                .Append(MethodEndedLogPrefix)
                .Append(MethodNameQuoteString)
                .Append(executionInfo.MethodName)
                .Append(MethodNameQuoteString)
                .Append(EndAtString)
                .Append(executionInfo.EndTimestamp.ToString())
                .AppendLine(string.Format(MethodExecutionDurationFormat, executionInfo.ElapsedMilliseconds));
        }

        /// <summary>
        /// [RU] Добавляет к логу отступы (табуляцию) для хранения уровней вложенности вызовов для методов.<br/>
        /// [EN] Adds to log the indentation (tabulation) for storing the nested levels for method calls.
        /// </summary>
        private void AddTabsToLog() {
            for (int i = 0; i < methodLevel; i++) {
                executionInfoBuilder.Append(TabulationPrefix);
            }
        }

        /// <summary>
        /// [RU] Добавляет к логу определённый шаг (описание исполняемой операции) в рамках выполнения текущего метода.<br/>
        /// [EN] Adds to log the specified step (the description of executed operation) within the execution of the current method.
        /// </summary>
        /// <param name="stepName">[RU] название шага внутри метода;<br/>[EN] the name of the step within the method</param>
        /// <param name="stepDescription">[RU] описание шага внутри метода;<br/>[EN] the description of the step within the method</param>
        public void LogMethodStep(string? stepName, string stepDescription) {
            ExecutionInfo currentExecutionInfo = executionStack.Peek();
            currentExecutionInfo.AddStep(stepDescription);

            AddTabsToLog();

            executionInfoBuilder
                .Append(EnableLoggerName ? string.Format(LoggerNameFormat, LoggerName) : string.Empty)
                .Append(MethodStepLogPrefix)
                .Append(MethodNameQuoteString)
                .Append(currentCallerMethodName)
                .Append(MethodNameQuoteString)
                .Append(StepNameSeparator)
                .Append(stepName != null ? string.Format(StepNameFormat, stepName) : string.Empty)
                .Append(stepDescription)
                .Append(AtString)
                .Append(DateTime.Now.ToString())
                .AppendLine(string.Format(MethodStepDurationFormat, currentExecutionInfo.ElapsedMilliseconds, currentExecutionInfo.GetDeltaWithPreviousStep()));
        }

        /// <summary>
        /// [RU] Добавляет к логу определённый шаг (описание исполняемой операции) в рамках выполнения текущего метода.<br/>
        /// [EN] Adds to log the specified step (the description of executed operation) within the execution of the current method.
        /// </summary>
        /// <param name="stepDescription">[RU] описание шага внутри метода;<br/>[EN] the description of the step within the method</param>
        public void LogMethodStep(string stepDescription) {
            LogMethodStep(null, stepDescription);
        }

        /// <summary>
        /// [RU] Производит полную очистку текущего экземпляра логгера: очищает текущий лог, а также очищает стек вызовов для исполняемых методов.<br/>
        /// [EN] Performs full cleaning for the current logger instance: clears the current log and clears the stack of executing methods.
        /// </summary>
        public void ClearAll() {
            ClearLog();
            ClearLoggedMethodsStack();
        }

        /// <summary>
        /// [RU] Очищает внутренний лог для текущего экземпляра логгера.<br/>
        /// [EN] Clears the internal log for the current logger instance.
        /// </summary>
        public void ClearLog() {
            executionInfoBuilder.Clear();
        }

        /// <summary>
        /// [RU] Возвращает строку, содержащую лог: записи с информацией об исполнении логируемых методов (длительность выполнения методов и шагов внутри методов).<br/>
        /// [EN] Returns string, containing the log: records with the execution information about logged methods (duration of methods execution and duration of steps within methods).
        /// </summary>
        /// <returns></returns>
        public string GetLog() {
            return executionInfoBuilder.ToString();
        }

        /// <summary>
        /// [RU] Очищает стек, содержащий логируемые методы, а также сбрасывает текущий уровень для логируемых методов в 0 (уровень отступа/табуляции).<br/>
        /// [EN] Clears the stack, containing logged methods. Also resets the current method level to 0 value (indentation/tabulation level).
        /// </summary>
        public void ClearLoggedMethodsStack() {
            executionStack.Clear();
            methodLevel = 0;
        }

        /// <summary>
        /// [RU] Завершает логирование текущего метода. Должен вызываться в конце метода, перед возвратом из метода.<br/>
        /// [EN] Ends logging for current method. Must be called at the end of the logged method, before returning from method.
        /// </summary>
        public void EndMethod() {
            if (executionStack.Count > 0) {
                ExecutionInfo executionInfo = executionStack.Pop();
                executionInfo.Stop();
                methodLevel--;

                AddMethodEndToLog(executionInfo);
            }            
        }
    }
}