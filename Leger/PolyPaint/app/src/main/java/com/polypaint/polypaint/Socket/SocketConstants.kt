package com.polypaint.polypaint.Socket

class SocketConstants{
    companion object {
        public const val SERVER_URL: String = "https://localhost:4200"
        const val CREATE_USER: String ="createUser"
        const val CREATE_USER_RESPONSE: String = "createUserResponse"
        const val LOGIN_USER: String = "loginUser"
        const val LOGIN_USER_RESPONSE: String = "loginUserResponse"

        const val CREATE_FORM: String = "createForm"
        const val FORM_CREATED: String = "formCreated"
        const val UPDATE_FORMS: String = "updateForms"
        const val FORMS_UPDATED: String = "formsUpdated"
        const val DELETE_FORMS: String = "deleteForms"
        const val FORMS_DELETED: String = "formsDeleted"
        const val SELECT_FORMS: String = "selectForms"
        const val FORMS_SELECTED: String = "formsSelected"
        const val REINITIALISE_CANVAS: String = "reinitialiseCanvas"
        const val CANVAS_REINITIALIZED: String = "canvasReinitialized"


        const val JOIN_CANVAS_TEST: String = "joinCanvasTest"
        const val JOIN_CANVAS_TEST_RESPONSE: String = "joinCanvasTestResponse"
        const val CANVAS_UPDATE_TEST: String = "canvasUpdateTest"
        const val CANVAS_UPDATE_TEST_RESPONSE: String = "canvasUpdateTestResponse"
    }
}