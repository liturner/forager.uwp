using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace PantryEditor
{
    namespace DataBaseModel
    {
		public class Recipe
		{
			[SQLite.Net.Attributes.PrimaryKey, SQLite.Net.Attributes.AutoIncrement]
			public int Id { get; set; }

			public int[] Ingredient_ids { get; set; }
		}

		public class Ingredient
		{

			public Ingredient()
			{
			}

			public Ingredient(string _name, Color _colour)
			{
				Name = _name;
			}

			[SQLite.Net.Attributes.PrimaryKey, SQLite.Net.Attributes.AutoIncrement]
			public int Id { get; set; }

			public string Name { get; set; }

			public int UsageCount { get; set; }

			//public Windows.UI.Color IngredientColour { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
