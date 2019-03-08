package com.polypaint.polypaint.Enum

enum class ShapeTypes{
    CLASS_SHAPE {override fun value () = 1},
    ARTIFACT {override fun value () = 2},
    ACTIVITY {override fun value () = 3},
    ROLE {override fun value () = 4},
    COMMENT {override fun value () = 5},
    PHASE {override fun value () = 6},
    DEFAULT {override fun value () = 10};
    abstract fun value(): Int
}