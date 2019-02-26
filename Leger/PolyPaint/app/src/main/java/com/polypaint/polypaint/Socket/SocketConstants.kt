package com.polypaint.polypaint.Socket

class SocketConstants{
    companion object {
        public const val SERVER_URL: String = "https://localhost:4200"
        const val CREATE_USER: String ="createUser"
        const val CREATE_USER_RESPONSE: String = "createUserResponse"
        const val LOGIN_USER: String = "loginUser"
        const val LOGIN_USER_RESPONSE: String = "loginUserResponse"

        const val UPDATE_FORMS: String = " updateForms"
        const val FORMS_UPDATED: String = "formsUpdated"


        const val JOIN_CANVAS_TEST: String = "joinCanvasTest"
        const val JOIN_CANVAS_TEST_RESPONSE: String = "joinCanvasTestResponse"
        const val CANVAS_UPDATE_TEST: String = "canvasUpdateTest"
        const val CANVAS_UPDATE_TEST_RESPONSE: String = "canvasUpdateTestResponse"
    }
}