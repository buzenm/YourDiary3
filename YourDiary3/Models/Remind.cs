using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YourDiary3.Models
{
    public class Remind:INotifyPropertyChanged
    {
        private string date;
        public string Date
        {
            get
            {
                return date;
            }
            set
            {
                if (date != value)
                {
                    date = value;
                    OnPropertyChanged("Date");
                }
            }
        }

        private string content;
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                if (content != value)
                {
                    content = value;
                    OnPropertyChanged("Content");
                    //fixContent = Regex.Replace(content, "''", "'");
                }
            }
        }

        private string fixContent;
        public string FixContent
        {
            get
            {
                //return Regex.Replace(content, "''", "'");
                //if (fixContent != null)
                //    return Regex.Replace(fixContent, "''", "'");
                //else
                //    return content;
                return fixContent;

            }
            set
            {
                if(fixContent != value)
                {
                    fixContent = value;
                    OnPropertyChanged("FixContent");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            //fixContent = content;
        }
    }
}
