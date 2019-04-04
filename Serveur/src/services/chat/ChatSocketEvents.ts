import * as SocketEvents from "../../constants/SocketEvents";
import ChatroomManager from "./components/ChatroomManager";
import { IEditChatroomData, IMessageData } from "../canvas/interfaces/interfaces";

export default class ChatSocketEvents {
    constructor(io: any, chatroomManager: ChatroomManager) {
        io.on('connection', function (socket: any) {

            socket.on("createChatroom", function (dataStr: string) {
                try {
                    const data: IEditChatroomData = JSON.parse(dataStr);
                    const chatroomId: string = chatroomManager.getChatroomIdFromName(data.chatroomName);

                    const response = {
                        isCreated: chatroomManager.addChatroom(chatroomId, data),
                        chatroomName: data.chatroomName
                    };

                    if (response.isCreated) {
                        // (broadcast)
                        io.sockets.emit("chatroomCreated", chatroomManager.getChatroomsSERI());
                        console.log(socket.id + " created  chatroom " + data.chatroomName);
                    } else {
                        console.log(socket.id + " failed to create chatroom " + data.chatroomName);
                    }

                    socket.emit("createChatroomResponse", JSON.stringify(response));
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });


            socket.on("removeChatroom", function (dataStr: string) {
                try {
                    const data: IEditChatroomData = JSON.parse(dataStr);
                    const chatroomId: string = chatroomManager.getChatroomIdFromName(data.chatroomName);

                    const response = {
                        isChatroomRemoved: chatroomManager.removeChatroom(chatroomId, data)
                    };

                    if (response.isChatroomRemoved) {
                        // TODO on gère cela ici? 
                        // io.sockets.clients(someroom).forEach(function(s){
                        //     s.leave(someroom);
                        // });

                        // (broadcast)
                        io.sockets.emit("chatroomRemoved", chatroomManager.getChatroomsSERI());
                        console.log(socket.id + " removed chatroom " + data.chatroomName);
                    } else {
                        console.log(socket.id + " failed to remove chatroom " + data.chatroomName);
                    }

                    socket.emit("removeChatroomResponse", JSON.stringify(response));
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("joinChatroom", function (dataStr: string) {
                try {
                    const data: IEditChatroomData = JSON.parse(dataStr);
                    const chatroomId: string = chatroomManager.getChatroomIdFromName(data.chatroomName);

                    const response = {
                        isChatroomJoined: chatroomManager.addUserToChatroom(chatroomId, data),
                        chatroomName: data.chatroomName
                    };

                    if (response.isChatroomJoined) {
                        socket.join(chatroomId);
                        console.log(socket.id + " joined chatroom " + data.chatroomName);
                    } else {
                        console.log(socket.id + " failed to join chatroom " + data.chatroomName);
                    }

                    socket.emit("joinChatroomResponse", JSON.stringify(response));
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("leaveChatroom", function (dataStr: string) {
                try {
                    const data: IEditChatroomData = JSON.parse(dataStr);
                    const chatroomId: string = chatroomManager.getChatroomIdFromName(data.chatroomName);

                    const response = {
                        isChatroomLeaved: chatroomManager.removeUserFromChatroom(chatroomId, data)
                    };

                    if (response.isChatroomLeaved) {
                        socket.leave(chatroomId);
                        console.log(socket.id + " leaved chatroom " + data.chatroomName);
                    } else {
                        console.log(socket.id + " failed to leave chatroom " + data.chatroomName);
                    }

                    socket.emit("leaveChatroomResponse", JSON.stringify(response));
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("sendMessage", function (dataStr: string) {
                try {
                    const data: IMessageData = JSON.parse(dataStr);
                    const chatroomId: string = chatroomManager.getChatroomIdFromName(data.chatroomName);

                    // Update timestamp
                    const date = new Date();
                    data.createdAt = date.getTime().toString();

                    const response = {
                        isMessageSent: chatroomManager.sendMessage(chatroomId, data)
                    };

                    if (response.isMessageSent) {
                        console.log(socket.id + " send message " + data.message + "in chatroom " + data.chatroomName);
                        io.to(chatroomId).emit("messageSent", JSON.stringify(data));
                    } else {
                        console.log(socket.id + " failed to send message " + data.message);
                    }

                    socket.emit("sendMessageResponse", JSON.stringify(response));

                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });


            socket.on("getChatrooms", function () {
                console.log("Sending the chatrooms on the server: " + chatroomManager.getChatroomsSERI());
                socket.emit(SocketEvents.GET_CHATROOMS_RESPONSE, chatroomManager.getChatroomsSERI());
            });


            socket.on("getClientsInChatroom", function (dataStr: string) {
                try {
                    const data: IEditChatroomData = JSON.parse(dataStr);
                    const chatroomId: string = chatroomManager.getChatroomIdFromName(data.chatroomName);
                    console.log("Sending the chatroom " + data.chatroomName + " clients: " + chatroomManager.getChatroomClientsSERI(chatroomId));
                    socket.emit("getClientsInChatroomResponse", chatroomManager.getChatroomClientsSERI(chatroomId));
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });


            socket.on('error', function (err: any) {
                console.log('received error from client:', socket.id);
                console.log(err);
            });
        });

    }
};