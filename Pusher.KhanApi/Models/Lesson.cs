using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pusher.KhanApi.Models
{
    public class Lesson : VideoItem , INotifyPropertyChanged
    {
       

        private bool _completed;

         [DataMember]
        public bool Completed
        {
            get { return _completed; }
            set
            {
                _completed = value;
                this.NotifyPropertyChanged("Completed");
            }
        }

         public event PropertyChangedEventHandler PropertyChanged;
         private void NotifyPropertyChanged(String propertyName)
         {
             PropertyChangedEventHandler handler = PropertyChanged;
             if (null != handler)
             {
                 handler(this, new PropertyChangedEventArgs(propertyName));
             }
         }
    }
}
