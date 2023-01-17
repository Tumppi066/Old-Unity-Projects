using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Michsky.UI.ModernUIPack;

public class Universe : MonoBehaviour
{
    public float gravitationalConstant = 1;
    public float timeStep = 1;
    public float timeStepSensitivity = 1;
    public float physicsTimeStep = 0;
    public float dayCounter = 0;
    public string date = "01/01/2000";
    public float daysPerSecond;
    public int[] monthAmountOfDays = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    public int valueRounding = 3;
    int day = 1;
    int month = 1;
    int year = 2000;
    public TMP_Text timeStepText;
    public bool useNBodyPhysics = false;
    public float daysToSkip;
    public string dateToSkipString;
    public int maxDaysPerSecond = 50;
    public GameObject persistentCanvasPrefab;

    private void Start()
    {
        day = System.DateTime.Now.Day;
        month = System.DateTime.Now.Month;
        year = System.DateTime.Now.Year;
        physicsTimeStep = Time.fixedDeltaTime;

        if (!FindObjectOfType<PersistentCanvasManager>())
        {
            Instantiate(persistentCanvasPrefab);
        }
    }
    public void SetTimeStepSensitivity(float sens)
    {
        timeStepSensitivity = sens;
    }
    public void IncreaseTimeStep()
    {
        timeStep += timeStepSensitivity * physicsTimeStep * 100 * 0.1f;
    }
    public void ChangeRounding(float rounding)
    {
        valueRounding = (int)rounding;
    }
    public void DecreaseTimeStep()
    {
        timeStep -= timeStepSensitivity * physicsTimeStep * 100 * 0.1f;
        if (timeStep < 0)
        {
            timeStep = 0;
        }
    }
    public void DeleteLogs(GameObject ob)
    {
        try
        {
            FindObjectOfType<PersistentCanvasManager>().DeleteLogs(ob);
        }
        catch
        {
            Debug.Log("<color=yellow>" + gameObject.name + "</color> " + "Unable to delete logs");
        }
    }
    public void Print(string message, GameObject ob)
    {
        Debug.Log("<color=yellow>" + ob.name + "</color> " + message);
    }

    public void SkipToDate(string dateToSkipTo)
    {
        int toDay;
        int toMonth;
        int toYear;
        try
        {
            dateToSkipString = dateToSkipTo;
            string[] date = dateToSkipTo.Split('/');
            toDay = int.Parse(date[0]);
            toMonth = int.Parse(date[1]);
            toYear = int.Parse(date[2]);

            if (toMonth > 12)
            {
                Print("<color=red>Please enter a valid date</color>", gameObject);
            }
            else if (toDay > monthAmountOfDays[toMonth] || toDay < 1)
            {
                Print("<color=red>Please enter a valid date</color>", gameObject);
            }
            else
            {
                Print("Skipping to date " + dateToSkipTo, gameObject);
                daysToSkip = (toYear - year) * 365 + (toMonth - month) * 30 + (toDay - day);
            }
        }
        catch { }

    }

    void UpdateDate()
    {
        string dayString;
        string monthString;
        string yearString;
        if(day >= monthAmountOfDays[month-1])
        {
            month += 1;
            day = 1;
        }
        if(month == 13)
        {
            year += 1;
            month = 1;
            day = 1;
        }
        if(day < 10){ dayString = "0" + day.ToString(); }
        else{ dayString = day.ToString(); }
        if(month < 10) { monthString = "0" + month.ToString(); }
        else { monthString = month.ToString(); }
        yearString = year.ToString();
        date = dayString + "/" + monthString + "/" + yearString;

    }

    void FixedUpdate()
    {
        if(dateToSkipString == date)
        {
            daysToSkip = 0;
            dateToSkipString = null;
            FindObjectOfType<Universe>().DeleteLogs(gameObject);
            FindObjectOfType<Universe>().Print("<color=green>Timeskip complete</color>", gameObject);
            timeStep = 0.01f;
        }
        if(daysToSkip < 0)
        {
            FindObjectOfType<Universe>().Print("It is not allowed to go back in time", gameObject);
            daysToSkip = 0;
        }
        if (daysToSkip != 0)
        {
            timeStep = 0.0125f * daysToSkip;
            daysPerSecond = 1 / (1 / timeStep * physicsTimeStep);
            if (daysPerSecond > maxDaysPerSecond)
            {
                timeStep = maxDaysPerSecond * physicsTimeStep;
            }
            FindObjectOfType<Universe>().DeleteLogs(gameObject);
            FindObjectOfType<Universe>().Print("Days to timeskip " + daysToSkip, gameObject);
            FindObjectOfType<Universe>().Print("Approximate time to timeskip " + Math.Round(daysToSkip/maxDaysPerSecond, 1) + " seconds at " + maxDaysPerSecond + " days/s", gameObject);
        }
        try
        {
            dayCounter += 1 * timeStep;
            if(dayCounter > 1)
            {
                float overshoot = dayCounter - 1;
                int overshootDays = (int)Math.Floor(overshoot);
                overshoot = overshoot - overshootDays;
                dayCounter = overshoot;
                day += 1 + overshootDays;
                if(daysToSkip != 0)
                {
                    daysToSkip -= 1 + overshootDays;
                }
            }
            if(dayCounter < -1)
            {
                dayCounter = dayCounter + 1;
                day -= 1;
                if (daysToSkip != 0)
                {
                    daysToSkip += 1;
                }
            }
            daysPerSecond = 1 / (1 / timeStep * physicsTimeStep);
            if(daysPerSecond > maxDaysPerSecond)
            {
                float neededSpeed = maxDaysPerSecond * physicsTimeStep;
                timeStep = neededSpeed;
                daysPerSecond = maxDaysPerSecond;
            }
            timeStepText.text = Math.Round(daysPerSecond, 3).ToString() + " days/s";
            UpdateDate();
            foreach (GameObject ob in GameObject.FindGameObjectsWithTag("ShowDate")){
                if (ob.GetComponent<TMP_Text>())
                {
                    ob.GetComponent<TMP_Text>().text = date;
                    if (ob.GetComponentInChildren<ProgressBar>())
                    {
                        ob.GetComponentInChildren<ProgressBar>().ChangeValue(dayCounter);
                    }
                }
            }
        }
        catch { }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            IncreaseTimeStep();
        }
        if (Input.GetKey(KeyCode.Q))
        {
            DecreaseTimeStep();
        }
    }

}
