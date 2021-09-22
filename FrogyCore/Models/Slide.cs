using FrogyCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrogyCore.Models
{
    [Serializable]
    public class Slide
    {
        public string ID { get; set; }

        public string WindowTitle { get; set; }

        public DeviceStatus DeviceStatus { get; set; }

        public TimeSpan StartTime { get; set; } = new TimeSpan(0, 0, 0);

        public TimeSpan EndTime { get; set; } = new TimeSpan(0, 0, 0);

        public Slide(string id, string windowTitle, DeviceStatus deviceStatus, TimeSpan startTime, TimeSpan endTime)
        {
            ID = id;
            WindowTitle = windowTitle;
            DeviceStatus = deviceStatus;
            StartTime = startTime;
            EndTime = endTime;
        }
    }

    [Serializable]
    public class Day
    {
        public List<Slide> Slides { get; set; } = new List<Slide>();

        //public List<Slide> DailySubsection { get; set; } = new List<Slide>();

        public void Refresh(TimeSpan timeSpan)
        {
            Slides.Last().EndTime = timeSpan;
        }

        public void Add(Slide slide)
        {
            Slides.Add(slide);
        }

        public Slide LatestSlide()
        {
            return Slides.Last();
        }
    }
}
