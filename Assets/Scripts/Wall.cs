using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    public int playerDamage = 1;

    private int wallHp = 3;

    private void Awake()
    {
        
    }

    public void TakeDamage(){
        wallHp -= playerDamage;
        if(wallHp <= 0){
            gameObject.SetActive(false);
        }
    }
}
