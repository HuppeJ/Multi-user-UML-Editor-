package com.polypaint.polypaint.Fragment

import android.app.Dialog
import android.content.DialogInterface
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.*
import androidx.appcompat.app.AlertDialog
import androidx.fragment.app.DialogFragment
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.Model.Link
import com.polypaint.polypaint.R
import kotlinx.android.synthetic.main.dialog_edit_link.*

class EditLinkDialogFragment: DialogFragment(), AdapterView.OnItemSelectedListener {
    var link: Link? = null
    var type: Int = 0
    override fun onNothingSelected(parent: AdapterView<*>?) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun onItemSelected(parent: AdapterView<*>?, view: View?, position: Int, id: Long) {
        type = position
        Toast.makeText(activity, position.toString(), Toast.LENGTH_LONG).show()
    }

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        return activity?.let {
            val linkId = arguments?.getString("linkId")
            link = ViewShapeHolder.getInstance().canevas.findLink(linkId !!)
            // Use the Builder class for convenient dialog construction
            val builder = AlertDialog.Builder(it)
            val view: View = it.layoutInflater.inflate(R.layout.dialog_edit_link,null)

            val spinner: Spinner = view.findViewById(R.id.link_type_spinner)
            val nameView: EditText = view.findViewById(R.id.link_name_text)
            val multiplicityFrom: EditText = view.findViewById(R.id.link_multiplicity_from_text)
            val multiplicityTo: EditText = view.findViewById(R.id.link_multiplicity_to_text)



            ArrayAdapter.createFromResource(
                activity,
                R.array.link_types_array,
                android.R.layout.simple_spinner_item
            ).also { adapter ->
                // Specify the layout to use when the list of choices appears
                adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
                // Apply the adapter to the spinner
                spinner.adapter = adapter
            }

            spinner.onItemSelectedListener  = this

            nameView.setText(link?.name)
            Log.d("linkType", link?.type.toString())
            spinner.setSelection(link?.type!!)
            multiplicityFrom.setText(link?.from?.multiplicity)
            multiplicityTo.setText(link?.to?.multiplicity)

            builder.setView(view)
                .setPositiveButton("Close") { dialog, id ->
                    link?.name = nameView.text.trim().toString()
                    Log.d("type", type.toString())
                    link?.type = type
                    link?.from?.multiplicity = multiplicityFrom.text.trim().toString()
                    link?.to?.multiplicity = multiplicityTo.text.trim().toString()
                    ViewShapeHolder.getInstance().linkMap.inverse()[link?.id]?.invalidate()
                    ViewShapeHolder.getInstance().linkMap.inverse()[link?.id]?.requestLayout()
                    ViewShapeHolder.getInstance().linkMap.inverse()[link?.id]?.dialog = null
                }
            // Create the AlertDialog object and return it
            builder.create()
        } ?: throw IllegalStateException("Activity cannot be null")
    }

}
