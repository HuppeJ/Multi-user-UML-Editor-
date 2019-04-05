package com.polypaint.polypaint.View

import android.content.Context
import android.graphics.Color
import android.graphics.PorterDuff
import android.graphics.drawable.GradientDrawable
import android.graphics.drawable.LayerDrawable
import android.view.View
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import com.polypaint.polypaint.R
import kotlinx.android.synthetic.main.basic_element.view.*
import kotlinx.android.synthetic.main.view_comment.view.*

class FreeTextView(context: Context): BasicElementView(context) {
    override var mMinimumWidth : Float = 126F
    override var mMinimumHeight : Float = 126F
    private var child : View? = null

    override fun onAttachedToWindow() {
        super.onAttachedToWindow()

        anchorPoint0.visibility = View.INVISIBLE
        anchorPoint1.visibility = View.INVISIBLE
        anchorPoint2.visibility = View.INVISIBLE
        anchorPoint3.visibility = View.INVISIBLE

        val activity : AppCompatActivity = context as AppCompatActivity

        child = activity.layoutInflater.inflate(R.layout.view_freetext, null)

        var nameText: TextView = child!!.findViewById(R.id.free_text_text) as TextView
        // TODO : Initialiser le text avec le basictElement.name lorsqu'on aura déterminé comment les view vont être parsées
        nameText.text = "freetext..."


        borderResizableLayout.addView(child)

        // TODO : On ne peut pas mettre à jour les dimmenssion d'une vue dans onAttachedToWindow alors le code ci-dessous ne fait rien en ce moment
        // TODO : Pour le moment je n'ai pas trouvé d'alternative qui fonctionne sans interférer avec le resize.
        borderResizableLayout.layoutParams.width =  (mMinimumWidth).toInt()

        borderResizableLayout.layoutParams.height = (mMinimumHeight).toInt()
    }
    override fun setAnchorsVisible(isVisible: Boolean){}
    override fun resize(newWidth:Int, newHeight:Int){
        if(newWidth >= mMinimumWidth){
            borderResizableLayout.layoutParams.width = newWidth
        }else{
            borderResizableLayout.layoutParams.width = mMinimumWidth.toInt()
        }

        if(newHeight >= mMinimumHeight){
            borderResizableLayout.layoutParams.height = newHeight
        }else{
            borderResizableLayout.layoutParams.height = mMinimumHeight.toInt()
        }

        borderResizableLayout.requestLayout()
        requestLayout()
    }

    override fun outlineColor(color: String, borderType: Int) {}
    override fun backgroundColor(color: String) {}
}