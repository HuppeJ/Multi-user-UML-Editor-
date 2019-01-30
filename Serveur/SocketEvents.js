module.exports = Object.freeze({
    // X = any events name
    // Server listens to X ex.: CREATE_USER
    // Server emits X_RESPONSE ex.: CREATE_USER_RESPONSE

    // Authentication 
    CREATE_USER: "createUser", 
    CREATE_USER_RESPONSE: "createUserResponse",
    LOGIN_USER: "loginUser",
    LOGIN_USER_RESPONSE: "loginUserResponse"
});