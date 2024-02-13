using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Kakuro
{
    public class Cell
    {
        public Cell()
        {
            Value = 0;
            HorValue = 0;
            VertValue = 0;
        }
        public int HorValue
        {
            get;
            set;
        }

        public int VertValue
        {
            get;
            set;
        }

        public int Value
        {
            get;
            set;
        }

        public bool Check
        {
            get;
            set;
        }
    }
}