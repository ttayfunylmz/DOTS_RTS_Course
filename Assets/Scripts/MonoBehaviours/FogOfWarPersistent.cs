using UnityEngine;

public class FogOfWarPersistent : MonoBehaviour
{


    [SerializeField] private RenderTexture fogOfWarRenderTexture;
    [SerializeField] private RenderTexture fogOfWarPersistentRenderTexture;
    [SerializeField] private RenderTexture fogOfWarPersistent2RenderTexture;
    [SerializeField] private Material fogOfWarPersistentMaterial;


    private bool isInit;
    private int updateCount = 0;


    private void Update()
    {
        if (!isInit)
        {
            return;
        }

        Graphics.Blit(fogOfWarRenderTexture, fogOfWarPersistentRenderTexture, fogOfWarPersistentMaterial, 0);
        Graphics.CopyTexture(fogOfWarPersistentRenderTexture, fogOfWarPersistent2RenderTexture);
    }

    private void LateUpdate()
    {
        // Wait for a few frames to initialize persistent texture
        // For some reason first frame runs before fogOfWarRenderTexture is rendered
        if (isInit)
        {
            return;
        }

        updateCount++;
        if (updateCount < 5)
        {
            return;
        }

        isInit = true;
        Graphics.Blit(fogOfWarRenderTexture, fogOfWarPersistentRenderTexture);
        Graphics.Blit(fogOfWarRenderTexture, fogOfWarPersistent2RenderTexture);
    }

}