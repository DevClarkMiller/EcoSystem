using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum Species { Rabbit, Wolf, Cat, Frog, Maggot, Fly };

public class Creature : Thing {
    protected HashSet<Attribute> detectableThings = new HashSet<Attribute>(); // Set of tags creature is concerned about
    private Rigidbody2D rb;

    public bool isSeeking = false, isBreeding = false, isBirthing = false;

    private bool isMale;
    public bool IsMale { get => isMale; }

    public Species species;

    Mutator mutator;

    [SerializeField]
    private float speed = 5.0f, maxSpeed = 5.0f, eatingSpeed = 2.0f;

    [SerializeField]
    private float hunger, hungerCap = 10.0f, hungerSpeed = 0.01f;  // Once this is reached, the creature will be dying of hunger

    [SerializeField]
    private float hp = 25.0f, maxHp = 25.0f;

    [SerializeField]
    private float breedTime, timeToBreed = 100.0f, breedingCooldown = 0, maxBreedingCooldown = 500.0f; // The time it takes to finish breeding

    public float BreedingCooldown { get => breedingCooldown; }

    [SerializeField]
    private float directionTime = 0.0f, maxDirectionTime = 250.0f; // The max time until the creature loses interest in a direction

    [SerializeField]
    private float birthTime = 0.0f, timeToBirth = 150.0f;

    [SerializeField]
    private short maxBirth = 2;

    [SerializeField]
    private HealthBar healthbar;

    private Creature currentPartner;

    public virtual void Start() {
        detectableThings.Add(Attribute.Creature);
        detectableThings.Add(Attribute.Food); // Every creature should care about food

        attributes.Add(Attribute.Creature);

        rb = GetComponent<Rigidbody2D>();

        mutator = FindFirstObjectByType<Mutator>(); // Will be tied to the mutator game object

        // Give a random gender
        isMale = mutator.Bool();
    }

    public bool IsPartner(Creature creature) { return creature.GetInstanceID() == currentPartner.GetInstanceID(); }

    void Update() {
        // Don't move if breeding
        if (!isSeeking && !isBreeding) FreeMove(); // Move randomly until something interesting is nearby
        healthbar.UpdateHealthBar(hp, maxHp);

        hunger += hungerSpeed;

        if (hunger >= hungerCap) hp -= 0.1f; // If creature is starving
        if (hp <= 0) Die();
        if (breedingCooldown > 0) breedingCooldown = Math.Max(0, breedingCooldown - 0.1f);
        if (birthTime > 0) birthTime = Math.Max(0, birthTime - 0.1f);
        if (isBirthing && birthTime == 0) EndBirth();
    }

    // Called once breeding is finished
    void EndBreed(Creature female) {
        // Both creatures are no longer breeding
        female.isBreeding = false;
        isBreeding = false;

        // Gotta set the cooldown for both creatures
        female.breedingCooldown = female.timeToBirth * 2;
        breedingCooldown = maxBreedingCooldown;

        // The female creature is now birthing
        female.StartBirth();

        speed = maxSpeed; // Give speed back
        breedTime = 0; // Reset the counter
        female.breedTime = 0;

        // Make both creatures start moving around normally again
        isSeeking = false;
        female.isSeeking = false;
    }

    // Initiates / continues a breed between two creatures
    void Breed(Creature creature) {
        //Debug.Log(";
        if (hp < maxHp) return; // Can't breed if hurt

        if (breedTime >= timeToBreed) // Once the breeding process is finished
            EndBreed(creature);

        if (breedingCooldown > 0) return; // Means the creature is on a breeding cooldown
        //else if(breedingCooldown)
        if (isBreeding) {
            breedTime += 0.1f; // Reduce breedtime
            return;
        }

        // Reduce the speed of the creature
        creature.isBreeding = true; // Force the other creature to start breeding
        isBreeding = true;
        creature.currentPartner = this;
        currentPartner = creature;
        speed = 0.01f;

        StopMoving();
        creature.StopMoving();
    }

    protected void CancelBreed() {
        isBreeding = false;
        breedTime = 0;
        speed = maxSpeed;
        isSeeking = false;
    }

    public void StartBirth() {
        birthTime = timeToBirth; // Reset the timer
        isBirthing = true;
        speed /= 2.0f;
    }

