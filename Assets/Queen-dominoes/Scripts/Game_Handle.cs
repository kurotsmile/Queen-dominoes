using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Handle : MonoBehaviour
{
    [Header("Obj Game")]
    public Carrot.Carrot carrot;
    public Domino_Manager domino_manager;
    public IronSourceAds ads;

    [Header("Panel Game")]
    public GameObject panel_home;
    public GameObject panel_play;
    public Text txt_count_win_main;

    [Header("Other")]
    public Sprite sp_icon_shop;
    public Sprite sp_icon_shop_buy;
    public Sprite sp_icon_shop_ads;
    public AudioSource[] sound;

    [Header("Domino Shop")]
    public Sprite sp_icon_shop_domino_nomal;
    public Sprite sp_icon_shop_domino_diamo;
    public Sprite sp_icon_shop_domino_wood;
    public Sprite sp_icon_shop_domino_black;
    public bool[] list_buy_domino;

    private Carrot.Carrot_Box box_shop = null;
    private Carrot.Carrot_Window_Msg box_msg_shop = null;
    private int sel_index_shop_item_temp = -1;
    private int cont_win = 0;

    void Start()
    {
        this.carrot.Load_Carrot(this.check_exit_app);
        this.ads.On_Load();

        this.carrot.shop.onCarrotPaySuccess += this.on_buy_success;
        this.ads.onRewardedSuccess=this.on_ads_Rewarded_success;
        this.carrot.game.act_click_watch_ads_in_music_bk=this.ads.ShowRewardedVideo;
        this.carrot.act_buy_ads_success=this.ads.RemoveAds;

        this.panel_home.SetActive(true);
        this.panel_play.SetActive(false);

        this.cont_win = PlayerPrefs.GetInt("cont_win",0);
        this.update_count_win_ui();

        if (this.carrot.get_status_sound()) this.carrot.game.load_bk_music(this.sound[0]);

        this.on_load_data_shop();
    }

    private void check_exit_app()
    {
        if (this.panel_play.activeInHierarchy)
        {
            this.btn_back_home();
            this.carrot.set_no_check_exit_app();
        }
    }

    public void btn_reset()
    {
        this.carrot.play_sound_click();
        this.domino_manager.reset();
    }

    public void btn_setting()
    {
        this.ads.show_ads_Interstitial();
        Carrot.Carrot_Box box_setting=this.carrot.Create_Setting();
        box_setting.set_act_before_closing(this.after_closer_setting);
        
        Carrot.Carrot_Box_Item item_shop_domino=box_setting.create_item_of_top("shop_domino");
        item_shop_domino.set_title("Domino's shop");
        item_shop_domino.set_tip("Change the styles of dominoes");
        item_shop_domino.set_icon_white(this.sp_icon_shop_domino_nomal);
        item_shop_domino.set_act(this.btn_shop);
    }

    private void after_closer_setting(List<string> list_change)
    {
        foreach(string s in list_change)
        {
            if (s == "list_bk_music") this.carrot.game.load_bk_music(this.sound[0]);
        }

        if (this.carrot.get_status_sound())
            this.sound[0].Play();
        else
            this.sound[0].Stop();
    }

    private void act_play()
    {
        this.ads.DestroyBannerAd();
        this.ads.show_ads_Interstitial();
        this.carrot.play_sound_click();
        this.panel_home.SetActive(false);
        this.panel_play.SetActive(true);
        this.domino_manager.reset();
    }

    public void btn_play_one_play()
    {
        this.domino_manager.set_model_play(false);
        this.act_play();
    }

    public void btn_play_two_play()
    {
        this.domino_manager.set_model_play(true);
        this.act_play();
    }

    public void play_sound(int index_sound)
    {
        if (this.carrot.get_status_sound()) this.sound[index_sound].Play();
    }

    public void btn_user()
    {
        this.carrot.user.show_login();
    }

    public void btn_rank()
    {
        this.ads.show_ads_Interstitial();
        this.carrot.game.Show_List_Top_player();
    }

    public void btn_rate()
    {
        this.carrot.show_rate();
    }

    public void btn_share()
    {
        this.carrot.show_share();
    }

    public void btn_carrot_app()
    {
        this.carrot.show_list_carrot_app();
    }

    public void btn_back_home()
    {
        this.ads.ShowBannerAd();
        this.ads.show_ads_Interstitial();
        this.carrot.play_sound_click();
        this.panel_home.SetActive(true);
        this.panel_play.SetActive(false);
        this.domino_manager.back_menu();
    }

    public void btn_shop()
    {
        int index_style_sel = this.domino_manager.get_index_style();

        this.carrot.play_sound_click();
        this.box_shop=this.carrot.Create_Box("list_shop");
        box_shop.set_icon(this.sp_icon_shop);
        box_shop.set_title("Shop");

        /*Nomal*/
        Carrot.Carrot_Box_Item item_d_nomal=box_shop.create_item("domino_nomal");
        item_d_nomal.set_title("Normal");
        item_d_nomal.set_tip("Default basic domino style of the game");
        item_d_nomal.set_icon_white(this.sp_icon_shop_domino_nomal);
        item_d_nomal.set_act(()=>this.change_style_domino(0));
        
        if(index_style_sel==0) item_d_nomal.GetComponent<Image>().color = this.carrot.get_color_highlight_blur(150);

        /*Diamo*/
        Carrot.Carrot_Box_Item item_d_diamo = box_shop.create_item("domino_diamo");
        item_d_diamo.set_title("Diamond");
        item_d_diamo.set_tip("Red jade domino design");
        item_d_diamo.set_icon_white(this.sp_icon_shop_domino_diamo);
        item_d_diamo.set_act(() => this.change_style_domino(1));

        if (index_style_sel != 1)
        {
            if (this.list_buy_domino[1] == true)
            {
                Carrot.Carrot_Box_Btn_Item item_d_diamo_ads = item_d_diamo.create_item();
                item_d_diamo_ads.set_icon(this.sp_icon_shop_ads);
                item_d_diamo_ads.set_color(this.carrot.color_highlight);
                item_d_diamo_ads.set_act(() => this.watch_ads_get_domino_style(1));

                Carrot.Carrot_Box_Btn_Item item_d_diamo_buy = item_d_diamo.create_item();
                item_d_diamo_buy.set_icon(this.sp_icon_shop_buy);
                item_d_diamo_buy.set_color(this.carrot.color_highlight);
                item_d_diamo_buy.set_act(() => this.buy_style_domino(1));
            }

        }
        else
        {
            item_d_diamo.GetComponent<Image>().color = this.carrot.get_color_highlight_blur(150);
        }


        /*Wood*/
        Carrot.Carrot_Box_Item item_d_wood = box_shop.create_item("domino_wood");
        item_d_wood.set_title("Wood");
        item_d_wood.set_tip("Wood domino design");
        item_d_wood.set_icon_white(this.sp_icon_shop_domino_wood);
        item_d_wood.set_act(() => this.change_style_domino(2));

        if (index_style_sel != 2)
        {
            if (this.list_buy_domino[2] == true)
            {
                Carrot.Carrot_Box_Btn_Item item_d_wood_ads = item_d_wood.create_item();
                item_d_wood_ads.set_icon(this.sp_icon_shop_ads);
                item_d_wood_ads.set_color(this.carrot.color_highlight);
                item_d_wood_ads.set_act(() => this.watch_ads_get_domino_style(2));

                Carrot.Carrot_Box_Btn_Item item_d_wood_buy = item_d_wood.create_item();
                item_d_wood_buy.set_icon(this.sp_icon_shop_buy);
                item_d_wood_buy.set_color(this.carrot.color_highlight);
                item_d_wood_buy.set_act(() => this.buy_style_domino(2));
            }
        }
        else
        {
            item_d_wood.GetComponent<Image>().color = this.carrot.get_color_highlight_blur(150);
        }

        /*Black*/
        Carrot.Carrot_Box_Item item_d_black = box_shop.create_item("domino_black");
        item_d_black.set_title("Black");
        item_d_black.set_tip("Black domino design");
        item_d_black.set_icon_white(this.sp_icon_shop_domino_black);
        item_d_black.set_act(() => this.change_style_domino(3));

        if (index_style_sel != 3)
        {
            if (this.list_buy_domino[3] == true)
            {
                Carrot.Carrot_Box_Btn_Item item_d_black_ads = item_d_black.create_item();
                item_d_black_ads.set_icon(this.sp_icon_shop_ads);
                item_d_black_ads.set_color(this.carrot.color_highlight);
                item_d_black_ads.set_act(() => this.watch_ads_get_domino_style(3));

                Carrot.Carrot_Box_Btn_Item item_d_black_buy = item_d_black.create_item();
                item_d_black_buy.set_icon(this.sp_icon_shop_buy);
                item_d_black_buy.set_color(this.carrot.color_highlight);
                item_d_black_buy.set_act(() => this.buy_style_domino(3));
            }
        }
        else
        {
            item_d_black.GetComponent<Image>().color = this.carrot.get_color_highlight_blur(150);
        }
    }

    public void change_style_domino(int index_style)
    {
        if (this.list_buy_domino[index_style])
        {
            this.show_option_used_domino(index_style);
        }
        else
        {
            if (this.box_shop != null) this.box_shop.close();
            this.carrot.play_sound_click();
            this.domino_manager.change_style_domino(index_style);
        }
    }

    private void show_option_used_domino(int index_style)
    {
        this.box_msg_shop=this.carrot.Show_msg("Domino Shop", "You can buy or watch ads to get this item!",Carrot.Msg_Icon.Question);
        this.box_msg_shop.add_btn_msg("Buy",()=>this.buy_style_domino(index_style));
        this.box_msg_shop.add_btn_msg("Watch ads",()=>this.watch_ads_get_domino_style(index_style));
        this.box_msg_shop.add_btn_msg("Cancel",this.close_option_domino_shop);
    }

    private void close_option_domino_shop()
    {
        this.box_msg_shop.close();
    }

    private void buy_style_domino(int index_style)
    {
        this.sel_index_shop_item_temp = index_style;
        this.carrot.shop.buy_product(2);
    }

    private void watch_ads_get_domino_style(int index_style)
    {
        this.sel_index_shop_item_temp = index_style;
        this.ads.show_ads_Interstitial();
    }

    private void on_buy_success(string s_id)
    {
        if (s_id == this.carrot.shop.get_id_by_index(2))
        {
            if (this.box_msg_shop != null) this.box_msg_shop.close();
            if (this.box_shop != null) this.box_shop.close();
            PlayerPrefs.SetInt("is_buy_style_"+this.sel_index_shop_item_temp, 1);
            this.domino_manager.change_style_domino(this.sel_index_shop_item_temp);
            this.box_msg_shop = this.carrot.Show_msg("Domino Shop", "You can use new style for dominoes!", Carrot.Msg_Icon.Success);
            this.on_load_data_shop();
        }
    }

    private void on_ads_Rewarded_success()
    {
        if (this.box_msg_shop != null) this.box_msg_shop.close();
        if (this.box_shop != null) this.box_shop.close();
        this.domino_manager.change_style_domino(this.sel_index_shop_item_temp);
        this.box_msg_shop = this.carrot.Show_msg("Domino Shop", "You can use new style for dominoes!", Carrot.Msg_Icon.Success);
    }

    public void add_count_win()
    {
        this.cont_win++;
        PlayerPrefs.SetInt("cont_win",this.cont_win);
        this.carrot.game.update_scores_player(this.cont_win);
        this.update_count_win_ui();
    }

    private void update_count_win_ui()
    {
        this.txt_count_win_main.text = this.cont_win.ToString();
    }

    private void on_load_data_shop()
    {
        for(int i = 1; i < this.list_buy_domino.Length; i++)
        {
            if (PlayerPrefs.GetInt("is_buy_style_"+i, 0) == 0)
                this.list_buy_domino[i] = true;
            else
                this.list_buy_domino[i] = false;
        }
    }
}
