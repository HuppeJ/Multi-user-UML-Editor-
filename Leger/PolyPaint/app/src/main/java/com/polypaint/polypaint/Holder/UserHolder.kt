package com.polypaint.polypaint.Holder

class UserHolder(){
    var username: String =""
    companion object {
        private val userHolder: UserHolder = UserHolder()

        fun getInstance(): UserHolder{
            return userHolder
        }
    }
}