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

namespace Manager
{
    public partial class ManagerWindow : Window
    {
        private Window DialogWindow;

        private void SetupControls()
        {
            //Setup elements
            TitleLabel.Content = Title;

            //Setup drag behavior
            ToolbarLayout.MouseDown += (object sender, MouseButtonEventArgs args) =>
            {
                DragMove();
            };

            //Setup close behavior
            CloseButton.Click += (object sender, RoutedEventArgs args) =>
            {
                WindowState = WindowState.Minimized;
            };

            KeyDown += (object sender, KeyEventArgs args) =>
            {
                if (args.Key == Key.Escape)
                {
                    WindowState = WindowState.Minimized;
                }
            };

            IsVisibleChanged += (object sender, DependencyPropertyChangedEventArgs args) =>
            {
                if (!IsVisible)
                {
                    try
                    {
                        if (DialogWindow != null && DialogWindow.IsVisible)
                        {
                            DialogWindow.Close();
                            DialogWindow = null;
                        }
                    }
                    catch (Exception e)
                    {
                    }
                } else
                {
                    UpdateList();
                }
            };

            //Setup events
            RunButton.Click += (object sender, RoutedEventArgs args) =>
            {
                try
                {
                    if (BackgroundTask != null)
                        BackgroundTask.Run();
                } catch (Exception e)
                {
                }
            };

            AddTaskButton.Click += (object sender, RoutedEventArgs args) =>
            {
                try
                {
                    if (DialogWindow != null && DialogWindow.IsVisible)
                    {
                        DialogWindow.Close();
                        DialogWindow = null;
                    }

                    InputDialogWindow dialogWindow = new InputDialogWindow("Enter task name");
                    this.DialogWindow = dialogWindow;

                    dialogWindow.Owner = this;
                    dialogWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                    dialogWindow.OnMessageEvent += (string message) =>
                    {
                        if (message == null || message.Length < 0) return;

                        if (TaskStorage == null) return;
                        lock (TaskStorage)
                        {
                            if (TaskStorage.StorageMap.ContainsKey(message)) return;

                            Task task = new Task();
                            task.Id = message;

                            //Import task data
                            ImportTaskWindow importTaskDialog = new ImportTaskWindow(task);
                            importTaskDialog.Owner = DialogWindow != null ? DialogWindow : this;
                            importTaskDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                            importTaskDialog.OnSelectEvent += (Task newTask) =>
                            {
                                if (newTask != null)
                                    newTask.ApplyFieldsTo(task, true);
                            };

                            importTaskDialog.ShowDialog();

                            //Set new task
                            if (task != null && task.Id != null && task.Id.Length > 0)
                                TaskStorage.Put(task.Id, task);

                            UpdateList();
                        }
                    };

                    dialogWindow.ShowDialog();
                } catch (Exception e)
                {
                }
            };

            UpdateList();
        }

