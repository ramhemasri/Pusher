﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Pusher.KhanApi.Models;

#if WINDOWS_PHONE
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using Newtonsoft.Json;
#endif

namespace Pusher.KhanApi
{
    public class MainViewModel : INotifyPropertyChanged
    {


        public MainViewModel()
        {
            this.Groups = new ObservableCollection<GroupItem>();
            this.Categories = new ObservableCollection<CategoryItem>();
            this.Lessons = new ObservableCollection<Lesson>();
           
        }

        #region Properties

        public ObservableCollection<GroupItem> Groups { get; private set; }

        public ObservableCollection<CategoryItem> Categories { get; private set; }

        public ObservableCollection<Lesson> Lessons { get; private set; }

        private const string FileName = "courseprogress.json";

        public bool Querying { get; set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>If <see cref="IsError" /> is true, then this will contain the fault error.
        /// You should not show this to the user, but communicate to devs.</summary>
        public string ErrorMessage { get; private set; }

        /// <summary>If this is true, there has been a fault and you should let the user know.</summary>
        public bool IsError { get; private set; }

        /// <summary>If the application encounters an error condition, call this method.</summary>
        /// <param name="message">The error details to send to the developers.</param>
        public void SetError(string message)
        {
            UIThread.MessageBox(message);
            this.IsError = true;
            this.ErrorMessage = message;

            this.NotifyPropertyChanged("ErrorMessage");
            this.NotifyPropertyChanged("IsError");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>call this any time you begin a server query</summary>
        /// <returns>a handle which should wrap the operation in a using statement.</returns>
        public IDisposable StartQuerying()
        {
            // TODO: implement refcounting
            Querying = true;
            UIThread.Invoke(() => NotifyPropertyChanged("Querying"));
            return new QueryingHandle(this);
        }

        /// <returns>true only the first time the user accesses the application.</returns>
        public void HasUserSeenIntro(Action<bool> action)
        {
            LocalStorage.HasUserSeenIntro(action);
        }

        public CategoryItem GetCategory(string categoryName)
        {
#if WINDOWS_PHONE
            var state = PhoneApplicationService.Current.State;
            
            object ocategory;
            if (state.TryGetValue(
                categoryName, 
                out ocategory)) return ocategory as CategoryItem;
#endif
            var category = this.Categories.Where(c => c.Name == categoryName).FirstOrDefault();
            category.LoadVideos();

#if WINDOWS_PHONE
            state[categoryName] = category;
#endif
            return category;
        }

        public void GetVideo(string category, string name, Action<VideoItem> result)
        {
            LocalStorage.GetVideo(category, name, vid =>
                {
                    if (vid != null)
                    {
                        result(vid);
                        return;
                    }

                    // didn't have the vid on disk, query the memory store.
                    vid = this.Categories
                        .Where(c => c.Name == category)
                        .SelectMany(c => c.Videos)
                        .SingleOrDefault(v => v.Name == name);

                    if (vid != null) LocalStorage.SaveVideo(vid);

                    result(vid);
                });
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            if (!this.IsDataLoaded)
            {
                this.IsDataLoaded = true;
                
                CategoryItem.Initialize(this.Groups, this.Categories);

                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var fileStream = store.OpenFile(FileName, FileMode.OpenOrCreate, FileAccess.Read))
                    {
                        using (var reader = new StreamReader(fileStream))
                        {
                            var data = reader.ReadToEnd();

                            if (string.IsNullOrWhiteSpace(data))
                            {
                                this.Lessons = new ObservableCollection<Lesson>();
                            }
                            else
                            {
                                this.Lessons = new ObservableCollection<Lesson>(JsonConvert.DeserializeObject<Lesson[]>(data));
                            }
                        }
                    }
                }
            }
        }

        public void SaveData()
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var fileStream = store.OpenFile(FileName, FileMode.Create, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(fileStream))
                    {
                        writer.Write(JsonConvert.SerializeObject(this.Lessons));
                        writer.Close();
                    }
                }
            }
        }


       

        #region Private Methods

        private void StopQuerying()
        {
            // TODO: implement refcounting
            this.Querying = false;
            NotifyPropertyChanged("Querying");
        }

        private class QueryingHandle : IDisposable
        {
            private MainViewModel model;

            public QueryingHandle(MainViewModel model)
            {
                this.model = model;
            }

            void IDisposable.Dispose()
            {
                //this.model.StopQuerying();
            }
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}