using System;
using UnityEngine;

/*
 * Everything was passed downwards to make this class not care about any low level details
 *
 */

public class Proximity : MonoBehaviour{
    private Creature creature;

    [SerializeField]
    private GameObject creatureObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        creature = creatureObj.GetComponent<Creature>();
    }

    private void OnTrigger(Collider2D collision) {
        Thing col = collision.gameObject.GetComponent<Thing>();
        bool sameCreature = collision.gameObject.GetInstanceID() == creatureObj.GetInstanceID();
        if (!col || sameCreature) return;
        creature.OnProximityTrigger2D(collision);
    }

    private void OnTriggerStay2D(Collider2D collision) { OnTrigger(collision); }

    private void OnTriggerEnter2D(Collider2D collision) { OnTrigger(collision); }
}
