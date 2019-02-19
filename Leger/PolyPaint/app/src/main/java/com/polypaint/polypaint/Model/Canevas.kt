package com.polypaint.polypaint.Model

import java.io.Serializable

class Canevas(var id: String, var name: String, var author: String, var owner: String, var accessibility: Int, var password: String?, var shapes: ArrayList<BasicShape>, var links: ArrayList<Link>): Serializable