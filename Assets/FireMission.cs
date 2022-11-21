using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMission : MonoBehaviour
{
    [SerializeField] public GameObject smoke;
    [SerializeField] public RadiationManager gameManager;

    public void OnParticleCollision(GameObject other)
    {
        Debug.Log("yes");
        if (other.gameObject.tag == "Fire")
        {
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(Particle());
        }
    }

    IEnumerator Particle()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        gameManager.TaskManager();
    }
}
