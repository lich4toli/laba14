using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace UrsheevGlazki
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Agent currentAgent = new Agent();
        private Regex regex = new Regex(@"\D");
        List<AgentType> AgentTypesDBList = UrsheevGlazkiEntities.GetContext().AgentType.ToList();
        public AddEditPage(Agent AgentInfo)
        {
            InitializeComponent();
            if (AgentInfo != null)
            {
                currentAgent = AgentInfo;
                ComboType.SelectedIndex = currentAgent.AgentTypeID - 1;
            }
            Init();
            DataContext = currentAgent;
        }

        public void Init()
        {
            salesHistoryBox.Items.Clear();
            foreach (ProductSale productSale in currentAgent.ProductSale)
            {
                salesHistoryBox.Items.Add($" {productSale.Product.Title} | {productSale.SaleDate.ToLongDateString()} | {productSale.ProductCount}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(currentAgent.Title))
                errors.AppendLine("Укажите наименование агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Address))
                errors.AppendLine("Укажите адрес агента");
            if (string.IsNullOrWhiteSpace(currentAgent.DirectorName))
                errors.AppendLine("Укажите ФИО директора");
            if (ComboType.SelectedItem == null)
                errors.AppendLine("Укажите тип агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Priority.ToString()))
                errors.AppendLine("Укажите приоритет агента");
            if (currentAgent.Priority <= 0)
                errors.AppendLine("Укажите положительный приоритет агента");
            if (string.IsNullOrWhiteSpace(currentAgent.INN))
                errors.AppendLine("Укажите ИНН агента");
            if (string.IsNullOrWhiteSpace(currentAgent.KPP))
                errors.AppendLine("Укажите КПП агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Phone))
                errors.AppendLine("Укажите телефон агента");

            else
            {
                string phoneNumber = regex.Replace(currentAgent.Phone, "");
                if (((phoneNumber[1] == '9' || phoneNumber[1] == '4' || phoneNumber[1] == '8') && phoneNumber.Length != 11) || (phoneNumber[1] == '3' && phoneNumber.Length != 12))
                    errors.AppendLine("Укажите правильно телефон агента");
            }
            if (string.IsNullOrWhiteSpace(currentAgent.Email))
                errors.AppendLine("Укажите почту агента");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            var currentType = (TextBlock)ComboType.SelectedValue;
            string currentTypeContent = currentType.Text;
            foreach (AgentType type in AgentTypesDBList)
            {
                if (type.Title.ToString() == currentTypeContent)
                {
                    currentAgent.AgentType = type;
                    currentAgent.AgentTypeID = type.ID;
                    break;
                }
            }

            if (currentAgent.ID == 0)
                UrsheevGlazkiEntities.GetContext().Agent.Add(currentAgent);

            try
            {
                UrsheevGlazkiEntities.GetContext().SaveChanges();
                MessageBox.Show("Дело сделано");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var currentProductSale = UrsheevGlazkiEntities.GetContext().ProductSale.ToList();
            currentProductSale = currentProductSale.Where(p => p.AgentID == currentAgent.ID).ToList();
            if (currentProductSale.Count != 0)
                MessageBox.Show("Невозможно удалить запись, так как существует реализация продукта");
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        UrsheevGlazkiEntities.GetContext().Agent.Remove(currentAgent);
                        UrsheevGlazkiEntities.GetContext().SaveChanges();
                        MessageBox.Show("Запись удалена");
                        Manager.MainFrame.GoBack();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void ChangePictureButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                currentAgent.Logo = myOpenFileDialog.FileName;
                LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }

        private void salesHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            SalesWindow salesWindow = new SalesWindow(currentAgent, this);

            salesWindow.Show();
        }
    }
}

