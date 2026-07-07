using System.Collections;
using UnityEngine;

namespace Baloon
{
    public class Launcher : SingletonPersistent<Launcher>
    {

        [SerializeField]
        GameObject storyPanel;

        [SerializeField]
        GameObject menuPanel;

        [SerializeField]
        GameObject rootField;

        bool showMenu = false;


        protected override void Awake()
        {
            base.Awake();

            storyPanel.SetActive(false);
            menuPanel.SetActive(false);
        }
        

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartCoroutine(DoStart());
            
            IEnumerator DoStart()
            {
                rootField.SetActive(false);

                if (!showMenu)
                    storyPanel.SetActive(true);
                else
                    menuPanel.SetActive(true);

                var showText = !showMenu;
                showMenu = true;

                yield return new WaitForSeconds(.1f);

                if(showText)
                    rootField.SetActive(true);

                
            }

            
        }

   
    }
}