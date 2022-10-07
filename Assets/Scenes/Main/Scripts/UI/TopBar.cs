using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace LD51
{
    [DisallowMultipleComponent]
    public class TopBar : MonoBehaviour
    {
        [SerializeField]
        TMP_Text nHumans;
        [SerializeField]
        TMP_Text nDeaths;
        [SerializeField]
        TMP_Text time;

        void Awake()
        {
            nHumans.text = "";
            nDeaths.text = "";
        }

        void Update()
        {
            nHumans.text = State.Local.PopulationV2.Count.ToString();
            nDeaths.text = State.Local.PopulationV2.Deaths.ToString();

            var ts = TimeSpan.FromSeconds(GameTime.Time);
            time.text = ts.ToString(@"mm\:ss");
        }
    }
}
