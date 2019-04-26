using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Threading;

namespace Manager
{
    public partial class ImportTaskWindow : Window
    {
        private static List<Task.TaskListItem> GlobalListItems;

        public delegate void OnCancel(Task task);
        public delegate void OnSelect(Task task);

        public event OnCancel OnCancelEvent;
        public event OnSelect OnSelectEvent;

        private Task Task;

        private Task.TaskListItem SelectedItem;

        private Thread TaskLoaderThread;

        public Task DialogTask
        {
            get
            {
                return Task;
            }

            set
            {
                Task = value;

                if (Task == null)
                    Task = new Task();
            }
        }

        public Task.TaskListItem DialogSelectedItem
        {
            get
            {
                return SelectedItem;
            }

            set
            {
                SelectedItem = value;

                if (TaskListBox != null)
                    TaskListBox.SelectedItem = SelectedItem;

                if (AcceptButton != null)
                    AcceptButton.IsEnabled = SelectedItem != null && SelectedItem.Task != null;
            }
        }

        public ImportTaskWindow() : this(null)
        {
        }

        public ImportTaskWindow(Task task)
        {
            //Initialize Window
            InitializeComponent();

            //Setup elements
            DialogTask = task;

            DialogSelectedItem = null;

            //Setup drag behavior
            ToolbarLayout.MouseDown += (object sender, MouseButtonEventArgs args) =>
            {
                DragMove();
            };

            //Setup close behavior
            CloseButton.Click += (object sender, RoutedEventArgs args) =>
            {
                if (OnDeny())
                    Close();
            };

            KeyDown += (object sender, KeyEventArgs args) =>
            {
                if (args.Key == Key.Return)
                {
                    args.Handled = true;

                    if (OnAccept())
                        Close();
                }

                if (args.Key == Key.Escape)
                {
                    if (OnDeny())
                        Close();
                }
            };

            IsVisibleChanged += (object sender, DependencyPropertyChangedEventArgs args) =>
            {
                if (!IsVisible)
                {
                    AbortUpdate();
                }
                else
                {
                    UpdateList();
                }
            };

            Closing += (object sender, CancelEventArgs args) =>
            {
                AbortUpdate();

                OnDeny();
            };

            //Setup events
            TaskListBox.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                try
                {
                    object selectedItemObject = TaskListBox.SelectedItem;

                    if (selectedItemObject != null)
                    {
                        DialogSelectedItem = selectedItemObject as Task.TaskListItem;
                    } else
                    {
                        DialogSelectedItem = null;
                    }
                } catch (Exception e)
                {
                }
            };

            TaskListBox.MouseDoubleClick += (object sender, MouseButtonEventArgs args) =>
            {
                if (SelectedItem != null && SelectedItem.Task != null && OnAccept())
                    Close();
            };

            AcceptButton.Click += (object sender, RoutedEventArgs args) =>
            {
                if (OnAccept())
                    Close();
            };

            DenyButton.Click += (object sender, RoutedEventArgs args) =>
            {
                if (OnDeny())
                    Close();
            };

            UpdateList();
        }

