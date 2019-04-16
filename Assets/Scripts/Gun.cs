using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Gun : MonoBehaviour
{
    public GameManager gm;
    public GameObject Bullet;
    private List<Vector3> _ofSets = new List<Vector3>();
    public int cost;
    public float DamageRadius;
    private float _Damage, _ReloadTime, _LastShotTime, _BulletSpeed;
    
    void Start()
    {
        switch (this.tag)
        {
            case "MachineGun":
                {
                    this.DamageRadius = 1000f;
                    this._ReloadTime = 2f;
                    this._Damage = 10f;
                    this._BulletSpeed = 3000f;
                    this.cost = 50;
                    this._ofSets.Add(new Vector3(0, 160, 0));
                    break;
                }
            case "DoubleMachineGun":
                {
                    this.DamageRadius = 1000f;
                    this._ReloadTime = 1.8f;
                    this._Damage = 20f;
                    this._BulletSpeed = 3000f;
                    this.cost = 100;
                    this._ofSets.Add(new Vector3(-70, 160, 0));
                    this._ofSets.Add(new Vector3(70, 160, 0));
                    break;
                }
            case "Ordnance":
                {
                    this.DamageRadius = 2000f;
                    this._ReloadTime = 3.5f;
                    this._Damage = 60f;
                    this._BulletSpeed = 2000f;
                    this.cost = 200;
                    this._ofSets.Add(new Vector3(0, 70, 0));
                    break;
                }
        }
    }

    private void _SpawnCartrdge(float distance)
    {
        foreach (Vector3 ofSet in this._ofSets)
        {
            GameObject bl = Instantiate(Bullet, new Vector3(this.transform.position.x + ofSet.x, this.transform.position.y + ofSet.y, this.transform.position.z+ofSet.z), Quaternion.identity);
            bl.GetComponent<Rigidbody>().velocity = new Vector3(this._BulletSpeed * Mathf.Cos((90f - this.transform.rotation.eulerAngles.y) * Mathf.PI / 180f), 0, this._BulletSpeed * Mathf.Sin((90f - this.transform.rotation.eulerAngles.y) * Mathf.PI / 180f));
            Destroy(bl, (float)(distance / _BulletSpeed));
        }
    }

    void Update()
    {
        if (gm && gm.play && gm.enemy.Count != 0)
        {
            for (int i = 0; i < gm.enemy.Count(); i++)
            {
                GameObject gun = this.gameObject;
                if (Mathf.Sqrt(Mathf.Pow(gun.transform.position.z - gm.enemy[i].transform.position.z, 2) + Mathf.Pow(gun.transform.position.x - gm.enemy[i].transform.position.x, 2)) < DamageRadius)
                {
                    float distance = Mathf.Sqrt(Mathf.Pow(gun.transform.position.z - gm.enemy[i].transform.position.z, 2) + Mathf.Pow(gun.transform.position.x - gm.enemy[i].transform.position.x, 2));
                    GameObject enemy = gm.enemy[i];
                    Quaternion target = new Quaternion();
                    if (gun.transform.position.x > enemy.transform.position.x)
                    {
                        target = Quaternion.Euler(0, (float)(180f / Mathf.PI * Mathf.Atan((gun.transform.position.z - enemy.transform.position.z) / (gun.transform.position.x - enemy.transform.position.x))) * -1f - 90, 0);
                        gun.transform.rotation = Quaternion.Slerp(gun.transform.rotation, target, 5f);
                    } else if (gun.transform.position.x < enemy.transform.position.x)
                    {
                        target = Quaternion.Euler(0, (float)(180f / Mathf.PI * Mathf.Atan((gun.transform.position.z - enemy.transform.position.z) / (gun.transform.position.x - enemy.transform.position.x))) * -1f + 90, 0);
                        gun.transform.rotation = Quaternion.Slerp(gun.transform.rotation, target, 5f);
                    }
                    if (Time.time - this._LastShotTime > this._ReloadTime)
                    {
                        //GameObject bl = Instantiate(Bullet, new Vector3(this.transform.position.x, 160, this.transform.position.z), Quaternion.identity);
                        //bl.GetComponent<Rigidbody>().velocity = new Vector3(_BulletSpeed * Mathf.Cos((90f - this.transform.rotation.eulerAngles.y) * Mathf.PI / 180f), 0, _BulletSpeed * Mathf.Sin((90f - this.transform.rotation.eulerAngles.y) * Mathf.PI / 180f));
                        _SpawnCartrdge(distance);

                        enemy.GetComponent<enemy>().Health -= _Damage;
                        if (enemy.GetComponent<enemy>().Health <= 0)
                        {
                            enemy.GetComponent<enemy>().TimeOfDeath = Time.time + (float)distance / _BulletSpeed;
                            gm.enemy.Remove(enemy);
                        }
                        _LastShotTime = Time.time;
                    }
                    break;
                }
            }
        }
    }
}
