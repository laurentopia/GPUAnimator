# GPU Animator [![Github license]](#)
A forked expansion of [Mesh Animation](https://github.com/codewriter-packages/Mesh-Animation). Mesh Animation is lightweight library for rendering hundreds of meshes in one draw call with GPU instancing.

[VladV](https://github.com/vanifatovvlad)'s work on Mesh Animation is a perfect lightweight implementation and pipeline for GPU/Shader animation. For my purposes I am expanding it and changing some approaches, so here it is. 

- Shader now receives time as a property block parameter instead of using _Time in the shader for more control at runtime.
- To step time, call MeshAnimator.DoUpdate(float deltaTime). Lets you batch updates and/or update less frequently. 
- Animations are called and referenced via AnimationClip and not string for increased safety and more available metadata.
- Animation crossfading and queueing. Crossfade is normalized so aligned animations like Walk->Run will blend gait properly. 
- Speed multiplier, including 0 and negative.

Issues
- Currently no support for non-looping anims. All animations will loop.
- The Unlit shader is not currently compatible with the above changes.
- There is an older Asset Store package called Mesh Animator. I would like to rename this whole thing to GPUAnimator, but I didn't want to break the diffing or make pull requests disorganized.
- You will need to write your own code to call DoUpdate on each MeshAnimator. I may include something for this like a Manager script.

#### NOTE: To use MeshAnimation library you need to install [Tri Inspector](https://github.com/codewriter-packages/Tri-Inspector) - Free and open-source library that improves unity inspector.

## How it works?
Mesh Animation bakes vertex positions for each frame of animation to texture. Custom shader then move mesh vertexes to desired positions on GPU. This allows draw the same original mesh multiple times with GPU Instancing. Unique animation parameters are overridden for each instance with Material Property Block.

## Limitations
* Supported up to 2048 vertices per mesh.
* Bakes one SkinnedMeshRenderer animation per prefab.
* Requires special shader for vertex animations.
* Animations can only be baked in editor mode.
* Possibly low animation quality on some GPUs.
* Vertex animation may be not supported on some old devices.

## How to use?

1. Create Mesh Animation Asset (in `Assets/Create/Mesh Animation` menu).
2. Assign skin, shader and animation clips fields in inspector.
3. Click `Bake` button.
4. Assign generated material to gameObject.
5. Add `MeshAnimator` component to gameObject.
6. Play animation from code.
7. Add TestGPUAnimator.cs which will call DoUpdate(float deltaTime) on MeshAnimator. Assumption is you write your own.
```c#
gameObject.GetComponent<MeshAnimator>().Play("Zombie Walking");
```
<br>

[![Mesh Animation](https://user-images.githubusercontent.com/26966368/92770369-90559200-f3a2-11ea-9f1f-37719a0637c7.png)](#)

## FAQ

##### Which Rig AnimationType are supported?
Works with Humanoid or Generic. Does not works with legacy. 
