using System;
using OOP.Infrastructure.Pausing;
using R3;
using UnityEngine;

namespace OOP.PlayerLogic.Controller
{
    public class Accelerator : IDisposable, IPausableTickable
    {
        private readonly Player _player;
        private readonly IDisposable _eventSubscription;

        private bool _isThrusting;
        private Vector2 _thrust;

        public Accelerator(Player player, PlayerInputListener playerInputListener)
        {
            _player = player;
            _eventSubscription = playerInputListener.PlayerEvents.OfType<IPlayerInputEvent, ThrustEvent>()
                .Subscribe(evt => _isThrusting = evt.IsThrusting);
        }

        public void Dispose()
        {
            _eventSubscription.Dispose();
        }

        public void Tick()
        {
            if (_isThrusting && _thrust.magnitude < _player.maxThrust)
            {
                _thrust += (Vector2)_player.transform.right * _player.acceleration * Time.deltaTime;
            }


            _thrust *= _player.thrustDampening;

            if (Mathf.Approximately(_thrust.magnitude, 0f)) return;
            _player.transform.position += (Vector3)_thrust * Time.deltaTime;
        }
    }
}