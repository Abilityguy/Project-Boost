using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    //Game Inits
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float boostThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;
    bool collisionEnabled = true;
    
    
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticle;
    [SerializeField] ParticleSystem successParticle;
    [SerializeField] ParticleSystem deathParticle;

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
        if(Debug.isDebugBuild) {
            DebugKeys();
        }
    }

    private void DebugKeys() {  
        if( Input.GetKeyDown(KeyCode.L)) {
            LoadNextScene();
        }
        else if(Input.GetKeyDown(KeyCode.C)) {
            collisionEnabled = !collisionEnabled;
        }
    }

    void OnCollisionEnter(Collision collision) {
        if( state != State.Alive || !collisionEnabled) { return; }

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
        successParticle.Play();
        state = State.Transcending;
        Invoke("LoadNextScene", 1f);
    }

    private void StartDeathSequence() {
        print("DEAD");
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticle.Play();
        state = State.Dying;
        Invoke("LoadFirstScene", 1f);
    }

    private void LoadFirstScene() {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;
        int nextSceneIndex = (currentSceneIndex + 1)%totalScenes;
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void Thrust() {
        if(Input.GetKey(KeyCode.Space)) {
            ApplyThrust();
            
        }
        else {
            audioSource.Stop();
            mainEngineParticle.Stop();
        }
    }

    private void ApplyThrust() {
        float boostTimeFrame = boostThrust*Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up*boostTimeFrame);
        if(!audioSource.isPlaying) {
            audioSource.PlayOneShot(mainEngine); //so it dosen't layer
        }
        mainEngineParticle.Play();
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
