package com.polypaint.polypaint.SocketReceptionModel

import com.polypaint.polypaint.Model.Link
import com.polypaint.polypaint.SocketEventModel.BaseEvent

class LinksUpdateEvent(username: String, var canevasName: String, var links: ArrayList<Link>): BaseEvent(username)