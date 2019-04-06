package com.polypaint.polypaint.Model


class ClassShape (id: String, type: Int, name: String, shapeStyle: ShapeStyle, links: ArrayList<String?> = ArrayList(), linksFrom: ArrayList<String?> = ArrayList(), var attributes: ArrayList<String?> = ArrayList(), var methods: ArrayList<String?> = ArrayList()):BasicShape(id, type, name, shapeStyle, links, linksFrom){

    fun copyClass(id: String = this.id, type: Int = this.type, name: String = this.name, shapeStyle: ShapeStyle = this.shapeStyle.copy(), linksTo: ArrayList<String?> = ArrayList<String?>(), linksFrom: ArrayList<String?> = ArrayList<String?>(), attributes: ArrayList<String?> = this.attributes.clone() as ArrayList<String?>, methods: ArrayList<String?> = this.methods.clone() as ArrayList<String?>) = ClassShape(id, type, name, shapeStyle, linksTo, linksFrom, attributes, methods)

}
