using System;
using UnityEngine;

public enum HelpBoxMessageType { None, Info, Warning, Error }

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class HelpBoxAttribute : PropertyAttribute
{

    public string text;
    public HelpBoxMessageType messageType;

    public HelpBoxAttribute(string text, HelpBoxMessageType messageType = HelpBoxMessageType.None)
    {
        this.text = text;
        this.messageType = messageType;
    }
}

