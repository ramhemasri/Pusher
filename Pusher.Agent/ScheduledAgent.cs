using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Pusher.KhanApi;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

namespace Pusher.Agent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            // TODO: see when last prompted & if time for next prompt

            var data = new MainViewModel();

            var nextLesson = data.Lessons.FirstOrDefault(l => !l.Completed);

            var toast = new ShellToast
            {
                Title = "Time for",
                Content = nextLesson.Name
            };
            toast.Show();

            var mainTile = ShellTile.ActiveTiles.First();

            mainTile.Update(new IconicTileData
            {
                WideContent1 = "Time for more",
                WideContent2 = "watch " + nextLesson.Name
            });

#if DEBUG
            ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
#endif

            NotifyComplete();
        }
    }
}