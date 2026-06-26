using System.Collections.Generic;
using System.Linq;
using CalculatorApp.Models;

namespace CalculatorApp.Services
{
    public class HistoryService
    {
        private readonly List<CalculationHistoryItem> _items = new();

        public void Add(CalculationHistoryItem item)
        {
            _items.Insert(0, item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public List<string> GetDisplayItems()
        {
            return _items
                .Select(item => item.DisplayText)
                .ToList();
        }
    }
}