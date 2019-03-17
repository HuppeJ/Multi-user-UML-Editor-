package com.polypaint.polypaint.Fragment

import android.app.Dialog
import android.content.DialogInterface
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.EditText
import androidx.appcompat.app.AlertDialog
import androidx.fragment.app.DialogFragment
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.R

class EditBasicElementDialogFragment: DialogFragment() {

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        var shapeId = arguments?.getString("shapeId")
        val shape = ViewShapeHolder.getInstance().canevas.findShape(shapeId!!)

        return activity?.let {
            // Use the Builder class for convenient dialog construction
            val builder = AlertDialog.Builder(it)
            val view: View = it.layoutInflater.inflate(R.layout.dialog_edit_basic_element,null)

            val nameText : EditText = view.findViewById(R.id.name_text)
            nameText.setText(shape?.name)

            builder.setView(view)
                .setNegativeButton("Close",
                    DialogInterface.OnClickListener { dialog, id ->
                    })
            builder.setView(view)
                .setPositiveButton("Apply",
                    DialogInterface.OnClickListener { dialog, id ->
                        // TODO : l'attribut name de la shape est updaté, mais il reste à faire :
                        // TODO : - Émettre que la shape a été update
                        // TODO : - Lier la View avec les données de la shape
                        shape?.name =  nameText.text.toString()
                    })


            // Create the AlertDialog object and return it
            builder.create()
        } ?: throw IllegalStateException("Activity cannot be null")
    }
}
