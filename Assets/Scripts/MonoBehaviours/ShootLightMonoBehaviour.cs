using UnityEngine;

public class ShootLightMonoBehaviour : MonoBehaviour
{


    [SerializeField] private float timer = 0.05f;


    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

}