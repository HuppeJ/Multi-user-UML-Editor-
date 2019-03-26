package com.polypaint.polypaint.Activity

import android.annotation.SuppressLint
import android.app.Activity
import android.content.Context
import android.content.Intent
import android.content.pm.ActivityInfo
import android.graphics.Canvas
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import co.zsmb.materialdrawerkt.builders.drawer
import co.zsmb.materialdrawerkt.builders.footer
import co.zsmb.materialdrawerkt.draweritems.badgeable.primaryItem
import co.zsmb.materialdrawerkt.draweritems.badgeable.secondaryItem
import com.github.nkzawa.emitter.Emitter
import com.github.nkzawa.socketio.client.Socket
import com.github.salomonbrys.kotson.fromJson
import com.google.gson.Gson
import com.mikepenz.materialdrawer.Drawer
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Enum.ShapeTypes
import com.polypaint.polypaint.Holder.UserHolder
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.Model.*
import com.polypaint.polypaint.R
import com.polypaint.polypaint.Socket.SocketConstants
import com.polypaint.polypaint.SocketReceptionModel.CanvasEvent
import com.polypaint.polypaint.SocketReceptionModel.FormsUpdateEvent
import com.polypaint.polypaint.SocketReceptionModel.GalleryEditEvent
import com.polypaint.polypaint.SocketReceptionModel.LinksUpdateEvent
import com.polypaint.polypaint.View.*
import kotlinx.android.synthetic.main.activity_drawing.*
import kotlinx.android.synthetic.main.basic_element.view.*
import kotlinx.android.synthetic.main.item_drawing.*
import java.lang.NullPointerException
import java.util.*
import kotlin.collections.ArrayList
import androidx.core.view.ViewCompat.setAlpha
import androidx.core.os.HandlerCompat.postDelayed
import android.provider.SyncStateContract.Helpers.update
import android.widget.FrameLayout
import android.widget.RelativeLayout
import com.polypaint.polypaint.Holder.SyncShapeHolder
import com.polypaint.polypaint.Holder.VFXHolder
import kotlinx.android.synthetic.main.dialog_edit_class.view.*
import kotlinx.android.synthetic.main.view_class.view.*
import kotlinx.android.synthetic.main.view_comment.view.*
import kotlinx.android.synthetic.main.view_image_element.view.*
import kotlinx.android.synthetic.main.view_phase.view.*
import android.graphics.Bitmap
import java.io.ByteArrayOutputStream
import android.util.Base64
import android.view.MotionEvent
import android.widget.CompoundButton
import android.widget.TextView
import com.github.salomonbrys.kotson.toJsonArray
import com.polypaint.polypaint.ResponseModel.GetSelectedFormsResponse
import com.polypaint.polypaint.ResponseModel.GetSelectedLinksResponse
import kotlinx.android.synthetic.main.toolbar.*
import org.w3c.dom.Comment


class DrawingActivity : AppCompatActivity(){

    var oldFrameRawX : Float = 0.0F
    var oldFrameRawY : Float = 0.0F
    var mMinimumWidth : Float = 300F
    var mMinimumHeight : Float = 100F
    var mMaximumWidth : Float = 1520F
    var mMaximumHeight : Float = 1200F

    var lassoView: LassoView? = null

    var isCanvasSelectedByYou : Boolean = false

    private var inflater : LayoutInflater? = null

    private var drawer: Drawer? = null
    private var socket: Socket? = null

    private var shapesToAdd: ArrayList<BasicShape> = ArrayList<BasicShape>()
    private var linksToAdd: ArrayList<Link> = ArrayList<Link>()

    private var clipboard: ArrayList<BasicShape> = ArrayList<BasicShape>()
    private var stackBasicShape: Stack<BasicShape> = Stack<BasicShape>()

