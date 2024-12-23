//Author: Dan Kabagambe
//Version: 1.0.0.0
//Copyright: Copyright © 2024 Dan Kabagambe


using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainManagement
{
    public class Domain
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Owner { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

    public class DomainManager
    {
        private readonly List<Domain> domains = new();

        // Method to add a domain to the list with a default duration of one year
        public void AddDomain(int id, string name, string owner, DateTime startDate)
        {
            domains.Add(new Domain
            {
                Id = id,
                Name = name,
                Owner = owner,
                StartDate = startDate,
                ExpiryDate = startDate.AddYears(1)
            });
        }

        // Method to get domains expiring within 30 days but not within 7 days
        public List<string> Get30DayDomains()
        {
            var sevenDayDomains = Get7DayDomains(); // Get domains within 7 days first
            return domains.Where(d => d.ExpiryDate > DateTime.Now.AddDays(7) && d.ExpiryDate <= DateTime.Now.AddDays(30))
                          .Select(d => d.Name)
                          .Except(sevenDayDomains) // Exclude those already in 7 day list
                          .ToList();
        }

        // Method to get domains expiring within 7 days
        public List<string> Get7DayDomains()
        {
            return domains.Where(d => d.ExpiryDate > DateTime.Now && d.ExpiryDate <= DateTime.Now.AddDays(7))
                          .Select(d => d.Name)
                          .ToList();
        }

        // Method to get expired domains
        public List<string> GetExpiredDomains()
        {
            return domains.Where(d => d.ExpiryDate <= DateTime.Now)
                          .Select(d => d.Name)
                          .ToList();
        }

        // Method to get domains in redemption period (2 days after expiry)
        public List<string> GetRedemptionDomains()
        {
            return domains.Where(d => d.ExpiryDate <= DateTime.Now.AddDays(-2) && d.ExpiryDate > DateTime.Now.AddDays(-4))
                          .Select(d => d.Name)
                          .ToList();
        }

        // Method to display all domains with their detailed status
        public void DisplayDomains()
        {
            Console.WriteLine("Domains Overview:");
            Console.WriteLine("ID\tName\t\tOwner\t\tStart Date\tExpiry Date\t30 Days\t7 Days\tExpired\tRedemption");
            Console.WriteLine("------------------------------------------------------------------------------------------");
            foreach (var domain in domains)
            {
                string thirtyDays = Get30DayDomains().Contains(domain.Name) ? "Yes" : "No";
                string sevenDays = Get7DayDomains().Contains(domain.Name) ? "Yes" : "No";
                string isExpired = GetExpiredDomains().Contains(domain.Name) ? "Yes" : "No";
                string inRedemption = GetRedemptionDomains().Contains(domain.Name) ? "Yes" : "No";

                Console.WriteLine($"{domain.Id}\t{domain.Name,-15}\t{domain.Owner,-15}\t{domain.StartDate:yyyy-MM-dd}\t{domain.ExpiryDate:yyyy-MM-dd}\t{thirtyDays}\t{sevenDays}\t{isExpired}\t{inRedemption}");
            }
        }

        // Search method for domains or owners with null check
        public void Search(string? searchTerm)
        {
            if (searchTerm == null)
            {
                Console.WriteLine("Search term cannot be null.");
                return;
            }

            var results = domains.Where(d =>
                d.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                d.Owner.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (results.Count == 0)
            {
                Console.WriteLine("No matching domains or owners found.");
            }
            else
            {
                Console.WriteLine("\nSearch Results:");
                Console.WriteLine("ID\tName\t\tOwner\t\tStart Date\tExpiry Date\t30 Days\t7 Days\tExpired\tRedemption");
                Console.WriteLine("------------------------------------------------------------------------------------------");
                foreach (var domain in results)
                {
                    string thirtyDays = Get30DayDomains().Contains(domain.Name) ? "Yes" : "No";
                    string sevenDays = Get7DayDomains().Contains(domain.Name) ? "Yes" : "No";
                    string isExpired = GetExpiredDomains().Contains(domain.Name) ? "Yes" : "No";
                    string inRedemption = GetRedemptionDomains().Contains(domain.Name) ? "Yes" : "No";

                    Console.WriteLine($"{domain.Id}\t{domain.Name,-15}\t{domain.Owner,-15}\t{domain.StartDate:yyyy-MM-dd}\t{domain.ExpiryDate:yyyy-MM-dd}\t{thirtyDays}\t{sevenDays}\t{isExpired}\t{inRedemption}");
                }
            }
        }
    }

    class Program
    {
        static void Main()
        {
            DomainManager manager = new DomainManager();

            // Adding sample domains with varied expiry dates to cover all categories
            DateTime now = DateTime.Now;

            // Active domain
            manager.AddDomain(1, "active.com", "Alex Active", now.AddYears(-1)); // Active

            // 30 days to expire
            manager.AddDomain(2, "thirty.com", "Tim Thirty", now.AddDays(-360)); // 30 Days Left

            // 7 days to expire
            manager.AddDomain(3, "seven.net", "Sam Seven", now.AddDays(-363)); // 7 Days Left

            // Just expired
            manager.AddDomain(4, "justexpired.org", "James Just", now.AddDays(-366)); // Expired

            // In redemption
            manager.AddDomain(5, "redemption.io", "Rachel Rays", now.AddDays(-368)); // In Redemption

            // Another Active domain
            manager.AddDomain(6, "newactive.site", "Ned New", now.AddDays(-300)); // Active

            // Another 30 days
            manager.AddDomain(7, "another30.com", "Alice Another", now.AddDays(-330)); // 30 Days Left

            // Another 7 days
            manager.AddDomain(8, "another7.net", "Bob Another", now.AddDays(-353)); // 7 Days Left

            // Another expired
            manager.AddDomain(9, "longexpired.org", "Larry Long", now.AddDays(-400)); // Expired

            // Another in redemption
            manager.AddDomain(10, "longredem.io", "Lila Long", now.AddDays(-370)); // In Redemption

            bool continueSearching = true;

            while (continueSearching)
            {
                manager.DisplayDomains();

                // Search functionality
                Console.Write("\nEnter a domain name or owner to search: ");
                string? searchTerm = Console.ReadLine();
                manager.Search(searchTerm);

                // Prompt user for next action
                Console.Write("\nWould you like to search again? (yes/no): ");
                string? userChoice = Console.ReadLine()?.ToLower();

                if (userChoice != "yes" && userChoice != "y")
                {
                    continueSearching = false;
                }
            }

            Console.WriteLine("Thank you for using the Domain Manager. Goodbye!");
        }
    }
}