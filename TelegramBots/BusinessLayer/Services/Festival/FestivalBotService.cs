using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Helpers;
using DataLayer.Context;
using DataLayer.Models.DTO;
using DataLayer.Models.Enums;
using DataLayer.Models.Festival;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DataLayer.Models.Festival.User;

namespace BusinessLayer.Services.Festival
{
	public class FestivalBotService
	{
        public static readonly CultureInfo CurrentCultureInfo = new CultureInfo("ru");
        private readonly FestivalDbContext _context;
        private readonly MemoryCacheHelper _memoryCacheHelper;
        public static string[][] MainKeyboard;
        public static string[][] StageKeyboard;
        public static string[][] ArtistKeyboard;
        public static Dictionary<long, int> UserCurrentStage = new Dictionary<long, int>();
        public static Dictionary<long, int> UserCurrentArtist = new Dictionary<long, int>();
        public static TelegramBotClient Client;

        static FestivalBotService()
        {
            MainKeyboard = new[]
            {
                new[] {PhraseHelper.Stages, PhraseHelper.Artists},
                new[] {PhraseHelper.Schedule},
                new[] {PhraseHelper.Map, PhraseHelper.HowToGetTo},
                new[] {PhraseHelper.About, PhraseHelper.Contacts}
            };

            StageKeyboard = new[]
            {
                new[] {PhraseHelper.BackToStages, PhraseHelper.MainMenu}
            };

            ArtistKeyboard = new[]
            {
                new[] {PhraseHelper.Information},
                new[] {PhraseHelper.NotifyMe},
                new[] {PhraseHelper.SubscribeToArtist},
                new[] {PhraseHelper.BackToArtists, PhraseHelper.MainMenu}
            };
        }

        public FestivalBotService(FestivalDbContext context, MemoryCacheHelper memoryCacheHelper)
        {
            _context = context;
            _memoryCacheHelper = memoryCacheHelper;
        }

        public static void Init()
        {
            if (Client == null)
            {
                QuartzService.ResetFestivalJobs();
                Client = new TelegramBotClient("767658547:AAGHxb3XWihezv02gflFhy542ZclQR9HwA4");
            }
        }

        public static async Task SetWebHook()
        {
            await Client.DeleteWebhookAsync();
            await Client.SetWebhookAsync($"{Links.AppLink}/api/message/festivalupdate");
        }

        public async Task SendTextMessage(AnswerMessageBase message)
        {
            await Client.SendTextMessage(message);
        }

        public async Task ProcessCallbackMessage(CallbackQuery callback)
        {
            var message = callback.Message;

            await ProcessMessageBase(message.Chat.Id, message.Chat.FirstName, message.Chat.LastName,
                callback.Data);
        }

        public async Task ProcessMessage(Message message)
        {
            await ProcessMessageBase(message.Chat.Id, message.Chat.FirstName, message.Chat.LastName,
                message.Text);
        }

