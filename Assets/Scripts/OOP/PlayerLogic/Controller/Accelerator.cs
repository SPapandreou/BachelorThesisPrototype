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

        private Vector2 _velocity;

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
            if (_isThrusting)
            {
                _velocity += (Vector2)_player.transform.right * _player.acceleration * Time.deltaTime;
            }
            
            var speed = _velocity.magnitude;
            speed -= _player.drag * Time.deltaTime;
            speed = Mathf.Max(speed, 0);
            speed = Mathf.Min(speed, _player.maxSpeed);
            _velocity = _velocity.normalized * speed;

            if (Mathf.Approximately(speed, 0f)) return;
            _player.transform.position += (Vector3)_velocity * Time.deltaTime;
        }
    }
}