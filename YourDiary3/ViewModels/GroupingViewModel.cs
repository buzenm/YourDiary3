using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourDiary3.Models;

namespace YourDiary3.ViewModels
{
    public class GroupingViewModel
    {
        private ObservableCollection<Diary> Items;

        public ObservableCollection<Group<string,Diary>> Groups { get; set; }

        public GroupingViewModel(ObservableCollection<Diary> diaries)
        {
            Groups = new ObservableCollection<Group<string, Diary>>();
            Items = diaries;
            Grouping();
        }

        private void Grouping()
        {
            var group = from p in Items
                        group p by p.Date.Substring(0, 8) into g
                        orderby g.Key
                        select g;

            foreach (var item in group)
            {
                Groups.Add(new Group<string, Diary>
                {
                    GroupTitle = item.Key,
                    GroupItems = new ObservableCollection<Diary>(item.ToList())
                });
            }
                      
        }
    }
}
