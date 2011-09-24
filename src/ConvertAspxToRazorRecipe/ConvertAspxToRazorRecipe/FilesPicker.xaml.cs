using System.Windows;

namespace ConvertAspxToRazorRecipe
{
    public partial class FilesPicker
    {
        public FilesPicker()
        {
            InitializeComponent();
        }

            
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            ((Window) Parent).DialogResult = true;
        }
    }
}
