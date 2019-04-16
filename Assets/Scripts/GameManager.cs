using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _MainPlane;
    [SerializeField] private GameObject _Plane;
    [SerializeField] private GameObject _PanelMenu;
    [SerializeField] private GameObject _PanelGameOver;
    [SerializeField] private GameObject _PanelWin;
    [SerializeField] private GameObject _PanelWinGame;
    [SerializeField] private Button _buttonStartGame;
    [SerializeField] private Button _buttonPause;
    [SerializeField] private Button _buttonPlay;
    [SerializeField] private Button _buttonNextLevel;
    [SerializeField] private Button _buttonRestartLevel;
    [SerializeField] private Button _buttonMachineGun;
    [SerializeField] private Button _buttonDoubleMachineGun;
    [SerializeField] private Button _buttonOrdnance;
    [SerializeField] private GameObject _MachineGun;
    [SerializeField] private GameObject _DoubleMachineGun;
    [SerializeField] private GameObject _Ordnance;
    [SerializeField] private GameObject _ObjDamageRadius;
    [SerializeField] private GameObject _Bullet;
    [SerializeField] private GameObject _Projectile;
    [SerializeField] private Text _CoinsText;
    [SerializeField] private Text _HealthText;
    [SerializeField] private Text _LevelText;
    [SerializeField] private GameObject _EnemyRecruit;
    [SerializeField] private GameObject _EnemyOfficer;
    [SerializeField] private GameObject _EnemySpecialForces;
    public struct course
    {
        public string tag;
        public float Time;
        public course(string tg, float tm)
        {
            tag = tg;
            Time = tm;
        }
    }
    List<List<course>> _Levels = new List<List<course>>();
    const int FieldWidth = 11, FieldLength = 16;
    private int _LastCoins;
    private float _LastTowerHealth, _PauseTime, _TimeSartOfPause, _TimeStartOfGame;
    public int X0 = -2700, Z0 = 1600, step = 300, coins;
    public int indPositionSelection, CurrentLevel = 3;
    public float TowerHealth;
    public GameObject[,] plans = new GameObject[FieldWidth, FieldLength];
    public bool[,] availablePlans, StartAvailablePlans;
    public bool play = false;
    public List<GameObject> enemy = new List<GameObject>();
    public List<GameObject> MountedGuns = new List<GameObject>();
    public List<course> CourseEnemy = new List<course>();
    public List<int> StarCoins = new List<int>();
    public Vector2[] Route;

    void Start()
    {
        // if (PlayerPrefs.HasKey("Level"))
        //    CurrentLevel = PlayerPrefs.GetInt("Level");
        _LevelText.text = "Level: " + CurrentLevel;
        _buttonPlay.gameObject.SetActive(false);
        _PanelGameOver.SetActive(false);
        _PanelWin.SetActive(false);
        _PanelWinGame.SetActive(false);
        _buttonMachineGun.onClick.AddListener(delegate () { PositionSelection(0); });
        _buttonDoubleMachineGun.onClick.AddListener(delegate () { PositionSelection(1); });
        _buttonOrdnance.onClick.AddListener(delegate () { PositionSelection(2); });
        _buttonStartGame.onClick.AddListener(delegate () { StartGame(); });
        _buttonRestartLevel.onClick.AddListener(delegate () { StartGame(); });
        _buttonNextLevel.onClick.AddListener(delegate () { StartGame(); });
        _buttonPlay.onClick.AddListener(delegate () { PlayGame(); });
        _buttonPause.onClick.AddListener(delegate () { PauseGame(); });
        StartAvailablePlans = new bool[FieldWidth, FieldLength] {
            { true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,},
            { true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,},
            { true,true,true,true,true,true,true,true,true,true,false,false,false,true,true,true,},
            { true,true,true,true,true,true,true,true,true,true,false,true,false,true,true,true,},
            { true,true,true,true,true,true,true,true,true,true,false,true,false,true,true,true,},
            { false,false,false,false,false,false,false,false,true,true,false,true,false,false,false,false,},
            { true,true,true,true,true,true,true,false,true,true,false,true,true,true,true,true,},
            { true,true,true,true,true,true,true,false,true,true,false,true,true,true,true,true,},
            { true,true,true,true,true,true,true,false,true,true,false,true,true,true,true,true,},
            { true,true,true,true,true,true,true,false,false,false,false,true,true,true,true,true,},
            { true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,},
        };
        availablePlans = new bool[FieldWidth, FieldLength];
        for (int i = 0; i < FieldWidth; i++)
            for (int j = 0; j < FieldLength; j++)
                availablePlans[i,j]  = StartAvailablePlans[i,j];
        GameObject[] Guns = new GameObject[3];
        Guns[0] = _MachineGun;
        Guns[1] = _DoubleMachineGun;
        Guns[2] = _Ordnance;
        for (int i = 0; i < FieldWidth; i++)
            for (int j = 0; j < FieldLength; j++)
                if (availablePlans[i, j])
                {
                    plans[i, j] = Instantiate(_Plane, new Vector3(_MainPlane.transform.position.x + X0 + j * step, _MainPlane.transform.position.y + 1, _MainPlane.transform.position.z + Z0 - i * step), Quaternion.identity, _MainPlane.transform);
                    plans[i, j].SetActive(false);
                    plans[i, j].GetComponent<Piece>().MainPlane = _MainPlane;
                    plans[i, j].GetComponent<Piece>().Guns = Guns;
                    plans[i, j].GetComponent<Piece>().gm = this;
                    plans[i, j].GetComponent<Piece>().ObjDamageRadius = _ObjDamageRadius;
                    plans[i, j].GetComponent<Piece>().Bullet = _Bullet;
                    plans[i, j].GetComponent<Piece>().Projectile = _Projectile;
                }

        using (StreamReader sr = new StreamReader("Assets/Level Editor/Data"))
        {
            string line, tag = "";
            string[] data;
            List<course> ls = new List<course>();
            while ((line = sr.ReadLine()) != null)
            {
                ls.Clear();
                data = line.Split(' ');
                StarCoins.Add(int.Parse(data[0]));
                for (int i = 1; i < data.Count() - 1; i += 2)
                {
                    switch (int.Parse(data[i]))
                    {
                        case 0:
                            tag = "Recruit";
                            break;
                        case 1:
                            tag = "Officer";
                            break;
                        case 2:
                            tag = "SpecialForces";
                            break;
                    }
                    ls.Add(new course(tag, float.Parse(data[i + 1])));
                }
                _Levels.Add(new List<course>(ls));
            }
        }
    }

    private void StartGame()
    {
        _PanelGameOver.SetActive(false);
        _PanelWin.SetActive(false);
        _PanelWinGame.SetActive(false);
        _PanelMenu.SetActive(false);
        _buttonStartGame.gameObject.SetActive(false);
        foreach (GameObject obj in enemy)
            Destroy(obj);
        foreach (GameObject obj in MountedGuns)
            Destroy(obj);
        availablePlans = new bool[FieldWidth, FieldLength];
        for (int i = 0; i < FieldWidth; i++)
            for (int j = 0; j < FieldLength; j++)
                availablePlans[i, j] = StartAvailablePlans[i, j];
        enemy.Clear();
        _PauseTime = 0;
        TowerHealth = 100;
        coins = StarCoins[CurrentLevel-1];
        indPositionSelection = -1;
        _LastCoins = 0;
        _LastTowerHealth = 0;
        CourseEnemy = new List<course>(_Levels[CurrentLevel-1]);
        _TimeStartOfGame = Time.time;
        play = true;
    }
    private void PauseGame()
    {
        play = false;
        _LevelText.text ="Level" + CurrentLevel.ToString();
        _buttonPlay.gameObject.SetActive(true);
        _TimeSartOfPause = Time.time;
        _PanelMenu.SetActive(true);
    }
    private void PlayGame()
    {
        _PanelMenu.SetActive(false);
        _buttonPlay.gameObject.SetActive(false);
        _PauseTime += Time.time - _TimeSartOfPause;
        play = true;
    }

    public int getFieldWidth()
    {
        return FieldWidth;
    }
    public int getFieldLength()
    {
        return FieldLength;
    }
    private void PositionSelection(int ind)
    {
        for (int i = 0; i < FieldWidth; i++)
            for (int j = 0; j < FieldLength; j++)
                if (availablePlans[i, j])
                    plans[i, j].SetActive(true);
        indPositionSelection = ind;
    }

    void Update()
    {
        if (play)
        {
            if (_LastCoins != coins)
            {
                _LastCoins = coins;
                _buttonMachineGun.enabled = coins < 50 ? false : true;
                _buttonDoubleMachineGun.enabled = coins < 100 ? false : true;
                _buttonOrdnance.enabled = coins < 200 ? false : true;
                _CoinsText.text = "Coins: " + coins.ToString();
            }
            if (_LastTowerHealth != TowerHealth)
            {
                _LastTowerHealth = TowerHealth;
                if (TowerHealth<=0)
                {
                    play = false;
                    _HealthText.text = "Health: 0";
                    _PanelGameOver.SetActive(true);
                }
                _HealthText.text = "Health: " + TowerHealth.ToString();
            }
            if (CourseEnemy.Count() != 0 && CourseEnemy[0].Time <= Time.time - _TimeStartOfGame - _PauseTime)
            {
                GameObject SpawnEnemy = new GameObject();
                switch (CourseEnemy[0].tag)
                {
                    case "Recruit":
                        SpawnEnemy = Instantiate(_EnemyRecruit, new Vector3(-3800, 10, 112), Quaternion.Euler(0, 90, 0));
                        break;
                    case "Officer":
                        SpawnEnemy = Instantiate(_EnemyOfficer, new Vector3(-3800, 10, 112), Quaternion.Euler(0, 90, 0));
                        break;
                    case "SpecialForces":
                        SpawnEnemy = Instantiate(_EnemySpecialForces, new Vector3(-3800, 10, 112), Quaternion.Euler(0, 90, 0));
                        break;
                }
                SpawnEnemy.GetComponent<enemy>().gm = this;
                SpawnEnemy.tag = CourseEnemy[0].tag;
                SpawnEnemy.GetComponent<enemy>().PanelWin = _PanelWin;
                SpawnEnemy.GetComponent<enemy>().PanelWinGame = _PanelWinGame;
                enemy.Add(SpawnEnemy);
                CourseEnemy.RemoveAt(0);
            }
        }
    }
}