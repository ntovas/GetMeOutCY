package gr.ntovas.getmeout

import android.app.AlertDialog
import android.content.ContentValues
import android.content.Intent
import android.os.Bundle
import android.provider.Telephony
import android.text.Editable
import android.text.TextWatcher
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.EditText
import java.lang.Exception

class FirstFragment : Fragment() {

    lateinit var sms : ContentValues;

    override fun onCreateView(
            inflater: LayoutInflater, container: ViewGroup?,
            savedInstanceState: Bundle?
    ): View? {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_first, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        val btnSave = view.findViewById<Button>(R.id.btn_save)
        btnSave.isEnabled = false;

        val edtSms = view.findViewById<EditText>(R.id.edt_sms)
        edtSms.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(p0: Editable?) {
                btnSave.isEnabled = !p0.isNullOrBlank();
            }

            override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {

            }

            override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {

            }
        })

        btnSave.setOnClickListener{
            try {
                val helper = SmsHelper(this.activity!!.applicationContext)
                sms.remove("body")
                sms.put("body", edtSms.text.toString())
                helper.insertSms(sms)

                showSuccessDialog()
            }catch (e: Exception){
                showDialogForDefault()
            }
        }

        view.findViewById<Button>(R.id.btn_getlast).setOnClickListener {
            try {
                val helper = SmsHelper(this.activity!!.applicationContext)
                sms = helper.getLast();
                edtSms.setText(sms.getAsString("body"))
            }catch (e: Exception){
                showDialogForDefault()
            }
        }

        view.findViewById<Button>(R.id.btn_change).setOnClickListener{
            changeDefault()
        }
    }

    private fun changeDefault(){
        var intent = Intent(Telephony.Sms.Intents.ACTION_CHANGE_DEFAULT);
        startActivity(intent);
    }

    private fun showDialogForDefault()
    {
        var dialog =  AlertDialog.Builder(this.activity);
        dialog.setPositiveButton("YES") { d, which ->
            d.dismiss()
        }

        var alert = dialog.create();
        alert.setTitle("Error");
        alert.setMessage("The app is not default sms app or you have sent no sms to 8998");

        alert.show();
    }

    private fun showSuccessDialog()
    {
        var dialog =  AlertDialog.Builder(this.activity);
        dialog.setPositiveButton("YES") { d, which ->
            changeDefault()
        }

        var alert = dialog.create();
        alert.setTitle("Success");
        alert.setMessage("You are free to go ;) \r\n Change back the app to default sms app.");

        alert.show();
    }
}