    private void EndBirth() {
        speed = maxSpeed;
        isBirthing = false;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        // Possibility of not birthing any children
        short numToBirth = (short)mutator.Rand(maxBirth + 1);
        for(short i = 0; i < numToBirth; i++) {
            // Design the new Creature here with some possible mutations
            GameObject childObj = Instantiate(gameObject); // Creates a clone
            // Then sets the new attributes
            Creature child = childObj.GetComponent<Creature>();

            SpriteRenderer childRenderer = childObj.GetComponent<SpriteRenderer>();
            SpriteRenderer currentPartnerRenderer = currentPartner.GetComponent<SpriteRenderer>();

            child.breedingCooldown = child.maxBreedingCooldown * 3; // Since the child won't mature for a while
            childRenderer.color = mutator.Color(renderer.color, currentPartnerRenderer.color);
        }

        currentPartner = null;
    }

    // Check if the gameobject is something to care about,
    // if hp is too low, don't fight for example. Or if full
    public virtual bool ShouldMove(Attribute attribute, Thing thing) {
        switch (attribute) {
            case Attribute.Food:
                return !isBreeding;
            case Attribute.Creature:
                Creature creature = (Creature) thing;
                // Only go breed if the other creature is a female and their cooldown is 0
                bool canBreed = creature.species == species && breedingCooldown == 0 && isMale && creature.breedingCooldown == 0 && !creature.isMale && !creature.isBirthing;
                if (canBreed) 
                    return true;
                
                break;
        }

        return false;
    }

    void Die() {
        Destroy(gameObject);
    }

    public Attribute? Detects(Attribute attribute) {
        if (detectableThings.Contains(attribute))
            return attribute;

        return null;
    }

    // Returns a list of the attributes which insertect with the current creatures detactableThings
    public List<Attribute> Detects(List<Attribute> attributes) {
        return attributes.Where(attribute => detectableThings.Contains(attribute)).ToList();
        //List<Attribute> detectedAttributes = new List<Attribute>();
        //foreach(Attribute attribute in attributes) {
        //    if (detectableThings.Contains(attribute)) {
        //        detectedAttributes.Add(attribute);
        //    }
        //}

        //return detectedAttributes;
    }

    protected void Eat(Food food) {
        float eaten = food.Eat(eatingSpeed);
        if (eaten > 0)
            hunger = Math.Max(0, hunger - eaten); // Make sure hunger doesn't go negative
    }

    // Move in a random direction until something is found
    public void FreeMove() {
        if (directionTime > 0) {
            directionTime -= 1;
            return;
        }

        // Move to a random point within the boundaries
        MoveTo(mutator.Coord());
        directionTime = maxDirectionTime;
    }

    public void MoveTo(Vector3 pos) {
        var dir = (pos - transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * speed, dir.y * speed);

        // Update rotation to face the direction of movement
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // Convert radians to degrees
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void StopMoving() { 
        rb.linearVelocity = Vector2.zero;
    }

    public virtual void OnCollide(Thing thing, Attribute attribute) {
        switch (attribute) {
            case Attribute.Food:
                if (!isBreeding) Eat((Food)thing); // Cast to it's proper type
                break;
            case Attribute.Creature:
                // Run away if predator, attack if prey, mate if same species.
                Creature creature = (Creature)thing; // Cast into a creature
                // Can only mate if you're a male and the other creature is female
                bool cooledDown = breedingCooldown == 0 && creature.BreedingCooldown == 0;
                if (cooledDown && creature.species == species && (!creature.IsMale && IsMale) && (!creature.isBreeding || IsPartner(creature))) 
                    Breed(creature);
                break;
        }
    }

    public virtual void OnLeaveCollide(Thing thing, List<Attribute> attributes) {
        foreach (Attribute attribute in attributes) {
            switch (attribute) {
                case Attribute.Food:
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

    private void OnTriggerStay2D(Collider2D collision) {
        Thing thing = collision.gameObject.GetComponent<Thing>();
        if (!thing) return; // Don't care about colliding with non things

        List<Attribute> attributes = Detects(thing.attributes);
        foreach (Attribute attribute in attributes) {
            OnCollide(thing, attribute);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        Thing thing = collision.gameObject.GetComponent<Thing>();
        if (!thing) return; // Don't care about colliding with non things

        List<Attribute> attributes = Detects(thing.attributes);
        OnLeaveCollide(thing, attributes);
    }

    // Move towards the thing if the proximity picks it up
    public void OnProximityTrigger2D(Collider2D collision) {
        Thing thing = collision.gameObject.GetComponent<Thing>();

        foreach (Attribute attribute in thing.attributes) { // Check if the creature detects any of the attributes
            if (Detects(attribute) != null && ShouldMove(attribute, thing)) {
                MoveTo(collision.gameObject.transform.position);
                isSeeking = true;
                break;
            }
        }
    }
}