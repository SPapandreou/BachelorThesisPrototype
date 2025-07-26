using Unity.Cinemachine;
using VContainer.Unity;

namespace OOP.PlayerLogic.Controller
{
    public class CameraController : IStartable
    {
        private readonly CinemachineCamera _mainCamera;
        private readonly Player _player;
        
        public CameraController(CinemachineCamera mainCamera, Player player)
        {
            _mainCamera = mainCamera;
            _player = player;
        }
        
        public void Start()
        {
            _mainCamera.Follow = _player.transform;
        }
    }
}