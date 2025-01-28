# GPU Animator ASE Edition

![RunSheep](https://user-images.githubusercontent.com/3117959/200225759-bc851932-d70b-4323-a906-276a0d123c32.gif)

## A fork of [GPUAnimation](https://github.com/benthroop/GPUAnimator) by [benthroop](https://github.com/benthroop) which adds ASE support (only, for now).
## GPUAnimation is itself a forked expansion of [Mesh Animation](https://github.com/codewriter-packages/Mesh-Animation) by [VladV](https://github.com/vanifatovvlad), a lightweight library for rendering hundreds of meshes in one draw call with GPU instancing. 
<br>

[Mesh Animation](https://github.com/codewriter-packages/Mesh-Animation) is a wonderful lightweight implementation and pipeline for GPU/Shader animation. For my purposes I am expanding it and changing some approaches, so here it is. 

- Shader now receives time as a property block parameter instead of using _Time in the shader for more control at runtime.
- To step time, call MeshAnimator.DoUpdate(float deltaTime). Lets you batch updates and/or update less frequently. 
- Animations are called and referenced via AnimationClip and not string for increased safety and more available metadata.
- Animation crossfading and queueing. Crossfade is normalized so aligned animations like Walk->Run will blend gait properly. 
- Speed multiplier, including 0 and negative.

Issues
- Currently no support for non-looping anims. All animations will loop.
- The Unlit shader is not currently compatible with the above changes.
- There is an older Asset Store package called Mesh Animator. I would like to rename this whole thing to GPUAnimator, but I didn't want to break the diffing or make pull requests disorganized yet.
- You will need to write your own code to call DoUpdate on each MeshAnimator. I may include something for this like a Manager script.

# INSTALLATION

1. First, you need to install [Tri Inspector](https://github.com/codewriter-packages/Tri-Inspector) - Free and open-source library also by VladV which provides a number of interesting Inspector features, similar to Odin. It's used mostly in the Bake UI.
2. Make sure after installing Tri Inspector to run the installation package in its instructions. 
3. If you still get compile errors at that point, reimport the Tri Inspector folder in the Project window.
4. With Tri Inspector installed, put https://github.com/benthroop/GPUAnimator.git into Unity Package Manager to install this package.
5. Done.

## How's it work?
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
7. Add TestMeshAnimator.cs which will call DoUpdate(float deltaTime) on MeshAnimator. Assumption is you write your own.
8. To use the VAT function into ASE, switch vertex output to Absolute then drop the VAT function and connect it to local vertex offset. A **GPU ANIMATION (VAT)** toggle will appear that'll toggle VAT on.

![image](https://github.com/user-attachments/assets/383f8598-d48b-4b91-968b-6adf1736b4a0)
![image](https://github.com/user-attachments/assets/b147966f-65a4-448d-858d-401be723653a)
<br>

[![Mesh Animation](https://user-images.githubusercontent.com/26966368/92770369-90559200-f3a2-11ea-9f1f-37719a0637c7.png)](#)

## FAQ

##### Which Rig AnimationType are supported?
Works with Humanoid or Generic. Does not works with legacy. 
