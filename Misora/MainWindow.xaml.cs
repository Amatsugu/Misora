using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Misora
{
	public partial class MainWindow : Window
	{
		private bool _isLoggedIn;

		public MainWindow()
		{
			InitializeComponent();
			string token = MisoraCore.LoadToken();
			if (!string.IsNullOrWhiteSpace(token))
			{
				tokenBox.Text = token;
				LoginButton_Click(null, null);
			}

		}

		private async void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			if (_isLoggedIn)
			{
				await MisoraCore.Logout();
				tokenBox.IsEnabled = true;
				LoginButton.Content = "Login";
			} else
			{
				if (_isLoggedIn = await MisoraCore.Login(tokenBox.Text))
				{
					tokenBox.IsEnabled = false;
					LoginButton.Content = "Logout";
				} else
				{
					MessageBox.Show("Login Failed", "Error");
				}
			}
		}
	}
}