    @SuppressLint("ClickableViewAccessibility")
    override fun onCreate (savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE
        setContentView(R.layout.activity_drawing)

        val activityToolbar : Toolbar = findViewById(R.id.toolbar)
        setSupportActionBar(activityToolbar)

        drawer = drawer {
            primaryItem("Gallery") {
                icon = R.drawable.ic_picture
                onClick { _ ->
                    false
                }
            }
            primaryItem("Chat") {
                icon = R.drawable.ic_chat
                onClick { _ ->
                    val intent = Intent(this@DrawingActivity, ChatActivity::class.java)
                    startActivity(intent)
                    true
                }
                selectable = false
            }
            footer{
                secondaryItem("Settings") {
                    icon = R.drawable.ic_settings
                }
            }

            toolbar = activityToolbar
        }

        ViewShapeHolder.getInstance().canevas = intent.getSerializableExtra("canevas") as Canevas
        canevas_title.text = ViewShapeHolder.getInstance().canevas.name

        inflater = getSystemService(Context.LAYOUT_INFLATER_SERVICE) as LayoutInflater


        parent_relative_layout.setOnClickListener{
            it as RelativeLayout
            it.dispatchSetSelected(false)
        }

        add_button.setOnClickListener {
            addOnCanevas(ShapeTypes.DEFAULT)
        }

        class_button.setOnClickListener {
            addOnCanevas(ShapeTypes.CLASS_SHAPE)
        }
        artefact_button.setOnClickListener {
            addOnCanevas(ShapeTypes.ARTIFACT)
        }
        activity_button.setOnClickListener {
            addOnCanevas(ShapeTypes.ACTIVITY)
        }
        role_button.setOnClickListener {
            addOnCanevas(ShapeTypes.ROLE)
        }
        comment_button.setOnClickListener {
            addOnCanevas(ShapeTypes.COMMENT)
        }
        phase_button.setOnClickListener {
            addOnCanevas(ShapeTypes.PHASE)
        }

        clear_canvas_button.setOnClickListener {
            emitClearCanvas()
            parent_relative_layout?.removeAllViews()
            ViewShapeHolder.getInstance().stackShapeCreatedId = Stack<String>()
        }

        duplicate_button.setOnClickListener{
            duplicateView()
        }
        cut_button.setOnClickListener{
            cutView()
        }
        stack_button.setOnClickListener{
            stackView()
        }
        unstack_button.setOnClickListener{
            unstackView()
        }

        save_button.setOnClickListener{
            saveCanevas()
        }

//        selection_button.setOnClickListener {
//            parent_relative_layout?.addView(LassoView(this))
//        }
        selection_button.setOnCheckedChangeListener(onSelectLasso)


        select_canevas_button.setOnCheckedChangeListener(onSelectCanvevas)

        VFXHolder.getInstance().vfxView = VfxView(this)
        parent_relative_layout.addView(VFXHolder.getInstance().vfxView)

        resizeCanvevasButton.setOnTouchListener(onTouchListenerResizeButton)

        SyncShapeHolder.getInstance().drawingActivity = this

        // TODO : Jé's fix
        if(ViewShapeHolder.getInstance().canevas.shapes !== null && !ViewShapeHolder.getInstance().canevas.shapes.isEmpty()) {
            shapesToAdd.addAll(ViewShapeHolder.getInstance().canevas.shapes)
            ViewShapeHolder.getInstance().canevas.shapes.clear()

            // TODO : review Adding LINKS
            linksToAdd.addAll(ViewShapeHolder.getInstance().canevas.links)
            ViewShapeHolder.getInstance().canevas.links.clear()
        }
        initializeViewFromCanevas()
    }

    private fun initializeViewFromCanevas(){
        if(ViewShapeHolder.getInstance().canevas != null){
            Log.d("init","****"+ViewShapeHolder.getInstance().canevas.name+"****")

            // Adding SHAPES
            for(form: BasicShape in shapesToAdd){
                Log.d("INFORLOOP","********")
                ViewShapeHolder.getInstance().canevas.addShape(form)
                addOnCanevas(form)
            }

            // TODO : review Adding LINKS
           for(link: Link in linksToAdd) {
                    ViewShapeHolder.getInstance().canevas.addLink(link)
                    val linkView: LinkView = LinkView(this)
                    linkView.setLinkAndAnchors(link)
                    ViewShapeHolder.getInstance().linkMap.forcePut(linkView, link.id)
                    parent_relative_layout?.addView(linkView)
            }

            // Sizing the Canvas
            parent_relative_layout.layoutParams.width = (ViewShapeHolder.getInstance().canevas.dimensions.x).toInt()
            parent_relative_layout.layoutParams.height = (ViewShapeHolder.getInstance().canevas.dimensions.y).toInt()

            // Selecting selected Forms and Links
            val app = application as PolyPaint
            app.socket?.emit(SocketConstants.GET_SELECTED_FORMS, ViewShapeHolder.getInstance().canevas.name)
            app.socket?.emit(SocketConstants.GET_SELECTED_LINKS, ViewShapeHolder.getInstance().canevas.name)

        }

    }
    override fun onResume() {
        super.onResume()
        val app = application as PolyPaint
        socket = app.socket

        toolbar_login_button.visibility = View.INVISIBLE

       // socket?.on(SocketConstants.CANVAS_UPDATE_TEST_RESPONSE, onCanvasUpdate)
//        socket?.on(SocketConstants.JOIN_CANVAS_TEST_RESPONSE, onJoinCanvas)
        socket?.on(SocketConstants.FORMS_UPDATED, onFormsUpdated)
        socket?.on(SocketConstants.FORMS_SELECTED, onFormsSelected)
        socket?.on(SocketConstants.FORMS_DESELECTED, onFormsDeselected)
        socket?.on(SocketConstants.FORMS_DELETED, onFormsDeleted)
        socket?.on(SocketConstants.LINKS_UPDATED, onLinksUpdated)
        socket?.on(SocketConstants.LINKS_SELECTED, onLinksSelected)
        socket?.on(SocketConstants.LINKS_DESELECTED, onLinksDeselected)
        socket?.on(SocketConstants.LINKS_DELETED, onLinksDeleted)
        socket?.on(SocketConstants.CANVAS_REINITIALIZED, onCanvasReinitialized)
        socket?.on(SocketConstants.FORM_CREATED, onFormsCreated)
        socket?.on(SocketConstants.LINK_CREATED, onLinkCreated)
        socket?.on(SocketConstants.CANVAS_RESIZED, onCanevasResized)
        socket?.on(SocketConstants.CANVAS_SELECTED, onCanevasSelected)
        socket?.on(SocketConstants.CANVAS_DESELECTED, onCanevasDeselected)
        socket?.on(SocketConstants.SELECTED_FORMS, onGetSelectedForms)
        socket?.on(SocketConstants.SELECTED_LINKS, onGetSelectedLinks)

        //socket?.emit(SocketConstants.JOIN_CANVAS_TEST)
    }

