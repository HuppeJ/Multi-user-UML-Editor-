package com.polypaint.polypaint.View

import androidx.appcompat.app.AppCompatActivity
import android.content.Context
import android.os.Bundle
import android.util.Log
import android.view.MotionEvent
import android.view.View
import android.widget.RelativeLayout
import androidx.fragment.app.DialogFragment
import com.polypaint.polypaint.Fragment.EditClassDialogFragment
import com.polypaint.polypaint.R
import kotlinx.android.synthetic.main.basic_element.view.*


class ClassView(context: Context): BasicElementView(context) {
    override fun onAttachedToWindow() {
        super.onAttachedToWindow()

        val activity : AppCompatActivity = context as AppCompatActivity

        val child: View = activity.layoutInflater.inflate(R.layout.view_class, null)
        borderResizableLayout.addView(child)
    }
}