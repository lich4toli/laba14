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
using System.Windows.Shapes;

namespace UrsheevGlazki
{
    /// <summary>
    /// Логика взаимодействия для PriorityWindow.xaml
    /// </summary>
    public partial class PriorityWindow : Window
    {
        private List<Agent> Agents;
        private EyesPage eyesPage;
        int maxPriority = 0;
        public PriorityWindow(List<Agent> Agents, EyesPage eyesPage)
        {
            InitializeComponent();
            this.Agents = Agents;
            this.eyesPage = eyesPage;
            Init();
        }

        private void Init()
        {
            foreach (Agent agent in Agents)
            {
                if (agent.Priority > maxPriority)
                    maxPriority = agent.Priority;
            }
            PriorityTextBox.Text = maxPriority.ToString();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Agent agent in Agents)
            {
                agent.Priority = maxPriority;
            }
            UrsheevGlazkiEntities.GetContext().SaveChanges();
            MessageBox.Show("Информация сохранена!");
            eyesPage.UpdateServices();
            this.Close();
        }

        private void PriorityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            StringBuilder error = new StringBuilder();
            if (string.IsNullOrWhiteSpace(PriorityTextBox.Text))
                error.AppendLine("Укажите приоритет");
            else maxPriority = Convert.ToInt32(PriorityTextBox.Text);

            if (error.Length > 0)
            {
                MessageBox.Show(error.ToString());
                return;
            }

        }

        private void PriorityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
                e.Handled = true;
        }
    }
}
