package com.polypaint.polypaint.Activity

import android.annotation.SuppressLint
import android.content.Context
import android.content.Intent
import android.content.pm.ActivityInfo
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.widget.Button
import androidx.appcompat.app.AppCompatActivity
import com.github.nkzawa.socketio.client.Socket
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Model.BasicElement
import com.polypaint.polypaint.R
import kotlinx.android.synthetic.main.activity_drawing.*
import kotlinx.android.synthetic.main.basic_element.*
import kotlinx.android.synthetic.main.basic_element.view.*

class DrawingActivity : AppCompatActivity(){
    // private var parentRelativeLayout : RelativeLayout? = null
    private var inflater : LayoutInflater? = null
    private var childIndexCount : Int = 0
    public var childSelected : Int = 0



    //TODO: déplacer dans object contenant tous les eventListeners possible
    private var onTouchListener = View.OnTouchListener { v, event ->
        v.x = event.rawX - v.width/2 - parent_relative_layout!!.x
        v.y = event.rawY - v.height/2 - parent_relative_layout!!.y

        childSelected = v.id

        v.requestFocus()
        true
    }


    @SuppressLint("ClickableViewAccessibility")
    override fun onCreate (savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE
        setContentView(R.layout.activity_drawing)

        inflater = getSystemService(Context.LAYOUT_INFLATER_SERVICE) as LayoutInflater



        var addButton: Button = findViewById(R.id.add_button)
        addButton.setOnClickListener {

            val mBasicElem = BasicElement(this)

            mBasicElem.id = childIndexCount
            val viewToAdd = inflater!!.inflate(R.layout.basic_element, null)
            mBasicElem.addView(viewToAdd,mBasicElem.childCount - 1)

            mBasicElem.setOnTouchListener(onTouchListener)
            parent_relative_layout?.addView(mBasicElem, childIndexCount)
            childIndexCount++
        }

        move_button.setOnClickListener {
            val txt = parent_relative_layout?.getChildAt(childSelected)!!.first_line.text
            parent_relative_layout?.getChildAt(childSelected )!!.first_line.text = txt.toString() + "3"
        }
    }


    /* Max

        private var onLongClickListener = View.OnLongClickListener { v ->
        first_line.text = "LONGCLICK"
        true
    }
            mBasicElem.setOnLongClickListener(onLongClickListener)


    var onTouchListenerRelLayout = View.OnClickListener { v ->
    childSelected = parentRelativeLayout!!.indexOfChild(parentRelativeLayout!!.focusedChild)
    for (i in 0..parentRelativeLayout!!.childCount)
        Log.d("focus : ", ""+i+childSelected)

    // Return true means this listener has complete process this event successfully.
    true
}
*/

    override fun onBackPressed() {
        val app = application as PolyPaint
        val socket: Socket? = app.socket
        socket?.disconnect()
        val intent = Intent(this, LoginActivity::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TOP
        startActivity(intent)
        finish()
    }
}