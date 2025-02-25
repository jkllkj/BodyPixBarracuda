using UnityEngine;
using UnityEngine.UI;
using Klak.TestTools;
using BodyPix;

sealed class Visualizer : MonoBehaviour
{
    [SerializeField] ImageSource _source = null;
    [SerializeField] ResourceSet _resources = null;
    [SerializeField] Vector2Int _resolution = new Vector2Int(512, 384);
    [SerializeField] RawImage _previewUI = null;
    [SerializeField] RawImage _maskUI = null;
    [SerializeField] bool _drawSkeleton = false;
    [SerializeField] Shader _shader = null;

    BodyPixRuntime _bodypix;
    Material _material;
    RenderTexture _mask;

    void Start()
    {
        _bodypix = new BodyPixRuntime(_resources, _resolution.x, _resolution.y);

        _material = new Material(_shader);

        var reso = _source.OutputResolution;
        _mask = new RenderTexture(reso.x, reso.y, 0);
        _maskUI.texture = _mask;
    }

    void OnDestroy()
    {
        _bodypix.Dispose();
        Destroy(_material);
        Destroy(_mask);
    }

    void LateUpdate()
    {
        _bodypix.ProcessImage(_source.Texture);
        _previewUI.texture = _source.Texture;

        Graphics.Blit(_bodypix.Mask, _mask, _material, 0);
    }

    void OnRenderObject()
    {
        if (!_drawSkeleton) return;

        _material.SetBuffer("_Keypoints", _bodypix.Keypoints);
        _material.SetFloat("_Aspect", (float)_resolution.x / _resolution.y);

        _material.SetPass(1);
        Graphics.DrawProceduralNow
          (MeshTopology.Triangles, 6, BodyPixRuntime.KeypointCount);

        _material.SetPass(2);
        Graphics.DrawProceduralNow(MeshTopology.Lines, 2, 12);
    }
}
