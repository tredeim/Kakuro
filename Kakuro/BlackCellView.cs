using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Kakuro
{
    public class BlackCellView : View
    {
        private int horizontalValue;
        private int verticalValue;

        public BlackCellView(Context context, int horValue, int vertValue) : base(context)
        {
            horizontalValue = horValue;
            verticalValue = vertValue;
        }

        public BlackCellView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public BlackCellView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public BlackCellView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        protected BlackCellView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            var rectPaint = new Paint();
            rectPaint.Color = Color.Black;
            rectPaint.SetStyle(Paint.Style.Fill);
            canvas.DrawRect(0, 0, 100, 100, rectPaint);

            if (horizontalValue + verticalValue != 0)
            {
                var linePaint = new Paint();
                linePaint.Color = Color.Gray;
                linePaint.StrokeWidth = 2;
                rectPaint.SetStyle(Paint.Style.FillAndStroke);
                canvas.DrawLine(0, 0, 100, 100, linePaint);

                var textPaint = new Paint();
                textPaint.TextSize = 25;
                textPaint.Color = Color.White;

                if (horizontalValue != 0)
                {
                    canvas.DrawText($"{horizontalValue}", 15, 85, textPaint);
                }

                if (verticalValue != 0)
                {
                    canvas.DrawText($"{verticalValue}", 60, 35, textPaint);
                }
            }
        }
    }
}