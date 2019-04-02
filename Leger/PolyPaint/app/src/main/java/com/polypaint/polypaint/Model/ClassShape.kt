package com.polypaint.polypaint.Model

class ClassShape (id: String, type: Int, name: String, shapeStyle: ShapeStyle, links: ArrayList<String?>, linksFrom: ArrayList<String?>, var attributes: ArrayList<String?> = ArrayList(), var methods: ArrayList<String?> = ArrayList()):BasicShape(id, type, name, shapeStyle, links, linksFrom){}