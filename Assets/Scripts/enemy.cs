using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemy : MonoBehaviour
{
    public GameObject PanelWin;
    public GameObject PanelWinGame;
    public GameManager gm;
    public float Health, TimeOfDeath=-1;
    public int Reward;
    private float _Damage, _Speed;
    private Vector2[] _Route;
    private int _Section = 0;

    void Start()
    {
        _Route = new Vector2[] { new Vector2(-600f, 110f), new Vector2(-600f, -1090f), new Vector2(300, -1090), new Vector2(300, 1000), new Vector2(900, 1000), new Vector2(900, 110), new Vector2(1960, 110) };
        switch (this.tag)
        {
            case "Recruit":
                {
                    this.Health = 30f;
                    this._Damage = 10;
                    this.Reward = 7;
                    this._Speed = 200;
                    this.TimeOfDeath = -1;
                    break;
                }
            case "Officer":
                {
                    this.Health = 70f;
                    this._Damage = 20;
                    this.Reward = 20;
                    this._Speed = 200;
                    this.TimeOfDeath = -1;
                    break;
                }
            case "SpecialForces":
                {
                    this.Health = 100f;
                    this._Damage = 40;
                    this.Reward = 20;
                    this._Speed = 200;
                    this.TimeOfDeath = -1;
                    break;
                }
        }
    }

    void Update()
    {
        if (gm.play)
        {
            if (_Section == _Route.Length)
            {
                gm.TowerHealth -= this._Damage;
                gm.enemy.Remove(this.gameObject);
                if (gm.TowerHealth>0 && gm.enemy.Count == 0 && gm.CourseEnemy.Count == 0)
                {
                    gm.CurrentLevel++;
                    PlayerPrefs.SetInt("Level", gm.CurrentLevel);
                    PlayerPrefs.Save();
                    PanelWin.SetActive(true);
                    gm.play = false;
                }
                Destroy(this.gameObject);
            }
            else
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(_Route[this._Section].x, this.transform.position.y, _Route[this._Section].y), _Speed * Time.deltaTime);
                if (this.transform.position == new Vector3(_Route[this._Section].x, this.transform.position.y, _Route[this._Section].y))
                    _Section++;
            }
            if (TimeOfDeath>0 && TimeOfDeath<=Time.time)
            {
                gm.coins += Reward;
                if (gm.enemy.Count==0 && gm.CourseEnemy.Count == 0)
                {
                    if (gm.CurrentLevel == gm.StarCoins.Count)
                    {
                        PanelWinGame.SetActive(true);
                        gm.CurrentLevel = 1;
                    }
                    else
                    {
                        PanelWin.SetActive(true);
                        gm.CurrentLevel++;
                    }
                    PlayerPrefs.SetInt("Level", gm.CurrentLevel);
                    PlayerPrefs.Save();
                    gm.play = false;
                }
                Destroy(this.gameObject);
            }
        }
    }
}
