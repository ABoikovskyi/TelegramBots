using System.Collections.Generic;
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

		public const string InsuranceStartText =
			"Доброго дня! Вас вітає Європейський страховий альянс!\r\nДякуємо, що Ви з нами!\r\n" +
			"Оберіть питання, з яким Ви би хотіли звернутись:";

		public const string InsuranceStartFromBegining = "Почати з початку";
		public const string InsuranceOperation1 = "Пункт 1";
		public const string InsuranceOperation2 = "Пункт 2";
		public const string InsuranceOperation3 = "Пункт 3";
		public const string InsuranceOperation4 = "Пункт 4";
		public const string InsuranceOperation5 = "Пункт 5";

		public const string InsuranceContacts =
			"<b>Контакти сервісного центру:</b>\r\n0800500200\r\n044(352)4522212\r\nEmail: service@eia.com.ua";

		public static Dictionary<string, string> InsuranceMainSteps = new Dictionary<string, string>
		{
			{
				InsuranceOperation1,
				"1. Запис на консультацію лікаря"
			},
			{
				InsuranceOperation2,
				"2. Запит на отримання медикаментів, діагностики по наявному призначенню лікаря при самостійному зверненні за медичною допомогою"
			},
			{
				InsuranceOperation3,
				"3. Перелік документів на виплату"
			},
			{
				InsuranceOperation4,
				"4. Запит на результат розгляду заяви на страхову виплату"
			},
			{
				InsuranceOperation5,
				"5. Телефони сервісного центру з медичного страхування"
			}
		};

		public static Dictionary<InsuranceStep, string> InsuranceStepsText = new Dictionary<InsuranceStep, string>
		{
			{
				InsuranceStep.Operation1Step1,
				"Введіть, будь ласка, номер полісу Застрахованої особи"
			},
			{
				InsuranceStep.Operation1Step2,
				"Зазначте прізвище, імя, по-батькові"
			},
			{
				InsuranceStep.Operation1Step3,
				"Опишіть скарги на стан здоров’я"
			},
			{
				InsuranceStep.Operation1Step4,
				"Зазначте бажаний час прийому, або інтервал часу"
			},
			{
				InsuranceStep.Operation1Step5,
				"Зазначте бажаний медичний заклад, район отримання медичної допомоги"
			},
			{
				InsuranceStep.Operation1End,
				"Дякуємо за Ваш запит, лікар-координатор зв’яжеться з Вами найближчим часом"
			},
			{
				InsuranceStep.Operation2Step1,
				"Введіть, будь ласка, номер полісу Застрахованої особи"
			},
			{
				InsuranceStep.Operation2Step2,
				"Зазначте прізвище, імя, по-батькові Застрахованої особи"
			},
			{
				InsuranceStep.Operation2Step3,
				"Прикріпіть будь ласка фото лікарського призначення з зазначенням діагнозу та ПІБ пацієнта"
			},
			{
				InsuranceStep.Operation2Step4,
				"Зазначте бажану аптеку, лабораторію для отримання послуги, або район пошуку цих закладів"
			},
			{
				InsuranceStep.Operation2End,
				"Дякуємо за Ваш запит, лікар-координатор зв’яжеться з Вами найближчим часом"
			},
			{
				InsuranceStep.Operation3Step1,
				"Введіть, будь ласка, адресу електронної пошти, на яку Ви хочете отримати документи"
			},
			{
				InsuranceStep.Operation3End,
				"Дякуємо, документи будуть направлені на Вашу адресу електронної пошти найближчим часом"
			},
			{
				InsuranceStep.Operation4Step1,
				"Введіть, будь ласка, номер полісу Застрахованої особи"
			},
			{
				InsuranceStep.Operation4Step2,
				"Зазначте прізвище, імя, по-батькові Застрахованої особи"
			},
			{
				InsuranceStep.Operation4End,
				"Дякуємо за запит, співробітник відділу врегулювання  зв’яжеться з Вами найближчим часом та повідомить статус справи"
			}
		};

		public const string InsuranceOperation3DocumentsText =
			@"Страхова виплата здійснюється на підставі таких документів:
- заяви на виплату за встановленою Страховиком формою;
-копії паспорту та довідки про присвоєння ідентифікаційного номера платника податків Застрахованої особи або її законного представника; копії свідоцтва про народження(для неповнолітніх застрахованих);
- у разі отримання амбулаторно-поліклінічних послуг та медикаментів для амбулаторного лікування: консультаційного висновку лікаря(або його завіреної медичним закладом копії) з зазначенням прізвища пацієнта, дати, анамнезу та повного діагнозу, переліку медичних послуг, лікарських засобів, виробів медичного призначення, рекомендованих у зв‘язку із даним захворюванням, засвідченого підписом і печаткою лікаря та печаткою медичного закладу;
- у разі отримання послуг та придбання медикаментів для стаціонарного лікування : оригіналу або засвідченої печаткою лікувального закладу копії виписки з картки стаціонарного хворого та листків лікарських призначень, оформлених належним чином за затвердженими Міністерством охорони здоров’я формами;
- у разі виклику бригади швидкої медичної допомоги: оригіналу або засвідченої медичним закладом копії карти виїзду бригади швидкої медичної допомоги, оформленої належним чином за затвердженою Міністерством охорони здоров’я формою;
- акту виконаних робіт із зазначенням прізвища ім’я по-батькові пацієнта, діагнозу, тривалості лікування, переліку наданих послуг, з розбиттям їх за датами та вартістю, загальної суми до сплати, засвідченого підписом і печаткою лікаря та печаткою медичного закладу, а також з зазначенням назви медичного закладу, адреси його місцезнаходження, контактного номеру телефону, банківських реквізитів, ЄДРПОУ;
- оригіналів фінансових документів, які підтверджують оплату отриманих медичних послуг або придбання медикаментів: фіскальних чеків, квитанцій до прибуткових касових ордерів, розрахункових квитанцій, банківских квитанцій про переказ коштів; товарних чеків(з обов’язковим зазначенням назви ФОП, адреси фактичного надання послуг та реєстрації, контактного телефону, ІПН)- лише у разі отримання послуг у фізичної особи- підприємця.
- у разі придбання рецептурних лікарських засобів, або лікарських засобів, які виготовляються в умовах аптеки: копія заповненого рецептурного бланку, затвердженої Міністерством охорони здоров’я форми;
- у разі отримання медичних послуг у фізичної особи-підприємця(ФОП, СПД) додатково надаються копії Ліцензії на здійснення медичної діяльності та Витягу з ЄДР.";

		public const string InsuranceOperation3AddressForDocuments =
			"Документи потрібно надати протягом 30 днів з дати отримання медичних послуг за адресою:\r\n3038, м. Київ, вул. Ямська, 28\r\nПрАТ \"Європейський страховий альянс\"";

		public const string InsuranceSendDocumentsToMail =
			"Отримати бланку заяви та переліку документів на електронну пошту";

		public const string InsuranceWrondFormat =
			"Некоректний формат вводу";
	}
}