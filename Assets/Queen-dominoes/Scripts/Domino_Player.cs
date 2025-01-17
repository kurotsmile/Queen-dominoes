using System.Collections.Generic;
using UnityEngine;

public class Domino_Player : MonoBehaviour
{
    public Transform area_body;
    public Animator ani;

    public GameObject panel_block_where_none;
    public GameObject panel_waiting_lost_turn;

    public domino_obj check_first_6_domino()
    {
        List<domino_obj> list_d = this.get_list_domino();
        for(int i = 0; i < list_d.Count; i++)
        {
            if ((list_d[i].num_a == list_d[i].num_b)&& (list_d[i].num_a==6)) return list_d[i];
        }
        return null;
    }

    public domino_obj get_true_first(domino_obj domino)
    {
        domino_obj d_first = this.get_domino_first();
        if (domino.num_a == d_first.get_num_main() || domino.num_b == d_first.get_num_main())
            return d_first;
        else
            return null;
    }

    public domino_obj get_true_last(domino_obj domino)
    {
        domino_obj d_last = this.get_domino_last();
        if (domino.num_a == d_last.get_num_main() || domino.num_b == d_last.get_num_main())
            return d_last;
        else
            return null;
    }

    public domino_obj get_domino_first()
    {
        List<domino_obj> list_d = this.get_list_domino();
        if (list_d.Count == 0) return null;
        return list_d[0];
    }

    public domino_obj get_domino_last()
    {
        List<domino_obj> list_d = this.get_list_domino();
        return list_d[list_d.Count-1];
    }

    public void clear()
    {
        GameObject.Find("Game").GetComponent<Game_Handle>().carrot.clear_contain(this.area_body);
    }

    public List<domino_obj> get_list_domino()
    {
        List<domino_obj> l = new List<domino_obj>();
        foreach (Transform tr in this.area_body)
        {
            if (tr.GetComponent<domino_obj>() != null)
            {
                l.Add(tr.GetComponent<domino_obj>());
            }
        }
        return l;
    }

    public void activer_player()
    {
        this.panel_waiting_lost_turn.SetActive(false);
        this.ani.Play("Panel_player");
    }

    public void cancel_palyer()
    {
        this.panel_waiting_lost_turn.SetActive(true);
        this.ani.Play("panel_player_nomal");
    }

    public bool play_npc(int num_left,int num_right)
    {
        List<domino_obj> list_d = this.get_list_domino();

        bool is_true_domino;

        is_true_domino=this.play_npc_left(num_left, list_d);
        if(is_true_domino==false) is_true_domino = this.play_npc_right(num_right, list_d);

        return is_true_domino;
    }

    public bool play_npc_left(int num_left,IList<domino_obj> list_d)
    {
        for (int i = 0; i < list_d.Count; i++)
        {
            if (list_d[i].num_a == num_left || list_d[i].num_b == num_left)
            {
                list_d[i].play();
                return true;
            }
        }
        return false;
    }

    public bool check_play_npc_left(int num_left, IList<domino_obj> list_d)
    {
        for (int i = 0; i < list_d.Count; i++)
        {
            if (list_d[i].num_a == num_left || list_d[i].num_b == num_left) return true;
        }
        return false;
    }

    private bool play_npc_right(int num_right, IList<domino_obj> list_d)
    {
        for (int i = list_d.Count - 1; i > 0; i--)
        {
            if (list_d[i].num_a == num_right || list_d[i].num_b == num_right)
            {
                list_d[i].play();
                return true;
            }
        }
        return false;
    }

    public void check_none_domino(int num_left, int num_right)
    {
        bool is_true_domino = false;
        List<domino_obj> list_d = this.get_list_domino();

        for (int i = 0; i < list_d.Count; i++)
        {
            if (list_d[i].num_a == num_left || list_d[i].num_b == num_left || list_d[i].num_a == num_right || list_d[i].num_b == num_right)
            {
                return;
            }
        }

        if (is_true_domino)
            this.panel_block_where_none.SetActive(false);
        else
            this.panel_block_where_none.SetActive(true);
    }

    public bool check_domino_dead_end(int num_left, int num_right)
    {
        bool is_true_domino = false;
        List<domino_obj> list_d = this.get_list_domino();

        for (int i = 0; i < list_d.Count; i++)
        {
            if (list_d[i].num_a == num_left || list_d[i].num_b == num_left || list_d[i].num_a == num_right || list_d[i].num_b == num_right)
            {
                is_true_domino = true;
            }
        }

        return is_true_domino;
    }

    public int get_scores()
    {
        int scores = 0;
        List<domino_obj> list_d = this.get_list_domino();

        for (int i = 0; i < list_d.Count; i++)
        {
            list_d[i].obj_close.SetActive(false);
            scores += list_d[i].num_a + list_d[i].num_b;
        }
        return scores;
    }

    public void shortened(GameObject domino_more_prefab)
    {
        List<domino_obj> list_d = this.get_list_domino();
        if (list_d.Count >= 15)
        {
            for(int i = 0; i < list_d.Count; i++)
            {
                if (i > 4 && i < 12)
                {
                    Destroy(list_d[i].gameObject);
                }
            }

            GameObject obj_domino_more = Instantiate(domino_more_prefab);
            obj_domino_more.transform.SetParent(this.area_body);
            obj_domino_more.transform.localPosition = Vector3.zero;
            obj_domino_more.transform.localScale =new Vector3(0.5f, 0.5f, 1f);
            obj_domino_more.transform.SetSiblingIndex(5);
        }
    }

}
