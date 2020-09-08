using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    //Game Inits
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float boostThrust = 100f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending}
    State state = State.Alive; 

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>(); //Generics
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.Alive) {
            Thrust();
            Rotate();
        }
    }

    void OnCollisionEnter(Collision collision) {
        if( state != State.Alive) { return; }

        switch (collision.gameObject.tag) {
            case "Friendly":
                print("OK");
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence() {
        print("Awesome!");
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        state = State.Transcending;
        Invoke("LoadNextScene", 1f);
    }

    private void StartDeathSequence() {
        print("DEAD");
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        state = State.Dying;
        Invoke("LoadFirstScene", 1f);
    }

    private void LoadFirstScene() {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene() {
        SceneManager.LoadScene(1);
    }

    private void Thrust() {
        if(Input.GetKey(KeyCode.Space)) {
            ApplyThrust();
        }
        else {
            audioSource.Stop();
        }
    }

    private void ApplyThrust() {
        float boostTimeFrame = boostThrust*Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up*boostTimeFrame);
        if(!audioSource.isPlaying) {
            audioSource.PlayOneShot(mainEngine); //so it dosen't layer
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
