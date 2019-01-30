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

namespace Forager
{
	/// <summary>
	/// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
	/// </summary>
	public sealed partial class MealPlannerPage : Page
	{
        public MealPlannerPage()
		{
			this.InitializeComponent();
        }

        // Move Stuff
        object moveSource = null;
        object moveDestination = null;
        List<object> movingItems = new List<object>();
        bool moveAllowed = false;

        private void ListView_Drop(object sender, DragEventArgs e)
        {
            moveDestination = ((ListViewBase)sender).ItemsSource;

            foreach (var item in movingItems)
            {
                // This catches the case where we need to make a meal planner instance
                // which references the Recipe
                MealPlanEntry_c tempMealPlanEntry = null;

                if (item.GetType() == typeof(Recipe_c))
                {
                    tempMealPlanEntry = new MealPlanEntry_c(item as Recipe_c);
                    tempMealPlanEntry.MealPlanID = ((MealPlan_c)DataContext).ID;
                }
                else if (item.GetType() == typeof(MealPlanEntry_c))
                {
                    // Take care about the difference between opy and move.
                    // Copy must create a new instance of MealPlanEntry
                    // Move just changes ids and values
                    if (e.AcceptedOperation == DataPackageOperation.Move)
                    {
                        tempMealPlanEntry = item as MealPlanEntry_c;
                        tempMealPlanEntry.MealPlanID = ((MealPlan_c)DataContext).ID;
                    }
                    else if (e.AcceptedOperation == DataPackageOperation.Copy)
                    {
                        tempMealPlanEntry = new MealPlanEntry_c(item as MealPlanEntry_c);
                        tempMealPlanEntry.MealPlanID = ((MealPlan_c)DataContext).ID;
                    }
                }

                // Keep the entry type enumeration up to date


                ((ObservableCollection<MealPlanEntry_c>)moveDestination).Add(tempMealPlanEntry);
            }

            if (e.AcceptedOperation == DataPackageOperation.Move)
            {
                foreach (MealPlanEntry_c item in movingItems)
                {
                     ((ObservableCollection<MealPlanEntry_c>)moveSource).Remove(item);
                }
            }

            e.Handled = true;
        }

        private void ListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            movingItems.Clear();
            moveSource = ((ListViewBase)sender).ItemsSource;

            foreach (var item in e.Items)
            {
                movingItems.Add(item);
                Debug.WriteLine(item);
            }

            moveAllowed = moveSource.GetType() == typeof(ObservableCollection<MealPlanEntry_c>);
        }

        private void ListView_DragOver(object sender, DragEventArgs e)
        {
            if (moveAllowed)
                e.AcceptedOperation = e.Modifiers.HasFlag(Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Control) ? DataPackageOperation.Copy : DataPackageOperation.Move;
            else
                e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            masterList.DataContext = ((App)App.Current).DataBase.MealPlannerTemp;
        }
    }
}
