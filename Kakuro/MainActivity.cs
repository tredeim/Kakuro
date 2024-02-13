using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace Kakuro
{
    [Activity(Label = "Kakuro", MainLauncher = true)]
    public class MainActivity : Activity
    {
        Button button1, button2, button3, button4, button5;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            button1 = FindViewById<Button>(Resource.Id.button1);
            button2 = FindViewById<Button>(Resource.Id.button2);
            button3 = FindViewById<Button>(Resource.Id.button3);
            button4 = FindViewById<Button>(Resource.Id.button4);
            button5 = FindViewById<Button>(Resource.Id.button5);

            button1.Click += delegate (object sender, EventArgs e)
            {
                var intent = new Intent(this, typeof(DifficultiesActivity));
                this.StartActivity(intent);
            };

            button2.Click += delegate (object sender, EventArgs e)
            {
                var intent = new Intent(this, typeof(RulesActivity));
                this.StartActivity(intent);
            };

            button3.Click += delegate (object sender, EventArgs e)
            {
                var intent = new Intent(this, typeof(RecordsActivity));
                this.StartActivity(intent);
            };

            button4.Click += delegate (object sender, EventArgs e)
            {
                var intent = new Intent(this, typeof(AboutProgramActivity));
                this.StartActivity(intent);
            };

            button5.Click += delegate (object sender, EventArgs e)
            {
                Finish();
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}

