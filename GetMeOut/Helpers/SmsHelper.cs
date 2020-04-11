using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Database;
using Uri = Android.Net.Uri;

namespace GetMeOut.Helpers
{
	public class SmsHelper
	{
		private Context _context;

		private string responseTemplate =
			"EΓKPINETAI [DATA] IΣXYEI ΓIA EYΛOΓO XPONIKO ΔIAΣTHMA. APPROVED [DATA] VALID FOR REASONABLE AMOUNT OF TIME.";

		public SmsHelper(Context context)
		{
			_context = context;
		}

		private List<ContentValues> GetMessages(string source)
		{
			var list = new List<ContentValues>();
			Uri uri = Uri.Parse("content://sms/"+source);
			var cursor = _context.ContentResolver.Query(uri, null, null, null, null);

			while (cursor.MoveToNext())
			{
				var map = new ContentValues();
				DatabaseUtils.CursorRowToContentValues(cursor, map);

				list.Add(map);

			}

			return list;

		}

		private List<ContentValues> GetSentMessages()
		{
			return GetMessages("sent");
		}

		private List<ContentValues> GetInboxMessages()
		{
			return GetMessages("inbox");
		}

		public ContentValues GetLast()
		{
			var list = GetSentMessages();

			var lastmsg = list
				.Where(c => c.GetAsString("address") == "8998")
				.OrderByDescending(c => c.GetAsLong("date")).FirstOrDefault();
			if (lastmsg == null)
			{
				throw new Exception("Message not found.");
			}

			var newmsg = new ContentValues(lastmsg);

			newmsg.Remove("_id");
			newmsg.Remove("deleted");
			newmsg.Remove("sync_state");
			newmsg.Remove("date");
			newmsg.Remove("date_sent");

			newmsg.Put("date", (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds);
			newmsg.Put("date_sent", (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds);

			return newmsg;
		}


		public void InsertMessages(ContentValues message)
		{
			_context.ContentResolver.Insert(Uri.Parse("content://sms/sent"), message);

			var list = GetInboxMessages();

			var lastmsg = list
				.Where(c => c.GetAsString("address") == "CYREPUBLIC")
				.OrderByDescending(c => c.GetAsLong("date")).FirstOrDefault();
			var newmsg = new ContentValues(lastmsg);
			newmsg.Remove("_id");
			newmsg.Remove("deleted");
			newmsg.Remove("sync_state");
			newmsg.Remove("date");
			newmsg.Remove("date_sent");
			newmsg.Remove("body");

			var body = responseTemplate.Replace("[DATA]", message.GetAsString("body"));

			newmsg.Put("body", body);
			newmsg.Put("date", (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds);
			newmsg.Put("date_sent", (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds);

			_context.ContentResolver.Insert(Uri.Parse("content://sms/inbox"), newmsg);
		}
	}
}
