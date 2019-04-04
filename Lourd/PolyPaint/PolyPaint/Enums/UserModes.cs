/**
 * Inspired from: Meshack Musundi
 * Source: https://www.codeproject.com/Articles/1181555/SignalChat-WPF-SignalR-Chat-Application
 */

namespace PolyPaint.Enums
{
    public enum UserModes
    {
        Login,
        Chat,
        CreateUser,
        Gallery,
        Drawing,
        History
    }

    public enum StrokeTypes
    {
        CLASS_SHAPE,
        ARTIFACT,
        ACTIVITY,
        ROLE,
        COMMENT,
        PHASE,
        LINK
    }

    public enum LinkTypes
    {
        LINE,
        ONE_WAY_ASSOCIATION,
        TWO_WAY_ASSOCIATION,
        HERITAGE,
        AGGREGATION,
        COMPOSITION
    }

    public enum LinkThicknesses
    {
        THIN,
        NORMAL,
        THICK
    }

    public enum LineStyles
    {
        FULL,
        DASHED,
        DOTTED
    }

    public enum LinkStyles
    {
        FULL,
        DOTTED
    }
}