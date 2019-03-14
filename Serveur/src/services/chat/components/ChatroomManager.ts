import Chatroom from "./Chatroom";
import { CHAT_ROOM_ID } from "../../../constants/RoomID";

export default class ChatroomManager {
    // mapping of all available chatrooms 
    // chatrooms est une Map : [key: chatroom.name, value: Chatroom]
    private chatrooms = new Map();

    public getChatroomIdFromName(chatroomName: string): string {
        return `${CHAT_ROOM_ID}-${chatroomName}`;
    }

    public addChatroom(chatroomId: string, socketId: any) {
        if (!this.isChatroom(chatroomId)) {
            const chatroom = new Chatroom(chatroomId);
            this.chatrooms.set(chatroomId, chatroom);
            return true;
        }
        return false;
    }

    public isChatroom(chatroomName: string) {
        return this.chatrooms.has(chatroomName);
    }

    public removeChatroom(chatroomName: string) {
        if (this.isChatroom(chatroomName)) {
            this.chatrooms.delete(chatroomName);
        }
    }

    public addUserToChatroom(chatroomName: string, socketId: any) {
        if (this.isChatroom(chatroomName)){
            const chatroom = this.chatrooms.get(chatroomName);
            if (!chatroom.hasUser(socketId)) {
                chatroom.addUser(socketId);
                return true;
            }
        }
        return false;
    }

    public isClientInChatroom(chatroomName: string, socketId: any) {
        if (this.isChatroom(chatroomName)){
            let chatroom = this.chatrooms.get(chatroomName);
            return chatroom.hasUser(socketId);
        }
        return false;
    }

    public removeClientFromChatroom(chatroomName: string, socketId: any) {
        if(this.isClientInChatroom(chatroomName, socketId)) {
            let chatroom = this.chatrooms.get(chatroomName);
            chatroom.removeUser(socketId);
            return true;
        }
        return false;
    }

    public getChatrooms() {
        return JSON.stringify(Array.from(this.chatrooms.keys()));
    }

    public getChatroomClients(chatroomName: string) {
        let strKeys = JSON.stringify(this.chatrooms.get(chatroomName));
        return strKeys;
    }
}
