using UnityEngine;
using System;
using NUnit.Framework.Constraints;

public class Mutator : MonoBehaviour {
    private System.Random rand;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // For getting random coords within the bounds
    private float minX;
    private float minY;
    private float maxX;
    private float maxY;

    private bool ready = false;
    public bool Ready { get => ready; }

    void Start(){
        // Set the initial bounds 
        Camera cam = Camera.main;
        minX = cam.transform.position.x - cam.orthographicSize * cam.aspect;
        maxX = cam.transform.position.x + cam.orthographicSize * cam.aspect;
        minY = cam.transform.position.y - cam.orthographicSize;
        maxY = cam.transform.position.y + cam.orthographicSize;

        rand = new System.Random();
        ready = true;
    }

    private float Mutate(float val, float tolerance, float maxVal) {
        float newVal = (float)rand.NextDouble() * (2 * tolerance % maxVal) + (Math.Max(0, val - tolerance));
        return newVal;
    }

    // Returns a rand number from 0 to maxVal
    public float Rand(float maxVal) {
        return UnityEngine.Random.Range(0.0f, maxVal + 1);
    }

    public int Rand(int maxVal) {
        return UnityEngine.Random.Range(0, maxVal + 1);
    }

    public bool Bool() { return UnityEngine.Random.Range(0, 2) != 0; }

    public bool Odds(int maxVal) {
        return UnityEngine.Random.Range(0, maxVal + 1) == maxVal;
    }

    public Vector2 Coord() {
        float ranX = UnityEngine.Random.Range(minX, maxX);
        float ranY = UnityEngine.Random.Range(minY, maxY);

        return new Vector2(ranX, ranY);
    }

    // Returns a color that is mutated from both parents
    public Color Color(Color color1, Color color2) {
        float r = Mutate((color1.r + color2.r) / 2, 75, 255);
        float g = Mutate((color1.g + color2.g) / 2, 75, 255);
        float b = Mutate((color1.b + color2.b) / 2, 75, 255);

        return new Color(r, g, b);
    }
}