    private fun addOnCanevas(shapeType: ShapeTypes){
        var shape = newShapeOnCanevas(shapeType)
        var view = newViewOnCanevas(shapeType)

        //addViewToLayout
        parent_relative_layout?.addView(view)
        //addShapeToCanevas
        ViewShapeHolder.getInstance().canevas.addShape(shape)
        //mapViewAndShapeId
        ViewShapeHolder.getInstance().map.put(view, shape.id)
        //stackFor Stack/Unstack
        ViewShapeHolder.getInstance().stackShapeCreatedId.push(shape.id)

        //EMIT
        /*
        val gson = Gson()
        val response :Response = Response(UserHolder.getInstance().username, shape)
        val obj: String = gson.toJson(response)
        Log.d("sending", obj)
        socket?.emit(SocketConstants.CANVAS_UPDATE_TEST, obj)
        */
        emitAddForm(shape)

        syncLayoutFromCanevas()

        //LAUNCH VFX
        VFXHolder.getInstance().fireVFX(
            (shape.shapeStyle.coordinates.x + shape.shapeStyle.width/2).toFloat(),
            (shape.shapeStyle.coordinates.y + shape.shapeStyle.height/2).toFloat(),this)
    }

    private fun addOnCanevas(basicShape: BasicShape){
                    Log.d("6666","****"+ViewShapeHolder.getInstance().map+"****")

        //TODO: Probablement une meilleure facon de mapper la value à l'enum ...
        when(basicShape.type){
            ShapeTypes.DEFAULT.value()-> {
                val viewType = newViewOnCanevas(ShapeTypes.DEFAULT)
                parent_relative_layout?.addView(viewType)

                //For Sync
                    ViewShapeHolder.getInstance().map.put(viewType, basicShape.id)
            }
            ShapeTypes.CLASS_SHAPE.value()-> {
                val viewType = newViewOnCanevas(ShapeTypes.CLASS_SHAPE)
                parent_relative_layout?.addView(viewType)

                //For Sync
                    ViewShapeHolder.getInstance().map.put(viewType, basicShape.id)
            }
            ShapeTypes.ARTIFACT.value()-> {
                val viewType = newViewOnCanevas(ShapeTypes.ARTIFACT)
                parent_relative_layout?.addView(viewType)

                //For Sync
                    ViewShapeHolder.getInstance().map.put(viewType, basicShape.id)
            }
            ShapeTypes.ACTIVITY.value()-> {
                val viewType = newViewOnCanevas(ShapeTypes.ACTIVITY)
                parent_relative_layout?.addView(viewType)

                //For Sync
                    ViewShapeHolder.getInstance().map.put(viewType, basicShape.id)
            }
            ShapeTypes.ROLE.value()-> {
                val viewType = newViewOnCanevas(ShapeTypes.ROLE)
                parent_relative_layout?.addView(viewType)

                //For Sync
                    ViewShapeHolder.getInstance().map.put(viewType, basicShape.id)
            }
            ShapeTypes.COMMENT.value()-> {
                val viewType = newViewOnCanevas(ShapeTypes.COMMENT)
                parent_relative_layout?.addView(viewType)

                //For Sync
                    ViewShapeHolder.getInstance().map.put(viewType, basicShape.id)
            }
            ShapeTypes.PHASE.value()-> {
                val viewType = newViewOnCanevas(ShapeTypes.PHASE)
                parent_relative_layout?.addView(viewType)

                //For Sync
                    ViewShapeHolder.getInstance().map.put(viewType, basicShape.id)
            }

        }

        syncLayoutFromCanevas()
    }

    private fun newShapeOnCanevas(shapeType: ShapeTypes) : BasicShape{
        var shapeStyle = ShapeStyle(Coordinates(0.0,0.0), 300.0, 100.0, 0.0, "white", 0, "white")
        var shape = BasicShape(UUID.randomUUID().toString(), shapeType.value(), "defaultShape1", shapeStyle, ArrayList<String?>(), ArrayList<String?>())

        when (shapeType) {
            ShapeTypes.DEFAULT -> {}
            ShapeTypes.CLASS_SHAPE -> {
                shape = ClassShape(UUID.randomUUID().toString(), shapeType.value(), "classShape1", shapeStyle, ArrayList<String?>(), ArrayList<String?>(),ArrayList<String?>(), ArrayList<String?>())
            }
            ShapeTypes.ARTIFACT -> {
                shape = BasicShape(UUID.randomUUID().toString(), shapeType.value(), "artefactShape1", shapeStyle, ArrayList<String?>(), ArrayList<String?>())
            }
            ShapeTypes.ACTIVITY -> {
                shape = BasicShape(UUID.randomUUID().toString(), shapeType.value(), "activityShape1", shapeStyle, ArrayList<String?>(), ArrayList<String?>())
            }
            ShapeTypes.ROLE -> {
                shape = BasicShape(UUID.randomUUID().toString(), shapeType.value(), "roleShape1", shapeStyle, ArrayList<String?>(), ArrayList<String?>())
            }
            ShapeTypes.COMMENT -> {
                shape = BasicShape(UUID.randomUUID().toString(), shapeType.value(), "commentShape1", shapeStyle, ArrayList<String?>(), ArrayList<String?>())
            }
            ShapeTypes.PHASE -> {
                shape = BasicShape(UUID.randomUUID().toString(), shapeType.value(), "phaseShape1", shapeStyle, ArrayList<String?>(), ArrayList<String?>())
            }
        }

        return shape
    }

