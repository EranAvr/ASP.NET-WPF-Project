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
using ClientGUI.Models;

namespace ClientGUI
{
    /// <summary>
    /// Interaction logic for UserControlPresentQuestion.xaml
    /// </summary>
    public partial class UserControlPresentQuestion : UserControl
    {
        private Question currentQuestion;
        public UserControlPresentQuestion(Question q)
        {
            InitializeComponent();
            currentQuestion = q;
        }

        private void OnLoad(Object sender, RoutedEventArgs e)
        {
            // present question info:
            tbQtext.Text = currentQuestion.QText;
            listBoxPresentAnswers.ItemsSource = currentQuestion.Answers;

            if (currentQuestion.QPic == null)
            {
                imgBorder.Visibility = Visibility.Collapsed;
            }
            else {
                imgBorder.Visibility = Visibility.Visible;
                imgQpic.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(currentQuestion.QPic); ;
            }
        }

        private void ListBoxAnswersSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (listBoxPresentAnswers.SelectedItem is string item) {
                currentQuestion.CorrectAnswer = item;
                //MessageBox.Show($"Selected: {currentQuestion}");
            }
        }
    }
}
