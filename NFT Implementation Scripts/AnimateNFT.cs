using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateNFT : MonoBehaviour
{
    public float horizontalSpeed = 80;
    public float verticalSpeed = .4f;
    private float vertical = 1;
    public float positiveY = 0;
    public float negativeY = 0;
    public float delta = 0.4f;
    // Start is called before the first frame update
    void Start()
    {
        negativeY = transform.position.y - delta;
        positiveY = transform.position.y + delta;
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
    }
    public void Animate()
    {
        transform.Rotate(0, horizontalSpeed * 1f * Time.deltaTime, 0);
        if (transform.position.y > positiveY)
        {
            vertical = -verticalSpeed;
        }
        if (transform.position.y < negativeY)
        {
            vertical = verticalSpeed;
        }
        transform.Translate(0, Time.deltaTime * vertical, 0);
    }

}
