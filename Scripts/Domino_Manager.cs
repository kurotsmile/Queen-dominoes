using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Domino_Data
{
    public int num_a;
    public int num_b;
    public int index_sp;
}

public class Domino_Manager : MonoBehaviour
{
    [Header("Obj Config")]
    public Sprite[] sp_domino;
    public Sprite[] sp_domino_diamo;
    public Sprite[] sp_domino_wood;
    public Sprite[] sp_domino_black;

    public int[] num_a;
    public int[] num_b;
    private List<Sprite[]> list_style;
    private int index_style_domino = 0;

    [Header("Tray")]
    public GameObject panel_tray_start;
    public GameObject panel_tray_play;

    [Header("Panel")]
    public GameObject panel_done;
    public GameObject panel_done_win;
    public GameObject panel_done_lose;
    public GameObject panel_done_player1;
    public GameObject panel_done_player2;

    [Header("Player info ui")]
    public Sprite sp_icon_robot;
    public Sprite sp_icon_player;
    public Sprite sp_icon_player1;
    public Sprite sp_icon_player2;
    public Text text_name_player1;
    public Text text_name_player2;
    public Image img_icon_player1;
    public Image img_icon_player2;


    [Header("Game Obj")]
    public GameObject Domino_obj_prefab;
    public GameObject Domino_prepared_prefab;
    public GameObject Domino_more_prefab;
    public Transform area_tray_short;

    public Domino_Player tray_player;
    public Domino_Player tray_npc;
    public Domino_Player tray_main;

    private List<domino_obj> list_domino;
    private float timer_next_loading = 0f;
    private bool is_broadcast = false;
    private int index_domino_broadcast=-1;

    private GameObject prepared_domino_left_obj;
    private GameObject prepared_domino_right_obj;
    private List<Domino_Data> list_data_domino;
    private bool is_two_play = false;

    private void load()
    {
        this.index_style_domino = PlayerPrefs.GetInt("index_style_domino", 0);

        this.list_style = new List<Sprite[]>();
        this.list_style.Add(this.sp_domino);
        this.list_style.Add(this.sp_domino_diamo);
        this.list_style.Add(this.sp_domino_wood);
        this.list_style.Add(this.sp_domino_black);

        this.timer_next_loading = 0f;
        this.is_broadcast = false;
        this.panel_tray_play.SetActive(false);
        this.panel_tray_start.SetActive(true);
        this.panel_done.SetActive(false);

        this.list_domino = new List<domino_obj>();

        this.tray_npc.cancel_palyer();
        this.tray_player.cancel_palyer();
        this.tray_player.panel_block_where_none.SetActive(false);

        this.list_data_domino = new List<Domino_Data>();
        for (int i = 0; i < sp_domino.Length; i++)
        {
            Domino_Data d_d = new Domino_Data();
            d_d.num_a= this.num_b[i];
            d_d.num_b = this.num_a[i];
            d_d.index_sp = i;
            this.list_data_domino.Add(d_d);
        }

        this.Shuffle(this.list_data_domino);

        for (int i = 0; i < this.list_data_domino.Count; i++)
        {
            GameObject obj_domino = Instantiate(this.Domino_obj_prefab);
            obj_domino.transform.SetParent(area_tray_short);
            obj_domino.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            obj_domino.transform.localPosition = new Vector3(0f, 0f, 0f);
            obj_domino.GetComponent<domino_obj>().num_a = this.list_data_domino[i].num_a;
            obj_domino.GetComponent<domino_obj>().num_b = this.list_data_domino[i].num_b;
            obj_domino.GetComponent<domino_obj>().index_sp = this.list_data_domino[i].index_sp;
            if (i % 2 == 0)
                obj_domino.GetComponent<domino_obj>().is_npc = true;
            else
                obj_domino.GetComponent<domino_obj>().is_npc = false;
            this.list_domino.Add(obj_domino.GetComponent<domino_obj>());
        }

        this.is_broadcast = true;
        this.index_domino_broadcast = this.list_domino.Count;
        this.select_style_domino(this.index_style_domino);
    }

