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
using System.Windows.Threading;

namespace TypingTest16
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        TimeSpan time; //remaining time
        TimeSpan testTime; //test time chosen by the user

        public MainWindow()
        {
            InitializeComponent();
            defaultValues();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick); //specifies the method that will handle the Tick event
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1); //specifies the time interval between two timer ticks
        }


        #region Private Methods
        private void defaultValues()
        {
            time = TimeSpan.FromMinutes(1); //Default testing time is 1 minute. time object will be changed during the test
            testTime = time;    //Default testing time is 1 minute. testTime will be kept intact during the test. Then it will be used to calculate the statistics
            labelTimer.Text = "00:01:00";
            comboBoxChooseTime.SelectedIndex = 0;
            textBoxInput.IsEnabled = false; //Input textbox must be disabled unless the test has started
            buttonStop.IsEnabled = false;   //Stop button must be disabled unless the test has started
            textBoxInput.Text = ""; //Deletes user-entered text from the input textbox after the test is finished
            comboBoxChooseExcerpt.IsEnabled = true;
            comboBoxChooseTime.IsEnabled = true;
        }
        #endregion

        #region Event Handlers
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            labelTimer.Text = time.ToString("c");   //"c" format specifier tells ToString method to use the standard hh:mm:ss form
            if (time.CompareTo(TimeSpan.FromSeconds(0)) == 0)
            {
                dispatcherTimer.Stop(); //if time object holds 0, stop the test
                defaultValues(); // test is over. Reinitialize the application
            }
            else
            {
                time = time.Add(TimeSpan.FromSeconds(-1)); //subtracts one second from time object
            }
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            buttonStart.IsEnabled = false;  //Starting new test in the middle of ongoing test makes no sense
            buttonStop.IsEnabled = true;
            textBoxInput.IsEnabled = true;
            comboBoxChooseExcerpt.IsEnabled = false; //disabled during the test
            comboBoxChooseTime.IsEnabled = false; //disabled during the test
            Keyboard.Focus(textBoxInput);   //Set focus to input textbox, allowing user to start typing immediately
            dispatcherTimer.Start(); //Start the countdown timer
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            defaultValues();
        }

        private void comboBoxChooseTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            time = TimeSpan.FromMinutes(comboBoxChooseTime.SelectedIndex + 1); //Notice that ComboBoxItem indexing starts from zero
            testTime = time;
            labelTimer.Text = time.ToString("c"); //"c" format specifier tells ToString method to use the standard hh:mm:ss form
        }
        #endregion
    }
}
