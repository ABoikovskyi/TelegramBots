using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Helpers;
using DataLayer.Context;
using DataLayer.Models.DTO;
using DataLayer.Models.NBCocktailsBar;

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

				if (userData == null || messageText == "/start" || messageText == "Выбрать ещё")
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

				if (userData.StepId.HasValue)
				{
					if (!isNewUser)
					{
						var currentCategory = _ingredientsData.ElementAt(userData.StepId.Value);
						var choosenIngredient = currentCategory.Value.FirstOrDefault(i => i.Name == messageText)?.Id;
						if (choosenIngredient.HasValue)
						{
							userData.Ingredient.Add(choosenIngredient.Value);
							if (userData.StepId.Value == _ingredientsData.Count - 1)
							{
								userData.StepId = null;
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
						await SendTextMessage(new AnswerMessageBase(userId,
							$"Выберите, пожалуйста, ингридиенты категории {nextCategoryData.Key.Name}",
							nextCategoryData.Value.Select(i => (object)i.Name).ToList()));
					}
					else
					{
						var recommendedCocktails = _context.Cocktails
							.Where(c => c.Ingredients.All(i => userData.Ingredient.Contains(i.IngredientId)))
							.Select(c => c.Name).ToList();
						recommendedCocktails.Add("Выбрать ещё");
						await SendTextMessage(new AnswerMessageBase(userId,
							"Мы вам рекомендуем следующие коктейли:",
							recommendedCocktails.Select(i => (object)i).ToList()));
					}
				}
			}
			catch (Exception ex)
			{
			}
		}
	}
}