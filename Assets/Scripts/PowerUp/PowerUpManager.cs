﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager powerUpManager;
    ///All power ups in game
    public List<PowerUp> powerUps;
    ///All power ups player has
    public List<PowerUpType> playerPowerUps;
    ///five power up player selected 3 active 2 passive
    public List<PowerUpType> selectedActivePowerUps;
    public int activePowerUplimit;
    Player p;
    Player_Shoot pShoot;
    void Awake()
    {
        if (PowerUpManager.powerUpManager == null)
        {
            PowerUpManager.powerUpManager = this;
        }
        else if (PowerUpManager.powerUpManager != null)
        {
            Destroy(PowerUpManager.powerUpManager.gameObject);
            PowerUpManager.powerUpManager = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        playerPowerUps = SaveAndLoadGameData.instance.savedData.playerPowerUps;
        selectedActivePowerUps = SaveAndLoadGameData.instance.savedData.selectedActivePowerUps;
    }

    ///Just For temporary powers
    ////Oyuncu yeteneği aktif hale getirince çağıralacak
    public void GivePower( PowerUp powerUp)
    {
        p = FindObjectOfType<Player>();
        pShoot = p.GetComponent<Player_Shoot>();
        
        ///Eğer aktif bir power up varsa geri dön
        ///Bunu şimdilik geçici bir çözüm olarak koyuyorum belki power uplar stacklenebilir bilmiyorum.
        if(p.isThereActivePowerUp)
        {
            return;
        }
        ///Geçici kod buraya kadar

        switch (powerUp.powerUpType)
        {
            case PowerUpType.MachineGun:
                powerUp.tempData.Clear();
                Debug.Log("machine gun 0");
                pShoot.canRecoil = false;
                powerUp.tempData.Add(pShoot.NormalShootTimeLimit);
                pShoot.NormalShootTimeLimit = 0.2f;
                p.isThereActivePowerUp = true;
                StartCoroutine( GetPowerBack(powerUp) );
            break;
            case PowerUpType.UnPerfectShield:
                Debug.Log("UnPerfect Shield 0");
                pShoot.canShoot = false;
                GameObject a = Instantiate(powerUp.neededPrefab,p.transform.position,Quaternion.identity);
                a.transform.SetParent(p.transform);
                //hasar almasını engelle
                p.dontGetDamage = true;
                p.isThereActivePowerUp = true;
                StartCoroutine( GetPowerBack(powerUp) );
            break;
            case PowerUpType.ExplosivePower:
                powerUp.tempData.Clear();
                ProjectileManager manager =  FindObjectOfType<ProjectileManager>();
                Debug.Log("Explosive power");
/*                 // tempdata 0 normal
                powerUp.tempData.Add(manager.NormalRecoilForce);
                // tempdata 1 middle
                powerUp.tempData.Add(manager.MiddleRecoilForce);
                // tempdata 2 powerdul
                powerUp.tempData.Add(manager.PowerfulRecoilForce); */

                manager.NormalRecoilForce *= 2;
                manager.MiddleRecoilForce *= 2;
                manager.PowerfulRecoilForce *= 2;

                p.isThereActivePowerUp = true;
                p.CallExplosivePowerUpEnumerator();
            break;

            default:
            break;
        }
    }
    public IEnumerator GetPowerBack( PowerUp powerUp)
    {
       
        if(PowerUpType.MachineGun == powerUp.powerUpType)
        {
            Debug.Log("machine gun 1");
            yield return new WaitForSeconds(powerUp.usingTime);
            pShoot.NormalShootTimeLimit =powerUp.tempData[0];
            pShoot.canRecoil = true;
            p.isThereActivePowerUp = false;
        }
        else if(powerUp.powerUpType == PowerUpType.UnPerfectShield)
        {
            Debug.Log("UnPerfect Shield 1");
            yield return new WaitForSeconds(powerUp.usingTime);
            p.dontGetDamage = false;
            pShoot.canShoot = true;
            p.isThereActivePowerUp = false;
        }
        else if(powerUp.powerUpType == PowerUpType.ExplosivePower)
        {
            Debug.Log("Explosve Power 1");
            yield return new WaitForSeconds(powerUp.usingTime);
            ProjectileManager manager =  FindObjectOfType<ProjectileManager>(); 

            manager.NormalRecoilForce /= 2;
            manager.MiddleRecoilForce /= 2;
            manager.PowerfulRecoilForce /= 2;
            p.isThereActivePowerUp = false;
        }
    }
     public IEnumerator GetPowerBack( PowerUpType powerUpType)
    {
        PowerUp powerUp = PowerUpManager.powerUpManager.powerUps
            .FirstOrDefault(s => s.powerUpType == powerUpType);
            
        if(PowerUpType.MachineGun == powerUp.powerUpType)
        {
            Debug.Log("machine gun 1");
            yield return new WaitForSeconds(powerUp.usingTime);
            pShoot.NormalShootTimeLimit =powerUp.tempData[0];
            pShoot.canRecoil = true;
            p.isThereActivePowerUp = false;
        }
        else if(powerUp.powerUpType == PowerUpType.UnPerfectShield)
        {
            Debug.Log("UnPerfect Shield 1");
            yield return new WaitForSeconds(powerUp.usingTime);
            p.dontGetDamage = false;
            pShoot.canShoot = true;
            p.isThereActivePowerUp = false;
        }
        else if(powerUp.powerUpType == PowerUpType.ExplosivePower)
        {
            Debug.Log("Explosve Power 1");

            ProjectileManager manager =  FindObjectOfType<ProjectileManager>(); 
            manager.NormalRecoilForce /= 2;
            manager.MiddleRecoilForce /= 2;
            manager.PowerfulRecoilForce /= 2;

            p.isThereActivePowerUp = false;
        }
    }
    public void ObtainPower(PowerUpType powerUpType)
    {
        playerPowerUps.Add(powerUpType);
        SavePlayersPowerUp();
    }
    void SavePlayersPowerUp()
    {
        SaveAndLoadGameData.instance.savedData.playerPowerUps = this.playerPowerUps;
        SaveAndLoadGameData.instance.Save();
    }
    public bool SelectPowerUp(PowerUp powerUp)
    {
       
        if(powerUp.usageType == UsageType.Temporary && selectedActivePowerUps.Count < activePowerUplimit)
        {
            selectedActivePowerUps.Add(powerUp.powerUpType);
            Debug.Log("Power up selected. "+ powerUp.powerUpName);
            return true;
        }
        else
        {
            Debug.Log("You can not have more then"+ activePowerUplimit +"power up 3 Active");
            return false;
        }
    }
    public bool DeselectPowerUp(PowerUp powerUp)
    {
       
        if(powerUp.usageType == UsageType.Temporary && selectedActivePowerUps.Count > 0)
        {
            selectedActivePowerUps.Remove(powerUp.powerUpType);
            return true;
        }
        else
        {
            Debug.Log("There is no power up selected. Select one for remove");
            return false;
        }
    }
    /* public PowerUpType[] SelectThreeRandomPowerUp()
    {
        List<PowerUp> list = new List<PowerUp>();
        list.AddRange(powerUps);
        PowerUpType[] powers = new PowerUpType[3];
        
        for (int i = 0; i < 3; i++)
        {
            int a = UnityEngine.Random.Range(0,list.Count);
            powers[i] = list[a].powerUpType;
            list.RemoveAt(a);
        }
        return powers;
    } */

   /*  public void Call_SpawnPowerUp(Vector2 pos,RoomController room,PowerUpType powerUpType)
    {
        StartCoroutine(SpawnPowerUpEnumerator(pos,room, onPowerUpCreated,powerUpType));
    }
     public IEnumerator SpawnPowerUpEnumerator(Vector2 pos,
        RoomController room,
        Action<PowerUpObject,RoomController,PowerUpType> onPowerUpCreated,
        PowerUpType powerUpType)
    {

        var particle = Instantiate(powerUpSpawnParticle,pos,Quaternion.identity);
        particle.GetComponent<ParticleSystem>().Play();
    
        yield return new WaitForSeconds(2f);
            
        PowerUpObject enemy = SpawnPowerUp(pos);
        onPowerUpCreated(enemy,room,powerUpType);
    }
    private PowerUpObject SpawnPowerUp(Vector2 pos)
    {
        return Instantiate(powerUpObject,pos,Quaternion.identity).GetComponent<PowerUpObject>();
    }
    void onPowerUpCreated(PowerUpObject obj,RoomController room,PowerUpType powerUpType)
    {
        obj.powerUpType = powerUpType;
        obj.SetUp();
        obj.room = room;
        room.roomPowerUps.Add(obj.gameObject);
    } */
}
