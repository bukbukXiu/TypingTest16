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
            textBoxInput.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, disablePaste)); //disabling Paste for input TextBox
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
            comboBoxChooseExcerpt.SelectedIndex = -1;   //No text fragment is selected by default
            buttonStart.IsEnabled = false;  //Start button is disabled unless user selects a text fragment
            textBoxSource.Text = "Choose one of the excerpts below"; //Message for the user
        }

        private void check()
        {
            ResultWindow resultWindow = new ResultWindow(); //create an instance of ResultWindow
            string[] sourceTextData = textBoxSource.Text.Split(' ');    //an array of words in the SOURCE text box
            string[] userTextData = textBoxInput.Text.Split(' ');   //an array of user-entered words
            int typos = 0;  //this variable will store the quantity of mistyped words
            int extraTypos = Math.Max(sourceTextData.Length, userTextData.Length) - Math.Min(sourceTextData.Length, userTextData.Length);
            int minWords = Math.Min(sourceTextData.Length, userTextData.Length);
            for (int i = 0; i < minWords; i++)
            {
                if (userTextData[i] != sourceTextData[i])
                {
                    typos++;    //current word is wrong.
                    AppendText(resultWindow.richTextBoxDiff, userTextData[i] + ' ', "red"); // if the processed word is differen from the one in the source text, red colour is applied
                }
                else
                {
                    AppendText(resultWindow.richTextBoxDiff, userTextData[i] + ' ', "green");   // if the processed word is right, green colour is applied
                }
            }
            int totalTypos = typos + extraTypos;
            double mistakeRate = Math.Round((double)(totalTypos / (double)sourceTextData.Length) * 100.0, 2); //Math.Round is used to show only two fractional digits 
            resultWindow.labelMistakeRate.Content = mistakeRate.ToString() + "%";
            TimeSpan timeSpent = testTime - time;   //timeSpent will store the time spent during the test
            double typingSpeed = Math.Round((double)typedWords() / timeSpent.TotalMinutes, 2);
            resultWindow.labelSpeed.Content = typingSpeed.ToString() + " words/min";
            resultWindow.labelTimeSpent.Content = timeSpent;
            resultWindow.Show();    //show the window to the user
        }

        private void AppendText(RichTextBox box, string text, string color)
        {
            BrushConverter bc = new BrushConverter();
            TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
            tr.Text = text;
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertFromString(color));
        }

        private int typedWords()
        {
            if (textBoxInput.Text == "") return 0;
            else return textBoxInput.Text.Split(' ').Length;
        }

        private void easyCheck()
        {
            TimeSpan timeSpent = testTime - time;   //timeSpent will store the time spent during the test
            string[] userTextData = textBoxInput.Text.Split(' ');   //an array of words
            ResultWindow resultWindow = new ResultWindow();
            resultWindow.labelMistakeRate.Content = "0%"; //text is identical so there's no mistakes
            double typingSpeed = Math.Round(typedWords() / timeSpent.TotalMinutes, 2);  //Math.Round is used to show only two fractional digits 
            resultWindow.labelSpeed.Content = typingSpeed.ToString() + " words/min";
            resultWindow.labelTimeSpent.Content = timeSpent;
            AppendText(resultWindow.richTextBoxDiff, textBoxInput.Text, "green"); //AppendText is used to make the text green
            resultWindow.Show();
        }

        private void disablePaste(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

        #region Event Handlers
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            labelTimer.Text = time.ToString("c");   //"c" format specifier tells ToString method to use the standard hh:mm:ss form
            if (time.CompareTo(TimeSpan.FromSeconds(0)) == 0)
            {
                check();
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
            check();
            dispatcherTimer.Stop();
            defaultValues();
        }

        private void comboBoxChooseTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            time = TimeSpan.FromMinutes(comboBoxChooseTime.SelectedIndex + 1); //Notice that ComboBoxItem indexing starts from zero
            testTime = time;
            labelTimer.Text = time.ToString("c"); //"c" format specifier tells ToString method to use the standard hh:mm:ss form
        }

        private void comboBoxChooseExcerpt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buttonStart.IsEnabled = true; //Start button becomes enabled if user chooses the text fragment
            switch (comboBoxChooseExcerpt.SelectedIndex)
            {
                case 0:
                    textBoxSource.Text = TextFragments.LOTR;
                    break;
                case 1:
                    textBoxSource.Text = TextFragments.HP;
                    break;
                case 2:
                    textBoxSource.Text = TextFragments.Nausea;
                    break;
                case 3:
                    textBoxSource.Text = TextFragments.Test;
                    break;
            }
        }

        private void textBoxInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxSource.Text.Equals(textBoxInput.Text))
            {
                easyCheck();
                dispatcherTimer.Stop();
                defaultValues();
            }
        }
        #endregion
    }
}
