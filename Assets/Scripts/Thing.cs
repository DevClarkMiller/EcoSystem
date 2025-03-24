using System.Collections.Generic;
using UnityEngine;

public enum Attribute{ Food, RottenFood, Fruit, Prey, Creature };
public class Thing: MonoBehaviour{
    public List<Attribute> attributes = new List<Attribute>();
}
