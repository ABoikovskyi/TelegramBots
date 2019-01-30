using System;
using System.IO;
using Microsoft.Win32.TaskScheduler;

namespace TelegramBots.Services
{
	public static class SchedulerService
	{
		public static void CreateTask(string exeFilePath, int id, DateTime scheduleTime)
		{
			var ts = TaskService.Instance;
			var td = ts.NewTask();
			td.Actions.Add(new ExecAction($"{Path.Combine(exeFilePath, "PostPublisher.exe")}", $"{id}", exeFilePath));
			td.Triggers.Add(new TimeTrigger {StartBoundary = scheduleTime});
			td.Principal.UserId = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
			td.Principal.RunLevel = TaskRunLevel.Highest;
			td.Principal.LogonType = TaskLogonType.InteractiveToken;
			//td.Settings.Compatibility = TaskCompatibility.V2_3;
			ts.RootFolder.RegisterTaskDefinition($"TelegramBotPublishTask{id}", td, TaskCreation.CreateOrUpdate,
				"uh1131963", "cu7rwa4RMX", TaskLogonType.Password);
			/*ts.RootFolder.RegisterTaskDefinition($"TelegramBotPublishTask{id}", td, TaskCreation.CreateOrUpdate,
				"NET_", "scores", TaskLogonType.Password);*/
		}

		public static void UpdateTask(string exeFilePath, int id, DateTime scheduleTime)
		{
			var task = TaskService.Instance.GetTask($"TelegramBotPublishTask{id}");
			if (task == null)
			{
				return;
			}

			CreateTask(exeFilePath, id, scheduleTime);
		}

		public static void DeleteTask(int id)
		{
			var name = $"TelegramBotPublishTask{id}";
			var task = TaskService.Instance.GetTask(name);
			if (task != null)
			{
				TaskService.Instance.RootFolder.DeleteTask(name);
			}
		}

		public static Task GetTask(int id)
		{
			var name = $"TelegramBotPublishTask{id}";
			return TaskService.Instance.GetTask(name);
		}
	}
}