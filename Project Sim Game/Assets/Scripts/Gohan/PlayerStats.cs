using UnityEngine;
using System.Collections;
using System;

public class PlayerStats : MonoBehaviour
{
    private ushort hpTrue;
    private ushort hpApparent;
    public ushort HP
    {
        get
        {
            return (ushort)((int)hpTrue >> 8);
        }
        set
        {
            hpTrue = (ushort)((int)value);
            if (hpTrue < 0x0)
            {
                hpTrue = 0x0;
            }
            if (hpTrue > maxHPTrue)
            {
                hpTrue = maxHPTrue;
            }
        }
    }

    private ushort maxHPTrue;
    [Range(1, 255),SerializeField]private ushort maxHP; // for seeing in inspector
    public ushort MaxHP
    {
        get
        {
            return (ushort)((int)maxHPTrue >> 8);
        }
        set
        {
            maxHPTrue = (ushort)((int) value << 8);
            if(maxHPTrue < 0x0100)
            {
                maxHPTrue = 0x0100;
            }
            if(maxHPTrue > 0xFFFF)
            {
                maxHPTrue = 0xFFFF;
            }
        }
    }

    private int hpIncreaseCounter;
    private int hpDecreaseCounter;

    public float testDamage;
    public float testHeal;

	// Use this for initialization
	void Start ()
    {
        testDamage = 0;
        testHeal = 0;

        hpIncreaseCounter = 0;
        hpDecreaseCounter = 0;
        MaxHP = maxHP;
        hpTrue = maxHPTrue;
	}

    // Update is called once per frame
    // but we only want to update when the global count changes
    //hence tick

    private ushort hpChangeRate = 0;

    void Update ()
    {
        if (GlobalStats.tick)
        {
            //debug 
            hpApparent = HP;

            Damage(testDamage);

            Heal(testHeal);

            testDamage = 0;
            testHeal = 0;
            // end debug
            

            if(hpIncreaseCounter > 0 || hpDecreaseCounter > 0)
            {
                hpChangeRate = (ushort)(maxHP * 16);
                for (ushort s = 0; s < hpChangeRate; s++)
                {
                    int deltaHP = Mathf.Min(hpIncreaseCounter, 1) - Mathf.Min(hpDecreaseCounter, 1);

                    if (hpIncreaseCounter > 0)
                    {
                        hpIncreaseCounter--;
                    }

                    if (hpDecreaseCounter > 0)
                    {
                        hpDecreaseCounter--;
                    }
                    

                    if ((int)hpTrue + deltaHP > (int)maxHPTrue) hpTrue = maxHPTrue;
                    else if ((int)hpTrue + deltaHP < 0) hpTrue = 0;
                    else hpTrue = (ushort)(hpTrue + deltaHP);
                }
            }
            HP = (hpTrue);
        }
	}

    //Takes in a decimal of apparent health recovered
    //converts it to the proper format and applies it to the counter
    public void Heal(float health)
    {
        if(health > 0f)
        {
            hpIncreaseCounter += Mathf.FloorToInt(health * 0x100);
        }
    }

    //Takes in a decimal of apparent damage taken
    //converts it to the proper format and applies it to the counter
    public void Damage(float damage)
    {
        if(damage > 0f)
        {
            hpDecreaseCounter += Mathf.FloorToInt(damage * 0x100);
        }
    }
}