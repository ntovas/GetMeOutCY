﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SMShadow
{
	class SendSmsService : Service
	{
		public override IBinder OnBind(Intent intent)
		{
			throw new NotImplementedException();
		}
	}
}