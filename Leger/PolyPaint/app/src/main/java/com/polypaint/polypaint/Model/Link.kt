package com.polypaint.polypaint.Model

class Link(var id: String, var name: String, var from: AnchorPoint, var to: AnchorPoint, var type: Int, var style: LinkStyle, var path: ArrayList<Coordinates>){}