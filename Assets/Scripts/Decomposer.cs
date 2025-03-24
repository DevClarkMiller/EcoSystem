using System.Collections.Generic;
using UnityEngine;

// Eats rotten food, has short lifespan
public class Decomposer : Creature{

    public override void Start() {
        base.Start();
        detectableThings.Remove(Attribute.Food);
        detectableThings.Add(Attribute.RottenFood);
    }

    // Instead of eating on regular food, only eat rotten food
    public override void OnCollide(Thing thing, Attribute attribute) {
        switch (attribute) {
            case Attribute.RottenFood:
                if (!isBreeding) Eat((Food)thing); // Cast to its proper type
                break;
        }
    }

    public override void OnLeaveCollide(Thing thing, List<Attribute> attributes) {
        foreach (Attribute attribute in attributes) {
            switch (attribute) {
                case Attribute.RottenFood:
                    if (!isBreeding) isSeeking = false;
                    break;

                case Attribute.Creature:
                    Creature creature = (Creature)thing;
                    // Only cancel the breed if the current partner leaves
                    if (isBreeding && IsPartner(creature)) CancelBreed();
                    break;
            }
        }
    }

    public override bool ShouldMove(Attribute attribute, Thing thing) {
        switch (attribute) {
            case Attribute.RottenFood: return !isBreeding;
            case Attribute.Creature:
                Creature creature = (Creature)thing;
                // Only go breed if the other creature is a female and their cooldown is 0
                bool canBreed = creature.species == species && BreedingCooldown == 0 && IsMale && creature.BreedingCooldown == 0 && !creature.IsMale && !creature.isBirthing;
                if (canBreed)
                    return true;

                break;
        }

        return false;
    }
}
