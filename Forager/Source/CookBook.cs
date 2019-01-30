using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.ApplicationModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.ObjectModel;
using SQLite.Net.Attributes;
using Windows.UI.Xaml.Controls;

namespace Forager
{
    public class CookBook_c : INotifyPropertyChanged
    {
        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

		[PrimaryKey, AutoIncrement]
		public int ID { get; set; } = -1;

        public string Name { get; set; }
  
        public string Description { get; set; }

        [Ignore]
        public ObservableCollection<Recipe_c> Recipes { get; set; }

		// Should know the filter options. I also do not like the idea of maintaining multiple lists publicly
		[Ignore]
		public ObservableCollection<Recipe_c> DisplayedRecipes { get; set; }

		[Ignore]
		private ObservableCollection<Recipe_c> AvailableRecipes { get; set; }

		[Ignore]
		private ObservableCollection<Recipe_c> MissingAnIngredient { get; set; }

        public void AvailableIngredients_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            AvailableRecipes.Clear();
            MissingAnIngredient.Clear();

            var l_availableIngredients = (ObservableCollection<Ingredient_c>)sender;

            foreach (Recipe_c recipe in Recipes)
            {
                int l_intersectCount = recipe.RequiredIngredients.Intersect(l_availableIngredients).Count();
                if (l_intersectCount == recipe.RequiredIngredients.Count)
                {
                    AvailableRecipes.Add(recipe);
                    continue;
                }
                else if (l_intersectCount == recipe.RequiredIngredients.Count - 1)
                {
                    MissingAnIngredient.Add(recipe);
                    continue;
                }
            }

            // DisplayFilter = DisplayFilter; // Trigger a refresh of the displayed recipes so we are not maintaining the list here also
                                           //NotifyPropertyChanged("DisplayedRecipes");
        }

   //     public void UpdateAvailableRecipes(object sender, IngredientsUpdatedEventArgs e)
   //     {
   //         AvailableRecipes.Clear();
   //         MissingAnIngredient.Clear();

   //         foreach (Recipe recipe in Recipes)
   //         {
   //             int l_intersectCount = recipe.RequiredIngredients.Intersect(e.TheDatabase.AvailableIngredients).Count();
   //             if (l_intersectCount == recipe.RequiredIngredients.Count)
   //             {
   //                 AvailableRecipes.Add(recipe);
   //                 continue;
   //             }
   //             else if (l_intersectCount == recipe.RequiredIngredients.Count - 1)
   //             {
   //                 MissingAnIngredient.Add(recipe);
   //                 continue;
   //             }
   //         }

			//DisplayFilter = DisplayFilter; // Trigger a refresh of the displayed recipes so we are not maintaining the list here also
			////NotifyPropertyChanged("DisplayedRecipes");
   //     }

        private FilterOptions_c.DisplayFilter_e m_displayFilter = 0;

        [Ignore]
        public FilterOptions_c.DisplayFilter_e DisplayFilter
        {
            get { return m_displayFilter; }
            set
            {
                switch (value)
                {
                    case FilterOptions_c.DisplayFilter_e.All:
                        DisplayedRecipes = Recipes;
                        break;
                    case FilterOptions_c.DisplayFilter_e.HaveIngredients:
                        DisplayedRecipes = AvailableRecipes;
                        break;
                    case FilterOptions_c.DisplayFilter_e.MissingAnIngredient:
                        DisplayedRecipes = MissingAnIngredient;
                        break;

                }
                m_displayFilter = value;
                NotifyPropertyChanged("DisplayedRecipes");
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public CookBook_c()
        {
            Recipes = new ObservableCollection<Recipe_c>();
            AvailableRecipes = new ObservableCollection<Recipe_c>();
            MissingAnIngredient = new ObservableCollection<Recipe_c>();
            DisplayedRecipes = Recipes;
        }

        /* As this is a UWP app, the save will always be to our specified location
        // in AppData. Export will allow the ability to save out to files for sharing
        public async void Save()
        {
            StorageFolder myCookBookFolder = (StorageFolder)await ApplicationData.Current.LocalFolder.TryGetItemAsync("MyCookBooks");
            if(myCookBookFolder == null)
            {
                myCookBookFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("MyCookBooks");
            }
            StorageFile myCookBookFile = await myCookBookFolder.CreateFileAsync("CookBook_" + Name + ".xml", CreationCollisionOption.ReplaceExisting);
            Tools.SerialiseXmlToFile(this, myCookBookFile);
        }

        public async void Delete()
        {
            StorageFolder myCookBookFolder = (StorageFolder)await ApplicationData.Current.LocalFolder.TryGetItemAsync("MyCookBooks");
            if (myCookBookFolder == null)
                return;

            StorageFile myCookBookFile = (StorageFile)await myCookBookFolder.TryGetItemAsync("CookBook_" + Name + ".xml");
            if (myCookBookFile == null)
                return;

            await myCookBookFile.DeleteAsync();
        }
        */
    }
}
