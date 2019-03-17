package com.polypaint.polypaint.View

import androidx.appcompat.app.AppCompatActivity
import android.content.Context
import android.graphics.Canvas
import android.os.Bundle
import android.util.Log
import android.view.MotionEvent
import android.view.View
import android.widget.ImageView
import android.widget.RelativeLayout
import android.widget.TextView
import androidx.fragment.app.DialogFragment
import com.polypaint.polypaint.Enum.ShapeTypes
import com.polypaint.polypaint.Fragment.EditClassDialogFragment
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.Particles.ParticleSystem
import com.polypaint.polypaint.R
import kotlinx.android.synthetic.main.basic_element.view.*
import kotlinx.android.synthetic.main.view_class.view.*

class PhaseView(context: Context): BasicElementView(context) {
        override var mMinimumWidth : Float = 400F
        override var mMinimumHeight : Float = 220F

    override fun onAttachedToWindow() {
        super.onAttachedToWindow()

        val activity : AppCompatActivity = context as AppCompatActivity

        var child: View = activity.layoutInflater.inflate(R.layout.view_phase, null)

        var nameText: TextView = child.findViewById(R.id.name) as TextView
        // TODO : Initialiser le text avec le basictElement.name lorsqu'on aura déterminé comment les view vont être parsées
        nameText.text = "basictElement.name"

        /*
        val shape = ViewShapeHolder.getInstance().canevas.findShape(
            ViewShapeHolder.getInstance().map.getValue(this)
        )
        Log.d("*******", shape.toString())
        */

        borderResizableLayout.addView(child)


        // TODO : On ne peut pas mettre à jour les dimmenssion d'une vue dans onAttachedToWindow alors le code ci-dessous ne fait rien en ce moment
        // TODO : Pour le moment je n'ai pas trouvé d'alternative qui fonctionne sans interférer avec le resize.
        borderResizableLayout.layoutParams.width =  (mMinimumWidth).toInt()

        borderResizableLayout.layoutParams.height = (mMinimumHeight).toInt()
        linearLayoutCompat.layoutParams.height = (2*mMinimumHeight/10).toInt()
        linearLayoutCompat2.layoutParams.height = (8*mMinimumHeight/10).toInt()


    }

    override fun resize(newWidth:Int, newHeight:Int){
        if(newWidth >= mMinimumWidth){
            borderResizableLayout.layoutParams.width = newWidth
        }

        if(newHeight >= mMinimumHeight){
            borderResizableLayout.layoutParams.height = newHeight
            linearLayoutCompat.layoutParams.height = (2*newHeight / 10)
            linearLayoutCompat2.layoutParams.height = (8 * newHeight / 10)
        }

        borderResizableLayout.requestLayout()
        requestLayout()
    }
}