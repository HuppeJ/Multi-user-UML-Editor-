import * as SocketEvents from "../../constants/SocketEvents";
import ChatroomManager from "./components/ChatroomManager";
import { IChatroomEditData } from "../canvas/interfaces/interfaces";


// TODO : ATTENTION AUX  const chatroomId: string = chatroomManager.getChatroomIdFromName(roomName);

export default class ChatSocketEvents {
    constructor(io: any, chatroomManager: ChatroomManager) {
        io.on('connection', function (socket: any) {
            console.log(socket.id + " connected on the server");

            socket.on(SocketEvents.CREATE_CHATROOM, function (dataStr: string) {
                const data: IChatroomEditData = JSON.parse(dataStr);

                const chatroomId: string = chatroomManager.getChatroomIdFromName(data.chatroomName);

                const response = {
                    isCreated: chatroomManager.addChatroom(chatroomId, socket.id),
                    chatroomName: data.chatroomName,
                };

                if (response.isCreated) {
                    console.log(socket.id + " created chatroom " + data.chatroomName);
                    // (broadcast)
                    io.sockets.emit("createChatroomResponse", chatroomManager.getChatrooms());
                } else {
                    console.log(socket.id + " failed to create chatroom " + data.chatroomName);
                }

                socket.emit(SocketEvents.CREATE_CHATROOM_RESPONSE, JSON.stringify(response));
            });

            socket.on(SocketEvents.JOIN_CHATROOM, function () {
                const defaultChatroom = "default_room";
                socket.join(defaultChatroom);

                if (chatroomManager.addChatroom(defaultChatroom, socket.id)) {
                    io.emit(SocketEvents.GET_CHATROOMS_RESPONSE, chatroomManager.getChatrooms());
                } else {
                    chatroomManager.addUserToChatroom(defaultChatroom, socket.id);
                    socket.emit(SocketEvents.JOIN_CHATROOM_RESPONSE, defaultChatroom);
                }
            });

            socket.on(SocketEvents.JOIN_SPECIFIC_CHATROOM, function (roomName: string) {
                const response = {
                    roomName: roomName,
                    isJoined: chatroomManager.addUserToChatroom(roomName, socket.id)
                };

                if (response.isJoined) {
                    const chatroomId: string = chatroomManager.getChatroomIdFromName(roomName);
                    socket.join(chatroomId);
                    console.log(socket.id + " joined chatroom " + roomName);
                } else {
                    console.log(socket.id + " failed to join chatroom " + roomName);
                }

                socket.emit(SocketEvents.JOIN_CHATROOM_RESPONSE, JSON.stringify(response));
            });

            // TODO : Est-ce que nous avons besoin de cet event?
            socket.on(SocketEvents.LEAVE_SPECIFIC_CHATROOM, function (roomName: string) {
                const response = {
                    roomName: roomName,
                    isJoined: chatroomManager.removeClientFromChatroom(roomName, socket.id)
                };

                if (response.isJoined) {
                    socket.leave(roomName);
                    console.log(socket.id + " left chatroom " + roomName);
                } else {
                    console.log(socket.id + " failed to leave chatroom " + roomName);
                }

                socket.emit(SocketEvents.LEAVE_CHATROOM_RESPONSE, JSON.stringify(response));

            });

            socket.on(SocketEvents.SEND_MESSAGE, function (messageDataStr: any) {
                const messageData = JSON.parse(messageDataStr);

                const date = new Date();
                const timestamp = date.getTime();

                messageData.createdAt = timestamp;
                const response = JSON.stringify(messageData);

                console.log(`SEND_MESSAGE, response:`, response);

                io.to('default_room').emit(SocketEvents.MESSAGE_SENT, response);
            });

            socket.on(SocketEvents.GET_CHATROOMS, function () {
                console.log("Sending the chatrooms on the server: " + chatroomManager.getChatrooms());
                socket.emit(SocketEvents.GET_CHATROOMS_RESPONSE, chatroomManager.getChatrooms());
            });

            socket.on(SocketEvents.GET_CHATROOM_CLIENTS, function (roomName: string) {
                console.log("Sending the chatroom " + roomName + " clients: " + chatroomManager.getChatroomClients(roomName));
                socket.emit(SocketEvents.GET_CHATROOM_CLIENTS_RESPONSE, chatroomManager.getChatroomClients(roomName));
            });

            socket.on('disconnect', function () {
                console.log('client disconnected...', socket.id);
            });

            socket.on('error', function (err: any) {
                console.log('received error from client:', socket.id);
                console.log(err);
            });
        });

    }
};