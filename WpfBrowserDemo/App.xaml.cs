using CefSharp;
using CefSharp.Handler;
using CefSharp.Wpf.HwndHost;
using System;
using System.Configuration;
using System.Data;
using System.Windows;

namespace WpfBrowserDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // 初始化CefSharp
            InitCefSharp();
            base.OnStartup(e);
        }

        private void InitCefSharp()
        {
            try
            {
                if (Cef.IsInitialized == false || Cef.IsInitialized == null)
                {
                    var cachePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache");
                    var settings = new CefSettings()
                    {
                        LogFile = $"log\\cef.log",
                        LogSeverity = LogSeverity.Error,
                        IgnoreCertificateErrors = true,
                        CachePath = cachePath,
                        //UserDataPath = userDataPath,
                        Locale = $"locales\\zh-CN",
                        //WindowlessRenderingEnabled = false,
                        //PackLoadingDisabled = true, //影响页面效果
                        //MultiThreadedMessageLoop = true //让浏览器的消息循环在一个单独的线程中执行
                    };
                    settings.CefCommandLineArgs.Add("enable-media-stream");
                    //https://peter.sh/experiments/chromium-command-line-switches/#use-fake-ui-for-media-stream
                    settings.CefCommandLineArgs.Add("use-fake-ui-for-media-stream");
                    //For screen sharing add (see https://bitbucket.org/chromiumembedded/cef/issues/2582/allow-run-time-handling-of-media-access#comment-58677180)
                    settings.CefCommandLineArgs.Add("enable-usermedia-screen-capturing");
                    //setting.CefCommandLineArgs.Add("disable-webgl", "1");
                    //setting.CefCommandLineArgs.Add("proxy-auto-detect", "0");
                    //setting.CefCommandLineArgs.Add("no-proxy-server", "1");
                    //setting.SetOffScreenRenderingBestPerformanceArgs();
                    settings.EnablePrintPreview();
                    //CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
                    ////设置浏览器JS可使用的最大内存值为1GB（1024）
                    //setting.CefCommandLineArgs.Add("--js-flags", $"--max_old_space_size=1024");
                    var initialized = Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);

                    if (!initialized)
                    {
                        //var exitCode = Cef.GetExitCode();

                        //if (exitCode == CefSharp.Enums.ResultCode.NormalExitProcessNotified)
                        //{
                        //    MessageBox.Show($"Cef.Initialize failed with {exitCode}, another process is already using cache path {cachePath}");
                        //}
                        //else
                        //{
                        //    MessageBox.Show($"Cef.Initialize failed with {exitCode}, check the log file for more details.");
                        //}

                        App.Current.Shutdown();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"初始化CefSharp失败: {ex.Message}");
            }
        }
    }

}
