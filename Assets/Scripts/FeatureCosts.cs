using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatureCostData", menuName = "ScriptableObjects/SpawnFeatureCostData", order = 3)]
public class FeatureCosts : ScriptableObject
{
    [SerializeField] private int stone;
    [SerializeField] private int wood;
    [SerializeField] private int iron;
    [SerializeField] private int gold;
    [SerializeField] private int grain;
    [SerializeField] private int meat;
    [SerializeField] private int water;

    public int Stone { get => stone; set => stone = value; }
    public int Wood { get => wood; set => wood = value; }
    public int Iron { get => iron; set => iron = value; }
    public int Gold { get => gold; set => gold = value; }
    public int Grain { get => grain; set => grain = value; }
    public int Meat { get => meat; set => meat = value; }
    public int Water { get => water; set => water = value; }

    public bool VerifyCosts(Dictionary<EResources, int> resources)
    {
        foreach (KeyValuePair<EResources, int> resource in resources)
        {
            int resourceCost = 0; 
            switch (resource.Key)
            {
                case EResources.Stone:
                    resourceCost = stone; 
                    break;
                case EResources.Wood:
                    resourceCost = wood;
                    break;
                case EResources.Iron:
                    resourceCost = iron;
                    break;
                case EResources.Gold:
                    resourceCost = gold;
                    break;
                case EResources.Grain:
                    resourceCost = grain;
                    break;
                case EResources.Meat:
                    resourceCost = meat;
                    break;
                case EResources.Water:
                    resourceCost = water;
                    break;
            }

            if (resourceCost > resource.Value)
            {
                return false;
            } 
        }

        return true;
    }
}
