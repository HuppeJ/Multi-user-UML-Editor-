package com.polypaint.polypaint.Tutorial

import android.util.Log
import android.view.View
import kotlinx.android.synthetic.main.dialog_tutorial.*

class Section1(root: View) : Section(root){
    init {
        this.start()
    }
    override fun start() {
        super.start()
        Log.d("Section1","Init")

    }

    override fun close() {
        super.close()
    }
}