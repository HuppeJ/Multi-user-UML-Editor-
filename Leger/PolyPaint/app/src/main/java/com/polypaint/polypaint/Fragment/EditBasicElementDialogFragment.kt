package com.polypaint.polypaint.Fragment

import android.app.Dialog
import android.content.DialogInterface
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.EditText
import androidx.appcompat.app.AlertDialog
import androidx.fragment.app.DialogFragment
import com.polypaint.polypaint.R

class EditBasicElementDialogFragment: DialogFragment() {

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        var id = arguments?.getString("id")
        Log.d("***",id)
        return activity?.let {
            // Use the Builder class for convenient dialog construction
            val builder = AlertDialog.Builder(it)
            val view: View = it.layoutInflater.inflate(R.layout.dialog_edit_basic_element,null)
            view.findViewById<EditText>(R.id.name_text).setText(id)
            builder.setView(view)
                .setPositiveButton("Close",
                    DialogInterface.OnClickListener { dialog, id ->
                    })
            // Create the AlertDialog object and return it
            builder.create()
        } ?: throw IllegalStateException("Activity cannot be null")
    }
}
