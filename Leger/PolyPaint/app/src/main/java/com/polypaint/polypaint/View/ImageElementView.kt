package com.polypaint.polypaint.View

import androidx.appcompat.app.AppCompatActivity
import android.content.Context
import android.graphics.Color
import android.graphics.ColorFilter
import android.graphics.PorterDuff
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
import com.polypaint.polypaint.R
import kotlinx.android.synthetic.main.basic_element.view.*
import kotlinx.android.synthetic.main.view_image_element.view.*


class ImageElementView(context: Context, shapeType: ShapeTypes): BasicElementView(context) {
    override var mMinimumWidth : Float = 140F
    override var mMinimumHeight : Float = 200F
    private var shapeType : ShapeTypes? = shapeType
    private var imgBackground : ImageView? = null
    private var imgBackgroundBack : ImageView? = null

    override fun onAttachedToWindow() {
        super.onAttachedToWindow()

        val activity : AppCompatActivity = context as AppCompatActivity

        var child = activity.layoutInflater.inflate(R.layout.view_image_element, null)

        var nameText: TextView = child.findViewById(R.id.view_image_element_name) as TextView
        // TODO : Initialiser le text avec le basictElement.name lorsqu'on aura déterminé comment les view vont être parsées
        nameText.text = "basictElement.name"

        /*
        val shape = ViewShapeHolder.getInstance().canevas.findShape(
            ViewShapeHolder.getInstance().map.getValue(this)
        )
        Log.d("*******", shape.toString())
        */

        var imgFront: ImageView = child.findViewById(R.id.image_element) as ImageView
        var imgBack: ImageView = child.findViewById(R.id.image_element_background) as ImageView
        imgBackground = imgFront
        imgBackgroundBack = imgBack
        when(this.shapeType){
            ShapeTypes.DEFAULT->{ }

            ShapeTypes.ARTIFACT -> {
                imgFront.setBackgroundResource(R.drawable.ic_artefact)
                imgBack.setBackgroundResource(R.drawable.ic_artefact_bck)
            }
            ShapeTypes.ACTIVITY -> {
                imgFront.setBackgroundResource(R.drawable.ic_activity)
                imgBack.setBackgroundResource(R.drawable.ic_activity_bck)
            }
            ShapeTypes.ROLE -> {
                imgFront.setBackgroundResource(R.drawable.ic_actor)
                imgBack.setBackgroundResource(R.drawable.ic_actor_bck)

            }
        }

        borderResizableLayout.addView(child)


        // TODO : On ne peut pas mettre à jour les dimmenssion d'une vue dans onAttachedToWindow alors le code ci-dessous ne fait rien en ce moment
        // TODO : Pour le moment je n'ai pas trouvé d'alternative qui fonctionne sans interférer avec le resize.
        borderResizableLayout.layoutParams.width =  (mMinimumWidth).toInt()

        borderResizableLayout.layoutParams.height = (mMinimumHeight).toInt()
        linearLayoutCompatImg.layoutParams.height = (7*mMinimumHeight/10).toInt()
        linearLayoutCompat2Img.layoutParams.height = (3*mMinimumHeight/10).toInt()

    }

    override fun resize(newWidth:Int, newHeight:Int){
        if(newWidth >= mMinimumWidth){
            borderResizableLayout.layoutParams.width = newWidth
        }else{
            borderResizableLayout.layoutParams.width = mMinimumWidth.toInt()
        }

        if(newHeight >= mMinimumHeight){
            borderResizableLayout.layoutParams.height = newHeight
            // TODO :  is null : linearLayoutCompatImg & linearLayoutCompat2Img
            linearLayoutCompatImg.layoutParams.height = (7*newHeight / 10)
            linearLayoutCompat2Img.layoutParams.height = (3 * newHeight / 10)
        }else{
            borderResizableLayout.layoutParams.height = (mMinimumHeight).toInt()
            linearLayoutCompatImg.layoutParams.height = (7*mMinimumHeight/10).toInt()
            linearLayoutCompat2Img.layoutParams.height = (3*mMinimumHeight/10).toInt()
        }

        borderResizableLayout.requestLayout()
        requestLayout()
    }

    override fun outlineColor(color : String, borderType: Int){
        when(borderType){
            0->{imgBackground!!.background.mutate().setColorFilter(Color.parseColor(color), PorterDuff.Mode.SRC_IN)}
            //TODO: Faire d'autre svg avec les contours en dashed
            1->{}
        }
    }

    override fun backgroundColor(color : String){
        imgBackgroundBack!!.background.mutate().setColorFilter(Color.parseColor(color), PorterDuff.Mode.SRC_IN)
    }
}