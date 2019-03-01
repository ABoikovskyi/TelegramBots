using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Helpers;
using DataLayer.Context;
using DataLayer.Models.DTO;
using DataLayer.Models.NBCocktailsBar;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services.NBCocktailsBar
{
	public class NBCocktailsBarBotServiceBase
	{
		private readonly NBCocktailsBarDbContext _context;
		private static readonly Dictionary<string, NBBarUserData> UsersData = new Dictionary<string, NBBarUserData>();
		private readonly Dictionary<IngredientCategory, List<Ingredient>> _ingredientsData;

		public NBCocktailsBarBotServiceBase(NBCocktailsBarDbContext context, MemoryCacheHelper memoryCacheHelper)
		{
			_context = context;
			_ingredientsData = memoryCacheHelper.GetIngredientsData();
		}

		public virtual Task SendTextMessage(AnswerMessageBase message)
		{
			return Task.FromResult(default(object));
		}

		public async Task ProcessMessageBase(string userId, string userFirstName, string userLastName,
			string messageText)
		{
			try
			{
				var isNewUser = false;
				UsersData.TryGetValue(userId, out var userData);

				if (userData == null || messageText == "/start" || messageText == PhraseHelper.ChooseMore)
				{
					userData = new NBBarUserData
					{
						UserId = userId,
						StepId = 0,
						Ingredient = new List<int>()
					};

					UsersData.Remove(userId);
					UsersData.Add(userId, userData);

					await SendTextMessage(new AnswerMessageBase(userId, "Привет! Я помогу тебе подобрать коктейль"));
					isNewUser = true;
				}

				if (!isNewUser && userData.StepId.HasValue)
				{
					var currentCategory = _ingredientsData.ElementAt(userData.StepId.Value);
					var isSkip = messageText == PhraseHelper.Skip;
					var choosenIngredient = isSkip
						? null
						: currentCategory.Value.FirstOrDefault(i => i.Name == messageText)?.Id;
					if (choosenIngredient.HasValue || isSkip)
					{
						if (!isSkip)
						{
							userData.Ingredient.Add(choosenIngredient.Value);
						}

						if (userData.StepId.Value == _ingredientsData.Count - 1)
						{
							userData.StepId = null;
							messageText = "";
						}
						else
						{
							userData.StepId++;
						}
					}
					else
					{
						await SendTextMessage(new AnswerMessageBase(userId, "Некорректная команда"));
					}
				}

				if (userData.StepId.HasValue)
				{
					var nextCategoryData = _ingredientsData.ElementAt(userData.StepId.Value);
					var ingredients = nextCategoryData.Value.Select(i => (object)i.Name).ToList();
					ingredients.Add(PhraseHelper.Skip);
					await SendTextMessage(new AnswerMessageBase(userId,
						$"Выберите, пожалуйста, ингредиенты категории {nextCategoryData.Key.Name}",
						ingredients));
				}
				else
				{
					var cocktails = _context.Cocktails.Include(c => c.Ingredients).ToList();
					var isEmptyMessage = string.IsNullOrEmpty(messageText);
					if (isEmptyMessage || messageText == PhraseHelper.Back)
					{
						var recommendedCocktails = cocktails
							.Where(c => userData.Ingredient.All(ui =>
								c.Ingredients.Select(i => i.IngredientId).Contains(ui)))
							.Select(c => c.Name).ToList();
						recommendedCocktails.Add(PhraseHelper.ChooseMore);
						await SendTextMessage(new AnswerMessageBase(userId,
							recommendedCocktails.Count == 1
								? "Увы, по вашим параметрам коктейлей не найдено"
								: "Мы рекомендуем вам следующие коктейли:",
							recommendedCocktails.Select(i => (object)i).ToList()));
					}
					else
					{
						var cocktail = cocktails.FirstOrDefault(c => c.Name == messageText);
						if (cocktail != null)
						{
							await SendTextMessage(new AnswerMessageBase(userId, cocktail.Description ?? "Описание отсутствует",
								new List<object> {PhraseHelper.Back, PhraseHelper.ChooseMore}));
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
		}
	}
}