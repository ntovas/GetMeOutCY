package gr.ntovas.getmeout

import android.content.ContentValues
import android.content.Context
import android.database.Cursor
import android.database.DatabaseUtils
import android.net.LinkAddress
import android.net.Uri
import java.lang.Exception


class SmsHelper(context: Context)
{
    var responseTemplate: String = "EΓKPINETAI [DATA] IΣXYEI ΓIA EYΛOΓO XPONIKO ΔIAΣTHMA. APPROVED [DATA] VALID FOR REASONABLE AMOUNT OF TIME."

    var context : Context = context;

    private fun getMessages(source: String, address: String): List<ContentValues?>? {
        val list = mutableListOf<ContentValues>()
        val uri: Uri = Uri.parse("content://sms/$source")
        val cursor: Cursor = context.contentResolver.query(uri,
            null, "address='"+address+"'", null, null)
        while (cursor.moveToNext()) {
            val map: ContentValues = ContentValues()
            DatabaseUtils.cursorRowToContentValues(cursor, map)
            list.add(map)
        }
        return list
    }

    private fun getSentMessages(): List<ContentValues?>? {
        return getMessages("sent", "8998")
    }

    private fun getInboxMessages(): List<ContentValues?>? {
        return getMessages("inbox", "CYREPUBLIC")
    }

    fun getLast() :ContentValues{
            val list = getSentMessages();

            val last = list
                ?.maxBy { c -> c?.getAsLong("date")!! }
                ?: throw Exception("cant find message");

            last.remove("_id");
            last.remove("deleted");
            last.remove("sync_state");
            last.remove("date");
            last.remove("date_sent");

            last.put("date", System.currentTimeMillis());
            last.put("date_sent", System.currentTimeMillis());

            return last;
    }

    fun insertSms(sms:ContentValues){
        context.contentResolver.insert(Uri.parse("content://sms/sent"), sms);

        val list = getInboxMessages();

        val last = list
            ?.maxBy { c -> c?.getAsLong("date")!! }
            ?: throw Exception("cant find message");

        last.remove("_id");
        last.remove("deleted");
        last.remove("sync_state");
        last.remove("date");
        last.remove("date_sent");
        last.remove("body")

        val body = responseTemplate.replace("[DATA]", sms.getAsString("body"))

        last.put("body", body)
        last.put("date", System.currentTimeMillis());
        last.put("date_sent", System.currentTimeMillis());
        context.contentResolver.insert(Uri.parse("content://sms/inbox"), last);
    }
}