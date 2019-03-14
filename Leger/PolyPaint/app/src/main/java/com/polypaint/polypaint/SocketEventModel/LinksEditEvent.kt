package com.polypaint.polypaint.SocketReceptionModel

import com.polypaint.polypaint.SocketEventModel.BaseEvent

class LinksEditEvent(username: String, var canevasName: String, var linksId: ArrayList<String>): BaseEvent(username)