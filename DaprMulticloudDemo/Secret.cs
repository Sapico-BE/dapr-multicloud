using System;

namespace DaprMulticloudDemo
{
    public class Secret
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
