using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Roro.Scripts.Helpers
{
    public class TextFontUnifier : MonoBehaviour
    {
        [SerializeField][Required]
        private Font m_Font;
        
        [Button]
        public void ChangeAllTextFonts()
        {  
            var txts = GameObject.FindObjectsOfType<Text>();

            for (int i = 0; i < txts.Length; i++)
            {
                txts[i].font = m_Font;
            }
        }
    }
}
