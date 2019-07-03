using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SirenEffect : MonoBehaviour
{
	public Light leftLight;
	public Light rightLight;
	public int speed;
	public AudioSource siren;
    public bool isSirenOn = false;

	private Vector3 leftTemp;
	private Vector3 rightTemp;

    // Update is called once per frame
    void Update()
    {
        if (isSirenOn)
        {
            leftLight.enabled = rightLight.enabled = true;
            leftTemp.y += speed * Time.deltaTime;
            rightTemp.y -= speed * Time.deltaTime;

            leftLight.transform.eulerAngles = leftTemp;
            rightLight.transform.eulerAngles = rightTemp;
        }
        else
        {
            leftLight.enabled = rightLight.enabled = false;
        }
    }
}
