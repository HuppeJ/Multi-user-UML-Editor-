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
import com.polypaint.polypaint.Model.ClassShape
import com.polypaint.polypaint.R

class EditClassDialogFragment: DialogFragment() {

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        var shapeId = arguments?.getString("shapeId")
        val shape = ViewShapeHolder.getInstance().canevas.findShape(shapeId!!) as ClassShape

        return activity?.let {
            // Use the Builder class for convenient dialog construction
            val builder = AlertDialog.Builder(it)
            val view: View = it.layoutInflater.inflate(R.layout.dialog_edit_class,null)

            val nameStr : EditText = view.findViewById(R.id.drawing_name_text)
            nameStr.setText(shape?.name)

            val attributesStr : EditText = view.findViewById(R.id.attributes_text)
            attributesStr.setText(shape?.attributes.joinToString("\n"))

            val methodsStr : EditText = view.findViewById(R.id.methods_text)
            methodsStr.setText(shape?.methods.joinToString("\n"))


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
                        shape?.name =  nameStr.text.toString()
                       // shape?.attributes =  (attributesStr.text!!.toString().split("\n").toTypedArray()) as ArrayList<String?>
                       // shape?.methods =  (attributesStr.text!!.toString().split("\n").toTypedArray()) as ArrayList<String?>
                    })


            // Create the AlertDialog object and return it
            builder.create()
        } ?: throw IllegalStateException("Activity cannot be null")

    }
}
