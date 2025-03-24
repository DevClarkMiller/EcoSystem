using UnityEngine;

public class GameManager : MonoBehaviour{
    [SerializeField]
    private int numTrees = 10;

    private Mutator mutator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        do {
            mutator = FindFirstObjectByType<Mutator>();
        } while (!mutator.Ready);
            

        // Generate some tree in random locations
        for (int i = 0; i < numTrees; i++) {
            GameObject tree = Resources.Load<GameObject>("Tree");
            Vector2 coord = mutator.Coord();

            Instantiate(tree, coord, tree.transform.rotation);
        }
    }
}
