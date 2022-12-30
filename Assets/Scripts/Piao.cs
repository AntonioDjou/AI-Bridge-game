using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piao : MonoBehaviour {

    public int move = 0;
    public int laziness;

    public int move_laziness;
    public GameObject left;
    public GameObject right;

    void Start () {
    }

    public void move_to(int move_dir, int move_laziness) {
        Debug.Log("mandou começar a mover o piao " + this);
        move = move_dir;
        this.move_laziness = move_laziness;
    }
	
	void FixedUpdate () {
        /**
         * +1 --> right
         * -1 --> left
         */
        if (move != 0) {
            float deltaTime = Time.deltaTime;
            GameObject target;
            float limite = 0.001f;
            if (move > 0) {
                target = right;
            } else {
                target = left;
            }
            // verificar se já ultrapassei o x do target; caso sim, pare; caso não, mova
            float deltaX = transform.position.x - target.transform.position.x;
            if (Mathf.Abs(deltaX) < limite) {
                move = 0;
            } else {
                Vector3 movement = Vector3.MoveTowards(transform.position, target.transform.position, 5*deltaTime / move_laziness);
                Debug.Log(move_laziness);
                Debug.Log(movement);
                transform.position = movement;
            }
        }

    }
}
