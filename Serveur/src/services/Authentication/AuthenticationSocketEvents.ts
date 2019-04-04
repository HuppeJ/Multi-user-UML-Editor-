import * as SocketEvents from "../../constants/SocketEvents";
import UserAccountManager from "../../components/UserAccountManager";

export default class AuthenticationSocketEvents {
    constructor(io: any, userAccountManager: UserAccountManager) {
        io.on('connection', function (socket: any) {
            socket.on(SocketEvents.CREATE_USER, async function (dataStr: any) {
                let isUserCreated = false;
                let data = JSON.parse(dataStr);

                if (await userAccountManager.isUsernameAvailable(data.username)) {
                    userAccountManager.addUser(data.username, data.password);
                    isUserCreated = true;
                }
                const response = JSON.stringify({
                    isUserCreated: isUserCreated
                });

                // console.log(`CREATE_USER, isUserCreated:`, isUserCreated)

                socket.emit(SocketEvents.CREATE_USER_RESPONSE, response);
            });

            socket.on(SocketEvents.LOGIN_USER, async function (dataStr: any) {
                let isLoginSuccessful = false;
                let data = JSON.parse(dataStr);

                isLoginSuccessful = await userAccountManager.authenticateUser(data.username, data.password, socket.id);

                const response = JSON.stringify({
                    isLoginSuccessful: isLoginSuccessful
                });

                if (isLoginSuccessful) {
                    console.log(socket.id + " connected with user " + data.username);
                } else {
                    console.log(socket.id + " failed to connect user " + data.username);
                }

                socket.emit(SocketEvents.LOGIN_USER_RESPONSE, response);
            });

            socket.on("logoutUser", function () {
                if (userAccountManager.disconnectUser(socket.id)) {
                    console.log(socket.id + " disconnected its user");
                } else {
                    console.log(socket.id + " failed to disconnect its user");
                }
            });
            
        })
    }   
};