    private fun newViewOnCanevas(shapeType : ShapeTypes) : BasicElementView{
        var viewType : BasicElementView = BasicElementView(this)
        val viewContainer = inflater!!.inflate(R.layout.basic_element, null)

        when(shapeType){
            ShapeTypes.DEFAULT->{
                viewType = BasicElementView(this)
            }
            ShapeTypes.CLASS_SHAPE->{
                viewType = ClassView(this)
            }
            ShapeTypes.ARTIFACT -> {
                viewType = ImageElementView(this, shapeType)
            }
            ShapeTypes.ACTIVITY -> {
                viewType = ImageElementView(this, shapeType)
            }
            ShapeTypes.ROLE -> {
                viewType = ImageElementView(this, shapeType)
            }
            ShapeTypes.COMMENT -> {
                viewType = CommentView(this)
            }
            ShapeTypes.PHASE -> {
                viewType = PhaseView(this)
            }
        }
        viewType.addView(viewContainer)

        return viewType
    }

    private fun duplicateView(){
        if(clipboard.isEmpty()){
            //Copying list to avoid ConcurrentModificationException
            val list = ViewShapeHolder.getInstance().map.keys.toMutableList()
            for (view in list){
                if(view.isSelected && !view.isSelectedByOther) {
                    view.isSelected = false
                    val shapeToDuplicate = ViewShapeHolder.getInstance().canevas.findShape(
                        ViewShapeHolder.getInstance().map.getValue(view)
                    )
                    if (shapeToDuplicate != null) {

                        val shapeDuplicated = shapeToDuplicate.copy()
                        shapeDuplicated.id = UUID.randomUUID().toString()
                        ViewShapeHolder.getInstance().canevas.addShape(shapeDuplicated)

                        addOnCanevas(shapeDuplicated)

                        emitAddForm(shapeDuplicated)

                        ViewShapeHolder.getInstance().stackShapeCreatedId.push(shapeDuplicated.id)

                        ViewShapeHolder.getInstance().map.inverse().getValue(shapeDuplicated.id).isSelected = true
                    }
                }
            }
        }else{
            for(shape in clipboard){
                ViewShapeHolder.getInstance().canevas.addShape(shape)
                ViewShapeHolder.getInstance().stackShapeCreatedId.push(shape.id)
                addOnCanevas(shape)
                emitAddForm(shape)

                ViewShapeHolder.getInstance().map.inverse().getValue(shape.id).isSelected = true

            }
            clipboard.clear()
        }
    }

    private fun cutView(){
        val list = ViewShapeHolder.getInstance().map.keys.toMutableList()
        for (view in list){
            if(view.isSelected && !view.isSelectedByOther){
                val shapeToCut = ViewShapeHolder.getInstance().canevas.findShape(ViewShapeHolder.getInstance().map.getValue(view))
                clipboard.add(shapeToCut!!)
                emitDeleteForm(shapeToCut!!)
                parent_relative_layout.removeView(view)
                ViewShapeHolder.getInstance().remove(view)

                ViewShapeHolder.getInstance().stackShapeCreatedId.remove(shapeToCut.id)
            }
        }
    }

    private fun stackView(){
        try {
            var idToStack = ViewShapeHolder.getInstance().stackShapeCreatedId.pop()
            var shapeToStack = ViewShapeHolder.getInstance().canevas.findShape(idToStack)
            stackBasicShape.push(shapeToStack)
            emitDeleteForm(shapeToStack!!)

            var viewToRemove = ViewShapeHolder.getInstance().map.inverse().getValue(idToStack)
            parent_relative_layout.removeView(viewToRemove)
            ViewShapeHolder.getInstance().remove(viewToRemove)

        }catch (e : EmptyStackException){}

    }
    private fun unstackView(){
        try {
            val shapeUnstacked = stackBasicShape.pop()

            ViewShapeHolder.getInstance().canevas.addShape(shapeUnstacked)
            addOnCanevas(shapeUnstacked)

            emitAddForm(shapeUnstacked)
            ViewShapeHolder.getInstance().stackShapeCreatedId.push(shapeUnstacked.id)

        }catch (e : EmptyStackException){}
        catch (e : NullPointerException){} //If stacking deleted shape
    }
    public fun syncLayoutFromCanevas(){
        for (view in ViewShapeHolder.getInstance().map.keys){
            val basicShapeId:  String = ViewShapeHolder.getInstance().map.getValue(view)
            val basicShape: BasicShape? = ViewShapeHolder.getInstance().canevas.findShape(basicShapeId)
            if(basicShape != null) {
                view.x = (basicShape.shapeStyle.coordinates.x).toFloat()
                view.y = (basicShape.shapeStyle.coordinates.y).toFloat()
                view.leftX = view.x
                view.topY = view.y
                view.rotation = basicShape.shapeStyle.rotation.toFloat()

                // TODO : Jé's Fix : j'ai bougé les view.resize dans les différents case pour que la fonction redéfinie des enfants de BasicShape soit appelée (ex.: pour que la fonction .resize de ImageElementView soit appelée)
                // TODO : les attributs xml des différentes View ne sont pas reconnues même avec le cast de la view (ex.:view as ImageElementView), voir les "// TODO : is null" ci-dessous, je n'ai pas trouvé pourquoi ça faisait cela^^
                when(basicShape.type){
                    ShapeTypes.DEFAULT.value()-> { }
                    ShapeTypes.CLASS_SHAPE.value()-> {
                        if(basicShape is ClassShape){
                            view as ClassView
                            view.class_name.text = basicShape.name
                            view.class_attributes.text = basicShape.attributes.toString()
                            view.class_methods.text = basicShape.methods.toString()
                            view.resize(basicShape.shapeStyle.width.toInt(), basicShape.shapeStyle.height.toInt())
                            view.outlineColor("BLACK")
                        }
                    }
                    ShapeTypes.ARTIFACT.value(), ShapeTypes.ACTIVITY.value(), ShapeTypes.ROLE.value() -> {
                            view as ImageElementView
                            // TODO :  is null : view_image_element_name
                            // view.view_image_element_name.text = basicShape.name
                            view.outlineColor(basicShape.shapeStyle.borderColor)
                            view.resize(basicShape.shapeStyle.width.toInt(), basicShape.shapeStyle.height.toInt())
                    }

                    ShapeTypes.COMMENT.value()-> {
                        view as CommentView
                        // TODO : is null : comment_text 
                        //var commentText: TextView = view.findViewById(R.id.comment_text) as TextView
                        //commentText.text = basicShape.name
                        view.outlineColor(basicShape.shapeStyle.borderColor)
                        view.resize(basicShape.shapeStyle.width.toInt(), basicShape.shapeStyle.height.toInt())

                    }
                    ShapeTypes.PHASE.value()-> {
                        view as PhaseView
                        // TODO : is null :view_phase_name  
                        // view.view_phase_name.text = basicShape.name
                        view.outlineColor(basicShape.shapeStyle.borderColor)
                        view.resize(basicShape.shapeStyle.width.toInt(), basicShape.shapeStyle.height.toInt())
                    }

                }

            }
        }
    }

