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
import com.mikepenz.materialdrawer.Drawer
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Enum.ShapeTypes
import com.polypaint.polypaint.Model.*
import com.polypaint.polypaint.R
import com.polypaint.polypaint.Socket.SocketConstants
import com.polypaint.polypaint.View.*
import kotlinx.android.synthetic.main.activity_drawing.*
import kotlinx.android.synthetic.main.basic_element.view.*
import java.lang.NullPointerException
import java.util.*
import kotlin.collections.ArrayList
import android.widget.RelativeLayout
import kotlinx.android.synthetic.main.view_class.view.*
import kotlinx.android.synthetic.main.view_image_element.view.*
import kotlinx.android.synthetic.main.view_phase.view.*
import android.graphics.Bitmap
import android.os.Handler
import java.io.ByteArrayOutputStream
import android.util.Base64
import android.view.MotionEvent
import android.widget.CompoundButton
import android.widget.TextView
import androidx.fragment.app.DialogFragment
import com.github.salomonbrys.kotson.get
import com.google.gson.*
import com.polypaint.polypaint.Enum.LinkTypes
import com.polypaint.polypaint.Fragment.TutorialDialogFragment
import com.polypaint.polypaint.Holder.*
import com.polypaint.polypaint.ResponseModel.GetSelectedFormsResponse
import com.polypaint.polypaint.ResponseModel.GetSelectedLinksResponse
import com.polypaint.polypaint.ResponseModel.HasUserDoneTutorialResponse
import com.polypaint.polypaint.SocketReceptionModel.*
import kotlinx.android.synthetic.main.toolbar.*
import kotlinx.android.synthetic.main.view_freetext.view.*
import java.lang.Exception
import java.lang.reflect.Type


class DrawingActivity : AppCompatActivity(){

    var oldFrameRawX : Float = 0.0F
    var oldFrameRawY : Float = 0.0F
    var mMinimumWidth : Float = 450F
    var mMinimumHeight : Float = 450F
    var mMaximumWidth : Float = 1680F
    var mMaximumHeight : Float = 1155F
    val shapeOffset: Float = 61F

    var lassoView: LassoView? = null

    var isCanvasSelectedByYou : Boolean = false

    private var inflater : LayoutInflater? = null

    private var drawer: Drawer? = null
    private var socket: Socket? = null

    private var shapesToAdd: ArrayList<BasicShape> = ArrayList<BasicShape>()
    private var linksToAdd: ArrayList<Link> = ArrayList<Link>()

    private var clipboard: ArrayList<DrawingElement> = ArrayList()
    private var stackDrawingElement: Stack<DrawingElement> = Stack()

    @SuppressLint("ClickableViewAccessibility")
    override fun onCreate (savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE
        setContentView(R.layout.activity_drawing)

        val activityToolbar : Toolbar = findViewById(R.id.toolbar)
        setSupportActionBar(activityToolbar)

        help_button.setOnClickListener {
            showTutorial()
        }

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

            toolbar = activityToolbar
        }

//        ViewShapeHolder.getInstance().canevas = intent.getSerializableExtra("canevas") as Canevas
        ViewShapeHolder.getInstance().canevas.shapes.clear()
        ViewShapeHolder.getInstance().canevas.links.clear()

        canevas_title.text = ViewShapeHolder.getInstance().canevas.name

        inflater = getSystemService(Context.LAYOUT_INFLATER_SERVICE) as LayoutInflater


        parent_relative_layout.setOnClickListener{
            it as RelativeLayout
            it.dispatchSetSelected(false)
        }

        freetext_button.setOnClickListener {
            selection_button.isChecked = false
            addOnCanevas(ShapeTypes.FREETEXT)
            //saveCanevas()
            SyncShapeHolder.getInstance().saveCanevas()
        }

        class_button.setOnClickListener {
            selection_button.isChecked = false
            addOnCanevas(ShapeTypes.CLASS_SHAPE)
            //saveCanevas()
            SyncShapeHolder.getInstance().saveCanevas()
        }
        artefact_button.setOnClickListener {
            selection_button.isChecked = false
            addOnCanevas(ShapeTypes.ARTIFACT)
            Log.d("before delay", "saveCanevasCall")
            //saveCanevas()
            SyncShapeHolder.getInstance().saveCanevas()
        }
        activity_button.setOnClickListener {
            selection_button.isChecked = false
            addOnCanevas(ShapeTypes.ACTIVITY)
            //saveCanevas()
            SyncShapeHolder.getInstance().saveCanevas()
        }
        role_button.setOnClickListener {
            selection_button.isChecked = false
            addOnCanevas(ShapeTypes.ROLE)
            //saveCanevas()
            SyncShapeHolder.getInstance().saveCanevas()
        }
        comment_button.setOnClickListener {
            selection_button.isChecked = false
            addOnCanevas(ShapeTypes.COMMENT)
            //saveCanevas()
            SyncShapeHolder.getInstance().saveCanevas()
        }
        phase_button.setOnClickListener {
            selection_button.isChecked = false
            addOnCanevas(ShapeTypes.PHASE)
            //saveCanevas()
            SyncShapeHolder.getInstance().saveCanevas()
        }
        link_button.setOnClickListener {
            selection_button.isChecked = false
            var linkDefaultPath : ArrayList<Coordinates> = ArrayList()
            linkDefaultPath.add(Coordinates(65.0,65.0))
            linkDefaultPath.add(Coordinates(275.0,275.0))
            var newLink: Link = Link(UUID.randomUUID().toString(),"Link", AnchorPoint(), AnchorPoint(), 2, LinkStyle("#FF000000",0,0), linkDefaultPath )

            ViewShapeHolder.getInstance().canevas.addLink(newLink)
            val linkView: LinkView = LinkView(this)
            linkView.setLinkAndAnchors(newLink)
            ViewShapeHolder.getInstance().linkMap.forcePut(linkView, newLink.id)
            val linksToUpdate = ArrayList<Link>()
            linksToUpdate.add(newLink)
            var linkObj: String =""
            val gson = Gson()
            val linkUpdate: LinksUpdateEvent = LinksUpdateEvent(UserHolder.getInstance().username, ViewShapeHolder.getInstance().canevas.name, linksToUpdate)
            linkObj = gson.toJson(linkUpdate)
            Log.d("emitingCreateLink", linkObj)
            socket?.emit(SocketConstants.CREATE_LINK, linkObj)
            parent_relative_layout?.addView(linkView)
            ViewShapeHolder.getInstance().stackDrawingElementCreatedId.push(newLink.id)
            //saveCanevas()
            SyncShapeHolder.getInstance().saveCanevas()
        }

