using UnityEngine;

public class RocketLauncher : MonoBehaviour {
    public Sprite[] spriteses;

    void Start() {
        GetComponent<SpriteRenderer>().sprite = spriteses[Random.Range(0, spriteses.Length)];
    }
}