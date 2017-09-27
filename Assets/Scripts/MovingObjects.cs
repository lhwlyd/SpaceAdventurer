using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObjects : MonoBehaviour {


	//The time it takes for this object to move
	public float moveTime = 0.1f;
	public LayerMask blockingLayer;


	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;
	private float inverseMoveTime;

	// Use this for initialization
	protected virtual void Start () {
		boxCollider = GetComponent<BoxCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
		inverseMoveTime = 1f / moveTime;
	}

	protected IEnumerator SmoothMovement(Vector3 end){
		//Cheaper calculation
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		while(sqrRemainingDistance > float.Epsilon){
			Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime * Time.deltaTime);
			//Actual move action
			rb2D.MovePosition (newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}


	}


	protected bool Move( int xDir, int yDir, out RaycastHit2D hit){
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);

		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;

		if (hit.transform == null) {
			StartCoroutine (SmoothMovement (end));
			return true;
		}

		return false;

	}

	protected virtual void AttemptMove<T> (int xDir, int yDir) where T : Component{
		//hit also get updated in the move method
		RaycastHit2D hit;
		bool canMove = Move ( xDir, yDir, out hit);

		if(hit.transform == null){
			return;
		}

		T hitComponent = hit.transform.GetComponent<T>();

		//!canMove only means can't pass through, not necessarily interact.
		if(!canMove && hitComponent != null){
			OnCantMove(hitComponent);
		}
	}

	/*
	 * interaction happens
	 */
	protected abstract void OnCantMove<T> (T component)
		where T : Component;
}
