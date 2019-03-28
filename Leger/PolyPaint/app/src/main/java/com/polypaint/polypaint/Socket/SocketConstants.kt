package com.polypaint.polypaint.Socket

class SocketConstants{
    companion object {
        public const val SERVER_URL: String = "https://localhost:5020"

        const val RESET_SERVER_STATE: String = "resetServerState"

        const val CREATE_USER: String ="createUser"
        const val CREATE_USER_RESPONSE: String = "createUserResponse"
        const val LOGIN_USER: String = "loginUser"
        const val LOGIN_USER_RESPONSE: String = "loginUserResponse"

        // Edition of the forms
        const val CREATE_FORM: String = "createForm"
        const val CREATE_FORM_RESPONSE: String = "createFormResponse"
        const val FORM_CREATED: String = "formCreated"

        const val UPDATE_FORMS: String = "updateForms"
        const val UPDATE_FORMS_RESPONSE: String = "updateFormsResponse"
        const val FORMS_UPDATED: String = "formsUpdated"

        const val DELETE_FORMS: String = "deleteForms"
        const val DELETE_FORMS_RESPONSE: String = "deleteFormsResponse"
        const val FORMS_DELETED: String = "formsDeleted"

        const val SELECT_FORMS: String = "selectForms"
        const val SELECT_FORMS_RESPONSE: String = "selectFormsResponse"
        const val FORMS_SELECTED: String = "formsSelected"

        const val DESELECT_FORMS: String = "deselectForms"
        const val DESELECT_FORMS_RESPONSE: String = "deselectFormsResponse"
        const val FORMS_DESELECTED: String = "formsDeselected"

        const val GET_SELECTED_FORMS: String = "getSelectedForms"
        const val SELECTED_FORMS: String = "selectedForms"

        const val GET_SELECTED_LINKS: String = "getSelectedLinks"
        const val SELECTED_LINKS: String = "selectedLinks"


        // Edition of links
        const val CREATE_LINK: String = "createLink"
        const val CREATE_LINK_RESPONSE: String = "createLinkResponse"
        const val LINK_CREATED: String = "linkCreated"

        const val UPDATE_LINKS: String = "updateLinks"
        const val UPDATE_LINKS_RESPONSE: String = "updateLinksResponse"
        const val LINKS_UPDATED: String = "linksUpdated"

        const val DELETE_LINKS: String = "deleteLinks"
        const val DELETE_LINKS_RESPONSE: String = "deleteLinksResponse"
        const val LINKS_DELETED: String = "linksDeleted"

        const val SELECT_LINKS: String = "selectLinks"
        const val SELECT_LINKS_RESPONSE: String = "selectLinksResponse"
        const val LINKS_SELECTED: String = "linksSelected"

        const val DESELECT_LINKS: String = "deselectLinks"
        const val DESELECT_LINKS_RESPONSE: String = "deselectLinksResponse"
        const val LINKS_DESELECTED: String = "linksDeselected"

        // Edition of the canvas
        const val REINITIALIZE_CANVAS: String = "reinitializeCanvas"
        const val REINITIALIZE_CANVAS_RESPONSE: String = "reinitializeCanvasResponse"
        const val CANVAS_REINITIALIZED: String = "canvasReinitialized"

        const val RESIZE_CANVAS: String = "resizeCanvas"
        const val RESIZE_CANVAS_RESPONSE: String = "resizeCanvasResponse"
        const val CANVAS_RESIZED: String = "canvasResized"

        // Canvas creation
        const val CREATE_CANVAS: String = "createCanvas"
        const val CREATE_CANVAS_RESPONSE: String = "createCanvasResponse"
        const val CANVAS_CREATED: String = "canvasCreated"

        // Canvas connection
        const val JOIN_CANVAS_ROOM: String = "joinCanvasRoom"
        const val JOIN_CANVAS_ROOM_RESPONSE: String = "joinCanvasRoomResponse"
        const val LEAVE_CANVAS_ROOM: String = "leaveCanvasRoom"
        const val LEAVE_CANVAS_ROOM_RESPONSE: String = "leaveCanvasRoomResponse"

        // Canvas deletion
        const val REMOVE_CANVAS: String = "removeCanvas"
        const val REMOVE_CANVAS_RESPONSE: String = "removeCanvasResponse"
        const val CANVAS_REMOVED: String = "canvasRemoved"

        // Get existing canvas
        const val GET_ALL_CANVAS: String = "getAllCanvas"
        const val GET_ALL_CANVAS_RESPONSE: String = "getAllCanvasResponse"
        const val GET_PUBLIC_CANVAS: String = "getPublicCanvas"
        const val GET_PUBLIC_CANVAS_RESPONSE: String = "getPublicCanvasResponse"
        const val GET_PRIVATE_CANVAS: String = "getPrivateCanvas"
        const val GET_PRIVATE_CANVAS_RESPONSE: String = "getPrivateCanvasResponse"



        const val SAVE_CANVAS: String = "saveCanvas"
        const val SAVE_CANVAS_RESPONSE: String = "saveCanvasResponse"


        const val SELECT_CANVAS: String = "selectCanvas"
        const val SELECT_CANVAS_RESPONSE: String = "selectCanvasResponse"
        const val CANVAS_SELECTED: String = "canvasSelected"

        const val DESELECT_CANVAS: String = "deselectCanvas"
        const val DESELECT_CANVAS_RESPONSE: String = "deselectCanvasResponse"
        const val CANVAS_DESELECTED: String = "canvasDeselected"

        // Messages

        const val JOIN_CHATROOM: String = "joinChatroom"


        const val SEND_MESSAGE: String = "sendMessage"
        const val MESSAGE_SENT: String = "messageSent"


        const val JOIN_CANVAS_TEST: String = "joinCanvasTest"
        const val JOIN_CANVAS_TEST_RESPONSE: String = "joinCanvasTestResponse"
        const val CANVAS_UPDATE_TEST: String = "canvasUpdateTest"
        const val CANVAS_UPDATE_TEST_RESPONSE: String = "canvasUpdateTestResponse"
    }
}