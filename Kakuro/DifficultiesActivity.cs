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
    [Activity(Label = "Выбрать сложность")]
    public class DifficultiesActivity : Activity
    {
        Button easyButton, normalButton, hardButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            // Create your application here
            SetContentView(Resource.Layout.DifficultiesPage);

            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            easyButton = FindViewById<Button>(Resource.Id.easyButton);
            normalButton = FindViewById<Button>(Resource.Id.normalButton);
            hardButton = FindViewById<Button>(Resource.Id.hardButton);


            easyButton.Click += delegate (object sender, EventArgs e)
            {
                var intent = new Intent(this, typeof(GridActivity));
                intent.PutExtra("difficulty", "easy");
                this.StartActivity(intent);
            };

            normalButton.Click += delegate (object sender, EventArgs e)
            {
                var intent = new Intent(this, typeof(GridActivity));
                intent.PutExtra("difficulty", "normal");
                this.StartActivity(intent);
            };

            hardButton.Click += delegate (object sender, EventArgs e)
            {
                var intent = new Intent(this, typeof(GridActivity));
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