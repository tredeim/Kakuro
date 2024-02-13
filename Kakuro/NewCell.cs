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
    public class NewCell
    {
        // заданное сразу число:
        public bool fix;
        // число:
        public int num;
        // список блоков:
        public List<Block> lstBlocks;
        public bool visited;
        //цвет ячейки
        public bool color;
        // конструкторы:

        public NewCell(bool color)
        {
            this.color = color;
            lstBlocks = new List<Block>();
        }
        public NewCell(int num, bool fix, bool color)
        {
            this.num = num;
            this.fix = fix;
            lstBlocks = new List<Block>();
            visited = false;
            this.color = color;
        }
    }
}