using System.Collections.Generic;
using UnityEngine;

public static class Wait
{
    public static readonly WaitForSeconds ForOneSecond = new WaitForSeconds(1f);
    public static readonly WaitForSeconds ForTwoSeconds = new WaitForSeconds(2f);
    public static readonly WaitForSeconds ForThreeSeconds = new WaitForSeconds(3f);
    public static readonly WaitForSeconds ForFiveSeconds = new WaitForSeconds(5f);
    public static readonly WaitForSeconds ForTenSeconds = new WaitForSeconds(10f);

    public static readonly WaitForSecondsRealtime ForOneRealSecond = new WaitForSecondsRealtime(1f);
    public static readonly WaitForSecondsRealtime ForTwoRealSeconds = new WaitForSecondsRealtime(2f);
    public static readonly WaitForSecondsRealtime ForThreeRealSeconds = new WaitForSecondsRealtime(3f);
    public static readonly WaitForSecondsRealtime ForFiveRealSeconds = new WaitForSecondsRealtime(5f);
    public static readonly WaitForSecondsRealtime ForTenRealSeconds = new WaitForSecondsRealtime(10f);


    private static readonly Dictionary<float, WaitForSeconds> scaledDict = new();
    private static readonly Dictionary<float, WaitForSecondsRealtime> realTimeDict = new();


    public static WaitForSeconds ForSeconds(float seconds)
    {
        if (scaledDict.TryGetValue(seconds, out var w))
            return w;
        w = new WaitForSeconds(seconds);
        scaledDict[seconds] = w;
        return w;
    }

    public static WaitForSecondsRealtime ForSecondsRealtime(float seconds)
    {
        if (realTimeDict.TryGetValue(seconds, out var w))
            return w;
        w = new WaitForSecondsRealtime(seconds);
        realTimeDict[seconds] = w;
        return w;
    }
}