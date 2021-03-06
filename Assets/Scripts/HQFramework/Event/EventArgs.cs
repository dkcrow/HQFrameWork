﻿using System;

public class EventArgs : System.EventArgs
{
    public object[] data;
    public EventDispatchType eventType;

    public EventArgs(EventDispatchType eventType, params object[] data)
    {
        this.data = data;
        this.eventType = eventType;
    }
}
