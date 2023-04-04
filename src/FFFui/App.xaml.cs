using AutoMapper;
using Fff.Crawler;
using FFFui.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml;

namespace FFFui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            TempFileCollector.Instance.Collect();
        }
    }
    
}
