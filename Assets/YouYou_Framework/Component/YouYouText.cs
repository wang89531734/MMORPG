using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YouYou
{
    public class YouYouText : Text
    {
        [Header("±æµÿªØ”Ô—‘Key")]
        [SerializeField]
        private string m_Localization;

        protected override void Start()
        {
            base.Start();
            if (GameEntry.Localization != null)
            {
                text = GameEntry.Localization.GetString(m_Localization);
            }
        }
    }
}
