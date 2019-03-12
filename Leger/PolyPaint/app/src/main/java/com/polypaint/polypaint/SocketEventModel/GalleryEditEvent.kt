package com.polypaint.polypaint.SocketReceptionModel

import com.polypaint.polypaint.Model.Canevas
import com.polypaint.polypaint.SocketEventModel.BaseEvent

class GalleryEditEvent(username: String, var canevasName: String, var password: String): BaseEvent(username)