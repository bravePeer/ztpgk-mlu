using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class EpisodeResultColorNotifier : MonoBehaviour
{
    [Header("References")]
    private MeshRenderer _meshRenderer;

    [Space]
    [SerializeField] private Material _winMaterial;
    [SerializeField] private Material _loseMaterial;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void OnEpisodeWon()
    {
        _meshRenderer.material = _winMaterial;
    }

    public void OnEpisodeLost()
    {
        _meshRenderer.material = _loseMaterial;
    }
}
