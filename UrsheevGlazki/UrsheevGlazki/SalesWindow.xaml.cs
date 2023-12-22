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
    /// Логика взаимодействия для SalesWindow.xaml
    /// </summary>
    public partial class SalesWindow : Window
    {
        private Agent currentAgent;
        private AddEditPage addEditPage;
        private List<Product> product;
        public SalesWindow(Agent currentAgent, AddEditPage addEditPage)
        {
            this.addEditPage = addEditPage;
            this.currentAgent = currentAgent;
            InitializeComponent();
            product = UrsheevGlazkiEntities.GetContext().Product.ToList();
            Init();
        }

        private void Init()
        {
            ComboBoxUpdate();
            SalesComboBox.Items.Clear();
            foreach (ProductSale productSale in currentAgent.ProductSale)
            {
                SalesComboBox.Items.Add($"{productSale.Product.Title} | {productSale.SaleDate.ToLongDateString()} | количество: {productSale.ProductCount}");
            }
            addEditPage.Init();
        }

        private void ComboBoxUpdate()
        {
            var currentProductsCB = product;
            currentProductsCB = currentProductsCB.Where(p => p.Title.ToLower().Contains(ProductNameTB.Text)).ToList();
            ProductNameCB.ItemsSource = currentProductsCB.Select(p => p.Title);
        }

        
        private void ListBoxUpdate()
        {
            var currentProductsCB = product;
            currentProductsCB = currentProductsCB.Where(p => p.Title.ToLower().Contains(ProductNameTB.Text)).ToList();
            ProductNameCB.ItemsSource = currentProductsCB.Select(p => p.Title);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (ProductNameCB.SelectedItem == null)
            {
                stringBuilder.AppendLine("Укажите тип продукции!");
            }
            if (ProductCountTB.Text.Length < 1)
            {
                stringBuilder.AppendLine("Укажите количество продукции!");
            }

            if (stringBuilder.Length > 0)
            {
                MessageBox.Show(stringBuilder.ToString());
            }
            else
            {
                try
                {
                    Product currentProduct = product[ProductNameCB.SelectedIndex];
                    ProductSale productSale = new ProductSale();
                    productSale.AgentID = currentAgent.ID;
                    productSale.ProductID = currentProduct.ID;
                    productSale.SaleDate = DateTime.Now;
                    productSale.ProductCount = Convert.ToInt32(ProductCountTB.Text);
                    productSale.Agent = currentAgent;
                    productSale.Product = currentProduct;
                    UrsheevGlazkiEntities.GetContext().ProductSale.Add(productSale);
                    UrsheevGlazkiEntities.GetContext().SaveChanges();
                    MessageBox.Show("Запись добавлена!");
                    Init();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SalesComboBox.SelectedItem == null)
            {
                MessageBox.Show("Необходимо выбрать удаляемую запись!");
            }
            else
            {
                try
                {
                    ProductSale deletingProductSale = currentAgent.ProductSale.ToList()[SalesComboBox.SelectedIndex];
                    UrsheevGlazkiEntities.GetContext().ProductSale.Remove(deletingProductSale);
                    UrsheevGlazkiEntities.GetContext().SaveChanges();
                    MessageBox.Show("Запись удалена!");
                    Init();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void ProductNameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            ComboBoxUpdate();
            ListBoxUpdate();
        }
    }
}
