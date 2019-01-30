using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Data;

namespace Forager
{
	/// <summary>
	/// 
	/// </summary>
	public class Database_c
    {
		public Database_c()
		{
            m_cookBooks = new ObservableCollection<CookBook_c>();
            m_ingredients = new ObservableCollection<Ingredient_c>();
            m_recipeIngredients = new ObservableCollection<RecipeIngredient_c>();
            m_recipes = new ObservableCollection<Recipe_c>();
        }

        /// <summary>
        /// This does a little more than just connect to the DB. If there is a DB already then
        /// it will just use the existing one. If there is no DB then it will copy the default
        /// 
        /// Future improvements should not include merging. This should be handled exclusively
        /// by cloud updating
        /// </summary>
        /// <returns></returns>
        private SQLiteConnection _createDatabaseConnection()
        {
#if DEBUG
            File.Delete(Path.Combine(ApplicationData.Current.LocalFolder.Path, "ForagerDB.sqlite3"));
#else
            if(!File.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, "ForagerDB.sqlite3")))
#endif
            {
                File.Copy(Path.Combine(Package.Current.InstalledLocation.Path, @"Assets\ForagerDB.sqlite3"), Path.Combine(ApplicationData.Current.LocalFolder.Path, "ForagerDB.sqlite3"));
            }                

