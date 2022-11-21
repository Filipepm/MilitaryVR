using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagCollision : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] public RadiationManager gameManager;
    [SerializeField] public FlagInstantiation flag;
    private ParticleSystem sparkle;
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Flag")
        { // only an object tagged Player stops the sound
            other.gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            other.GetComponent<Grabbable>().enabled = false;
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
            sparkle = other.GetComponentInChildren<ParticleSystem>();
            flag.ChangeIndex();
            StartCoroutine(SparkleSystem());
            gameManager.TaskManager();
        }
    }

    public IEnumerator SparkleSystem()
    {
        sparkle.Play();
        yield return new WaitForSeconds(1f);
        sparkle.Stop();
    }
}
