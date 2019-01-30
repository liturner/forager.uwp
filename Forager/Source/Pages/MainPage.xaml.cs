using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Forager
{
    public class FilterOptions_c
    {
        public enum DisplayFilter_e
        {
            All = 0,
            HaveIngredients,
            MissingAnIngredient
        }

    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Variables
        private FilterOptions_c m_FilterOptions = new FilterOptions_c();
        private App p_App = null;

        public CookBook_c SelectedCookBook { get { return (CookBook_c)bookColumn.SelectedItem; } }
        public Recipe_c SelectedRecipe { get { return (Recipe_c)recipeColumn.SelectedItem; } }

        public MainPage()
        {
            p_App = (Forager.App)App.Current;
            p_App.RootPage = this;
            this.InitializeComponent();

            // Events
            Application.Current.Suspending += Current_Suspending;
            Application.Current.Resuming += Current_Resuming;

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size { Width = 1010, Height = 400 });
            ApplicationView.PreferredLaunchViewSize = new Size { Height = 550, Width = 1010 };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        private void Current_Resuming(object sender, object e)
        {

        }

        private void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            // Get the ingredients selected
            if (p_App.DataBase.AvailableIngredients.Count > 0)
            {
                int[] selectedIngredients = new int[p_App.DataBase.AvailableIngredients.Count];
                for (int i = 0; i < p_App.DataBase.AvailableIngredients.Count; i++)
                {
                    selectedIngredients[i] = p_App.DataBase.AvailableIngredients[i].ID;
                }
                //ApplicationData.Current.RoamingSettings.Values["currentPantry"] = selectedIngredients;
            }
        }

        private void evt_IngredientsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the ingredients selected
            p_App.DataBase.AvailableIngredients.Clear();

            foreach (Ingredient_c selectedIngredient in ingredientList.SelectedItems)
            {
                p_App.DataBase.AvailableIngredients.Add(selectedIngredient);
            }
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = p_App.DataBase;

            // This will ensure that the filter is correctly set on first load
            foreach (CookBook_c cookBook in p_App.DataBase.CookBooks)
            {
                cookBook.DisplayFilter = (FilterOptions_c.DisplayFilter_e)globalFilter.SelectedIndex;
            }
            
            pantry.Source = p_App.DataBase.IngredientsGrouped;
            (ingredientListZoom.ZoomedOutView as ListViewBase).ItemsSource = pantry.View.CollectionGroups;

            mainFrame.Navigate(typeof(LandingPage));

            // Load in previous state
            /*int[] selectedIngredients = (int[])ApplicationData.Current.RoamingSettings.Values["currentPantry"];
            if (selectedIngredients != null)
            {
                foreach (int l_ingredientId in selectedIngredients)
                {
                    ingredientList.SelectedItems.Add(Ingredients.Get(l_ingredientId));
                }
            }*/




            printHelper.RegisterForPrinting();
        }


        Tools.PrintHelper printHelper = new Tools.PrintHelper();

        private async void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintRecipePage p2p = new PrintRecipePage();
            p2p.DataContext = detailColumn.DataContext;
            p2p.Initialise();

            printHelper.PageToPrint = p2p;

            await printHelper.ShowPrintUIAsync();
        }













        // Library code


        private void recipeColumn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //foreach(Recipe deselectedRecipe in e.RemovedItems)
            //    deselectedRecipe.PropertyChanged -= SelectedRecipe_PropertyChanged;

            object selectedItem = null;
            foreach (var item in e.AddedItems)
            {
                selectedItem = item;
                // selectedRecipe.PropertyChanged += SelectedRecipe_PropertyChanged;
            }

            if (selectedItem == null)
            {
                detailColumn.Content = null;

                btnAddToMealPlan.IsEnabled = false;
                btnPrint.IsEnabled = false;
                tglEdit.IsEnabled = false;
                btnDeleteRecipe.IsEnabled = false;
            }
            else if (selectedItem.GetType() == typeof(Recipe_c))
            {
                detailColumn.Navigate(typeof(ViewRecipePage));

                btnAddToMealPlan.IsEnabled = true;
                btnPrint.IsEnabled = true;
                tglEdit.IsEnabled = true;
                btnDeleteRecipe.IsEnabled = true;
            }
            else if (selectedItem.GetType() == typeof(MealPlan_c))
                detailColumn.Navigate(typeof(MealPlannerPage));

            detailColumn.DataContext = selectedItem;
        }

        //private void SelectedRecipe_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    Recipe p_updatedRecipe = (Recipe)sender;
        //    p_App.m_database.

        //    throw new NotImplementedException();
        //}

        private async void AddCookBook_Click(object sender, RoutedEventArgs e)
        {
            CookBookDialog newCookBookDialog = new CookBookDialog();
            newCookBookDialog.PrimaryButtonClick += NewCookBookDialog_PrimaryButtonClick;
            await newCookBookDialog.ShowAsync();
        }

        private void NewCookBookDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            p_App.DataBase.CookBooks.Add(((CookBookDialog)sender).CookBook);
        }

        private void DeleteCookBook_Click(object sender, RoutedEventArgs e)
        {
            if (bookColumn.SelectedItem != null)
            {
                CookBook_c bookToBurn = (CookBook_c)bookColumn.SelectedItem;
                p_App.DataBase.CookBooks.Remove(bookToBurn);
            }
        }





        private async void CreateRecipe_Click(object sender, RoutedEventArgs e)
        {
            RecipeDialog newRecipeDialog = new RecipeDialog();
            newRecipeDialog.PrimaryButtonClick += NewRecipeDialog_PrimaryButtonClick;

            try
            {
                await newRecipeDialog.ShowAsync();
            }
            catch (Exception _exception)
            {
                // Log this. For some reason I hit a lot of exceptions here???
            }
        }

        private void NewRecipeDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            CookBook_c selectedCookbook = (CookBook_c)bookColumn.SelectedItem;
            RecipeDialog recipeDialog = (RecipeDialog)sender;

            Recipe_c newRecipe = new Recipe_c();
            newRecipe.Name = recipeDialog.RecipeTitle;
            p_App.DataBase.AddRecipeToCookBook(selectedCookbook, newRecipe);

            // Trigger a refresh of the cokbook to ensure the book is shown
            // selectedCookbook.AvailableIngredients_CollectionChanged(this, null);
        }





        private void DeleteRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (recipeColumn.SelectedItem != null)
            {
                CookBook_c selectedCookbook = (CookBook_c)bookColumn.SelectedItem;
                selectedCookbook.Recipes.Remove((Recipe_c)recipeColumn.SelectedItem);
            }
        }

        private void AppBarToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            detailColumn.Navigate(typeof(EditRecipePage));
        }

        private void AppBarToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            detailColumn.Navigate(typeof(ViewRecipePage));
            CookBook_c selectedCookbook = (CookBook_c)bookColumn.SelectedItem;
            //selectedCookbook.UpdateAvailableRecipes(this, new IngredientsUpdatedEventArgs(p_App.DataBase, m_FilterOptions));
        }


        private async void DeleteIngredient_Click(object sender, RoutedEventArgs e)
        {
            DeleteConfirmDialog l_deleteConfirmDialog = new DeleteConfirmDialog();
            l_deleteConfirmDialog.PrimaryButtonClick += DeleteConfirmDialog_PrimaryButtonClick;
            await l_deleteConfirmDialog.ShowAsync();
        }

        private void DeleteConfirmDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            foreach (Ingredient_c selectedIngredient in ingredientList.SelectedItems)
            {
                p_App.DataBase.Ingredients.Remove(selectedIngredient);
            }

        }














        private void globalFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (CookBook_c cookBook in p_App.DataBase.CookBooks)
            {
                cookBook.DisplayFilter = (FilterOptions_c.DisplayFilter_e)globalFilter.SelectedIndex;
            }
        }







        private async void CreateIngredient_Click(object sender, RoutedEventArgs e)
        {
            IngredientDialog l_ingredientDialog = new IngredientDialog();
            l_ingredientDialog.PrimaryButtonClick += IngredientDialog_PrimaryButtonClick;
            try
            {
                await l_ingredientDialog.ShowAsync();
            }
            catch (Exception _exception)
            {
                // ToDo: Add logging here because its often throwing an issue
            }
        }

        private void IngredientDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            IngredientDialog l_ingredientDialog = (IngredientDialog)sender;
            p_App.DataBase.Ingredients.Add(l_ingredientDialog.Ingredient);
        }

        private void TogglePaneButton_Click(object sender, RoutedEventArgs e)
        {
            mainSplitView.IsPaneOpen = !mainSplitView.IsPaneOpen;
        }

        private void IHave_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mainFrame.Visibility = Visibility.Collapsed;
            exploreFrame.Visibility = Visibility.Visible;

            // Clear the selected items and prepare the UI for switch to the Pantry mode
            library.DataContext = null;
            recipeColumn.DataContext = null;
            bookColumn.SelectedItem = null;

            // Display the Pantry
            recipeColumn.Visibility = Visibility.Collapsed;
            ingredientListZoom.Visibility = Visibility.Visible;
            searchAndFilterRow.Visibility = Visibility.Collapsed;
            bookColumn.Visibility = Visibility.Collapsed;

            btnCreateIngredient.Visibility = Visibility.Visible;
            btnDeleteIngredient.Visibility = Visibility.Visible;
            btnCreateRecipe.Visibility = Visibility.Collapsed;
            btnDeleteMealPlan.Visibility = Visibility.Collapsed;
            btnCreateMealPlan.Visibility = Visibility.Collapsed;
        }

        private void IWant_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Display the Recipe list
            library.DataContext = null;
            recipeColumn.DataContext = p_App.DataBase.Recipes;
            bookColumn.SelectedItem = null;

            mainFrame.Visibility = Visibility.Collapsed;
            exploreFrame.Visibility = Visibility.Visible;

            recipeColumn.Visibility = Visibility.Visible;
            ingredientListZoom.Visibility = Visibility.Collapsed;
            searchAndFilterRow.Visibility = Visibility.Visible;
            bookColumn.Visibility = Visibility.Visible;

            btnCreateIngredient.Visibility = Visibility.Collapsed;
            btnDeleteIngredient.Visibility = Visibility.Collapsed;
            btnCreateRecipe.Visibility = Visibility.Collapsed;
            btnDeleteMealPlan.Visibility = Visibility.Collapsed;
            btnCreateMealPlan.Visibility = Visibility.Collapsed;

            mainSplitView.IsPaneOpen = true;
        }

        private void ShowSettings_Click(object sender, RoutedEventArgs e)
        {
            MainNavigation.SelectedItem = null;

            mainFrame.Navigate(typeof(SettingsPage));
            mainFrame.Visibility = Visibility.Visible;
            exploreFrame.Visibility = Visibility.Collapsed;
        }

        private void bookColumn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bookColumn.SelectedItem != null)
            {
                library.DataContext = (CookBook_c)bookColumn.SelectedItem;
                recipeColumn.DataContext = ((CookBook_c)bookColumn.SelectedItem).DisplayedRecipes;
                btnCreateRecipe.Visibility = Visibility.Visible;

                if (mainSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
                    mainSplitView.IsPaneOpen = false;
            }
            else
            {
                btnCreateRecipe.Visibility = Visibility.Collapsed;
            }
        }

        private void DayPlanner_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Clean up from settings
            mainFrame.Visibility = Visibility.Collapsed;
            exploreFrame.Visibility = Visibility.Visible;

            // Display the Recipe list
            recipeColumn.Visibility = Visibility.Visible;
            ingredientListZoom.Visibility = Visibility.Collapsed;
            searchAndFilterRow.Visibility = Visibility.Collapsed;
            bookColumn.Visibility = Visibility.Collapsed;

            btnCreateIngredient.Visibility = Visibility.Collapsed;
            btnDeleteIngredient.Visibility = Visibility.Collapsed;
            btnCreateRecipe.Visibility = Visibility.Collapsed;
            btnDeleteMealPlan.Visibility = Visibility.Visible;
            btnCreateMealPlan.Visibility = Visibility.Visible;

            recipeColumn.DataContext = p_App.DataBase.MealPlans;
            detailColumn.Content = null;
        }

        private void DeleteMealPlan_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void CreateMealPlan_Click(object sender, RoutedEventArgs e)
        {
            MealPlanDialog l_mealPlanDialog = new MealPlanDialog(p_App.DataBase);
            l_mealPlanDialog.PrimaryButtonClick += MealPlantDialog_PrimaryButtonClick;
            await l_mealPlanDialog.ShowAsync();
        }

        private void MealPlantDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            MealPlanDialog l_dialog = (MealPlanDialog)sender;
            p_App.DataBase.MealPlans.Add(l_dialog.MealPlan);
        }

        private async void AddToMealPlanner_Click(object sender, RoutedEventArgs e)
        {
            AddToMealPlan l_addToMealPlanDialog = new AddToMealPlan();
            await l_addToMealPlanDialog.ShowAsync();

            p_App.DataBase.MealPlannerTemp.Add(recipeColumn.SelectedItem as Recipe_c);
        }

        private void MainNavigation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clean up the common elements of the UI
            //MealPlannerCommandBar.Visibility = Visibility.Collapsed;
            //RecipeCommandBar.Visibility = Visibility.Collapsed;

            // This normally happens when navigating to Settings for example
            if (MainNavigation.SelectedItem == null)
            {
                bookColumn.Visibility = Visibility.Collapsed;
            }
            // Ingredients
            else if(MainNavigation.SelectedIndex == 0)
            {

            }
            // Meal Planner
            else if(MainNavigation.SelectedIndex == 1)
            {
                //MealPlannerCommandBar.Visibility = Visibility.Visible;
            }
            // Recipes
            else if (MainNavigation.SelectedIndex == 2)
            {
                //RecipeCommandBar.Visibility = Visibility.Visible;
            }
        }

        private void RecipeCommandBar_LayoutUpdated(object sender, object e)
        {

        }
    }
}
