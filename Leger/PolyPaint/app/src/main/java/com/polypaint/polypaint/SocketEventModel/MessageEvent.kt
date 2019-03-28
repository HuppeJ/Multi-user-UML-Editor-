package com.polypaint.polypaint.SocketReceptionModel

import com.polypaint.polypaint.SocketEventModel.BaseEvent

class MessageEvent(username: String, var chatroomName: String, var createdAt: String, var message: String): BaseEvent(username)