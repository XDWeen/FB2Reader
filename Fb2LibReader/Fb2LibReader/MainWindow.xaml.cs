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


namespace Fb2LibReader
{
    public partial class MainWindow : Window
    {
        public BookInteraction Interaction = new BookInteraction();


        public MainWindow()
        {
            InitializeComponent();
        }
        
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Interaction.OpenBook();
            Interaction.BookToList();
        }


        private async void TextBlock_leftButton(object sender, RoutedEventArgs e)
        {
            if (Interaction.Book == null)
            {
                await Interaction.OpenBook();
                Interaction.BookToList();
            }



            foreach (var word in Interaction.ReadBook())
            {
                textBlock.Inlines.Add(word);
            }
            
               //textBlock.Text += Interaction.ReadBook(Interaction.Book, true);
            
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(textBlock.Inlines.Count().ToString());
        }
    }
}
