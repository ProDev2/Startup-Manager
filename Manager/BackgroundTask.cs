using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class BackgroundTask : BackgroundWorker
    {
        public BackgroundTask() : base(ConfigStorage.GetUpdateInterval())
        {
        }

        protected override void OnTick()
        {
            Process thisProcess = Process.GetCurrentProcess();
            Task thisTask = new Task(thisProcess);

            TaskStorage storage = TaskStorage.Global;

            ArrayList tasks = new ArrayList();
            foreach (KeyValuePair<string, Task> taskEntry in storage.StorageMap)
            {
                Task task = taskEntry.Value;
                if (task == null || !task.IsValid()) continue;

                tasks.Add(task);
            }

            Dictionary<Task, Process> processes = GetProcessesByTasks(tasks);

            ArrayList handledTasks = new ArrayList();
            foreach (Task task in tasks)
            {
                if (task == null || thisTask.Equals(task)) continue;
                if (!task.IsValid()) continue;

                if (task.Name == null) continue;

                if (handledTasks.Contains(task.Name)) continue;
                handledTasks.Add(task.Name);

                processes.TryGetValue(task, out Process process);
                if (process == null)
                {
                    task.Start();
                }
            }
        }

        public Dictionary<Task, Process> GetProcessesByTasks(ArrayList tasks)
        {
            if (tasks == null) return null;

            Dictionary<Task, Process> processes = new Dictionary<Task, Process>();

            foreach (Process process in Process.GetProcesses())
            {
                string processName = process.ProcessName;
                if (processName == null) continue;

                foreach (Task task in tasks)
                {
                    string taskName = task.Name;
                    if (taskName == null) continue;

                    if (taskName.ToLower().Equals(processName.ToLower()))
                    {
                        if (processes.ContainsKey(task))
                        {
                            processes[task] = process;
                        }
                        else
                        {
                            processes.Add(task, process);
                        }
                    }
                }
            }

            return processes;
        }

        public Process GetProcessByName(string name)
        {
            if (name == null) return null;

            foreach (Process process in Process.GetProcesses())
            {
                string processName = process.ProcessName;
                if (processName == null) continue;

                if (name.ToLower().Equals(processName.ToLower()))
                {
                    return process;
                }
            }

            return null;
        }
    }
}
