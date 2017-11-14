using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRangeInitializer : MonoBehaviour {

    public LayerMask LayerToAffect;
    public int range;

    private Hashtable oldUpdated;

	// Use this for initialization
	void Start () {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(range, range), LayerToAffect);

        foreach(Collider2D obj in colliders ){
            Activatable temp = obj.GetComponent<Activatable>();
            if(temp != null){
                temp.SetActive(true);
            }
        }
	}

    private void Update()
    {

		Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(range, range), LayerToAffect);

		foreach (Collider2D obj in colliders)
		{
			Activatable temp = obj.GetComponent<Activatable>();
			if (temp != null)
			{
                temp.StartCoroutine("SetActive", true);
			}

		}


    }

}
