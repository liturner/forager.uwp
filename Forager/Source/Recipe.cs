using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forager
{
    public class Recipe_c
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

		[AutoIncrement, PrimaryKey]
		public int ID { get; set; } = -1;

        private string m_name = "";
        public string Name
        {
            get { return m_name; }
            set
            {
                m_name = value;
                NotifyPropertyChanged("Name");
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

        private string m_mainImage;
        public string MainImage
        {
            get { return m_mainImage; }
            set
            {
                m_mainImage = value;
                NotifyPropertyChanged("MainImage");
            }
        }

        private string m_description = "";
        public string Description
        {
            get { return m_description; }
            set
            {
                m_description = value;
                NotifyPropertyChanged("Description");
            }
        }

        private string m_directions = "";
        public string Directions
        {
            get { return m_directions; }
            set
            {
                m_directions = value;
                NotifyPropertyChanged("Directions");
            }
        }

        [Ignore]
        public string ServingsText
        {
            get
            {
                return "Serves " + _quantity.ToString() + " " + _unit;
            }
        }

        [Ignore]
        public ObservableCollection<RecipeIngredient_c> RecipeIngredients { get; set; }

        [Ignore]
        public List<string> Tags { get; set; }

        [Ignore]
        public string Colour { get; set; }

        [Ignore]
        public List<Ingredient_c> RequiredIngredients
        {
            get
            {
                return new List<Ingredient_c>( RecipeIngredients.Where(recipeIngredient => recipeIngredient.Required == true).Select(recipIngredient => recipIngredient.Ingredient) );
            }
        }

        public Recipe_c()
        {
            RecipeIngredients = new ObservableCollection<RecipeIngredient_c>();
        }
    }
}
