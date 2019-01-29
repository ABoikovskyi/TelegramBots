using System;
using System.Web;
using Microsoft.Win32.TaskScheduler;

namespace TelegramBots.Services
{
	public static class SchedulerService
	{
		public static void CreateTask(int id, DateTime scheduleTime)
		{
			TaskService.Instance.AddTask($"TelegramBotPublishTask{id}", new TimeTrigger {StartBoundary = scheduleTime},
				new ExecAction("powershell.exe",
					$"Invoke-WebRequest -UseBasicParsing -Uri {PopCornBotService.AppLink}/popcorn/PublishPost?postId={HttpUtility.UrlEncode(StringCipher.EncryptPost(id))}"));
		}

		public static void UpdateTask(int id, DateTime scheduleTime)
		{
			var task = TaskService.Instance.GetTask($"TelegramBotPublishTask{id}");
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
			var name = $"TelegramBotPublishTask{id}";
			var task = TaskService.Instance.GetTask(name);
			if (task != null)
			{
				TaskService.Instance.RootFolder.DeleteTask(name);
			}
		}
	}
}