        clear_canvas_button.setOnClickListener {
            selection_button.isChecked = false
            emitClearCanvas()
            parent_relative_layout?.removeAllViews()
            ViewShapeHolder.getInstance().removeAll()
            ViewShapeHolder.getInstance().stackDrawingElementCreatedId = Stack<String>()
            parent_relative_layout?.addView(VFXHolder.getInstance().vfxView)
            //saveCanevas()
            SyncShapeHolder.getInstance().saveCanevas()
        }

        duplicate_button.setOnClickListener{
            //selection_button.isChecked = false
            duplicateView()
            //saveCanevas()
            SyncShapeHolder.getInstance().saveCanevas()
        }
        cut_button.setOnClickListener{
            selection_button.isChecked = false
            cutView()
            //saveCanevas()
            SyncShapeHolder.getInstance().saveCanevas()
        }
        stack_button.setOnClickListener{
            //selection_button.isChecked = false
            stackView()
            //saveCanevas()
            SyncShapeHolder.getInstance().saveCanevas()
        }
        unstack_button.setOnClickListener{
            //selection_button.isChecked = false
            unstackView()
            //saveCanevas()
            SyncShapeHolder.getInstance().saveCanevas()
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

    }

    private fun initializeViewFromCanevas(){
        if(ViewShapeHolder.getInstance().canevas != null){
            Log.d("init","****"+ViewShapeHolder.getInstance().canevas.name+"****")

            runOnUiThread {
                // Adding SHAPES
                for (form: BasicShape in shapesToAdd) {
                    Log.d("INFORLOOP", "********")
                    ViewShapeHolder.getInstance().canevas.addShape(form)
                    addOnCanevas(form)
                }
                shapesToAdd.clear()

                // TODO : review Adding LINKS
                for (link: Link in linksToAdd) {
                    ViewShapeHolder.getInstance().canevas.addLink(link)
                    val linkView: LinkView = LinkView(this)
                    linkView.setLinkAndAnchors(link)
                    ViewShapeHolder.getInstance().linkMap.forcePut(linkView, link.id)
                    linkView.setPaintColorWithLinkStyle()
                    parent_relative_layout?.addView(linkView)

                }
                linksToAdd.clear()

                // Sizing the Canvas
                resize((ViewShapeHolder.getInstance().canevas.dimensions.x).toInt(), (ViewShapeHolder.getInstance().canevas.dimensions.y).toInt())

            }
            // Selecting selected Forms and Links
            val app = application as PolyPaint
            val gson = Gson()
            val galleryEditEvent: GalleryEditEvent = GalleryEditEvent(
                UserHolder.getInstance().username,
                ViewShapeHolder.getInstance().canevas.name,
                ViewShapeHolder.getInstance().canevas.password
            )
            val sendObj = gson.toJson(galleryEditEvent)
            app.socket?.emit(SocketConstants.GET_SELECTED_FORMS, sendObj)
            app.socket?.emit(SocketConstants.GET_SELECTED_LINKS, sendObj)
        }

    }

    override fun onResume() {
        super.onResume()
        val app = application as PolyPaint
        socket = app.socket

//        toolbar_login_button.visibility = View.INVISIBLE

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
        socket?.on(SocketConstants.HAS_USER_DONE_TUTORIAL_RESPONSE, onHasUserDoneTutorial)
        socket?.on(SocketConstants.GET_CANVAS_RESPONSE, onGetCanevas)

        getCanevas()
    }

    private fun addOnCanevas(shapeType: ShapeTypes){
        var shape = newShapeOnCanevas(shapeType)
        var view = newViewOnCanevas(shapeType)

        //addViewToLayout
        parent_relative_layout?.addView(view)
        parent_relative_layout?.dispatchSetSelected(false)

        //addShapeToCanevas
        ViewShapeHolder.getInstance().canevas.addShape(shape)
        //mapViewAndShapeId
        ViewShapeHolder.getInstance().map.forcePut(view, shape.id)
        //stackFor Stack/Unstack
        ViewShapeHolder.getInstance().stackDrawingElementCreatedId.push(shape.id)

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

        view.isSelected=true


        //LAUNCH VFX
        VFXHolder.getInstance().fireVFX(
            (shape.shapeStyle.coordinates.x + 100F).toFloat(),
            (shape.shapeStyle.coordinates.y + 100F).toFloat(),this)
        //Play Sound VFX
        PlaySoundHolder.getInstance().playNotification1(this)

    }

