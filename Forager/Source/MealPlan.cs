using SQLite.Net.Attributes;
using System;
using System.Collections.ObjectModel;

namespace Forager
{
    public enum MealPlanEntryTime_e
    {
        Breakfast,
        Lunch,
        Dinner,
        Snack
    }

    public class MealPlan_c
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Ignore]
        public ObservableCollection<MealPlanEntry_c> BreakfastItems { get; set; } = new ObservableCollection<MealPlanEntry_c>();
        [Ignore]
        public ObservableCollection<MealPlanEntry_c> LunchItems { get; set; } = new ObservableCollection<MealPlanEntry_c>();
        [Ignore]
        public ObservableCollection<MealPlanEntry_c> DinnerItems { get; set; } = new ObservableCollection<MealPlanEntry_c>();
        [Ignore]
        public ObservableCollection<MealPlanEntry_c> SnackItems { get; set; } = new ObservableCollection<MealPlanEntry_c>();

        public string Name { get; set; }

        public MealPlan_c()
        {
            BreakfastItems.CollectionChanged += ItemSet_CollectionChanged;
            LunchItems.CollectionChanged += ItemSet_CollectionChanged;
            DinnerItems.CollectionChanged += ItemSet_CollectionChanged;
            SnackItems.CollectionChanged += ItemSet_CollectionChanged;
        }

        private void ItemSet_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (MealPlanEntry_c item in e.NewItems)
                {
                    //if(item.ID < 0)
                    //Add the MealPlanEntry to the DataBase. Have the DB manage itself


                }
            }
        }
    }

    public class MealPlanEntry_c
    {
        public MealPlanEntry_c() { }

        public MealPlanEntry_c(Recipe_c _recipe)
        {
            Recipe = _recipe;
            RecipeID = Recipe.ID;
        }

        public MealPlanEntry_c(MealPlanEntry_c _other)
        {
            Recipe = _other.Recipe;
            RecipeID = Recipe.ID;
            MealPlanID = _other.MealPlanID;
            MealPlanEntryTime = _other.MealPlanEntryTime;
        }

        public override string ToString()
        {
            return Recipe.Name;
        }

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = -1;

        public int MealPlanID { get; set; } = -1;

        public int RecipeID { get; set; } = -1;

        public Recipe_c Recipe { get; set; }

        public MealPlanEntryTime_e MealPlanEntryTime { get; set; }
    }
}