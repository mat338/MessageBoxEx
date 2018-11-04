using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace MessageBoxTest
{
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

            //this.ShowInTaskbar = true;
            //this.AllowsTransparency = true;
            //this.WindowStyle = WindowStyle.None;//WindowStyle.ToolWindow;
            //this.Opacity = 0.0;

            MessageBoxResult result = MessageBox.Show("メッセージ", "キャプション", MessageBoxButton.OKCancel, MessageBoxImage.Information, MessageBoxResult.Cancel);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("メッセージ", "キャプション", MessageBoxButton.OKCancel, MessageBoxImage.Information, MessageBoxResult.Cancel);

			textBox.Text = result.ToString();
		}

		private void button2_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new emanual.Wpf.Utility.MessageBoxEx();
			dlg.Message = "これはテスト用のメッセージです。";
            //dlg.Width=350;

            //dlg.TextBlock.Inlines.Add("メッセージの表示は TextBlock クラスを使っていますので、テキストの中に、");
            //dlg.TextBlock.Inlines.Add(new System.Windows.Documents.Bold(new System.Windows.Documents.Run("ボールド体")));
            //dlg.TextBlock.Inlines.Add("や ");
            //dlg.TextBlock.Inlines.Add(new System.Windows.Documents.Italic(new System.Windows.Documents.Run("Italic")));
            //dlg.TextBlock.Inlines.Add(" 体を織り交ぜることができます。");
            //dlg.TextBlock.Inlines.Add("部分的に");
            //var span = new System.Windows.Documents.Span(new System.Windows.Documents.Run("文字色"));
            //span.Foreground = new SolidColorBrush(Colors.Red);
            //dlg.TextBlock.Inlines.Add(span);
            //dlg.TextBlock.Inlines.Add("を変えることも可能です。");
            dlg.Width = this.Width - 50;
            dlg.Height = this.Height - 50;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //dlg.Left = this.Left + 50;
            //dlg.Top = this.Top + 50;
            //dlg.Background = Brushes.Wheat;
            dlg.Button = MessageBoxButton.YesNoCancel;
			dlg.Image = MessageBoxImage.Warning;
			dlg.Result = MessageBoxResult.No;
			dlg.ShowDialog();

			MessageBoxResult result = dlg.Result;

			textBox.Text = result.ToString();
		}

		protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
		{
			Debug.Print(String.Format("e.Key={0}", e.Key));
			base.OnPreviewKeyDown(e);
		}
	}
}
