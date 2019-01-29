using System;
using Microsoft.Win32.TaskScheduler;

namespace TelegramBots.Services
{
	public static class SchedulerService
	{
		public static void CreateTask(int id, DateTime scheduleTime)
		{
			TaskService.Instance.AddTask($"BotPublishTask{id}", new TimeTrigger {StartBoundary = scheduleTime},
				new ExecAction("notepad.exe", ""));
		}

		public static void UpdateTask(int id, DateTime scheduleTime)
		{
			var task = TaskService.Instance.GetTask($"BotPublishTask{id}");
			if (task == null)
			{
				CreateTask(id, scheduleTime);
				return;
			}

			task.Definition.Triggers[0].StartBoundary = scheduleTime;
			task.RegisterChanges();
		}

		public static void DeleteTask(int id)
		{
			TaskService.Instance.RootFolder.DeleteTask($"BotPublishTask{id}");
		}
	}
}