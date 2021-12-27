using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace ASHttpStarter
{
    class ASHttpStarter
    {
        class Options
        {
            [Option("TargetWindowTitlePrefix", Default = "AssistantSeika")]
            public string TargetWindowTitlePrefix { get; set; }

            [Option("TabControlAutomationId", Default = "tabControl")]
            public string TabControlAutomationId { get; set; }

            [Option("ProductTabName", Default = "使用製品")]
            public string ProductTabName { get; set; }

            [Option("ScanningButtonAutomationId", Default = "ButtonScan")]
            public string ScanningButtonAutomationId { get; set; }

            [Option("ScanningTimeoutMillis", Default = 120*1000)]
            public int ScanningTimeoutMillis { get; set; }

            [Option("SpeakerTabName", Default = "話者一覧")]
            public string SpeakerTabName { get; set; }

            [Option("HTTPFuncTabSelectIntervalMillis", Default = 0)]
            public int HTTPFuncTabSelectIntervalMillis { get; set; }
            
            [Option("HTTPFuncTabName", Default = "HTTP機能設定")]
            public string HTTPFuncTabName { get; set; }

            [Option("HTTPButtonAutomationId", Default = "ButtonHTTP")]
            public string HTTPButtonAutomationId { get; set; }

            [Option('v', "Verbose", Default = false)]
            public bool Verbose { get; set; }

        }

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions);
        }

        static void RunOptions(Options opts) 
        {
            var targetWindowTitlePrefix = opts.TargetWindowTitlePrefix;
            Process proc = Process.GetProcesses().First(p => p.MainWindowTitle.StartsWith(targetWindowTitlePrefix));
            if (opts.Verbose) Console.WriteLine("操作対象ウインドウ: {0}", proc.MainWindowTitle);

            var rootElement = AutomationElement.FromHandle(proc.MainWindowHandle);

            if (opts.Verbose) Console.WriteLine("タブコントロール を取得");
            var tabControlId = opts.TabControlAutomationId;
            var tabControl = rootElement.FindFirst(
                TreeScope.Element | TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, tabControlId)
            );

            if (opts.Verbose) Console.WriteLine("使用製品 タブに切り替え");
            var productTabName = opts.ProductTabName;
            var productTab = tabControl.FindFirst(
                TreeScope.Element | TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, productTabName)
            );
            var selectProductTab = productTab.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
            selectProductTab.Select();

            if (opts.Verbose) Console.WriteLine("製品スキャン ボタンをクリック");
            var scanningButtonId = opts.ScanningButtonAutomationId;
            var scanningButton = rootElement.FindFirst(
                TreeScope.Element | TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, scanningButtonId)
            );
            var invokeScanningButton = scanningButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            invokeScanningButton.Invoke();

            // TODO: エラーチェック
            if (opts.Verbose) Console.WriteLine("話者一覧 タブに切り替わるまで待機");
            var speakerTabName = opts.SpeakerTabName;
            var currentTabSelection = tabControl.GetCurrentPattern(SelectionPattern.Pattern) as SelectionPattern;
            var startTime = DateTime.Now;
            while (true)
            {
                var selectedTab = currentTabSelection.Current.GetSelection().First();
                var selectedTabName = selectedTab.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;

                if (selectedTabName == speakerTabName) break;

                var elapsed = DateTime.Now - startTime;
                if (TimeSpan.FromMilliseconds(opts.ScanningTimeoutMillis) <= elapsed)
                {
                    Console.Error.WriteLine("製品スキャンがタイムアウトしました。AssistantSeika側でエラーが発生している可能性があります");
                    Environment.Exit(1);
                }

                Thread.Sleep(100);
            }

            if (opts.Verbose) Console.WriteLine("製品スキャン完了。待機");
            Thread.Sleep(opts.HTTPFuncTabSelectIntervalMillis);

            if (opts.Verbose) Console.WriteLine("HTTP機能設定 タブに切り替え");
            var httpFuncTabName = opts.HTTPFuncTabName;
            var httpFuncTab = tabControl.FindFirst(
                TreeScope.Element | TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, httpFuncTabName)
            );
            var invokeHttpFuncTab = httpFuncTab.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
            invokeHttpFuncTab.Select();

            if (opts.Verbose) Console.WriteLine("起動する/再起動する ボタンをクリック");
            var httpButtonId = opts.HTTPButtonAutomationId;
            var httpButton = rootElement.FindFirst(
                TreeScope.Element | TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, httpButtonId)
            );
            var invokeHttpButton = httpButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            invokeHttpButton.Invoke();
        }
    }
}