    private fun addOnCanevas(basicShape: BasicShape){
        Log.d("6666","****"+ViewShapeHolder.getInstance().map+"****")

        //TODO: Probablement une meilleure facon de mapper la value à l'enum ...
        runOnUiThread {
            when (basicShape.type) {
//                ShapeTypes.DEFAULT.value() -> {
//                    val viewType = newViewOnCanevas(ShapeTypes.DEFAULT)
//                    parent_relative_layout?.addView(viewType)
//
//                    //For Sync
//                    ViewShapeHolder.getInstance().map.put(viewType, basicShape.id)
//                }
                ShapeTypes.CLASS_SHAPE.value() -> {
                    val viewType = newViewOnCanevas(ShapeTypes.CLASS_SHAPE)
                    parent_relative_layout?.addView(viewType)

                    //For Sync
                    ViewShapeHolder.getInstance().map.forcePut(viewType, basicShape.id)
                }
                ShapeTypes.ARTIFACT.value() -> {
                    val viewType = newViewOnCanevas(ShapeTypes.ARTIFACT)
                    parent_relative_layout?.addView(viewType)

                    //For Sync
                    ViewShapeHolder.getInstance().map.forcePut(viewType, basicShape.id)
                }
                ShapeTypes.ACTIVITY.value() -> {
                    val viewType = newViewOnCanevas(ShapeTypes.ACTIVITY)
                    parent_relative_layout?.addView(viewType)

                    //For Sync
                    ViewShapeHolder.getInstance().map.forcePut(viewType, basicShape.id)
                }
                ShapeTypes.ROLE.value() -> {
                    val viewType = newViewOnCanevas(ShapeTypes.ROLE)
                    parent_relative_layout?.addView(viewType)

                    //For Sync
                    ViewShapeHolder.getInstance().map.forcePut(viewType, basicShape.id)
                }
                ShapeTypes.COMMENT.value() -> {
                    val viewType = newViewOnCanevas(ShapeTypes.COMMENT)
                    parent_relative_layout?.addView(viewType)

                    //For Sync
                    ViewShapeHolder.getInstance().map.forcePut(viewType, basicShape.id)
                }
                ShapeTypes.PHASE.value() -> {
                    val viewType = newViewOnCanevas(ShapeTypes.PHASE)
                    parent_relative_layout?.addView(viewType)

                    //For Sync
                    ViewShapeHolder.getInstance().map.forcePut(viewType, basicShape.id)
                }
                ShapeTypes.FREETEXT.value() -> {
                    val viewType = newViewOnCanevas(ShapeTypes.FREETEXT)
                    parent_relative_layout?.addView(viewType)

                    //For Sync
                    ViewShapeHolder.getInstance().map.forcePut(viewType, basicShape.id)
                }

            }
            syncLayoutFromCanevas()
        }


    }

    private fun newShapeOnCanevas(shapeType: ShapeTypes) : BasicShape{
        var shapeStyle = ShapeStyle(Coordinates(shapeOffset.toDouble(),shapeOffset.toDouble()), 10.0, 10.0, 0.0, "black", 0, "white")
        var shape = BasicShape(UUID.randomUUID().toString(), shapeType.value(), "defaultShape1", shapeStyle, ArrayList<String?>(), ArrayList<String?>())

        when (shapeType) {
//            ShapeTypes.DEFAULT -> {}
            ShapeTypes.CLASS_SHAPE -> {
                shapeStyle.width = 168.0
                shapeStyle.height = 189.0
                shape = ClassShape(UUID.randomUUID().toString(), shapeType.value(), "Class", shapeStyle, ArrayList<String?>(), ArrayList<String?>(),ArrayList<String?>(), ArrayList<String?>())
            }
            ShapeTypes.ARTIFACT -> {
                shapeStyle.width = 168.0
                shapeStyle.height = 168.0
                shape = BasicShape(UUID.randomUUID().toString(), shapeType.value(), "Artifact", shapeStyle, ArrayList<String?>(), ArrayList<String?>())
            }
            ShapeTypes.ACTIVITY -> {
                shapeStyle.width = 168.0
                shapeStyle.height = 168.0
                shape = BasicShape(UUID.randomUUID().toString(), shapeType.value(), "Activity", shapeStyle, ArrayList<String?>(), ArrayList<String?>())
            }
            ShapeTypes.ROLE -> {
                shapeStyle.width = 168.0
                shapeStyle.height = 168.0
                shape = BasicShape(UUID.randomUUID().toString(), shapeType.value(), "Actor", shapeStyle, ArrayList<String?>(), ArrayList<String?>())
            }
            ShapeTypes.COMMENT -> {
                shapeStyle.width = 189.0
                shapeStyle.height = 189.0
                shape = BasicShape(UUID.randomUUID().toString(), shapeType.value(), "Comment", shapeStyle, ArrayList<String?>(), ArrayList<String?>())
            }
            ShapeTypes.PHASE -> {
                shapeStyle.width = 189.0
                shapeStyle.height = 189.0
                shape = BasicShape(UUID.randomUUID().toString(), shapeType.value(), "Phase", shapeStyle, ArrayList<String?>(), ArrayList<String?>())
            }
            ShapeTypes.FREETEXT->{
                shapeStyle.width = 189.0
                shapeStyle.height = 189.0
                shape = BasicShape(UUID.randomUUID().toString(), shapeType.value(), "Text", shapeStyle, ArrayList<String?>(), ArrayList<String?>())
            }
        }

        return shape
    }

    private fun newViewOnCanevas(shapeType : ShapeTypes) : BasicElementView{
        var viewType : BasicElementView = BasicElementView(this)
        val viewContainer = inflater!!.inflate(R.layout.basic_element, null)

        when(shapeType){
//            ShapeTypes.DEFAULT->{
//                viewType = BasicElementView(this)
//            }
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
            ShapeTypes.FREETEXT->{
                viewType = FreeTextView(this)
            }
        }
        viewType.addView(viewContainer)

        return viewType
    }

