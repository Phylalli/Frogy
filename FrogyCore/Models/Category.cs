using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrogyCore.Models
{
    [Serializable]
    public class Category
    {
        public string Name { get; set; }

        public List<string> SoftwaresID { get; set; } = new List<string>();

        public TimeSpan DailyQuota { get; set; } = new TimeSpan(0, 0, 0);

        public Category(string name)
        {
            Name = name;
        }

        public void AddSoftware(string id)
        {
            foreach(string softwareID in SoftwaresID)
            {
                if(softwareID == id)
                {
                    throw new Exception("Software " + id + " in " + Name + " already exist.");
                }
            }
            SoftwaresID.Add(id);
        }

        public void RemoveSoftware(string id)
        {
            foreach (string softwareID in SoftwaresID)
            {
                if (softwareID == id)
                {
                    SoftwaresID.Remove(softwareID);
                }
            }
            throw new Exception("Software " + id + " in " + Name + " not found.");
        }
    }

    [Serializable]
    public class CategoryInfo
    {
        public List<Category> Categories { get; set; } = new List<Category>();

        public Category GetCategoryDetail(string name)
        {
            foreach(Category category in Categories)
            {
                if(category.Name == name)
                {
                    return category;
                }
            }
            throw new Exception("Category " + name + " not found.");
        }

        public void AddCategory(string name)
        {
            foreach (Category category in Categories)
            {
                if (category.Name == name)
                {
                    throw new Exception("Category " + name + " already exist.");
                }
            }
            Categories.Add(new Category(name));
        }

        public void RemoveCategory(string name)
        {
            foreach (Category category in Categories)
            {
                if (category.Name == name)
                {
                    Categories.Remove(category);
                }
            }
            throw new Exception("Category " + name + " already exist.");
        }

        public void SetDailyQuota(string name, TimeSpan quota)
        {
            try
            {
                GetCategoryDetail(name).DailyQuota = quota;
            }
            catch
            {
                throw;
            }
        }

        public void AddSoftware(string name, string id)
        {
            try
            {
                GetCategoryDetail(name).AddSoftware(id);
            }
            catch
            {
                throw;
            }
        }

        public void RemoveSoftware(string name, string id)
        {
            try
            {
                GetCategoryDetail(name).RemoveSoftware(id);
            }
            catch
            {
                throw;
            }
        }
    }
}
