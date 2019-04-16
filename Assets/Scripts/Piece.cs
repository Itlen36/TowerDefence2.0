using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public GameManager gm;
    public GameObject[] Guns;
    public GameObject MainPlane;
    public GameObject ObjDamageRadius;
    public GameObject Bullet;
    public GameObject Projectile;
    GameObject SelectedGun, Radius;

    void OnMouseEnter()
    {
        if (gm.indPositionSelection != -1)
        {
            SelectedGun = Instantiate(Guns[gm.indPositionSelection], new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), Quaternion.identity);
            Radius = Instantiate(ObjDamageRadius, new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z), Quaternion.identity);
            switch (gm.indPositionSelection)
            {
                case 0:
                    this.SelectedGun.tag = "MachineGun";
                    Radius.transform.localScale = new Vector3(2000, 0.01f, 2000);
                    this.SelectedGun.GetComponent<Gun>().Bullet = Bullet;
                    break;
                case 1:
                    this.SelectedGun.tag = "DoubleMachineGun";
                    Radius.transform.localScale = new Vector3(2000, 0.01f, 2000);
                    this.SelectedGun.GetComponent<Gun>().Bullet = Bullet;
                    break;
                case 2:
                    this.SelectedGun.tag = "Ordnance";
                    Radius.transform.localScale = new Vector3(4000, 0.01f, 4000);
                    this.SelectedGun.GetComponent<Gun>().Bullet = Projectile;
                    break;
            }
        }
    }
    void OnMouseExit()
    {
        if (gm.indPositionSelection != -1)
        {
            Destroy(SelectedGun);
            Destroy(Radius);
        }
    }

    private void OnMouseDown()
    {
        gm.indPositionSelection = -1;
        Destroy(Radius);
        for (int i = 0; i < gm.getFieldWidth(); i++)
            for (int j = 0; j < gm.getFieldLength(); j++)
                if (gm.availablePlans[i, j])
                    gm.plans[i, j].SetActive(false);
        gm.availablePlans[(int)((gm.Z0 - this.transform.position.z + 1) / gm.step), (int)((this.transform.position.x - gm.X0) / gm.step)] = false;
        gm.coins -= this.SelectedGun.GetComponent<Gun>().cost;
       // this.SelectedGun.GetComponent<Gun>().Bullet = Bullet;
       // this.SelectedGun.GetComponent<Gun>().Projectile = Projectile;
        this.SelectedGun.GetComponent<Gun>().gm = gm;
        gm.MountedGuns.Add(SelectedGun);
    }
}
