import Chatroom from "./Chatroom";
import { CHAT_ROOM_ID } from "../../../constants/RoomID";
import { IEditChatroomData, IMessageData } from "../../canvas/interfaces/interfaces";

export default class ChatroomManager {
    private chatrooms = new Map(); // [key: chatroomId, value: Chatroom]

    public getChatroomIdFromName(chatroomName: string): string {
        return `${CHAT_ROOM_ID}-${chatroomName}`;
    }

    public sendMessage(chatroomId: string, data: IMessageData) {
        const chatroom: Chatroom = this.chatrooms.get(chatroomId);
        if (!chatroom) {
            return false;
        }

        return chatroom.sendMessage(data);
    }

    public addChatroom(chatroomId: string, data: IEditChatroomData) {
        if (this.chatrooms.has(chatroomId)) {
            return false;
        }

        const chatroom = new Chatroom(data);
        this.chatrooms.set(chatroomId, chatroom);
        return true;
    }

    public removeChatroom(chatroomId: string, data: IEditChatroomData) {
        if (this.chatrooms.has(chatroomId)) {
            this.chatrooms.delete(chatroomId);
            return true;
        }

        return false;
    }

    public addUserToChatroom(chatroomId: string, data: IEditChatroomData) {
        const chatroom: Chatroom = this.chatrooms.get(chatroomId);
        if (chatroom && !chatroom.hasUser(data.username)) {
            chatroom.addUser(data.username);
            return true;
        }

        return false;
    }

    public removeUserFromChatroom(chatroomId: string, data: IEditChatroomData) {
        const chatroom: Chatroom = this.chatrooms.get(chatroomId);
        if (chatroom && chatroom.hasUser(data.username)) {
            chatroom.removeUser(data.username);
            return true;
        }

        return false;
    }

    public getChatroomsSERI(): string {
        return JSON.stringify({
            chatrooms: JSON.stringify(Array.from(this.chatrooms.keys()))
        });
    }

    public getChatroomClientsSERI(chatroomId: string) {
        const chatroom: Chatroom = this.chatrooms.get(chatroomId);
        if (chatroom) {
            return chatroom.getConnectedUsersSERI();
        }

        return JSON.stringify({
            connectedUsers: ""
        });;
    }
}
