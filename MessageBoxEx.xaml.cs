﻿/* WPF 対応のメッセージボックス

 これは Window クラスから派生するので、基本的に Window と同じ使い方ができる
 たとえば、Background や Foreground プロパティを使って背景色や文字色を指定可能である
 あるいは Owner プロパティを設定してダイアログの表示位置を指定できる

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new emanual.Wpf.Utility.MessageBoxEx();

			//dlg.Message = "これはテスト用のメッセージです。";

			dlg.TextBlock.Inlines.Add("メッセージの表示は TextBlock クラスを使っていますので、テキストの中に、");
			dlg.TextBlock.Inlines.Add(new System.Windows.Documents.Bold(new System.Windows.Documents.Run("ボールド体")));
			dlg.TextBlock.Inlines.Add("や ");
			dlg.TextBlock.Inlines.Add(new System.Windows.Documents.Italic(new System.Windows.Documents.Run("Italic")));
			dlg.TextBlock.Inlines.Add(" 体を織り交ぜることができます。");
			dlg.TextBlock.Inlines.Add("部分的に");
			var span = new System.Windows.Documents.Span(new System.Windows.Documents.Run("文字色"));
			span.Foreground = new SolidColorBrush(Colors.Red);
			dlg.TextBlock.Inlines.Add(span);
			dlg.TextBlock.Inlines.Add("を変えることも可能です。");

			//dlg.Owner = this;
			//dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			dlg.Left = this.Left + 50;
			dlg.Top = this.Top + 50;
			//dlg.Background = Brushes.Wheat;
			dlg.Button = MessageBoxButton.YesNoCancel;
			dlg.Image = MessageBoxImage.Warning;
			dlg.Result = MessageBoxResult.No;
			dlg.ShowDialog();

			MessageBoxResult result = dlg.Result;
		}
*/
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using System.Diagnostics;

namespace emanual.Wpf.Utility
{
	public partial class MessageBoxEx : Window
	{
		private string FMessage = String.Empty; // メッセージ
		private MessageBoxButton FButton = MessageBoxButton.OK; // ボタン
		private MessageBoxImage FImage = MessageBoxImage.Information; // ダイアログに表示するアイコン
		private MessageBoxResult FResult = MessageBoxResult.None; // このプロパティを設定するとデフォルトのボタンとしてダイアログを開くときにフォーカスを与える

		public string Message { get { return FMessage; } set { FMessage = value; } }
		public MessageBoxButton Button { get { return FButton; } set { FButton = value; } }
		public MessageBoxImage Image { get { return FImage; } set { FImage = value; } }
		public MessageBoxResult Result { get { return FResult; } set { FResult = value; } }
		public TextBlock TextBlock { get { return PART_TextBlock; } }

		//---------------------------------------------------------------------------------------------
		public MessageBoxEx()
		{
			InitializeComponent();
			this.Foreground = Brushes.DarkBlue;
		}

		//---------------------------------------------------------------------------------------------
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (PART_TextBlock.Inlines.Count < 1)
				PART_TextBlock.Text = FMessage;

			this.SetupButtonImage();
			this.SetupButton();
		}

