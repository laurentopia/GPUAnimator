using UnityEditor.VersionControl;

namespace CodeWriter.MeshAnimation
{
    using UnityEngine;

    public static class MeshAnimationAssetExtensions
    {
        private static readonly int AnimationInfoAProp = Shader.PropertyToID("_AnimInfoA");
        private static readonly int AnimationInfoBProp = Shader.PropertyToID("_AnimInfoB");
        private static readonly int AnimationWeightProp = Shader.PropertyToID("_AnimWeight");
        private static readonly int AnimationLoopProp = Shader.PropertyToID("_AnimLoop");
        private static readonly int EmissionColorProp = Shader.PropertyToID("_EmissionColor");
        private static readonly int AnimationTimeProp = Shader.PropertyToID("_AnimTime");

        public static void SetAnim(this MeshAnimationAsset asset,
            MaterialPropertyBlock block,
            string animationName,
            float speed = 1f,
            float? normalizedTime = 0f,
            bool animA = true)
        {
            MeshAnimationAsset.AnimationData data = null;

            foreach (var animationData in asset.animationData)
            {
                if (animationData.name != animationName)
                {
                    continue;
                }

                data = animationData;
                break;
            }

            if (data == null)
            {
//                Debug.Log("Data was null");
                return;
            }

            var start = data.startFrame;
            var length = data.lengthFrames;
            var s = speed / Mathf.Max(data.lengthSeconds, 0.01f);
            var time = normalizedTime.HasValue
                ? Time.timeSinceLevelLoad - Mathf.Clamp01(normalizedTime.Value) / s
                : block.GetVector(AnimationInfoAProp).z;

            block.SetFloat(AnimationLoopProp, data.looping ? 1 : 0);

            if (animA)
            {
                block.SetVector(AnimationInfoAProp, new Vector4(start, length, s, time));
            }
            else
            {
                block.SetVector(AnimationInfoBProp, new Vector4(start, length, s, time));
            }
        }

        public static float Length(this MeshAnimationAsset asset, string animationName)
        {
            MeshAnimationAsset.AnimationData data = null;

            foreach (var animationData in asset.animationData)
            {
                if (animationData.name != animationName)
                {
                    continue;
                }

                data = animationData;
                break;
            }

            if (data == null)
            {
                Debug.Log("Data was null");
                return 0;
            }

            return data.lengthSeconds;
        }

        public static void SetTime(this MeshAnimationAsset asset, MaterialPropertyBlock block, float time)
        {
            block.SetFloat(AnimationTimeProp, time);
        }

        public static void SetWeight(this MeshAnimationAsset asset, MaterialPropertyBlock block, float weight)
        {
            block.SetFloat(AnimationWeightProp, weight);
        }

        public static AnimationClip GetClipByIndex(this MeshAnimationAsset asset, int idx)
        {
            return asset.animationClips[idx];
        }

        public static int GetAnimationCount(this MeshAnimationAsset asset)
        {
            return asset.animationClips.Length;
        }
    }
}