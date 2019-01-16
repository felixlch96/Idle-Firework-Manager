using UnityEngine;

public class Mission
{
    public string missionDesc;
    public int countNeeded;
    public int countCurrent;
    public int missionType;
    public int gemReward;
    public bool isCompleted;

    //constructor
    public Mission(string desc, int type, int goalCount, int reward)
    {        
        missionDesc = desc;
        countNeeded = goalCount;
        missionType = type;
        countCurrent = 0;
        gemReward = reward;
        isCompleted = false;
    }

    public Mission(string desc, int type, int currentCount, int goalCount, int reward)
    {
        missionDesc = desc;
        countCurrent = currentCount;
        countNeeded = goalCount;
        missionType = type;
        countCurrent = 0;
        gemReward = reward;
        isCompleted = false;
    }

    public void Increment(int amount)
    {
        countCurrent = Mathf.Min(countCurrent + amount, countNeeded);

        //if the mission's requirement has achieved,
        if (countCurrent >= countNeeded && !isCompleted)
        {
            this.isCompleted = true;
        }        
    }
}
