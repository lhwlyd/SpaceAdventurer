using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

	public int startingHealth = 100;                            // The amount of health the player starts the game with.
	public int currentHealth;                                   // The current health the player has.
	public Slider healthSlider;                                 // Reference to the UI's health bar.
	public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
	// public AudioClip deathClip;                                 // The audio clip to play when the player dies.
	public float flashSpeed = 0.5f;                               // The speed the damageImage will fade at.
    public Color flashColour = new Color(1f, 0f, 0f, 1f);

	Animator anim;                                              // Reference to the Animator component.
	AudioSource playerAudio;                                    // Reference to the AudioSource component.

	bool isDead;                                                // Whether the player is dead.
	bool damaged;                                               // True when the player gets damaged.
                                                                // Use this for initialization
    public LightController characterLightController;

	private float inhaleTime;

	public float timeToBreathe = 1.5f;


	//UI control
	public Text foodText;


	void Awake () {
        currentHealth = GameManager.instance.playerHealth;
		foodText = GameObject.Find("FoodText").GetComponent<Text>();
        damageImage = GameObject.Find("DamageImage").GetComponent<Image>();
        healthSlider = GameObject.Find("PlayerHealthSlider").GetComponent<Slider>();
        characterLightController = GameObject.Find("CharacterLight").GetComponent<LightController>();

        foodText.text = "Oxygen Left " + currentHealth + " %";
	}
	
	// Update is called once per frame
	void Update () {
		inhaleTime += Time.fixedDeltaTime;
		GameManager.instance.survivedTime += Time.fixedDeltaTime;
		if (inhaleTime >= timeToBreathe)
		{
			inhaleTime = 0;
            currentHealth -= 1;
            foodText.text = " Oxygen Left : " + currentHealth + " %";
			characterLightController.UpdateLight(currentHealth);


		}

        if( damaged ){
            Debug.Log("Damaged!");
            damageImage.color = flashColour;
        } else { // Player was just damaged, and so should be fading
            // Fade it
            Debug.Log("Fading");
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime );
            
        }



        damaged = false;
	}

    public void TakeDamage( int amount ){
        damaged = true;
        currentHealth -= amount;
        healthSlider.value = currentHealth;
        characterLightController.UpdateLight(currentHealth);

        foodText.text = "Lost " + amount + " oxygen...\nOxygen Left : " + currentHealth + " %";
    }

    public void AddHp( int amount ){
        /*if( currentHealth + amount >= 100){
            currentHealth = 100;
        } else {
			currentHealth += amount;
		}*/
        currentHealth += amount;
        if (currentHealth >= 100)
            currentHealth = 100;
        foodText.text = "Gained " + amount + " oxygen!\nOxygen Left : " + currentHealth + " %";

	}


}
