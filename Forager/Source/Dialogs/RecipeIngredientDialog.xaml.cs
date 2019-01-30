using System;
using System.Collections.Generic;
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

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Forager
{
    public sealed partial class RecipeIngredientDialog : ContentDialog
    {

        // This will act as a Pointer to the ingredients Pantry
        private Database_c m_database = null;
        public RecipeIngredient_c RecipeIngredient { get; private set; }

        public RecipeIngredientDialog(Database_c _database, RecipeIngredient_c _ingredient = null)
        {
            this.InitializeComponent();

            IsPrimaryButtonEnabled = _ingredient == null ? false : true;
            m_database = _database;
            RecipeIngredient = _ingredient == null ? new RecipeIngredient_c() : _ingredient;

            this.DataContext = RecipeIngredient;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_Loading(FrameworkElement sender, object args)
        {
            //detailsSection.DataContext = Enum.GetValues(typeof(Forager.RecipeIngredient_c.Quantity_c.Measure));
        }

        // https://msdn.microsoft.com/en-gb/library/windows/apps/windows.ui.xaml.controls.autosuggestbox.aspx
        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if(args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = m_database.Ingredients.Where(ingredient => ingredient.Name.ToLower().Contains(sender.Text.ToLower()));
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            Ingredient_c selectedIngredient = (Ingredient_c)args.SelectedItem;
            RecipeIngredient.Ingredient = selectedIngredient;
            IsPrimaryButtonEnabled = true;
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
                return;

            //searchSection.DataContext = ThePantry.Ingredients.Where(ingredient => ingredient.Name.ToLower().Contains(sender.Text.ToLower()));
        }
    }
}
