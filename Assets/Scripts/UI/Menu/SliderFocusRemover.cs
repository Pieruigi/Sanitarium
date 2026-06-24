using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Baloon.UI
{
    public class SliderFocusRemover : MonoBehaviour
    {

        Slider slider;

        void Awake()
        {
            slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener((a) => { RemoveFocus(); });
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        async Task RemoveFocus()
        {
            await Task.Delay(100);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}