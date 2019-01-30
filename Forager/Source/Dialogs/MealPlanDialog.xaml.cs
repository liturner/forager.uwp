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
    public sealed partial class MealPlanDialog : ContentDialog, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // This will act as a Pointer to the ingredients Pantry
        private Database_c m_database = null;
        public MealPlan_c MealPlan { get; private set; }

        public MealPlanDialog(Database_c _database, MealPlan_c _mealPlan = null)
        {
            this.InitializeComponent();

            IsPrimaryButtonEnabled = _mealPlan == null ? false : true;
            m_database = _database;
            MealPlan = _mealPlan == null ? new MealPlan_c() : _mealPlan;

            this.DataContext = MealPlan;

            IsPrimaryButtonEnabled = false;
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
