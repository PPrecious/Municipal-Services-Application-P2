using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    public static class IssueRepository
    {
        private static readonly List<Issue> issues = new List<Issue>();

        public static void AddIssue(Issue issue)
        {
            issues.Add(issue);
        }

        public static List<Issue> GetIssues()
        {
            return issues;
        }

        public static int GetIssueCount()
        {
            return issues.Count;
        }
    }
}