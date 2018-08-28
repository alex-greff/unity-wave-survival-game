/*Alex Greff
19/01/2016
NotificationManager
Used to control notifications
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NotificationType {
    LARGE = 0,
    TOP = 1,
}

public class NotificationManager : MonoBehaviour {
    public static NotificationManager instance;

    public Notification large;
    public Notification top;

    //An array of all notifications
    private Notification[] notifications = new Notification[2];

    private const string TOP_NOTIFICATION_NAME = "Top Notification";
    private const string LARGE_NOTIFICATION_NAME = "Large Notification";

    //Getter and setters
    public static NotificationManager Instance {
        get {
            return instance;
        }
    }

    void Awake () {
        //Singleton behavior pattern
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        //Get references to notification gameobjects
        large = transform.Find(LARGE_NOTIFICATION_NAME).GetComponent<Notification>();
        top = transform.Find(TOP_NOTIFICATION_NAME).GetComponent<Notification>();
    }

    void Start () {
        //Set references to the array
        notifications[0] = large;
        notifications[1] = top;
    }

    //Show notification methods
    public void ShowNotification (NotificationType type, string message, float duration) {
        notifications[(int)type].StartAnimation(message, duration);
    }

    public void ShowNotification (NotificationType type, string message, float duration, Color color) {
        notifications[(int)type].StartAnimation(message, duration, color);
    }

    public void ShowNotification (NotificationType type, string message) {
        notifications[(int)type].StartAnimation(message);
    }

    public void CancelNotification (NotificationType type) {
        notifications[(int)type].EndAnimation();
    }

    //Cancels all the notification
    public void CancelAllNotifications () {
        foreach (StretchFade sf in notifications) {
            sf.EndAnimation();
        }
    }

    //Returns if the notification of type is running
    public bool isRunning (NotificationType type) {
        return notifications[(int)type].isRunning;
    }
}
