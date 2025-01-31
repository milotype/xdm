﻿using System;
using System.Net;
using Gtk;
using TraceLog;
using Translations;
using XDM.Core;
using XDM.Core.DataAccess;
using XDMApp = XDM.Core.Application;
using System.Linq;
using XDM.Core.BrowserMonitoring;

namespace XDM.GtkUI
{
    class Program
    {
        private const string DisableCachingName = @"TestSwitch.LocalAppContext.DisableCaching";
        private const string DontEnableSchUseStrongCryptoName = @"Switch.System.Net.DontEnableSchUseStrongCrypto";

        static void Main(string[] args)
        {
            Gtk.Application.Init();
            GLib.ExceptionManager.UnhandledException += ExceptionManager_UnhandledException;
            var globalStyleSheet = @"
                                    .large-font{ font-size: 16px; }
                                    .medium-font{ font-size: 14px; }
                                    ";

            var screen = Gdk.Screen.Default;
            var provider = new CssProvider();
            provider.LoadFromData(globalStyleSheet);
            Gtk.StyleContext.AddProviderForScreen(screen, provider, 800);
            //var screen = Gdk.Screen.Default;
            //var provider = new CssProvider();
            //provider.LoadFromData(@".dark 
            //                                    {
            //                                        color: gray;
            //                                        background: rgb(36,41,46);
            //                                    }

            //                                    treeview.view :selected 
            //                                    {
            //                                        background-color: rgb(10,106,182);
            //                                        color: white;
            //                                    }
            //.listt
            //{
            //font-family: Segoe UI;
            //}
            //                                    .dark2
            //                                    {
            //                                        color: gray;
            //                                        background: rgb(35,35,35);
            //                                        /*background: rgb(36,41,46);*/
            //                                    }
            //                                    .toolbar-border-dark
            //                                    {  
            //                                        border-bottom: 1px solid rgb(20,20,20);
            //                                    }
            //                                    .toolbar-border-light
            //                                    {  
            //                                        border-bottom: 2px solid rgb(240,240,240);
            //                                    }
            //                                  ");
            //Gtk.StyleContext.AddProviderForScreen(screen, provider, 800);

            ServicePointManager.ServerCertificateValidationCallback += (a, b, c, d) => true;
            ServicePointManager.DefaultConnectionLimit = 100;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;

            AppContext.SetSwitch(DisableCachingName, true);
            AppContext.SetSwitch(DontEnableSchUseStrongCryptoName, true);

            TextResource.Load(Config.Instance.Language);

            var debugMode = Environment.GetEnvironmentVariable("XDM_DEBUG_MODE");
            if (!string.IsNullOrEmpty(debugMode) && debugMode == "1")
            {
                var logFile = System.IO.Path.Combine(Config.DataDir, "log.txt");
                Log.InitFileBasedTrace(System.IO.Path.Combine(Config.DataDir, "log.txt"));
            }
            Log.Debug("Application_Startup");

            if (Config.Instance.AllowSystemDarkTheme)
            {
                Gtk.Settings.Default.ThemeName = "Adwaita";
                Gtk.Settings.Default.ApplicationPreferDarkTheme = true;
            }

            var core = new ApplicationCore();
            var app = new XDMApp();
            var win = new MainWindow();

            ApplicationContext.Configurer()
                .RegisterApplicationWindow(win)
                .RegisterApplication(app)
                .RegisterApplicationCore(core)
                .RegisterCapturedVideoTracker(new VideoTracker())
                .RegisterClipboardMonitor(new ClipboardMonitor())
                .RegisterLinkRefresher(new LinkRefresher())
                .RegisterPlatformUIService(new GtkPlatformUIService())
                .Configure();

            ArgsProcessor.Process(args, 0);

            //var t = new System.Threading.Thread(() =>
            //  {
            //      while (true)
            //      {
            //          System.Threading.Thread.Sleep(5000);
            //          Console.WriteLine("Trigger GC");
            //          GC.Collect();
            //      }
            //  });
            //t.Start();

            Gtk.Application.Run();

            //var app = new XDM.Core.XDM.Core();
            //var appWin = new AppWinPeer(app);
            //appWin.ShowAll();
            //Application.Run();


            //            Environment.SetEnvironmentVariable("PANGOCAIRO_BACKEND", "fc", EnvironmentVariableTarget.User);
            //            //Console.WriteLine(Environment.GetEnvironmentVariable("PANGOCAIRO_BACKEND"));
            //            //var arr = new string[] {  "PANGOCAIRO_BACKEND=fc" };
            //            Application.Init();// "app", ref arr);
            //            Gtk.Settings.Default.ThemeName = "Adwaita";
            //            Gtk.Settings.Default.ApplicationPreferDarkTheme = true;

            //            App app = new App();



            //            var appWin = new AppWin();
            //            Console.WriteLine("Starting show all");
            //            appWin.Show();
            //            Console.WriteLine("Finished show all");
            //            Application.Run();
        }

        private static void ExceptionManager_UnhandledException(GLib.UnhandledExceptionArgs args)
        {
            Log.Debug("GLib ExceptionManager_UnhandledException: " + args.ExceptionObject);
            args.ExitApplication = false;
        }
    }
}
