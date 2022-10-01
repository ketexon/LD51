using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD51
{
    public class Human : MonoBehaviour
    {
        void Start()
        {
            GameManager.Instance.EveryTenSeconds += Populate;
        }

        void Populate()
        {
            Debug.Log("Populate");
        }
    }
}
