using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlanetType", menuName = "PlanetType", order = 1)]
public class PlanetTypes : ScriptableObject
{
    public Color planetTypeColor;
    public float planetTypeDensity;
    public string planetTypeName;
}
