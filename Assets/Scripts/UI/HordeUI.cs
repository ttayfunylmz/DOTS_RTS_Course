using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class HordeUI : MonoBehaviour
{


    [SerializeField] private RectTransform arrowRectTransform;


    private Entity spawningSoonEntity;


    private void Start()
    {
        DOTSEventsManager.Instance.OnHordeStartSpawningSoon += DOTSEventsManager_OnHordeStartSpawningSoon;
        DOTSEventsManager.Instance.OnHordeStartedSpawning += DOTSEventsManager_OnHordeStartedSpawning;

        Hide();
    }

    private void Update()
    {
        UpdateArrowVisual();
    }

    private void UpdateArrowVisual()
    {
        if (spawningSoonEntity == Entity.Null)
        {
            return;
        }

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        LocalTransform spawningSoonLocalTransform = entityManager.GetComponentData<LocalTransform>(spawningSoonEntity);

        UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width * .5f, Screen.height * .5f));
        if (Physics.Raycast(cameraRay, out UnityEngine.RaycastHit raycastHit, 999f))
        {
            Vector3 dir = ((Vector3)spawningSoonLocalTransform.Position - raycastHit.point).normalized;
            arrowRectTransform.eulerAngles = new Vector3(0, 0, -Quaternion.LookRotation(dir).eulerAngles.y + 90);
        }
    }

    private void DOTSEventsManager_OnHordeStartSpawningSoon(object sender, System.EventArgs e)
    {
        spawningSoonEntity = (Entity)sender;

        Show();
        UpdateArrowVisual();
    }

    private void DOTSEventsManager_OnHordeStartedSpawning(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

}