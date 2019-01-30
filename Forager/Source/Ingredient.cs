using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using SQLite.Net.Attributes;

namespace Forager
{
    public class Ingredient_c
    {
        [AutoIncrement, PrimaryKey]
        public int ID { get; set; }

        public string Name { get; set; }
        /*{
            get
            {
                return _names[0].String;
            }
        }*/

        //public List<Tools.I18nString> _names { get; set; }

        public int UsageCount = 0;

        public Ingredient_c()
        {
        }

        public Ingredient_c(int _id, string _name, Color _colour)
        {
            //_names = new List<Tools.I18nString>();
            //_names.Add(new Tools.I18nString(_name));
            Name = _name;
            ID = _id;   
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
