using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Domino_prepared : MonoBehaviour, IDropHandler
{
    public bool is_left = false;
    public int number_main;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData != null)
        {
            domino_obj domino_drop = eventData.pointerDrag.GetComponent<domino_obj>();
            GameObject.Find("Game").GetComponent<Game_Handle>().domino_manager.add_domino_play(domino_drop,this.is_left);
            if (domino_drop.num_a == this.number_main)
                domino_drop.num_main = domino_drop.num_b;
            else
                domino_drop.num_main = domino_drop.num_a;
        }
    }

    public void check_rotate(bool is_horizontal)
    {
        if (is_horizontal)
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(80f,50f);
        else
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(45f, 75f);
    }
}