    private fun duplicateView(){
        if(clipboard.isEmpty()){
            var shapesToDuplicate : ArrayList<DrawingElement> = ArrayList()
            //BASIC ELEMENT VIEW
            //Copying list to avoid ConcurrentModificationException
            val list = ViewShapeHolder.getInstance().map.keys.toMutableList()
            for (view in list){
                if(view.isSelected && !view.isSelectedByOther) {
                    view.isSelected = false
                    val drawingElementToDuplicate = ViewShapeHolder.getInstance().canevas.findShape(ViewShapeHolder.getInstance().map.getValue(view))
                    shapesToDuplicate.add(drawingElementToDuplicate!!)
                }
            }
            //LINK VIEW

            val listLink = ViewShapeHolder.getInstance().linkMap.keys.toMutableList()
            for (view in listLink){
                if(view.isSelected && !view.isSelectedByOther) {
                    //if(view.link?.to?.formId == "" && view.link?.from == AnchorPoint()){
                        view.isSelected = false
                        val drawingElementToDuplicate = ViewShapeHolder.getInstance().canevas.findLink(ViewShapeHolder.getInstance().linkMap.getValue(view))
                        shapesToDuplicate.add(drawingElementToDuplicate!!)
                    //}
                }
            }

            //Add All DrawingElementOnCanevas
            var drawingElementDuplicated : ArrayList<DrawingElement> = ArrayList()
            if (shapesToDuplicate != null) {
                for (drawingElem in shapesToDuplicate){
                    if(drawingElem is BasicShape){
                        var shapeDuplicated = drawingElem.copy()
                        if(drawingElem is ClassShape) {
                            shapeDuplicated = drawingElem.copyClass()
                        }
                        //val shapeDuplicated = drawingElem.copy()
                        shapeDuplicated.id = UUID.randomUUID().toString()
                        drawingElementDuplicated.add(shapeDuplicated)
                        ViewShapeHolder.getInstance().canevas.addShape(shapeDuplicated)
                        addOnCanevas(shapeDuplicated)
                        emitAddForm(shapeDuplicated)

                        ViewShapeHolder.getInstance().map.inverse()[shapeDuplicated.id]?.isSelected = true

                        ViewShapeHolder.getInstance().stackDrawingElementCreatedId.push(shapeDuplicated.id)

                    }else if(drawingElem is Link){
                        var linkDuplicated = drawingElem.copy()
                        //linkDuplicated.id = UUID.randomUUID().toString()
                        drawingElementDuplicated.add(linkDuplicated)
                        ViewShapeHolder.getInstance().stackDrawingElementCreatedId.push(linkDuplicated.id)
                        //emitAddLink ?
                        runOnUiThread {
                            ViewShapeHolder.getInstance().canevas.addLink(linkDuplicated)
                            val linkView: LinkView = LinkView(this)

                            linkView.setLinkAndAnchors(linkDuplicated)
                            ViewShapeHolder.getInstance().linkMap.forcePut(linkView, linkDuplicated.id)
                            parent_relative_layout?.addView(linkView)
                            linkView.isSelected = true
                        }
                    }
                }
            }

            if(lassoView != null){
                Log.d("duplicateWithLasso","not null")
                lassoView!!.linksIn = ArrayList()
                lassoView!!.viewsIn = ArrayList()

                for (drawingElem in drawingElementDuplicated){
                    if(drawingElem is BasicShape){
                        ViewShapeHolder.getInstance().map.inverse()[drawingElem.id]?.hideButtonsAndAnchors()
                        lassoView!!.viewsIn.add(ViewShapeHolder.getInstance().map.inverse()[drawingElem.id]!!)
                    }else if (drawingElem is Link){
                        ViewShapeHolder.getInstance().linkMap.inverse()[drawingElem.id]?.boundingBox?.isLasso = true
                        ViewShapeHolder.getInstance().linkMap.inverse()[drawingElem.id]?.hideButtons()
                        lassoView!!.linksIn.add(ViewShapeHolder.getInstance().linkMap.inverse()[drawingElem.id]!!)

                    }
                }

            }
        }else{
            for(drawingElem in clipboard){
                if(drawingElem is BasicShape){
                    ViewShapeHolder.getInstance().canevas.addShape(drawingElem)
                    ViewShapeHolder.getInstance().stackDrawingElementCreatedId.push(drawingElem.id)
                    addOnCanevas(drawingElem)
                    emitAddForm(drawingElem)

                    ViewShapeHolder.getInstance().map.inverse().getValue(drawingElem.id).isSelected = true
                }else if(drawingElem is Link){
                    //TODO : LINKS
                    ViewShapeHolder.getInstance().stackDrawingElementCreatedId.push(drawingElem.id)
                    //emitAddLink ?
                    runOnUiThread {
                        ViewShapeHolder.getInstance().canevas.addLink(drawingElem)
                        val linkView: LinkView = LinkView(this)
                        linkView.setLinkAndAnchors(drawingElem)
                        ViewShapeHolder.getInstance().linkMap.forcePut(linkView, drawingElem.id)
                        parent_relative_layout?.addView(linkView)
                    }
                }
            }
            clipboard = ArrayList()
        }
    }

    private fun cutView(){
        clipboard = ArrayList()
        //BasicElementView
        val list = ViewShapeHolder.getInstance().map.keys.toMutableList()
        for (view in list){
            if(view.isSelected && !view.isSelectedByOther){
                val shapeToCut = ViewShapeHolder.getInstance().canevas.findShape(ViewShapeHolder.getInstance().map.getValue(view))
                //Couper les liens
                if(shapeToCut != null){
                    for(linkToId in shapeToCut.linksTo){
                        ViewShapeHolder.getInstance().canevas.findLink(linkToId!!)?.to = AnchorPoint()
                    }
                    shapeToCut.linksTo = ArrayList()

                    for(linkFromId in shapeToCut.linksFrom){
                        ViewShapeHolder.getInstance().canevas.findLink(linkFromId!!)?.from = AnchorPoint()
                    }
                    shapeToCut.linksFrom = ArrayList()
                }

                clipboard.add(shapeToCut!!)
                emitDeleteForm(shapeToCut!!)
                parent_relative_layout.removeView(view)
                ViewShapeHolder.getInstance().remove(view)

                ViewShapeHolder.getInstance().stackDrawingElementCreatedId.remove(shapeToCut.id)
            }
        }
        //LinkView
        val listLink = ViewShapeHolder.getInstance().linkMap.keys.toMutableList()
        for (view in listLink){
            if(view.isSelected && !view.isSelectedByOther){
                val linkToCut = ViewShapeHolder.getInstance().canevas.findLink(ViewShapeHolder.getInstance().linkMap.getValue(view))
                clipboard.add(linkToCut!!)

                //TODO: Comment tu enlève un linkView de parent_relative_layout
                view.deleteLink()
                ViewShapeHolder.getInstance().stackDrawingElementCreatedId.remove(linkToCut.id)
            }
        }
    }

