using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class UtilTwo
{
    public static T GetOrAddComponent<T>(GameObject Go) where T : UnityEngine.Component
    {
        T Component = Go.GetComponent<T>();
        if (Component == null)
        {
            Component = Go.AddComponent<T>();
        }

        return Component;
    }

    public static GameObject FindChild(GameObject Go, string Name = null, bool Recursive = false)
    {
        Transform transform = FindChild<Transform>(Go, Name, Recursive);
        if (transform == null)
        {
            return null;
        }

        return transform.gameObject;
    }
    
    public static T FindChild<T>(GameObject Go, string Name = null, bool Recursive = false) where T : UnityEngine.Object
    {
        if (Go == null)
        {
            return null;
        }

        if (Recursive == false)
        {
            for (int i = 0; i < Go.transform.childCount; i++)
            {
                Transform transform = Go.transform.GetChild(i);
                if (string.IsNullOrEmpty(Name) || transform.name == Name)
                {
                    T Component = transform.GetComponent<T>();
                    if (Component != null)
                    {
                        return Component;
                    }
                }
            }
        }
        else
        {
            foreach (T Component in Go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(Name) || Component.name == Name)
                {
                    return Component;
                }
            }
        }

        return null;
    }  
}
