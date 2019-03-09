package com.polypaint.polypaint.SocketReceptionModel

import com.polypaint.polypaint.SocketEventModel.BaseEvent

class FormsEditEvent(username: String, var canvasName: String, var formsId: ArrayList<String>): BaseEvent(username)