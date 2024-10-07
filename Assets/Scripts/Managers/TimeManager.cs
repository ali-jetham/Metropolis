using System;
using System.Timers;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public event Action OnMinutePassed, OnHourPassed;
    public event Action OnDayPassed;
    public event Action OnMonthPassed;
    public event Action OnYearPassed;

    public event Action On2Am;
    public event Action On7Am;
    public event Action On9Am;
    public event Action On10Am;
    public event Action On12Pm;

    public event Action On10MinutePassed;


    public int Minute { get; set; }
    public int Hour { get; set; }
    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }

    private Timer _timer;
    private bool _on2AmInvoked;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
            Application.quitting += StopTimer;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void StopTimer()
    {
        _timer.Stop();
        _timer.Dispose();
        Debug.Log("TimeManager: timer stopped");
    }

    /// <summary>
    /// Initialize the fields and setup the timer.
    /// </summary>
    public void Initialize()
    {
        Minute = 0;
        Hour = 0;
        Day = 1;
        Month = 7;
        Year = 2024;

        _timer = new(1000);
        _timer.Elapsed += UpdateTime;
        _timer.Start();

        Debug.Log("TimeManager: Initialized");
        Debug.Log($"Minute: {Minute}, Hour: {Hour}, Day: {Day}, Month: {Month}, Year: {Year}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public void UpdateTime(object source, ElapsedEventArgs e)
    {
        Minute++;
        Debug.Log("TimeManager: OnMinutePassed invoke");
        Debug.Log($"{Year}:{Month}:{Day}:{Hour}:{Minute}");
        OnMinutePassed?.Invoke();

        if (Minute % 10 == 0)
        {
            Debug.Log("TimeManager: On10MinutePassed");
            On10MinutePassed?.Invoke();
        }

        if (Minute >= 60)
        {
            Minute = 0;
            Hour++;
            Debug.Log("TimeManager: OnHourPassed invoke");
            OnHourPassed?.Invoke();
        }



        if (Hour == 2 && !_on2AmInvoked)
        {
            Debug.Log("TimeManager: On2Am invoked");
            _on2AmInvoked = true;
            On2Am?.Invoke();
        }

        if (Hour >= 24)
        {
            Hour = 0;
            Day++;
            OnDayPassed?.Invoke();
            _on2AmInvoked = false;
        }

        if (Day >= DaysInMonth(Year, Month))
        {
            Day = 1;
            Month++;
            OnMonthPassed?.Invoke();
        }

        if (Month >= 12)
        {
            Month = 1;
            Year++;
            OnYearPassed?.Invoke();
        }

        if (Year >= 9999)
        {
            Year = 1;
            OnYearPassed?.Invoke();
        }

    }


    /// <summary>
    /// Return the number of days in a month.
    /// </summary>
    /// <param name="year">The year, used for calculating days of february.</param>
    /// <param name="month">The month for which to calculate days.</param>
    /// <returns></returns>
    public static int DaysInMonth(int year, int month)
    {
        return month switch
        {
            1 or 3 or 5 or 7 or 8 or 10 or 12 => 31,
            4 or 6 or 9 or 11 => 30,
            2 => DateTime.IsLeapYear(year) ? 29 : 28,
            _ => 0
        };
    }
}
