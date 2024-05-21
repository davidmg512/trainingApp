using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace trainingApp
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ResourceLoader resourceLoader;

        public MainPage()
        {
            this.InitializeComponent();

            string currentLanguage = ApplicationLanguages.PrimaryLanguageOverride;
            foreach (ComboBoxItem item in LanguageSelector.Items)
            {
                if (item.Tag.ToString() == currentLanguage)
                {
                    LanguageSelector.SelectedItem = item;
                    break;
                }
            }
            this.resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

        }

        private void AddExercise_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string day = button.Tag.ToString();

            ExerciseCard newExerciseCard = new ExerciseCard();

            switch (day)
            {
                case "Lunes":
                    EjerciciosLunes.Items.Add(newExerciseCard);
                    break;
                case "Martes":
                    EjerciciosMartes.Items.Add(newExerciseCard);
                    break;
                case "Miercoles":
                    EjerciciosMiercoles.Items.Add(newExerciseCard);
                    break;
                case "Jueves":
                    EjerciciosJueves.Items.Add(newExerciseCard);
                    break;
                case "Viernes":
                    EjerciciosViernes.Items.Add(newExerciseCard);
                    break;
                case "Sabado":
                    EjerciciosSabado.Items.Add(newExerciseCard);
                    break;
                case "Domingo":
                    EjerciciosDomingo.Items.Add(newExerciseCard);
                    break;
            }
        }

        
        private void LanguageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedLanguage = selectedItem.Tag.ToString();
                ApplicationLanguages.PrimaryLanguageOverride = selectedLanguage;

                // Reiniciar la aplicación para aplicar el nuevo idioma

                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame?.Navigate(typeof(MainPage));
            }
        }

        private void ReloadPage()
        {
            // Recargar la página actual
            Frame.Navigate(this.GetType());
            Frame.BackStack.Clear(); // Limpiar el historial de navegación
        }
    }
}
