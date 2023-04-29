using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Microsoft.Win32;

namespace ClientGUI
{
    /// <summary>
    /// Interaction logic for NewTestMenu.xaml
    /// </summary>
    public partial class UserControlEditInfo : UserControl
    {
        Exam currentExam;
        ExamBaseInfo info { get; }
        private AuthResponse userAuth;
        //string examID;
        //string questionID;

        public UserControlEditInfo(Exam e, AuthResponse auth)
        {
            InitializeComponent();
            currentExam = e;
            info = e.Info;
            userAuth = auth;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            tbInfoName.Text = info.Name;
            tbInfoAuthor.Text = info.Author;
            tbInfoAuthorID.Text = info.AuthorID;
            tbInfoDate.Text = info.Date == null ? "" : info.Date.ToString();
            tbInfoStart.Text = info.TimeStart==null ? "" : info.TimeStart.ToString();
            tbInfoDuration.Text = info.TimeDuration == null ? "" : info.TimeDuration.ToString();
            cbInfoRndQuestions.IsChecked = info.Randomize;
        }

        // Repository Buttons:
        private void buttonSaveOnClick(object sender, RoutedEventArgs e)
        {
            // Validation and checks-
            if (tbInfoName.Text == string.Empty) {
                MessageBox.Show("Name field is empty.");
                return;
            }
            Regex timeRg = new Regex("^([0-1][0-9]|2[0-3]):[0-5][0-9]$");
            if (!timeRg.IsMatch(tbInfoStart.Text)) {
                MessageBox.Show("Time must follow 'HH:mm' pattern");
                return;
            }
            if (tbInfoDate.SelectedDate == null) {
                MessageBox.Show("Date not selected");
                return;
            }
            if (! new Regex("^[1-9]$").Match(tbInfoDuration.Text).Success) {
                MessageBox.Show("Exam duration should be between 1-9");
                return;
            }

            // Updating current exam info-
            info.Name = tbInfoName.Text;
            info.Author = tbInfoAuthor.Text;
            info.AuthorID = tbInfoAuthorID.Text;
            info.TimeStart = tbInfoStart.Text;
            info.Date = new DateTime(
                tbInfoDate.SelectedDate.Value.Date.Year,
                tbInfoDate.SelectedDate.Value.Date.Month,
                tbInfoDate.SelectedDate.Value.Date.Day,
                TimeOnly.Parse(info.TimeStart).Hour,
                TimeOnly.Parse(info.TimeStart).Minute,
                TimeOnly.Parse(info.TimeStart).Second,
                DateTimeKind.Utc);
            
            
            info.TimeDuration = int.Parse(tbInfoDuration.Text);
            info.Randomize = (bool)cbInfoRndQuestions.IsChecked;

            MessageBox.Show("Saved on temporary Repo");
        }
        private void buttonClearOnClick(object sender, RoutedEventArgs e)
        {
            info.Clear();
            tbInfoName.Text = string.Empty;
            tbInfoAuthor.Text = string.Empty;
            tbInfoDate.Text = string.Empty;
            tbInfoStart.Text = string.Empty;
            tbInfoDuration.Text = string.Empty;
            cbInfoRndQuestions.IsChecked = false;
        }

    }
}
