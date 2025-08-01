using System.Globalization;
using ECS.Managed.Performance;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace ECS.Managed.UI.HUD
{
    public class PerformanceStatsUpdater : MonoBehaviour
    {
        public float refreshRate;
        public UIDocument hud;
        public FpsCounter fpsCounter;

        private float _time;

        private Label _entitiesLabel;
        private Label _fpsLabel;
        private Label _averageFpsLabel;

        private void Awake()
        {
            _time = refreshRate;
            _entitiesLabel = hud.rootVisualElement.Q<Label>("EntitiesLabel");
            _fpsLabel = hud.rootVisualElement.Q<Label>("FpsLabel");
            _averageFpsLabel = hud.rootVisualElement.Q<Label>("AverageFpsLabel");
            DontDestroyOnLoad(gameObject);
        }
        
        private void Update()
        {
            _time += Time.deltaTime;

            if (_time < refreshRate) return;
            
            var entityCount = World.DefaultGameObjectInjectionWorld.EntityManager.UniversalQuery
                .CalculateEntityCount();
            _entitiesLabel.text = entityCount.ToString();
            _fpsLabel.text = fpsCounter.GetCurrentFps().ToString("F0", CultureInfo.CurrentCulture);
            _averageFpsLabel.text = fpsCounter.GetAverageFps().ToString("F0", CultureInfo.CurrentCulture);
            _time = 0;
        }
    }
}