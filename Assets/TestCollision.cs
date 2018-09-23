using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
	void OnTriggerEnter2D()
	{
		Debug.LogFormat(this, "TriggerEnter on {0}", gameObject.name);
	}
}
