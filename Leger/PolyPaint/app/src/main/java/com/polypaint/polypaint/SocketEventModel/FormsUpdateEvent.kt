package com.polypaint.polypaint.SocketReceptionModel

import com.polypaint.polypaint.Model.BasicShape
import com.polypaint.polypaint.SocketEventModel.BaseEvent

class FormsUpdateEvent(username: String, var canevasName: String, var forms: ArrayList<BasicShape>): BaseEvent(username)