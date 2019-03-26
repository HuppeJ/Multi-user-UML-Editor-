package com.polypaint.polypaint.Fragment

import android.app.Dialog
import android.content.DialogInterface
import android.graphics.Color
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.*
import androidx.appcompat.app.AlertDialog
import androidx.fragment.app.DialogFragment
import com.polypaint.polypaint.Holder.SyncShapeHolder
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.Model.BasicShape
import com.polypaint.polypaint.Model.ClassShape
import com.polypaint.polypaint.R
import com.skydoves.colorpickerview.ColorEnvelope
import com.skydoves.colorpickerview.ColorPickerDialog
import com.skydoves.colorpickerview.listeners.ColorEnvelopeListener

class EditBasicElementDialogFragment: DialogFragment(){
    var shape : BasicShape? = null
    var viewSelf : View? = null
    var colorBorder : String = ""
    var colorBackground : String = ""

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        var shapeId = arguments?.getString("shapeId")
        shape = ViewShapeHolder.getInstance().canevas.findShape(shapeId!!)

        return activity?.let {
            // Use the Builder class for convenient dialog construction
            val builder = AlertDialog.Builder(it)
            viewSelf = it.layoutInflater.inflate(R.layout.dialog_edit_basic_element,null)

            val nameText : EditText = viewSelf!!.findViewById(R.id.name_text)
            nameText.setText(shape?.name)
            colorBorder = shape?.shapeStyle!!.borderColor
            colorBackground = shape?.shapeStyle!!.backgroundColor

            val borderColorPickerButton: Button = viewSelf!!.findViewById(R.id.border_color_picker_button)

            borderColorPickerButton.setBackgroundColor(Color.parseColor(colorBorder))

            borderColorPickerButton.setOnClickListener {
                ColorPickerDialog.Builder(context)
                    .setPositiveButton("Select", ColorEnvelopeListener{ envelope: ColorEnvelope, fromUser: Boolean ->
                        colorBorder = "#"+envelope.hexCode
                        borderColorPickerButton.setBackgroundColor(Color.parseColor(colorBorder))
                    })
                    .setNegativeButton("Cancel", DialogInterface.OnClickListener{ dialog: DialogInterface?, which: Int ->
                        dialog?.dismiss()
                    })
                    .attachAlphaSlideBar(false)
                    .show()
            }

            val backgroundColorPickerButton: Button = viewSelf!!.findViewById(R.id.background_color_picker_button)

            backgroundColorPickerButton.setBackgroundColor(Color.parseColor(colorBackground))

            backgroundColorPickerButton.setOnClickListener {
                ColorPickerDialog.Builder(context)
                    .setPositiveButton("Select", ColorEnvelopeListener{ envelope: ColorEnvelope, fromUser: Boolean ->
                        colorBackground = "#"+envelope.hexCode
                        backgroundColorPickerButton.setBackgroundColor(Color.parseColor(colorBackground))
                    })
                    .setNegativeButton("Cancel", DialogInterface.OnClickListener{ dialog: DialogInterface?, which: Int ->
                        dialog?.dismiss()
                    })
                    .attachAlphaSlideBar(false)
                    .show()
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
        shape?.shapeStyle!!.borderColor = colorBorder
        shape?.shapeStyle!!.backgroundColor = colorBackground

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
