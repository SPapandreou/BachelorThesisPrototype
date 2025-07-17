using OOP.Infrastructure.Pausing;
using UnityEngine;

namespace OOP.PlayerLogic.Controller
{
    public class Rotator : IPausableTickable
    {
        private readonly Player _player;
        private readonly Camera _mainCamera;
        
        
        public Rotator(Player player, Camera mainCamera)
        {
            _player = player;
            _mainCamera = mainCamera;
        }
        
        public void Tick()
        {
            Vector3 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePos - _player.transform.position;
            
            var targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            float currentAngle = _player.transform.eulerAngles.z;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, _player.turnSpeed * Time.deltaTime);
            
            _player.transform.rotation = Quaternion.Euler(0, 0, newAngle);
            
        }
    }
}