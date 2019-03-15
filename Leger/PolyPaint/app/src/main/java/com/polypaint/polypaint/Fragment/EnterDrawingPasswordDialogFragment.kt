package com.polypaint.polypaint.Fragment

import android.app.Dialog
import android.content.DialogInterface
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.Button
import android.widget.EditText
import androidx.appcompat.app.AlertDialog
import androidx.fragment.app.DialogFragment
import com.polypaint.polypaint.R
import kotlinx.android.synthetic.main.dialog_enter_drawing.*

class EnterDrawingPasswordDialogFragment : DialogFragment(){
    var password : String? = ""
    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        password = arguments?.getString("password")
        Log.d("***", password)

        return activity?.let {
            // Use the Builder class for convenient dialog construction
            val builder = AlertDialog.Builder(it)
            val view: View = it.layoutInflater.inflate(R.layout.dialog_enter_drawing,null)

            val passwordText : EditText = view.findViewById(R.id.password_text)
            val passwordButton : Button = view.findViewById(R.id.password_button)
            passwordButton.setOnClickListener {
                validatePassword(passwordText)
            }
            builder.setView(view)
                .setPositiveButton("Close",
                    DialogInterface.OnClickListener { dialog, id ->

                    })
            // Create the AlertDialog object and return it
            builder.create()
        } ?: throw IllegalStateException("Activity cannot be null")
    }
    private fun validatePassword(passwordText : EditText){
        if(passwordText!!.text.toString().trim() == password){
            passwordText!!.text.clear()
            //TODO : JOIN CANEVAS HERE
        }else{
            passwordText!!.text.clear()
            passwordText!!.error = "Wrong password, try again"
        }
    }
}