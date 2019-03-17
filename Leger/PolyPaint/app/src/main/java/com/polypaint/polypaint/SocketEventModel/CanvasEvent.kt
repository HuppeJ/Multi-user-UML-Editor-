package com.polypaint.polypaint.SocketReceptionModel

import com.polypaint.polypaint.Model.Canevas
import com.polypaint.polypaint.SocketEventModel.BaseEvent

class CanvasEvent(username: String, var canevas: Canevas): BaseEvent(username)