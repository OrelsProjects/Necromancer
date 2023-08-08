using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[CreateAssetMenu(fileName = "AreaData", menuName = "Necromancer/Area/Data", order = 1)]
public class AreaData : ScriptableObject
{
    public int AreaNumber;
    public Areas Area;
    public RoundData RoundData;
    
    public string AreaNameString
    {
        get
        {
            switch (Areas.Area1)
            {
                case Areas.Area1:
                    return "Area1";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}