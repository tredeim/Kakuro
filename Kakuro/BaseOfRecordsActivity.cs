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
    [Activity(Label = "Рекорды")]
    public class BaseOfRecordsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BaseOfRecordsPage);

            string difficulty = Intent.GetStringExtra("difficulty"); // В эту строку передается сложность

            var recordsTable = RecordsHelper.GetRecordTable();

            RecordsHelper.GameDifficulty dif = RecordsHelper.GameDifficulty.hard;

            if (difficulty == "easy")
                dif = RecordsHelper.GameDifficulty.easy;
            else if (difficulty == "medium")
                dif = RecordsHelper.GameDifficulty.medium;

            int i = 0;

            TextView tv1 = FindViewById<TextView>(2131230852);
            TextView tv2 = FindViewById<TextView>(2131230853);
            TextView tv3 = FindViewById<TextView>(2131230854);
            TextView tv4 = FindViewById<TextView>(2131230855);
            TextView tv5 = FindViewById<TextView>(2131230856);


            foreach (var record in recordsTable)
            {
                if (record.Difficulty == dif)
                {
                    i++;
                    if (i == 1)
                        tv1.Text = $"1. {record.RecordTime.Minutes}:{record.RecordTime.Seconds}";
                    if (i == 2)
                        tv2.Text = $"2. {record.RecordTime.Minutes}:{record.RecordTime.Seconds}";
                    if (i == 3)
                        tv3.Text = $"3. {record.RecordTime.Minutes}:{record.RecordTime.Seconds}";
                    if (i == 4)
                        tv4.Text = $"4. {record.RecordTime.Minutes}:{record.RecordTime.Seconds}";
                    if (i == 5)
                        tv5.Text = $"5. {record.RecordTime.Minutes}:{record.RecordTime.Seconds}";
                }
            }

            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}