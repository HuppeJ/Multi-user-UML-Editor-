package com.polypaint.polypaint.Tutorial

import android.view.View
import kotlinx.android.synthetic.main.dialog_tutorial.*

open class Section(root: View){

    var sectionPages : ArrayList<SubSection> = ArrayList()
    var next : Section? = null
    open fun start(){

    }
    open fun close(){}

    open fun goToNext(){
        this.close()
        this.next?.start()
    }
    open fun hasNext() : Boolean{
        return next != null
    }
    open fun goToSection(section:Section){
        this.close()
        section.start()
    }
}