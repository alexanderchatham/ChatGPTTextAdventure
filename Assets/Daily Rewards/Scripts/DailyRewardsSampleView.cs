using System;
using TMPro;
using UnityEngine;

namespace Unity.Services.Samples.DailyRewards
{
    public class DailyRewardsSampleView : MonoBehaviour
    {
        public GameObject endedEventGameObject;

        public TextMeshProUGUI daysLeftText;
        public TextMeshProUGUI comeBackInText;
        public TextMeshProUGUI secondsPerDayText;

        public CalendarView calendar;
        public BonusDayView bonusDay;

        public void UpdateStatus(DailyRewardsEventManager eventManager)
        {
            calendar.UpdateStatus(eventManager);
            bonusDay.UpdateStatus(eventManager);

            UpdateTimers(eventManager);
        }

        public void UpdateTimers(DailyRewardsEventManager eventManager)
        {
            secondsPerDayText.text = $"1 day = {eventManager.secondsPerDay} sec";

            if (eventManager.daysRemaining <= 0)
            {
                endedEventGameObject.SetActive(true);
                daysLeftText.text = "Days Left: 0";
                comeBackInText.text = "Event Over";
            }
            else
            {
                endedEventGameObject.SetActive(false);
                daysLeftText.text = $"Days Left: {eventManager.daysRemaining}";
                var timeSpan = TimeSpan.FromSeconds(eventManager.secondsTillClaimable);
                if (eventManager.secondsTillClaimable > 0)
                {
                    comeBackInText.text = $"Come Back in: {timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}";
                }
                else
                {
                    comeBackInText.text = "Claim Now!";
                }
            }
        }
        

        public void SetAllDaysUnclaimable()
        {
            calendar.SetUnclaimable();
            bonusDay.SetUnclaimable();
        }
    }
}
