using System.Collections.Generic;
using UnityEngine;

//A static class that keeps track of all gravity attractors
public static class GravityObjects {

    //List of all gravity attractors
    public static List<GravityMass> gravList = new List<GravityMass>();

    //Method called by new gravity masses to put themselves on the list
    public static void AddGravObj(GravityMass newGrav)
    {
        gravList.Add(newGrav);
    }

    //TODO--make this happen
    public static void RemoveGravObj()
    {

    }
}