            return new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), Path.Combine(ApplicationData.Current.LocalFolder.Path, "ForagerDB.sqlite3"));
        }

        /// <summary>
        /// Event handler to deal with propogating cookbook collection changes down to the underlying data storage mechanism
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_cookBooks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch(e.Action)
			{
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					foreach(CookBook_c l_cookBook in e.NewItems)
					{
						m_database.Insert(l_cookBook);
					}		
					break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (CookBook_c l_cookBook in e.OldItems)
                    {
                        // Clean up all of the ties to the recipes
                        m_database.Table<CookBookRecipes>().Delete(cookBookRecipe => cookBookRecipe.CookBookID == l_cookBook.ID);
                        m_database.Delete(l_cookBook);
                    }
                    break;
			}
		}

		/// <summary>
		/// Event handler to deal with propogating recipes collection changes down to the underlying data storage mechanism. This
		/// will only make the recipe, not the relationship. That is handled by a seperate explicit call
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_recipes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				// This will happen for brand new recipes
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					foreach (Recipe_c l_recipe in e.NewItems)
					{
						m_database.Insert(l_recipe);
					}
					break;

                // ToDo: On Delete, make sure to remove all relations
			}
		}

        private void m_recipeIngredients_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                // This will happen for brand new recipes
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (RecipeIngredient_c recipeIngredient in e.NewItems)
                    {
                        m_database.Insert(recipeIngredient);
                    }
                    break;

                    // ToDo: On Delete, make sure to remove all relations
            }
        }

        private void m_mealPlans_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (MealPlan_c mealPlan in e.NewItems)
                    {
                        m_database.Insert(mealPlan);
                    }
                    break;
            }
        }

        private void m_ingredients_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				// This will happen for brand new ingredients
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					foreach (Ingredient_c l_ingredient in e.NewItems)
					{
						// Store in database
						m_database.Insert(l_ingredient);

						// Update the group list
						GroupInfoList<object> info = m_ingredientsGrouped.FirstOrDefault(group => (char)group.Key == l_ingredient.Name[0]);
						if (info != null)
							m_ingredientsGrouped.Remove(info);
						else
							info = new GroupInfoList<object>();

						info.Key = l_ingredient.Name[0];
						info.Add(l_ingredient);
						m_ingredientsGrouped.Add(info);
                    }
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
					foreach (Ingredient_c l_ingredient in e.OldItems)
					{
						m_database.Delete(l_ingredient);

						GroupInfoList<object> info = m_ingredientsGrouped.FirstOrDefault(group => group.Contains(l_ingredient));
						m_ingredientsGrouped.Remove(info);
						info.Remove(l_ingredient);

						if (info.Count > 0)
							m_ingredientsGrouped.Add(info);
					}
					break;
			}
		}

        

        private SQLiteConnection m_database;

        private ObservableCollection<CookBook_c> m_cookBooks;
		private ObservableCollection<Ingredient_c> m_ingredients;
		private ObservableCollection<RecipeIngredient_c> m_recipeIngredients;
		private ObservableCollection<Recipe_c> m_recipes;
        private ObservableCollection<MealPlan_c> m_mealPlans;

        // These replace the old "Pantry" system
        public ObservableCollection<Ingredient_c> Ingredients { get { return m_ingredients; } }
        public ObservableCollection<Ingredient_c> AvailableIngredients = new ObservableCollection<Ingredient_c>();
		private ObservableCollection<GroupInfoList<object>> m_ingredientsGrouped = new ObservableCollection<GroupInfoList<object>>();
		public ReadOnlyObservableCollection<GroupInfoList<object>> IngredientsGrouped;
        
        // These replace the old "Library" system
        public ObservableCollection<CookBook_c> CookBooks { get { return m_cookBooks; } }
        public ObservableCollection<Recipe_c> Recipes { get { return m_recipes; } }
        public ObservableCollection<MealPlan_c> MealPlans { get { return m_mealPlans; } }

        // Volatile Data Storage for the Meal Planner
        public ObservableCollection<Recipe_c> MealPlannerTemp { get; } = new ObservableCollection<Recipe_c>();

        public bool Initialise()
		{
            m_database = _createDatabaseConnection();

            // Create each of the required tables. I believe this skips if there is an existing table?
            // ToDo: Maybe wrap this in an "AlreadyExists" check before calling?
            m_database.CreateTable<CookBook_c>();
            m_database.CreateTable<CookBookRecipes>();
            m_database.CreateTable<Recipe_c>();
            m_database.CreateTable<RecipeIngredient_c>();
            m_database.CreateTable<Ingredient_c>();
            m_database.CreateTable<MealPlan_c>();

			// Load up all information from the database into the database collections
			// ToDo: This is a possible area for performance improvement in future. Maybe we can dynamically read?
			m_cookBooks = new ObservableCollection<CookBook_c>(m_database.Table<CookBook_c>());
			m_recipes = new ObservableCollection<Recipe_c>(m_database.Table<Recipe_c>());
			m_recipeIngredients = new ObservableCollection<RecipeIngredient_c>(m_database.Table<RecipeIngredient_c>());
			m_ingredients = new ObservableCollection<Ingredient_c>(m_database.Table<Ingredient_c>());
            m_mealPlans = new ObservableCollection<MealPlan_c>(m_database.Table<MealPlan_c>());

            var l_cookBookRecipes = m_database.Table<CookBookRecipes>();
			IngredientsGrouped = new ReadOnlyObservableCollection<GroupInfoList<object>>(m_ingredientsGrouped);

			m_cookBooks.CollectionChanged += m_cookBooks_CollectionChanged;
			m_recipes.CollectionChanged += m_recipes_CollectionChanged;
            m_recipeIngredients.CollectionChanged += m_recipeIngredients_CollectionChanged;
			m_ingredients.CollectionChanged += m_ingredients_CollectionChanged;
            m_mealPlans.CollectionChanged += m_mealPlans_CollectionChanged;

            // Group the ingredients by first letter
            {

				ObservableCollection<GroupInfoList<object>> groups = new ObservableCollection<GroupInfoList<object>>();

				var query = from ingredient in Ingredients
							orderby ((Ingredient_c)ingredient).Name
							group ingredient by ((Ingredient_c)ingredient).Name[0] into g
							select new { GroupName = g.Key, Items = g };

				foreach (var g in query)
				{
					GroupInfoList<object> info = new GroupInfoList<object>();
					info.Key = g.GroupName;
					foreach (var item in g.Items)
					{
						info.Add(item);
					}
					m_ingredientsGrouped.Add(info);
				}

			}

			// The approach is bottom up, so we initialise all ingredients, then search the collections for the "Foreign Key".
			// I expect this will be fine for the start, but as the app grows this will likely get to be too heavy? To be honest
			// as we have an ID I was thinking to just make a map as it would speed up the searching a lot

			// ToDo: We can make clearly much more performant algorithms here            
            foreach (CookBookRecipes entry in l_cookBookRecipes)
            {
                CookBook_c l_cookBook = m_cookBooks.First(_cookBook => _cookBook.ID == entry.CookBookID);
                Recipe_c l_recipe = m_recipes.First(_recipe => _recipe.ID == entry.RecipeID);

				if (l_cookBook == null)
					throw new IndexOutOfRangeException("Unable to find CookBook ID in database. Unable to create CookBook - Recipe relationship");
				if (l_recipe == null)
					throw new IndexOutOfRangeException("Unable to find Recipe ID in database. Unable to create CookBook - Recipe relationship");

                // Get the RecipeIngredients related to this cookbook, set them up and add to the cookbook
                var l_recipeIngredients = m_recipeIngredients.Where(recipeIngredient => recipeIngredient.RecipeID == l_recipe.ID);

                foreach(RecipeIngredient_c recipeIngredient in l_recipeIngredients)
                {
                    l_recipe.RecipeIngredients.Add(recipeIngredient);
                    recipeIngredient.Ingredient = m_ingredients.FirstOrDefault(ingredient => ingredient.ID == recipeIngredient.IngredientID);
                }

				l_cookBook.Recipes.Add(l_recipe);
            }
            
            // Register all instances for property monitoring so that we can auto save the changes
            foreach(CookBook_c cookBook in m_cookBooks)
            {
                cookBook.PropertyChanged += CookBook_PropertyChanged;
                AvailableIngredients.CollectionChanged += cookBook.AvailableIngredients_CollectionChanged;
            }
            foreach (Recipe_c recipe in m_recipes) { recipe.PropertyChanged += Recipe_PropertyChanged; }
            foreach (RecipeIngredient_c recipeIngredient in m_recipeIngredients) { recipeIngredient.PropertyChanged += RecipeIngredient_PropertyChanged; }

			return true;
		}

        

        private void RecipeIngredient_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Name")
                m_database.Update((RecipeIngredient_c)sender);
        }

        private void Recipe_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {               
            m_database.Update((Recipe_c)sender);
        }

        private void CookBook_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName != "DisplayedRecipes")
                m_database.Update((CookBook_c)sender);
        }

        // Relational table to cope with the many to many nature of the CookBook and Recipe. Added here as only relevant for database storage
        class CookBookRecipes
		{
			public CookBookRecipes() { }
			public CookBookRecipes(int _cookBookID, int _recipeID) { CookBookID = _cookBookID; RecipeID = _recipeID; }

			public int CookBookID { get; set; }
			public int RecipeID { get; set; }
		}

		/*class RecipeRecipeIngredients
		{
            public RecipeRecipeIngredients() { }
            public RecipeRecipeIngredients(int _recipeID, int _recipeIngredientID) { RecipeID = _recipeID; RecipeIngredientID = _recipeIngredientID; }

			public int RecipeID { get; set; }
			public int RecipeIngredientID { get; set; }
		}*/

		/// <summary>
		/// This will create the DB relationship as well as update the actual base objects in the database class.
		/// </summary>
		/// <param name="_cookBook"></param>
		/// <param name="_recipe"></param>
		public void AddRecipeToCookBook(CookBook_c _cookBook, Recipe_c _recipe)
		{
            // Deal with the relationship table and foreign key elements
            if (!m_cookBooks.Contains(_cookBook)) m_cookBooks.Add(_cookBook);
            if (!m_recipes.Contains(_recipe)) m_recipes.Add(_recipe);
            m_database.InsertOrReplace(new CookBookRecipes(_cookBook.ID, _recipe.ID));

            // Add the recipe to the observable data structure so that the UI can easily read it
            _cookBook.Recipes.Add(_recipe);  
        }

        public void AddRecipeIngredientToRecipe(Recipe_c _recipe, RecipeIngredient_c _recipeIngredient)
        {
            // Deal with the relationship table and foreign key elements
            if (!m_recipes.Contains(_recipe)) m_recipes.Add(_recipe);
            _recipeIngredient.RecipeID = _recipe.ID;
            if (!m_recipeIngredients.Contains(_recipeIngredient)) m_recipeIngredients.Add(_recipeIngredient);

            // Add the recipe to the observable data structure so that the UI can easily read it
            _recipe.RecipeIngredients.Add(_recipeIngredient);
        }

        public void RemoveRecipeIngredientFromRecipe(Recipe_c _recipe, RecipeIngredient_c _recipeIngredient)
        {
            _recipe.RecipeIngredients.Remove(_recipeIngredient);
            m_database.Delete(_recipeIngredient);
        }
	}
}
