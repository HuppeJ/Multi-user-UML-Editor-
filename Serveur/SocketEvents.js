module.exports = Object.freeze({
    // X = any events name
    // Server listens to X ex.: CREATE_USER
    // Server emits X_RESPONSE ex.: CREATE_USER_RESPONSE

    // Authentication 
    CREATE_USER: "createUser", 
    CREATE_USER_RESPONSE: "createUserResponse",
    LOGIN_USER: "loginUser",
    LOGIN_USER_RESPONSE: "loginUserResponse",

    // Chat
    REGISTER_TO_CHAT: "registerToChat",
    REGISTER_TO_CHAT_RESPONSE: "registerToChatResponse",

    CREATE_CHATROOM: "createChatroom",
    CREATE_CHATROOM_RESPONSE: "createChatroom",
    JOIN_CHATROOM: "joinChatroom",
    JOIN_SPECIFIC_CHATROOM: "joinSpecificRoom",
    JOIN_CHATROOM_RESPONSE: "joinChatroomResponse",
    LEAVE_CHATROOM: "leaveChatroom",
    LEAVE_SPECIFIC_CHATROOM: "leaveSpecificChatroom",
    LEAVE_CHATROOM_RESPONSE: "leaveChatroomResponse",
    GET_CHATROOMS: "getChatrooms",
    GET_CHATROOMS_RESPONSE: "getChatroomsResponse",

    SEND_MESSAGE: "sendMessage",
    MESSAGE_SENT: "messageSent",


});