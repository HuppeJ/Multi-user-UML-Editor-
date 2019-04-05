package com.polypaint.polypaint.View

import androidx.appcompat.app.AppCompatActivity
import android.content.Context
import android.content.res.ColorStateList
import android.graphics.*
import android.graphics.drawable.GradientDrawable
import android.graphics.drawable.LayerDrawable
import android.os.Bundle
import android.util.Log
import android.view.MotionEvent
import android.view.View
import android.widget.RelativeLayout
import androidx.core.content.ContextCompat
import androidx.core.graphics.drawable.DrawableCompat
import androidx.fragment.app.DialogFragment
import com.polypaint.polypaint.Activity.DrawingActivity
import com.polypaint.polypaint.Fragment.EditBasicElementDialogFragment
import com.polypaint.polypaint.Fragment.EditClassDialogFragment
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.R
import kotlinx.android.synthetic.main.basic_element.view.*
import kotlinx.android.synthetic.main.view_class.view.*


class ClassView(context: Context): BasicElementView(context) {
    override var mMinimumWidth : Float = 126F
    override var mMinimumHeight : Float = 126F
    var child : View? = null

    override fun onAttachedToWindow() {
        super.onAttachedToWindow()

        val activity : AppCompatActivity = context as AppCompatActivity

        child = activity.layoutInflater.inflate(R.layout.view_class, null)


        borderResizableLayout.addView(child)
        /*
        borderResizableLayout.layoutParams.height = (mMinimumHeight).toInt()
        linearLayoutCompat.layoutParams.height = (mMinimumHeight/7).toInt()
        linearLayoutCompat2.layoutParams.height = (3*mMinimumHeight/7).toInt()
        linearLayoutCompat3.layoutParams.height = (3*mMinimumHeight/7).toInt()
        */
    }

    override fun resize(newWidth:Int, newHeight:Int){
        if(newWidth >= mMinimumWidth){
            borderResizableLayout.layoutParams.width = newWidth
        }else{
            borderResizableLayout.layoutParams.width = mMinimumWidth.toInt()
        }

        if(newHeight >= mMinimumHeight){
            borderResizableLayout.layoutParams.height = newHeight
            linearLayoutCompat.layoutParams.height = 60
            linearLayoutCompat2.layoutParams.height = (3 * (newHeight-60) / 7)
            linearLayoutCompat3.layoutParams.height = (4 * (newHeight-60) / 7)
        }else{
            borderResizableLayout.layoutParams.height = (mMinimumHeight).toInt()
            linearLayoutCompat.layoutParams.height = 60
            linearLayoutCompat2.layoutParams.height = (3*(mMinimumHeight-60)/7).toInt()
            linearLayoutCompat3.layoutParams.height = (4*(mMinimumHeight-60)/7).toInt()
        }

        borderResizableLayout.requestLayout()
        requestLayout()
    }

    override fun editShape(){
        val shapeId: String? = ViewShapeHolder.getInstance().map[this]

        var activity: AppCompatActivity = context as AppCompatActivity
        var dialog: DialogFragment = EditClassDialogFragment()

        var bundle: Bundle = Bundle()
        bundle.putString("shapeId", shapeId)

        dialog.arguments = bundle

        Log.d("****", dialog.arguments.toString())
        dialog.show(activity.supportFragmentManager, "alllooooo")
    }

    override fun outlineColor(color : String, borderType: Int){

        var lDrawable = child!!.linearLayoutCompat.background.mutate() as LayerDrawable
        var gDrawable = lDrawable.findDrawableByLayerId(R.id.view_class_borders) as GradientDrawable


        var lDrawable2 = child!!.linearLayoutCompat2.background.mutate() as LayerDrawable
        var gDrawable2 = lDrawable2.findDrawableByLayerId(R.id.view_class_borders2) as GradientDrawable


        var lDrawable3 = child!!.linearLayoutCompat3.background.mutate() as LayerDrawable
        var gDrawable3 = lDrawable3.findDrawableByLayerId(R.id.view_class_borders) as GradientDrawable

        when(borderType){
            0 -> {  gDrawable.setStroke(1, Color.parseColor(color))
                gDrawable2.setStroke(1, Color.parseColor(color))
                gDrawable3.setStroke(1, Color.parseColor(color))
            }
            1 -> {  gDrawable.setStroke(1, Color.parseColor(color), 10F,10F)
                gDrawable2.setStroke(1, Color.parseColor(color),10F,10F)
                gDrawable3.setStroke(1, Color.parseColor(color),10F,10F)
            }
        }


    }

    override fun backgroundColor(color: String) {
        child!!.class_layout2.background.mutate().setColorFilter(Color.parseColor(color),PorterDuff.Mode.SRC_IN)

    }

}