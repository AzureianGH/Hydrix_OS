using Cosmos.Core;
using Cosmos.System;
using System;
using static Cosmos.HAL.PIT;
using Console = System.Console;

namespace HydrixOS.Core.Threading
{
    public class Scheduler
    {
        private static Task[] tasks;
        private static int currentTaskIndex;
        private static int taskCount;

        public static void Initialize(int maxTasks)
        {
            tasks = new Task[maxTasks];
            currentTaskIndex = -1;
            taskCount = 0;
        }

        public static void Start()
        {
            // Start timer interrupt to trigger context switch
            PITTimer timer = new PITTimer(TimerInterruptHandler, 10000000, true); // Set timer interrupt to occur every 10 milliseconds

            // Loop indefinitely
            while (true)
            {
                CPU.Halt(); // Halt CPU until next interrupt
            }
        }

        public static void CreateTask(Action taskAction)
        {
            if (taskCount < tasks.Length)
            {
                tasks[taskCount++] = new Task(taskAction);
            }
            else
            {
                Console.WriteLine("Error: Maximum number of tasks reached.");
            }
        }

        // Timer interrupt handler
        public static void TimerInterruptHandler()
        {
            // Find next ready task
            do
            {
                currentTaskIndex = (currentTaskIndex + 1) % taskCount;
            }
            while (tasks[currentTaskIndex].State != TaskState.Ready);

            // Switch to next task
            var nextTask = tasks[currentTaskIndex];
            nextTask.State = TaskState.Running;
            nextTask.Execute();
        }
    }

    public class Task
    {
        private readonly Action taskAction;

        public TaskState State { get; set; }

        public Task(Action taskAction)
        {
            this.taskAction = taskAction;
            State = TaskState.Ready;
        }

        public void Execute()
        {
            State = TaskState.Running;
            taskAction(); // Execute the task action
            State = TaskState.Finished;
        }
    }

    public enum TaskState
    {
        Ready,
        Running,
        Finished
    }
}