    void Shuffle(List<Domino_Data> a)
    {
        for (int i = a.Count - 1; i > 0; i--)
        {
            int rnd = UnityEngine.Random.Range(0, i);
            Domino_Data temp = a[i];
            a[i] = a[rnd];
            a[rnd] = temp;
        }
    }

    private void Update()
    {
        if (this.is_broadcast)
        {
            this.timer_next_loading += 1.2f * Time.deltaTime;
            if (this.timer_next_loading > 0.2f)
            {
                this.timer_next_loading = 0f;
                this.index_domino_broadcast--;
                this.list_domino[this.index_domino_broadcast].play_broadcast();
                if (this.index_domino_broadcast <= 0)
                {
                    this.panel_tray_play.SetActive(true);
                    this.panel_tray_start.SetActive(false);
                    this.is_broadcast = false;
                    this.GetComponent<Game_Handle>().carrot.delay_function(1f,this.check_first_domino);
                }
            }
        }
    }

    public void add_domino_player(domino_obj domino)
    {
        domino.transform.SetParent(this.tray_player.area_body);
        domino.transform.localScale = new Vector3(1f, 1f, 1f);
        domino.transform.localPosition = Vector3.zero;
        domino.open_card();
        domino.set_tr_father(this.tray_player.area_body);
    }

    public void add_domino_npc(domino_obj domino)
    {
        domino.transform.SetParent(this.tray_npc.area_body);
        domino.transform.localScale = new Vector3(1f, 1f, 1f);
        domino.transform.localPosition = Vector3.zero;
        if (this.is_two_play == true)
            domino.open_card();
        else
            domino.obj_close.SetActive(true);
        domino.set_tr_father(this.tray_npc.area_body);
    }

    public void add_domino_play(domino_obj domino,bool is_add_left)
    {
        if (is_add_left)
        {
            if (this.tray_main.get_domino_first() != null)
            {
                if (domino.num_a == this.tray_main.get_domino_first().get_num_main())
                    domino.num_main = domino.num_b;
                else
                    domino.num_main = domino.num_a;

                if (domino.num_a != this.tray_main.get_domino_first().get_num_main()) domino.change_rotate();
            }

        }
        else
        {
            if (this.tray_main.get_domino_last() != null)
            {
                if (domino.num_b == this.tray_main.get_domino_last().get_num_main())
                    domino.num_main = domino.num_a;
                else
                    domino.num_main = domino.num_b;

                if (domino.num_a == this.tray_main.get_domino_last().get_num_main()) domino.change_rotate();
            }
        }

        domino.transform.SetParent(this.tray_main.area_body);
        domino.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        domino.transform.localPosition = Vector3.zero;
        domino.obj_close.SetActive(false);
        domino.check_rotate();
        domino.set_tr_father(this.tray_main.area_body);

        if (is_add_left)
            domino.transform.SetAsFirstSibling();
        else
            domino.transform.SetAsLastSibling();

        this.clear_prepared_domino();
        if (this.is_two_play == false)
        {
            this.check_next_player(domino.is_npc);
        }
        else
        {
            this.tray_player.cancel_palyer();
            this.tray_npc.cancel_palyer();
            this.check_next_two_player(domino.is_npc);
        }
        this.tray_main.shortened(this.Domino_more_prefab);
        this.GetComponent<Game_Handle>().play_sound(2);
    }

