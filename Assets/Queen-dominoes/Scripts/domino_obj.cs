using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class domino_obj : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    public Image img_domino;
    public GameObject obj_close;

    public int num_a;
    public int num_b;
    public int num_main=-1;
    public int index_sp=-1;

    public bool is_npc = false;
    public bool is_loading = false;
    public bool is_play = false;
    public bool is_open = false;

    public CanvasGroup canvasGrop;

    private float speed_move =5;
    private float timer_move = 0f;

    private bool is_drag = false;
    private bool is_done = false;

    private Transform tr_father;

    public void play_broadcast()
    {
        this.is_loading = true;
        this.transform.SetParent(this.transform.parent.root);
    }

    private void Update()
    {
        if (this.is_loading)
        {
            this.timer_move += 1.2f * Time.deltaTime;
            if (this.is_npc)
                this.transform.Translate(Vector3.up * (this.speed_move * Time.deltaTime));
            else
                this.transform.Translate(Vector3.down * (this.speed_move * Time.deltaTime));

            if (this.timer_move > 1f)
            {
                this.stop_effect_move();
                this.timer_move = 0f;
            }
        }

        if (this.is_play)
        {
            this.timer_move += 1.2f * Time.deltaTime;
            if (this.is_npc)
                this.transform.Translate(Vector3.down * (this.speed_move * Time.deltaTime));
            else
                this.transform.Translate(Vector3.up * (this.speed_move * Time.deltaTime));

            if (this.timer_move > 1f)
            {
                GameObject.Find("Game").GetComponent<Game_Handle>().domino_manager.add_domino_play(this,true);
                this.is_play = false;
                this.timer_move = 0f;
            }
        }
    }

    private void stop_effect_move()
    {
        this.is_loading = false;
        if (this.is_npc)
            GameObject.Find("Game").GetComponent<Game_Handle>().domino_manager.add_domino_npc(this);
        else
            GameObject.Find("Game").GetComponent<Game_Handle>().domino_manager.add_domino_player(this);
    }

    public void play()
    {
        if (this.is_loading == false)
        {
            this.transform.SetParent(this.transform.parent.root);
            this.is_play = true;
        }
    }

    public void check_rotate()
    {
        if (this.num_a != this.num_b)
        {
            this.img_domino.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(80f, 50f);
        }
        else
        {
            this.num_main = this.num_a;
        }
        this.is_done = true;

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (this.is_drag)
        {
            Vector3 pos_mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = new Vector3(pos_mouse.x, pos_mouse.y, 0f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (this.is_loading == false && this.is_play == false&&this.is_done==false&&this.is_open==true)
        {
            this.transform.SetParent(transform.parent.root);
            this.is_drag = true;
            GameObject.Find("Game").GetComponent<Game_Handle>().domino_manager.prepared_domino(this);
            this.canvasGrop.blocksRaycasts = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (this.is_open)
        {
            this.is_drag = false;
            this.canvasGrop.blocksRaycasts = true;
            if (this.is_done == false)
            {
                this.transform.SetParent(this.tr_father);
                this.transform.localPosition = Vector3.zero;
            }
            GameObject.Find("Game").GetComponent<Game_Handle>().domino_manager.clear_prepared_domino();
            if (!this.is_done) GameObject.Find("Game").GetComponent<Game_Handle>().play_sound(3);
        }
    }

    public void set_tr_father(Transform arean_father)
    {
        this.tr_father = arean_father;
    }

    public void check_rotate_and_set_main_number(domino_obj domino)
    {
        if (this.num_a == domino.get_num_main())
        {
            this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            this.num_main = this.num_b;
        }
        else
        {
            this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            this.num_main = this.num_a;
        }
    }

    public void set_number_main(int number_new)
    {
        this.num_main = number_new;
    }

    public int get_num_main()
    {
        return this.num_main;
    }

    public void change_rotate()
    {
        this.img_domino.transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    public void open_card()
    {
        this.obj_close.SetActive(false);
        this.is_open = true;
    }
}
