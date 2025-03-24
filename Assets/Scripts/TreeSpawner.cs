using UnityEngine;

public class TreeSpawner : MonoBehaviour{
    [SerializeField]
    private GameObject fruit;

    [SerializeField]
    private int fruitToBear = 5;

    [SerializeField]
    private float fruitTime = 0, timeToFruit = 100.0f;

    private CircleCollider2D circleCollider;
    private Vector2 centerPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        circleCollider = GetComponent<CircleCollider2D>();
        centerPoint = (Vector2)transform.position + circleCollider.offset;
        Fruit();
    }

    // Update is called once per frame
    void Update(){
        if (fruitTime > 0)
            fruitTime -= 0.1f;
        else
            Fruit();
    }

    // Produce fruit
    public void Fruit() {
        for (int i = 0; i < fruitToBear; i++) {
            float adjustedRadius = circleCollider.radius * circleCollider.transform.lossyScale.x; // Adjust for world space scale
            Vector2 randomPoint = centerPoint + Random.insideUnitCircle * adjustedRadius * 0.5f;
            GameObject newFruit = Instantiate(fruit, randomPoint, fruit.transform.rotation); // Create a new fruit object at the random point with the regular rotation
        }

        fruitTime = timeToFruit;
    }
}
