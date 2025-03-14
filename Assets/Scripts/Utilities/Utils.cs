using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class Utils
{
    private static int indexItem = 0;
    public static NormalItem.eNormalType GetRandomNormalType()
    {
        Array values = Enum.GetValues(typeof(NormalItem.eNormalType));
        NormalItem.eNormalType result = (NormalItem.eNormalType)values.GetValue(URandom.Range(0, values.Length));

        return result;
    }

    public static NormalItem.eNormalType GetRandomNormalTypeExcept()
    {
        NormalItem.eNormalType[] list = (NormalItem.eNormalType[])Enum.GetValues(typeof(NormalItem.eNormalType));

        int rnd = URandom.Range(0, list.Length);
        NormalItem.eNormalType result = list[indexItem];
        indexItem = (indexItem + 1) >= list.Length ? URandom.Range(0, list.Length) : indexItem + 1;

        return result;
    }
}
