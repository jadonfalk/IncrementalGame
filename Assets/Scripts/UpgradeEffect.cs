using UnityEngine;

[System.Serializable]
public struct UpgradeEffect
{
    public float value;
    public ResourceType targetResource;
    public bool isMultiplier; // true = %, false = flat
}