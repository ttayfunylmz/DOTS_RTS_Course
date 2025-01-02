using UnityEngine;

public class RagdollDeadTimer : MonoBehaviour
{


    [SerializeField] private float timer = 6f;
    [SerializeField] private float sinkTimer = 1f;
    private bool hasColliders = true;


    private void Update()
    {
        timer -= Time.deltaTime;

        if (hasColliders && timer <= 3f)
        {
            foreach (CharacterJoint characterJoint in GetComponentsInChildren<CharacterJoint>())
            {
                Destroy(characterJoint);
            }
            foreach (Rigidbody rigidbody in GetComponentsInChildren<Rigidbody>())
            {
                Destroy(rigidbody);
            }
            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                Destroy(collider);
            }
            hasColliders = false;
        }

        if (timer <= sinkTimer)
        {
            transform.position += Vector3.down * Time.deltaTime;
        }

        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

}