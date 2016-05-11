using UnityEngine;
using System.Collections;

public class ClickEvent : MonoBehaviour {

	public Main mainInstance;

	void OnMouseDown()
	{
		if (mainInstance == null) {
			Debug.Log ("Main instance need to locate.");
			return;
		} else {
			mainInstance.getClickOnItemEvent(this.gameObject);
			Debug.Log("Click:" + this.gameObject);
		}
	}
}
