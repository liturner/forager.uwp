using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public sealed partial class RecipeDialog : ContentDialog, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private string m_recipeTitle;
        public string RecipeTitle
        {
            get
            {
                return m_recipeTitle;
            }
            set
            {
                m_recipeTitle = value;
                if (this.PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("RecipeTitle"));
                }

            }
        }

        public RecipeDialog()
        {
            this.InitializeComponent();

            IsPrimaryButtonEnabled = false;

            this.DataContext = this;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void txtTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Validate the title in here
            IsPrimaryButtonEnabled = TitleValid();
        }

        private bool TitleValid()
        {
            return true;
        }


    }
}
