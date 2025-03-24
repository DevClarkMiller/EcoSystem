using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class Food : Thing{
    [SerializeField]
    private float hp, maxHp = 10.0f; // The amount remaining on the food

    [SerializeField]
    private bool canRot = true, isRotten = false;

    public bool IsRotten { get => isRotten; } // Only can get this property publically, not set

    [SerializeField]
    Sprite rottenSprite;

    [SerializeField]
    private float rotTime, timeToRot = 50.0f;

    public float RotTime { get => rotTime; }

    private Mutator mutator;

    public float Eat(float eatingSpeed) {
        hp -= eatingSpeed;
        return hp > 0 ?eatingSpeed : 0;
    }

    public void Start() {
        hp = maxHp;
        if (!attributes.Contains(Attribute.Food))
            attributes.Add(Attribute.Food);
        rotTime = 0.0f;

        mutator = FindFirstObjectByType<Mutator>();
    }

    void Update(){
        if (hp <= 0) {
            Destroy(gameObject); // Food is eaten
        }

        if (canRot && !isRotten) {
            rotTime += 0.01f;
            if (rotTime >= timeToRot)
                Rot(); // Food rots, 
        }
    }

    // Marks the food as rotten, meaning only a decomposer can eat it now
    void Rot() {
        isRotten = true;
        attributes.Remove(Attribute.Food);
        attributes.Add(Attribute.RottenFood);

        // Now switch the image to its rotten version if it has one
        if (rottenSprite)
            GetComponent<SpriteRenderer>().sprite = rottenSprite;

        // 1/10 chance to spawn a maggot

        if (mutator.Odds(10)) {
            GameObject decomposer = mutator.Bool() ? Resources.Load<GameObject>("Maggot") : Resources.Load<GameObject>("Fly");
            // Spawn either a fly or a maggot
            Instantiate(decomposer, transform.position, transform.rotation);
        }
    }
}