    public void prepared_domino(domino_obj domino)
    {
        domino_obj domino_true_first = this.tray_main.get_true_first(domino);
        if(domino_true_first!=null)
        {
            this.prepared_domino_left_obj = Instantiate(this.Domino_prepared_prefab);
            this.prepared_domino_left_obj.transform.SetParent(this.tray_main.area_body);
            this.prepared_domino_left_obj.transform.localScale = new Vector3(1f, 1f, 1f);
            this.prepared_domino_left_obj.transform.SetAsFirstSibling();
            this.prepared_domino_left_obj.GetComponent<Domino_prepared>().is_left = true;
            this.prepared_domino_left_obj.GetComponent<Domino_prepared>().number_main = domino_true_first.get_num_main();
        }

        domino_obj domino_true_last = this.tray_main.get_true_last(domino);
        if (domino_true_last!=null)
        {
            this.prepared_domino_right_obj = Instantiate(this.Domino_prepared_prefab);
            this.prepared_domino_right_obj.transform.SetParent(this.tray_main.area_body);
            this.prepared_domino_right_obj.transform.localScale = new Vector3(1f, 1f, 1f);
            this.prepared_domino_right_obj.transform.SetAsLastSibling();
            this.prepared_domino_right_obj.GetComponent<Domino_prepared>().is_left = false;
            this.prepared_domino_right_obj.GetComponent<Domino_prepared>().number_main = domino_true_last.get_num_main();
        }

        if (domino.num_a == domino.num_b)
        {
            if (this.prepared_domino_left_obj != null) this.prepared_domino_left_obj.GetComponent<Domino_prepared>().check_rotate(false);
            if (this.prepared_domino_right_obj != null) this.prepared_domino_right_obj.GetComponent<Domino_prepared>().check_rotate(false);
        }
        else
        {
            if (this.prepared_domino_left_obj != null) this.prepared_domino_left_obj.GetComponent<Domino_prepared>().check_rotate(true);
            if (this.prepared_domino_right_obj != null) this.prepared_domino_right_obj.GetComponent<Domino_prepared>().check_rotate(true);
        }
    }

    public void clear_prepared_domino()
    {
        Destroy(this.prepared_domino_right_obj);
        Destroy(this.prepared_domino_left_obj);
    }

    public void reset()
    {
        this.GetComponent<Game_Handle>().play_sound(1);
        this.tray_npc.clear();
        this.tray_player.clear();
        this.tray_main.clear();
        this.load();
    }

    public void check_first_domino()
    {
        domino_obj domino_first;
        domino_first=this.tray_player.check_first_6_domino();
        if (domino_first == null)
        {
            this.tray_npc.activer_player();
            domino_first = this.tray_npc.check_first_6_domino();
        }
        else
        {
            this.tray_player.activer_player();
        }
        domino_first.play();
    }

    public void check_next_player(bool is_npc)
    {
        this.tray_npc.cancel_palyer();
        this.tray_player.cancel_palyer();

        int num_left = this.tray_main.get_domino_first().get_num_main();
        int num_right = this.tray_main.get_domino_last().get_num_main();

        if (this.tray_player.check_domino_dead_end(num_left, num_right) == false && this.tray_npc.check_domino_dead_end(num_left, num_right)==false)
        {
            if (this.tray_player.get_scores() < this.tray_npc.get_scores())
                this.show_done(0);
            else
                this.show_done(1);
            return;
        }
        else
        {
            if (is_npc)
            {
                this.tray_player.activer_player();
                this.tray_player.check_none_domino(num_left, num_right);
            }
            else
            {
                this.tray_npc.activer_player();
                if (this.tray_npc.play_npc(this.tray_main.get_domino_first().get_num_main(), this.tray_main.get_domino_last().get_num_main()) == false) this.check_next_player(true);
            }

            if (this.tray_npc.get_list_domino().Count == 0 || this.tray_player.get_list_domino().Count == 0)
            {
                if (this.tray_player.get_list_domino().Count == 0) this.show_done(0);
                else this.show_done(1);

                this.tray_player.panel_block_where_none.SetActive(false);
            }
        }
    }