        public async Task ProcessMessageBase(long chatId, string userFirstName, string userLastName,
            string messageText)
        {
            try
            {
                var festivalInfo = _memoryCacheHelper.GetFestivalInfo();
                var artists = _memoryCacheHelper.GetArtists();

                //stage message checking
                var stage = _context.Stages.FirstOrDefault(c => c.Name == messageText);
                if (stage != null)
                {
                    SetUserStage(chatId, stage.Id);
                    var currentSchedule = _context.Schedule
                        .Where(d => d.StartDate.Date == DateTime.Now && d.StageId == stage.Id)
                        .OrderBy(d => d.StartDate).Select(d => new {ArtistName = d.Artist.Name, d.StartDate, d.EndDate})
                        .ToList()
                        .ToDictionary(d => $"{d.ArtistName} {d.StartDate:HH:mm}-{d.EndDate:HH:mm}", d => d.ArtistName);

                    await SendTextMessage(new AnswerMessageBase(chatId, "Расписание текущего дня")
                    {
                        InlineKeyboard = currentSchedule
                    });
                    return;
                }

                // artist message checking
                var artist = artists.FirstOrDefault(a => messageText.StartsWith(a.Name));
                if (artist != null)
                {
                    SetUserCurrentArtist(chatId, artist.Id);
                    var artistSchedule = _context.Schedule.FirstOrDefault(s => s.ArtistId == artist.Id);
                    var text = $"{artist.Name}\r\n" +
                               $"{(artistSchedule == null ? "Расписания еще нету" : $"{artistSchedule.StartDate.ToString("dd MMMM HH:mm", CurrentCultureInfo)} - {artistSchedule.EndDate:HH:mm}")}";
                    if (artist.Image != null)
                    {
                        await SendTextMessage(new AnswerMessageBase(chatId, artist.Image, ArtistKeyboard)
                            {IsPhoto = true});
                    }

                    await SendTextMessage(new AnswerMessageBase(chatId, text, ArtistKeyboard));

                    return;
                }

                //date message checking
                if (DateTime.TryParse(messageText, CurrentCultureInfo, DateTimeStyles.None, out var festivalDay))
                {
                    var daySchedule = _context.Schedule.Where(s =>
                            s.StartDate.Day == festivalDay.Day && s.StartDate.Month == festivalDay.Month)
                        .Select(s => new
                        {
                            StageId = s.Stage.Id,
                            StageName = s.Stage.Name,
                            ArtistName = s.Artist.Name,
                            s.StartDate,
                            s.EndDate
                        }).ToList().OrderBy(s => s.StageId).GroupBy(s => s.StageName)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    foreach (var stageSchedule in daySchedule)
                    {
                        await SendTextMessage(new AnswerMessageBase(chatId, stageSchedule.Key)
                        {
                            InlineKeyboard = stageSchedule.Value.OrderBy(v=>v.StartDate).ToDictionary(
                                s => $"{s.ArtistName} {s.StartDate:HH:mm}-{s.EndDate:HH:mm}", s => s.ArtistName)
                        });
                    }

                    return;
                }

                if (UserCurrentArtist.ContainsKey(chatId))
                {
                    artist = artists.First(c => c.Id == UserCurrentArtist[chatId]);
                    switch (messageText)
                    {
                        case PhraseHelper.Information:
                        {
                            var links = string.Join("\r\n", new[]
                                {
                                    artist.Website, artist.Facebook, artist.Youtube,
                                    artist.Instagram, artist.Twitter, artist.Itunes
                                }
                                .Where(d => !string.IsNullOrEmpty(d)));
                            var text = $"{artist.Description}\r\n\r\nLinks:\r\n{links}";
                            await SendTextMessage(new AnswerMessageBase(chatId, text, ArtistKeyboard));

                            return;
                        }
                        case PhraseHelper.NotifyMe:
                        {
                            var scheduleId = _context.Schedule.FirstOrDefault(s => s.ArtistId == artist.Id)?.Id;
                            var reply = await SubscribeToArtist(chatId, SubscriptionType.Notify, artist.Id, scheduleId);
                            await SendTextMessage(new AnswerMessageBase(chatId, reply, ArtistKeyboard));
                            return;
                        }
                        case PhraseHelper.SubscribeToArtist:
                        {
                            var reply = await SubscribeToArtist(chatId, SubscriptionType.Common, artist.Id);
                            await SendTextMessage(new AnswerMessageBase(chatId, reply, ArtistKeyboard));
                            return;
                        }
                        case PhraseHelper.BackToArtists:
                        {
                            UserCurrentArtist.Remove(chatId);
                            messageText = UserCurrentStage.TryGetValue(chatId, out var stageId)
                                ? _context.Stages.First(s => s.Id == stageId).Name
                                : PhraseHelper.MainMenu;
                            break;
                        }
                    }
                }

                switch (messageText)
                {
                    case PhraseHelper.Start:
                    case PhraseHelper.MainMenu:
                    {
                        if (messageText == PhraseHelper.Start)
                        {
                            InsertNewUser(chatId, userFirstName, userLastName);
                        }

                        UserCurrentStage.Remove(chatId);
                        UserCurrentArtist.Remove(chatId);
                        await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.FestivalHelloText,
                            MainKeyboard));
                        return;
                    }
                    case PhraseHelper.HowToGetTo:
                    {
                        await SendTextMessage(new AnswerMessageBase(chatId, festivalInfo.HowToGetTo, MainKeyboard));
                        return;
                    }
                    case PhraseHelper.Map:
                    {
                        await SendTextMessage(new AnswerMessageBase(chatId, "Карта", festivalInfo.Map, MainKeyboard));
                        return;
                    }
                    case PhraseHelper.Contacts:
                    {
                        await SendTextMessage(new AnswerMessageBase(chatId, festivalInfo.Contacts, MainKeyboard)
                        {
                            IsHtml = true
                        });
                        return;
                    }
                    case PhraseHelper.About:
                    {
                        await SendTextMessage(new AnswerMessageBase(chatId, festivalInfo.Description, MainKeyboard)
                        {
                            IsHtml = true
                        });
                        return;
                    }
                    case PhraseHelper.Stages:
                    case PhraseHelper.BackToStages:
                    {
                        var stages = _context.Stages.Select(s => (object)s.Name).ToList();
                        stages.Add(PhraseHelper.MainMenu);

                        await SendTextMessage(new AnswerMessageBase(chatId, "Выберите сцену", stages));
                        return;
                    }
                    case PhraseHelper.Artists:
                    case PhraseHelper.BackToArtists:
                    {
                        await SendTextMessage(new AnswerMessageBase(chatId, "Выберите артиста", ArtistsKeyboard(artists)));
                        return;
                    }
                    case PhraseHelper.Schedule:
                    {
                        var daysKeyboard = _context.Schedule.Select(s => s.StartDate.Date).OrderBy(d => d).ToList()
                            .Select(d => (object)d.ToString("dd MMMM", CurrentCultureInfo)).Distinct().ToList();
                        daysKeyboard.Add(PhraseHelper.MainMenu);
                        await SendTextMessage(new AnswerMessageBase(chatId, "Выберите дату", daysKeyboard));
                        return;
                    }
                    default:
                    {
                        await SendTextMessage(new AnswerMessageBase(chatId, PhraseHelper.InvalidCommand, MainKeyboard));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private async Task<string> SubscribeToArtist(long chatId, SubscriptionType type, int? artistId = null, int? scheduleId = null)
        {
            var user = _context.Users.First(u => u.ChatId == chatId);
            if (!artistId.HasValue)
            {
                return null;
            }

            if (_context.UserSubscription.Any(s => s.UserId == user.Id && s.ArtistId == artistId && s.Type == type))
            {
                return $"{user.FirstName}, вы уже подписаны артиста";
            }

            var userSubscription = new UserSubscription
            {
                UserId = user.Id,
                ArtistId = artistId.Value,
                ScheduleId = scheduleId,
                Type = type
            };
            _context.UserSubscription.Add(userSubscription);
            _context.SaveChanges();

            if (type == SubscriptionType.Notify && scheduleId.HasValue)
            {
                var date = _context.Schedule.FirstOrDefault(s => s.Id == scheduleId.Value)?.StartDate;
                if (date != null)
                {
                    await QuartzService.StartNotifyUserJob(userSubscription.Id, date.Value.AddMinutes(-10));
                }
            }

            return $"{user.FirstName}, теперь вы подписаны артиста";
        }

        private void SetUserStage(long chatId, int stageId)
        {
            if (UserCurrentStage.ContainsKey(chatId))
            {
                UserCurrentStage[chatId] = stageId;
            }
            else
            {
                UserCurrentStage.Add(chatId, stageId);
            }
        }

        private static void SetUserCurrentArtist(long chatId, int artistId)
        {
            if (UserCurrentArtist.ContainsKey(chatId))
            {
                UserCurrentArtist[chatId] = artistId;
            }
            else
            {
                UserCurrentArtist.Add(chatId, artistId);
            }
        }

        private static string[][] ArtistsKeyboard(List<Artist> artists)
        {
            var structured = new List<string[]>();
            var step = 2;
            for (var i = 0; i < artists.Count; i = i + step)
            {
                structured.Add(artists.Skip(i).Take(step)
                    .Select(c => new string(c.Name)).ToArray());
            }

            structured.Add(new[] { PhraseHelper.MainMenu });

            return structured.ToArray();
        }

        private void InsertNewUser(long chatId, string userFirstName, string userLastName)
        {
            if (!_context.Users.Any(u => u.ChatId == chatId))
            {
                _context.Add(new User
                {
                    ChatId = chatId,
                    FirstName = userFirstName,
                    LastName = userLastName
                });
                _context.SaveChanges();
            }
        }

        public async Task SendNewPostAlert(Post post)
        {
            var insertSubscriptionLink = post.ArtistId.HasValue && post.IsCommonPost;
            var neededUsers = post.ArtistId.HasValue && !post.IsCommonPost
                ? _context.UserSubscription.Include(s => s.User).Where(s => s.ArtistId == post.ArtistId.Value)
                    .Select(s => s.User).ToList()
                : _context.Users.ToList();
            var subscribedUsers = insertSubscriptionLink
                ? _context.UserSubscription.Include(s => s.User).Where(s => s.ArtistId == post.ArtistId.Value)
                    .Select(s => s.User.ChatId).ToList()
                : null;

            var subscriptionLink = insertSubscriptionLink
                ? new[]
                {
                    new[] {PhraseHelper.MainMenu}
                }
                : null;
            foreach (var user in neededUsers)
            {
                await SendTextMessage(
                    new AnswerMessageBase(user.ChatId, $"{post.Title}\r\n\r\n{post.Desription}\r\n\r\n{post.Link}")
                    {
                        Keyboard = insertSubscriptionLink && !subscribedUsers.Contains(user.ChatId)
                            ? subscriptionLink
                            : MainKeyboard
                    });
            }
        }

        public async Task SendNotifyMeAlert(int notifyId)
        {
            var notifyInfo = _context.UserSubscription.Where(s => s.Id == notifyId)
                .Select(s => new
                {
                    s.User.ChatId,
                    ArtistName = s.Artist.Name,
                    StageName = s.Schedule.Stage.Name,
                    s.Schedule.StartDate
                }).FirstOrDefault();

            if (notifyInfo == null)
            {
                return;
            }

            await SendTextMessage(new AnswerMessageBase(notifyInfo.ChatId,
                $"Артист {notifyInfo.ArtistName} выйдет на {notifyInfo.StageName} в {notifyInfo.StartDate:HH:mm}"));
        }
    }
}