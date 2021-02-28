using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceData", menuName = "ScriptableObjects/SpawnResourceData", order = 2)]
public class Resources : ScriptableObject
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
}
