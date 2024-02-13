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
    public enum BlockDirection { BD_HORIZ, BD_VERT }

    public class Block
    {
        //координаты черной клетки:
        public int blackX;
        public int blackY;
        //направление:
        public BlockDirection dir = BlockDirection.BD_HORIZ;
        //число белых клеток:
        public int nWhite;
        //сумма:
        public int sum = 0;
        //числа:
        public HashSet<int> setNums;
        //конструктор:
        public Block(int x, int y, BlockDirection bd, int sum)
        {
            blackX = x;
            blackY = y;
            dir = bd;
            this.sum = sum;
            setNums = new HashSet<int>();
        }
        //смещение клеток:
        public int getDx()
        {
            return dir == BlockDirection.BD_HORIZ ? 1 : 0;
        }
        public int getDy()
        {
            return dir == BlockDirection.BD_HORIZ ? 0 : 1;
        }
    }
}

