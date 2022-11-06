namespace CodeWriter.MeshAnimation
{
    using JetBrains.Annotations;
    using UnityEngine;
    using TriInspector;

    [DrawWithTriInspector]
    public class MeshAnimator : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private MeshRenderer meshRenderer = default;

        [Required]
        [SerializeField]
        private MeshAnimationAsset meshAnimation = default;
        
        public MeshAnimationAsset Asset
        {
            get => meshAnimation;
        }

        public AnimationClip startingClip;
        
        private MaterialPropertyBlock _propertyBlock;

        [ReadOnly]
        [SerializeField] AnimationClip clipA;
        
        [ReadOnly]
        [SerializeField] AnimationClip clipB;
        
        [ReadOnly]
        [SerializeField] float calculatedSpeed;
        
        [ReadOnly]                
        [SerializeField] float t;
        
        [ReadOnly]
        [SerializeField] float weight;
        
        public float speedMultiplier;
        
        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();

            MeshCache.GenerateSecondaryUv(this.meshRenderer.GetComponent<MeshFilter>().sharedMesh);
        }

        void Start()
        {
            RandomizeTime();
            weight = 0f;
            speedMultiplier = 1f;

            if (startingClip == null)
            {
                startingClip = Asset.animationClips[0];
            }
            
            SetAnim(startingClip, 1f, 0f, true);
            SetAnim(startingClip, 1f, 0f, false);
        }
        
        [PublicAPI]
        public void SetAnim(AnimationClip clip, float speed = 1f, float? normalizedTime = 0f, bool animA = true)
        {
            bool doSet = false;
            if (animA)
            {
                if (clipA != clip)
                {
                    clipA = clip;
                    doSet = true;
                }
            }
            else
            {
                if (clipB != clip)
                {
                    clipB = clip;
                    doSet = true;
                }
            }

            if (doSet)
            {
                string aStr = animA ? "A" : "B";
                //Debug.Log($"SSSetting anim {aStr} to {clip.name}");
                meshRenderer.GetPropertyBlock(_propertyBlock);
                meshAnimation.SetAnim(_propertyBlock, clip.name, speed, normalizedTime, animA);
                meshRenderer.SetPropertyBlock(_propertyBlock);
            }
        }

        public void SetWeight(float w)
        {
            meshRenderer.GetPropertyBlock(_propertyBlock);
            meshAnimation.SetWeight(_propertyBlock, w);
            meshRenderer.SetPropertyBlock(_propertyBlock);
        }

        public void SetTime(float t)
        {
            meshRenderer.GetPropertyBlock(_propertyBlock);
            meshAnimation.SetTime(_propertyBlock, t);
            meshRenderer.SetPropertyBlock(_propertyBlock);
        }

        public void RandomizeTime()
        {
            t += Random.Range(0f, 1f);
        }

        private void SetClip(AnimationClip clip, bool doAnimA)
        {
            SetAnim(clip, 1f, 0f, doAnimA);
        }

        [ReadOnly]
        [SerializeField] 
        AnimationClip queuedAnim;
        
        
        [ReadOnly]
        [SerializeField] float crossSpeed;
        
        private void CrossfadeAB()
        {
            crossSpeed = 1f;
        }

        private void UpdateCrossfade(float deltaTime)
        {
            if (crossSpeed > 0f)
            {
                weight += crossSpeed * deltaTime;
                if (weight > 1f)
                {
                    weight = 1f;
                    crossSpeed = 0f;
                    FlipAnims();
                }
            }
            else if (crossSpeed < 0f)
            {
                weight += crossSpeed * deltaTime;
                if (weight < 0f)
                {
                    weight = 0f;
                    crossSpeed = 0f;
                }
            }
        }

        public void PlayClipNow(AnimationClip clip)
        {
            ///TODO: stomp on everything and shove this clip into slot A
        }

        public void PlayClipWithCrossfade(AnimationClip clip)
        {
            queuedAnim = clip;
        }
        
        private void FlipAnims()
        {
            SetClip(clipB, true);
            weight = 0f;
        }

        private void FlushAnimQueue()
        {   
            if (queuedAnim != null)
            {
                if (crossSpeed == 0 && weight == 0)
                {
                    //Don't crossfade to the same clip
                    if (queuedAnim != clipA)
                    {
                        SetClip(queuedAnim, false);
                        queuedAnim = null;
                        CrossfadeAB();
                    }
                    else
                    {
                        queuedAnim = null;
                    }
                }
            }
        }
        
        public void DoUpdate(float deltaTime)
        {
            calculatedSpeed = 1f/Mathf.Lerp(clipA.length, clipB.length, weight);
            t += deltaTime * calculatedSpeed * speedMultiplier;

            FlushAnimQueue();
            UpdateCrossfade(deltaTime);
            SetWeight(weight);
            
            ///This supports looping in reverse (speed multipler < 0)
            var loopTime = 1f; 
            if (t < loopTime && t >= 0f)
            {
                SetTime(t);
            }
            else if (t > 0f)
            {
                t = 0f;
                SetTime(t);
            }
            else if (t < 0f)
            {
                t = 1f;
                SetTime(t);
            }
        }
    }
}