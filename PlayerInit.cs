
public class PlayerInit : SaveInfo
{
    public PlayerInit()
    {
        maxCountMulticolor = 3;
        maxCountBeaver = 3;
        maxCountExplosive = 3;
        maxCountTimestop = 3;

        curCountMulticolor = 1;
        curCountBeaver = 1;
        curCountExplosive = 1;
        curCountTimestop = 1;

        skillPoint = 1;
        playerExp = 0;

       // maxOpenLvl = 80;
    }
}

public enum PlayerSkill
{
    SK_BONUS_MAX_MULTICOLOR = 0,        //0
    SK_BONUS_MAX_BEAVER = 1,      //1
    SK_BONUS_MAX_EXPLOSIVE = 2,       //2
    SK_BONUS_MAX_TIMESTOP = 3,     //3
    
    SK_MULTICOLOR_POWER = 4,     //4
    SK_BEAVER_POWER = 5,   //5
    SK_EXPLOSIVE_POWER = 6,  //6
    SK_TIMESTOP_POWER = 7,     //7
    
    SK_BONUS_SPEED = 8,//8

    SK_BONUS_EXP = 9
}

public static class levelUp
{
    public static bool CheckLevelUp(SaveInfo player)
    {
        int prevLvl = player.playerLvl;

        while (player.playerExp >= pointToNextLvl(player.playerLvl))
        {
            levelUpProcess(player);
        }

        return prevLvl < player.playerLvl;
    }

    public static int pointToNextLvl(int lvl)
    {
        if(lvl==0)
            return 2000;

        return (int)((lvl * lvl * 10000)*0.75);
    }

    static void levelUpProcess(SaveInfo player)
    {
        player.playerLvl++;
        player.skillPoint++;
    }
}