		//---------------------------------------------------------------------------------------------
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			this.DragMove();
		}

		//---------------------------------------------------------------------------------------------
		// Button プロパティに基づいてボタンを追加する
		// また、ダイアログを開いたときにフォーカスを与えるデフォルトのボタンを設定する
		private void SetupButton()
		{
			if (FButton == MessageBoxButton.OK)
			{
				this.CreateButton("btnOK", "O K");
			}
			else if (FButton == MessageBoxButton.OKCancel)
			{
				this.CreateButton("btnOK", "O K");
				this.CreateButton("btnCancel", "Cancel");
			}
			else if (FButton == MessageBoxButton.YesNo)
			{
				this.CreateButton("btnYes", "Yes");
				this.CreateButton("btnNo", "No");
			}
			else if (FButton == MessageBoxButton.YesNoCancel)
			{
				this.CreateButton("btnYes", "Yes");
				this.CreateButton("btnNo", "No");
				this.CreateButton("btnCancel", "Cancel");
			}

			// 右端のボタンのマージンを大きくするため
			var border = new Border();
			border.Width = 10;
			PART_StackPanel.Children.Add(border);
		}

		//---------------------------------------------------------------------------------------------
		private void button_Click(object sender, EventArgs e)
		{
			Button button = sender as Button;

			if (button.Name == "btnOK")
				FResult = MessageBoxResult.OK;
			else if (button.Name == "btnCancel")
				FResult = MessageBoxResult.Cancel;
			else if (button.Name == "btnYes")
				FResult = MessageBoxResult.Yes;
			else if (button.Name == "btnNo")
				FResult = MessageBoxResult.No;

			this.Close();
		}

		//---------------------------------------------------------------------------------------------
		// Button プロパティに基づいてボタンを作成する
		// 同時に Result プロパティに基づいてデフォルトのボタンにフォーカスを与える
		private void CreateButton(string name, string caption)
		{
			var button = new Button();
			button.Name = name;
			button.Width = 80;
			button.Content = caption;
			button.Margin = new Thickness(0, 30, 4, 0);

			button.Click += new RoutedEventHandler(button_Click);

			PART_StackPanel.Children.Add(button);

			if (FResult == MessageBoxResult.None)
			{
				if ((name == "btnOK") || (name == "btnYes"))
					Keyboard.Focus(button);
			}
			else if (FResult == MessageBoxResult.OK)
			{
				if (name == "btnOK")
					Keyboard.Focus(button);
			}
			else if (FResult == MessageBoxResult.Cancel)
			{
				if (name == "btnCancel")
					Keyboard.Focus(button);
			}
			else if (FResult == MessageBoxResult.Yes)
			{
				if (name == "btnYes")
					Keyboard.Focus(button);
			}
			else if (FResult == MessageBoxResult.No)
			{
				if (name == "btnNo")
					Keyboard.Focus(button);
			}
		}

		//---------------------------------------------------------------------------------------------
		// Image プロパティに基づいてアイコンを設定する
		private void SetupButtonImage()
		{
			StockIconId id = StockIconId.SIID_WARNING;

			switch (FImage)
			{
				case MessageBoxImage.Stop:
					// case MessageBoxImage.Hand:
					// case MessageBoxImage.Error:
					id = StockIconId.SIID_ERROR;
					break;
				case MessageBoxImage.Question:
					id = StockIconId.SIID_HELP;
					break;
				case MessageBoxImage.Exclamation:
					// case MessageBoxImage.Warning:
					id = StockIconId.SIID_WARNING;
					break;
			}

			PART_Image.Source = this.GetStockIconById(id, StockIconFlags.Large);
		}

		//*********************************************************************************************
		// ダイアログ用アイコンを Windows ストックアイコンから取得する

		[DllImport("User32.dll")]
		private static extern bool DestroyIcon(IntPtr hIcon);

		[DllImport("Shell32.dll")]
		private static extern IntPtr SHGetStockIconInfo(
			StockIconId siid,   // 取得するアイコンの IDを指定する StockIconId enum 型
			StockIconFlags uFlags, // 取得するアイコンの種類を指定する StockIconFlags enum 型
			ref StockIconInfo psii    //（戻り値）StockIconInfo 型
		);

		[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct StockIconInfo
		{
			public uint cbSize;        // 構造体のサイズ（バイト数）
			public IntPtr hIcon;       //（戻り値）アイコンへのハンドル
			public int iSysImageIndex; //（戻り値）システムアイコンキャッシュ内のアイコンのインデックス
			public int iIcon;          //（戻り値）取り出したアイコンのインデックス
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szPath;      //（戻り値）アイコンリソースを保持するファイル名 
		}

		[Flags]
		private enum StockIconFlags
		{
			Large = 0x000000000,       // 大きなアイコン
			Small = 0x000000001,       // 小さなアイコン
			ShellSize = 0x000000004,   // シェルのサイズのアイコン
			Handle = 0x000000100,      // 指定のアイコンのハンドル
			SystemIndex = 0x000004000, // システムのイメージリストのインデックス
			LinkOverlay = 0x000008000, // リンクを示すオーバーレイ付きのアイコン
			Selected = 0x000010000     // 選択状態のアイコン
		}

		private enum StockIconId
		{
			SIID_HELP = 23,
			SIID_WARNING = 78,
			SIID_INFO = 79,
			SIID_ERROR = 80
		}

		//---------------------------------------------------------------------------------------------
		// 指定のストックアイコンを取得する
		// id   : アイコンの種類を指定する StockIconId enum 型
		// flag : アイコンのサイズを指定する StockIconFlags enum 型
		private BitmapSource GetStockIconById(StockIconId id, StockIconFlags flag)
		{
			BitmapSource bitmapSource = null;
			StockIconFlags flags = StockIconFlags.Handle | flag;

			var info = new StockIconInfo();
			info.cbSize = (uint)Marshal.SizeOf(typeof(StockIconInfo));

			IntPtr result = SHGetStockIconInfo(id, flags, ref info);

			if (info.hIcon != IntPtr.Zero)
			{
				bitmapSource = Imaging.CreateBitmapSourceFromHIcon(info.hIcon, Int32Rect.Empty, null);
				DestroyIcon(info.hIcon);
			}

			return bitmapSource;
		}

		//---------------------------------------------------------------------------------------------
	} // end of MessageBoxEx class
} // end of emanual.Wpf.Utility namespace
