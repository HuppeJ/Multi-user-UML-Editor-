package com.polypaint.polypaint.Fragment

import android.app.Dialog
import android.content.DialogInterface
import android.graphics.Color
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.AdapterView
import android.widget.ArrayAdapter
import android.widget.EditText
import android.widget.Spinner
import androidx.appcompat.app.AlertDialog
import androidx.fragment.app.DialogFragment
import com.polypaint.polypaint.Holder.SyncShapeHolder
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.Model.BasicShape
import com.polypaint.polypaint.Model.ClassShape
import com.polypaint.polypaint.R

class EditBasicElementDialogFragment: DialogFragment(), AdapterView.OnItemSelectedListener {
    var shape : BasicShape? = null
    var viewSelf : View? = null
    var color: String = ""
    override fun onItemSelected(parent: AdapterView<*>?, view: View?, position: Int, id: Long) {
        when (parent?.id){
            R.id.border_color_spinner ->{
                when(position){
                    0-> color = "BLACK"
                    1-> color = "GREEN"
                    2-> color = "YELLOW"
                }
            }
        }
    }
    override fun onNothingSelected(parent: AdapterView<*>?) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }
    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        var shapeId = arguments?.getString("shapeId")
        shape = ViewShapeHolder.getInstance().canevas.findShape(shapeId!!)

        return activity?.let {
            // Use the Builder class for convenient dialog construction
            val builder = AlertDialog.Builder(it)
            viewSelf = it.layoutInflater.inflate(R.layout.dialog_edit_basic_element,null)

            val nameText : EditText = viewSelf!!.findViewById(R.id.name_text)
            nameText.setText(shape?.name)

            val borderColorSpinner : Spinner = viewSelf!!.findViewById(R.id.border_color_spinner)
            setAdapter(borderColorSpinner, R.array.link_colors_array)
            borderColorSpinner.onItemSelectedListener = this
            when(shape?.shapeStyle!!.borderColor){
                "BLACK"-> borderColorSpinner.setSelection(0)
                "GREEN"-> borderColorSpinner.setSelection(1)
                "YELLOW"-> borderColorSpinner.setSelection(2)
            }


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
        shape?.shapeStyle!!.borderColor = color
        Log.d("BasicElemColor",color)

        SyncShapeHolder.getInstance().drawingActivity!!.syncLayoutFromCanevas()
    }

    private fun setAdapter(spinner: Spinner, array: Int){
        ArrayAdapter.createFromResource(
            activity,
            array,
            android.R.layout.simple_spinner_item
        ).also { adapter ->
            // Specify the layout to use when the list of choices appears
            adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
            // Apply the adapter to the spinner
            spinner.adapter = adapter
        }
    }
}
