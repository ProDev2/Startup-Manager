using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class TaskStorage : Storage<Task>
    {
        private static TaskStorage Storage;

        public static TaskStorage Global
        {
            get
            {
                if (Storage == null)
                    Storage = new TaskStorage();

                return Storage;
            }
        }

        public TaskStorage() : base(Config.RelativeTaskStoragePath, true)
        {
        }

        //Static access methods
        public static Task GetTask(string key, Task defValue)
        {
            Task value = GetTask(key);
            if (value != null)
                return value;
            else
            {
                SetTask(key, defValue);

                return defValue;
            }
        }

        public static Task GetTask(string key)
        {
            TaskStorage storage = TaskStorage.Global;
            if (storage == null) return default(Task);

            return storage.Get(key);
        }

        public static void SetTask(string key, Task value)
        {
            TaskStorage storage = TaskStorage.Global;
            if (storage == null) return;

            storage.Put(key, value);
        }
    }
}
