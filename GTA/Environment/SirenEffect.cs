using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SirenEffect : MonoBehaviour
{
	public Light leftLight, leftLight1;
	public Light rightLight, rightLight1;
	public int speed;
	public AudioSource siren;

	private Vector3 leftTemp;
	private Vector3 rightTemp;

    // Update is called once per frame
    void Update()
    {
		leftTemp.y += speed * Time.deltaTime;
		rightTemp.y -= speed * Time.deltaTime;

		leftLight.transform.eulerAngles = leftLight1.transform.eulerAngles = leftTemp;
		rightLight.transform.eulerAngles = rightLight1.transform.eulerAngles = rightTemp;
    }
}
