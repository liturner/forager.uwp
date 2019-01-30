using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forager
{
	public class RecipeIngredient_c : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public RecipeIngredient_c()
        {
            Required = true;
        }

        [AutoIncrement, PrimaryKey]
        public int ID { get; set; }

        private int _ingredientID;
        public int IngredientID
        {
            get { return _ingredientID; }
            set
            {
                _ingredientID = value;
                NotifyPropertyChanged("IngredientID");
            }
        }

        private Ingredient_c _ingredient;
        [Ignore]
        public Ingredient_c Ingredient
        {
            get { return _ingredient; }
            set
            {
                _ingredient = value;
                _ingredientID = _ingredient.ID;
                NotifyPropertyChanged("Ingredient");
                NotifyPropertyChanged("Name");
            }
        }

        public int RecipeID { get; set; } = -1;

        private bool _required;
        public bool Required
        {
            get { return _required; }
            set
            {
                _required = value;
                NotifyPropertyChanged("Required");
            }
        }

        private double _quantity;        
        public double Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                NotifyPropertyChanged("Quantity");
            }
        }

        private string _unit;
        public string Unit
        {
            get { return _unit; }
            set
            {
                _unit = value;
                NotifyPropertyChanged("Unit");
            }
        }

        // DO NOT USE! ONLY FOR DB AND UPDATING ISSUES
        private string _notes;
        public string Notes
        {
            get
            {
                return _notes;
            }
            set
            {
                _notes = value;
                NotifyPropertyChanged("Notes");
            }
        }
        
        [Ignore]
        public string Name
        {
            get
            {
                return Required ? Ingredient.Name : "(opt) " + Ingredient.Name;
            }
        }

        /* This did not quite work. When changing the field, the UI did not instantly update
        private bool _required;
        public bool Required
        {
            get
            {
                return _required;
            }
            set
            {
                _required = value;
                NotifyPropertyChanged("Required");
            }
        }
        */
    }
}
