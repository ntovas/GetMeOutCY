using System;
using Android.App;
using Android.Content;
using Android.OS;

namespace GetMeOut
{
	class SendSmsService : Service
	{
		public override IBinder OnBind(Intent intent)
		{
			throw new NotImplementedException();
		}
	}
}