using System.Linq;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu]
    public class PlayerControllerData : ControllerData
    {
        [SerializeField]
        private float _movementAudioDuration,
                      _sensitivity,
                      _jumpHeight,
                      _actionDistance,
                      _maxItemCameraOffset,
                      _dropForce,
                      _scopeSpeedMultiplier,
                      _period,
                      _offset;
        public float MovementAudioDuration => _movementAudioDuration;
        public float Sensitivity => _sensitivity;
        public float JumpHeight => _jumpHeight;
        public float ActionDistance => _actionDistance;
        public float MaxItemCameraOffset => _maxItemCameraOffset;
        public float DropForce => _dropForce;
        public float ScopeSpeedMultiplier => _scopeSpeedMultiplier;
        public float Period => _period;
        public float Offset => _offset;

        [System.Serializable]
        public struct State
        {
            public enum Types
            {
                Idle,
                Walk,
                Run,
                Crouch,
                Jump,
                Mounted,
                Vehicled
            }
            [SerializeField]
            private Types _type;
            public Types Type => _type;

            [SerializeField]
            private float _speed;
            public float Speed => _speed;
        }
        [SerializeField]
        private State[] _states;
        public State GetState(State.Types type)
            => _states.First(state => state.Type == type);

        [SerializeField]
        private AnimationCurve _velocityDamageAnimationCurve,
                               _recoilDecreaseAnimationCurve;
        public AnimationCurve VelocityDamageAnimationCurve => _velocityDamageAnimationCurve;
        public AnimationCurve RecoilDecreaseAnimationCurve => _recoilDecreaseAnimationCurve;
    }
}