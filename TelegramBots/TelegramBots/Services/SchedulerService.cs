using System;
using System.IO;
using Microsoft.Win32.TaskScheduler;

namespace TelegramBots.Services
{
	public static class SchedulerService
	{
		public static void CreateTask(string exeFilePath, int id, DateTime scheduleTime)
		{
			TaskService.Instance.AddTask($"TelegramBotPublishTask{id}", new TimeTrigger {StartBoundary = scheduleTime},
				new ExecAction($"{Path.Combine(exeFilePath, "PostPublisher.exe")}", $"{id}"));
		}

		public static void UpdateTask(int id, DateTime scheduleTime)
		{
			var task = TaskService.Instance.GetTask($"TelegramBotPublishTask{id}");
			if (task == null)
			{
				return;
			}

			task.Definition.Triggers[0].StartBoundary = scheduleTime;
			task.RegisterChanges();
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