﻿using System.Collections.Generic;
using DataLayer.Models.Enums;

namespace BusinessLayer.Helpers
{
	public static class PhraseHelper
	{
		public const string Start = "/start";
		public const string MainMenu = "Главное меню";
		public const string About = "О нас";
		public const string Contacts = "Контакты";
		public const string Tickets = "Билеты";
		public const string Concerts = "Концерты";
		public const string Sales = "Акции";
		public const string Back = "⬅ Назад";
		public const string InvalidCommand = "Некорректная команда";

		public const string FutureConcerts = "Будущие";
		public const string PastConcerts = "Предыдущие";
		public const string AllConcerts = "Все";
		public const string BackToConcerts = "Назад к концертам";

		public const string ConcertTickets = "Билеты";
		public const string Facebook = "Facebook";
		public const string ShortDescription = "Краткая информация";
		public const string FullDescription = "Полное описание";
		public const string Bio = "Биография группы";
		public const string Poster = "Афиша";
		public const string VideoPoster = "Видео-афиша";
		public const string Media = "Медиа";
		public const string Subscribe = "Подписаться на новости концерта";

		public const string FestivalHelloText = "Вас приветствует telegram-бот фестиваля Atlas Weekend";
		public const string Information = "Информация";
		public const string SubscribeToArtist = "Подписаться на обновления";
		public const string NotifyMe = "Уведомить за 10 минут до выхода на сцену";
		public const string BackToArtists = "Назад к артистам";
		public const string Stages = "Сцены";
		public const string Artists = "Артисты";
		public const string Schedule = "Расписание";
		public const string BackToStages = "Назад к сценам";
		public const string Map = "Карта фестиваля";
		public const string HowToGetTo = "Как добраться";
		public const string AllArtists = "Все артисты";
		public const string ChooseMore = "Выбрать ещё";

		public const string MainMenuText = "🥃";
		public const string Idrink = "Я пью 🥃";
		public const string DrinkHistory = "История выпиваний 📅";
		public const string SubscribeToFriend = "Подписаться на друга 🔔";
		public const string Settings = "Настройки ⚙️";
		public const string AboutBot = "О боте ℹ️";

		public const string IdrinkAboutBotText =
			"Вас приветствует telegram-бот \"Я пью\". Отмечайте когда и где вы пьете, подписывайтесь на друзей!\r\n\r\nОбратная связь: info@idrink.com.ua";

		public const string SubscribedToList = "Мои подписки 📝";
		public const string MySubscribersList = "Мои подписчики 📝";
		public const string SetGeolocationQuestion = "Укажете геолокацию? 📍";
		public const string SetGeolocation = "Указать 📍";
		public const string NoLocation = "Не указывать ⛔";
		public const string IdrinkCongrats = "Поздравляем! Вы пьёте впервые 🎉";

		public const string IdrinkCongratsWithDate =
			"Поздравляем! Вы продержались без алкоголя {0} дней {1} часов {2} минут 🎉🎉🎉\r\nСчетчик сброшен 🍺🍷🥃🍹";

		public const string Location = "Локация";
		public const string DrinkHistoryQuestion = "Выберите период 📆";
		public const string LastWeek = "Последняя неделя";
		public const string LastMonth = "Последний месяц";
		public const string YouDrinkAt = "🕒 Употребление алкоголя началось в {0}";

		public const string HowToSubscribeToFriend =
			"Что бы подписаться на выпивающего друга, вы можете отправить контакт через 📎 -> Контакт или отправив имя пользователя (@username) 👋.";

		public const string AlreadySubscribed = "Вы уже подписаны этого пользователя";
		public const string SubscribeToYouself = "Вы не можете подписаться сами на себя 🤔";
		public const string ContactDoesntUseBot = "{0} {1} еще не пользуется ботом 😔\r\nМожет стоит поделиться? 😏";
		public const string SuccessfullySubscribe = "Вы успешно подписаны на {0} {1} 🙌";
		public const string SuccessfullyUnSubscribe = "Вы успешно отписались ✔️";
		public const string SuccessfullyRemoveSubscriber = "Вы успешно удалили подписчика ✔️";
		public const string YouHaveNewSubscriber = "На вас подписался пользователь {0} {1}{2} 👋";
		public const string DrinkingNow = "{0} {1}{2} пьёт 🥃";
		public const string SubscribeTo = "Подписаться в ответ 🤝";
		public const string SubscribeToCode = "subscribeTo";
		public const string ForUnsubscribe = "Для отписки от пользователя отправьте его номер #️⃣";
		public const string YouDontSubscriberToAnyone = "Вы ни на кого не подписаны 😔";
		public const string ForUnsubscribeFromMe = "Что бы отписать от себя пользователя отправьте его номер #️⃣";
		public const string NoSubscribers = "У вас нету подписчиков 😔";
		public const string YouAreNowSubscribed = "Вы не подписаны на этого пользователя 🤔";
		public const string ThisUserNotSubscribedOnYou = "Данный пользователь не подписан на вас 🤔";
		public const string AreYouInterested = "Интересуетесь? 😏";
		public const string Yes = "Да 👍";
		public const string YesCode = "InterestedYes";
		public const string No = "Нет 👎";
		public const string NoCode = "InterestedNo";
		public const string InterestedToDrinkWithYou = "{0} {1}{2} интересуется вашим времяпрепровождением 💡";

		public const string YouDrinkTooMuch =
			"С момента последнего употребления алкоголя прошло менее 30 минут. Успокойтесь, попейте водички 😉";

		public const string ManageYourSettings = "Управляйте личными настройками ⚙️";
		public const string WouldYouLikeToAddPhoto = "Добавите фото? 📷";
		public const string BotDontWorkWithGroups = "Бот не работает с группами или супергруппами 🤷";
		public const string Skip = "Пропустить ⏭️";

		public const string Register = "Регистрация";
		public const string FindTender = "Найти тендер";
		public const string MySubscriptions = "Мои подписки";
		public const string SubscribeToTender = "Подписаться на обновления тендера";

		public const string ProzorroAboutBotText =
			"Привет. Это бот для мониторинга тендров Prozorro. Пожалуйста, пройдите регистрацию для получения доступа к функциям";

		public const string RegistrationSuccess = "Поздравляю! Вы успешно зарегестрировавились. " + HowToFindTender;
		public const string YouAlreadyRegistered = "Вы уже зарегистрированы";

		public const string HowToFindTender =
			"Для получения информации о тендере, введите его номер (например UA-2020-04-01-000977-b)";

		public const string SubscribeOnTender = "Подписаться на тендер ";
		public const string SuccessfullySubscribeOnTender = "Вы успешно подписались на тендер";
		public const string Unsubscribe = "Отписаться";
		public const string UnsubscribeCode = "Unsubscribe";
		public const string SuccessfullyUnsubscribeFromTender = "Вы успешно отписались от обновлений тендера";
		public const string InvalidTender = "Некорректная индентификатор тендера";

		public const string InsuranceWrongFormat = "Некоректний формат вводу";
	}
}