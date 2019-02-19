package com.polypaint.polypaint.Model

class ClassShape (id: String, type: Int, name: String, shapeStyle: ShapeStyle, links: Array<String?>, var attributes: Array<Attribute?>, var methods: Array<Method?>):BasicShape(id, type, name, shapeStyle, links){}