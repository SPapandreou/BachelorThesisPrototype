using UnityEngine;

namespace OOP.PlayerLogic
{
    public class PlayingField : MonoBehaviour
    {
        public Rect Bounds{ get; private set; }

        private void Awake()
        {
            Bounds = new Rect(transform.position - transform.localScale/2, transform.localScale);
        }
        
    }
}