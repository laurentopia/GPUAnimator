using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CodeWriter.MeshAnimation;
using TriInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[DrawWithTriInspector]
public class TestMeshAnimator : MonoBehaviour
{
    public MeshAnimator anim;
    public bool animate;

    public AnimationClip animOne;
    public AnimationClip animTwo;

    private void Start()
    {
        anim.speedMultiplier += Random.Range(-.25f, .25f);
    }

    [Button]
    public void PlayRandomAnimWithCrossfade()
    {
        var ridx = Random.Range(0, anim.Asset.GetAnimationCount());
        var clip = anim.Asset.GetClipByIndex(ridx);
        anim.PlayClipWithCrossfade(clip);
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
        if (Input.GetKeyDown(KeyCode.R)){ PlayRandomAnimWithCrossfade();}
        if (animate){ anim.DoUpdate(Time.smoothDeltaTime); }
    }
}