    public fun syncCanevasFromLayout(){
        for (shape in ViewShapeHolder.getInstance().canevas.shapes){
            val basicElem = ViewShapeHolder.getInstance().map.inverse().getValue(shape.id)
            shape.shapeStyle.coordinates.x = (basicElem.x).toDouble()
            shape.shapeStyle.coordinates.y = (basicElem.y).toDouble()
            shape.shapeStyle.width = basicElem.borderResizableLayout.width.toDouble()
            shape.shapeStyle.height = basicElem.borderResizableLayout.height.toDouble()
            shape.shapeStyle.rotation = basicElem.rotation.toDouble()
        }

    }

    public fun syncCanevasPropsFromLayout(){
        ViewShapeHolder.getInstance().canevas.dimensions.x = (parent_relative_layout.layoutParams.width).toDouble()
        ViewShapeHolder.getInstance().canevas.dimensions.y = (parent_relative_layout.layoutParams.height).toDouble()
    }

    private fun emitClearCanvas(){
        val gson = Gson()
        val canvasEvent: CanvasEvent = CanvasEvent(UserHolder.getInstance().username, ViewShapeHolder.getInstance().canevas)
        val sendObj: String = gson.toJson(canvasEvent)

        Log.d("emitingClearCanvas", sendObj)
        socket?.emit(SocketConstants.REINITIALIZE_CANVAS, sendObj)
    }

    private fun emitAddForm(basicShape: BasicShape){
        var obj: String =""
        val gson = Gson()
        val formsArray: ArrayList<BasicShape> = ArrayList()
        formsArray.add(basicShape)
        val formsUpdate: FormsUpdateEvent = FormsUpdateEvent(UserHolder.getInstance().username,ViewShapeHolder.getInstance().canevas.name, formsArray)
        obj = gson.toJson(formsUpdate)
        Log.d("emitingCreateForm", obj)
        socket?.emit(SocketConstants.CREATE_FORM, obj)

    }

    private fun emitDeleteForm(basicShape: BasicShape){
        var obj: String =""
        val gson = Gson()
        val formsArray: ArrayList<BasicShape> = ArrayList()
        formsArray.add(basicShape)
        val formsUpdate: FormsUpdateEvent=FormsUpdateEvent(UserHolder.getInstance().username, ViewShapeHolder.getInstance().canevas.name, formsArray)
        obj = gson.toJson(formsUpdate)
        Log.d("emitingDelete", obj)
        socket?.emit(SocketConstants.DELETE_FORMS, obj)
    }