    public void check_next_two_player(bool is_npc)
    {

        int num_left = this.tray_main.get_domino_first().get_num_main();
        int num_right = this.tray_main.get_domino_last().get_num_main();

        if (this.tray_player.get_list_domino().Count == 0)
        {
            this.show_done(2);
        }
        else if (this.tray_npc.get_list_domino().Count == 0)
        {
            this.show_done(3);
        }
        else if (this.tray_player.check_domino_dead_end(num_left, num_right) == false && this.tray_npc.check_domino_dead_end(num_left, num_right) == false)
        {
            if (this.tray_player.get_scores() < this.tray_npc.get_scores())
                this.show_done(2);
            else
                this.show_done(3);
        }
        else
        {
            if (is_npc)
            {
                this.tray_player.activer_player();
                this.tray_player.check_none_domino(num_left, num_right);
            }
            else
            {
                this.tray_npc.activer_player();
                this.tray_npc.check_none_domino(num_left, num_right);
            }
        }
    }

    public void show_done(int index_type_win)
    {
        this.panel_done_win.SetActive(false);
        this.panel_done_lose.SetActive(false);
        this.panel_done_player1.SetActive(false);
        this.panel_done_player2.SetActive(false);

        if (index_type_win==0)
        {
            this.panel_done_win.SetActive(true);
            this.GetComponent<Game_Handle>().play_sound(4);
            this.GetComponent<Game_Handle>().add_count_win();
        }
        else if(index_type_win==1)
        {
            this.panel_done_lose.SetActive(true);
            this.GetComponent<Game_Handle>().play_sound(5);
        }else if (index_type_win == 2)
        {
            this.panel_done_player1.SetActive(true);
            this.GetComponent<Game_Handle>().play_sound(4);
        }
        else if (index_type_win == 3)
        {
            this.panel_done_player2.SetActive(true);
            this.GetComponent<Game_Handle>().play_sound(4);
        }

        this.panel_done.SetActive(true);
        this.GetComponent<Game_Handle>().carrot.play_vibrate();
    }

    public void btn_skip_player()
    {
        this.GetComponent<Game_Handle>().carrot.play_sound_click();

        if (this.is_two_play)
        {
            this.tray_player.cancel_palyer();
            this.tray_npc.cancel_palyer();
            if (this.tray_npc.panel_block_where_none.activeSelf)
            {
                this.check_next_two_player(true);
                this.tray_npc.panel_block_where_none.SetActive(false);
            }
            else
            {
                this.check_next_two_player(false);
                this.tray_player.panel_block_where_none.SetActive(false);
            }     
        }
        else
        {
            this.tray_player.panel_block_where_none.SetActive(false);
            this.tray_npc.panel_block_where_none.SetActive(false);
            this.check_next_player(false);
        }
    }

    public void change_style_domino(int index_style)
    {
        this.index_style_domino = index_style;
        PlayerPrefs.SetInt("index_style_domino", index_style);
        this.select_style_domino(index_style);
    }

    private void select_style_domino(int index_style)
    {
        if (this.list_domino==null) return;
        for (int i = 0; i < this.list_domino.Count; i++)
        {
            this.list_domino[i].img_domino.sprite = this.list_style[index_style][this.list_domino[i].index_sp];
        }
    }

    public int get_index_style()
    {
        return this.index_style_domino;
    }

    public void back_menu()
    {
        this.timer_next_loading = 0f;
        this.is_broadcast = false;
        this.GetComponent<Game_Handle>().carrot.clear_contain(this.area_tray_short);
    }

    public void set_model_play(bool is_two_play)
    {
        this.is_two_play = is_two_play;
        if (this.is_two_play)
        {
            this.img_icon_player1.sprite = this.sp_icon_player1;
            this.img_icon_player2.sprite = this.sp_icon_player2;
            this.text_name_player2.text = "Player 2";
            this.text_name_player1.text = "Player 1";
        }
        else
        {
            this.img_icon_player1.sprite = this.sp_icon_player;
            this.img_icon_player2.sprite = this.sp_icon_robot;
            this.text_name_player2.text = "Robot";
            this.text_name_player1.text = "You";
        }
    }

    public bool get_status_model_two_play()
    {
        return this.is_two_play;
    }
}
