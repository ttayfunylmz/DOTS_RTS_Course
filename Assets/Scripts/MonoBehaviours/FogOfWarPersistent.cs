using UnityEngine;

public class FogOfWarPersistent : MonoBehaviour
{
    [SerializeField] private RenderTexture fogOfWarRenderTexture;
    [SerializeField] private RenderTexture fogOfWarPersistentRenderTexture;
    [SerializeField] private RenderTexture fogOfWarPersistent2RenderTexture;
    [SerializeField] private Material fogOfWarPersistentMaterial;
    
    private void Start() 
    {
        Graphics.Blit(fogOfWarRenderTexture, fogOfWarPersistentRenderTexture);
        Graphics.Blit(fogOfWarRenderTexture, fogOfWarPersistent2RenderTexture);
    }

    private void Update() 
    {
        Graphics.Blit(fogOfWarRenderTexture, fogOfWarPersistentRenderTexture, fogOfWarPersistentMaterial, 0);
        Graphics.CopyTexture(fogOfWarPersistentRenderTexture, fogOfWarPersistent2RenderTexture);
    }
}
