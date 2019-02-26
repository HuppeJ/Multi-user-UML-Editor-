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
import kotlinx.android.synthetic.main.view_class.view.*


class ClassView(context: Context): BasicElementView(context) {
    override var mMinimumWidth : Float = 300F
    override var mMinimumHeight : Float = 370F


    override fun onAttachedToWindow() {
        super.onAttachedToWindow()

        val activity : AppCompatActivity = context as AppCompatActivity

        val child: View = activity.layoutInflater.inflate(R.layout.view_class, null)
        borderResizableLayout.addView(child)

        borderResizableLayout.layoutParams.height = (mMinimumHeight).toInt()
        linearLayoutCompat.layoutParams.height = (mMinimumHeight/7).toInt()
        linearLayoutCompat2.layoutParams.height = (3*mMinimumHeight/7).toInt()
        linearLayoutCompat3.layoutParams.height = (3*mMinimumHeight/7).toInt()
    }

    override fun resize(newWidth:Int, newHeight:Int){
        if(newWidth >= mMinimumWidth){
            borderResizableLayout.layoutParams.width = newWidth
        }

        if(newHeight >= mMinimumHeight){
            borderResizableLayout.layoutParams.height = newHeight
            linearLayoutCompat.layoutParams.height = (newHeight / 7)
            linearLayoutCompat2.layoutParams.height = (3 * newHeight / 7)
            linearLayoutCompat3.layoutParams.height = (3 * newHeight / 7)
        }

        borderResizableLayout.requestLayout()
        requestLayout()
    }
}