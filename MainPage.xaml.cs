using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using trainingApp.Modelo;
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
        private DatabaseManager databaseManager;

        public MainPage()
        {
            this.InitializeComponent();
            this.databaseManager = new DatabaseManager();

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

            InitializeDatabase();
            //LoadRutinas();
            LoadData();
        }

        private async void InitializeDatabase()
        {
            await databaseManager.InitializeDatabaseAsync();
        }

        private async void LoadRutinas()
        {
            ObservableCollection<Rutina> rutinas = await databaseManager.RetrieveRutinasAsync();

            // Añadir las rutinas a los elementos visuales correspondientes
            foreach (Rutina rutina in rutinas)
            {
                switch (rutina.DiaDeSemana)
                {
                    case "Lunes":
                        EjerciciosLunes.ItemsSource = rutina.Ejercicios;
                        break;
                    case "Martes":
                        EjerciciosMartes.ItemsSource = rutina.Ejercicios;
                        break;
                    case "Miercoles":
                        EjerciciosMiercoles.ItemsSource = rutina.Ejercicios;
                        break;
                    case "Jueves":
                        EjerciciosJueves.ItemsSource = rutina.Ejercicios;
                        break;
                    case "Viernes":
                        EjerciciosViernes.ItemsSource = rutina.Ejercicios;
                        break;
                    case "Sabado":
                        EjerciciosSabado.ItemsSource = rutina.Ejercicios;
                        break;
                    case "Domingo":
                        EjerciciosDomingo.ItemsSource = rutina.Ejercicios;
                        break;
                }
            }
        }

        private void LoadData()
        {
            var rutinas = databaseManager.GetRutinas();
            foreach (var rutina in rutinas)
            {
                foreach (var ejercicio in rutina.Ejercicios)
                {
                    var newExerciseCard = new ExerciseCard
                    {
                        Nombre = ejercicio.Nombre,
                        Descripcion = ejercicio.Descripcion,
                        Repeticiones = ejercicio.Repeticiones,
                        Series = ejercicio.Series
                    };
                    switch (rutina.DiaDeSemana)
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
            }
        }

        private async void AddExercise_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string day = button.Tag.ToString();
            var newExerciseCard = new ExerciseCard();

            newExerciseCard.Accepted += (s, args) => SaveExerciseToDatabase(newExerciseCard, day);

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

        private async void SaveExerciseToDatabase(ExerciseCard exerciseCard, string day)
        {
            Ejercicio newExercise = new Ejercicio
            {
                Nombre = exerciseCard.ExerciseName.Text,
                Descripcion = exerciseCard.Description.Text,
                Repeticiones = int.Parse(exerciseCard.Repetitions.Text),
                Series = int.Parse(exerciseCard.Sets.Text)
            };

            // Insertar el ejercicio en la base de datos
            await databaseManager.InsertEjercicioAsync(newExercise);

            // Obtener la rutina correspondiente
            ObservableCollection<Rutina> rutinas = await databaseManager.RetrieveRutinasAsync();
            Rutina targetRutina = rutinas.FirstOrDefault(r => r.DiaDeSemana == day);

            if(targetRutina == null)
            {
                targetRutina = new Rutina { DiaDeSemana = day };
                // Insertar la nueva rutina en la base de datos
                await databaseManager.InsertRutinaAsync(targetRutina);
                // Añadir la nueva rutina a la colección local
                rutinas.Add(targetRutina);
            }

            targetRutina.Ejercicios.Add(newExercise);
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
