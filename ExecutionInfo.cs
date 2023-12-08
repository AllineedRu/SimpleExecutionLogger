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

namespace SimpleExecutionLogger {
    /// <summary>
    /// [RU] Содержит дополнительную информацию об исполнении метода: имя метода, экземпляр класса Stopwatch, отслеживающий длительность исполнения метода, дату и время начала и завершения исполнения метода, а также список шагов, входящих в метод.<br/>
    /// [EN] Contains additional information about method execution: the name of the method, the instance of Stopwatch class tracking the duration of method execution, date and time of start/end of method execution and the list of steps included in the method.
    /// </summary>
    public class ExecutionInfo {
        /// <summary>
        /// [RU] Экземпляр класса Stopwatch, хранящий количество миллисекунд, прошедших с момента начала исполнения метода<br/>
        /// [EN] Instance of Stopwatch class, holding the amount of milliseconds, elapsed from the beginning of the method execution
        /// </summary>
        private readonly Stopwatch stopwatch;

        /// <summary>
        /// [RU] Название исполняемого метода<br/>
        /// [EN] The name of the executing method
        /// </summary>
        private readonly string methodName;

        /// <summary>
        /// [RU] Дата и время начала исполнения метода<br/>
        /// [EN] The date and time when method started its execution
        /// </summary>
        private DateTime startTimestamp;

        /// <summary>
        /// [RU] Дата и время завершения исполнения метода<br/>
        /// [EN] The date and time when method finished its execution
        /// </summary>
        private DateTime endTimestamp;

        /// <summary>
        /// [RU] Список шагов, входящих в исполнение метода<br/>
        /// [EN] The list of steps included in the method execution
        /// </summary>
        private readonly List<ExecutionStep> steps = new List<ExecutionStep>();

        /// <summary>
        /// [RU] Дата и время начала исполнения метода<br/>
        /// [EN] The date and time when method started its execution
        /// </summary>
        public DateTime StartTimestamp {
            get {
                return startTimestamp;
            }
        }

        /// <summary>
        /// [RU] Дата и время завершения исполнения метода<br/>
        /// [EN] The date and time when method finished its execution
        /// </summary>
        public DateTime EndTimestamp {
            get {
                return endTimestamp;
            }
        }

        /// <summary>
        /// [RU] Название исполняемого метода<br/>
        /// [EN] The name of the executing method
        /// </summary>
        public string MethodName {
            get {
                return methodName;
            }
        }

        /// <summary>
        /// [RU] Возвращает количество миллисекунд, прошедших с момента начала исполнения метода
        /// [EN] Returns the amount of milliseconds elapsed from the start of method execution
        /// </summary>
        public long ElapsedMilliseconds {
            get {
                return stopwatch.ElapsedMilliseconds;
            }
        }

        /// <summary>
        /// [RU] Получает дельту между текущим временем, прошедшим с момента старта метода и зафиксированным временем для предыдущего шага в текущем методе.<br/>
        /// [EN] Gets the delta between the current timstamp, elapsed from the moment of method start and fixed time of the previous step within this method.
        /// </summary>
        /// <param name="elapsedMillisecondsForCurrentStep">[RU] время (в миллисекундах), прошедшее до момента текущего шага с момента старта метода;<br/>[EN] the time (in milliseconds), that elapsed to the moment of the current step since the method start time</param>
        /// <returns></returns>
        private long GetDeltaWithPreviousStep(long elapsedMillisecondsForCurrentStep) {
            if (steps.Count > 0) {
                long lastStepElapsedMilliseconds = steps[steps.Count - 1].ElapsedMilliseconds;
                return elapsedMillisecondsForCurrentStep - lastStepElapsedMilliseconds;
            }
            return 0;
        }

        /// <summary>
        /// [RU] Добавляет новый шаг для включения его в лог с заданным пользовательским описанием действий этого шага.<br/>
        /// [EN] Adds a new step for inclusion into the log with specified custom description of actions performed in this step.
        /// </summary>
        /// <param name="description">[RU] описание шага;<br/>[EN] the description of the step</param>
        public void AddStep(string description) {
            long elapsedMillisecondsForCurrentStep = stopwatch.ElapsedMilliseconds;
            steps.Add(new ExecutionStep(description, elapsedMillisecondsForCurrentStep, GetDeltaWithPreviousStep(elapsedMillisecondsForCurrentStep)));
        }

        /// <summary>
        /// [RU] Добавляет новый шаг для включения его в лог с заданным пользовательским именем шага и описанием действий этого шага.<br/>
        /// [EN] Adds a new step for inclusion into the log with specified custom step name and description of actions performed in this step. 
        /// </summary>
        /// <param name="name">[RU] имя шага;<br/>[EN] the name of the step</param>
        /// <param name="description">[RU] описание шага;<br/>[EN] the description of the step</param>
        public void AddStep(string name, string description) {
            long elapsedMillisecondsForCurrentStep = stopwatch.ElapsedMilliseconds;
            steps.Add(new ExecutionStep(name, description, elapsedMillisecondsForCurrentStep, GetDeltaWithPreviousStep(elapsedMillisecondsForCurrentStep)));
        }

        /// <summary>
        /// [RU] Получает дельту, или разницу (в миллисекундах) между последним добавленным в метод шагом и предыдущим шагом.<br/>
        /// [EN] Gets the delta (in milliseconds) between the last added step into the method and the previous step.
        /// </summary>
        /// <returns>[RU] Значение разницы (в миллисекундах) между последним добавленным в метод шагом и предыдущим шагом;<br/>[EN] The delta value (in milliseconds) between the last added step into the method and the previous step</returns>
        public long GetDeltaWithPreviousStep() {
            if (steps.Count > 0) {
                return steps[steps.Count - 1].DeltaWithPreviousStep;
            }
            return 0;
        }

        /// <summary>
        /// [RU] Конструктор, создающий экземпляр класса с указанием имени исполняемого метода<br/>
        /// [EN] Constructor creating a new instance of the class with the specified name of executing method
        /// </summary>
        /// <param name="methodName">[RU] Имя исполняемого метода;<br/>[EN] The name of the executing method</param>
        public ExecutionInfo(string methodName) {
            stopwatch = new Stopwatch();
            this.methodName = methodName;
        }

        /// <summary>
        /// [RU] Начинает отсчёт времени исполнения для метода.<br/>
        /// [EN] Starts counting the time of execution for the method.
        /// </summary>
        public void Start() {
            startTimestamp = DateTime.Now;
            stopwatch.Start();
        }

        /// <summary>
        /// [RU] Завершает отсчёт времени исполнения для метода.<br/>
        /// [EN] Ends counting the time of execution for the method.
        /// </summary>
        public void Stop() {
            endTimestamp = DateTime.Now;
            stopwatch.Stop();            
        }
    }
}
