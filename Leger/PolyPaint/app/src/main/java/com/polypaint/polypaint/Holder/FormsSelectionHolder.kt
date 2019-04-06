package com.polypaint.polypaint.Holder

import com.polypaint.polypaint.Model.BasicShape
import com.polypaint.polypaint.Model.Link
import com.polypaint.polypaint.Model.Message
import com.polypaint.polypaint.Model.User

class FormsSelectionHolder() {

    var formsSelectedId: ArrayList<String> = ArrayList()
    var linksSelectedId: ArrayList<String> = ArrayList()

    companion object {
        private val formsSelectionHolder: FormsSelectionHolder = FormsSelectionHolder()

        fun getInstance(): FormsSelectionHolder{
            return formsSelectionHolder
        }
    }



}