    private var onFormsUpdated: Emitter.Listener = Emitter.Listener {
        Log.d("onFormsUpdated", "alllooo")

        val gson = Gson()

        val obj: FormsUpdateEvent = gson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            for(form: BasicShape in obj.forms) {
                Log.d("formsUpdate", obj.username + form.name)
                runOnUiThread {
                    ViewShapeHolder.getInstance().canevas.updateShape(form)
                    syncLayoutFromCanevas()
                }
            }
        }
    }

    private var onFormsSelected: Emitter.Listener = Emitter.Listener {
        Log.d("onFormsSelected", "alllooo")

        val gson = Gson()

        val obj: FormsUpdateEvent = gson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            for(form: BasicShape in obj.forms) {
                Log.d("formsSelect", obj.username + form.name)
                runOnUiThread {
                    val view: BasicElementView? = ViewShapeHolder.getInstance().map.inverse()[form.id]
                    if(view != null) {
                        view.setIsSelectedByOther(true)
                    }

                    syncLayoutFromCanevas()
                }
            }
        }
    }

    private var onFormsDeselected: Emitter.Listener = Emitter.Listener {
        Log.d("onFormsDeselected", "alllooo")

        val gson = Gson()

        val obj: FormsUpdateEvent = gson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            for(form: BasicShape in obj.forms) {
                Log.d("formsDeselect", obj.username + form.name)
                runOnUiThread {
                    val view: BasicElementView? = ViewShapeHolder.getInstance().map.inverse()[form.id]
                    if(view != null) {
                        view.setIsSelectedByOther(false)
                    }

                    syncLayoutFromCanevas()
                }
            }
        }
    }

    private var onFormsDeleted: Emitter.Listener = Emitter.Listener {
        Log.d("onFormsDeleted", "alllooo")

        val gson = Gson()
        val obj: FormsUpdateEvent = gson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            for(form: BasicShape in obj.forms) {
                Log.d("formsDeleted", obj.username + form.name)
                runOnUiThread {
                    val view: BasicElementView? = ViewShapeHolder.getInstance().map.inverse()[form.id]
                    if(view != null) {
                        parent_relative_layout?.removeView(view)
                    }
                    ViewShapeHolder.getInstance().remove(form)
                    ViewShapeHolder.getInstance().stackShapeCreatedId.remove(form.id)

                }
            }
        }
    }

    private var onLinksUpdated: Emitter.Listener = Emitter.Listener {
        Log.d("onLinksUpdated", "alllooo")

        val gson = Gson()

        val obj: LinksUpdateEvent = gson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            for(link: Link in obj.links) {
                Log.d("linksUpdate", obj.username + link.name)
                runOnUiThread {
                    val view: LinkView? = ViewShapeHolder.getInstance().linkMap.inverse()[link.id]
                    ViewShapeHolder.getInstance().canevas.updateLink(link)
                    if(view != null) {
                        view.setLinkAndAnchors(link)
                        view.invalidate()
                        view.requestLayout()
                    }
                }
            }
        }
    }

    private var onLinksSelected: Emitter.Listener = Emitter.Listener {
        Log.d("onLinksSelected", "alllooo")

        val gson = Gson()

        val obj: LinksUpdateEvent = gson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            for(link: Link in obj.links) {
                Log.d("linksSelect", obj.username + link.name)
                runOnUiThread {
                    val view: LinkView? = ViewShapeHolder.getInstance().linkMap.inverse()[link.id]
                    if(view != null) {
                        view.setIsSelectedByOther(true)
                        view.invalidate()
                        view.requestLayout()
                    }
                }
            }
        }
    }

    private var onLinksDeselected: Emitter.Listener = Emitter.Listener {
        Log.d("onLinksDeselected", "alllooo")

        val gson = Gson()

        val obj: LinksUpdateEvent = gson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            for(link: Link in obj.links) {
                Log.d("linksDeselect", obj.username + link.name)
                runOnUiThread {
                    val view: LinkView? = ViewShapeHolder.getInstance().linkMap.inverse()[link.id]
                    if(view != null) {
                        view.setIsSelectedByOther(false)
                        view.invalidate()
                        view.requestLayout()
                    }
                }
            }
        }
    }

    private var onLinksDeleted: Emitter.Listener = Emitter.Listener {
        Log.d("onLinksDeleted", "alllooo")

        val gson = Gson()
        val obj: LinksUpdateEvent = gson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            for(link: Link in obj.links) {
                Log.d("linksDeleted", obj.username + link.name)
                runOnUiThread {
                    val linkView: LinkView? = ViewShapeHolder.getInstance().linkMap.inverse()[link.id]
                    linkView?.deleteLink()
                }
            }
        }
    }

    private var onCanvasReinitialized: Emitter.Listener = Emitter.Listener {
        Log.d("onCanvasReinitialized", "alllooo")
        runOnUiThread {
            ViewShapeHolder.getInstance().removeAll()
            ViewShapeHolder.getInstance().stackShapeCreatedId = Stack<String>()
            parent_relative_layout?.removeAllViews()
            parent_relative_layout.addView(VFXHolder.getInstance().vfxView)
        }
    }

    private var onFormsCreated: Emitter.Listener = Emitter.Listener {
        Log.d("onFormsCreated", "alllooo")

        val gson = Gson()
        val obj: FormsUpdateEvent = gson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            for(form: BasicShape in obj.forms) {
                Log.d("formsCreated", obj.username + form.name)
                runOnUiThread {
                    ViewShapeHolder.getInstance().canevas.addShape(form)
                    addOnCanevas(form)
                }
            }
        }
    }

    private var onLinkCreated: Emitter.Listener = Emitter.Listener {
        Log.d("onLinkCreated", "alllooo")

        val gson = Gson()
        val obj: LinksUpdateEvent = gson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            for(link: Link in obj.links) {
                Log.d("linkCreated", obj.username + link.name)
                runOnUiThread {
                    ViewShapeHolder.getInstance().canevas.addLink(link)
                    val linkView: LinkView = LinkView(this)
                    linkView.setLinkAndAnchors(link)
                    ViewShapeHolder.getInstance().linkMap.forcePut(linkView, link.id)
                    parent_relative_layout?.addView(linkView)
                }
            }
        }
    }

    private var onCanevasResized: Emitter.Listener = Emitter.Listener {
        Log.d("onCanevasResized", "alllooo")

        val gson = Gson()
        val obj: CanvasEvent =  gson.fromJson(it[0].toString())

        if(obj.username != UserHolder.getInstance().username) {
            runOnUiThread {
                ViewShapeHolder.getInstance().canevas.dimensions = obj.canevas.dimensions
                parent_relative_layout.layoutParams.width = (obj.canevas.dimensions.x).toInt()
                parent_relative_layout.layoutParams.height = (obj.canevas.dimensions.y).toInt()
                parent_relative_layout.requestLayout()
            }
        }
    }

    private var onCanevasSelected: Emitter.Listener = Emitter.Listener {
        Log.d("onCanevasSelected", "alllooo")

        val gson = Gson()
        val obj: GalleryEditEvent =  gson.fromJson(it[0].toString())

        if(obj.username != UserHolder.getInstance().username) {
            isCanvasSelectedByYou = false
            runOnUiThread {
                select_canevas_button.setChecked(false)
                select_canevas_button.setEnabled(false)
                parent_relative_layout.setBackgroundResource(R.drawable.borders_red_bg_white)
            }
        } else {
            isCanvasSelectedByYou = true
            runOnUiThread {
                select_canevas_button.setChecked(true)
                select_canevas_button.setEnabled(true)
                parent_relative_layout.setBackgroundResource(R.drawable.borders_blue_bg_white)
                resizeCanvevasButton.setBackgroundResource(R.drawable.ic_resize)
            }
        }
    }

    private var onCanevasDeselected: Emitter.Listener = Emitter.Listener {
        Log.d("onCanevasDeselected", "alllooo")

        val gson = Gson()
        val obj: GalleryEditEvent =  gson.fromJson(it[0].toString())

        runOnUiThread {
            isCanvasSelectedByYou = false
            select_canevas_button.setChecked(false)
            select_canevas_button.setEnabled(true)
            parent_relative_layout.setBackgroundResource(R.drawable.borders_transparent_bg_white)
            resizeCanvevasButton.setBackgroundResource(0)
        }
    }

    private var onGetSelectedForms: Emitter.Listener = Emitter.Listener {
        Log.d("onGetSelectedForms", "alllooo")

        val gson = Gson()
        val obj: GetSelectedFormsResponse =  gson.fromJson(it[0].toString())

        for(formId: String in obj.selectedForms) {
            runOnUiThread {
                val view: BasicElementView? = ViewShapeHolder.getInstance().map.inverse()[formId]
                if(view != null) {
                    view.setIsSelectedByOther(true)
                }
            }
        }
    }

    private var onGetSelectedLinks: Emitter.Listener = Emitter.Listener {
        Log.d("onGetSelectedLinks", "alllooo")

        val gson = Gson()
        val obj: GetSelectedLinksResponse =  gson.fromJson(it[0].toString())

        for(linkId: String in obj.selectedLinks) {
            runOnUiThread {
                val view: LinkView? = ViewShapeHolder.getInstance().linkMap.inverse()[linkId]
                if(view != null) {
                    view.setIsSelectedByOther(true)
                }
            }
        }
    }

    override fun onPause(){
        socket?.off(SocketConstants.FORMS_UPDATED, onFormsUpdated)
        socket?.off(SocketConstants.FORMS_DESELECTED, onFormsDeselected)
        socket?.off(SocketConstants.FORMS_SELECTED, onFormsSelected)
        socket?.off(SocketConstants.FORMS_DELETED, onFormsDeleted)
        socket?.off(SocketConstants.LINKS_UPDATED, onLinksUpdated)
        socket?.off(SocketConstants.LINKS_SELECTED, onLinksSelected)
        socket?.off(SocketConstants.LINKS_DESELECTED, onLinksDeselected)
        socket?.off(SocketConstants.LINKS_DELETED, onLinksDeleted)
        socket?.off(SocketConstants.CANVAS_REINITIALIZED, onCanvasReinitialized)
        socket?.off(SocketConstants.FORM_CREATED, onFormsCreated)
        socket?.off(SocketConstants.LINK_CREATED, onLinkCreated)
        socket?.off(SocketConstants.CANVAS_RESIZED, onCanevasResized)
        socket?.off(SocketConstants.CANVAS_SELECTED, onCanevasSelected)
        socket?.off(SocketConstants.CANVAS_DESELECTED, onCanevasDeselected)
        socket?.off(SocketConstants.SELECTED_FORMS, onGetSelectedForms)
        socket?.off(SocketConstants.SELECTED_LINKS, onGetSelectedLinks)

        super.onPause()
    }

    override fun onBackPressed() {
        if(drawer!!.isDrawerOpen){
            drawer?.closeDrawer()
        } else {
            // TODO : Jé's Fix
            ViewShapeHolder.getInstance().map.clear()

            val gson = Gson()
            val galleryEditEvent: GalleryEditEvent = GalleryEditEvent(
                UserHolder.getInstance().username,
                ViewShapeHolder.getInstance().canevas.name,
                ViewShapeHolder.getInstance().canevas.password
            )
            val sendObj = gson.toJson(galleryEditEvent)
            Log.d("leaveObj", sendObj)
            socket?.emit(SocketConstants.LEAVE_CANVAS_ROOM, sendObj)
            val intent = Intent(this, GalleryActivity::class.java)
            intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TOP
            startActivity(intent)
//        finish()
        }
    }

    private fun saveCanevas() {
        Log.d("saveCanevas", "saveCanevasCall")

        val bitmap: Bitmap = loadBitmapFromView(findViewById(R.id.parent_relative_layout), 350, 450);
        val thumbnailString: String = bitMapToString(bitmap)
        ViewShapeHolder.getInstance().canevas.thumbnailLeger = thumbnailString

        val canvasEvent: CanvasEvent = CanvasEvent(UserHolder.getInstance().username, ViewShapeHolder.getInstance().canevas!!)
        val gson = Gson()
        val sendObj = gson.toJson(canvasEvent)
        Log.d("createObj", sendObj)
        socket?.emit(SocketConstants.SAVE_CANVAS, sendObj)

    }

    private fun loadBitmapFromView(v: View, width: Int, height: Int): Bitmap {
        Log.d("loadBitmapFromView", "loadBitmapFromViewCall")

        val b = Bitmap.createBitmap( v.width, v.height, Bitmap.Config.ARGB_8888)
        val c = Canvas(b)
        v.layout(0, 0, v.layoutParams.width, v.layoutParams.height)
        v.draw(c)
        Log.d("v.draw(c) (la Bitmap b)", b.toString())

        return b
    }

    fun bitMapToString(bitmap: Bitmap): String {
        val baos = ByteArrayOutputStream()
        bitmap.compress(Bitmap.CompressFormat.PNG, 100, baos)
        val b = baos.toByteArray()
        val temp = Base64.encodeToString(b, Base64.DEFAULT)
        return temp
    }

    open protected var onTouchListenerResizeButton = View.OnTouchListener { v, event ->
        when(event.action){
            MotionEvent.ACTION_DOWN -> {
                if (isCanvasSelectedByYou) {
                    oldFrameRawX = event.rawX
                    oldFrameRawY = event.rawY
                }
            }
            MotionEvent.ACTION_MOVE -> {
                if (isCanvasSelectedByYou) {
                    var deltaX: Int = (event.rawX - oldFrameRawX).toInt()
                    var deltaY: Int = (event.rawY - oldFrameRawY).toInt()
                    val newWidth = parent_relative_layout.width + deltaX
                    val newHeight = parent_relative_layout.height + deltaY

                    resize(newWidth, newHeight)

                    oldFrameRawX = event.rawX
                    oldFrameRawY = event.rawY
                }
            }
            MotionEvent.ACTION_UP -> {
                if (isCanvasSelectedByYou) {
                    syncCanevasPropsFromLayout()
                    emitCanevasUpdate()
                }

            }
        }
        true
    }

    open fun resize(newWidth:Int, newHeight:Int){
        if(newWidth >= mMinimumWidth && newHeight <= mMaximumWidth){
            parent_relative_layout.layoutParams.width = newWidth
        }
        if(newHeight >= mMinimumHeight && newHeight <= mMaximumHeight){
            parent_relative_layout.layoutParams.height = newHeight
        }
        parent_relative_layout.requestLayout()
    }

    private fun emitCanevasUpdate(){
        val dataStr: String = this.createCanevasUpdateEvent()

        if(dataStr !="") {
            Log.d("emitingUpdate", dataStr)
            socket?.emit(SocketConstants.RESIZE_CANVAS, dataStr)
        }
    }

    private fun createCanevasUpdateEvent(): String {
        val gson = Gson()
        val canvasEvent: CanvasEvent = CanvasEvent(UserHolder.getInstance().username, ViewShapeHolder.getInstance().canevas)
        val sendObj: String = gson.toJson(canvasEvent)
        return sendObj
    }


    open protected var onSelectCanvevas = CompoundButton.OnCheckedChangeListener { _, isChecked ->
        val gson = Gson()
        val canvasEvent: GalleryEditEvent = GalleryEditEvent(UserHolder.getInstance().username, ViewShapeHolder.getInstance().canevas.name, "")
        val dataStr: String = gson.toJson(canvasEvent)

        if(isChecked) {
            socket?.emit(SocketConstants.SELECT_CANVAS, dataStr)
        } else {
            socket?.emit(SocketConstants.DESELECT_CANVAS, dataStr)

        }
    }

    open protected var onSelectLasso = CompoundButton.OnCheckedChangeListener { _, isChecked ->

        if(isChecked) {
            lassoView = LassoView(this)
            parent_relative_layout?.addView(lassoView)
            parent_relative_layout?.dispatchSetSelected(false)
        } else {
            parent_relative_layout?.removeView(lassoView)
        }
    }

    /*override fun onBackPressed() {
        socket?.off(SocketConstants.CANVAS_UPDATE_TEST_RESPONSE, onCanvasUpdate)
        socket?.off(SocketConstants.JOIN_CANVAS_TEST_RESPONSE, onJoinCanvas)
        socket?.off(SocketConstants.FORMS_UPDATED, onFormsUpdated)
        super.onBackPressed()
    }*/
    /*
    override fun onBackPressed() {
        val intent = Intent(this, GalleryActivity::class.java)
        startActivity(intent)
        val app = application as PolyPaint
        val socket: Socket? = app.socket
        socket?.disconnect()
        val intent = Intent(this, LoginActivity::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TOP
        startActivity(intent)
        finish()

    }*/
}