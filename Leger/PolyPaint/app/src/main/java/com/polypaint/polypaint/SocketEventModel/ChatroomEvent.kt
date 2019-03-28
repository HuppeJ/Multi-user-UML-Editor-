package com.polypaint.polypaint.SocketReceptionModel

import com.polypaint.polypaint.SocketEventModel.BaseEvent

class ChatroomEvent(username: String, var chatroomName: String): BaseEvent(username)