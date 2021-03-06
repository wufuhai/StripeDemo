using System;

namespace QNE.Models.ViewModel
{
    public class CompanyListViewModel
    {
        public Guid TenantId { get; set; }
        public string CompanyName { get; set; }
        public string RegNo { get; set; }
        public string PictureUrl { get; set; }

        public string Owner { get; set; }

        public string ProductKey { get; set; }

        public string OwnerId { get; set; }

        public string Country { get; set; }

        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }

        public string AvatarText => GetTwoCharactersName(CompanyName);

        public SubscriptionInfoViewModel Subscription { get; set; } = new SubscriptionInfoViewModel();

        private string GetTwoCharactersName(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            if (input.Length == 1) return input.ToUpper();

            var text = "";
            var words = input.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 2)
            {
                text = new string(new[] { words[0][0], words[1][0] });
            }
            else if (text.Length < 2)
            {
                text = input.Substring(0, 2);
            }

            text = text.ToUpper();
            return text;
        }

    }
}
