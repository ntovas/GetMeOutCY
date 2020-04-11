using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SMShadow.Helpers;
using AlertDialog = Android.App.AlertDialog;
using Exception = System.Exception;

namespace GetMeOut
{
	[Activity(Label = "@string/app_name",
		Theme = "@style/AppTheme.NoActionBar",
		MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		private EditText edtSms;
		private ContentValues sms;
		private SmsHelper smsHelper;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			SetContentView(Resource.Layout.activity_main);



			var btnChange = FindViewById<Button>(Resource.Id.change);
			btnChange.Click += (sender, args) => ChangeDefault();

			var btnGet = FindViewById<Button>(Resource.Id.btn_get);
			btnGet.Click += (sender, args) => GetLastMessage();

			var btnSave = FindViewById<Button>(Resource.Id.btn_save);
			btnSave.Click += (sender, args) => CreateNewMessage();
			btnSave.Enabled = false;
			

			edtSms = FindViewById<EditText>(Resource.Id.sms_body);
			edtSms.TextChanged += (sender, args) => { btnSave.Enabled = args.AfterCount > 0; };

			smsHelper = new SmsHelper(this.ApplicationContext);
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

		private void ChangeDefault()
		{
			var intent = new Intent(Telephony.Sms.Intents.ActionChangeDefault);
			
			StartActivity(intent);
		}

		private void GetLastMessage()
		{
			try
			{
				sms = smsHelper.GetLast();

				edtSms.Text = sms.GetAsString("body");
			}
			catch (Exception e)
			{
				ShowDialogForDefault();
			}
		}

		private void CreateNewMessage()
		{
			try
			{
				sms.Remove("body");
				sms.Put("body", edtSms.Text);
				smsHelper.InsertMessages(sms);
				edtSms.Text = string.Empty;
				ShowSuccessDialog();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

		}


		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		private void ShowDialogForDefault()
		{
			var dialog = new AlertDialog.Builder(this);
			var alert = dialog.Create();
			alert.SetTitle("Error");
			alert.SetMessage("The app is not default sms app or you have sent no sms to 8998");
			alert.SetButton("OK", (c, ev) =>
			{
				alert.Hide();
			});
			alert.Show();
		}

		private void ShowSuccessDialog()
		{
			var dialog = new AlertDialog.Builder(this);
			var alert = dialog.Create();
			alert.SetTitle("Success");
			alert.SetMessage("You are free to go ;) \r\n Change back the app to default sms app.");
			alert.SetButton("OK", (c, ev) =>
			{
				ChangeDefault();
				alert.Hide();
			});
			alert.Show();
		}
	}
}

