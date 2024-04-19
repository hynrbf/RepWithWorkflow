using System.Collections.Generic;
using System.Linq;
using Common.Entities;

namespace BackJobsWebScraper.Utilities
{
    public class HierarchyHelper
    {
        private readonly List<WebScrap> _items;

        public HierarchyHelper(List<WebScrap> items)
        {
            _items = items;
        }

        public List<WebScrap> GetAllChildren(string parentId)
        {
            return GetChildrenRecursive(parentId).ToList();
        }

        private IEnumerable<WebScrap> GetChildrenRecursive(string parentId)
        {
            var children = _items.Where(i => i.ParentId == parentId);

            foreach (var child in children)
            {
                yield return child;

                foreach (var grandChild in GetChildrenRecursive(child.Id))
                {
                    yield return grandChild;
                }
            }
        }
    }
}
