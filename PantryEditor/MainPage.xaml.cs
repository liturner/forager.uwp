using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLite.Net;
using SQLite.Net.Attributes;
using Windows.ApplicationModel.DataTransfer;

using System.Diagnostics;


// Die Vorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 dokumentiert.

namespace PantryEditor
{
    public class MyListView : ListView
    {
        public ObservableCollection<string> dataThing = new ObservableCollection<string>();

        public MyListView()
        {
            base.DataContext = dataThing;
        }
    }

    public class Recipe_c { };

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

        ObservableCollection<MealPlanEntry_c> BreakfastItems = new ObservableCollection<MealPlanEntry_c>();
        ObservableCollection<MealPlanEntry_c> LunchItems = new ObservableCollection<MealPlanEntry_c>();
        ObservableCollection<MealPlanEntry_c> DinnerItems = new ObservableCollection<MealPlanEntry_c>();
        ObservableCollection<MealPlanEntry_c> SnackItems = new ObservableCollection<MealPlanEntry_c>();

        public MealPlan_c()
        {
            BreakfastItems.CollectionChanged += ItemSet_CollectionChanged;
            LunchItems.CollectionChanged += ItemSet_CollectionChanged;
            DinnerItems.CollectionChanged += ItemSet_CollectionChanged;
            SnackItems.CollectionChanged += ItemSet_CollectionChanged;
        }

        private void ItemSet_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach(MealPlanEntry_c item in e.NewItems)
                {
                    //if(item.ID < 0)
                        //Add the MealPlanEntry to the DataBase. Have the DB manage itself

                    
                }
            }

            throw new NotImplementedException();
        }
    }

    public class MealPlanEntry_c
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public int MealPlanID { get; set; }

        public int RecipeID { get; set; }

        public Recipe_c Recipe { get; set; }

        public MealPlanEntryTime_e MealPlanEntryTime { get; set; }
    }

	/// <summary>
	/// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
	/// </summary>
	public sealed partial class MainPage : Page
	{
        public MainPage()
		{
			this.InitializeComponent();

            masterList.dataThing.Add("Billy");
            masterList.dataThing.Add("Mike");
            masterList.dataThing.Add("Justin");
            masterList.dataThing.Add("Alex");
        }

        // Move Stuff
        Collection<string> moveSource = null;
        Collection<string> moveDestination = null;
        List<string> movingItems = new List<string>();

        private void ListView_Drop(object sender, DragEventArgs e)
        {
            moveDestination = ((ListView)sender).DataContext as Collection<string>;
            foreach (var x in movingItems)
                moveDestination.Add(x);

            if(e.AcceptedOperation == DataPackageOperation.Move)
                foreach (var x in movingItems)
                    moveSource.Remove(x);

            e.Handled = true;
        }

        private void ListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            movingItems.Clear();
            moveSource = ((ListView)sender).DataContext as Collection<string>;
            foreach (var x in e.Items)
            {
                movingItems.Add(x as string);
                Debug.WriteLine(x);
            }
        }

        private void ListView_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = e.Modifiers.HasFlag(Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Control) ? DataPackageOperation.Copy : DataPackageOperation.Move;
        }
    }
}
