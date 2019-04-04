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
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.Model.Link
import com.polypaint.polypaint.R
import com.polypaint.polypaint.View.LinkView
import com.skydoves.colorpickerview.ColorEnvelope
import com.skydoves.colorpickerview.ColorPickerDialog
import com.skydoves.colorpickerview.listeners.ColorEnvelopeListener

class EditLinkDialogFragment: DialogFragment(), AdapterView.OnItemSelectedListener {
    var link: Link? = null
    var type: Int = 0
    var color: String = ""
    var thickness: Int = 0
    var style: Int = 0
    var nameView: EditText? = null
    var multiplicityFrom: EditText? = null
    var multiplicityTo: EditText? = null

    override fun onNothingSelected(parent: AdapterView<*>?) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun onItemSelected(parent: AdapterView<*>?, view: View?, position: Int, id: Long) {
        when (parent?.id){
            R.id.link_type_spinner->{
                Log.d("setType", position.toString())
                type = position
            }
            R.id.link_style_spinner->{
                style = position
            }
            R.id.link_thickness_spinner->{
                when(position){
                    0-> thickness = 10
                    1-> thickness = 15
                    2-> thickness = 20
                }
            }
        }
    }

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        return activity?.let {
            val linkId = arguments?.getString("linkId")
            link = ViewShapeHolder.getInstance().canevas.findLink(linkId !!)
            // Use the Builder class for convenient dialog construction
            val builder = AlertDialog.Builder(it)
            val view: View = it.layoutInflater.inflate(R.layout.dialog_edit_link,null)

            val localLink = link
            if(localLink != null) {
                type = localLink.type
                multiplicityFrom?.setText(localLink.from.multiplicity)
                multiplicityTo?.setText(localLink.to.multiplicity)
                color = localLink.style.color
                thickness = localLink.style.thickness
                style = localLink.style.type
            }

            val typeSpinner: Spinner = view.findViewById(R.id.link_type_spinner)
            val thicknessSpinner: Spinner = view.findViewById(R.id.link_thickness_spinner)
            val styleSpinner: Spinner = view.findViewById(R.id.link_style_spinner)

            val colorPickerButton:Button = view.findViewById(R.id.link_color_picker)
            colorPickerButton.setBackgroundColor(Color.parseColor(color))
            colorPickerButton.setOnClickListener {
                ColorPickerDialog.Builder(context)
                    .setPositiveButton("Select", ColorEnvelopeListener{ envelope: ColorEnvelope, fromUser: Boolean ->
                        color = "#"+envelope.hexCode
                        colorPickerButton.setBackgroundColor(Color.parseColor(color))
                    })
                    .setNegativeButton("Cancel", DialogInterface.OnClickListener{ dialog: DialogInterface?, which: Int ->
                        dialog?.dismiss()
                    })
                    .attachAlphaSlideBar(false)
                    .show()
            }

            nameView = view.findViewById(R.id.link_name_text)
            multiplicityFrom = view.findViewById(R.id.link_multiplicity_from_text)
            multiplicityTo = view.findViewById(R.id.link_multiplicity_to_text)

            setAdapter(typeSpinner, R.array.link_types_array)

            setAdapter(thicknessSpinner, R.array.link_thickness_array)
            setAdapter(styleSpinner, R.array.link_styles_array)

            typeSpinner.onItemSelectedListener = this

            thicknessSpinner.onItemSelectedListener = this
            styleSpinner.onItemSelectedListener = this

            nameView?.setText(link?.name)
            multiplicityFrom?.setText(link?.from?.multiplicity)
            multiplicityTo?.setText(link?.to?.multiplicity)

            Log.d("linkType", link?.type.toString())
            Log.d("linkColor", link?.style?.color)
            Log.d("linkThickness", link?.style?.thickness.toString())
            Log.d("linkStyle", link?.style?.type.toString())
            typeSpinner.setSelection(link?.type!!)

            when(link?.style?.thickness){
                10-> thicknessSpinner.setSelection(0)
                15-> thicknessSpinner.setSelection(1)
                20-> thicknessSpinner.setSelection(2)
            }
            styleSpinner.setSelection(link?.style?.type!!)

            builder.setView(view)
                .setPositiveButton("Close") { dialog, id ->

                }
            // Create the AlertDialog object and return it
            builder.create()
        } ?: throw IllegalStateException("Activity cannot be null")
    }

    override fun onDestroy() {
        link?.name = nameView?.text?.trim().toString()
        Log.d("type", type.toString())
        link?.type = type
        link?.from?.multiplicity = multiplicityFrom?.text?.trim().toString()
        link?.to?.multiplicity = multiplicityTo?.text?.trim().toString()
        link?.style?.color = color
        link?.style?.thickness = thickness
        link?.style?.type = style

        val linkView: LinkView? = ViewShapeHolder.getInstance().linkMap.inverse()[link?.id]

        if(linkView != null){
            linkView.emitUpdate()
            linkView.invalidate()
            linkView.requestLayout()
            linkView.dialog = null
        }

        super.onDestroy()
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
