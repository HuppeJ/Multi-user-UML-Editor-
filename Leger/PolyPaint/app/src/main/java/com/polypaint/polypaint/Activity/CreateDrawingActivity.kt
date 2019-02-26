package com.polypaint.polypaint.Activity

import android.content.pm.ActivityInfo
import android.os.Bundle
import android.content.Intent
import android.text.TextUtils
import android.util.Log
import android.view.MenuItem
import android.widget.Button
import android.widget.EditText
import android.widget.RadioButton
import android.widget.RadioGroup
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import androidx.fragment.app.DialogFragment
import co.zsmb.materialdrawerkt.builders.drawer
import com.mikepenz.materialdrawer.Drawer
import com.polypaint.polypaint.Enum.AccessibilityTypes
import com.polypaint.polypaint.Fragment.EditClassDialogFragment
import com.polypaint.polypaint.Holder.UserHolder
import com.polypaint.polypaint.Model.Canevas
import com.polypaint.polypaint.R
import java.util.*

class CreateDrawingActivity: AppCompatActivity(){
    private var drawer: Drawer? = null
    private var isDrawingPrivate: Boolean = false
    private var isPasswordProtected: Boolean = false
    private var nameView: EditText? = null
    private var passwordView: EditText? = null

    override fun onCreate (savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE

        setContentView(R.layout.activity_create_drawing)

        val activityToolbar : Toolbar = findViewById(R.id.toolbar)
        setSupportActionBar(activityToolbar)

        /*drawer = drawer {
            override fun onOptionsItemSelected(item: MenuItem): Boolean {
                when (item.getItemId()) {
                    android.R.id.home -> {
                        onBackPressed();
                        return true;
                    }

                    else -> {
                        return super.onOptionsItemSelected(item)
                    }
                }
            }
        }
        drawer?.actionBarDrawerToggle?.isDrawerIndicatorEnabled = false
        supportActionBar?.setDisplayHomeAsUpEnabled(true)*/


        val radioPublic: RadioButton = findViewById(R.id.radio_public)
        val radioPrivate: RadioButton = findViewById(R.id.radio_private)
        radioPublic.isChecked = true
        radioPublic.setOnClickListener { _->
            isDrawingPrivate = false
        }
        radioPrivate.setOnClickListener { _->
            isDrawingPrivate = true
        }

        nameView = findViewById(R.id.drawing_name_text)
        passwordView = findViewById(R.id.drawing_password_text)
        var createDrawingButton: Button = findViewById(R.id.create_drawing_button)
        createDrawingButton.setOnClickListener { _->
            createDrawing()
        }

        /*val ft = supportFragmentManager.beginTransaction()
        //ft.add(R.id.list_container, RoomsListFragment())
        ft.add(R.id.details_container, MessageListFragment(), MessageListFragment.TAG)
        ft.commit()*/

    }

    private fun createDrawing(){

        var name = nameView?.text.toString().trim()
        if(TextUtils.isEmpty(name)){
            nameView?.requestFocus()
            nameView?.error = "Enter a name"
            name = ""
            return
        }
        var password = passwordView?.text.toString().trim()
        isPasswordProtected = TextUtils.isEmpty(password)

        var canevas: Canevas = Canevas(UUID.randomUUID().toString(), name, UserHolder.getInstance().username, UserHolder.getInstance().username, AccessibilityTypes.PUBLIC.ordinal, null, ArrayList(), ArrayList())


        val intent = Intent(this, DrawingActivity::class.java)
        intent.putExtra("canevas", canevas)
        startActivity(intent)

//
//        var dialog: DialogFragment = EditClassDialogFragment()
//        var bundle: Bundle = Bundle()
//        bundle.putString("id", "asdfasg")
//        dialog.arguments = bundle
//
//        Log.d("****", dialog.arguments.toString())
//        dialog.show(supportFragmentManager, "alllooooo")




    }

}