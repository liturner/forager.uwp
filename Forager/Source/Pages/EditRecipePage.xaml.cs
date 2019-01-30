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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Forager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditRecipePage : Page
    {
        private App p_App = null;

        public EditRecipePage()
        {
            p_App = (Forager.App)App.Current;
            this.InitializeComponent();
        }

        private async void btnAddRecipeIngredient_Click(object sender, RoutedEventArgs e)
        {
            RecipeIngredientDialog l_recipeIngredientDialog = new RecipeIngredientDialog(p_App.DataBase);
            l_recipeIngredientDialog.PrimaryButtonClick += RecipeIngredientDialog_PrimaryButtonClick;
            await l_recipeIngredientDialog.ShowAsync();
        }

        private void RecipeIngredientDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            RecipeIngredientDialog l_recipeIngredientDialog = (RecipeIngredientDialog)sender;
            Recipe_c selectedRecipe = p_App.RootPage.SelectedRecipe;

            p_App.DataBase.AddRecipeIngredientToRecipe(selectedRecipe, l_recipeIngredientDialog.RecipeIngredient);
        }

        private async void btnEditRecipeIngredient_Click(object sender, RoutedEventArgs e)
        {
            RecipeIngredientDialog ingredientDialog = new RecipeIngredientDialog(p_App.DataBase, (RecipeIngredient_c)recipeIngredientList.SelectedItem);
            ingredientDialog.PrimaryButtonClick += EditRecipeIngredientDialog_PrimaryButtonClick;
            await ingredientDialog.ShowAsync();
        }

        private void EditRecipeIngredientDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //p_App.DataBase.((RecipeIngredientDialog)sender).RecipeIngredient
            //CookBook selectedCookbook = (CookBook)bookColumn.SelectedItem;
        }

        private async void DeleteRecipeIngredient_Click(object sender, RoutedEventArgs e)
        {
            Recipe_c p_recipe = (Recipe_c)DataContext;
            RecipeIngredient_c p_selectedIngredient = recipeIngredientList.SelectedItem as RecipeIngredient_c;
            DeleteConfirmDialog l_deleteConfirmDialog = new DeleteConfirmDialog();
            l_deleteConfirmDialog.Content = "Are you sure you want to remove '" + p_selectedIngredient.Name + "' from the '" + p_recipe.Name + "' recipe?";
            l_deleteConfirmDialog.PrimaryButtonClick += DeleteConfirmDialog_PrimaryButtonClick;
            await l_deleteConfirmDialog.ShowAsync();
        }

        private void DeleteConfirmDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Recipe_c p_recipe = (Recipe_c)DataContext;
            foreach (RecipeIngredient_c selectedRecipeIngredient in recipeIngredientList.SelectedItems)
            {
                p_App.DataBase.RemoveRecipeIngredientFromRecipe(p_recipe, selectedRecipeIngredient);
            }      
        }

        private void recipeIngredientList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(recipeIngredientList.SelectedItem != null)
            {
                btnEditIngredient.Visibility = Visibility.Visible;
                btnDeleteIngredient.Visibility = Visibility.Visible;
            }
            else
            {
                btnEditIngredient.Visibility = Visibility.Collapsed;
                btnDeleteIngredient.Visibility = Visibility.Collapsed;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Recipe_c dataSource = DataContext as Recipe_c;

            rebDirections.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, dataSource.Directions);
        }

        private void SaveDirections()
        {
            Recipe_c dataSource = DataContext as Recipe_c;
            string outText = "";

            rebDirections.Document.GetText(Windows.UI.Text.TextGetOptions.FormatRtf, out outText);

            dataSource.Directions = outText;
        }

        private void rebDirections_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveDirections();
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.UI.Text.ITextSelection selectedText = rebDirections.Document.Selection;
            if (selectedText != null)
            {
                Windows.UI.Text.ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Bold = Windows.UI.Text.FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }

            SaveDirections();
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.UI.Text.ITextSelection selectedText = rebDirections.Document.Selection;
            if (selectedText != null)
            {
                Windows.UI.Text.ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Italic = Windows.UI.Text.FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }

            SaveDirections();
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.UI.Text.ITextSelection selectedText = rebDirections.Document.Selection;
            if (selectedText != null)
            {
                Windows.UI.Text.ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                if (charFormatting.Underline == Windows.UI.Text.UnderlineType.None)
                {
                    charFormatting.Underline = Windows.UI.Text.UnderlineType.Single;
                }
                else
                {
                    charFormatting.Underline = Windows.UI.Text.UnderlineType.None;
                }
                selectedText.CharacterFormat = charFormatting;
            }

            SaveDirections();
        }
    }
}
