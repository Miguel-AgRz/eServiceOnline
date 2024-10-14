using System.Collections.Generic;
using Sanjel.BusinessEntities.RigJobs;

namespace eServiceOnline.Models.OperationBoard
{
    public class ScheduleBoardStyle
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public string FontColor { get; set; }

        public List<ScheduleBoardStyle> GetScheduleBoardStyles()
        {
            return new List<ScheduleBoardStyle>()
            {
                new ScheduleBoardStyle() {Id = (int) JobLifeStatus.None, Text ="None", Color = "#FFFFFF", FontColor = "#FFFFFF"},
                new ScheduleBoardStyle() {Id = (int) JobLifeStatus.Alerted, Text ="Alerted", Color = "#FFFFFF", FontColor = "#FFFFFF"},
                new ScheduleBoardStyle() {Id = (int) JobLifeStatus.Pending, Text ="Pending", Color = "#FFC0CB", FontColor = "#FFC0CB"},
                new ScheduleBoardStyle() {Id = (int) JobLifeStatus.Confirmed, Text ="Confirmed", Color = "#00FF00", FontColor = "#00FF00"},
                new ScheduleBoardStyle() {Id = (int) JobLifeStatus.Scheduled, Text ="Scheduled", Color = "#00FFFF", FontColor = "#00FFFF"},
                new ScheduleBoardStyle() {Id = (int) JobLifeStatus.Dispatched, Text ="Dispatched", Color = "#FFFF00", FontColor = "#FFFF00"},
                new ScheduleBoardStyle() {Id = (int) JobLifeStatus.InProgress, Text ="InProgress", Color = "#00FF00", FontColor = "#00FF00"},
                new ScheduleBoardStyle() {Id = (int) JobLifeStatus.Completed, Text ="Completed", Color = "#FFFFFF", FontColor = "#FFFFFF"},
                new ScheduleBoardStyle() {Id = (int) JobLifeStatus.Deleted, Text ="Deleted", Color = "#FFFFFF", FontColor = "#FFFFFF"},
                new ScheduleBoardStyle() {Id = (int) JobLifeStatus.Canceled, Text ="Canceled", Color = "#FFFFFF", FontColor = "#FFFFFF"}
            };
        }
    }
}