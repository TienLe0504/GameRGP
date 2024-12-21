using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject cam;
    [SerializeField] private float parallaxEffect;
    private float xPosistion;
    private float lenght;
    void Start()
    {
        cam = GameObject.Find("Main Camera");
        xPosistion = transform.position.x;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceMoved = cam.transform.position.x*(1-parallaxEffect);
        float distanceToMove = cam.transform.position.x * parallaxEffect;
        transform.position = new Vector3(xPosistion + distanceToMove, transform.position.y);
        if (distanceMoved > xPosistion + lenght)
            xPosistion = xPosistion + lenght;
        else if(distanceMoved<xPosistion-lenght)
            xPosistion = xPosistion - lenght;
    }
}
