using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
using ClientGUI.Models;

namespace ClientGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string baseAddress = "https://localhost:7252/";
        private AuthResponse? currentAuthResponse;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClearOnClick(Object sender, RoutedEventArgs e)
        {
            radioStu.IsChecked = true;
            emailInput.Text = "";
            passInput.Text = "";
        }

        private async void LoginOnClick(Object sender, RoutedEventArgs e)
        {
            // We checked 'Student' by default, to avoid null.
            string? accountType = radioStu.IsChecked == true ?
                                    ConstantsRepository.userTypeStudent :
                                    ConstantsRepository.userTypeTeacher;
            string email = emailInput.Text.ToString();
            string pass = passInput.Text.ToString();
            // check for empty fields-
            if (email == string.Empty || pass == string.Empty)
                return;
            //MessageBox.Show(inputUser + "\n" + inputPass + "\n" + accountType);
            if (await ValidateAccount(email, pass))
            {
                // check for correct user type-
                if (accountType != currentAuthResponse.userType)
                {
                    MessageBox.Show("Account type is wrong!");
                }
                else
                {
                    // if all is good-
                    MessageBox.Show($"Welcome {currentAuthResponse.userName}");
                    switch (accountType)
                    {
                        case "Student":
                            StudentWindow stuWin = new StudentWindow(currentAuthResponse);
                            stuWin.ShowDialog();
                            break;
                        case "Teacher":
                            TeacherWindow teachWin = new TeacherWindow(currentAuthResponse);
                            teachWin.ShowDialog();
                            break;
                    }
                }
                // Logout user-
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(ConstantsRepository.clientConnectBaseAddress);
                HttpResponseMessage response = await httpClient.DeleteAsync($"api/Users/Sessions/Logout/{currentAuthResponse.sessionId}");
            }
            else
            {
                // In case of invalid input-
                MessageBox.Show(currentAuthResponse.message);
            }
            
        }

        private async Task<bool> ValidateAccount(string userEmail, string userPassword)
        {
            // check if session for current user already exists-
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            StringContent reqBody = new StringContent(JsonSerializer.Serialize(
                new SessionObj(email: userEmail, pass: userPassword)), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync("api/Users/Sessions/Login", reqBody);

            // Save current authentication response-
            string jsonResBody = await response.Content.ReadAsStringAsync();
            currentAuthResponse = JsonSerializer.Deserialize<AuthResponse>(jsonResBody);
            
            return currentAuthResponse.isAuth;
        }
    }
}
