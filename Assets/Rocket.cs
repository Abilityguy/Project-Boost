using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    //Game Inits
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float boostThrust = 100f;
    Rigidbody rigidBody;
    AudioSource rocketThrust;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>(); //Generics
        rocketThrust = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Thrust();
        Rotate();
    }

    void OnCollisionEnter(Collision collision) {
        switch (collision.gameObject.tag) {
            case "Friendly":
                print("OK");
                break;
            case "Fuel":
                print("FUEL");
                break;
            default:
                print("DEAD");
                break;
        }
    }

    private void Thrust() {
        float boostTimeFrame = boostThrust*Time.deltaTime;

        if(Input.GetKey(KeyCode.Space)) {
            rigidBody.AddRelativeForce(Vector3.up*boostTimeFrame);
            if(!rocketThrust.isPlaying) {
                rocketThrust.Play(); //so it dosen't layer
            }
        }
        else {
            rocketThrust.Stop();
        }
    }

    private void Rotate() {

        rigidBody.freezeRotation = true; //take manual control of rotation

        float rotationFrameSpeed = rcsThrust*Time.deltaTime;
        if(Input.GetKey(KeyCode.A)) {
            //rigidBody.AddRelativeForce(Vector3.left);
            transform.Rotate(Vector3.forward*rotationFrameSpeed);
        }
        else if(Input.GetKey(KeyCode.D)) {
            //rigidBody.AddRelativeForce(Vector3.right);
            transform.Rotate(-Vector3.forward*rotationFrameSpeed);
        }

        rigidBody.freezeRotation = false; //resume physics based rotation
    }
}