    private fun stackView(){
        //Max : Comme c'est là, on peut stack qqc qui est sélectionné par les autres, plus facile comme ça, et pas spécifié dans le complément
        try {
            var idToStack = ViewShapeHolder.getInstance().stackDrawingElementCreatedId.pop()

            var drawingToStack = ViewShapeHolder.getInstance().findDrawingElement(idToStack)
            Log.d("stackView", "id: "+idToStack)
            //Basic Shape
            if(drawingToStack is BasicShape){
                Log.d("stackView", "S: "+drawingToStack.name+" "+idToStack)
                //Couper les liens
                for(linkToId in drawingToStack.linksTo){
                    ViewShapeHolder.getInstance().canevas.findLink(linkToId!!)?.to = AnchorPoint()
                }
                drawingToStack.linksTo = ArrayList()

                for(linkFromId in drawingToStack.linksFrom){
                    ViewShapeHolder.getInstance().canevas.findLink(linkFromId!!)?.from = AnchorPoint()
                }
                drawingToStack.linksFrom = ArrayList()

                stackDrawingElement.push(drawingToStack)
                emitDeleteForm(drawingToStack!!)

                var viewToRemove = ViewShapeHolder.getInstance().map.inverse().getValue(idToStack)
                parent_relative_layout.removeView(viewToRemove)
                ViewShapeHolder.getInstance().remove(viewToRemove)

                lassoView?.viewsIn?.remove(viewToRemove)
            }
            //Link
            else if(drawingToStack is Link){
                Log.d("stackView", "L: "+drawingToStack.name+" "+idToStack)

                ViewShapeHolder.getInstance().linkMap.inverse()[idToStack]?.deleteLink()
                stackDrawingElement.push(drawingToStack)

                lassoView?.linksIn?.remove(ViewShapeHolder.getInstance().linkMap.inverse()[idToStack])
            }

        }catch (e : EmptyStackException){}

    }
    private fun unstackView(){
        try {
            val shapeUnstacked = stackDrawingElement.pop()
            if(shapeUnstacked is BasicShape){
                ViewShapeHolder.getInstance().canevas.addShape(shapeUnstacked)
                addOnCanevas(shapeUnstacked)

                emitAddForm(shapeUnstacked)
                ViewShapeHolder.getInstance().stackDrawingElementCreatedId.push(shapeUnstacked.id)
            }else if(shapeUnstacked is Link){
                //TODO : LINKS
                Log.d("unstackView", "Link "+shapeUnstacked.name)
                ViewShapeHolder.getInstance().stackDrawingElementCreatedId.push(shapeUnstacked.id)
                //EmitAddLink ?
                runOnUiThread {
                    ViewShapeHolder.getInstance().canevas.addLink(shapeUnstacked)
                    val linkView: LinkView = LinkView(this)
                    linkView.setLinkAndAnchors(shapeUnstacked)
                    ViewShapeHolder.getInstance().linkMap.forcePut(linkView, shapeUnstacked.id)
                    parent_relative_layout?.addView(linkView)
                }
            }


        }catch (e : EmptyStackException){}
        catch (e : NullPointerException){} //If stacking deleted shape
    }
    public fun syncLayoutFromCanevas(){
        Log.d("syncLayoutFromCanevas","***wawaw****")

        for (view in ViewShapeHolder.getInstance().map.keys){

            val basicShapeId: String = ViewShapeHolder.getInstance().map.getValue(view)
            val basicShape: BasicShape? = ViewShapeHolder.getInstance().canevas.findShape(basicShapeId)
            if(!view.isSelected) {
                if (basicShape != null) {
                    view.x = (basicShape.shapeStyle.coordinates.x).toFloat() - shapeOffset
                    view.y = (basicShape.shapeStyle.coordinates.y).toFloat() - shapeOffset
                    view.leftX = view.x
                    view.topY = view.y
                    view.rotation = basicShape.shapeStyle.rotation.toFloat()

                    // TODO : Jé's Fix : j'ai bougé les view.resize dans les différents case pour que la fonction redéfinie des enfants de BasicShape soit appelée (ex.: pour que la fonction .resize de ImageElementView soit appelée)
                    // TODO : les attributs xml des différentes View ne sont pas reconnues même avec le cast de la view (ex.:view as ImageElementView), voir les "// TODO : is null" ci-dessous, je n'ai pas trouvé pourquoi ça faisait cela^^
                    when (basicShape.type) {
//                        ShapeTypes.DEFAULT.value() -> {
//                        }
                        ShapeTypes.CLASS_SHAPE.value() -> {
                            if (basicShape is ClassShape) {
                                runOnUiThread {
                                    view as ClassView
                                    Log.d(
                                        "syncLayoutFromCanevas",
                                        basicShape.name + " w " + basicShape.shapeStyle.width.toInt() + " h " + basicShape.shapeStyle.height.toInt()
                                    )
                                    view.class_name.text = basicShape.name
                                    var tmp: String = ""
                                    if (basicShape.attributes != null) {
                                        for (e in basicShape.attributes) {
                                            tmp += e + "\n"
                                        }
                                    }
                                    view.class_attributes.text = tmp
                                    var tmp2 = ""
                                    if (basicShape.methods != null) {
                                        for (e in basicShape.methods) {
                                            tmp2 += e + "\n"
                                        }
                                    }
                                    view.class_methods.text = tmp2
                                    view.resize(
                                        basicShape.shapeStyle.width.toInt(),
                                        basicShape.shapeStyle.height.toInt()
                                    )

                                }
                            }
                        }
                        ShapeTypes.ARTIFACT.value(), ShapeTypes.ACTIVITY.value(), ShapeTypes.ROLE.value() -> {
                            runOnUiThread {
                                view as ImageElementView
                                // TODO :  is null : view_image_element_name
                                view.view_image_element_name.text = basicShape.name
                                //view.outlineColor(basicShape.shapeStyle.borderColor, basicShape.shapeStyle.borderStyle)
                                //view.backgroundColor(basicShape.shapeStyle.backgroundColor)
                                view.resize(basicShape.shapeStyle.width.toInt(), basicShape.shapeStyle.height.toInt())
                            }

                        }
                        ShapeTypes.COMMENT.value() -> {
                            runOnUiThread {
                                view as CommentView
                                var commentText: TextView = view.findViewById(R.id.comment_text) as TextView
                                commentText.text = basicShape.name
                                //view.outlineColor(basicShape.shapeStyle.borderColor, basicShape.shapeStyle.borderStyle)
                                //view.backgroundColor(basicShape.shapeStyle.backgroundColor)
                                view.resize(basicShape.shapeStyle.width.toInt(), basicShape.shapeStyle.height.toInt())
                            }

                        }

                        ShapeTypes.PHASE.value() -> {
                            runOnUiThread {
                                view as PhaseView
                                view.view_phase_name.text = basicShape.name
//                                view.outlineColor(basicShape.shapeStyle.borderColor, basicShape.shapeStyle.borderStyle)
                                view.resize(basicShape.shapeStyle.width.toInt(), basicShape.shapeStyle.height.toInt())
                                //view.backgroundColor(basicShape.shapeStyle.backgroundColor)
                            }
                        }
                        ShapeTypes.FREETEXT.value() -> {
                            runOnUiThread {
                                view as FreeTextView
                                view.free_text_text.text = basicShape.name
                                //view.outlineColor(basicShape.shapeStyle.borderColor, basicShape.shapeStyle.borderStyle)
                                view.resize(basicShape.shapeStyle.width.toInt(), basicShape.shapeStyle.height.toInt())
                                //view.backgroundColor(basicShape.shapeStyle.backgroundColor)
                            }
                        }

                    }
                }
            }
            if (basicShape != null) {
                runOnUiThread {
                    view.outlineColor(
                        basicShape.shapeStyle.borderColor,
                        basicShape.shapeStyle.borderStyle
                    )
                    view.backgroundColor(basicShape.shapeStyle.backgroundColor)
                }
            }
        }
    }

