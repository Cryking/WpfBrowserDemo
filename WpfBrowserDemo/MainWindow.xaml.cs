using CefSharp;
using CefSharp.DevTools.SystemInfo;
using CefSharp.Wpf.HwndHost;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Win32;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfBrowserDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    /// <summary>
    /// COM互操作接口定义
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
    interface IWebBrowser2
    {
        [DispId(550)]
        void Silent([In, MarshalAs(UnmanagedType.Bool)] bool bSilent);
    }

    /// <summary>
    /// 处理证书验证和导航错误的接口
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("0002DF05-0000-0000-C000-000000000046")]
    public interface IWebBrowserEvents2
    {
        // 各种事件处理方法
    }

    
    /// <summary>
    /// WebBrowser控件的扩展方法
    /// </summary>
    public static class WebBrowserExtensions
    {
        public static void SuppressScriptErrors(this WebBrowser webBrowser, bool hide)
        {
            try
            {
                // 方法1：使用反射访问WebBrowser控件的内部属性
                FieldInfo field = typeof(WebBrowser).GetField(
                    "_axIWebBrowser2",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (field != null)
                {
                    object axIWebBrowser2 = field.GetValue(webBrowser);
                    if (axIWebBrowser2 != null)
                    {
                        axIWebBrowser2.GetType().InvokeMember(
                            "Silent",
                            BindingFlags.SetProperty,
                            null,
                            axIWebBrowser2,
                            new object[] { hide });
                        return; // 成功则返回
                    }
                }

                // 方法2：使用COM接口直接设置
                var comObj = webBrowser.GetType().GetField(
                    "_axIWebBrowser2",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                if (comObj != null)
                {
                    var browser = comObj.GetValue(webBrowser) as IWebBrowser2;
                    if (browser != null)
                    {
                        browser.Silent(true);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"抑制脚本错误时出错: {ex.Message}");
            }
        }
    }
    public partial class MainWindow : Window
    {
        private bool isWebView2Initialized = false;
        const string Url = "https://staging-uc-fe.noprod.hnlshm.com/login";

        public MainWindow()
        {
            InitializeComponent();
        }

        

        private async void InitializeWebView2()
        {
            try
            {
                await WebView2.EnsureCoreWebView2Async(null);

                //// 处理新窗口请求，在当前控件中打开，不创建新窗口
                //WebView2.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;

                isWebView2Initialized = true;
                Console.WriteLine($"isWebView2Initialized:{isWebView2Initialized}");

                //// 窗口加载时自动导航到指定网址
                //NavigateToUrl("https://staging-uc-fe.noprod.hnlshm.com/login");
                WebView2.CoreWebView2.Navigate(Url);


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"初始化WebView2失败: {ex.Message}");
            }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            // 点击导航按钮时导航到文本框中的URL
            string url = UrlTextBox.Text;
            if (!string.IsNullOrWhiteSpace(url))
            {
                if (WebView2Radio.IsChecked.Value && isWebView2Initialized)
                {
                    WebView2.CoreWebView2.Navigate(url);
                }
                else if (CefSharpRadio.IsChecked.Value)
                {
                    CefBrowser.Load(url);
                    //CefBrowser.PrintToPdfAsync("d:\\123.pdf");
                }
                else
                {
                    NavigateToUrl(url);
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (WebBrowserRadio.IsChecked == true && WebBrowser != null)
                //{
                //    // 刷新WebBrowser页面
                //    WebBrowser.Refresh();
                //}
                if (WebView2Radio.IsChecked == true && isWebView2Initialized && WebView2 != null)
                {
                    // 刷新WebView2页面
                    WebView2.Reload();
                }
                else if (CefSharpRadio.IsChecked == true)
                {
                    // 刷新CefSharp页面
                    CefBrowser.GetBrowser().Reload();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"刷新时出错: {ex.Message}");
                // 如果标准刷新失败，尝试重新导航
                string currentUrl = UrlTextBox?.Text ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(currentUrl))
                {
                    NavigateToUrl(currentUrl);
                }
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (WebBrowserRadio.IsChecked == true && WebBrowser != null)
                //{
                //    // 停止WebBrowser导航 - 通过反射调用内部Stop方法
                //    WebBrowser.InvokeScript("stop");
                //}
                if (WebView2Radio.IsChecked == true && isWebView2Initialized && WebView2 != null && WebView2.CoreWebView2 != null)
                {
                    // 停止WebView2导航
                    WebView2.CoreWebView2.Stop();
                }
                else if (CefSharpRadio.IsChecked == true)
                {
                    // 停止CefSharp导航
                    CefBrowser.GetBrowser().StopLoad();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"停止导航时出错: {ex.Message}");
            }
        }



        //private void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        //{
        //    // 导航完成后更新URL文本框
        //    if (WebBrowserRadio.IsChecked == true && UrlTextBox != null)
        //    {
        //        UrlTextBox.Text = e.Uri?.ToString() ?? string.Empty;
        //    }

        //    //// 再次确保脚本错误被禁用
        //    //if (WebBrowser != null)
        //    //{
        //    //    WebBrowser.SuppressScriptErrors(true);
        //    //}
        //}

        //private void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        //{
        //    // 禁用脚本错误对话框
        //    if (WebBrowser != null)
        //    {
        //        WebBrowser.SuppressScriptErrors(true);
        //    }
        //}

        //private void WebBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        //{
        //    // 在导航过程中确保脚本错误被禁用
        //    if (WebBrowser != null)
        //    {
        //        WebBrowser.SuppressScriptErrors(true);
        //    }
        //}

        private void WebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                Console.WriteLine($"导航失败，错误: {e.WebErrorStatus}");
            }
            // 导航完成后更新URL文本框
            if (WebView2Radio.IsChecked == true && UrlTextBox != null)
            {
                UrlTextBox.Text = WebView2.Source?.ToString() ?? string.Empty;
            }
        }

        private void CefSharpBrowser_LoadingStateChanged(object sender, EventArgs e)
        {
            // 导航完成后更新URL文本框
            this.Dispatcher.Invoke(() =>
            {
                if (CefSharpRadio.IsChecked == true && UrlTextBox != null)
                {
                    UrlTextBox.Text = CefBrowser.Address;
                }
            });
        }

        private void WebBrowserRadio_Checked(object sender, RoutedEventArgs e)
        {
            //// 切换到WebBrowser
            //if (WebBrowser != null)
            //{
            //    WebBrowser.Visibility = Visibility.Visible;
            //}

            if (WebView2 != null)
            {
                WebView2.Visibility = Visibility.Collapsed;
            }

            if (CefBrowser != null)
            {
                CefBrowser.Visibility = Visibility.Collapsed;
            }

            // 如果当前URL不为空，重新导航
            if (!string.IsNullOrWhiteSpace(UrlTextBox?.Text ?? string.Empty))
            {
                NavigateToUrl(UrlTextBox.Text);
            }
        }

        private void WebView2Radio_Checked(object sender, RoutedEventArgs e)
        {
            //// 切换到WebView2
            //if (WebBrowser != null)
            //{
            //    WebBrowser.Visibility = Visibility.Collapsed;
            //}

            if (CefBrowser != null)
            {
                CefBrowser.Visibility = Visibility.Collapsed;
            }

            if (WebView2 != null)
            {
                WebView2.Visibility = Visibility.Visible;
            }
            if (!isWebView2Initialized)
            {
                InitializeWebView2();
            }
            // 如果WebView2已初始化，重新导航到当前URL
            if (isWebView2Initialized && !string.IsNullOrWhiteSpace(UrlTextBox?.Text ?? string.Empty) && WebView2?.CoreWebView2 != null)
            {
                WebView2.CoreWebView2.Navigate(UrlTextBox.Text);
            }
        }

        private void CefSharpRadio_Checked(object sender, RoutedEventArgs e)
        {
            //if (WebBrowser != null)
            //{
            //    WebBrowser.Visibility = Visibility.Collapsed;
            //}

            if (WebView2 != null)
            {
                WebView2.Visibility = Visibility.Collapsed;
            }

            if (CefBrowser != null)
            {
                CefBrowser.Visibility = Visibility.Visible;
                
                // 绑定加载状态变化事件
                CefBrowser.LoadingStateChanged += CefSharpBrowser_LoadingStateChanged;
                
                // 重新导航到当前URL
                if (!string.IsNullOrWhiteSpace(UrlTextBox?.Text ?? string.Empty))
                {
                    CefBrowser.Load(UrlTextBox.Text);
                }
            }
        }

        private void NavigateToUrl(string url)
        {
            // 确保URL格式正确
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
            }

            // 根据当前选择的引擎进行导航
            //if (WebBrowserRadio?.IsChecked == true)
            //{
            //}
            if (WebView2Radio?.IsChecked == true)
            {
                // 使用WebView2导航
                if (isWebView2Initialized && WebView2 != null && WebView2.CoreWebView2 != null)
                {
                    try
                    {
                        WebView2.CoreWebView2.Navigate(url);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"WebView2导航时出错: {ex.Message}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("WebView2尚未初始化，无法导航");
                }
            }
            else if (CefSharpRadio?.IsChecked == true)
            {
                // 使用CefSharp导航
                if (CefBrowser != null)
                {
                    try
                    {
                        CefBrowser.Load(url);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"CefSharp导航时出错: {ex.Message}");
                    }
                }
            }
        }

        //private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        //{
        //    // 在当前WebView2控件中打开新窗口请求的URL，而不是打开新窗口
        //    try
        //    {
        //        // 获取新窗口的URL
        //        string newUrl = e.Uri;

        //        if (!string.IsNullOrEmpty(newUrl))
        //        {
        //            // 在当前WebView2中导航到新URL
        //            WebView2.CoreWebView2.Navigate(newUrl);

        //            // 设置Handled为true，表示我们已经处理了这个新窗口请求
        //            e.Handled = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"处理新窗口请求时出错: {ex.Message}");
        //    }
        //}



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 在窗口关闭时正确关闭CefSharp
            try
            {
                //if (Cef.IsInitialized)
                {
                    Cef.Shutdown();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"关闭CefSharp时出错: {ex.Message}");
            }
        }

        //private void SetWebBrowserCompatibility()
        //{
        //    try
        //    {
        //        // 获取当前应用程序的名称和版本
        //        var appName = System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        //        // 打开注册表项
        //        using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true))
        //        {
        //            // 设置使用最新版本的IE渲染引擎
        //            // 11001对应IE 11边缘模式
        //            // 如果不存在，则创建该项
        //            if (key != null)
        //            {
        //                key.SetValue(appName, 11001, RegistryValueKind.DWord);
        //            }
        //        }

        //        // 禁用脚本错误对话框
        //        using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_SCRIPT_ERROR_DIALOG", true))
        //        {
        //            if (key != null)
        //            {
        //                key.SetValue(appName, 0, RegistryValueKind.DWord);
        //            }
        //        }

        //        // 处理HTTPS/安全证书问题
        //        // 禁用证书错误检查
        //        using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_ERROR_PAGE_BYPASS_ZONE_CHECK_FOR_HTTPS_URL", true))
        //        {
        //            if (key != null)
        //            {
        //                key.SetValue(appName, 1, RegistryValueKind.DWord);
        //            }
        //        }

        //        // 处理重定向问题
        //        using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_HTTP_USERNAME_PASSWORD_DISABLE", true))
        //        {
        //            if (key != null)
        //            {
        //                key.SetValue(appName, 0, RegistryValueKind.DWord);
        //            }
        //        }

        //        // 处理CORS问题
        //        using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_CROSS_DOMAIN_PINGS_RESTRICTIONS", true))
        //        {
        //            if (key != null)
        //            {
        //                key.SetValue(appName, 0, RegistryValueKind.DWord);
        //            }
        //        }

        //        // 添加对测试环境的额外安全设置
        //        // 允许混合内容
        //        using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BLOCK_LMZ_SCRIPT", true))
        //        {
        //            if (key != null)
        //            {
        //                key.SetValue(appName, 0, RegistryValueKind.DWord);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // 记录错误但不影响程序运行
        //        System.Diagnostics.Debug.WriteLine($"设置浏览器兼容性时出错: {ex.Message}");
        //    }
        //}
    }
}