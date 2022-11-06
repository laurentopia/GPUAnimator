using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CodeWriter.MeshAnimation;
using TriInspector;
using UnityEngine;

[DrawWithTriInspector]
public class TestMeshAnimator : MonoBehaviour
{
    public MeshAnimator anim;
    public bool animate;

    public AnimationClip animOne;
    public AnimationClip animTwo;
    
    [Button]
    public void PlayRandomAnimWithCrossfade()
    {
        var ridx = Random.Range(0, anim.Asset.GetAnimationCount());
        var clip = anim.Asset.GetClipByIndex(ridx);
        anim.PlayClipWithCrossfade(clip);
        Debug.Log($"Playing {clip.name}");
    }
    [Button]
    public void PlayAnimOne()
    {
        anim.PlayClipWithCrossfade(animOne);
    }
    
    [Button]
    public void PlayAnimTwo()
    {
        anim.PlayClipWithCrossfade(animTwo);
    }
    
    void Update()
    {
        if (animate){ anim.DoUpdate(Time.smoothDeltaTime); }
    }
}