    fun showTutorial(){
        var activity: AppCompatActivity = this@DrawingActivity as AppCompatActivity
        var dialog: TutorialDialogFragment = TutorialDialogFragment()
        //var bundle: Bundle = Bundle()
        //bundle.putSerializable("canevas", selectedCanevas)
        //dialog.arguments = bundle

        //Log.d("****", dialog.arguments.toString())
        dialog.showModal(activity.supportFragmentManager, "TutorialDialog")
    }

    public fun syncCanevasFromLayout(){
        for (shape in ViewShapeHolder.getInstance().canevas.shapes){
            val basicElem = ViewShapeHolder.getInstance().map.inverse().getValue(shape.id)

            Log.d("syncCanevasFromLayout", shape.name+" w "+basicElem.borderResizableLayout.width+" h "+basicElem.borderResizableLayout.height)
            shape.shapeStyle.coordinates.x = (basicElem.x).toDouble() + shapeOffset
            shape.shapeStyle.coordinates.y = (basicElem.y).toDouble() + shapeOffset
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

    private fun getCanevas() {
        Log.d("getCanvas", "alllooo")

        val gson = Gson()
        val galleryEditEvent: GalleryEditEvent = GalleryEditEvent(
            UserHolder.getInstance().username,
            ViewShapeHolder.getInstance().canevas.name,
            ViewShapeHolder.getInstance().canevas.password
        )
        val sendObj = gson.toJson(galleryEditEvent)

        socket?.emit(SocketConstants.GET_CANVAS, sendObj)
        socket?.emit(SocketConstants.HAS_USER_DONE_TUTORIAL, UserHolder.getInstance().username)
    }

    private var onFormsUpdated: Emitter.Listener = Emitter.Listener {
        Log.d("onFormsUpdated", it[0].toString())

        val gsonBuilder: GsonBuilder = GsonBuilder()
        gsonBuilder.registerTypeAdapter(FormsUpdateEvent::class.java, deserializer)
        val customGson: Gson = gsonBuilder.create()

        val obj: FormsUpdateEvent = customGson.fromJson(it[0].toString())
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
        Log.d("onFormsSelected", it[0].toString())

        val gsonBuilder: GsonBuilder = GsonBuilder()
        gsonBuilder.registerTypeAdapter(FormsUpdateEvent::class.java, deserializer)
        val customGson: Gson = gsonBuilder.create()

        val obj: FormsUpdateEvent = customGson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            for(form: BasicShape in obj.forms) {
                Log.d("formsSelect", obj.username + form.name)
                runOnUiThread {
                    val view: BasicElementView? = ViewShapeHolder.getInstance().map.inverse()[form.id]
                    if(view != null) {
                        view.setIsSelectedByOther(true)
                        view.invalidate()
                        view.requestLayout()
                    }


                    //syncLayoutFromCanevas()
                }
            }
        }
    }

    private var onFormsDeselected: Emitter.Listener = Emitter.Listener {
        Log.d("onFormsDeselected", it[0].toString())

        val gsonBuilder: GsonBuilder = GsonBuilder()
        gsonBuilder.registerTypeAdapter(FormsUpdateEvent::class.java, deserializer)
        val customGson: Gson = gsonBuilder.create()

        val obj: FormsUpdateEvent = customGson.fromJson(it[0].toString())
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
        Log.d("onFormsDeleted", it[0].toString())

        val gsonBuilder: GsonBuilder = GsonBuilder()
        gsonBuilder.registerTypeAdapter(FormsUpdateEvent::class.java, deserializer)
        val customGson: Gson = gsonBuilder.create()

        val obj: FormsUpdateEvent = customGson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            for(form: BasicShape in obj.forms) {
                Log.d("formsDeleted", obj.username + form.name)
                runOnUiThread {
                    val view: BasicElementView? = ViewShapeHolder.getInstance().map.inverse()[form.id]
                    if(view != null) {
                        parent_relative_layout?.removeView(view)
                    }
                    ViewShapeHolder.getInstance().remove(form)
                    ViewShapeHolder.getInstance().stackDrawingElementCreatedId.remove(form.id)

                }
            }
        }
    }