        protected bool OnAccept()
        {
            try
            {
                if (SelectedItem != null && SelectedItem.Task != null)
                {
                    Task SelectedTask = SelectedItem.Task;

                    Task newTask = new Task(this.Task);
                    SelectedTask.ApplyTo(newTask, true);

                    if (OnSelectEvent != null)
                        OnSelectEvent(newTask);
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        protected bool OnDeny()
        {
            try
            {
                if (SelectedItem != null)
                {
                    Task SelectedTask = SelectedItem.Task;

                    Task newTask = new Task(this.Task);
                    SelectedTask.ApplyTo(newTask, true);

                    if (OnCancelEvent != null)
                        OnCancelEvent(newTask);
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return true;
        }

        private void AbortUpdate()
        {
            //Hide loading progress bar
            if (LoadingProgressBar != null)
                LoadingProgressBar.Visibility = Visibility.Collapsed;

            //Interrupt thread
            try
            {
                if (TaskLoaderThread != null && TaskLoaderThread.IsAlive)
                    TaskLoaderThread.Abort();
            } catch (Exception e)
            {
            }
        }

        private void UpdateList()
        {
            AbortUpdate();

            if (TaskListBox == null) return;

            //Start loading
            if (GlobalListItems == null)
                GlobalListItems = new List<Task.TaskListItem>();

            List<Task.TaskListItem> items = new List<Task.TaskListItem>();

            try
            {
                if (TaskListBox.ItemsSource != null)
                    items.AddRange(TaskListBox.ItemsSource as List<Task.TaskListItem>);
            }
            catch (Exception e)
            {
            }

            if (GlobalListItems != null && (items.Count <= 0 || items.Count < GlobalListItems.Count))
            {
                lock (GlobalListItems)
                {
                    items.Clear();
                    items.AddRange(GlobalListItems);
                }
            }

            if (items.Count > 0)
            {
                try
                {
                    TaskListBox.ItemsSource = items;
                }
                catch (Exception e)
                {
                }
            } else
            {
                //Show loading progress bar
                if (LoadingProgressBar != null)
                    LoadingProgressBar.Visibility = Visibility.Visible;
            }

            //Start thread
            try
            {
                TaskLoaderThread = new Thread(() =>
                {
                    try
                    {
                        Thread currentThread = Thread.CurrentThread;
                        if (currentThread != null && !currentThread.IsAlive) return;
                    } catch (Exception e)
                    {
                    }

                    bool itemsChanged = false;

                    try
                    {
                        foreach (Process process in Process.GetProcesses())
                        {
                            try
                            {
                                Thread currentThread = Thread.CurrentThread;
                                if (currentThread != null && !currentThread.IsAlive) return;
                            }
                            catch (Exception e)
                            {
                            }

                            if (process == null) continue;

                            Task newTask = new Task(process);
                            if (!newTask.IsValid()) continue;

                            Task.TaskListItem newTaskItem = new Task.TaskListItem(newTask);
                            if (!items.Contains(newTaskItem))
                            {
                                items.Add(newTaskItem);
                                itemsChanged = true;
                            }
                        }
                    } catch (Exception e)
                    {
                    }

                    try
                    {
                        if (itemsChanged && TaskListBox != null && items != null)
                        {
                            SortItems(items);

                            if (GlobalListItems == null)
                                GlobalListItems = new List<Task.TaskListItem>();
                            lock (GlobalListItems)
                            {
                                GlobalListItems.Clear();
                                GlobalListItems.AddRange(items);
                            }

                            try
                            {
                                Thread currentThread = Thread.CurrentThread;
                                if (currentThread != null && !currentThread.IsAlive) return;
                            }
                            catch (Exception e)
                            {
                            }

                            TaskListBox.Dispatcher.Invoke(new Action(() =>
                            {
                                try
                                {
                                    Thread currentThread = Thread.CurrentThread;
                                    if (currentThread != null && !currentThread.IsAlive) return;
                                }
                                catch (Exception e)
                                {
                                }

                                try
                                {
                                    TaskListBox.ItemsSource = items;
                                }
                                catch (Exception ex)
                                {
                                }
                            }));
                        }
                    } catch (Exception e)
                    {
                    }

                    //Hide loading progress bar
                    try
                    {
                        if (LoadingProgressBar != null)
                        {
                            LoadingProgressBar.Dispatcher.Invoke(new Action(() =>
                            {
                                if (LoadingProgressBar != null)
                                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                            }));
                        }
                    } catch (Exception e)
                    {
                    }
                });
                TaskLoaderThread.Start();
            }
            catch (Exception e)
            {
            }
        }

        private void SortItems(List<Task.TaskListItem> list)
        {
            try
            {
                list.Sort((Task.TaskListItem item1, Task.TaskListItem item2) =>
                 {
                     if (item1 == null || item1.Task == null) return 1;
                     if (item2 == null || item2.Task == null) return -1;

                     string id1 = item1.Task.Id;
                     string id2 = item2.Task.Id;

                     if (id1 == null) return 1;
                     if (id2 == null) return -1;

                     return id1.CompareTo(id2);
                 });
            } catch (Exception e)
            {
            }
        }
    }
}
