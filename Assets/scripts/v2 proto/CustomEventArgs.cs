using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this file implements some commonplace arguments we may need to pass through events. More can be added if needed

public class IntArgs : System.EventArgs
{
    public int value;

    public IntArgs(int i)
    {
        this.value = i;
    }
}
public class FloatArgs : System.EventArgs
{
    public float value;

    public FloatArgs(float f)
    {
        this.value = f;
    }
}
public class Vector3Args : System.EventArgs
{
    public Vector3 vector;

    public Vector3Args(Vector3 v)
    {
        this.vector = v;
    }
}