    private var onLinksUpdated: Emitter.Listener = Emitter.Listener {
        Log.d("onLinksUpdated", it[0].toString())

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
        Log.d("onLinksSelected", it[0].toString())

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
        Log.d("onLinksDeselected", it[0].toString())

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
        Log.d("onLinksDeleted", it[0].toString())

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
        Log.d("onCanvasReinitialized", "allo")
        runOnUiThread {
            ViewShapeHolder.getInstance().removeAll()
            ViewShapeHolder.getInstance().stackDrawingElementCreatedId = Stack<String>()
            parent_relative_layout?.removeAllViews()
            parent_relative_layout.addView(VFXHolder.getInstance().vfxView)
        }
    }

    private var onFormsCreated: Emitter.Listener = Emitter.Listener {
        Log.d("onFormsCreated", it[0].toString())


        val gsonBuilder: GsonBuilder = GsonBuilder()
        gsonBuilder.registerTypeAdapter(FormsUpdateEvent::class.java, deserializer)
        val customGson: Gson = gsonBuilder.create()

        val obj: FormsUpdateEvent = customGson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            for(form in obj.forms) {
                Log.d("formsCreated", obj.username + form.name)
                runOnUiThread {
                    ViewShapeHolder.getInstance().canevas.addShape(form)
                    addOnCanevas(form)
                }
            }
        }
    }

    private var onLinkCreated: Emitter.Listener = Emitter.Listener {
        Log.d("onLinkCreated", it[0].toString())

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
        Log.d("onCanevasResized", it[0].toString())

        val gson = Gson()
        val obj: CanvasResizeEvent =  gson.fromJson(it[0].toString())

        if(obj.username != UserHolder.getInstance().username) {
            runOnUiThread {
                ViewShapeHolder.getInstance().canevas.dimensions = obj.dimensions
                parent_relative_layout.layoutParams.width = (obj.dimensions.x).toInt()
                parent_relative_layout.layoutParams.height = (obj.dimensions.y).toInt()
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
        Log.d("onGetSelectedForms", it[0].toString())

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
        Log.d("onGetSelectedLinks", it[0].toString())

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

    private var onHasUserDoneTutorial: Emitter.Listener = Emitter.Listener {
        Log.d("onHasUserDoneTutorial", it[0].toString())

        val gson = Gson()
        val obj: HasUserDoneTutorialResponse =  gson.fromJson(it[0].toString())

        if(!obj.hasUserDoneTutorial){
            showTutorial()
        }
    }


    private var onGetCanevas: Emitter.Listener = Emitter.Listener {
        Log.d("onGetCanvas", "alllooo")

        val gsonBuilder: GsonBuilder = GsonBuilder()
        gsonBuilder.registerTypeAdapter(Canevas::class.java, canvasDeserializer)
        val customGson: Gson = gsonBuilder.create()

        val canvas: Canevas =  customGson.fromJson(it[0].toString())

        shapesToAdd.addAll(canvas.shapes)

        // TODO : review Adding LINKS
        linksToAdd.addAll(canvas.links)

        initializeViewFromCanevas()
    }

    private var canvasDeserializer: JsonDeserializer<Canevas> = JsonDeserializer<Canevas> { json: JsonElement, typeOfT: Type, context: JsonDeserializationContext ->
        val gson = Gson()
        val jsonObject: JsonObject = json.getAsJsonObject();

        val gsonBuilder: GsonBuilder = GsonBuilder()
        gsonBuilder.registerTypeAdapter(FormsUpdateEvent::class.java, deserializer)
        val customGson: Gson = gsonBuilder.create()


        val forms: ArrayList<BasicShape> = ArrayList()

        val jsonForms = jsonObject.get("shapes").asJsonArray
        for(form in jsonForms){
            if(form.get("type").asInt == ShapeTypes.CLASS_SHAPE.value()){
                val newForm: ClassShape = gson.fromJson(form)
                forms.add(newForm)
            } else {
                val newForm: BasicShape = gson.fromJson(form)
                forms.add(newForm)
            }
        }

        Canevas(
            jsonObject.get("id").asString,
            jsonObject.get("name").asString,
            jsonObject.get("author").asString,
            jsonObject.get("owner").asString,
            jsonObject.get("accessibility").asInt,
            jsonObject.get("password").asString,
            forms,
            gson.fromJson(jsonObject.get("links")),
            jsonObject.get("thumbnail").asString,
            gson.fromJson(jsonObject.get("dimensions"))
        )
    }

    private var deserializer: JsonDeserializer<FormsUpdateEvent> = JsonDeserializer<FormsUpdateEvent> { json: JsonElement, typeOfT: Type, context: JsonDeserializationContext ->
        val gson = Gson()
        val jsonObject: JsonObject = json.getAsJsonObject();

        val forms: ArrayList<BasicShape> = ArrayList()

        val jsonForms = jsonObject.get("forms").asJsonArray
        for(form in jsonForms){
            if(form.get("type").asInt == ShapeTypes.CLASS_SHAPE.value()){
                val newForm: ClassShape = gson.fromJson(form)
                forms.add(newForm)
            } else {
                val newForm: BasicShape = gson.fromJson(form)
                forms.add(newForm)
            }
        }

        FormsUpdateEvent(
            jsonObject.get("username").getAsString(),
            jsonObject.get("canevasName").getAsString(),
            forms
        )
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
        socket?.off(SocketConstants.GET_CANVAS_RESPONSE, onGetCanevas)

        super.onPause()
    }

    override fun onBackPressed() {
        if(drawer!!.isDrawerOpen){
            drawer?.closeDrawer()
        } else {
            // TODO : Jé's Fix
            parent_relative_layout?.dispatchSetSelected(false)
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
//            val intent = Intent(this, GalleryActivity::class.java)
//            intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TOP
//            startActivity(intent)
            setResult(Activity.RESULT_OK)
            finish()
        }
    }

    fun saveCanevas() {
        Log.d("saveCanevas", "saveCanevasCall")

        runOnUiThread {
            Handler().postDelayed({
                try {
                    Log.d("after delay", "saveCanevasCall")
                    val bitmap: Bitmap = loadBitmapFromView(findViewById(R.id.parent_relative_layout), 50, 80)
                    val resized = Bitmap.createScaledBitmap(
                        bitmap,
                        (bitmap!!.width!!.times(1 / 2.1)).toInt(),
                        (bitmap!!.height!!.times(1 / 2.1)).toInt(),
                        true
                    );
                    val thumbnailString: String = bitMapToString(resized)
                    Log.d("bitmapString", "*|*|*|*|*|*|*|*|*|*|*|*|*|*|*|*|*|*|*|*|*|*|*|*|"/*thumbnailString*/)

                    ViewShapeHolder.getInstance().canevas.thumbnail = thumbnailString

                    val canvasEvent: CanvasEvent =
                        CanvasEvent(UserHolder.getInstance().username, ViewShapeHolder.getInstance().canevas!!)
                    val gson = Gson()
                    val sendObj = gson.toJson(canvasEvent)
                    Log.d("createObj", sendObj)
                    socket?.emit(SocketConstants.SAVE_CANVAS, sendObj)
                }catch (e: Exception){
                    Log.d("Exception", "Trying to save thumbnail")
                }

            }, 2000)
        }
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
        if(newWidth >= mMinimumWidth && newWidth <= mMaximumWidth){
            parent_relative_layout.layoutParams.width = newWidth
        }
        if(newHeight >= mMinimumHeight && newHeight <= mMaximumHeight){
            parent_relative_layout.layoutParams.height = newHeight
        }
        parent_relative_layout.requestLayout()
    }

    private fun emitCanevasUpdate(){
        val dataStr: String = this.createCanevasResizeEvent()

        if(dataStr !="") {
            Log.d("emitingUpdate", dataStr)
            socket?.emit(SocketConstants.RESIZE_CANVAS, dataStr)
            //saveCanevas()
            SyncShapeHolder.getInstance().saveCanevas()
        }
    }

    private fun createCanevasUpdateEvent(): String {
        val gson = Gson()
        val canvasEvent: CanvasEvent = CanvasEvent(UserHolder.getInstance().username, ViewShapeHolder.getInstance().canevas)
        val sendObj: String = gson.toJson(canvasEvent)
        return sendObj
    }

    private fun createCanevasResizeEvent(): String {
        val gson = Gson()
        val canvasEvent: CanvasResizeEvent = CanvasResizeEvent(UserHolder.getInstance().username, ViewShapeHolder.getInstance().canevas.name, ViewShapeHolder.getInstance().canevas.dimensions)
        val sendObj: String = gson.toJson(canvasEvent)
        return sendObj
    }


    open protected var onSelectCanvevas = CompoundButton.OnCheckedChangeListener { _, isChecked ->
        val gson = Gson()
        val canvasEvent: GalleryEditEvent = GalleryEditEvent(UserHolder.getInstance().username, ViewShapeHolder.getInstance().canevas.name, "")
        val dataStr: String = gson.toJson(canvasEvent)

        if(isChecked) {
            socket?.emit(SocketConstants.SELECT_CANVAS, dataStr)
            val localSocket = socket
            if(localSocket == null || !localSocket.connected()) {
                isCanvasSelectedByYou = true
                runOnUiThread {
                    select_canevas_button.setChecked(true)
                    select_canevas_button.setEnabled(true)
                    parent_relative_layout.setBackgroundResource(R.drawable.borders_blue_bg_white)
                    resizeCanvevasButton.setBackgroundResource(R.drawable.ic_resize)
                }
            }
        } else {
            socket?.emit(SocketConstants.DESELECT_CANVAS, dataStr)

            val localSocket = socket
            if(localSocket == null || !localSocket.connected()) {
                runOnUiThread {
                    isCanvasSelectedByYou = false
                    select_canevas_button.setChecked(false)
                    select_canevas_button.setEnabled(true)
                    parent_relative_layout.setBackgroundResource(R.drawable.borders_transparent_bg_white)
                    resizeCanvevasButton.setBackgroundResource(0)
                }
            }
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