        private void TaskListBox_ImportTaskButton_OnClick(object sender, RoutedEventArgs args)
        {
            try
            {
                Task task = ((sender as FrameworkElement).DataContext as Task.TaskListItem).Task;
                if (task == null) return;

                if (DialogWindow != null && DialogWindow.IsVisible)
                {
                    DialogWindow.Close();
                    DialogWindow = null;
                }

                ImportTaskWindow dialogWindow = new ImportTaskWindow(task);
                this.DialogWindow = dialogWindow;

                dialogWindow.Owner = this;
                dialogWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                dialogWindow.OnSelectEvent += (Task newTask) =>
                {
                    if (newTask == null || newTask.Id == null || newTask.Id.Length <= 0) return;

                    if (TaskStorage == null) return;
                    lock (TaskStorage)
                    {
                        TaskStorage.RemoveValue(task);

                        TaskStorage.Put(newTask.Id, newTask);

                        UpdateList();
                    }
                };

                dialogWindow.ShowDialog();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void TaskListBox_RemoveTaskButton_OnClick(object sender, RoutedEventArgs args)
        {
            try
            {
                Task task = ((sender as FrameworkElement).DataContext as Task.TaskListItem).Task;
                if (task == null) return;

                if (TaskStorage != null)
                    TaskStorage.RemoveValue(task);

                UpdateList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void TaskListBox_Text_Id_OnClick(object sender, MouseButtonEventArgs args)
        {
            try
            {
                Task task = ((sender as FrameworkElement).DataContext as Task.TaskListItem).Task;
                if (task == null) return;

                if (DialogWindow != null && DialogWindow.IsVisible)
                {
                    DialogWindow.Close();
                    DialogWindow = null;
                }

                InputDialogWindow dialogWindow = new InputDialogWindow("Enter task name", task.Id);
                this.DialogWindow = dialogWindow;

                dialogWindow.Owner = this;
                dialogWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                dialogWindow.OnMessageEvent += (string message) =>
                {
                    if (message == null || message.Length < 0) return;

                    if (TaskStorage == null) return;
                    lock (TaskStorage)
                    {
                        TaskStorage.RemoveValue(task);

                        task.Id = message;
                        TaskStorage.Put(task.Id, task);

                        UpdateList();
                    }
                };

                dialogWindow.ShowDialog();
            } catch (Exception e)
            {
            }
        }

        private void TaskListBox_Text_Name_OnClick(object sender, MouseButtonEventArgs args)
        {
            try
            {
                Task task = ((sender as FrameworkElement).DataContext as Task.TaskListItem).Task;
                if (task == null) return;

                if (DialogWindow != null && DialogWindow.IsVisible)
                {
                    DialogWindow.Close();
                    DialogWindow = null;
                }

                InputDialogWindow dialogWindow = new InputDialogWindow("Enter process name", task.Name);
                this.DialogWindow = dialogWindow;

                dialogWindow.Owner = this;
                dialogWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                dialogWindow.OnMessageEvent += (string message) =>
                {
                    if (message == null) return;

                    if (TaskStorage == null) return;
                    lock (TaskStorage)
                    {
                        TaskStorage.RemoveValue(task);

                        task.Name = message;
                        TaskStorage.Put(task.Id, task);

                        UpdateList();
                    }
                };

                dialogWindow.ShowDialog();
            }
            catch (Exception e)
            {
            }
        }

        private void TaskListBox_Text_FilePath_OnClick(object sender, MouseButtonEventArgs args)
        {
            try
            {
                Task task = ((sender as FrameworkElement).DataContext as Task.TaskListItem).Task;
                if (task == null) return;

                if (DialogWindow != null && DialogWindow.IsVisible)
                {
                    DialogWindow.Close();
                    DialogWindow = null;
                }

                InputDialogWindow dialogWindow = new InputDialogWindow("Enter file path", task.FilePath);
                this.DialogWindow = dialogWindow;

                dialogWindow.Owner = this;
                dialogWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                dialogWindow.OnMessageEvent += (string message) =>
                {
                    if (message == null) return;

                    if (TaskStorage == null) return;
                    lock (TaskStorage)
                    {
                        TaskStorage.RemoveValue(task);

                        task.FilePath = message;
                        TaskStorage.Put(task.Id, task);

                        UpdateList();
                    }
                };

                dialogWindow.ShowDialog();
            }
            catch (Exception e)
            {
            }
        }

        private void UpdateList()
        {
            if (TaskListBox == null) return;

            try
            {
                if (TaskStorage == null)
                {
                    TaskListBox.Items.Clear();
                    return;
                }

                List<Task.TaskListItem> items = new List<Task.TaskListItem>();

                foreach (KeyValuePair<string, Task> taskEntry in TaskStorage.StorageMap)
                {
                    Task task = taskEntry.Value;
                    if (task == null) continue;

                    items.Add(new Task.TaskListItem(task));
                }

                TaskListBox.ItemsSource = items;
            } catch (Exception e)
            {
            }
        }
    }
}
