package com.polypaint.polypaint.Fragment

import android.app.Dialog
import android.content.DialogInterface
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.EditText
import androidx.appcompat.app.AlertDialog
import androidx.fragment.app.DialogFragment
import com.polypaint.polypaint.Holder.SyncShapeHolder
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.Model.BasicShape
import com.polypaint.polypaint.Model.ClassShape
import com.polypaint.polypaint.R

class EditBasicElementDialogFragment: DialogFragment() {
    var shape : BasicShape? = null
    var viewSelf : View? = null

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        var shapeId = arguments?.getString("shapeId")
        shape = ViewShapeHolder.getInstance().canevas.findShape(shapeId!!)

        return activity?.let {
            // Use the Builder class for convenient dialog construction
            val builder = AlertDialog.Builder(it)
            viewSelf = it.layoutInflater.inflate(R.layout.dialog_edit_basic_element,null)

            val nameText : EditText = viewSelf!!.findViewById(R.id.name_text)
            nameText.setText(shape?.name)

            builder.setView(viewSelf)
                .setNegativeButton("Close",
                    DialogInterface.OnClickListener { dialog, id ->
                    })

            // Create the AlertDialog object and return it
            builder.create()
        } ?: throw IllegalStateException("Activity cannot be null")
    }
    override fun onDestroy() {
        close()
        Log.d("EditClassDialog","OnDestroy")
        super.onDestroy()
    }

    private fun close(){

        val nameText : EditText = viewSelf!!.findViewById(R.id.name_text)
        shape?.name =  nameText.text.toString()

        SyncShapeHolder.getInstance().drawingActivity!!.syncLayoutFromCanevas()
    }
}
