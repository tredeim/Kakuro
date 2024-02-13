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
    public class RecordsActivity : Activity
    {
        Button easyRecordsButton, normalRecordsButton, hardRecordsButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.RecordsPage);

            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            easyRecordsButton = FindViewById<Button>(2131230790);
            normalRecordsButton = FindViewById<Button>(2131230840);
            hardRecordsButton = FindViewById<Button>(2131230809);

            easyRecordsButton.Click += delegate (object sender, EventArgs e)
            {
                var intent = new Intent(this, typeof(BaseOfRecordsActivity));
                intent.PutExtra("difficulty", "easy");
                this.StartActivity(intent);
            };

            normalRecordsButton.Click += delegate (object sender, EventArgs e)
            {
                var intent = new Intent(this, typeof(BaseOfRecordsActivity));
                intent.PutExtra("difficulty", "normal");
                this.StartActivity(intent);
            };

            hardRecordsButton.Click += delegate (object sender, EventArgs e)
            {
                var intent = new Intent(this, typeof(BaseOfRecordsActivity));
                intent.PutExtra("difficulty", "hard");
                this.StartActivity(intent);
            };

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