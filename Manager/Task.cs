using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class Task
    {
        public string Id;
        public string Name;
        public string FilePath;

        public Task(Task src)
        {
            if (src != null)
                src.ApplyTo(this);
        }

        public Task()
        {
        }

        public Task(Process process)
        {
            UseProcess(process);
        }

        public void ApplyTo(Task src)
        {
            ApplyFieldsTo(src, false);
        }

        public void ApplyTo(Task src, bool keepId)
        {
            if (src == null) return;

            string srcId = src.Id;

            ApplyFieldsTo(src, false);

            if (keepId && srcId != null && srcId.Length > 0)
                src.Id = srcId;
        }

        public void ApplyFieldsTo(Task src, bool onlyMissingFields)
        {
            if (src == null) return;

            lock (src)
            {
                if (!onlyMissingFields || src.Id == null || src.Id.Length <= 0) src.Id = Id;
                if (!onlyMissingFields || src.Name == null || src.Name.Length <= 0) src.Name = Name;
                if (!onlyMissingFields || src.FilePath == null || src.FilePath.Length <= 0) src.FilePath = FilePath;
            }
        }

        public bool IsValid()
        {
            return 
                Id != null && 
                Name != null && 
                FilePath != null;
        }

        public void UseProcess(Process process)
        {
            UseProcess(process, true);
        }

        public void UseProcess(Process process, bool overrideId)
        {
            if (process == null) return;

            try
            {
                if (overrideId)
                    Id = process.ProcessName;

                Name = process.ProcessName;
            } catch (Exception e)
            {
            }

            try
            {
                string ProcessFilePath = process.MainModule.FileName;
                FilePath = Path.GetFullPath(ProcessFilePath);
            }
            catch (Exception e)
            {
            }
        }

        public bool Start()
        {
            try
            {
                if (FilePath != null && File.Exists(FilePath))
                {
                    Process process = new Process();

                    process.StartInfo.FileName = FilePath;

                    process.Start();

                    return true;
                }
            }
            catch (Exception e)
            {
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is Task task &&
                   (Id == task.Id) &&
                   (Name == task.Name) &&
                   (FilePath == task.FilePath);
        }

        public class TaskListItem
        {
            public Task Task;

            public string Id
            {
                get
                {
                    if (Task == null)
                        return default(string);
                    if (Task.Id == null)
                        return Config.EmptyItemPlaceholder;
                    return Task.Id;
                }
            }

            public string Name
            {
                get
                {
                    if (Task == null)
                        return default(string);
                    if (Task.Name == null)
                        return Config.EmptyItemPlaceholder;
                    return Task.Name;
                }
            }

            public string FilePath
            {
                get
                {
                    if (Task == null)
                        return default(string);
                    if (Task.FilePath == null)
                        return Config.EmptyItemPlaceholder;
                    return Task.FilePath;
                }
            }

            public TaskListItem(Task task)
            {
                if (task == null)
                    throw new ArgumentNullException("No task");
                this.Task = task;
            }

            public override bool Equals(object obj)
            {
                return obj is TaskListItem item &&
                       EqualityComparer<Task>.Default.Equals(Task, item.Task);
            }
        }
    }
}
