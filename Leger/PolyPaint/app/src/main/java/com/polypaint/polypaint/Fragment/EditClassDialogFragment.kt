package com.polypaint.polypaint.Fragment

import android.app.Dialog
import android.content.DialogInterface
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.EditText
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.fragment.app.DialogFragment
import com.polypaint.polypaint.Activity.DrawingActivity
import com.polypaint.polypaint.Holder.SyncShapeHolder
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.Model.BasicShape
import com.polypaint.polypaint.Model.ClassShape
import com.polypaint.polypaint.R

class EditClassDialogFragment: DialogFragment() {

    var shape : ClassShape? = null
    var viewSelf : View? = null

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        var shapeId = arguments?.getString("shapeId")
        shape = ViewShapeHolder.getInstance().canevas.findShape(shapeId!!) as ClassShape

        return activity?.let {
            // Use the Builder class for convenient dialog construction
            val builder = AlertDialog.Builder(it)
            viewSelf = it.layoutInflater.inflate(R.layout.dialog_edit_class,null)

            val nameStr : EditText = viewSelf!!.findViewById(R.id.drawing_name_text)
            nameStr.setText(shape?.name)

            val attributesStr : EditText = viewSelf!!.findViewById(R.id.attributes_text)
            attributesStr.setText(shape?.attributes!!.joinToString("\n"))

            val methodsStr : EditText = viewSelf!!.findViewById(R.id.methods_text)
            methodsStr.setText(shape?.methods!!.joinToString("\n"))

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
        val nameStr : EditText = viewSelf!!.findViewById(R.id.drawing_name_text)
        val attributesStr : EditText = viewSelf!!.findViewById(R.id.attributes_text)
        val methodsStr : EditText = viewSelf!!.findViewById(R.id.methods_text)

        shape?.name =  nameStr.text.toString()

        var attributes : ArrayList<String?> = ArrayList()
        for(attribute in attributesStr.text!!.toString().split("\n")){
            attributes.add(attribute)
        }

        shape?.attributes =  attributes

        var methods : ArrayList<String?> = ArrayList()
        for(method in methodsStr.text!!.toString().split("\n")){
            methods.add(method)
        }
        shape?.methods =  methods

        SyncShapeHolder.getInstance().drawingActivity!!.syncLayoutFromCanevas()
    }
}
