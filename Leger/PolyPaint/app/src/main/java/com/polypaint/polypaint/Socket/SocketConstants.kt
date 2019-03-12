package com.polypaint.polypaint.Socket

class SocketConstants{
    companion object {
        public const val SERVER_URL: String = "https://localhost:4200"
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




        // Edition of links
        const val CREATE_LINK: String = "createLink"
        const val CREATE_LINK_RESPONSE: String = "createLinkResponse"
        const val LINK_CREATED: String = "linkCreated"

        const val UPDATE_LINKS: String = "updateLinks"
        const val UPDATE_LINKS_RESPONSE: String = "updateLinksResponse"
        const val LINKS_UPDATED: String = "linksUpdated"

        const val DELETE_LINKS: String = ""
        const val DELETE_LINKS_RESPONSE: String = ""
        const val LINKS_DELETED: String = ""

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
        const val CANVAS_ROOM_CREATED: String = "canvasRoomCreated"

        // Canvas connection
        const val JOIN_CANVAS_ROOM: String = "joinCanvasRoom"
        const val JOIN_CANVAS_ROOM_RESPONSE: String = "joinCanvasRoomResponse"
        const val LEAVE_CANVAS_ROOM: String = "leaveCanvasRoom"
        const val LEAVE_CANVAS_ROOM_RESPONSE: String = "leaveCanvasRoomResponse"

        // Canvas deletion
        const val REMOVE_CANVAS: String = "removeCanvas"
        const val REMOVE_CANVAS_ROOM_RESPONSE: String = "removeCanvasRoomResponse"
        const val CANVAS_ROOM_REMOVED: String = "canvasRoomRemoved"

        // Get existing canvas
        const val GET_ALL_CANVAS: String = "getAllCanvas"
        const val GET_ALL_CANVAS_RESPONSE: String = "getAllCanvasResponse"





        const val JOIN_CANVAS_TEST: String = "joinCanvasTest"
        const val JOIN_CANVAS_TEST_RESPONSE: String = "joinCanvasTestResponse"
        const val CANVAS_UPDATE_TEST: String = "canvasUpdateTest"
        const val CANVAS_UPDATE_TEST_RESPONSE: String = "canvasUpdateTestResponse"
    }
}