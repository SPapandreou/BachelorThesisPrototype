using System;
using OOP.Infrastructure.Pausing;
using UnityEngine;

namespace OOP.PlayerLogic.Controller
{
    public class PlayerTeleporter : IPausableTickable
    {
        public event Action PlayerTeleported;
        
        private const float Margin = 0.01f;
        
        private readonly Player _player;
        private readonly PlayingField _playingField;

        public PlayerTeleporter(Player player, PlayingField playingField)
        {
            _player = player;
            _playingField = playingField;
        }

        public void Tick()
        {
            if (_playingField.Bounds.Contains(_player.transform.position)) return;

            var newPosition = new Vector2(ConvertX(_player.transform.position.x), ConvertY(_player.transform.position.y));
            _player.transform.position = newPosition;
            PlayerTeleported?.Invoke();
        }

        private float ConvertX(float x)
        {
            if (x < _playingField.Bounds.xMin)
            {
                return _playingField.Bounds.xMax - Margin;
            }

            if (x > _playingField.Bounds.xMax)
            {
                return _playingField.Bounds.xMin + Margin;
            }

            return x;
        }

        private float ConvertY(float y)
        {
            if (y < _playingField.Bounds.yMin)
            {
                return _playingField.Bounds.yMax - Margin;
            }

            if (y > _playingField.Bounds.yMax)
            {
                return _playingField.Bounds.yMin + Margin;
            }

            return y;
        }
    }
}