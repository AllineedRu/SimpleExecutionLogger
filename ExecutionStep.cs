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
namespace SimpleExecutionLogger {
    /// <summary>
    /// [RU] Описывает логический шаг (или действие), выполняемое в логируемом методе.<br/>
    /// [EN] Describes the logical step (or action) performed within the logged method
    /// </summary>
    public class ExecutionStep {
        /// <summary>
        /// [RU] Название логического шага
        /// [EN] The name of the logical step
        /// </summary>
        private readonly string? name;

        /// <summary>
        /// [RU] Описание логического шага
        /// [EN] The description of the logical step
        /// </summary>
        private readonly string description;

        /// <summary>
        /// [RU] Значение времени (в миллисекундах), прошедшего между текущим шагом и временем начала исполнения логируемого метода.<br/>
        /// [EN] The value of time (in milliseconds) elapsed between the current step and the execution start time of the logged method.
        /// </summary>
        private readonly long elapsedMilliseconds;

        /// <summary>
        /// [RU] Значение времени (в миллисекундах), прошедшего между текущим шагом и предыдущим шагом в пределах одного логируемого метода.<br/>
        /// [EN] The value of time (in milliseconds) elapsed between the current step and the previous step within a single logged method.
        /// </summary>
        private readonly long deltaWithPreviousStep;

        /// <summary>
        /// [RU] Название логического шага
        /// [EN] The name of the logical step
        /// </summary>
        public string? Name {
            get {
                return name;
            }
        }

        /// <summary>
        /// [RU] Описание логического шага
        /// [EN] The description of the logical step
        /// </summary>
        public string Description {
            get {
                return description;
            }
        }

        /// <summary>
        /// [RU] Значение времени (в миллисекундах), прошедшего между текущим шагом и предыдущим шагом в пределах одного логируемого метода.<br/>
        /// [EN] The value of time (in milliseconds) elapsed between the current step and the previous step within a single logged method.
        /// </summary>
        public long DeltaWithPreviousStep {
            get {
                return deltaWithPreviousStep;
            }
        }

        /// <summary>
        /// [RU] Значение времени (в миллисекундах), прошедшего между текущим шагом и временем начала исполнения логируемого метода.<br/>
        /// [EN] The value of time (in milliseconds) elapsed between the current step and the execution start time of the logged method.
        /// </summary>
        public long ElapsedMilliseconds {
            get {
                return elapsedMilliseconds;
            }
        }

        /// <summary>
        /// [RU] Конструктор, создает новый экземпляр шага с заданными параметрами.<br/>
        /// [EN] Constructor, creates a new instance of step with specified parameters.
        /// </summary>
        /// <param name="description">[RU] описание шага;<br/>[EN] the step description</param>
        /// <param name="elapsedMilliseconds">[RU] значение времени (в миллисекундах), прошедшего между текущим шагом и временем начала исполнения логируемого метода;<br/>[EN] The value of time (in milliseconds) elapsed between the current step and the execution start time of the logged method</param>
        /// <param name="deltaWithPreviousStep">[RU] значение времени (в миллисекундах), прошедшего между текущим шагом и предыдущим шагом в пределах одного логируемого метода;<br/>The value of time (in milliseconds) elapsed between the current step and the previous step within a single logged method</param>
        public ExecutionStep(string description, long elapsedMilliseconds, long deltaWithPreviousStep) {
            this.description = description;
            this.elapsedMilliseconds = elapsedMilliseconds;
            this.deltaWithPreviousStep = deltaWithPreviousStep;
        }

        /// <summary>
        /// [RU] Конструктор, создает новый экземпляр шага с заданными параметрами.<br/>
        /// [EN] Constructor, creates a new instance of step with specified parameters.
        /// </summary>
        /// <param name="name">[RU] название шага;<br/>[EN] the name of the step</param>
        /// <param name="description">[RU] описание шага;<br/>[EN] the step description</param>
        /// <param name="elapsedMilliseconds">[RU] значение времени (в миллисекундах), прошедшего между текущим шагом и временем начала исполнения логируемого метода;<br/>[EN] The value of time (in milliseconds) elapsed between the current step and the execution start time of the logged method</param>
        /// <param name="deltaWithPreviousStep">[RU] значение времени (в миллисекундах), прошедшего между текущим шагом и предыдущим шагом в пределах одного логируемого метода;<br/>The value of time (in milliseconds) elapsed between the current step and the previous step within a single logged method</param>
        public ExecutionStep(string name, string description, long elapsedMilliseconds, long deltaWithPreviousStep) : this(description, elapsedMilliseconds, deltaWithPreviousStep) {
            this.name = name;
        }
    }
}
