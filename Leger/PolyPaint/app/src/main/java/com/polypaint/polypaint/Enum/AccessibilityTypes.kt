package com.polypaint.polypaint.Enum

enum class AccessibilityTypes{
    PRIVATE{override fun value () = 0},
    PUBLIC{override fun value () = 1};
    abstract fun value(): Int
}