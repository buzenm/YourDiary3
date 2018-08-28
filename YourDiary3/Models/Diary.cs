using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourDiary3.Models
{
    public class Diary:INotifyPropertyChanged
    {
        private string date;
        public string Date
        {
            get
            { return date; }
            set
            {
                if (value != date)
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
                if (value != content)
                {
                    content = value;
                    OnPropertyChanged("Content");
                }
            }
        }
        private string weather;
        public string Weather
        {
            get
            {
                return weather;
            }
            set
            {
                if (weather != value)
                {
                    weather = value;
                    OnPropertyChanged("Weather");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
