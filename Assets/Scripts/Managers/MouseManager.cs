using UnityEngine;
using System.Collections.Generic;


public class MouseManager : MonoBehaviour{
    [SerializeField]
    private List<GameObject> followers; // Everything that's going to follow the mouse

    private Vector3 pos;
    private float moveSpeed = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        if (Input.GetMouseButton(0)) {
            pos = Input.mousePosition;
            pos = Camera.main.ScreenToWorldPoint(pos);


            foreach (GameObject follower in followers) {
                if (!follower)
                    followers.Remove(follower);
                follower.transform.position = Vector2.Lerp(follower.transform.position, pos, moveSpeed);
            }
        }
    }
}
