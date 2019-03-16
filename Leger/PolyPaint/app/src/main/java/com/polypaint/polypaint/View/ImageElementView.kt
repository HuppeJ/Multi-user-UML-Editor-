package com.polypaint.polypaint.View

import androidx.appcompat.app.AppCompatActivity
import android.content.Context
import android.os.Bundle
import android.util.Log
import android.view.MotionEvent
import android.view.View
import android.widget.ImageView
import android.widget.RelativeLayout
import androidx.fragment.app.DialogFragment
import com.polypaint.polypaint.Enum.ShapeTypes
import com.polypaint.polypaint.Fragment.EditClassDialogFragment
import com.polypaint.polypaint.R
import kotlinx.android.synthetic.main.basic_element.view.*
import kotlinx.android.synthetic.main.view_class.view.*


class ImageElementView(context: Context, shapeType: ShapeTypes): BasicElementView(context) {
    override var mMinimumWidth : Float = 220F
    override var mMinimumHeight : Float = 320F
    private var shapeType : ShapeTypes? = shapeType


    override fun onAttachedToWindow() {
        super.onAttachedToWindow()

        val activity : AppCompatActivity = context as AppCompatActivity

        var child: View? = null

        when(this.shapeType){
            ShapeTypes.DEFAULT->{ }

            ShapeTypes.ARTIFACT -> {
                child = activity.layoutInflater.inflate(R.layout.view_artifact, null)
            }
            ShapeTypes.ACTIVITY -> {
                child = activity.layoutInflater.inflate(R.layout.view_activity, null)
            }
            ShapeTypes.ROLE -> {
                child = activity.layoutInflater.inflate(R.layout.view_role, null)
            }

        }

        borderResizableLayout.addView(child)

        this.resize(mMinimumWidth.toInt(), mMinimumHeight.toInt())


        // TODO : On ne peut pas mettre à jour les dimmenssion d'une vue dans onAttachedToWindow alors le code ci-dessous ne fait rien en ce moment
        // TODO : Pour le moment je n'ai pas trouvé d'alternative qui fonctionne sans interférer avec le resize.
        borderResizableLayout.layoutParams.width =  (mMinimumWidth).toInt()

        borderResizableLayout.layoutParams.height = (mMinimumHeight).toInt()
        linearLayoutCompat.layoutParams.height = (9*mMinimumHeight/10).toInt()
        linearLayoutCompat2.layoutParams.height = (1*mMinimumHeight/10).toInt()

    }

    override fun resize(newWidth:Int, newHeight:Int){
        Log.d("newWidth", newWidth.toString())
        Log.d("newHeight", newHeight.toString())

        if(newWidth >= mMinimumWidth){
            borderResizableLayout.layoutParams.width = newWidth
        }

        if(newHeight >= mMinimumHeight){
            borderResizableLayout.layoutParams.height = newHeight
            linearLayoutCompat.layoutParams.height = (9*newHeight / 10)
            linearLayoutCompat2.layoutParams.height = (1 * newHeight / 10)
        }

        borderResizableLayout.requestLayout()
        requestLayout()
    }
}