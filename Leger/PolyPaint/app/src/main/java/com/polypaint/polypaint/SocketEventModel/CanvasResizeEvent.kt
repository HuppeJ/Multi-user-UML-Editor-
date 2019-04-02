package com.polypaint.polypaint.SocketReceptionModel

import com.polypaint.polypaint.Model.Canevas
import com.polypaint.polypaint.Model.Coordinates
import com.polypaint.polypaint.SocketEventModel.BaseEvent

class CanvasResizeEvent(username: String, var canevasName: String, var dimensions: Coordinates): BaseEvent(username)