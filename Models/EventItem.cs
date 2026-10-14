using System;

namespace MunicipalServicesApp.Models
{
    public class EventItem
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return EventDate.ToString("dd MMM yyyy") + " - " + Title;
        }
    }
}