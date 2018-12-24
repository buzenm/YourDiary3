using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourDiary3.Models
{
    public class Group<S,T>
    {
        public S GroupTitle { get; set; }

        public ObservableCollection<T> GroupItems { get; set; }

    }
}
