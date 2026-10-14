
using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    public class EventRepository
    {
        private const int MaxSearchHistory = 20;

        private readonly SortedDictionary<DateTime, List<EventItem>> eventsByDate;
        private readonly Dictionary<int, EventItem> eventsById;
        private readonly HashSet<string> categories;
        private readonly Stack<string> searchHistory;
        private readonly Queue<EventItem> recentlyViewed;
        private readonly HashSet<int> viewedEventIds;

        private int nextEventId = 1;

        public EventRepository()
        {
            eventsByDate = new SortedDictionary<DateTime, List<EventItem>>();
            eventsById = new Dictionary<int, EventItem>();
            categories = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);
            searchHistory = new Stack<string>();
            recentlyViewed = new Queue<EventItem>();
            viewedEventIds = new HashSet<int>();

            SeedEvents();
        }

        private void SeedEvents()
        {
            AddEvent("Community Clean-Up Day",
                "Community", 3, "Central Park",
                "Join local volunteers to clean and improve public spaces.");

            AddEvent("Small Business Market",
                "Business", 5, "Town Square",
                "Explore products from local entrepreneurs and small businesses.");

            AddEvent("Public Health Awareness Workshop",
                "Health", 7, "Community Hall",
                "Learn about local health services and healthy living.");

            AddEvent("Youth Skills Development",
                "Education", 10, "Youth Centre",
                "A workshop focusing on career skills and personal development.");

            AddEvent("Local Music Festival",
                "Entertainment", 12, "Riverside Grounds",
                "Enjoy live performances from local musicians.");

            AddEvent("Recycling Information Session",
                "Environment", 14, "Municipal Hall",
                "Learn about recycling, waste reduction and collection services.");

            AddEvent("Community Sports Day",
                "Sports", 17, "Sports Grounds",
                "Participate in fun sports activities for the community.");

            AddEvent("Digital Skills Workshop",
                "Education", 20, "Public Library",
                "An introduction to useful digital and computer skills.");

            AddEvent("Local Arts Exhibition",
                "Arts", 24, "Cultural Centre",
                "Discover artwork created by artists in the local community.");

            AddEvent("Community Safety Meeting",
                "Safety", 28, "Municipal Hall",
                "Discuss community safety concerns and local initiatives.");
        }

        public void AddEvent(
            string title,
            string category,
            int daysFromToday,
            string location,
            string description)
        {
            EventItem item = new EventItem
            {
                EventId = nextEventId++,
                Title = title,
                Category = category,
                EventDate = DateTime.Today.AddDays(daysFromToday),
                Location = location,
                Description = description
            };

            eventsById.Add(item.EventId, item);

            DateTime eventDate = item.EventDate.Date;

            if (!eventsByDate.ContainsKey(eventDate))
            {
                eventsByDate[eventDate] = new List<EventItem>();
            }

            eventsByDate[eventDate].Add(item);

            if (!string.IsNullOrWhiteSpace(category))
            {
                categories.Add(category.Trim());
            }
        }

        public List<string> GetCategories()
        {
            List<string> result = categories
                .OrderBy(c => c)
                .ToList();

            result.Insert(0, "All Categories");
            return result;
        }

        public EventItem GetEventById(int eventId)
        {
            EventItem item;

            if (eventsById.TryGetValue(eventId, out item))
            {
                return item;
            }

            return null;
        }

        public List<EventItem> GetAllEvents()
        {
            return eventsByDate
                .SelectMany(dateGroup => dateGroup.Value)
                .OrderBy(item => item.EventDate)
                .ThenBy(item => item.Title)
                .ToList();
        }

        public List<EventItem> SearchEvents(
            string keyword,
            string category,
            bool useDate,
            DateTime startDate,
            DateTime endDate)
        {
            keyword = (keyword ?? string.Empty).Trim();
            category = (category ?? "All Categories").Trim();

            if (useDate && startDate.Date > endDate.Date)
            {
                throw new ArgumentException(
                    "The start date cannot be after the end date.");
            }

            // Record only meaningful searches.
            if (keyword.Length > 0 ||
                (category.Length > 0 &&
                 !category.Equals(
                     "All Categories",
                     StringComparison.OrdinalIgnoreCase)))
            {
                string searchRecord =
                    keyword + "|" +
                    (category.Equals(
                        "All Categories",
                        StringComparison.OrdinalIgnoreCase)
                        ? string.Empty
                        : category);

                RecordSearch(searchRecord);
            }

            IEnumerable<EventItem> results = GetAllEvents();

            if (keyword.Length > 0)
            {
                results = results.Where(item =>
                    ContainsText(item.Title, keyword) ||
                    ContainsText(item.Category, keyword) ||
                    ContainsText(item.Location, keyword) ||
                    ContainsText(item.Description, keyword));
            }

            if (category.Length > 0 &&
                !category.Equals(
                    "All Categories",
                    StringComparison.OrdinalIgnoreCase))
            {
                results = results.Where(item =>
                    string.Equals(
                        item.Category,
                        category,
                        StringComparison.OrdinalIgnoreCase));
            }

            if (useDate)
            {
                results = results.Where(item =>
                    item.EventDate.Date >= startDate.Date &&
                    item.EventDate.Date <= endDate.Date);
            }

            return results
                .OrderBy(item => item.EventDate)
                .ThenBy(item => item.Title)
                .ToList();
        }

        private bool ContainsText(string source, string search)
        {
            return !string.IsNullOrEmpty(source) &&
                   source.IndexOf(
                       search,
                       StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void RecordSearch(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return;
            }

            searchHistory.Push(search.Trim());

            if (searchHistory.Count > MaxSearchHistory)
            {
                string[] newestSearches = searchHistory
                    .Take(MaxSearchHistory)
                    .ToArray();

                searchHistory.Clear();

                for (int i = newestSearches.Length - 1; i >= 0; i--)
                {
                    searchHistory.Push(newestSearches[i]);
                }
            }
        }

        public List<EventItem> GetRecommendations()
        {
            List<EventItem> allEvents = GetAllEvents()
                .Where(item => item.EventDate.Date >= DateTime.Today)
                .ToList();

            if (allEvents.Count == 0)
            {
                return new List<EventItem>();
            }

            if (searchHistory.Count == 0)
            {
                return allEvents
                    .OrderBy(item => item.EventDate)
                    .Take(5)
                    .ToList();
            }

            EventPriorityQueue priorityQueue =
                new EventPriorityQueue();

            foreach (EventItem item in allEvents)
            {
                if (viewedEventIds.Contains(item.EventId))
                {
                    continue;
                }

                int score = CalculateRecommendationScore(item);

                int daysAway = Math.Max(
                    0,
                    (item.EventDate.Date - DateTime.Today).Days);

                int priority = 100000 - (score * 100) + daysAway;

                priorityQueue.Enqueue(item, priority);
            }

            List<EventItem> recommendations =
                new List<EventItem>();

            while (priorityQueue.Count > 0 &&
                   recommendations.Count < 5)
            {
                recommendations.Add(priorityQueue.Dequeue());
            }

            if (recommendations.Count == 0)
            {
                recommendations = allEvents
                    .OrderBy(item => item.EventDate)
                    .Take(5)
                    .ToList();
            }

            return recommendations;
        }

        private int CalculateRecommendationScore(EventItem item)
        {
            int score = 0;

            string[] itemWords = GetWords(
                item.Title + " " +
                item.Category + " " +
                item.Description + " " +
                item.Location);

            string[] searches = searchHistory.Take(10).ToArray();

            for (int i = 0; i < searches.Length; i++)
            {
                string[] parts = searches[i].Split('|');

                string keyword = parts.Length > 0
                    ? parts[0].Trim()
                    : string.Empty;

                string searchedCategory = parts.Length > 1
                    ? parts[1].Trim()
                    : string.Empty;

                int weight = Math.Max(1, 11 - i);

                if (!string.IsNullOrWhiteSpace(searchedCategory) &&
                    string.Equals(
                        item.Category,
                        searchedCategory,
                        StringComparison.OrdinalIgnoreCase))
                {
                    score += 10 * weight;
                }

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    if (ContainsText(item.Title, keyword))
                    {
                        score += 8 * weight;
                    }

                    if (ContainsText(item.Category, keyword))
                    {
                        score += 7 * weight;
                    }

                    if (ContainsText(item.Description, keyword))
                    {
                        score += 4 * weight;
                    }

                    if (ContainsText(item.Location, keyword))
                    {
                        score += 2 * weight;
                    }

                    string[] searchWords = GetWords(keyword);

                    foreach (string word in searchWords)
                    {
                        if (itemWords.Contains(
                            word,
                            StringComparer.OrdinalIgnoreCase))
                        {
                            score += 2 * weight;
                        }
                    }
                }
            }

            int daysAway = Math.Max(
                0,
                (item.EventDate.Date - DateTime.Today).Days);

            if (daysAway <= 7)
            {
                score += 3;
            }
            else if (daysAway <= 14)
            {
                score += 2;
            }

            return score;
        }

        private string[] GetWords(string text)
        {
            char[] separators =
            {
                ' ', '\t', '\r', '\n', ',', '.', ';', ':',
                '-', '_', '/', '\\', '(', ')', '[', ']',
                '!', '?', '\'', '"'
            };

            return (text ?? string.Empty)
                .ToLowerInvariant()
                .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                .Where(word => word.Length > 2)
                .Distinct()
                .ToArray();
        }

        public void RecordViewed(EventItem item)
        {
            if (item == null ||
                !eventsById.ContainsKey(item.EventId))
            {
                return;
            }

            if (viewedEventIds.Contains(item.EventId))
            {
                Queue<EventItem> updatedQueue =
                    new Queue<EventItem>();

                while (recentlyViewed.Count > 0)
                {
                    EventItem existing = recentlyViewed.Dequeue();

                    if (existing.EventId != item.EventId)
                    {
                        updatedQueue.Enqueue(existing);
                    }
                }

                while (updatedQueue.Count > 0)
                {
                    recentlyViewed.Enqueue(updatedQueue.Dequeue());
                }
            }

            recentlyViewed.Enqueue(item);
            viewedEventIds.Add(item.EventId);

            while (recentlyViewed.Count > 10)
            {
                EventItem removed = recentlyViewed.Dequeue();

            }
        }

        public List<EventItem> GetRecentlyViewed()
        {
            return recentlyViewed.ToList();
        }
    }

    public class EventPriorityQueue
    {
        private readonly SortedDictionary<int, Queue<EventItem>> items;

        public EventPriorityQueue()
        {
            items = new SortedDictionary<int, Queue<EventItem>>();
        }

        public int Count { get; private set; }

        public void Enqueue(EventItem item, int priority)
        {
            if (!items.ContainsKey(priority))
            {
                items[priority] = new Queue<EventItem>();
            }

            items[priority].Enqueue(item);
            Count++;
        }

        public EventItem Dequeue()
        {
            if (Count == 0)
            {
                return null;
            }

            int firstPriority = items.Keys.First();
            Queue<EventItem> queue = items[firstPriority];

            EventItem item = queue.Dequeue();

            if (queue.Count == 0)
            {
                items.Remove(firstPriority);
            }

            Count--;
            return item;
        }
    }
}
